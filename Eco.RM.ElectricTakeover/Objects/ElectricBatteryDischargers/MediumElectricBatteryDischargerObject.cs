using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.RM.ElectricTakeover.Components;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Medium Electric Battery Discharger")]
[RequireComponent(typeof(BatteryDischargerComponent))]
public partial class MediumElectricBatteryDischargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(MediumElectricBatteryDischargerItem);
    static MediumElectricBatteryDischargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryDischargerComponent>().Initialize(4, 2000, 20, false);
    }
}

[Serialized]
[LocDisplayName("Medium Electric Battery Discharger")]
public partial class MediumElectricBatteryDischargerItem : WorldObjectItem<MediumElectricBatteryDischargerObject> { }