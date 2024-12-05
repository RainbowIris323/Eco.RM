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
using Eco.Gameplay.Systems.Exhaustion;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.Shared.Items;
using Eco.Mods.Components.VehicleModules;
using static Eco.Gameplay.Components.PartsComponent;
using Eco.Mods.TechTree;
using Eco.RM.ElectricTakeover.Components;
using Eco.RM.ElectricTakeover.Utility;
using Eco.RM.Framework.Config;
using Eco.RM.ElectricTakeover.Items;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Battery Powered Medium Fishing Trawler")]
[LocDescription("")]
[IconGroup("World Object Minimap")]
[Weight(25000)]
[WaterPlaceable]
[Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
public partial class BatteryPoweredMediumFishingTrawlerItem : WorldObjectItem<BatteryPoweredMediumFishingTrawlerObject>, IPersistentData
{
    public float InteractDistance => DefaultInteractDistance.WaterPlacement;
    public bool ShouldHighlight(Type block) => false;
    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
}

public class BatteryPoweredMediumFishingTrawlerRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(BatteryPoweredMediumFishingTrawlerRecipe).Name,
        Assembly = typeof(BatteryPoweredMediumFishingTrawlerRecipe).AssemblyQualifiedName,
        HiddenName = "Battery Powered Medium Fishing Trawler",
        LocalizableName = Localizer.DoStr("Battery Powered Medium Fishing Trawler"),
        IngredientList =
        [
            new RMIngredient(nameof(MediumFishingTrawlerItem), false, 1, true),
            new RMIngredient(nameof(ImprovedElectricUpgradeKitItem), false, 1, true),

        ],
        ProductList =
        [
            new RMCraftable(nameof(BatteryPoweredMediumFishingTrawlerItem), 1),
        ],
        BaseExperienceOnCraft = 0,
        BaseLabor = 1,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(AssemblyTableItem),
    };

    static BatteryPoweredMediumFishingTrawlerRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public BatteryPoweredMediumFishingTrawlerRecipe()
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
[RequireComponent(typeof(FishingComponent))]
[RequireComponent(typeof(ModularVehicleComponent))]
[RequireComponent(typeof(MinimapComponent))]           
[RequireComponent(typeof(PartsComponent))]
[RepairRequiresSkill(typeof(ShipwrightSkill), 5)]
[ExhaustableUnlessOverridenVehicle]
[Ecopedia("Crafted Objects", "Vehicles", subPageName: "Battery Powered Medium Fishing Trawler Item")]
public partial class BatteryPoweredMediumFishingTrawlerObject : PhysicsWorldObject, IRepresentsItem
{
    static BatteryPoweredMediumFishingTrawlerObject()
    {
        AddOccupancy<BatteryPoweredMediumFishingTrawlerObject>([]);
    }
    public override float InteractDistance => DefaultInteractDistance.WaterPlacement;
    public override TableTextureMode TableTexture => TableTextureMode.Metal;
    public override bool PlacesBlocks => false;
    public override LocString DisplayName { get { return Localizer.DoStr("Battery Powered Medium Fishing Trawler"); } }
    public Type RepresentedItemType { get { return typeof(BatteryPoweredMediumFishingTrawlerItem); } }
    private BatteryPoweredMediumFishingTrawlerObject() { }
    private static readonly Type[] SegmentTypeList = Array.Empty<Type>();
    private static readonly Type[] AttachmentTypeList =
    [
        typeof(FlaxTrawlerNetItem),
        typeof(NylonTrawlerNetItem),
    ];
    protected override void Initialize()
    {
        base.Initialize();         
        GetComponent<BatteryConsumptionComponent>().Initialize(1, 150);
        GetComponent<VehicleComponent>().HumanPowered(0.5f);
        GetComponent<PublicStorageComponent>().Initialize(8, 1400000);
        GetComponent<FishingComponent>().Initialize(8, 1400000);
        GetComponent<ModularVehicleComponent>().Initialize(0, 1, SegmentTypeList, AttachmentTypeList);
        GetComponent<MinimapComponent>().InitAsMovable();
        GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
        GetComponent<VehicleComponent>().Initialize(ElectricTakeoverConfig.Obj.GetBatteryPoweredVehicleSpeed(10), 2,1, null, true);
        GetComponent<BoatComponent>().Size = BoatComponent.BoatSize.Large;
        GetComponent<VehicleComponent>().FailDriveMsg = Localizer.Do($"You are too hungry to drive {DisplayName}!");
        {
            GetComponent<PartsComponent>().Config(() => LocString.Empty,
            [
                new() { TypeName = nameof(HempMooringRopeItem), Quantity = 2},
                new() { TypeName = nameof(PortableSteamEngineItem), Quantity = 1},
                new() { TypeName = nameof(LubricantItem), Quantity = 2},
            ]);
        }
    }
}