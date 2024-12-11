using Eco.Shared.Items;
using Eco.Shared.Localization;
using Eco.Shared.IoC;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Gameplay.Systems.NewTooltip.TooltipLibraryFiles;
using Eco.RM.ElectricTakeover.Items;
using Eco.Core.Properties;

namespace Eco.RM.ElectricTakeover.Utility;

[TooltipLibrary]
public static class BatteryTooltipLibrary
{

    public static void Initialize()
    {
        ChangeWatcher.WatchPropOnAllInstances(null, typeof(BatteryItem), nameof(BatteryItem.Charge), (a, b) =>
        {
            ServiceHolder<ITooltipSubscriptions>.Obj.MarkTooltipPartDirty(nameof(ChargeTooltip), instance: (BatteryItem)a);
        });
    }

    [NewTooltip(CacheAs.Disabled)]
    public static LocString ChargeTooltip(this BatteryItem item)
    {
        var s = new LocStringBuilder();
        s.AppendLine(Localizer.DoStr($"Current Charge: {item.Charge}wh / {item.MaxCharge}wh"));
        s.AppendLine(Localizer.DoStr($"Max Charge Rate: {item.ChargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Max Discharge Rate: {item.DischargeRate}w"));
        return new TooltipSection(Localizer.DoStr("Battery Info"), s.ToLocString());
    }
}