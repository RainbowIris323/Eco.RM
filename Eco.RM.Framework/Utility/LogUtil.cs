using Eco.Shared.Logging;

namespace Eco.RM.Framework.Utility;

public class LogUtil
{
    public static FormattableString AssembleMessage(string type, string message) => $"[{type}][Raynbo Mods]: {message}";
    public static void Inform(string message)
    {
        Log.WriteLineLoc(AssembleMessage("Info", message));
    }
    public static void Warn(string message)
    {
        Log.WriteWarningLineLoc(AssembleMessage("Warn", message));
    }
    public static void Error(string message)
    {
        Log.WriteErrorLineLoc(AssembleMessage("Error", message));
    }
}
