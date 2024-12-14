using Eco.Gameplay.Items;
using Eco.Shared.Utils;
using Eco.Gameplay.DynamicValues;
using Eco.Shared.Localization;
using Eco.Core.Utils;
using Eco.Gameplay.Items.Recipes;
using Eco.RM.Framework.Models;
using Eco.RM.Framework.Plugins;

/**
 * THIS CODE IS NOT MINE!!!
 * 
 * ITS A MODIFIED VERSION OF https://github.com/TheKye/EM-Framework/blob/10.0-Staging/Eco.EM.Framework/Resolvers/Datasets/EMRecipeResolver.cs
 */

namespace Eco.RM.Framework.Resolvers;

public interface IConfigurableRecipe { }

public class RMRecipeResolver : AutoSingleton<RMRecipeResolver>
{
    public Dictionary<string, RecipeDefaultModel> LoadedDefaultRecipes { get; private set; } = [];
    public Dictionary<string, RecipeModel> LoadedConfigRecipes { get; private set; } = [];

    public static void AddDefaults(RecipeDefaultModel defaults)
    {
        Obj.LoadedDefaultRecipes.AddUnique(defaults.ModelType, defaults);
    }
    public List<Recipe> ResolveRecipe(IConfigurableRecipe recipe) => GetRecipe(recipe);
    public IDynamicValue ResolveLabor(IConfigurableRecipe recipe) => GetLaborValue(recipe);
    public IDynamicValue ResolveCraftMinutes(IConfigurableRecipe recipe) => GetCraftTime(recipe);
    public Type ResolveStation(IConfigurableRecipe recipe) => GetConfigStation(recipe);

    public float ResolveExperience(IConfigurableRecipe recipe) => GetExperience(recipe);

    public LocString ResolveRecipeName(IConfigurableRecipe recipe) => GetRecipeName(recipe);

    private LocString GetRecipeName(IConfigurableRecipe recipe)
    {
        var dModel = LoadedDefaultRecipes[recipe.GetType().Name];
        // check if config override
        var loaded = LoadedConfigRecipes.TryGetValue(recipe.GetType().Name, out RecipeModel model);
        if (loaded && model.LocalizableName != dModel.LocalizableName)
            return Localizer.DoStr(model.LocalizableName);

        if (loaded)
            return Localizer.DoStr(model.LocalizableName);

        // return default
        return Localizer.DoStr(dModel.LocalizableName);
    }

    private float GetExperience(IConfigurableRecipe recipe)
    {
        var dModel = LoadedDefaultRecipes[recipe.GetType().Name];
        // check if config override
        var loaded = LoadedConfigRecipes.TryGetValue(recipe.GetType().Name, out RecipeModel model);
        if (loaded && model.BaseExperienceOnCraft != dModel.BaseExperienceOnCraft)
            return model.BaseExperienceOnCraft;

        if (loaded)
            return model.BaseExperienceOnCraft;

        // return default
        return dModel.BaseExperienceOnCraft;
    }

    #region Decison Methods
    // checks to decide which version of the recipe should be loaded in
    private List<Recipe> GetRecipe(IConfigurableRecipe recipe)
    {
        var dModel = LoadedDefaultRecipes[recipe.GetType().Name];
        var dRecipe = CreateDefaultRecipeFromModel(dModel);

        // check if config override
        var loaded = LoadedConfigRecipes.TryGetValue(recipe.GetType().Name, out RecipeModel model);
        if (loaded && !RecipeModel.Compare(dModel, model) || loaded && !model.EnableRecipe)
        {
            return [CreateRecipeFromModel(model, dModel)];
        }

        // return default
        return [dRecipe];
    }

    private static Recipe CreateRecipeFromModel(RecipeModel model, RecipeDefaultModel def)
    {
        var recipe = new Recipe();
        recipe.Init(
        def.HiddenName,
        Localizer.DoStr(model.LocalizableName),
        CreateIngredientList(model, def.RequiredSkillType, def.IngredientImprovementTalents),
        CreateProductList(model));
        return recipe;
    }

    private static Recipe CreateDefaultRecipeFromModel(RecipeDefaultModel def)
    {
        var recipe = new Recipe();
        recipe.Init(
        def.HiddenName,
        Localizer.DoStr(def.LocalizableName),
        CreateIngredientList(def, def.RequiredSkillType, def.IngredientImprovementTalents),
        CreateProductList(def));
        return recipe;
    }

    private static List<IngredientElement> CreateIngredientList(RecipeModel model, Type skill, Type talent)
    {
        List<IngredientElement> ingredients = [];
        foreach (var value in model.IngredientList)
        {
            try
            {
                if (value.Static)
                {
                    if (!value.Tag)
                        ingredients.Add(new IngredientElement(Item.Get(value.Item), value.Amount, true));
                    else
                        ingredients.Add(new IngredientElement(value.Item, value.Amount, true));
                }
                else if (skill != null && talent != null)
                {
                    if (!value.Tag)
                        ingredients.Add(new IngredientElement(Item.Get(value.Item), value.Amount, skill, talent));
                    else
                        ingredients.Add(new IngredientElement(value.Item, value.Amount, skill, talent));
                }
                else if (skill != null && talent == null)
                {
                    if (!value.Tag)
                        ingredients.Add(new IngredientElement(Item.Get(value.Item), value.Amount, skill));
                    else
                        ingredients.Add(new IngredientElement(value.Item, value.Amount, skill));
                }
                else
                {
                    if (!value.Tag)
                        ingredients.Add(new IngredientElement(Item.Get(value.Item), value.Amount, false));
                    else
                        ingredients.Add(new IngredientElement(value.Item, value.Amount, false));

                }
            }
            catch
            {
                throw new Exception($"[RM Framework]: {value.Item} was an invalid ingredient lookup for an Item or Tag. Please review the recipe config for {model.ModelType}");
            }
        }
        return ingredients;
    }

    private static List<CraftingElement> CreateProductList(RecipeModel model)
    {
        List<CraftingElement> products = [];
        foreach (var value in model.ProductList)
        {
            try
            {
                Item i = Item.Get(value.Item);
                Type ce = typeof(CraftingElement<>);
                var generic = ce.MakeGenericType(i.GetType());
                CraftingElement element = Activator.CreateInstance(generic, value.Amount) as CraftingElement;
                products.Add(element);
            }
            catch
            {
                throw new Exception($"[EM Framework]: {value.Item} was an invalid product lookup for an Item or Tag. Please review the recipe config for {model.ModelType}");
            }
        }
        return products;
    }

    private IDynamicValue GetCraftTime(IConfigurableRecipe recipe)
    {
        var dModel = LoadedDefaultRecipes[recipe.GetType().Name];
        // check if config override
        var loaded = LoadedConfigRecipes.TryGetValue(recipe.GetType().Name, out RecipeModel model);
        if (loaded && (model.BaseCraftTime != dModel.BaseCraftTime || model.CraftTimeIsStatic != dModel.CraftTimeIsStatic))
            return CreateTimeValue(model, dModel);

        if (loaded)
            return CreateTimeValue(model, dModel);

        // return default
        return CreateDefaultTimeValue(dModel);
    }

    private Type GetConfigStation(IConfigurableRecipe recipe)
    {
        try
        {
            var dModel = LoadedDefaultRecipes[recipe.GetType().Name];

            // check if config override
            var loaded = LoadedConfigRecipes.TryGetValue(recipe.GetType().Name, out RecipeModel model);

            if (loaded && !model.EnableRecipe)
                return string.Empty.GetType();

            if (loaded && model.CraftingStation != dModel.CraftingStation)
                return Item.GetCreatedObj(Item.GetType(model.CraftingStation));

            if (loaded && !model.EnableRecipe)
                return string.Empty.GetType();

            if (loaded)
                return Item.GetCreatedObj(Item.GetType(model.CraftingStation));

            // return default
            return Item.GetCreatedObj(Item.GetType(dModel.CraftingStation));
        }
        catch
        {
            throw new Exception($"[RM Framework]: {recipe.GetType().Name} had an invalid crafting station lookup. Check and make sure the CREATING ITEM and not OBJECT is referenced.");
        }

    }

    private IDynamicValue GetLaborValue(IConfigurableRecipe recipe)
    {
        var dModel = LoadedDefaultRecipes[recipe.GetType().Name];

        // check if config override
        var loaded = LoadedConfigRecipes.TryGetValue(recipe.GetType().Name, out RecipeModel model);
        if (loaded && (model.BaseLabor != dModel.BaseLabor))
            return CreateLaborValue(model, dModel);

        if (loaded)
            return CreateLaborValue(model, dModel);

        // return default
        return CreateDefaultLaborValue(dModel);
    }
    #endregion

    #region Value Creators
    private static IDynamicValue CreateLaborValue(RecipeModel model, RecipeDefaultModel def)
    {
        return CreateLaborInCaloriesValue(model.BaseLabor);
    }

    private static IDynamicValue CreateDefaultLaborValue(RecipeDefaultModel def)
    {
        return CreateLaborInCaloriesValue(def.BaseLabor);
    }

    private static IDynamicValue CreateTimeValue(RecipeModel model, RecipeDefaultModel def)
    {
        Type ModelType = Type.GetType(model.Assembly);
        bool isStatic = model.CraftTimeIsStatic || def.RequiredSkillType == null;
        return isStatic
            ? CreateCraftTimeValue(model.BaseCraftTime)
            : CreateCraftTimeValue(ModelType, model.BaseCraftTime, def.RequiredSkillType, def.SpeedImprovementTalents);
    }

    private static IDynamicValue CreateDefaultTimeValue(RecipeDefaultModel def)
    {
        Type ModelType = Type.GetType(def.Assembly);
        bool isStatic = def.CraftTimeIsStatic || def.RequiredSkillType == null;
        return isStatic
            ? CreateCraftTimeValue(def.BaseCraftTime)
            : CreateCraftTimeValue(ModelType, def.BaseCraftTime, def.RequiredSkillType, def.SpeedImprovementTalents);
    }


    // SLG code for creating IDynamicValues of RecipeFamilies
    private static IDynamicValue CreateCraftTimeValue(float start) => new ConstantValue(start * RecipeManager.CraftTimeModifier);
    private static IDynamicValue CreateCraftTimeValue(Type beneficiary, float start, Type skillType, params Type[] talents)
    {
        var smv = new ModuleModifiedValue(start * RecipeManager.CraftTimeModifier, skillType, DynamicValueType.Speed);
        return talents != null
            ? new MultiDynamicValue(MultiDynamicOps.Multiply, talents.Select(x => new TalentModifiedValue(beneficiary, x) as IDynamicValue).Concat(new[] { smv }).ToArray())
            : smv;
    }

    private static IDynamicValue CreateLaborInCaloriesValue(float start) => new ConstantValue(start);
    #endregion

    // Load
    public void Initialize()
    {
        LoadConfigOverrides();
    }

    // Load overrides from config changes.
    private void LoadConfigOverrides()
    {
        SerializedSynchronizedCollection<RecipeModel> newModels = [];
        var previousModels = RMRecipeConfigPlugin.Config.Recipes;

        foreach (var type in typeof(IConfigurableRecipe).ConcreteTypes())
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);

            var loadtypes = LoadedDefaultRecipes.Values.ToList();
            foreach (var dModel in loadtypes)
            {
                var m = previousModels.FirstOrDefault(x => x.ModelType == dModel.ModelType);
                if (m != null && !m.ModelType.EqualsCaseInsensitive(dModel.ModelType))
                {
                    if (!newModels.Contains(m))
                    {
                        newModels.Add(m);
                    }
                }
                else
                {
                    if (!newModels.Contains(dModel))
                        newModels.Add(dModel);
                }
            }
        }
        newModels.OrderBy(x => x.ModelType);
        RMRecipeConfigPlugin.Config.Recipes = newModels;

        foreach (var model in RMRecipeConfigPlugin.Config.Recipes)
        {
            if (!LoadedConfigRecipes.ContainsKey(model.ModelType))
            {
                LoadedConfigRecipes.TryAdd(model.ModelType, model);
            }
        }
    }
}