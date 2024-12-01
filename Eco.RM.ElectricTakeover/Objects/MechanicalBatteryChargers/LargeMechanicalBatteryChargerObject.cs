using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Large Mechanical Battery Charger")]
[RequireComponent(typeof(BatteryChargerComponent))]
public partial class LargeMechanicalBatteryChargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(LargeMechanicalBatteryChargerItem);
    static LargeMechanicalBatteryChargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryChargerComponent>().Initialize(4, 3000, 20, 0.7f, true);
    }
}

[Serialized]
[LocDisplayName("Large Mechanical Battery Charger")]
public partial class LargeMechanicalBatteryChargerItem : WorldObjectItem<LargeMechanicalBatteryChargerObject> { }