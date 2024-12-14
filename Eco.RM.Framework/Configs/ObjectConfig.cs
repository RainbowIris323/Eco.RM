using Eco.Core.Utils;
using Eco.RM.Framework.Models;
using Eco.Shared.Localization;

namespace Eco.RM.Framework.Configs;

public class RMObjectConfig
{
    [LocDisplayName("Use Object Configs")]
    [LocDescription("Enable to use config file based object data or disable to let the mod handle its own object data.")]
    public bool UseOverrides { get; set; } = false;

    [LocDisplayName("Object Configuration")]
    [LocDescription("Object Configuration. ANY change to this list will require a server restart to take effect.")]
    public SerializedSynchronizedCollection<RMObjectConfigModel> Objects { get; set; } = [];   
}