using Eco.Core.Utils;
using Eco.Shared.Utils;

namespace Eco.RM.Framework.Config;

public interface IConfiguredObject { }

public class RMGlobalConfigResolver : AutoSingleton<RMGlobalConfigResolver>
{
    public static SerializedSynchronizedCollection<RMObjectConfigModel> PreviousObjectConfigs { get; private set; } = new();
    public static SerializedSynchronizedCollection<RMObjectConfigModel> LoadedObjectConfigs { get; private set; } = new();
    public static void AddDefaults(string objectName, Dictionary<string, object> objectFeilds)
    {
        var newDefaults = new SerializedSynchronizedCollection<RMObjectFeildConfigModel>();
        objectFeilds.ForEach((a) =>
        {
            newDefaults.Add(new RMObjectFeildConfigModel(a.Key, a.Value));
        });
        var newModel = new RMObjectConfigModel(objectName, newDefaults);

        var m = PreviousObjectConfigs.SingleOrDefault(x => x.Name == newModel.Name);

        if (m != null) LoadedObjectConfigs.Add(m);
        else LoadedObjectConfigs.Add(newModel);
    }
    public static object GetObjectValue(string objectName, string feildName)
    {
        var objectModel = LoadedObjectConfigs.Where((a) =>  a.Name == objectName).First();
        if (objectModel == null) throw new Exception($"Tried to access a config that does not exist. ObjectType: '{objectName}'");
        var feildModel = objectModel.Feilds.Where((a) => a.Key == feildName).First();
        if (feildModel == null) throw new Exception($"Tried to access a config that does not exist. ObjectType: '{objectName}', FeildType: '{feildName}'");
        return feildModel.Value;
    }

    public void Initialize()
    {
        try
        {
            PreviousObjectConfigs = RMGlobalConfigPlugin.Config.ObjectConfigs;
        }
        catch
        {
            PreviousObjectConfigs = new();
        }
        foreach (var type in typeof(IConfiguredObject).ConcreteTypes())
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }

        RMGlobalConfigPlugin.Config.ObjectConfigs = LoadedObjectConfigs;
    }
}