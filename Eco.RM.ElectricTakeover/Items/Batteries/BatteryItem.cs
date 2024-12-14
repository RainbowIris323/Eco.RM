using Eco.Core.Controller;
using System.ComponentModel;
using Eco.Core.Items;
using Eco.Gameplay.Items;
using Eco.Shared.Serialization;
using Eco.Shared.Localization;

namespace Eco.RM.ElectricTakeover.Items;

[Serialized]
[LocDisplayName("Battery")]
public partial class Battery : Item { }

[Serialized]
[Tag("Battery")]
[MaxStackSize(1)]
public abstract class BatteryItem : Item, IController, INotifyPropertyChanged
{
    public abstract Type BatteryType { get; }
    [Serialized, Notify] public float Charge { get; set; } = 0f;
    [Serialized] public float MaxCharge { get; set; } = 0f;
    [Serialized] public float ChargeRate { get; set; } = 0f;
    [Serialized] public float DischargeRate { get; set; } = 0f;

    #region IController

    public event PropertyChangedEventHandler? PropertyChanged;
    int controllerID;
    public ref int ControllerID => ref controllerID;

    #endregion

    public override Item Clone()
    {
        var item = (BatteryItem)Activator.CreateInstance(BatteryType);
        item.Charge = Charge;
        item.MaxCharge = MaxCharge;
        item.ChargeRate = ChargeRate;
        item.DischargeRate = DischargeRate;
        return item;
    }
    public override bool IsStackable => false;
    public override Item Merge(Item? mergingInto, int thisCount, int otherCount, bool splittingStack)
    {
        var isOtherEmpty = mergingInto == null || otherCount <= 0;

        if (isOtherEmpty && !splittingStack)
            return this;
        if (IsUnique && splittingStack)
            return isOtherEmpty ? Clone() : mergingInto!;

        return this;
    }
}
