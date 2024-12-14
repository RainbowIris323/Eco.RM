using Eco.Core.Utils;
using Eco.Gameplay.Civics.Misc;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.Gameplay.Settlements;
using Eco.Shared.Items;
using Eco.Shared.Serialization;
using Eco.Shared.Services;
using Eco.Shared.Utils;
using Eco.RM.WorldPeace.Systems;
using Eco.Gameplay.Settlements.Components;
using Eco.Gameplay.Utils;
using Eco.Gameplay.Civics.Demographics;

namespace Eco.RM.WorldPeace.Federation;

[RequireComponent(typeof(SettlementFoundationComponent))]
[Serialized, Priority(100)]
public class GlobalFederationComponent : WorldObjectComponent
{
    public SettlementFoundationComponent SettlementFoundationComponent => Parent.GetOrCreateComponent<SettlementFoundationComponent>();
    public Settlement Settlement => SettlementFoundationComponent.Settlement;
    public Color InfluenceColor { get; set; } = new Color(RMWorldPeacePlugin.Config.FederationColor[0], RMWorldPeacePlugin.Config.FederationColor[1], RMWorldPeacePlugin.Config.FederationColor[2]);

    public override void Initialize()
    {
        base.Initialize();
        if (Settlement.Founded == false)
        {
            Settlement.FoundSettlement([Parent.Creator]);
            Settlement.Leader?.SetName(Parent.Creator.Player, $"{RMWorldPeacePlugin.Config.FederationName} Admin");
            Settlement.Leader?.SetProposedState(ProposableState.Active);
            if (Settlement.ImmigrationPolicy != null)
            {
                Settlement.ImmigrationPolicy.PropertyHeirWhenCitizensLeave = DemographicManager.Obj.Get(SpecialDemographics.Admins);
                Settlement.ImmigrationPolicy.AllowChildSettlementsToSecede = false;
                Settlement.ImmigrationPolicy.SetProposedState(ProposableState.Active);
            }
            if (Settlement.ElectionProcess != null)
            {;
                Settlement.ElectionProcess.Voters = Settlement.Creator;
                Settlement.ElectionProcess.Vetoers = Settlement.Creator;
                Settlement.ElectionProcess.WhoCanStartElections = Settlement.Creator;
                Settlement.ElectionProcess.MaximumElectionHours = 0.5f;
                Settlement.ElectionProcess.PercentOfAllVotersToInstantWin = 1;
                Settlement.ElectionProcess.SetProposedState(ProposableState.Active);
            }
            if (Settlement.Constitution != null)
            {
                Settlement.Constitution.SetProposedState(ProposableState.Active);
            }
            Settlement.Citizenship.QueueCitizensCacheUpdate();
            UserManager.Users.MailLoc($"A new {Text.InfoLight(Settlement.SettlementType.DisplayName)} has been founded, {Settlement.MarkedUpName}! {'\n' + Settlement.FounderDesc()}", NotificationCategory.Settlements);
            Parent.CloseUIForAll(true);
        }
        Settlement.Influence.Color = InfluenceColor;
        Settlement.Influence.InfluenceWholeWorld = true;
        Settlement.SetName(Parent.Creator.Player, RMWorldPeacePlugin.Config.FederationName);
        Parent.GetDeed().SetName(Parent.Creator.Player, $"{RMWorldPeacePlugin.Config.FederationName} Deed");
    }
}