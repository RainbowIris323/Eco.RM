using Eco.Core.Utils;
using Eco.RM.Framework.Utility;
using Eco.Shared.Utils;

namespace Eco.RM.Framework.Config;

public interface IConfiguredObject { }

public class RMObjectResolver : AutoSingleton<RMObjectResolver>
{
    public static bool OverridesEnabled => RMObjectConfigPlugin.Config.UseOverrides;
    public static SerializedSynchronizedCollection<RMObjectConfigModel> DefaultObjectModels { get; private set; } = [];
    public static SerializedSynchronizedCollection<RMObjectConfigModel> ConfiguredObjectModels { get; private set; } = [];
    ///<summary>Loads default values of an object that can be overriden.</summary>
    ///<param name="objectName">The name of the class that uses the data.</param>
    ///<param name="feilds">A dictionary of all configurable property names and default values.</param>
    public static void AddDefaults(string objectName, Dictionary<string, object> feilds)
    {
        if (DefaultObjectModels.SingleOrDefault(x => x.Name == objectName) != null) { LogUtil.Warn($"An object with the same name of '{objectName}' has allready been assigned defaults within 'RMObjectResolver.DefaultObjectConfigs'"); return; }
        var defaults = new SerializedSynchronizedCollection<RMObjectFeildConfigModel>();
        feilds.ForEach((a) =>
        {
            defaults.Add(new RMObjectFeildConfigModel(a.Key, a.Value));
        });
        var model = new RMObjectConfigModel(objectName, defaults);
        DefaultObjectModels.Add(model);
        AddDefaultConfigToConfigured(model);
    }
    ///<summary>Adds a default object model to the configured object models if there is not currently a model of the same name within it.</summary>
    ///<param name="model">The default model for the object.</param>
    public static void AddDefaultConfigToConfigured(RMObjectConfigModel model)
    {
        if (ConfiguredObjectModels.SingleOrDefault(x => x.Name == model.Name) != null) return;
        ConfiguredObjectModels.Add(model);
    }
    ///<summary>Gets the value of an object feild from loaded models.</summary>
    ///<param name="objectName">The name of the object that set the data.</param>
    ///<param name="feildName">The feild name used in creation of the defaults.</param>
    ///<returns>The value of the feild. You may need to cast it to the known type.</returns>
    public static object GetObjectValue(string objectName, string feildName)
    {
        var model = (OverridesEnabled ? ConfiguredObjectModels : DefaultObjectModels).Where((a) =>  a.Name == objectName).FirstOrDefault();
        if (model == null) throw new Exception($"Tried to access a config that does not exist. ObjectType: '{objectName}'");
        var feild = model.Feilds.Where((a) => a.Key == feildName).FirstOrDefault();
        if (feild == null) throw new Exception($"Tried to access a config that does not exist. ObjectType: '{objectName}', FeildType: '{feildName}'");
        return feild.Value;
    }

    public void Initialize()
    {
        ConfiguredObjectModels = RMObjectConfigPlugin.Config.Objects; //Gets the saved config.

        foreach (var type in typeof(IConfiguredObject).ConcreteTypes())
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }

        RMObjectConfigPlugin.Config.Objects = ConfiguredObjectModels; //Saves the updated config.
    }
}