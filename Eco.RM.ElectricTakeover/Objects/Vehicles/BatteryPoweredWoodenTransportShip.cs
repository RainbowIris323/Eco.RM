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
[LocDisplayName("Battery Powered Wooden Transport Ship")]
[LocDescription("")]
[IconGroup("World Object Minimap")]
[Weight(20000)]
[WaterPlaceable]
[Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
public partial class BatteryPoweredWoodenTransportShipItem : WorldObjectItem<BatteryPoweredWoodenTransportShipObject>, IPersistentData
{
    public float InteractDistance => DefaultInteractDistance.WaterPlacement;
    public bool ShouldHighlight(Type block) => false;
    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
}

public class BatteryPoweredWoodenTransportShipRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(BatteryPoweredWoodenTransportShipRecipe).Name,
        Assembly = typeof(BatteryPoweredWoodenTransportShipRecipe).AssemblyQualifiedName,
        HiddenName = "Battery Powered Wooden Transport Ship",
        LocalizableName = Localizer.DoStr("Battery Powered Wooden Transport Ship"),
        IngredientList =
        [
            new RMIngredient(nameof(WoodenTransportShipItem), false, 1, true),
            new RMIngredient(nameof(ImprovedElectricUpgradeKitItem), false, 1, true),

        ],
        ProductList =
        [
            new RMCraftable(nameof(BatteryPoweredWoodenTransportShipItem), 1),
        ],
        BaseExperienceOnCraft = 0,
        BaseLabor = 1,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(AssemblyTableItem),
    };

    static BatteryPoweredWoodenTransportShipRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public BatteryPoweredWoodenTransportShipRecipe()
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
[RequireComponent(typeof(BatteryConsumptionComponent))]
[RequireComponent(typeof(PublicStorageComponent))]
[RequireComponent(typeof(TailingsReportComponent))]
[RequireComponent(typeof(MovableLinkComponent))]
[RequireComponent(typeof(VehicleComponent))]
[RequireComponent(typeof(BoatComponent))]
[RequireComponent(typeof(LadderComponent))]
[RequireComponent(typeof(MinimapComponent))]           
[RequireComponent(typeof(PartsComponent))]
[RepairRequiresSkill(typeof(ShipwrightSkill), 2)]
[Ecopedia("Crafted Objects", "Vehicles", subPageName: "Battery Powered Wooden Transport Ship Item")]
public partial class BatteryPoweredWoodenTransportShipObject : PhysicsWorldObject, IRepresentsItem
{
    static BatteryPoweredWoodenTransportShipObject()
    {
        AddOccupancy<BatteryPoweredWoodenTransportShipObject>([]);
    }
    public override float InteractDistance => DefaultInteractDistance.WaterPlacement;
    public override TableTextureMode TableTexture => TableTextureMode.Wood;
    public override bool PlacesBlocks            => false;
    public override LocString DisplayName { get { return Localizer.DoStr("Wooden Transport Ship"); } }
    public Type RepresentedItemType { get { return typeof(BatteryPoweredWoodenTransportShipItem); } }
    private BatteryPoweredWoodenTransportShipObject() { }
    protected override void Initialize()
    {
        base.Initialize();
        GetComponent<BatteryConsumptionComponent>().Initialize(1, 150);
        GetComponent<VehicleComponent>().HumanPowered(0.5f);
        GetComponent<LinkComponent>().Initialize(15);
        GetComponent<PublicStorageComponent>().Initialize(48, 14000000);
        GetComponent<MinimapComponent>().InitAsMovable();
        GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
        GetComponent<VehicleComponent>().Initialize(ElectricTakeoverConfig.Obj.GetBatteryPoweredVehicleSpeed(10), 2,7, null, true);
        GetComponent<BoatComponent>().Size = BoatComponent.BoatSize.Large;
        GetComponent<VehicleComponent>().FailDriveMsg = Localizer.Do($"You are too hungry to drive {DisplayName}!");
        {
            GetComponent<PartsComponent>().Config(() => LocString.Empty,
            [
                new() { TypeName = nameof(WoodenHullPlanksItem), Quantity = 4},
                new() { TypeName = nameof(PortableSteamEngineItem), Quantity = 1},
                new() { TypeName = nameof(LubricantItem), Quantity = 2},
            ]);
        }
    }
}