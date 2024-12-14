using Eco.Gameplay.Components;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Skills;
using Eco.Mods.TechTree;
using Eco.RM.Framework.Models;
using Eco.RM.Framework.Resolvers;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items;

[Serialized]
[LocDisplayName("Small Battery")]
[LocDescription("A device used to store a small ammount energy for later use.")]
public class SmallBatteryItem : BatteryItem, IConfiguredObject
{
    public override Type BatteryType => typeof(SmallBatteryItem);
    static readonly Dictionary<string, object> DefaultConfig = new()
    {
        { nameof(ChargeRate), 150f },
        { nameof(DischargeRate), 100f },
        { nameof(MaxCharge), 300f }
    };
    static SmallBatteryItem() { RMObjectResolver.AddDefaults(nameof(SmallBatteryItem), DefaultConfig); }
    public SmallBatteryItem()
    {
        ChargeRate = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(SmallBatteryItem), nameof(ChargeRate)));
        DischargeRate = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(SmallBatteryItem), nameof(DischargeRate)));
        MaxCharge = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(SmallBatteryItem), nameof(MaxCharge)));
    }
}

[RequiresSkill(typeof(BasicEngineeringSkill), 1)]
public class SmallBatteryRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(SmallBatteryRecipe).Name,
        Assembly = typeof(SmallBatteryRecipe).AssemblyQualifiedName,
        HiddenName = "Small Battery",
        LocalizableName = Localizer.DoStr("Small Battery"),
        IngredientList =
        [
            new RMIngredient(nameof(IronBarItem), false, 30),
            new RMIngredient(nameof(CopperBarItem), false, 30),
            new RMIngredient(nameof(CrushedCoalItem), false, 15)
        ],
        ProductList =
        [
            new RMCraftable(nameof(SmallBatteryItem), 1),
        ],
        BaseExperienceOnCraft = 10,
        BaseLabor = 300,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(WainwrightTableItem),
        RequiredSkillType = typeof(BasicEngineeringSkill),
        RequiredSkillLevel = 1,
        IngredientImprovementTalents = typeof(BasicEngineeringLavishResourcesTalent),
        SpeedImprovementTalents = [typeof(BasicEngineeringParallelSpeedTalent), typeof(BasicEngineeringFocusedSpeedTalent)],
    };

    static SmallBatteryRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public SmallBatteryRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}
