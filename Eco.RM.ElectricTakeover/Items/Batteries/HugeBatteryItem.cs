using Eco.Gameplay.Components;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Skills;
using Eco.Mods.TechTree;
using Eco.RM.Framework.Config;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items;

[Serialized]
[LocDisplayName("Huge Battery")]
[LocDescription("A device used to store a huge ammount energy for later use.")]
public class HugeBatteryItem : BatteryItem, IConfiguredObject
{
    public override Type BatteryType => typeof(HugeBatteryItem);
    public static readonly Dictionary<string, object> DefaultConfig = new()
    {
        { nameof(ChargeRate), 3000 },
        { nameof(DischargeRate), 2000f },
        { nameof(MaxCharge), 8000f }
    };
    static HugeBatteryItem() { RMObjectResolver.AddDefaults(nameof(HugeBatteryItem), DefaultConfig); }
    public HugeBatteryItem()
    {
        ChargeRate = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(HugeBatteryItem), nameof(ChargeRate)));
        DischargeRate = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(HugeBatteryItem), nameof(DischargeRate)));
        MaxCharge = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(HugeBatteryItem), nameof(MaxCharge)));
    }
}

[RequiresSkill(typeof(ElectronicsSkill), 1)]
public class HugeBatteryRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(HugeBatteryRecipe).Name,
        Assembly = typeof(HugeBatteryRecipe).AssemblyQualifiedName,
        HiddenName = "Huge Battery",
        LocalizableName = Localizer.DoStr("Huge Battery"),
        IngredientList =
        [
            new RMIngredient(nameof(LargeBatteryItem), false, 3, true),
            new RMIngredient(nameof(FlatSteelItem), false, 30),
            new RMIngredient(nameof(GoldFlakesItem), false, 30),
            new RMIngredient(nameof(AdvancedCircuitItem), false, 5),
            new RMIngredient(nameof(BasicCircuitItem), false, 15)
        ],
        ProductList =
        [
            new RMCraftable(nameof(HugeBatteryItem), 1),
        ],
        BaseExperienceOnCraft = 10,
        BaseLabor = 300,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(ElectronicsAssemblyItem),
        RequiredSkillType = typeof(ElectronicsSkill),
        RequiredSkillLevel = 1,
        IngredientImprovementTalents = typeof(ElectronicsLavishResourcesTalent),
        SpeedImprovementTalents = [typeof(ElectronicsParallelSpeedTalent), typeof(ElectronicsFocusedSpeedTalent)],
    };

    static HugeBatteryRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public HugeBatteryRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}
