using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Medium Electric Battery Charger")]
[RequireComponent(typeof(BatteryChargerComponent))]
public partial class MediumElectricBatteryChargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(MediumElectricBatteryChargerItem);
    static MediumElectricBatteryChargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryChargerComponent>().Initialize(4, 3000, 20, 0.8f, false);
    }
}

[Serialized]
[LocDisplayName("Medium Electric Battery Charger")]
public partial class MediumElectricBatteryChargerItem : WorldObjectItem<MediumElectricBatteryChargerObject> { }