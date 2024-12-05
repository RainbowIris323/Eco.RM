using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Components.Storage;
using Eco.Gameplay.Items;
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
using Eco.Gameplay.Items.Recipes;
using Eco.RM.Framework.Config;
using Eco.RM.ElectricTakeover.Items;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Battery Powered Steam Truck")]
[LocDescription("A truck that runs on steam.")]
[IconGroup("World Object Minimap")]
[Weight(25000)]
[Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
public partial class BatteryPoweredSteamTruckItem : WorldObjectItem<BatteryPoweredSteamTruckObject>, IPersistentData
{
    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
}

public class BatteryPoweredSteamTruckRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(BatteryPoweredSteamTruckRecipe).Name,
        Assembly = typeof(BatteryPoweredSteamTruckRecipe).AssemblyQualifiedName,
        HiddenName = "Battery Powered Steam Truck",
        LocalizableName = Localizer.DoStr("Battery Powered Steam Truck"),
        IngredientList =
        [
            new RMIngredient(nameof(SteamTruckItem), false, 1, true),
            new RMIngredient(nameof(ImprovedElectricUpgradeKitItem), false, 1, true),

        ],
        ProductList =
        [
            new RMCraftable(nameof(BatteryPoweredSteamTruckItem), 1),
        ],
        BaseExperienceOnCraft = 0,
        BaseLabor = 1,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(AssemblyTableItem),
    };

    static BatteryPoweredSteamTruckRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public BatteryPoweredSteamTruckRecipe()
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
[RepairRequiresSkill(typeof(MechanicsSkill), 2)]
[Ecopedia("Crafted Objects", "Vehicles", subPageName: "Battery Powered Steam Truck Item")]
public partial class BatteryPoweredSteamTruckObject : PhysicsWorldObject, IRepresentsItem
{
    static BatteryPoweredSteamTruckObject()
    {
        AddOccupancy<BatteryPoweredSteamTruckObject>([]);
    }
    public override TableTextureMode TableTexture => TableTextureMode.Metal;
    public override bool PlacesBlocks => false;
    public override LocString DisplayName { get { return Localizer.DoStr("Battery Powered Steam Truck"); } }
    public Type RepresentedItemType { get { return typeof(BatteryPoweredSteamTruckItem); } }
    private BatteryPoweredSteamTruckObject() { }
    protected override void Initialize()
    {
        base.Initialize();         
        GetComponent<CustomTextComponent>().Initialize(200);
        GetComponent<BatteryConsumptionComponent>().Initialize(1, 300);
        GetComponent<VehicleComponent>().HumanPowered(1);
        GetComponent<StockpileComponent>().Initialize(new Vector3i(2,2,3));
        GetComponent<PublicStorageComponent>().Initialize(24, 5000000);
        GetComponent<MinimapComponent>().InitAsMovable();
        GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
        GetComponent<VehicleComponent>().Initialize(ElectricTakeoverConfig.Obj.GetBatteryPoweredVehicleSpeed(18), 3,2);
        GetComponent<VehicleComponent>().FailDriveMsg = Localizer.Do($"You are too hungry to drive {DisplayName}!");
        {
            GetComponent<PartsComponent>().Config(() => LocString.Empty,
            [
                new() { TypeName = nameof(PortableSteamEngineItem), Quantity = 1},
                new() { TypeName = nameof(IronWheelItem), Quantity = 2},
                new() { TypeName = nameof(LightBulbItem), Quantity = 1},
                new() { TypeName = nameof(LubricantItem), Quantity = 1},
            ]);
        }
    }
}