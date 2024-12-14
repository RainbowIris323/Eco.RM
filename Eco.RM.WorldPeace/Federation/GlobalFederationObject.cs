using System.ComponentModel;
using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Civics;
using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Settlements;
using Eco.Gameplay.Settlements.Components;
using Eco.Gameplay.Systems;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Shared.Items;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.WorldPeace.Federation;

[Serialized]
[Tag("Hidden")]
[RequireComponent(typeof(PropertyAuthComponent))]
[RequireComponent(typeof(GlobalFederationComponent))]
[RequireComponent(typeof(StatusComponent))]
[RequireComponent(typeof(BorderControlComponent))]
[RequireComponent(typeof(NameDataTrackerComponent))]
[RequireComponent(typeof(AuthDataTrackerComponent))]
[RequireComponent(typeof(CitizenRosterComponent))]
[RequireComponent(typeof(SettlementRosterComponent))]
[RequireComponent(typeof(ConstitutionComponent))]
[RelatedFeature(nameof(FeatureConfig.UseSettlementSystem))]
[HasCivicAction(typeof(CivicAction_AddChildSettlement))]
[HasCivicAction(typeof(CivicAction_CedeChildSettlement))]
[HasCivicAction(typeof(CivicAction_DissolveSettlement))]
[HasCivicAction(typeof(CivicAction_CancelSettlementActions))]
[HasCivicAction(typeof(CivicAction_RevokeCitizenship))]
public class GlobalFederationObject : SettlementFoundationObject
{
    public override int SlotCount => 20;
    public override SettlementType SettlementType => new SettlementType(2);
    public virtual Type RepresentedItemType => typeof(GlobalFederationItem);
    protected override void Initialize()
    {
        base.Initialize();
        SetAdminForceEnabled(true);
    }
}

[Serialized]
[LocDisplayName("Global Federation Foundation")]
[LocDescription("World Peace Before It Even Begins.")]
[RelatedFeature(nameof(FeatureConfig.UseSettlementSystem))]
public class GlobalFederationItem : SettlementFoundationItem<GlobalFederationObject>, IPersistentData
{
    protected override SettlementType SettlementType => new SettlementType(2);
    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
}