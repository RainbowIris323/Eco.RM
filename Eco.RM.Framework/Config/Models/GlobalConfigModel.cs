using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.RM.Framework.Config;

public class RMGlobalConfigModel
{
    [LocDescription("Object Configuration. ANY change to this list will require a server restart to take effect.")]
    [LocDisplayName("Object Configuration")]
    public SerializedSynchronizedCollection<RMObjectConfigModel> ObjectConfigs { get; set; } = new();
}