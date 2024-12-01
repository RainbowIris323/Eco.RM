using Eco.Core.Utils;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Components.Storage;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.RM.ElectricTakeover.Items;
using Eco.Shared.IoC;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Components;

[Serialized]
[RequireComponent(typeof(PropertyAuthComponent))]
[LocDisplayName("Battery Storage"), LocDescription("A storage for active batteries.")]
public class BatteryStorageComponent : PublicStorageComponent
{
    ///<summary>An event called whenever the batteries within the storage are added or removed.</summary>
    public ThreadSafeAction OnBatteriesChanged = new();
    public ThreadSafeAction ChargeChanged = new();
    public BatteryStorageComponent() { }
    public BatteryStorageComponent(int numBatterySlots) { Initialize(numBatterySlots); }
    public float MaxCharge { get; private set; } = 0f;
    public float ChargeRate { get; private set; } = 0f;
    public float DischargeRate { get; private set; } = 0f;
    private float _Charge = 0f;
    public float Charge { get => _Charge; private set
        {
            if (value == _Charge) return;
            _Charge = value;
            ChargeChanged.Invoke();
        }
    }
    public int BatteryCount { get; private set; } = 0;
    public IEnumerable<BatteryItem> NonFullBatteries => GetBatteries().Where((a) => a.Charge < a.MaxCharge);
    public IEnumerable<BatteryItem> NonEmptyBatteries => GetBatteries().Where((a) => a.Charge > 0);
    public new void Initialize(int numBatterySlots)
    {
        base.Initialize(numBatterySlots);
        base.Inventory.RemoveAllRestrictions();
        base.Inventory.AddInvRestriction(new TagRestriction("Battery"));
        base.Inventory.OnChanged.Add(BatteriesChanged);
        UpdateBatteryInformation();
        OnBatteriesChanged.Invoke();
    }
    public override void Destroy()
    {
        OnBatteriesChanged.Clear();
        base.Inventory.OnChanged.Remove(BatteriesChanged);

        base.Destroy();
    }
    ///<summary>Gets all batteries within the storage.</summary>
    ///<returns>A list of the batteries within the storage.</returns>
    public IEnumerable<BatteryItem> GetBatteries()
    {
        var batteries = new List<BatteryItem>();
        foreach (var stack in base.Inventory.NonEmptyStacks) batteries.Add((BatteryItem)stack.Item);
        return batteries;
    }
    public void UpdateBatteryInformation()
    {
        var maxCharge = 0f;
        var chargeRate = 0f;
        var dischargeRate = 0f;
        var charge = 0f;
        var batteryCount = 0;
        foreach (var battery in GetBatteries())
        {
            maxCharge += battery.MaxCharge;
            if (battery.Charge < battery.MaxCharge) chargeRate += battery.ChargeRate;
            if (battery.Charge > 0) dischargeRate += battery.DischargeRate;
            charge += battery.Charge;
            batteryCount += 1;
        }
        MaxCharge = maxCharge;
        ChargeRate = chargeRate;
        DischargeRate = dischargeRate;
        BatteryCount = batteryCount;
        Charge = charge;
    }
    ///<summary>Invokes the <c>BatteryStorageComponent.OnBatteriesChanged</c> event when a battery is added or removed.</summary>
    public void BatteriesChanged(User user)
    {
        UpdateBatteryInformation();
        OnBatteriesChanged.Invoke();
    }
    public bool ApplyDischarge(float watts)
    {
        if (watts > DischargeRate) return false;
        var hours = ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime / 3600;
        var wattHours = watts * hours;
        var perBattery = wattHours / NonEmptyBatteries.Count();
        var skippedDischarge = 0f;
        foreach (var battery in NonEmptyBatteries)
        {
            if (battery.Charge < perBattery) { 
                skippedDischarge += perBattery - battery.Charge; 
                battery.Charge = 0;
                continue;
            }
            battery.Charge -= perBattery;
        }
        UpdateBatteryInformation();
        if (skippedDischarge > 0f) return ApplyDischarge(skippedDischarge / hours);
        return true;
    }
    public bool ApplyCharge(float watts)
    {
        if (watts > ChargeRate) return false;
        var hours = ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime / 3600;
        var wattHours = watts * hours;
        var perBattery = wattHours / NonFullBatteries.Count();
        var skippedCharge = 0f;
        foreach (var battery in NonFullBatteries)
        {
            if (battery.MaxCharge - battery.Charge < perBattery)
            {
                skippedCharge += perBattery - (battery.MaxCharge - battery.Charge);
                battery.Charge = battery.MaxCharge;
                continue;
            }
            battery.Charge += perBattery;
        }
        UpdateBatteryInformation();
        if (skippedCharge > 0f) return ApplyCharge(skippedCharge / hours);
        return true;
    }
}