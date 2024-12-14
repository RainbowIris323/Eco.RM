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
using Eco.RM.Framework.Resolvers;
using Eco.RM.ElectricTakeover.Items;
using Eco.RM.Framework.Models;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Battery Powered Cart")]
[LocDescription("Large cart for hauling sizable loads.")]
[IconGroup("World Object Minimap")]
[Weight(15000)]
[Ecopedia("Crafted Objects", "Vehicles", createAsSubPage: true)]
public partial class BatteryPoweredCartItem : WorldObjectItem<BatteryPoweredCartObject>, IPersistentData
{
    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
}

public class BatteryPoweredCartRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(BatteryPoweredCartRecipe).Name,
        Assembly = typeof(BatteryPoweredCartRecipe).AssemblyQualifiedName,
        HiddenName = "Battery Powered Cart",
        LocalizableName = Localizer.DoStr("Battery Powered Cart"),
        IngredientList =
        [
            new RMIngredient(nameof(PoweredCartItem), false, 1, true),
            new RMIngredient(nameof(SimpleElectricUpgradeKitItem), false, 1, true),

        ],
        ProductList =
        [
            new RMCraftable(nameof(BatteryPoweredCartItem), 1),
        ],
        BaseExperienceOnCraft = 0,
        BaseLabor = 1,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(AssemblyTableItem),
    };

    static BatteryPoweredCartRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public BatteryPoweredCartRecipe()
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
[RepairRequiresSkill(typeof(BasicEngineeringSkill), 5)]
[Ecopedia("Crafted Objects", "Vehicles", subPageName: "Battery Powered Cart Item")]
public partial class BatteryPoweredCartObject : PhysicsWorldObject, IRepresentsItem
{
    static BatteryPoweredCartObject()
    {
        AddOccupancy<BatteryPoweredCartObject>([]);
    }
    public override TableTextureMode TableTexture => TableTextureMode.Metal;
    public override bool PlacesBlocks            => false;
    public override LocString DisplayName { get { return Localizer.DoStr("Powered Cart"); } }
    public Type RepresentedItemType { get { return typeof(BatteryPoweredCartItem); } }
    private BatteryPoweredCartObject() { }
    protected override void Initialize()
    {
        base.Initialize();         
        GetComponent<CustomTextComponent>().Initialize(200);
        GetComponent<BatteryConsumptionComponent>().Initialize(1, 110);
        GetComponent<VehicleComponent>().HumanPowered(0.5f);
        GetComponent<StockpileComponent>().Initialize(new Vector3i(2,2,3));
        GetComponent<PublicStorageComponent>().Initialize(18, 3500000);
        GetComponent<MinimapComponent>().InitAsMovable();
        GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Vehicles"));
        GetComponent<VehicleComponent>().Initialize(ElectricTakeoverConfig.Obj.GetBatteryPoweredVehicleSpeed(12), 1.5f, 1);
        GetComponent<VehicleComponent>().FailDriveMsg = Localizer.Do($"You are too hungry to drive {DisplayName}!");
        {
            GetComponent<PartsComponent>().Config(() => LocString.Empty,
            [
                new() { TypeName = nameof(IronWheelItem), Quantity = 2},
                new() { TypeName = nameof(LubricantItem), Quantity = 2},
            ]);
        }
    }
}