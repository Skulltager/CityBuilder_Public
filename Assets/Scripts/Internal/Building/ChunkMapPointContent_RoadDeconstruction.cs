
using SheetCodes;

public class ChunkMapPointContent_RoadDeconstruction : ChunkMapPointContent_BuildingBase
{
    public readonly RoadRecord roadRecord;
    public override bool allowResourcePickup => deconstructionProgress.value >= 1;
    public override ContentTaskLocation contentTaskLocation => ContentTaskLocation.Edge;
    public override ChunkMapPointContentType contentType => ChunkMapPointContentType.BuildingDeconstruction;
    public readonly EventVariable<ChunkMapPointContent_RoadDeconstruction, float> deconstructionProgress;
    public readonly EventVariable<ChunkMapPointContent_RoadDeconstruction, float> resourcesProgress;
    public readonly BuildingTask_DeconstructRoad buildingTask;
    public readonly int startingInventoryCount;
    public int remainingResources;

    public ChunkMapPointContent_RoadDeconstruction(Map map, Point centerPoint, CardinalDirection direction, RoadRecord roadRecord, Player owner, ChunkMapPointContent_Building building)
        : base(map, centerPoint, direction, BuildingIdentifier.RoadConstruction.GetRecord(), owner, new Inventory_Unlimited(false))
    {
        this.roadRecord = roadRecord;
        deconstructionProgress = new EventVariable<ChunkMapPointContent_RoadDeconstruction, float>(this, 0);
        resourcesProgress = new EventVariable<ChunkMapPointContent_RoadDeconstruction, float>(this, 0);
        buildingTask = new BuildingTask_DeconstructRoad(this, 100, 100);

        foreach (Resource resource in building.inventory.resources.Values)
        {
            inventory.AddResources(resource.record.Identifier, resource.amount.value);
            startingInventoryCount += resource.amount.value;
        }

        foreach (ResourceCostRecord resourceCost in roadRecord.ResourceCosts)
        {
            inventory.AddResources(resourceCost.Resource.Identifier, resourceCost.Amount);
            startingInventoryCount += resourceCost.Amount;
        }

        foreach (Resource resource in inventory.resources.Values)
            resource.startingAmount.value = resource.amount.value;

        map.SetChunkMapRoadState(centerPoint, false);
    }

    public ChunkMapPointContent_RoadDeconstruction(Map map, Point centerPoint, CardinalDirection direction, Player owner, ChunkMapPointContent_RoadConstruction construction)
        : base(map, centerPoint, direction, BuildingIdentifier.RoadConstruction.GetRecord(), owner, new Inventory_Unlimited(false))
    {
        this.roadRecord = construction.roadRecord;
        deconstructionProgress = new EventVariable<ChunkMapPointContent_RoadDeconstruction, float>(this, 1 - construction.constructionProgress.value);
        resourcesProgress = new EventVariable<ChunkMapPointContent_RoadDeconstruction, float>(this, 0);
        buildingTask = new BuildingTask_DeconstructRoad(this, 100, 100);

        startingInventoryCount = construction.totalAmountResourcesNeeded;
        construction.inventory.CopyInventoryTo(inventory);

        foreach (ResourceCostRecord resourceCost in roadRecord.ResourceCosts)
            inventory.resources[resourceCost.Resource.Identifier].startingAmount.value = resourceCost.Amount;
    }

    private void OnValueChanged_Inventory_ResourceAmount(int oldValue, int newValue)
    {
        remainingResources += newValue - oldValue;
        resourcesProgress.value = (float)remainingResources / startingInventoryCount;
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
        ChunkMapPointContent_RoadConstruction construction = new ChunkMapPointContent_RoadConstruction(map, pivotPoint, direction, owner, this);
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