using Eco.Core.Plugins.Interfaces;

namespace Eco.RM.ElectricTakeover;

public class Eco_RM_ElectricTakeover : IModInit
{
    public static ModRegistration Register() => new()
    {
        ModName = "Eco.RM.ElectricTakeover",
        ModDescription = "Allows for powering vehicles and work tables with reuseable zero waste batteries ",
        ModDisplayName = "Eco.RM.ElectricTakeover",
    };
}