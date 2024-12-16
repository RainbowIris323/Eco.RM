using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Components;
using Eco.Gameplay.Objects;
using Eco.RM.SmogFilter.Components;
using Eco.Gameplay.Items;
using Eco.Shared.Serialization;
using Eco.Core.Items;
using Eco.Gameplay.Occupancy;
using Eco.Shared.Localization;
using Eco.Shared.Math;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Shared.Items;
using Eco.Shared.Utils;
using Eco.Core.Controller;
using Eco.RM.Framework.Resolvers;
using Eco.RM.Framework.Utility;
using Eco.Mods.TechTree;

namespace Eco.RM.SmogFilter.Objects;

[Serialized]
[LocDisplayName("Smog Filter")]
[LocDescription("Filters the waste out of smog.")]
[Ecopedia("Crafted Objects", "Specialty", createAsSubPage: true)]
[Weight(5000)]
public class SmogFilterItem : WorldObjectItem<SmogFilterObject>, IPersistentData, IConfiguredObject
{
    protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(DirectionAxisFlags.None | DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(WorldObjectType));
    [NewTooltip(CacheAs.SubType, 7)] public static LocString PowerConsumptionTooltip() => Localizer.Do($"Consumes: {Text.Info(100)}w of {new ElectricPower().Name} power.");
    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
}

[Serialized]
[Tag("Usable")]
[LocDisplayName("Smog Filter")]
[RequireNoRoomContainment]
[RequireComponent(typeof(PowerGridComponent))]
[RequireComponent(typeof(PowerConsumptionComponent))]
[RequireComponent(typeof(OccupancyRequirementComponent))]
[RequireComponent(typeof(ForSaleComponent))]
[RequireComponent(typeof(PropertyAuthComponent))]
[RequireComponent(typeof(OnOffComponent))]
[RequireComponent(typeof(SmogFilterComponent))]
[RequireComponent(typeof(AttachmentComponent))]
[Ecopedia("Crafted Objects", "Specialty", subPageName: "Smog Filter Item")]
public class SmogFilterObject : WorldObject, IRepresentsItem
{
    public Type RepresentedItemType => typeof(SmogFilterItem);

    protected override void Initialize()
    {
        GetComponent<PowerConsumptionComponent>().Initialize(100);
        GetComponent<PowerGridComponent>().Initialize(10, new ElectricPower());
    }
    static SmogFilterObject()
    {
        AddOccupancy<WasteFilterObject>(new List<BlockOccupancy>(){
            new BlockOccupancy(new Vector3i(0, 0, 0), typeof(PipeSlotBlock), new Quaternion(0f, -0.7071068f, 0f, 0.7071068f), BlockOccupancyType.InputPort),
            new BlockOccupancy(new Vector3i(0, 0, 0), typeof(PipeSlotBlock), new Quaternion(-0.7071071f, 2.634177E-07f, 2.634179E-07f, 0.7071065f), BlockOccupancyType.OutputPort)
        });
    }
}