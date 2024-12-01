using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Components.Storage;
using Eco.Gameplay.Items;
using Eco.Gameplay.Skills;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Occupancy;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Items;
using static Eco.Gameplay.Components.PartsComponent;
using Eco.Mods.TechTree;
using Eco.RM.ElectricTakeover.Components;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Battery Powered Industrial Barge")]
[LocDescription("")]
[IconGroup("World Object Minimap")]
[Weight(30000)]
[WaterPlaceable]
[Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
public partial class BatteryPoweredIndustrialBargeItem : WorldObjectItem<BatteryPoweredIndustrialBargeObject>, IPersistentData
{
    public float InteractDistance => DefaultInteractDistance.WaterPlacement;
    public bool ShouldHighlight(Type block) => false;
    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
}

[Serialized]
[RequireComponent(typeof(StandaloneAuthComponent))]
[RequireComponent(typeof(BatteryConsumptionComponent))]
[RequireComponent(typeof(PublicStorageComponent))]
[RequireComponent(typeof(TailingsReportComponent))]
[RequireComponent(typeof(MovableLinkComponent))]
[RequireComponent(typeof(VehicleComponent))]
[RequireComponent(typeof(BoatComponent))]
[RequireComponent(typeof(LadderComponent))]
[RequireComponent(typeof(MinimapComponent))]           
[RequireComponent(typeof(PartsComponent))]
[RepairRequiresSkill(typeof(ShipwrightSkill), 6)]
[Ecopedia("Crafted Objects", "Vehicles", subPageName: "Battery Powered Industrial Barge Item")]
public partial class BatteryPoweredIndustrialBargeObject : PhysicsWorldObject, IRepresentsItem
{
    static BatteryPoweredIndustrialBargeObject()
    {
        WorldObject.AddOccupancy<BatteryPoweredIndustrialBargeObject>(new List<BlockOccupancy>(0));
    }
    public override float InteractDistance => DefaultInteractDistance.WaterPlacement;
    public override TableTextureMode TableTexture => TableTextureMode.Metal;
    public override bool PlacesBlocks            => false;
    public override LocString DisplayName { get { return Localizer.DoStr("Battery Powered Industrial Barge"); } }
    public Type RepresentedItemType { get { return typeof(BatteryPoweredIndustrialBargeItem); } }

    private static string[] fuelTagList = new string[]
    {
        "Liquid Fuel",
    };
    private BatteryPoweredIndustrialBargeObject() { }
    protected override void Initialize()
    {
        base.Initialize();
        this.GetComponent<BatteryConsumptionComponent>().Initialize(1, 150);
        this.GetComponent<VehicleComponent>().HumanPowered(1);
        this.GetComponent<PublicStorageComponent>().Initialize(96, 32000000);
        this.GetComponent<MinimapComponent>().InitAsMovable();
        this.GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
        this.GetComponent<VehicleComponent>().Initialize(10, 2,1, null, true);
        this.GetComponent<BoatComponent>().Size = BoatComponent.BoatSize.Large;
        this.GetComponent<VehicleComponent>().FailDriveMsg = Localizer.Do($"You are too hungry to drive {this.DisplayName}!");
                    {
            this.GetComponent<PartsComponent>().Config(() => LocString.Empty, new PartInfo[]
            {
                                    new() { TypeName = nameof(CombustionEngineItem), Quantity = 2},
                                    new() { TypeName = nameof(LubricantItem), Quantity = 2},
                                    new() { TypeName = nameof(MetalRudderItem), Quantity = 1},
                                });
        }
    }
}
