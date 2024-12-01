using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Items;
using Eco.Gameplay.Skills;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Occupancy;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Shared.Math;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Items;
using static Eco.Gameplay.Components.PartsComponent;
using Eco.Mods.TechTree;
using Eco.RM.ElectricTakeover.Components;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Battery Powered Crane")]
[LocDescription("Allows the placement and transport of materials in an area.")]
[IconGroup("World Object Minimap")]
[Weight(25000)]
[RequireComponent(typeof(OccupancyRequirementComponent))]
[Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
public partial class BatteryPoweredCraneItem : WorldObjectItem<BatteryPoweredCraneObject>, IPersistentData
{
    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
    protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(this.WorldObjectType));
}

[Serialized]
[RequireComponent(typeof(StandaloneAuthComponent))]
[RequireComponent(typeof(BatteryConsumptionComponent))]
[RequireComponent(typeof(VehicleComponent))]
[RequireComponent(typeof(MinimapComponent))]           
[RequireComponent(typeof(OccupancyRequirementComponent))]
[RequireComponent(typeof(MovableLinkComponent))]
[RequireComponent(typeof(CraneToolComponent))]
[RequireComponent(typeof(PhysicsValueSyncComponent))]
[RequireComponent(typeof(PartsComponent))]
[RepairRequiresSkill(typeof(MechanicsSkill), 5)]
[LocDisplayName("Battery Powered Crane")]
[Ecopedia("Crafted Objects", "Vehicles", subPageName: "Battery Powered Crane Item")]
public partial class BatteryPoweredCraneObject : PhysicsWorldObject, IRepresentsItem
{
    static BatteryPoweredCraneObject()
    {
        AddOccupancy<BatteryPoweredCraneObject>(new List<BlockOccupancy>(0));
    }
    public override TableTextureMode TableTexture => TableTextureMode.Metal;
    public Type RepresentedItemType { get { return typeof(BatteryPoweredCraneItem); } }

    private static string[] fuelTagList = new string[]
    {
        "Burnable Fuel",
    };
    private BatteryPoweredCraneObject() { }
    protected override void Initialize()
    {
        base.Initialize();
        GetComponent<BatteryConsumptionComponent>().Initialize(1, 150);
        GetComponent<CraneToolComponent>().Initialize(200, 150);
        GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
        GetComponent<VehicleComponent>().Initialize(30, 1,1);
        GetComponent<VehicleComponent>().FailDriveMsg = Localizer.Do($"You are too hungry to drive {DisplayName}!");
        {
            GetComponent<PartsComponent>().Config(() => LocString.Empty, new PartInfo[]
            {
                new() { TypeName = nameof(PortableSteamEngineItem), Quantity = 1},
            });
        }
    }
}
