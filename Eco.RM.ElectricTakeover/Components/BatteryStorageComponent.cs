using Eco.Core.Controller;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Components.Storage;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Players;
using Eco.RM.ElectricTakeover.Items;
using Eco.Shared.IoC;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;

namespace Eco.RM.ElectricTakeover.Components;

[Serialized]
[HasIcon("BatteryStorageComponent")]
[RequireComponent(typeof(PropertyAuthComponent))]
[LocDisplayName("Battery Storage"), LocDescription("A storage for active batteries.")]
public class BatteryStorageComponent : StorageComponent
{
    public BatteryStorageComponent() { }
    public BatteryStorageComponent(int numBatterySlots) { Initialize(numBatterySlots); }
    [Serialized] public Inventory Storage { get; private set; }
    public override Inventory Inventory => Storage;
    public float MaxCharge { get; private set; } = 0f;
    public float ChargeRate { get; private set; } = 0f;
    public float DischargeRate { get; private set; } = 0f;
    public float Charge { get; private set; } = 0f;
    public int BatteryCount { get; private set; } = 0;
    public IEnumerable<BatteryItem> NonFullBatteries => GetBatteries().Where((a) => a.Charge < a.MaxCharge);
    public IEnumerable<BatteryItem> NonEmptyBatteries => GetBatteries().Where((a) => a.Charge > 0);

    public void Initialize(int numBatterySlots)
    {
        Storage ??= new AuthorizationInventory(numBatterySlots);
        Storage.SetOwner(Parent);
        Inventory.AddInvRestriction(new TagRestriction("Battery"));
        Inventory.OnChanged.Add(BatteriesChanged);
        UpdateBatteryInformation();
    }
    public override void Destroy()
    {
        Inventory.OnChanged.Remove(BatteriesChanged);
        base.Destroy();
    }
    ///<summary>Gets all batteries within the storage.</summary>
    ///<returns>A list of the batteries within the storage.</returns>
    public IEnumerable<BatteryItem> GetBatteries()
    {
        var batteries = new List<BatteryItem>();
        foreach (var stack in Inventory.NonEmptyStacks) batteries.Add((BatteryItem)stack.Item);
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
    public void BatteriesChanged(User user)
    {
        UpdateBatteryInformation();
    }
    public bool ApplyNetEnergy(float watts)
    {
        if (watts == 0f) return true;
        var hours = ServiceHolder<IWorldObjectManager>.Obj.TickDeltaTime / 3600;
        var netWattHours = watts * hours;
        var skip = 0f;
        if (watts > 0f)
        {
            if (watts > ChargeRate) return false;
            NonFullBatteries.ForEach((battery) =>
            {
                var netEnergyShare = netWattHours * (battery.ChargeRate / ChargeRate);
                if (netEnergyShare + battery.Charge > battery.MaxCharge)
                {
                    skip += netEnergyShare + battery.Charge - battery.MaxCharge;
                    battery.Charge = battery.MaxCharge;
                    return;
                }
                battery.Charge += netEnergyShare;
            });
        }
        if (watts < 0f)
        {
            if (Math.Abs(watts) > DischargeRate) return false;
            NonEmptyBatteries.ForEach((battery) =>
            {
                var netEnergyShare = netWattHours * (battery.DischargeRate / DischargeRate);
                if (netEnergyShare + battery.Charge < 0)
                {
                    skip += netEnergyShare + battery.Charge;
                    battery.Charge = 0;
                    return;
                }
                battery.Charge += netEnergyShare;
            });
        }
        UpdateBatteryInformation();
        if (skip != 0f) return ApplyNetEnergy(skip / hours);
        return true;
    }
}