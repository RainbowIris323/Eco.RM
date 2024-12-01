using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Small Mechanical Battery Discharger")]
[RequireComponent(typeof(BatteryDischargerComponent))]
public partial class SmallMechanicalBatteryDischargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(SmallMechanicalBatteryDischargerItem);
    static SmallMechanicalBatteryDischargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryDischargerComponent>().Initialize(4, 400, 20, true);
    }
}

[Serialized]
[LocDisplayName("Small Mechanical Battery Discharger")]
public partial class SmallMechanicalBatteryDischargerItem : WorldObjectItem<SmallMechanicalBatteryDischargerObject> { }