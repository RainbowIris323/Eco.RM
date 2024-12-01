using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Huge Mechanical Battery Discharger")]
[RequireComponent(typeof(BatteryDischargerComponent))]
public partial class HugeMechanicalBatteryDischargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(HugeMechanicalBatteryDischargerItem);
    static HugeMechanicalBatteryDischargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryDischargerComponent>().Initialize(4, 8000, 20, true);
    }
}

[Serialized]
[LocDisplayName("Huge Mechanical Battery Discharger")]
public partial class HugeMechanicalBatteryDischargerItem : WorldObjectItem<HugeMechanicalBatteryDischargerObject> { }