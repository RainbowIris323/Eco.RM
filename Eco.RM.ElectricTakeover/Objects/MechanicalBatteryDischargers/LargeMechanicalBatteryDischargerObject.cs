using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Large Mechanical Battery Discharger")]
[RequireComponent(typeof(BatteryDischargerComponent))]
public partial class LargeMechanicalBatteryDischargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(LargeMechanicalBatteryDischargerItem);
    static LargeMechanicalBatteryDischargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryDischargerComponent>().Initialize(4, 4000, 20, true);
    }
}

[Serialized]
[LocDisplayName("Large Mechanical Battery Discharger")]
public partial class LargeMechanicalBatteryDischargerItem : WorldObjectItem<LargeMechanicalBatteryDischargerObject> { }