using Eco.Core.Utils;
using Eco.RM.Framework.Models;
using Eco.Shared.Localization;

namespace Eco.RM.Framework.Configs;

public class RMRecipeConfig
{

    [LocDisplayName("Use Recipe Configs")]
    [LocDescription("Enable to use config file based Recipes or disable to let the mod handle its own recipes")]
    public bool UseOverrides { get; set; } = false;

    [LocDisplayName("Recipe Configs")]
    [LocDescription("Recipes loaded by modules. ANY change to this list will require a server restart to take effect.")]
    public SerializedSynchronizedCollection<RecipeModel> Recipes { get; set; } = [];
}