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
[RequireComponent(typeof(PowerGeneratorComponent))]
[RequireComponent(typeof(PowerGridComponent))]
[RequireComponent(typeof(BatteryInformationComponent))]
public class BatteryDischargerComponent : WorldObjectComponent, IController, INotifyPropertyChanged
{
    [SyncToView, Autogen, Notify, ShowFullObject] public TextElement ChargerInformationDisplay { get; } = new(Localizer.DoStr("Charger Information"), "");
    private BatteryInformationComponent BatteryInformationComponent => Parent.GetOrCreateComponent<BatteryInformationComponent>();
    private PowerGridComponent PowerGridComponent => Parent.GetOrCreateComponent<PowerGridComponent>();
    private PowerGeneratorComponent PowerGeneratorComponent => Parent.GetOrCreateComponent<PowerGeneratorComponent>();
    private BatteryStorageComponent BatteryStorageComponent => Parent.GetOrCreateComponent<BatteryStorageComponent>();
    public float MaxDischargeRate = 0f;
    public string EnergyTypeString = "Electric";
    public float CurrentDischargeRate => GetDischargeShare();
    public BatteryDischargerComponent() { }
    public BatteryDischargerComponent(int numBatterySlots, float maxDischargeRate, float powerGridRadius, bool isMechanical = false) { Initialize(numBatterySlots, maxDischargeRate, powerGridRadius, isMechanical); }
    public void Initialize(int numBatterySlots, float maxDischargeRate, float powerGridRadius, bool isMechanical = false)
    {
        BatteryInformationComponent.Initialize(numBatterySlots);
        PowerGridComponent.Initialize(powerGridRadius, isMechanical ? new MechanicalPower() : new ElectricPower());
        if (isMechanical) EnergyTypeString = "Mechanical";
        PowerGeneratorComponent.Initialize(0);
        MaxDischargeRate = maxDischargeRate;
    }
    public override void Tick()
    {
        BatteryStorageComponent.ApplyDischarge(CurrentDischargeRate);
    }
    public override void LateTick()
    {
        PowerGeneratorComponent.UpdateJoulesPerSecond(CurrentDischargeRate);
        UpdateInformation();
    }
    public override void Destroy()
    {
        base.Destroy();
    }
    public void UpdateInformation()
    {
        var s = new LocStringBuilder();
        s.AppendLine(Localizer.DoStr($"Grid Demand: {PowerGridComponent.PowerGrid.EnergyDemand}w"));
        s.AppendLine(Localizer.DoStr($"Non Battery Supply: {GetNonBatteryEnergySupply()}w"));
        s.AppendLine(Localizer.DoStr($"Current Production Rate: {CurrentDischargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Machine Discharge Rate: {MaxDischargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Battery Discharge Rate: {BatteryStorageComponent.DischargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Active Batteries: {BatteryStorageComponent.NonEmptyBatteries.Count()}"));
        s.AppendLine(Localizer.DoStr($"Output Energy Type: {EnergyTypeString}"));
        ChargerInformationDisplay.SetText(s.ToString());
    }
    public IEnumerable<BatteryDischargerComponent> GetBatteryDischargersInGrid()
    {
        var components = new List<BatteryDischargerComponent>();
        PowerGridComponent.PowerGrid.Components.ForEach((component) =>
        {
            component.Parent.TryGetComponent<BatteryDischargerComponent>(out BatteryDischargerComponent? c);
            if (c != null) components.Add(c);
        });
        return components;
    }

    public float GetNonBatteryEnergySupply()
    {
        var energy = PowerGridComponent.PowerGrid.EnergySupply;
        GetBatteryDischargersInGrid().ForEach((component) => energy -= component.PowerGeneratorComponent.JoulesPerSecond);
        return energy;
    }

    public float GetDischargeShare()
    {
        var energyReq = Math.Clamp(PowerGridComponent.PowerGrid.EnergyDemand - GetNonBatteryEnergySupply(), 0, float.MaxValue);
        var components = GetBatteryDischargersInGrid();
        var totalDischargeRate = 0f;
        foreach (var component in components) totalDischargeRate += component.BatteryStorageComponent.DischargeRate < component.MaxDischargeRate ? component.BatteryStorageComponent.DischargeRate : component.MaxDischargeRate;
        return energyReq * ((BatteryStorageComponent.DischargeRate < MaxDischargeRate ? BatteryStorageComponent.DischargeRate : MaxDischargeRate) / totalDischargeRate);
    }
}
