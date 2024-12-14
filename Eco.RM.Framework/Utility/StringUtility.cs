namespace Eco.RM.Framework.Utility;

public class StringUtility
{
    public static string ID => "Raynbo Mods";
    public static FormattableString AssembleLogMessage(string type, string message) => $"[{type}][{ID}]: {message}";
}
