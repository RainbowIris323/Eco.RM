using Eco.Gameplay.Components;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Skills;
using Eco.Mods.TechTree;
using Eco.RM.Framework.Config;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items;

[Serialized]
[LocDisplayName("Medium Battery")]
[LocDescription("A device used to store a sufficent ammount energy for later use.")]
public class MediumBatteryItem : BatteryItem, IConfiguredObject
{
    public override Type BatteryType => typeof(MediumBatteryItem);
    static readonly Dictionary<string, object> DefaultConfig = new()
    {
        { nameof(ChargeRate), 750f },
        { nameof(DischargeRate), 500f },
        { nameof(MaxCharge), 1500f }
    };
    static MediumBatteryItem() { RMObjectResolver.AddDefaults(nameof(MediumBatteryItem), DefaultConfig); }
    public MediumBatteryItem()
    {
        ChargeRate = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(MediumBatteryItem), nameof(ChargeRate)));
        DischargeRate = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(MediumBatteryItem), nameof(DischargeRate)));
        MaxCharge = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(MediumBatteryItem), nameof(MaxCharge)));
    }
}

[RequiresSkill(typeof(MechanicsSkill), 1)]
public class MediumBatteryRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(MediumBatteryRecipe).Name,
        Assembly = typeof(MediumBatteryRecipe).AssemblyQualifiedName,
        HiddenName = "Medium Battery",
        LocalizableName = Localizer.DoStr("Medium Battery"),
        IngredientList =
        [
            new RMIngredient(nameof(SmallBatteryItem), false, 3, true),
            new RMIngredient(nameof(IronPlateItem), false, 30),
            new RMIngredient(nameof(CopperPlateItem), false, 30),
            new RMIngredient(nameof(CrushedCoalItem), false, 30)
        ],
        ProductList =
        [
            new RMCraftable(nameof(MediumBatteryItem), 1),
        ],
        BaseExperienceOnCraft = 10,
        BaseLabor = 300,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(MachinistTableItem),
        RequiredSkillType = typeof(MechanicsSkill),
        RequiredSkillLevel = 1,
        IngredientImprovementTalents = typeof(MechanicsLavishResourcesTalent),
        SpeedImprovementTalents = [typeof(MechanicsParallelSpeedTalent), typeof(MechanicsFocusedSpeedTalent)],
    };

    static MediumBatteryRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public MediumBatteryRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}


