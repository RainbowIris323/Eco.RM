using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Components.Auth;
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
using Eco.Gameplay.Items.Recipes;
using Eco.RM.Framework.Config;
using Eco.RM.ElectricTakeover.Items;
using Eco.RM.ElectricTakeover.Utility;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Battery Powered Crane")]
[LocDescription("Allows the placement and transport of materials in an area.")]
[IconGroup("World Object Minimap")]
[Weight(25000)]
[RequireComponent(typeof(OccupancyRequirementComponent))]
[Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
public partial class BatteryPoweredCraneItem : WorldObjectItem<BatteryPoweredCraneObject>, IPersistentData
{
    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
    protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(this.WorldObjectType));
}

public class BatteryPoweredCraneRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(BatteryPoweredCraneRecipe).Name,
        Assembly = typeof(BatteryPoweredCraneRecipe).AssemblyQualifiedName,
        HiddenName = "Battery Powered Crane",
        LocalizableName = Localizer.DoStr("Battery Powered Crane"),
        IngredientList =
        [
            new RMIngredient(nameof(CraneItem), false, 1, true),
            new RMIngredient(nameof(ImprovedElectricUpgradeKitItem), false, 1, true),

        ],
        ProductList =
        [
            new RMCraftable(nameof(BatteryPoweredCraneItem), 1),
        ],
        BaseExperienceOnCraft = 0,
        BaseLabor = 1,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(AssemblyTableItem),
    };

    static BatteryPoweredCraneRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public BatteryPoweredCraneRecipe()
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
[RequireComponent(typeof(VehicleComponent))]
[RequireComponent(typeof(MinimapComponent))]           
[RequireComponent(typeof(OccupancyRequirementComponent))]
[RequireComponent(typeof(MovableLinkComponent))]
[RequireComponent(typeof(CraneToolComponent))]
[RequireComponent(typeof(PhysicsValueSyncComponent))]
[RequireComponent(typeof(PartsComponent))]
[RepairRequiresSkill(typeof(MechanicsSkill), 5)]
[LocDisplayName("Battery Powered Crane")]
[Ecopedia("Crafted Objects", "Vehicles", subPageName: "Battery Powered Crane Item")]
public partial class BatteryPoweredCraneObject : PhysicsWorldObject, IRepresentsItem
{
    static BatteryPoweredCraneObject()
    {
        AddOccupancy<BatteryPoweredCraneObject>([]);
    }
    public override TableTextureMode TableTexture => TableTextureMode.Metal;
    public Type RepresentedItemType { get { return typeof(BatteryPoweredCraneItem); } }

    private static string[] fuelTagList =
    [
        "Burnable Fuel",
    ];
    private BatteryPoweredCraneObject() { }
    protected override void Initialize()
    {
        base.Initialize();
        GetComponent<BatteryConsumptionComponent>().Initialize(1, 150);
        GetComponent<CraneToolComponent>().Initialize(0, 150);
        GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
        GetComponent<VehicleComponent>().Initialize(ElectricTakeoverConfig.Obj.GetBatteryPoweredVehicleSpeed(30), 1,1);
        GetComponent<VehicleComponent>().FailDriveMsg = Localizer.Do($"You are too hungry to drive {DisplayName}!");
        {
            GetComponent<PartsComponent>().Config(() => LocString.Empty,
            [
                new() { TypeName = nameof(PortableSteamEngineItem), Quantity = 1},
            ]);
        }
    }
}
