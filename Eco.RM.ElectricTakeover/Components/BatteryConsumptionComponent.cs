using Eco.Core.Controller;
using System.ComponentModel;
using Eco.Gameplay.Objects;
using Eco.RM.Framework.UI;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;

namespace Eco.RM.ElectricTakeover.Components;

[Serialized]
[LocDisplayName("Battery Consumption")]
public partial class BatteryConsumption : Item { }

[Serialized]
[AutogenClass, HasIcon("BatteryConsumption")]
[RequireComponent(typeof(BatteryStorageComponent))]
[RequireComponent(typeof(StatusComponent))]
[LocDisplayName("Battery Consumption"), LocDescription("Allows objects to run from a internal battery storage.")]
public class BatteryConsumptionComponent : WorldObjectComponent, IController, INotifyPropertyChanged, IOperatingWorldObjectComponent
{
    [SyncToView, Autogen, Notify, ShowFullObject] public TextElement ChargerInformationDisplay { get; } = new(Localizer.DoStr("Battery Information"), "");
    public BatteryStorageComponent BatteryStorageComponent => Parent.GetOrCreateComponent<BatteryStorageComponent>();
    public VehicleComponent VehicleComponent => Parent.GetComponent<VehicleComponent>();
    public bool Discharging = false;
    public float DischargeRate = 0f;
    public override bool Enabled => BatteryStorageComponent.Charge > 0 && BatteryStorageComponent.DischargeRate > DischargeRate;
    public bool Operating => Enabled;
    [SyncToView] public new bool ForceActiveTab { get; internal set; } = true;
    public StatusElement status;
    public BatteryConsumptionComponent() { }
    public BatteryConsumptionComponent(int numBatterySlots, float dischargeRate) { Initialize(numBatterySlots, dischargeRate); }
    public void Initialize(int numBatterySlots, float dischargeRate)
    {
        BatteryStorageComponent.Initialize(numBatterySlots);
        DischargeRate = dischargeRate;
        status = Parent.GetOrCreateComponent<StatusComponent>().CreateStatusElement();
        UpdateStatus();
    }
    public override void Tick()
    {
        if (Parent.Operating)
        {
            Discharging = BatteryStorageComponent.ApplyNetEnergy(-DischargeRate);
            return;
        }
        Discharging = false;
    }
    public override void LateTick()
    {
        UpdateInformation();
        if (VehicleComponent == null) return;
        if (Enabled) return;
        if (VehicleComponent.Driver == null) return;
        VehicleComponent.Driver.ErrorLoc($"No charge availible!");
        VehicleComponent.Dismount(VehicleComponent.Driver.ID);
    }
    public void UpdateInformation()
    {
        var s = new LocStringBuilder();
        s.AppendLine(Localizer.DoStr($"Discharge Rate: {DischargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Operating: {(Discharging ? "True" : "False")}"));
        s.AppendLine(Localizer.DoStr($"Battery Discharge Rate: {BatteryStorageComponent.DischargeRate}w"));
        s.AppendLine(Localizer.DoStr($"Current Charge: {BatteryStorageComponent.Charge}w"));
        s.AppendLine(Localizer.DoStr($"Active Batteries: {BatteryStorageComponent.NonEmptyBatteries.Count()}"));
        ChargerInformationDisplay.SetText(s.ToString());
    }
    public void UpdateStatus() => status.SetStatusMessage(Enabled, Localizer.DoStr("Has availible charge."), BatteryStorageComponent.Charge <= 0 ? Localizer.DoStr("No availible charge.") : (BatteryStorageComponent.Charge <= 0 ? Localizer.DoStr("Low battery discharge rate.") : Localizer.DoStr("ERROR")));
}
