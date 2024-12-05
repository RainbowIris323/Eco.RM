using Eco.Core.Plugins.Interfaces;

namespace Eco.RM.Framework;

public class Eco_RM_Framework : IModInit
{
    public static ModRegistration Register() => new()
    {
        ModName = "Eco.RM.Framework",
        ModDescription = "A framework used for other mods made by Raynbo Mods.",
        ModDisplayName = "Eco.RM.Framework",
    };
}