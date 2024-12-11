using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Items;
using Eco.Gameplay.Skills;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Occupancy;
using Eco.Gameplay.Systems.Exhaustion;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Items;
using static Eco.Gameplay.Components.PartsComponent;
using Eco.Mods.TechTree;
using Eco.RM.ElectricTakeover.Components;
using Eco.Gameplay.Items.Recipes;
using Eco.RM.Framework.Config;
using Eco.RM.ElectricTakeover.Items;
using Eco.RM.ElectricTakeover.Utility;
using Eco.Shared.Utils;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Battery Powered Excavator")]
[LocDescription("Like a Skid Steer but more versatile. Great for high slope excavation.")]
[IconGroup("World Object Minimap")]
[Weight(30000)]
[RequireComponent(typeof(VehicleToolComponent))]
[Tag("Excavation")]
[Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
public partial class BatteryPoweredExcavatorItem : WorldObjectItem<BatteryPoweredExcavatorObject>, IPersistentData
{
    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
}

public class BatteryPoweredExcavatorRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(BatteryPoweredExcavatorRecipe).Name,
        Assembly = typeof(BatteryPoweredExcavatorRecipe).AssemblyQualifiedName,
        HiddenName = "Battery Powered Excavator",
        LocalizableName = Localizer.DoStr("Battery Powered Excavator"),
        IngredientList =
        [
            new RMIngredient(nameof(ExcavatorItem), false, 1, true),
            new RMIngredient(nameof(AdvancedElectricUpgradeKitItem), false, 1, true),

        ],
        ProductList =
        [
            new RMCraftable(nameof(BatteryPoweredExcavatorItem), 1),
        ],
        BaseExperienceOnCraft = 0,
        BaseLabor = 1,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(AssemblyTableItem),
    };

    static BatteryPoweredExcavatorRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public BatteryPoweredExcavatorRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}

[Serialized]
[RequireComponent(typeof(StandaloneAuthComponent))]
[RequireComponent(typeof(PaintableComponent))]
[RequireComponent(typeof(BatteryConsumptionComponent))]
[RequireComponent(typeof(MovableLinkComponent))]
[RequireComponent(typeof(VehicleComponent))]
[RequireComponent(typeof(CustomTextComponent))]
[RequireComponent(typeof(VehicleToolComponent))]
[RequireComponent(typeof(MinimapComponent))]           
[RequireComponent(typeof(PartsComponent))]
[RepairRequiresSkill(typeof(IndustrySkill), 2)]
[ExhaustableUnlessOverridenVehicle]
[Ecopedia("Crafted Objects", "Vehicles", subPageName: "Battery Powered Excavator Item")]
public partial class BatteryPoweredExcavatorObject : PhysicsWorldObject, IRepresentsItem
{
    static BatteryPoweredExcavatorObject()
    {
        AddOccupancy<BatteryPoweredExcavatorObject>([]);
    }
    public override TableTextureMode TableTexture => TableTextureMode.Metal;
    public override bool PlacesBlocks            => false;
    public override LocString DisplayName { get { return Localizer.DoStr("Battery Powered Excavator"); } }
    public Type RepresentedItemType { get { return typeof(BatteryPoweredExcavatorItem); } }
    private BatteryPoweredExcavatorObject() { }
    protected override void Initialize()
    {
        base.Initialize();         
        GetComponent<CustomTextComponent>().Initialize(200);
        GetComponent<BatteryConsumptionComponent>().Initialize(1, 275);
        GetComponent<VehicleComponent>().HumanPowered(2);
        GetOrCreateComponent<VehicleToolComponent>().Initialize(7, 3500000, 0, 0, 0, true, BatteryPoweredVehicleUtilities.GetInventoryRestriction(this));
        GetComponent<MinimapComponent>().InitAsMovable();
        GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
        GetComponent<VehicleComponent>().Initialize(ElectricTakeoverConfig.Obj.GetBatteryPoweredVehicleSpeed(14), 1.5f,1);
        GetComponent<VehicleComponent>().FailDriveMsg = Localizer.Do($"You are too hungry to drive {DisplayName}!");
        {
            GetComponent<PartsComponent>().Config(() => LocString.Empty,
            [
                new() { TypeName = nameof(AdvancedCombustionEngineItem), Quantity = 1},
                new() { TypeName = nameof(RubberWheelItem), Quantity = 2},
                new() { TypeName = nameof(LubricantItem), Quantity = 2},
            ]);
        }
    }
}
