using Eco.Core.Items;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Skills;
using Eco.Mods.TechTree;
using Eco.RM.Framework.Config;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Items;

[Serialized]
[LocDescription("A kit used to turn advanced non-battery powered machines into battery powered ones.")]
[LocDisplayName("Advanced Electric Upgrade Kit")]
[MaxStackSize(5)]
public class AdvancedElectricUpgradeKitItem : Item { }


[RequiresSkill(typeof(IndustrySkill), 3)]
public class AdvancedElectricUpgradeKitRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(AdvancedElectricUpgradeKitRecipe).Name,
        Assembly = typeof(AdvancedElectricUpgradeKitRecipe).AssemblyQualifiedName,
        HiddenName = "Advanced Electric Upgrade Kit",
        LocalizableName = Localizer.DoStr("Advanced Electric Upgrade Kit"),
        IngredientList =
        [
                new RMIngredient(nameof(LargeBatteryItem), false, 3, true),
                new RMIngredient(nameof(ImprovedElectricUpgradeKitItem), false, 3, true),
                new RMIngredient(nameof(SteelPlateItem), false, 30),
                new RMIngredient(nameof(CopperWiringItem), false, 30),
                new RMIngredient(nameof(GoldWiringItem), false, 15),
                new RMIngredient(nameof(PlasticItem), false, 30),

        ],
        ProductList =
        [
            new RMCraftable(nameof(AdvancedElectricUpgradeKitItem), 1),
        ],
        BaseExperienceOnCraft = 30,
        BaseLabor = 1800,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(ElectricMachinistTableItem),
        RequiredSkillType = typeof(IndustrySkill),
        RequiredSkillLevel = 3,
        IngredientImprovementTalents = typeof(IndustryLavishResourcesTalent),
        SpeedImprovementTalents = [typeof(IndustryParallelSpeedTalent), typeof(IndustryFocusedSpeedTalent)],
    };

    static AdvancedElectricUpgradeKitRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public AdvancedElectricUpgradeKitRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}
