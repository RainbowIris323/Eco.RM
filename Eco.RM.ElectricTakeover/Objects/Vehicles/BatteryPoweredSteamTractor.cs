using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Items;
using Eco.Gameplay.Skills;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Occupancy;
using Eco.Gameplay.Systems.Exhaustion;
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
[LocDisplayName("Battery Powered Steam Tractor")]
[LocDescription("A tractor powered through steam.")]
[IconGroup("World Object Minimap")]
[Weight(25000)]
[Tag("Excavation")]
[Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
public partial class BatteryPoweredSteamTractorItem : WorldObjectItem<BatteryPoweredSteamTractorObject>, IPersistentData
{
    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
}

public class BatteryPoweredSteamTractorRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(BatteryPoweredSteamTractorRecipe).Name,
        Assembly = typeof(BatteryPoweredSteamTractorRecipe).AssemblyQualifiedName,
        HiddenName = "Battery Powered Steam Tractor",
        LocalizableName = Localizer.DoStr("Battery Powered Steam Tractor"),
        IngredientList =
        [
            new RMIngredient(nameof(SteamTractorItem), false, 1, true),
            new RMIngredient(nameof(ImprovedElectricUpgradeKitItem), false, 1, true),

        ],
        ProductList =
        [
            new RMCraftable(nameof(BatteryPoweredSteamTractorItem), 1),
        ],
        BaseExperienceOnCraft = 0,
        BaseLabor = 1,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(AssemblyTableItem),
    };

    static BatteryPoweredSteamTractorRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public BatteryPoweredSteamTractorRecipe()
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
[RequireComponent(typeof(MovableLinkComponent))]
[RequireComponent(typeof(VehicleComponent))]
[RequireComponent(typeof(CustomTextComponent))]
[RequireComponent(typeof(ModularVehicleComponent))]
[RequireComponent(typeof(MinimapComponent))]           
[RequireComponent(typeof(PartsComponent))]
[RepairRequiresSkill(typeof(MechanicsSkill), 2)]
[ExhaustableUnlessOverridenVehicle]
[Ecopedia("Crafted Objects", "Vehicles", subPageName: "Battery Powered Steam Tractor Item")]
public partial class BatteryPoweredSteamTractorObject : PhysicsWorldObject, IRepresentsItem
{
    static BatteryPoweredSteamTractorObject()
    {
        AddOccupancy<BatteryPoweredSteamTractorObject>([]);
    }
    public override TableTextureMode TableTexture => TableTextureMode.Metal;
    public override bool PlacesBlocks => false;
    public override LocString DisplayName { get { return Localizer.DoStr("Battery Powered Steam Tractor"); } }
    public Type RepresentedItemType { get { return typeof(BatteryPoweredSteamTractorItem); } }
    private BatteryPoweredSteamTractorObject() { }
    private static readonly Type[] SegmentTypeList = Array.Empty<Type>();
    private static readonly Type[] AttachmentTypeList =
    [
        typeof(SteamTractorPlowItem),
        typeof(SteamTractorHarvesterItem),
        typeof(SteamTractorSowerItem),
        typeof(SteamTractorScoopItem),
    ];
    protected override void Initialize()
    {
        base.Initialize();         
        GetComponent<CustomTextComponent>().Initialize(200);
        GetComponent<BatteryConsumptionComponent>().Initialize(1, 225);
        GetComponent<VehicleComponent>().HumanPowered(2);
        GetComponent<ModularVehicleComponent>().Initialize(0, 1, SegmentTypeList, AttachmentTypeList);
        GetComponent<MinimapComponent>().InitAsMovable();
        GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
        GetComponent<VehicleComponent>().Initialize(ElectricTakeoverConfig.Obj.GetBatteryPoweredVehicleSpeed(12), 1);
        GetComponent<VehicleToolComponent>().Initialize(12, 2500000, 100, 200, 0, true, VehicleUtilities.GetInventoryRestriction(this));
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