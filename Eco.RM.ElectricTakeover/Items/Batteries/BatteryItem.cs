using Eco.Core.Controller;
using System.ComponentModel;
using Eco.Core.Items;
using Eco.Gameplay.Items;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items;

[Serialized]
[Tag("Battery")]
[LocDescription("A device used to store energy for later use")]
[MaxStackSize(1)]
public abstract class BatteryItem : Item, IController, INotifyPropertyChanged
{
    public abstract Type BatteryType { get; }
    [Serialized] public float Charge { get; set; } = 0f;
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
}
