using Eco.RM.Framework.Config;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items;

[Serialized]
[LocDisplayName("Large Battery")]
public class LargeBatteryItem : BatteryItem, IConfiguredObject
{
    public override Type BatteryType => typeof(LargeBatteryItem);
    public LargeBatteryItem()
    {
        Dictionary<string, object> defaults = new()
        {
            { "ChargeRate", 1500f },
            { "DischargeRate", 1000f },
            { "MaxCharge", 4000f }
        };
        RMGlobalConfigResolver.AddDefaults(nameof(LargeBatteryItem), defaults);

        ChargeRate = Convert.ToSingle(RMGlobalConfigResolver.GetObjectValue(nameof(LargeBatteryItem), "ChargeRate"));
        DischargeRate = Convert.ToSingle(RMGlobalConfigResolver.GetObjectValue(nameof(LargeBatteryItem), "DischargeRate"));
        MaxCharge = Convert.ToSingle(RMGlobalConfigResolver.GetObjectValue(nameof(LargeBatteryItem), "MaxCharge"));
    }
}

