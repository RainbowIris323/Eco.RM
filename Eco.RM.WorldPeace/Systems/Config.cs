using Eco.Core.Utils;
using Eco.Shared.Localization;

namespace Eco.RM.WorldPeace.Systems;

public class RMWorldPeaceConfig
{
    [LocDisplayName("Use Global Federation")]
    [LocDescription("If true global federation will be created on command '/rm-setup-wp'.")]
    public bool GlobalFederationEnabled { get; set; } = false;

    [LocDisplayName("Federation Name")]
    [LocDescription("Name of the global federation.")]
    public string FederationName { get; set; } = "Eco Cookie Munchers";

    [LocDisplayName("Towns Enabled")]
    [LocDescription("If true sub settlements will be possible other wise not.")]
    public bool TownsEnabled { get; set; } = true;

    [LocDisplayName("Force Global Currency")]
    [LocDescription("If true only the global currency will be useable in stores.")]
    public bool ForceGlobalCurrency { get; set; } = true;

    [LocDisplayName("Admin Excused From Force Global Currency")]
    [LocDescription("If true other currencys can be used by admins.")]
    public bool AdminExcusedFromForceGlobalCurrency { get; set; } = true;

    [LocDisplayName("Federation Color")]
    [LocDescription("The color of the federation influence in RGB format.")]
    public SerializedSynchronizedCollection<float> FederationColor { get; set; } = [255, 0, 255];

    [LocDisplayName("Use Global Currency")]
    [LocDescription("If true global currency will be created on command '/rm-setup-wp'.")]
    public bool GlobalCurrencyEnabled { get; set; } = false;

    [LocDisplayName("Currency Name")]
    [LocDescription("Name of the global currency used on the server.")]
    public string CurrencyName { get; set; } = "Eco Cookies";

    [LocDisplayName("Base Grant On First Join")]
    [LocDescription("The base ammount to give when the player joins on day 0.")]
    public int BaseGrantOnJoin { get; set; } = 300;

    [LocDisplayName("Extra Grant On First Join Per Day Late")]
    [LocDescription("The ammount extra to give when the player joins after day 0 (value * days is added to the base).")]
    public int ExtraGrantOnJoinPerDayLate { get; set; } = 30;
}