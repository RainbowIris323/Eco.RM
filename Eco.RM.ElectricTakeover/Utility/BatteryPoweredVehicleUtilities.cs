using Eco.Gameplay.Items;
using Eco.Mods.TechTree;
using Eco.RM.ElectricTakeover.Objects;
using Eco.Shared.Utils;
using Eco.World.Blocks;

namespace Eco.RM.ElectricTakeover.Utility;

// A place for misc vehicle utilities
public class BatteryPoweredVehicleUtilities
{
    // Mapping for custom stack sizes in vehicles by vehicle type as key
    // We can have different stack sizes in different vehicles with this
    public static Dictionary<Type, StackLimitTypeRestriction> AdvancedVehicleStackSizeMap = new Dictionary<Type, StackLimitTypeRestriction>();

    static BatteryPoweredVehicleUtilities() => CreateBlockStackSizeMaps();

    private static void CreateBlockStackSizeMaps()
    {
        var blockItems = Item.AllItemsIncludingHidden.Where(x => x is BlockItem).Cast<BlockItem>().ToList();

        // Excavator
        var excavatorMap = new StackLimitTypeRestriction(true, 30);

        excavatorMap.AddListRestriction(blockItems.GetItemsByBlockAttribute<Diggable>(), 20);
        excavatorMap.AddListRestriction(blockItems.GetItemsByBlockAttribute<Minable>(), 80);

        AdvancedVehicleStackSizeMap.Add(typeof(BatteryPoweredExcavatorObject), excavatorMap);

        // Skidsteer (same as excavator currently)
        AdvancedVehicleStackSizeMap.Add(typeof(BatteryPoweredSkidSteerObject), excavatorMap);

        // Tractor
        var tractorMap = new StackLimitTypeRestriction();
        tractorMap.AddListRestriction(ItemUtils.GetItemsByTag("Seeds", "Crop"), 500);
        AdvancedVehicleStackSizeMap.Add(typeof(BatteryPoweredSteamTractorObject), tractorMap);
    }

    public static StackLimitTypeRestriction GetInventoryRestriction(object obj) => AdvancedVehicleStackSizeMap.GetOrDefault(obj.GetType());
}