
using SheetCodes;

public class ChunkMapPointContent_BuildingDeconstruction : ChunkMapPointContent_BuildingBase
{
    public override bool allowResourcePickup => deconstructionProgress.value >= 1;
    public override ContentTaskLocation contentTaskLocation => ContentTaskLocation.Edge;
    public override ChunkMapPointContentType contentType => ChunkMapPointContentType.BuildingDeconstruction;
    public readonly EventVariable<ChunkMapPointContent_BuildingDeconstruction, float> deconstructionProgress;
    public readonly EventVariable<ChunkMapPointContent_BuildingDeconstruction, float> resourcesProgress;
    public readonly BuildingTask_DeconstructBuilding buildingTask;
    public readonly int startingInventoryCount;
    public int remainingResources;

    public ChunkMapPointContent_BuildingDeconstruction(Map map, Point centerPoint, CardinalDirection direction, BuildingRecord record, Player owner, ChunkMapPointContent_Building building)
        :base(map, centerPoint, direction, record, owner, new Inventory_Unlimited(false))
    {
        deconstructionProgress = new EventVariable<ChunkMapPointContent_BuildingDeconstruction, float>(this, 0);
        resourcesProgress = new EventVariable<ChunkMapPointContent_BuildingDeconstruction, float>(this, 0);
        buildingTask = new BuildingTask_DeconstructBuilding(this, 100, 100);

        foreach (Resource resource in building.inventory.resources.Values)
        {
            inventory.AddResources(resource.record.Identifier, resource.amount.value);
            startingInventoryCount += resource.amount.value;
        }

        foreach (ResourceCostRecord resourceCost in record.ResourceCosts)
        {
            inventory.AddResources(resourceCost.Resource.Identifier, resourceCost.Amount);
            startingInventoryCount += resourceCost.Amount;
        }

        foreach (Resource resource in inventory.resources.Values)
            resource.startingAmount.value = resource.amount.value;
    }

    public ChunkMapPointContent_BuildingDeconstruction(Map map, Point centerPoint, CardinalDirection direction, BuildingRecord record, Player owner, ChunkMapPointContent_BuildingConstruction construction)
        : base(map, centerPoint, direction, record, owner, new Inventory_Unlimited(false))
    {
        deconstructionProgress = new EventVariable<ChunkMapPointContent_BuildingDeconstruction, float>(this, 1 - construction.constructionProgress.value);
        resourcesProgress = new EventVariable<ChunkMapPointContent_BuildingDeconstruction, float>(this, 0);
        buildingTask = new BuildingTask_DeconstructBuilding(this, 100, 100);

        startingInventoryCount = construction.totalAmountResourcesNeeded;
        construction.inventory.CopyInventoryTo(inventory);

        foreach (ResourceCostRecord resourceCost in record.ResourceCosts)
            inventory.resources[resourceCost.Resource.Identifier].startingAmount.value = resourceCost.Amount;
    }

    private void OnValueChanged_Inventory_ResourceAmount(int oldValue, int newValue)
    {
        remainingResources += newValue - oldValue;
        resourcesProgress.value = (float) remainingResources / startingInventoryCount;
    }

    private void OnValueChanged_ResourcesProgress(float oldValue, float newValue)
    {
        if (newValue <= 0)
            FinishDeconstruction();
    }

    public void FinishDeconstruction()
    {
        RemoveFromMap();
    }

    public void CancelDeconstruction()
    {
        ChunkMapPointContent_BuildingConstruction construction = new ChunkMapPointContent_BuildingConstruction(map, pivotPoint, direction, record, owner, this);
        construction.constructionProgress.value = 1 - deconstructionProgress.value;
        RemoveFromMap(construction);
        construction.PlaceOnMap();
    }

    public override void PlaceOnMap()
    {
        base.PlaceOnMap();
        owner.townCenterBuilding.villagerTasks.Add(buildingTask);

        foreach (Resource resource in inventory.resources.Values)
            resource.amount.onValueChangeImmediate += OnValueChanged_Inventory_ResourceAmount;

        resourcesProgress.onValueChangeImmediate += OnValueChanged_ResourcesProgress;
    }

    public override void RemoveFromMap(ChunkMapPointContent replacement = null)
    {
        owner.townCenterBuilding.villagerTasks.Remove(buildingTask);

        base.RemoveFromMap(replacement);

        foreach (Resource resource in inventory.resources.Values)
            resource.amount.onValueChange -= OnValueChanged_Inventory_ResourceAmount;

        resourcesProgress.onValueChange -= OnValueChanged_ResourcesProgress;
    }
}