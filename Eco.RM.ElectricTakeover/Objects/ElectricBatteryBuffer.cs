using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Skills;
using Eco.Mods.TechTree;
using Eco.RM.ElectricTakeover.Components;
using Eco.RM.Framework.Config;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Electric Battery Buffer")]
[RequireComponent(typeof(BatteryBufferComponent))]
public partial class ElectricBatteryBufferObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(ElectricBatteryBufferItem);
    static ElectricBatteryBufferObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryBufferComponent>().Initialize(4, 20, false, true, true);
    }
}

[Serialized]
[LocDisplayName("Electric Battery Buffer"), LocDescription("Allows the batteries within to contribute to a connected electric power grid.")]
public partial class ElectricBatteryBufferItem : WorldObjectItem<ElectricBatteryBufferObject> { }



[RequiresSkill(typeof(BasicEngineeringSkill), 1)]
public class ElectricBatteryBufferRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(ElectricBatteryBufferRecipe).Name,
        Assembly = typeof(ElectricBatteryBufferRecipe).AssemblyQualifiedName,
        HiddenName = "Electric Battery Buffer",
        LocalizableName = Localizer.DoStr("Electric Battery Buffer"),
        IngredientList =
        [
            new RMIngredient(nameof(IronBarItem), false, 30),
            new RMIngredient(nameof(CopperBarItem), false, 30),
        ],
        ProductList =
        [
            new RMCraftable(nameof(ElectricBatteryBufferItem), 1),
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

    static ElectricBatteryBufferRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public ElectricBatteryBufferRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}
