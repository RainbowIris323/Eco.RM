using System.ComponentModel;
using Eco.Core.Controller;
using Eco.Gameplay.Objects;
using Eco.RM.Framework.UI;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Components;

[Serialized]
[NoIcon, AutogenClass]
[LocDisplayName("Battery Information"), LocDescription("Displays information about inserted batteries.")]
[RequireComponent(typeof(BatteryStorageComponent))]
public class BatteryInformationComponent : WorldObjectComponent, IController, INotifyPropertyChanged
{
    [SyncToView, Autogen, Notify, ShowFullObject] public TextElement BatteryInformationDisplay { get; } = new(Localizer.DoStr("Battery Information"), "");
    private BatteryStorageComponent BatteryStorageComponent => Parent.GetOrCreateComponent<BatteryStorageComponent>();
    public BatteryInformationComponent() {}
    public BatteryInformationComponent(int numBatterySlots) { Initialize(numBatterySlots); }
    public void Initialize(int numBatterySlots)
    {
        BatteryStorageComponent.Initialize(numBatterySlots);

        BatteryStorageComponent.OnBatteriesChanged.Add(UpdateInformation);
        BatteryStorageComponent.ChargeChanged.Add(UpdateInformation);

        UpdateInformation();
    }
    public override void Destroy()
    {
        base.Destroy();

        BatteryStorageComponent.OnBatteriesChanged.Remove(UpdateInformation);
        BatteryStorageComponent.ChargeChanged.Remove(UpdateInformation);
    }
    public void UpdateInformation()
    {
        var s = new LocStringBuilder();
        s.AppendLine(Localizer.DoStr($"Charge: {BatteryStorageComponent.Charge}wh"));
        s.AppendLine(Localizer.DoStr($"Max Charge: {BatteryStorageComponent.MaxCharge}wh"));
        s.AppendLine(Localizer.DoStr($"Charge Rate: {BatteryStorageComponent.ChargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Discharge Rate: {BatteryStorageComponent.DischargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Batteries: {BatteryStorageComponent.BatteryCount}"));
        s.AppendLine(Localizer.DoStr($"Full Batteries: {BatteryStorageComponent.BatteryCount - BatteryStorageComponent.NonFullBatteries.Count()}"));
        s.AppendLine(Localizer.DoStr($"Empty Batteries: {BatteryStorageComponent.BatteryCount - BatteryStorageComponent.NonEmptyBatteries.Count()}"));
        BatteryInformationDisplay.SetText(s.ToString());
    }
}