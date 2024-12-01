using Eco.Core.Utils;
using Eco.Shared.Localization;
using System.Text.Json.Serialization;

namespace Eco.RM.Framework.Config;

public class RMObjectConfigModel
{
    [LocDisplayName("Name")]
    public string Name { get; set; }

    [LocDisplayName("Feilds")]
    public SerializedSynchronizedCollection<RMObjectFeildConfigModel> Feilds { get; set; }

    [JsonConstructor]
    public RMObjectConfigModel(string name, SerializedSynchronizedCollection<RMObjectFeildConfigModel> feilds)
    {
        Name = name;
        Feilds = feilds;
    }
}