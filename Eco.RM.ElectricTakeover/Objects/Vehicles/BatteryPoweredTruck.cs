using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Components.Storage;
using Eco.Gameplay.Items;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Skills;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Occupancy;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Shared.Math;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Items;
using static Eco.Gameplay.Components.PartsComponent;
using Eco.Mods.TechTree;
using Eco.RM.ElectricTakeover.Components;
using Eco.RM.ElectricTakeover.Utility;
using Eco.RM.Framework.Resolvers;
using Eco.RM.ElectricTakeover.Items;
using Eco.RM.Framework.Models;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Battery Powered Truck")]
[LocDescription("Modern truck for hauling sizable loads.")]
[IconGroup("World Object Minimap")]
[Weight(25000)]
[Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
public partial class BatteryPoweredTruckItem : WorldObjectItem<BatteryPoweredTruckObject>, IPersistentData
{
    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
}

public class BatteryPoweredTruckRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(BatteryPoweredTruckRecipe).Name,
        Assembly = typeof(BatteryPoweredTruckRecipe).AssemblyQualifiedName,
        HiddenName = "Battery Powered Truck",
        LocalizableName = Localizer.DoStr("Battery Powered Truck"),
        IngredientList =
        [
            new RMIngredient(nameof(TruckItem), false, 1, true),
            new RMIngredient(nameof(AdvancedElectricUpgradeKitItem), false, 1, true),

        ],
        ProductList =
        [
            new RMCraftable(nameof(BatteryPoweredTruckItem), 1),
        ],
        BaseExperienceOnCraft = 0,
        BaseLabor = 1,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(AssemblyTableItem),
    };

    static BatteryPoweredTruckRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public BatteryPoweredTruckRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}

[Serialized]
[RequireComponent(typeof(StandaloneAuthComponent))]
[RequireComponent(typeof(PaintableComponent))]
[RequireComponent(typeof(BatteryConsumptionComponent))]
[RequireComponent(typeof(PublicStorageComponent))]
[RequireComponent(typeof(TailingsReportComponent))]
[RequireComponent(typeof(MovableLinkComponent))]
[RequireComponent(typeof(VehicleComponent))]
[RequireComponent(typeof(CustomTextComponent))]
[RequireComponent(typeof(ModularStockpileComponent))]
[RequireComponent(typeof(MinimapComponent))]           
[RequireComponent(typeof(PartsComponent))]
[RepairRequiresSkill(typeof(IndustrySkill), 2)]
[Ecopedia("Crafted Objects", "Vehicles", subPageName: "Truck Item")]
public partial class BatteryPoweredTruckObject : PhysicsWorldObject, IRepresentsItem
{
    static BatteryPoweredTruckObject()
    {
        AddOccupancy<BatteryPoweredTruckObject>([]);
    }
    public override TableTextureMode TableTexture => TableTextureMode.Metal;
    public override bool PlacesBlocks            => false;
    public override LocString DisplayName { get { return Localizer.DoStr("Battery Powered Truck"); } }
    public Type RepresentedItemType { get { return typeof(BatteryPoweredTruckItem); } }
    private BatteryPoweredTruckObject() { }
    protected override void Initialize()
    {
        base.Initialize();         
        GetComponent<CustomTextComponent>().Initialize(200);
        GetComponent<BatteryConsumptionComponent>().Initialize(1, 250);
        GetComponent<VehicleComponent>().HumanPowered(1);
        GetComponent<StockpileComponent>().Initialize(new Vector3i(2,2,3));
        GetComponent<PublicStorageComponent>().Initialize(36, 8000000);
        GetComponent<MinimapComponent>().InitAsMovable();
        GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
        GetComponent<VehicleComponent>().Initialize(ElectricTakeoverConfig.Obj.GetBatteryPoweredVehicleSpeed(20), 4,2);
        GetComponent<VehicleComponent>().FailDriveMsg = Localizer.Do($"You are too hungry to drive {DisplayName}!");
        {
            GetComponent<PartsComponent>().Config(() => LocString.Empty,
            [
                new() { TypeName = nameof(CombustionEngineItem), Quantity = 1},
                new() { TypeName = nameof(RubberWheelItem), Quantity = 2},
                new() { TypeName = nameof(LightBulbItem), Quantity = 1},
                new() { TypeName = nameof(LubricantItem), Quantity = 2},
            ]);
        }
    }
}