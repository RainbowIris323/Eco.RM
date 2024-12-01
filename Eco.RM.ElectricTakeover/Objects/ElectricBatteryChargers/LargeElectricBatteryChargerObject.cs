using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Large Electric Battery Charger")]
[RequireComponent(typeof(BatteryChargerComponent))]
public partial class LargeElectricBatteryChargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(LargeElectricBatteryChargerItem);
    static LargeElectricBatteryChargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryChargerComponent>().Initialize(4, 3000, 20, 0.9f, false);
    }
}

[Serialized]
[LocDisplayName("Large Electric Battery Charger")]
public partial class LargeElectricBatteryChargerItem : WorldObjectItem<LargeElectricBatteryChargerObject> { }