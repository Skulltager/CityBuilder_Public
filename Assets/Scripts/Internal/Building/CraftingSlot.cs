using SheetCodes;
public class CraftingSlot
{
    public readonly ChunkMapPointContent_Building_CraftingStation craftingStation;
    public readonly EventVariable<CraftingSlot, float> craftingProgress;
    public readonly EventVariable<CraftingSlot, bool> resourcesAssigned;
    public readonly ResourceCostRecord[] selectedResourceCosts;
    public bool villagerAssigned;

    public CraftingSlot(ChunkMapPointContent_Building_CraftingStation craftingStation)
    {
        this.craftingStation = craftingStation;
        craftingProgress = new EventVariable<CraftingSlot, float>(this, 0);
        selectedResourceCosts = new ResourceCostRecord[craftingStation.record.CraftingRecipes.RecipeCosts.Length];
        resourcesAssigned = new EventVariable<CraftingSlot, bool>(this, false);
    }

    public bool AddProgress(float progress)
    {
        float newProgressAmount = craftingProgress.value + progress / craftingStation.record.CraftingRecipes.CraftingTime;
        if (newProgressAmount < 1)
        {
            craftingProgress.value = newProgressAmount;
            return false;
        }

        FinishCrafting();
        return true;
    }

    private void FinishCrafting()
    {
        craftingProgress.value = 0;

        foreach (ResourceCostRecord resourceCost in selectedResourceCosts)
        {
            Resource resource = craftingStation.inventory.resources[resourceCost.Resource.Identifier];
            resource.amount.value -= resourceCost.Amount;
            resource.lockedAmount.value -= resourceCost.Amount;
        }
        resourcesAssigned.value = false;
        craftingStation.inventory.resources[craftingStation.record.CraftingRecipes.CraftedResource.Identifier].amount.value++;
        craftingStation.RecalculateDesiredResourcesAndCraftingSlots();
    }
}