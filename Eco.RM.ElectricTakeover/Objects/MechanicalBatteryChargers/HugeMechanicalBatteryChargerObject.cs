using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Huge Mechanical Battery Charger")]
[RequireComponent(typeof(BatteryChargerComponent))]
public partial class HugeMechanicalBatteryChargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(HugeMechanicalBatteryChargerItem);
    static HugeMechanicalBatteryChargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryChargerComponent>().Initialize(4, 12000, 20, 0.8f, true);
    }
}

[Serialized]
[LocDisplayName("Huge Mechanical Battery Charger")]
public partial class HugeMechanicalBatteryChargerItem : WorldObjectItem<HugeMechanicalBatteryChargerObject> { }