using Eco.Core.Plugins.Interfaces;

namespace Eco.RM.WorldPeace;

public class Eco_RM_WorldPeace : IModInit
{
    public static ModRegistration Register() => new()
    {
        ModName = "Eco.RM.WorldPeace",
        ModDescription = "A utility mod used for global federation auto setup.",
        ModDisplayName = "Eco.RM.WorldPeace",
    };
} 