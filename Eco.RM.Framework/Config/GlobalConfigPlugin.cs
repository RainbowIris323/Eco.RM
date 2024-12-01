using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Shared.Localization;
using Eco.Shared.Utils;

namespace Eco.RM.Framework.Config;

[LocDisplayName("Raynbo Mods Config Plugin")]
[Priority(200)]
public class RMGlobalConfigPlugin : Singleton<RMGlobalConfigPlugin>, IModKitPlugin, IConfigurablePlugin, IModInit
{
    private static readonly PluginConfig<RMGlobalConfigModel> config;
    public IPluginConfig PluginConfig => config;
    public static RMGlobalConfigModel Config => config.Config;
    public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

    public object GetEditObject() => config.Config;
    public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
    public string GetStatus() => $"Loaded {RMGlobalConfigResolver.LoadedObjectConfigs.Count} Object Configs With {RMGlobalConfigResolver.LoadedObjectConfigs.Sum((a) => a.Feilds.Count)} Total Feilds";

    static RMGlobalConfigPlugin()
    {
        config = new PluginConfig<RMGlobalConfigModel>("RMGlobalConfig");
    }

    public static void PostInitialize()
    {
        RMGlobalConfigResolver.Obj.Initialize();
        config.SaveAsync();
    }

    public override string ToString() => Localizer.DoStr("Global Config Plugin");

    public string GetCategory() => "Raynbo Mods";
}