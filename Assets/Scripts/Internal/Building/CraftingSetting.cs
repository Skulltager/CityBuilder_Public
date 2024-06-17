using SheetCodes;

public class CraftingSetting
{
    public readonly RecipeOptionRecord recipeOption;
    public readonly EventVariable<CraftingSetting, ResourceCostRecord> selectedCost;

    public CraftingSetting(RecipeOptionRecord recipeOption)
    {
        this.recipeOption = recipeOption;
        selectedCost = new EventVariable<CraftingSetting, ResourceCostRecord>(this, recipeOption.ResourceCost[0]);
    }
}