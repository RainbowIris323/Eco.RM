using Eco.RM.Framework.Config;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items;

[Serialized]
[LocDisplayName("Small Battery")]
public class SmallBatteryItem : BatteryItem, IConfiguredObject
{
    public override Type BatteryType => typeof(SmallBatteryItem);
    public SmallBatteryItem()
    {
        Dictionary<string, object> defaults = new()
        {
            { "ChargeRate", 150f },
            { "DischargeRate", 100f },
            { "MaxCharge", 300f }
        };
        RMGlobalConfigResolver.AddDefaults(nameof(SmallBatteryItem), defaults);

        ChargeRate = Convert.ToSingle(RMGlobalConfigResolver.GetObjectValue(nameof(SmallBatteryItem), "ChargeRate"));
        DischargeRate = Convert.ToSingle(RMGlobalConfigResolver.GetObjectValue(nameof(SmallBatteryItem), "DischargeRate"));
        MaxCharge = Convert.ToSingle(RMGlobalConfigResolver.GetObjectValue(nameof(SmallBatteryItem), "MaxCharge"));
    }
}



