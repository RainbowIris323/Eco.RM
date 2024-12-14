using Eco.Core.Controller;
using Eco.Core.Items;
using Eco.Gameplay.Components.Auth;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Occupancy;
using Eco.Gameplay.Systems.NewTooltip;
using Eco.Mods.TechTree;
using Eco.Shared.Items;
using Eco.Shared.Localization;
using Eco.Shared.Math;
using Eco.Shared.Serialization;
using Eco.RM.Framework.Resolvers;
using Eco.RM.Framework.Models;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[RequireComponent(typeof(OnOffComponent))]
[RequireComponent(typeof(PropertyAuthComponent))]
[RequireComponent(typeof(MinimapComponent))]
[RequireComponent(typeof(LinkComponent))]
[RequireComponent(typeof(CraftingComponent))]
[RequireComponent(typeof(OccupancyRequirementComponent))]
[RequireComponent(typeof(ForSaleComponent))]
[LocDisplayName("Assembly Table"), LocDescription("A table used for assembling varius objects from components.")]
[Ecopedia("Work Stations", "Craft Tables", subPageName: "Assembly Table Item"), Tag("Usable")]
public class AssemblyTableObject : WorldObject, IRepresentsItem
{
    public Type RepresentedItemType => typeof(AssemblyTableItem);

    protected override void Initialize()
    {
        GetComponent<MinimapComponent>().SetCategory(Localizer.DoStr("Crafting"));
    }
}

[Serialized]
[Weight(1000)]
[IconGroup("World Object Minimap")]
[Ecopedia("Work Stations", "Craft Tables", createAsSubPage: true)]
[Tag(nameof(SurfaceTags.CanBeOnRug))]
[LocDisplayName("Assembly Table"), LocDescription("A table used for assembling varius objects from components.")]
public partial class AssemblyTableItem : WorldObjectItem<AssemblyTableObject>, IPersistentData
{
    protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(0 | DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(WorldObjectType));

    [Serialized, SyncToView, NewTooltipChildren(CacheAs.Instance, flags: TTFlags.AllowNonControllerTypeForChildren)] public object PersistentData { get; set; }
}

public class AssemblyTableRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(AssemblyTableRecipe).Name,
        Assembly = typeof(AssemblyTableRecipe).AssemblyQualifiedName,
        HiddenName = "Assembly Table",
        LocalizableName = Localizer.DoStr("Assembly Table"),
        IngredientList =
        [
            new RMIngredient(nameof(WorkbenchItem), false, 1, true),

        ],
        ProductList =
        [
            new RMCraftable(nameof(AssemblyTableItem), 1),
        ],
        BaseExperienceOnCraft = 0,
        BaseLabor = 1,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(WorkbenchItem),
    };

    static AssemblyTableRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public AssemblyTableRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}