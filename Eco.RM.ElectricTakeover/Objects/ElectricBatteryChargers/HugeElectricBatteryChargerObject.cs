using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Huge Electric Battery Charger")]
[RequireComponent(typeof(BatteryChargerComponent))]
public partial class HugeElectricBatteryChargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(HugeElectricBatteryChargerItem);
    static HugeElectricBatteryChargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryChargerComponent>().Initialize(4, 12000, 20, 1f, false);
    }
}

[Serialized]
[LocDisplayName("Huge Electric Battery Charger")]
public partial class HugeElectricBatteryChargerItem : WorldObjectItem<HugeElectricBatteryChargerObject> { }