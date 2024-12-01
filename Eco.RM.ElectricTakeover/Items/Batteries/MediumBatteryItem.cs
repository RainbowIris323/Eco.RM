using Eco.RM.Framework.Config;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items;

[Serialized]
[LocDisplayName("Medium Battery")]
public class MediumBatteryItem : BatteryItem, IConfiguredObject
{
    public override Type BatteryType => typeof(MediumBatteryItem);
    public MediumBatteryItem()
    {
        Dictionary<string, object> defaults = new()
        {
            { "ChargeRate", 750f },
            { "DischargeRate", 500f },
            { "MaxCharge", 1500f }
        };
        RMGlobalConfigResolver.AddDefaults(nameof(MediumBatteryItem), defaults);

        ChargeRate = Convert.ToSingle(RMGlobalConfigResolver.GetObjectValue(nameof(MediumBatteryItem), "ChargeRate"));
        DischargeRate = Convert.ToSingle(RMGlobalConfigResolver.GetObjectValue(nameof(MediumBatteryItem), "DischargeRate"));
        MaxCharge = Convert.ToSingle(RMGlobalConfigResolver.GetObjectValue(nameof(MediumBatteryItem), "MaxCharge"));
    }
}

