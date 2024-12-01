using Eco.Core.Controller;
using System.ComponentModel;
using Eco.Gameplay.Components;
using Eco.Gameplay.Objects;
using Eco.RM.Framework.UI;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Components;

[Serialized]
[NoIcon, AutogenClass]
[RequireComponent(typeof(PowerConsumptionComponent))]
[RequireComponent(typeof(PowerGridComponent))]
[RequireComponent(typeof(BatteryInformationComponent))]
public class BatteryChargerComponent : WorldObjectComponent, IController, INotifyPropertyChanged
{
    [SyncToView, Autogen, Notify, ShowFullObject] public TextElement ChargerInformationDisplay { get; } = new(Localizer.DoStr("Charger Information"), "");
    private BatteryInformationComponent BatteryInformationComponent => Parent.GetOrCreateComponent<BatteryInformationComponent>();
    private PowerGridComponent PowerGridComponent => Parent.GetOrCreateComponent<PowerGridComponent>();
    private PowerConsumptionComponent PowerConsumptionComponent => Parent.GetOrCreateComponent<PowerConsumptionComponent>();
    private BatteryStorageComponent BatteryStorageComponent => Parent.GetOrCreateComponent<BatteryStorageComponent>();
    public float MaxChargeRate = 0f;
    public float ConversionRate = 1f;
    public string EnergyTypeString = "Electric";
    public float CurrentChargeRate => BatteryStorageComponent.ChargeRate < MaxChargeRate ? BatteryStorageComponent.ChargeRate : MaxChargeRate;
    public float CurrentConsumptionRate => CurrentChargeRate + CurrentChargeRate * (100 - ConversionRate * 100);
    public BatteryChargerComponent() { }
    public BatteryChargerComponent(int numBatterySlots, float maxChargeRate, float powerGridRadius, float conversionRate = 1f, bool isMechanical = false) { Initialize(numBatterySlots, maxChargeRate, powerGridRadius, conversionRate, isMechanical); }
    public void Initialize(int numBatterySlots, float maxChargeRate, float powerGridRadius, float conversionRate = 1f, bool isMechanical = false)
    {
        BatteryInformationComponent.Initialize(numBatterySlots);
        PowerGridComponent.Initialize(powerGridRadius, isMechanical ? new MechanicalPower() : new ElectricPower());
        if (isMechanical) EnergyTypeString = "Mechanical";
        PowerConsumptionComponent.Initialize(0);
        MaxChargeRate = maxChargeRate;
        UpdateInformation();
    }
    public override void Tick()
    {
        BatteryStorageComponent.ApplyCharge(CurrentChargeRate);
    }
    public override void LateTick()
    {
        PowerConsumptionComponent.OverridePowerConsumption(CurrentConsumptionRate);
        UpdateInformation();
    }
    public override void Destroy()
    {
        base.Destroy();
    }
    public void UpdateInformation()
    {
        var s = new LocStringBuilder();
        s.AppendLine(Localizer.DoStr($"Current Charge Rate: {CurrentChargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Machine Charge Rate: {MaxChargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Battery Charge Rate: {BatteryStorageComponent.ChargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Active Batteries:    {BatteryStorageComponent.NonFullBatteries.Count()}"));
        s.AppendLine(Localizer.DoStr($"Energy Loss:         {100 - ConversionRate * 100}%"));
        s.AppendLine(Localizer.DoStr($"Input Energy:        {EnergyTypeString}"));
        s.AppendLine(Localizer.DoStr($"Energy Consumption:  {CurrentConsumptionRate}w"));
        ChargerInformationDisplay.SetText(s.ToString());
    }
}
