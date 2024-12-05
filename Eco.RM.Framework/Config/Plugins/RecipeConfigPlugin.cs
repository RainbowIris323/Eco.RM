using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Shared.Localization;
using Eco.Shared.Utils;

namespace Eco.RM.Framework.Config;

[LocDisplayName("Recipe Config Plugin")]
[Priority(200)]
public class RMRecipeConfigPlugin : Singleton<RMRecipeConfigPlugin>, IModKitPlugin, IConfigurablePlugin, IModInit, IDisplayablePlugin
{
    private static readonly PluginConfig<RMRecipeConfig> config;
    public IPluginConfig PluginConfig => config;
    public static RMRecipeConfig Config => config.Config;
    public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

    public object GetEditObject() => config.Config;
    public void OnEditObjectChanged(object o, string param) => this.SaveConfig();
    public string GetStatus() => $"N/A";
    public string GetDisplayText() => GetStatus();

    static RMRecipeConfigPlugin()
    {
        config = new PluginConfig<RMRecipeConfig>("RMRecipeConfig");
    }

    public static void Initialize()
    {
        RMRecipeResolver.Obj.Initialize();
        config.SaveAsync();
    }

    public override string ToString() => Localizer.DoStr("Recipe Config");

    public string GetCategory() => "Raynbo Mods";
}