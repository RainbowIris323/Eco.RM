using Eco.Gameplay.Components;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Skills;
using Eco.Mods.TechTree;
using Eco.RM.Framework.Config;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items;

[Serialized]
[LocDisplayName("Large Battery")]
[LocDescription("A device used to store a large ammount energy for later use.")]
public class LargeBatteryItem : BatteryItem, IConfiguredObject
{
    public override Type BatteryType => typeof(LargeBatteryItem);
    static readonly Dictionary<string, object> DefaultConfig = new()
    {
        { nameof(ChargeRate), 1500f },
        { nameof(DischargeRate), 1000f },
        { nameof(MaxCharge), 4000f }
    };
    static LargeBatteryItem() { RMObjectResolver.AddDefaults(nameof(LargeBatteryItem), DefaultConfig); }
    public LargeBatteryItem()
    {
        ChargeRate = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(LargeBatteryItem), nameof(ChargeRate)));
        DischargeRate = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(LargeBatteryItem), nameof(DischargeRate)));
        MaxCharge = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(LargeBatteryItem), nameof(MaxCharge)));
    }
}

[RequiresSkill(typeof(IndustrySkill), 1)]
public class LargeBatteryRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(LargeBatteryRecipe).Name,
        Assembly = typeof(LargeBatteryRecipe).AssemblyQualifiedName,
        HiddenName = "Large Battery",
        LocalizableName = Localizer.DoStr("Large Battery"),
        IngredientList =
        [
            new RMIngredient(nameof(MediumBatteryItem), false, 3, true),
            new RMIngredient(nameof(SteelPlateItem), false, 30),
            new RMIngredient(nameof(GoldWiringItem), false, 15),
            new RMIngredient(nameof(CopperPlateItem), false, 30),
            new RMIngredient(nameof(CrushedCoalItem), false, 60)
        ],
        ProductList =
        [
            new RMCraftable(nameof(LargeBatteryItem), 1),
        ],
        BaseExperienceOnCraft = 10,
        BaseLabor = 300,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(MachinistTableItem),
        RequiredSkillType = typeof(IndustrySkill),
        RequiredSkillLevel = 1,
        IngredientImprovementTalents = typeof(IndustryLavishResourcesTalent),
        SpeedImprovementTalents = [typeof(IndustryParallelSpeedTalent), typeof(IndustryFocusedSpeedTalent)],
    };

    static LargeBatteryRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public LargeBatteryRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}
