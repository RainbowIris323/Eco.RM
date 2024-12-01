using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Medium Mechanical Battery Charger")]
[RequireComponent(typeof(BatteryChargerComponent))]
public partial class MediumMechanicalBatteryChargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(MediumMechanicalBatteryChargerItem);
    static MediumMechanicalBatteryChargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryChargerComponent>().Initialize(4, 3000, 20, 0.6f, true);
    }
}

[Serialized]
[LocDisplayName("Medium Mechanical Battery Charger")]
public partial class MediumMechanicalBatteryChargerItem : WorldObjectItem<MediumMechanicalBatteryChargerObject> { }