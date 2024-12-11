using Eco.Core.Controller;
using System.ComponentModel;
using Eco.Gameplay.Components;
using Eco.Gameplay.Objects;
using Eco.RM.Framework.UI;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;
using Eco.Gameplay.Items;

namespace Eco.RM.ElectricTakeover.Components;

[Serialized]
[LocDisplayName("Battery Buffer")]
public partial class BatteryBuffer : Item { }

[Serialized]
[AutogenClass, HasIcon("BatteryBuffer")]
[RequireComponent(typeof(PowerGeneratorComponent))]
[RequireComponent(typeof(PowerConsumptionComponent))]
[RequireComponent(typeof(PowerGridComponent))]
[RequireComponent(typeof(BatteryStorageComponent))]
[LocDisplayName("Battery Buffer"), LocDescription("Allows a connected power grid to use energy from a internal battery storage when the grid is low on energy and recharge when there is excess.")]
public partial class BatteryBufferComponent : WorldObjectComponent, IController, INotifyPropertyChanged
{
    [SyncToView, Autogen, Notify, ShowFullObject] public TextElement BufferInformationDisplay { get; } = new(Localizer.DoStr("Buffer Information"), "");
    public PowerGridComponent PowerGridComponent => Parent.GetOrCreateComponent<PowerGridComponent>();
    public PowerGeneratorComponent PowerGeneratorComponent => Parent.GetOrCreateComponent<PowerGeneratorComponent>();
    public BatteryStorageComponent BatteryStorageComponent => Parent.GetOrCreateComponent<BatteryStorageComponent>();
    public PowerConsumptionComponent PowerConsumptionComponent => Parent.GetOrCreateComponent<PowerConsumptionComponent>();
    public float NetEnergyShare { get; private set; }
    public bool ChargingEnabled { get; private set; } = true;
    public bool DischargingEnabled { get; private set; } = true;
    public BatteryBufferComponent() { }
    public BatteryBufferComponent(int numBatterySlots, float powerGridRadius, bool isMechanical = false, bool chargingEnabled = true, bool dischargingEnabled = true) { Initialize(numBatterySlots, powerGridRadius, isMechanical, chargingEnabled, dischargingEnabled); }
    public void Initialize(int numBatterySlots, float powerGridRadius, bool isMechanical = false, bool chargingEnabled = true, bool dischargingEnabled = true)
    {
        BatteryStorageComponent.Initialize(numBatterySlots);
        PowerGridComponent.Initialize(powerGridRadius, isMechanical ? new MechanicalPower() : new ElectricPower());
        PowerGeneratorComponent.Initialize(0);
        PowerConsumptionComponent.Initialize(0);
        ChargingEnabled = chargingEnabled;
        DischargingEnabled = dischargingEnabled;
    }
    public override void Tick()
    {
        NetEnergyShare = GetNetEnergyShare();
        BatteryStorageComponent.ApplyNetEnergy(NetEnergyShare);
    }
    public override void LateTick()
    {
        PowerGeneratorComponent.UpdateJoulesPerSecond(DischargingEnabled ? BatteryStorageComponent.DischargeRate : 0);
        if (NetEnergyShare > 0)
        {
            PowerConsumptionComponent.OverridePowerConsumption(Math.Abs(NetEnergyShare));
        }
        else
        {
            PowerConsumptionComponent.OverridePowerConsumption(0);
        }
        UpdateInformation();
    }
    public void UpdateInformation()
    {
        var s = new LocStringBuilder();
        s.AppendLine(Localizer.DoStr($"Charge: {BatteryStorageComponent.Charge}wh / {BatteryStorageComponent.MaxCharge}wh"));
        s.AppendLine(Localizer.DoStr($"Net Energy: {NetEnergyShare}w"));
        s.AppendLine(Localizer.DoStr($"Max Charge Rate: {BatteryStorageComponent.ChargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Max Discharge Rate: {BatteryStorageComponent.DischargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Battery Count: {BatteryStorageComponent.BatteryCount}"));
        s.AppendLine(Localizer.DoStr($"Full Batteries: {BatteryStorageComponent.BatteryCount - BatteryStorageComponent.NonFullBatteries.Count()}"));
        s.AppendLine(Localizer.DoStr($"Empty Batteries: {BatteryStorageComponent.BatteryCount - BatteryStorageComponent.NonEmptyBatteries.Count()}"));
        s.AppendLine(Localizer.DoStr($"Charging Enabled: {(ChargingEnabled ? "True" : "False")}"));
        s.AppendLine(Localizer.DoStr($"Discharging Enabled: {(DischargingEnabled ? "True" : "False")}"));
        BufferInformationDisplay.SetText(s.ToString());
    }
    public IEnumerable<BatteryBufferComponent> GetBatteryBuffersInGrid()
    {
        var components = new List<BatteryBufferComponent>();
        PowerGridComponent.PowerGrid.Components.ForEach((component) =>
        {
            component.Parent.TryGetComponent<BatteryBufferComponent>(out BatteryBufferComponent? c);
            if (c != null) components.Add(c);
        });
        return components;
    }
    public float GetNetEnergyShare()
    {
        var components = GetBatteryBuffersInGrid();
        var supply = PowerGridComponent.PowerGrid.EnergySupply;
        components.ForEach((component) => supply -= component.PowerGeneratorComponent.JoulesPerSecond);
        var demand = PowerGridComponent.PowerGrid.EnergyDemand;
        components.ForEach((component) => demand -= component.PowerConsumptionComponent.JoulesPerSecond);
        var net = supply - demand;
        if (net < 0)
        {
            if (!DischargingEnabled) return 0;
            var totalDischargeRate = 0f;
            components.ForEach((component) => totalDischargeRate += component.DischargingEnabled ? component.BatteryStorageComponent.DischargeRate : 0f);
            if (totalDischargeRate < Math.Abs(net)) return 0;
            return BatteryStorageComponent.DischargeRate / totalDischargeRate * net;
        }
        else if (net > 0)
        {
            if (!ChargingEnabled) return 0;
            var totalChargeRate = 0f;
            components.ForEach((component) => totalChargeRate += component.ChargingEnabled ? component.BatteryStorageComponent.ChargeRate : 0f);
            if (totalChargeRate < net) return BatteryStorageComponent.ChargeRate;
            return BatteryStorageComponent.ChargeRate / totalChargeRate * net;
        }
        else return 0;
    }
}
