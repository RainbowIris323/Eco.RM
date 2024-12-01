using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Medium Mechanical Battery Discharger")]
[RequireComponent(typeof(BatteryDischargerComponent))]
public partial class MediumMechanicalBatteryDischargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(MediumMechanicalBatteryDischargerItem);
    static MediumMechanicalBatteryDischargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryDischargerComponent>().Initialize(4, 2000, 20, true);
    }
}

[Serialized]
[LocDisplayName("Medium Mechanical Battery Discharger")]
public partial class MediumMechanicalBatteryDischargerItem : WorldObjectItem<MediumMechanicalBatteryDischargerObject> { }