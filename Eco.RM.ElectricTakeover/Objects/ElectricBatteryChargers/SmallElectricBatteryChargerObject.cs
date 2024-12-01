using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Small Electric Battery Charger")]
[RequireComponent(typeof(BatteryChargerComponent))]
public partial class SmallElectricBatteryChargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(SmallElectricBatteryChargerItem);
    static SmallElectricBatteryChargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryChargerComponent>().Initialize(4, 600, 20, 0.7f, false);
    }
}

[Serialized]
[LocDisplayName("Small Electric Battery Charger")]
public partial class SmallElectricBatteryChargerItem : WorldObjectItem<SmallElectricBatteryChargerObject> { }