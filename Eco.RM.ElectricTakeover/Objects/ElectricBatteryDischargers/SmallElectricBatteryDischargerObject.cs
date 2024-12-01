using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Small Electric Battery Discharger")]
[RequireComponent(typeof(BatteryDischargerComponent))]
public partial class SmallElectricBatteryDischargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(SmallElectricBatteryDischargerItem);
    static SmallElectricBatteryDischargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryDischargerComponent>().Initialize(4, 400, 20, false);
    }
}

[Serialized]
[LocDisplayName("Small Electric Battery Discharger")]
public partial class SmallElectricBatteryDischargerItem : WorldObjectItem<SmallElectricBatteryDischargerObject> { }