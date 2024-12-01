using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Small Mechanical Battery Charger")]
[RequireComponent(typeof(BatteryChargerComponent))]
public partial class SmallMechanicalBatteryChargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(SmallMechanicalBatteryChargerItem);
    static SmallMechanicalBatteryChargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryChargerComponent>().Initialize(4, 600, 20, 0.5f, true);
    }
}

[Serialized]
[LocDisplayName("Small Mechanical Battery Charger")]
public partial class SmallMechanicalBatteryChargerItem : WorldObjectItem<SmallMechanicalBatteryChargerObject> { }