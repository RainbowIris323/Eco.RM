using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Skills;
using Eco.Mods.TechTree;
using Eco.RM.ElectricTakeover.Components;
using Eco.RM.Framework.Resolvers;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.RM.Framework.Models;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Mechanical Battery Charger")]
[RequireComponent(typeof(BatteryBufferComponent))]
public partial class MechanicalBatteryChargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(MechanicalBatteryChargerItem);
    static MechanicalBatteryChargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryBufferComponent>().Initialize(2, 20, true, true, false);
    }
}

[Serialized]
[LocDisplayName("Mechanical Battery Charger"), LocDescription("Allows the batteries within to charge from a connected mechanical power grid.")]
public partial class MechanicalBatteryChargerItem : WorldObjectItem<MechanicalBatteryChargerObject>
{
}


[RequiresSkill(typeof(BasicEngineeringSkill), 1)]
public class MechanicalBatteryChargerRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(MechanicalBatteryChargerRecipe).Name,
        Assembly = typeof(MechanicalBatteryChargerRecipe).AssemblyQualifiedName,
        HiddenName = "Mechanical Battery Charger",
        LocalizableName = Localizer.DoStr("Mechanical Battery Charger"),
        IngredientList =
        [
            new RMIngredient(nameof(IronBarItem), false, 30),
            new RMIngredient(nameof(CopperBarItem), false, 30),
        ],
        ProductList =
        [
            new RMCraftable(nameof(MechanicalBatteryChargerItem), 1),
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

    static MechanicalBatteryChargerRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public MechanicalBatteryChargerRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}
