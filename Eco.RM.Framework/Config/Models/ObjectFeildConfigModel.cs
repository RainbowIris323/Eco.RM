using Eco.Core.Utils;
using Eco.Shared.Localization;
using System.Text.Json.Serialization;

namespace Eco.RM.Framework.Config;

public class RMObjectFeildConfigModel
{
    [LocDisplayName("Key")]
    public string Key { get; set; }

    [LocDisplayName("Value")]
    public object Value { get; set; }

    [JsonConstructor]
    public RMObjectFeildConfigModel(string key, object value)
    {
        Key = key;
        Value = value;
    }
}