using Eco.Shared.Localization;
using Eco.Shared.Serialization;
using System.ComponentModel;

/**
 * THIS CODE IS NOT MINE!!!
 * 
 * ITS A MODIFIED VERSION OF https://github.com/TheKye/EM-Framework/blob/10.0-Staging/Eco.EM.Framework/DataStructures/RecipeModel.cs
 */

namespace Eco.RM.Framework.Models;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class RMIngredient : IEquatable<RMIngredient>
{
    [LocDisplayName("Ingredient Item")] public string Item { get; set; }
    [LocDisplayName("Item is Tag")] public bool Tag { get; set; }
    [LocDisplayName("Ingredient Amount")] public float Amount { get; set; }
    [LocDisplayName("Static Ingredient")] public bool Static { get; set; }

    public RMIngredient(string item, bool isTag, float amount, bool isStatic = false)
    {
        Item = item;
        Tag = isTag;
        Amount = amount;
        Static = isStatic;
    }

    public RMIngredient() { }

    public override string ToString() => $"{Item}";

    public bool Equals(RMIngredient other)
    {
        if (other is null)
            return false;

        return this.Item == other.Item && this.Amount == other.Amount && this.Tag == other.Tag && this.Static == other.Static;
    }

    public override bool Equals(object obj) => Equals(obj as RMIngredient);

    public override int GetHashCode() => (Item, Amount, Tag, Static).GetHashCode();
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class RMCraftable : IEquatable<RMCraftable>
{
    [LocDisplayName("Craftable Item")] public string Item { get; set; }
    [LocDisplayName("Crafting Amount")] public float Amount { get; set; }

    public RMCraftable(string item, float amount = 1)
    {
        Item = item;
        Amount = amount;
    }

    public RMCraftable() { }

    public override string ToString() => $"{Item}";

    public bool Equals(RMCraftable other)
    {
        if (other is null)
            return false;

        return this.Item == other.Item && this.Amount == other.Amount;
    }

    public override bool Equals(object obj) => Equals(obj as RMCraftable);

    public override int GetHashCode() => (Item, Amount).GetHashCode();
}

[LocDisplayName("Recipe Model")]
public class RecipeModel
{
    [LocDisplayName("Recipe Name")] public string LocalizableName { get; set; }
    [LocDisplayName("Crafting Experience - Baseline Value")] public float BaseExperienceOnCraft { get; set; }
    [LocDisplayName("Labor - Baseline Value")] public float BaseLabor { get; set; }
    [LocDisplayName("Craft Time - Baseline Value")] public float BaseCraftTime { get; set; }
    [LocDisplayName("Craftime - Is Static")] public bool CraftTimeIsStatic { get; set; }
    [LocDisplayName("Crafting Station")] public string CraftingStation { get; set; }
    [LocDisplayName("Ingredient List")] public List<RMIngredient> IngredientList { get; set; }
    [LocDisplayName("Product List")] public List<RMCraftable> ProductList { get; set; }
    [LocDisplayName("Enable Recipe")] public bool EnableRecipe { get; set; } = true;

    public override string ToString() => $"{Assembly.Split(",", StringSplitOptions.TrimEntries)[1]} - {ModelType}";

    public static bool Compare(RecipeModel A, RecipeModel B)
    {
        return A.IngredientList.SequenceEqual(B.IngredientList) && A.ProductList.SequenceEqual(B.ProductList);
    }
    [LocDisplayName("Model Type"), ReadOnly(true)] public string ModelType { get; set; }
    [LocDisplayName("Qualified Assembly"), ReadOnly(true)] public string Assembly { get; set; }
}

[LocDisplayName("Default Recipe Model")]
public class RecipeDefaultModel : RecipeModel
{
    [JsonIgnore, ReadOnly(true)] public string HiddenName { get; set; }
    [JsonIgnore, ReadOnly(true)] public Type RequiredSkillType { get; set; }
    [JsonIgnore, ReadOnly(true)] public int RequiredSkillLevel { get; set; }
    [JsonIgnore, ReadOnly(true)] public Type IngredientImprovementTalents { get; set; }
    [JsonIgnore, ReadOnly(true)] public Type[] SpeedImprovementTalents { get; set; }
}