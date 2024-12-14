using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Items;
using Eco.Gameplay.Skills;
using Eco.Mods.TechTree;
using Eco.RM.Framework.Resolvers;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using Eco.RM.Framework.Models;

namespace Eco.RM.ElectricTakeover.Items;

[Serialized]
[LocDescription("A kit used to turn less simple non-battery powered machines into battery powered ones.")]
[LocDisplayName("Improved Electric Upgrade Kit")]
[MaxStackSize(5)]
public class ImprovedElectricUpgradeKitItem : Item
{
}


[RequiresSkill(typeof(MechanicsSkill), 3)]
public class ImprovedElectricUpgradeKitRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(ImprovedElectricUpgradeKitRecipe).Name,
        Assembly = typeof(ImprovedElectricUpgradeKitRecipe).AssemblyQualifiedName,
        HiddenName = "Improved Electric Upgrade Kit",
        LocalizableName = Localizer.DoStr("Improved Electric Upgrade Kit"),
        IngredientList =
        [
            new RMIngredient(nameof(MediumBatteryItem), false, 3, true),
            new RMIngredient(nameof(SimpleElectricUpgradeKitItem), false, 3, true),
            new RMIngredient(nameof(IronPlateItem), false, 30),
            new RMIngredient(nameof(CopperPlateItem), false, 30),
            new RMIngredient(nameof(CopperWiringItem), false, 15),
            new RMIngredient(nameof(LumberItem), false, 15),

        ],
        ProductList =
        [
            new RMCraftable(nameof(ImprovedElectricUpgradeKitItem), 1),
        ],
        BaseExperienceOnCraft = 15,
        BaseLabor = 900,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(MachinistTableItem),
        RequiredSkillType = typeof(MechanicsSkill),
        RequiredSkillLevel = 3,
        IngredientImprovementTalents = typeof(MechanicsLavishResourcesTalent),
        SpeedImprovementTalents = [typeof(MechanicsParallelSpeedTalent), typeof(MechanicsFocusedSpeedTalent)],
    };

    static ImprovedElectricUpgradeKitRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public ImprovedElectricUpgradeKitRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}
