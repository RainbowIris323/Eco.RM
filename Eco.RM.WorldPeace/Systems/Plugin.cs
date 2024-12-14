using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Players;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Gameplay.Economy;
using Eco.Simulation.Time;
using Eco.Gameplay.Objects;
using Eco.RM.WorldPeace.Federation;
using Eco.Shared.Serialization;
using Eco.Shared.IoC;

namespace Eco.RM.WorldPeace.Systems;

[ChatCommandHandler]
public class RMWorldPeacePlugin : Singleton<RMWorldPeacePlugin>, IModKitPlugin, IConfigurablePlugin, IModInit, IDisplayablePlugin
{
    private static readonly PluginConfig<RMWorldPeaceConfig> config;
    [Serialized] public static GlobalFederationObject? GlobalFederationObject { get; private set; } = null;
    [Serialized] public static Currency? GlobalCurrency { get; private set; } = null;
    [Serialized] public static BankAccount? GlobalCurrencyBank { get; private set; } = null;
    public IPluginConfig PluginConfig => config;
    public static RMWorldPeaceConfig Config => config.Config;
    public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();

    public object GetEditObject() => config.Config;
    public void OnEditObjectChanged(object o, string param) => config.SaveAsync();
    public string GetStatus()
    {
        var s = new LocStringBuilder();
        s.AppendLineLoc($"The Global Federation Is {(config.Config.GlobalFederationEnabled ? "Enabled" : "Disabled")}.");
        s.AppendLineLoc($"The Global Currency Is {(config.Config.GlobalCurrencyEnabled ? "Enabled" : "Disabled")}.");
        return s.ToString();
    }
    public string GetDisplayText() => GetStatus();

    static RMWorldPeacePlugin()
    {
        config = new PluginConfig<RMWorldPeaceConfig>("RMWorldPeace");
    }

    public static void Initialize()
    {
        UserManager.NewUserJoinedEvent.Add(OnUserLogin);
        config.SaveAsync();
    }

    public override string ToString() => Localizer.DoStr("World Peace Plugin");

    public string GetCategory() => "Raynbo Mods";

    [ChatCommand("Sets up the federation and currency based on config settings.", "rmwp-setup", ChatAuthorizationLevel.Admin)]
    public static void RMWorldPeaceSetupCommand(User user)
    {
        if (config == null)
        {
            user.MsgLocStr(Localizer.DoStr("Config not found!"), style: Shared.Services.NotificationStyle.Chat);
            return;
        }
        if (config.Config.GlobalCurrencyEnabled == true) SetupCurrency(user);
        if (config.Config.GlobalFederationEnabled == true) SetupFederation(user);
    }

    [ChatCommand("Views the federation object UI.", "rmwp-view", ChatAuthorizationLevel.Admin)]
    public static void RMWorldPeaceViewFederationCommand(User user)
    {
        if (GlobalFederationObject == null)
        {
            user.MsgLocStr(Localizer.DoStr("Federation has not been setup yet!"), style: Shared.Services.NotificationStyle.Chat);
            return;
        }
        GlobalFederationObject.OpenUI(user.Player);
    }

    public static void SetupCurrency(User user)
    {
        var currency = CurrencyManager.Currencies.FirstOrDefault((currency) => currency != null && currency.Name == config.Config.CurrencyName, null);

        if (currency == null)
        {
            currency = CurrencyManager.AddCurrency(user, config.Config.CurrencyName, Shared.Items.CurrencyType.Backed);
            user.MsgLocStr(Localizer.DoStr("Created global currency."), style: Shared.Services.NotificationStyle.Chat);
        }

        GlobalCurrency = currency;

        string globalAccountName = config.Config.CurrencyName + " - Treasury";

        var treasuryBankAccount = BankAccountManager.Obj.Accounts.FirstOrDefault((account) => account != null && account.Name == globalAccountName, null);

        if (treasuryBankAccount == null)
        {
            BankAccountManager.CreateAccount(user, globalAccountName);
            treasuryBankAccount = BankAccountManager.Obj.Accounts.FirstOrDefault((account) => account != null && account.Name == globalAccountName, null);

            if (treasuryBankAccount != null)
            {
                BankAccountManager.AddAccountManager(user, treasuryBankAccount, user);
                treasuryBankAccount.AddCurrency(currency, float.MaxValue);
                GlobalCurrencyBank = treasuryBankAccount;
                user.MsgLocStr(Localizer.DoStr("Created bank for global currency."), style: Shared.Services.NotificationStyle.Chat);
            }
        }
    }

    public static void SetupFederation(User user)
    {
        GlobalFederationObject = (GlobalFederationObject)WorldObjectManager.ForceAdd(typeof(GlobalFederationObject), user, new System.Numerics.Vector3(3, 100, 3), new Shared.Math.Quaternion(0, 0, 0, 0), false);
        ServiceHolder<IWorldObjectManager>.Obj.Add(GlobalFederationObject, user, new System.Numerics.Vector3(3, 100, 3), new Shared.Math.Quaternion(0, 0, 0, 0));
    }

    public static void OnUserLogin(User user)
    {
        if (config != null && config.Config.BaseGrantOnJoin + (config.Config.ExtraGrantOnJoinPerDayLate * WorldTime.Day) > 0 && config.Config.CurrencyName != null && config.Config.CurrencyName != "")
        {
            var currency = CurrencyManager.Currencies.FirstOrDefault((currency) => currency != null && currency.Name == config.Config.CurrencyName, null);

            if (currency != null)
            {
                BankAccountManager.Obj.SpawnMoney(currency, user, (float)(config.Config.BaseGrantOnJoin + (config.Config.ExtraGrantOnJoinPerDayLate * WorldTime.Day)));
            }
        }
    }
}