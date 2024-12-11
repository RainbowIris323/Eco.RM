﻿using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Items.Recipes;
using Eco.Gameplay.Objects;
using Eco.Gameplay.Skills;
using Eco.Mods.TechTree;
using Eco.RM.ElectricTakeover.Components;
using Eco.RM.Framework.Config;
using Eco.Shared.Localization;
using Eco.Shared.Serialization;

namespace Eco.RM.ElectricTakeover.Objects;

[Serialized]
[LocDisplayName("Electric Battery Charger")]
[RequireComponent(typeof(BatteryBufferComponent))]
public partial class ElectricBatteryChargerObject : WorldObject, IRepresentsItem
{
    public virtual Type RepresentedItemType => typeof(ElectricBatteryChargerItem);
    static ElectricBatteryChargerObject() { }
    protected override void Initialize()
    {
        GetOrCreateComponent<BatteryBufferComponent>().Initialize(2, 20, false, true, false);
    }
}

[Serialized]
[LocDisplayName("Electric Battery Charger"), LocDescription("Allows the batteries within to charge from a connected electric power grid.")]
public partial class ElectricBatteryChargerItem : WorldObjectItem<ElectricBatteryChargerObject>
{
}


[RequiresSkill(typeof(BasicEngineeringSkill), 1)]
public class ElectricBatteryChargerRecipe : RecipeFamily, IConfigurableRecipe
{
    static RecipeDefaultModel Defaults => new()
    {
        ModelType = typeof(ElectricBatteryChargerRecipe).Name,
        Assembly = typeof(ElectricBatteryChargerRecipe).AssemblyQualifiedName,
        HiddenName = "Electric Battery Charger",
        LocalizableName = Localizer.DoStr("Electric Battery Charger"),
        IngredientList =
        [
            new RMIngredient(nameof(IronBarItem), false, 30),
            new RMIngredient(nameof(CopperBarItem), false, 30),
        ],
        ProductList =
        [
            new RMCraftable(nameof(ElectricBatteryChargerItem), 1),
        ],
        BaseExperienceOnCraft = 10,
        BaseLabor = 300,
        BaseCraftTime = 1,
        CraftTimeIsStatic = false,
        CraftingStation = nameof(WainwrightTableItem),
        RequiredSkillType = typeof(BasicEngineeringSkill),
        RequiredSkillLevel = 1,
        IngredientImprovementTalents = typeof(BasicEngineeringLavishResourcesTalent),
        SpeedImprovementTalents = [typeof(BasicEngineeringParallelSpeedTalent), typeof(BasicEngineeringFocusedSpeedTalent)],
    };

    static ElectricBatteryChargerRecipe() { RMRecipeResolver.AddDefaults(Defaults); }

    public ElectricBatteryChargerRecipe()
    {
        Recipes = RMRecipeResolver.Obj.ResolveRecipe(this);
        LaborInCalories = RMRecipeResolver.Obj.ResolveLabor(this);
        CraftMinutes = RMRecipeResolver.Obj.ResolveCraftMinutes(this);
        ExperienceOnCraft = RMRecipeResolver.Obj.ResolveExperience(this);
        Initialize(Localizer.DoStr(Defaults.LocalizableName), GetType());
        CraftingComponent.AddRecipe(RMRecipeResolver.Obj.ResolveStation(this), this);
    }
}
