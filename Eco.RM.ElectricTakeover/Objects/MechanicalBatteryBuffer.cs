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
[LocDisplayName("Mechanical Battery Buffer")]
[RequireComponent(typeof(BatteryBufferComponent))]
public partial class MechanicalBatteryBufferObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(MechanicalBatteryBufferItem);
    static MechanicalBatteryBufferObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryBufferComponent>().Initialize(4, 20, true, true, true);
    }
}

[Serialized]
[LocDisplayName("Mechanical Battery Buffer"), LocDescription("Allows the batteries within to contribute to a connected mechanical power grid.")]
public partial class MechanicalBatteryBufferItem : WorldObjectItem<MechanicalBatteryBufferObject>
{
}

[RequiresSkill(typeof(BasicEngineeringSkill), 1)]
public class MechanicalBatteryBufferRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(MechanicalBatteryBufferRecipe).Name,
        Assembly = typeof(ElectricBatteryBufferRecipe).AssemblyQualifiedName,
        HiddenName = "Mechanical Battery Buffer",
        LocalizableName = Localizer.DoStr("Mechanical Battery Buffer"),
        IngredientList =
        [
            new RMIngredient(nameof(IronBarItem), false, 30),
            new RMIngredient(nameof(CopperBarItem), false, 30),
        ],
        ProductList =
        [
            new RMCraftable(nameof(MechanicalBatteryBufferItem), 1),
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

    static MechanicalBatteryBufferRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public MechanicalBatteryBufferRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}
