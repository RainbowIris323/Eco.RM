using Eco.RM.Framework.Resolvers;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;

namespace Eco.RM.SmogFilter.Utility;

[Serialized]
public class SmogFilterConfig : AutoSingleton<SmogFilterConfig>, IConfiguredObject
{
    [Serialized] public float SmogPerCompostBlock { get; set; }
    [Serialized] public float SmogConsumptionRate { get; set; }
    [Serialized] public float OxygenProductionRate { get; set; }
    static readonly Dictionary<string, object> DefaultConfig = new()
    {
        { nameof(SmogPerCompostBlock), 100f },
        { nameof(SmogConsumptionRate), 2f },
        { nameof(OxygenProductionRate), 1f }
    };

    static SmogFilterConfig() { RMObjectResolver.AddDefaults(nameof(SmogFilterConfig), DefaultConfig); }
    public SmogFilterConfig()
    {
        SmogPerCompostBlock = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(SmogFilterConfig), nameof(SmogPerCompostBlock)));
        SmogConsumptionRate = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(SmogFilterConfig), nameof(SmogConsumptionRate)));
        OxygenProductionRate = Convert.ToSingle(RMObjectResolver.GetObjectValue(nameof(SmogFilterConfig), nameof(OxygenProductionRate)));
    }
} 