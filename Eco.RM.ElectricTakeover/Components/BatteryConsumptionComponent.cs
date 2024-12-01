using Eco.Core.Controller;
using System.ComponentModel;
using Eco.Gameplay.Components;
using Eco.Gameplay.Objects;
using Eco.RM.Framework.UI;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;

namespace Eco.RM.ElectricTakeover.Components;

[Serialized]
[NoIcon, AutogenClass]
[RequireComponent(typeof(BatteryInformationComponent))]
[RequireComponent(typeof(OnOffComponent))]
public class BatteryConsumptionComponent : WorldObjectComponent, IController, INotifyPropertyChanged
{
    [SyncToView, Autogen, Notify, ShowFullObject] public TextElement ChargerInformationDisplay { get; } = new(Localizer.DoStr("Charger Information"), "");
    private BatteryInformationComponent BatteryInformationComponent => Parent.GetOrCreateComponent<BatteryInformationComponent>();
    private BatteryStorageComponent BatteryStorageComponent => Parent.GetOrCreateComponent<BatteryStorageComponent>();
    private OnOffComponent OnOffComponent => Parent.GetOrCreateComponent<OnOffComponent>();
    public override bool Enabled => Discharging && OnOffComponent.On;
    public bool Discharging = true;
    public float DischargeRate = 0f;
    public BatteryConsumptionComponent() { }
    public BatteryConsumptionComponent(int numBatterySlots, float dischargeRate) { Initialize(numBatterySlots, dischargeRate); }
    public void Initialize(int numBatterySlots, float dischargeRate)
    {
        BatteryInformationComponent.Initialize(numBatterySlots);
        DischargeRate = dischargeRate;
    }
    public override void Tick()
    {
        if (OnOffComponent.On)
        {
            Discharging = BatteryStorageComponent.ApplyDischarge(DischargeRate);
            return;
        }
        Discharging = false;
    }
    public override void LateTick()
    {
        UpdateInformation();
    }
    public override void Destroy()
    {
        base.Destroy();
    }
    public void UpdateInformation()
    {
        var s = new LocStringBuilder();
        s.AppendLine(Localizer.DoStr($"Discharge Rate: {DischargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Battery Discharge Rate: {BatteryStorageComponent.DischargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Active Batteries: {BatteryStorageComponent.NonEmptyBatteries.Count()}"));
        ChargerInformationDisplay.SetText(s.ToString());
    }
}
