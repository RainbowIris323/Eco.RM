using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Large Electric Battery Discharger")]
[RequireComponent(typeof(BatteryDischargerComponent))]
public partial class LargeElectricBatteryDischargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(LargeElectricBatteryDischargerItem);
    static LargeElectricBatteryDischargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryDischargerComponent>().Initialize(4, 4000, 20, false);
    }
}

[Serialized]
[LocDisplayName("Large Electric Battery Discharger")]
public partial class LargeElectricBatteryDischargerItem : WorldObjectItem<LargeElectricBatteryDischargerObject> { }