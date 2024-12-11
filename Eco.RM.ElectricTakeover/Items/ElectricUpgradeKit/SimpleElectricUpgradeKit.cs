using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Skills;
using Eco.Mods.TechTree;
using Eco.RM.Framework.Config;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items;

[Serialized]
[LocDescription("A kit used to turn simple non-battery powered machines into battery powered ones.")]
[LocDisplayName("Simple Electric Upgrade Kit")]
[MaxStackSize(5)]
public class SimpleElectricUpgradeKitItem : Item
{
}


[RequiresSkill(typeof(BasicEngineeringSkill), 3)]
public class SimpleElectricUpgradeKitRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(SimpleElectricUpgradeKitRecipe).Name,
        Assembly = typeof(SimpleElectricUpgradeKitRecipe).AssemblyQualifiedName,
        HiddenName = "Simple Electric Upgrade Kit",
        LocalizableName = Localizer.DoStr("Simple Electric Upgrade Kit"),
        IngredientList =
        [
            new RMIngredient(nameof(SmallBatteryItem), false, 3, true),
            new RMIngredient(nameof(IronBarItem), false, 30),
            new RMIngredient(nameof(CopperBarItem), false, 30),
            new RMIngredient(nameof(GoldBarItem), false, 15),
            new RMIngredient(nameof(HewnLogItem), false, 15),

        ],
        ProductList =
        [
            new RMCraftable(nameof(SimpleElectricUpgradeKitItem), 1),
        ],
        BaseExperienceOnCraft = 10,
        BaseLabor = 300,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(WainwrightTableItem),
        RequiredSkillType = typeof(BasicEngineeringSkill),
        RequiredSkillLevel = 3,
        IngredientImprovementTalents = typeof(BasicEngineeringLavishResourcesTalent),
        SpeedImprovementTalents = [typeof(BasicEngineeringParallelSpeedTalent), typeof(BasicEngineeringFocusedSpeedTalent)],
    };

    static SimpleElectricUpgradeKitRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public SimpleElectricUpgradeKitRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}
