using Eco.RM.Framework.Config;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items;

[Serialized]
[LocDisplayName("Huge Battery")]
public class HugeBatteryItem : BatteryItem, IConfiguredObject
{
    public override Type BatteryType => typeof(HugeBatteryItem);
    public HugeBatteryItem()
    {
        Dictionary<string, object> defaults = new()
        {
            { "ChargeRate", 3000 },
            { "DischargeRate", 2000f },
            { "MaxCharge", 8000f }
        };
        RMGlobalConfigResolver.AddDefaults(nameof(HugeBatteryItem), defaults);

        ChargeRate = Convert.ToSingle(RMGlobalConfigResolver.GetObjectValue(nameof(HugeBatteryItem), "ChargeRate"));
        DischargeRate = Convert.ToSingle(RMGlobalConfigResolver.GetObjectValue(nameof(HugeBatteryItem), "DischargeRate"));
        MaxCharge = Convert.ToSingle(RMGlobalConfigResolver.GetObjectValue(nameof(HugeBatteryItem), "MaxCharge"));
    }
}

