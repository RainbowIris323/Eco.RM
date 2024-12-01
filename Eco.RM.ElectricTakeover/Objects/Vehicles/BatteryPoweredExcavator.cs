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
        WorldObject.AddOccupancy<BatteryPoweredExcavatorObject>(new List<BlockOccupancy>(0));
    }
    public override TableTextureMode TableTexture => TableTextureMode.Metal;
    public override bool PlacesBlocks            => false;
    public override LocString DisplayName { get { return Localizer.DoStr("Battery Powered Excavator"); } }
    public Type RepresentedItemType { get { return typeof(BatteryPoweredExcavatorItem); } }
    private BatteryPoweredExcavatorObject() { }
    protected override void Initialize()
    {
        base.Initialize();         
        this.GetComponent<CustomTextComponent>().Initialize(200);
        this.GetComponent<BatteryConsumptionComponent>().Initialize(1, 275);
        this.GetComponent<FuelConsumptionComponent>().Initialize(275);
        this.GetComponent<VehicleComponent>().HumanPowered(2);
        this.GetComponent<VehicleToolComponent>().Initialize(7, 3500000,
        100, 200, 0, true, VehicleUtilities.GetInventoryRestriction(this));
        this.GetComponent<MinimapComponent>().InitAsMovable();
        this.GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
        this.GetComponent<VehicleComponent>().Initialize(14, 1.5f,1);
        this.GetComponent<VehicleComponent>().FailDriveMsg = Localizer.Do($"You are too hungry to drive {this.DisplayName}!");
        {
            this.GetComponent<PartsComponent>().Config(() => LocString.Empty, new PartInfo[]
            {
                new() { TypeName = nameof(AdvancedCombustionEngineItem), Quantity = 1},
                new() { TypeName = nameof(RubberWheelItem), Quantity = 2},
                new() { TypeName = nameof(LubricantItem), Quantity = 2},
            });
        }
    }
}
