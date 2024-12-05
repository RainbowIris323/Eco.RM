using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Shared.Localization;
using Eco.Shared.Utils;

namespace Eco.RM.Framework.Config;

[LocDisplayName("Object Config Plugin")]
[Priority(200)]
public class RMObjectConfigPlugin : Singleton<RMObjectConfigPlugin>, IModKitPlugin, IConfigurablePlugin, IModInit, IDisplayablePlugin
{
    private static readonly PluginConfig<RMObjectConfig> config;
    public IPluginConfig PluginConfig => config;
    public static RMObjectConfig Config => config.Config;
    public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

    public object GetEditObject() => config.Config;
    public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
    public string GetStatus()
    {
        var s = new LocStringBuilder();
        s.AppendLineLoc($"Using {(RMObjectResolver.OverridesEnabled ? "Configured" : "Default")} Models.");
        s.AppendLineLoc($"Loaded {RMObjectResolver.DefaultObjectModels.Count} Default Objects With {RMObjectResolver.DefaultObjectModels.Sum((a) => a.Feilds.Count)} Total Feilds.");
        s.AppendLineLoc($"Loaded {RMObjectResolver.ConfiguredObjectModels.Count} Configured Objects With {RMObjectResolver.ConfiguredObjectModels.Sum((a) => a.Feilds.Count)} Total Feilds.");
        return s.ToString();
    }
    public string GetDisplayText() => GetStatus();

    static RMObjectConfigPlugin()
    {
        config = new PluginConfig<RMObjectConfig>("RMObjectConfig");
    }

    public static void Initialize()
    {
        RMObjectResolver.Obj.Initialize();
        config.SaveAsync();
    }

    public override string ToString() => Localizer.DoStr("Object Config");

    public string GetCategory() => "Raynbo Mods";
}