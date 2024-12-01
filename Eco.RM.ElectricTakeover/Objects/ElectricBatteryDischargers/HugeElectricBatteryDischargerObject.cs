using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Huge Electric Battery Discharger")]
[RequireComponent(typeof(BatteryDischargerComponent))]
public partial class HugeElectricBatteryDischargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(HugeElectricBatteryDischargerItem);
    static HugeElectricBatteryDischargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryDischargerComponent>().Initialize(4, 8000, 20, false);
    }
}

[Serialized]
[LocDisplayName("Huge Electric Battery Discharger")]
public partial class HugeElectricBatteryDischargerItem : WorldObjectItem<HugeElectricBatteryDischargerObject> { }