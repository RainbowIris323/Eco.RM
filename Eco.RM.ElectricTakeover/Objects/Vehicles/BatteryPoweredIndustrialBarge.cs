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
using Eco.Gameplay.Items.Recipes;
using Eco.RM.Framework.Resolvers;
using Eco.RM.ElectricTakeover.Items;
using Eco.RM.ElectricTakeover.Utility;
using Eco.RM.Framework.Models;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Battery Powered Industrial Barge")]
[LocDescription("")]
[IconGroup("World Object Minimap")]
[Weight(30000)]
[WaterPlaceable]
[Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
public partial class BatteryPoweredIndustrialBargeItem : WorldObjectItem<BatteryPoweredIndustrialBargeObject>, IPersistentData
{
    public float InteractDistance => DefaultInteractDistance.WaterPlacement;
    public bool ShouldHighlight(Type block) => false;
    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
}

public class BatteryPoweredIndustrialBargeRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(BatteryPoweredIndustrialBargeRecipe).Name,
        Assembly = typeof(BatteryPoweredIndustrialBargeRecipe).AssemblyQualifiedName,
        HiddenName = "Battery Powered Industrial Barge",
        LocalizableName = Localizer.DoStr("Battery Powered Industrial Barge"),
        IngredientList =
        [
            new RMIngredient(nameof(IndustrialBargeItem), false, 1, true),
            new RMIngredient(nameof(AdvancedElectricUpgradeKitItem), false, 1, true),

        ],
        ProductList =
        [
            new RMCraftable(nameof(BatteryPoweredIndustrialBargeItem), 1),
        ],
        BaseExperienceOnCraft = 0,
        BaseLabor = 1,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(AssemblyTableItem),
    };

    static BatteryPoweredIndustrialBargeRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public BatteryPoweredIndustrialBargeRecipe()
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
[RepairRequiresSkill(typeof(ShipwrightSkill), 6)]
[Ecopedia("Crafted Objects", "Vehicles", subPageName: "Battery Powered Industrial Barge Item")]
public partial class BatteryPoweredIndustrialBargeObject : PhysicsWorldObject, IRepresentsItem
{
    static BatteryPoweredIndustrialBargeObject()
    {
        AddOccupancy<BatteryPoweredIndustrialBargeObject>([]);
    }
    public override float InteractDistance => DefaultInteractDistance.WaterPlacement;
    public override TableTextureMode TableTexture => TableTextureMode.Metal;
    public override bool PlacesBlocks => false;
    public override LocString DisplayName { get { return Localizer.DoStr("Battery Powered Industrial Barge"); } }
    public Type RepresentedItemType { get { return typeof(BatteryPoweredIndustrialBargeItem); } }
    private BatteryPoweredIndustrialBargeObject() { }
    protected override void Initialize()
    {
        base.Initialize();
        GetComponent<BatteryConsumptionComponent>().Initialize(1, 150);
        GetComponent<VehicleComponent>().HumanPowered(1);
        GetComponent<PublicStorageComponent>().Initialize(96, 32000000);
        GetComponent<MinimapComponent>().InitAsMovable();
        GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
        GetComponent<VehicleComponent>().Initialize(ElectricTakeoverConfig.Obj.GetBatteryPoweredVehicleSpeed(10), 2,1, null, true);
        GetComponent<BoatComponent>().Size = BoatComponent.BoatSize.Large;
        GetComponent<VehicleComponent>().FailDriveMsg = Localizer.Do($"You are too hungry to drive {DisplayName}!");
        {
            GetComponent<PartsComponent>().Config(() => LocString.Empty,
            [
                new() { TypeName = nameof(CombustionEngineItem), Quantity = 2},
                new() { TypeName = nameof(LubricantItem), Quantity = 2},
                new() { TypeName = nameof(MetalRudderItem), Quantity = 1},
            ]);
        }
    }
}
