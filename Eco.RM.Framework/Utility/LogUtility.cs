using Eco.Shared.Logging;

namespace Eco.RM.Framework.Utility;

public class LogUtililty
{
    public static void Inform(string message)
    {
        Log.WriteLineLoc(StringUtility.AssembleLogMessage("Info", message));
    }
    public static void Warn(string message)
    {
        Log.WriteWarningLineLoc(StringUtility.AssembleLogMessage("Warn", message));
    }
    public static void Error(string message)
    {
        Log.WriteErrorLineLoc(StringUtility.AssembleLogMessage("Error", message));
    }
}
