using Eco.Core.Plugins.Interfaces;

namespace Eco.RM.SmogFilter;

public class Eco_RM_SmogFilter : IModInit
{
    public static ModRegistration Register() => new()
    {
        ModName = "Eco.RM.SmogFilter",
        ModDescription = "Allows for filtering the waste from smog.",
        ModDisplayName = "Eco.RM.SmogFilter",
    };
}