using Eco.Core.Utils;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Occupancy;
using Eco.Gameplay.Pipes.Gases;
using Eco.Gameplay.Pipes.LiquidComponents;
using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.TextLinks;
using Eco.Shared.Serialization;
using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Components.Storage;
using Eco.Gameplay.Systems.Messaging.Notifications;
using Eco.Mods.TechTree;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using Eco.RM.SmogFilter.Utility;

namespace Eco.RM.SmogFilter.Components;

[Serialized]
public class SmogFilterItemData : IClearRequestHandler
{
    [Serialized] public float ProcessedSmog { get; set; }

    public Result TryHandleClearRequest(Player player) => Result.FailLoc($"Cannot clear saved state of {typeof(SmogFilterComponent).UILink()}!");
}


[Serialized]
[RequireComponent(typeof(LiquidConverterComponent))]
[RequireComponent(typeof(LinkComponent))]
[RequireComponent(typeof(InOutLinkedInventoriesComponent))]
[RequireComponent(typeof(StatusComponent))]
[RequireComponent(typeof(MustBeOwnedComponent))]
[Priority(Priority)]
[Ecopedia(null, "Pipe Component"), NoIcon]
public class SmogFilterComponent : WorldObjectComponent, IOperatingWorldObjectComponent, IPersistentData
{
    public const int Priority = PowerGridComponent.Priority + 1;
    [Serialized] public SmogFilterItemData SmogFilterData { get; set; } = new();
    [Serialized] public bool ShutdownFromFullInv;
    public object PersistentData { get => SmogFilterData; set => SmogFilterData = value as SmogFilterItemData ?? new(); }

    private LiquidConverterComponent Converter;
    public StatusElement Status;

    public PeriodicUpdate UpdateThrottle = new PeriodicUpdate(5, true);

    public override void Initialize()
    {
        Status = Parent.GetComponent<StatusComponent>().CreateStatusElement();
        Converter = Parent.GetComponent<LiquidConverterComponent>();
        Converter.Setup(typeof(SmogItem), typeof(OxygenItem), BlockOccupancyType.InputPort, BlockOccupancyType.OutputPort, SmogFilterConfig.Obj.SmogConsumptionRate, SmogFilterConfig.Obj.OxygenProductionRate);
        Converter.In.BufferSize = 1f;
        Converter.OnConvert += Converted;
        Parent.GetComponent<LinkComponent>().OnInventoryContentsChanged.Add(OnStorageUpdated);
    }

    public LocString DisplayStatus => Localizer.Do($"{Text.StyledPercent(SmogFilterData.ProcessedSmog / SmogFilterConfig.Obj.SmogPerCompostBlock)} of {Item.Get("CompostItem").UILink()} currently filtered.");

    public void Converted(float amount)
    {
        SmogFilterData.ProcessedSmog += amount;
        while (SmogFilterData.ProcessedSmog > SmogFilterConfig.Obj.SmogPerCompostBlock)
        {
            var invs = Parent.GetComponent<LinkComponent>().GetSortedLinkedInventories(Parent.Owners);
            if (!invs.TryAddItemNonUnique<CompostItem>())
            {
                Parent.GetComponent<OnOffComponent>().On = false;
                NotificationManager.ServerMessageToAlias(Localizer.Format("{0} disabled, no room left for filtered waste.", Parent.UILink()), Parent.Owners);
                Status.SetStatusMessage(false, Localizer.DoStr("No room for filtered waste."));
                this.Changed(nameof(DisplayStatus));
                Parent.UpdateEnabledAndOperating();
                ShutdownFromFullInv = true;
                return;
            }
            else
            {
                SmogFilterData.ProcessedSmog -= SmogFilterConfig.Obj.SmogPerCompostBlock;
                Status.SetStatusMessage(true, DisplayStatus);
            }
        }

        if (UpdateThrottle.DoUpdate)
            Status.SetStatusMessage(true, DisplayStatus);
    }
    public void OnStorageUpdated()
    {
        if (ShutdownFromFullInv)
        {
            Parent.GetComponent<OnOffComponent>().On = true;
            ShutdownFromFullInv = false;
        }
    }
    public bool Operating => Converter.In.BufferAmount > 0 || Converter.In.LastTickConsumed > 0;
}