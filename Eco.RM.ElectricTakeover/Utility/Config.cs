using Eco.RM.Framework.Resolvers;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;

namespace Eco.RM.ElectricTakeover.Utility;

[Serialized]
public class ElectricTakeoverConfig : AutoSingleton<ElectricTakeoverConfig>, IConfiguredObject
{
    [Serialized] public float BatteryPoweredVehicleSpeedBuffRatio { get; set; } = 0f;
    static readonly Dictionary<string, object> DefaultConfig = new()
    {
        { nameof(BatteryPoweredVehicleSpeedBuffRatio), 1.5f }
    };

    static ElectricTakeoverConfig() { RMObjectResolver.AddDefaults(nameof(ElectricTakeoverConfig), DefaultConfig); }
    public ElectricTakeoverConfig()
    {
        BatteryPoweredVehicleSpeedBuffRatio = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(ElectricTakeoverConfig), nameof(BatteryPoweredVehicleSpeedBuffRatio)));
    }

    public float GetBatteryPoweredVehicleSpeed(float current) => current * BatteryPoweredVehicleSpeedBuffRatio;
} 