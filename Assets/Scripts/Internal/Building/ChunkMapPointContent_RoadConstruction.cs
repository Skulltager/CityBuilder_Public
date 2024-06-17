
using SheetCodes;
using UnityEngine;

public class ChunkMapPointContent_RoadConstruction : ChunkMapPointContent_BuildingBase
{
    public override ChunkMapPointContentType contentType => ChunkMapPointContentType.BuildingConstruction;
    public override ContentTaskLocation contentTaskLocation => ContentTaskLocation.Edge;

    public readonly RoadRecord roadRecord;
    public readonly EventVariable<ChunkMapPointContent_RoadConstruction, float> resourcesProgress;
    public readonly EventVariable<ChunkMapPointContent_RoadConstruction, float> constructionProgress;
    public readonly BuildingTask_ConstructRoad buildingTask;
    public readonly int totalAmountResourcesNeeded;

    public ChunkMapPointContent_RoadConstruction(Map map, Point centerPoint, CardinalDirection direction, RoadRecord roadRecord, Player owner)
        : base(map, centerPoint, direction, BuildingIdentifier.RoadConstruction.GetRecord(), owner, new Inventory_Unlimited(false))
    {
        this.roadRecord = roadRecord;
        buildingTask = new BuildingTask_ConstructRoad(this, 100, 100);
        constructionProgress = new EventVariable<ChunkMapPointContent_RoadConstruction, float>(this, 0);
        resourcesProgress = new EventVariable<ChunkMapPointContent_RoadConstruction, float>(this, 0);

        foreach (ResourceCostRecord resourceCost in roadRecord.ResourceCosts)
        {
            totalAmountResourcesNeeded += resourceCost.Amount;
            inventory.resources[resourceCost.Resource.Identifier].currentDesiredAmount.value = resourceCost.Amount;
            inventory.resources[resourceCost.Resource.Identifier].totalDesiredAmount.value = resourceCost.Amount;
        }
    }

    public ChunkMapPointContent_RoadConstruction(Map map, Point centerPoint, CardinalDirection direction, Player owner, ChunkMapPointContent_RoadDeconstruction deconstruction)
        : base(map, centerPoint, direction, BuildingIdentifier.RoadConstruction.GetRecord(), owner, new Inventory_Unlimited(false))
    {
        roadRecord = deconstruction.roadRecord;
        buildingTask = new BuildingTask_ConstructRoad(this, 100, 100);
        constructionProgress = new EventVariable<ChunkMapPointContent_RoadConstruction, float>(this, 0);
        resourcesProgress = new EventVariable<ChunkMapPointContent_RoadConstruction, float>(this, 0);

        deconstruction.inventory.CopyInventoryTo(inventory);

        foreach (ResourceCostRecord resourceCost in roadRecord.ResourceCosts)
        {
            totalAmountResourcesNeeded += resourceCost.Amount;
            inventory.resources[resourceCost.Resource.Identifier].currentDesiredAmount.value = resourceCost.Amount;
            inventory.resources[resourceCost.Resource.Identifier].totalDesiredAmount.value = resourceCost.Amount;
        }
    }

    private void OnValueChanged_ConstructionProgress(float oldValue, float newValue)
    {
        if (newValue >= 1)
        {
            FinishConstruction();
        }
    }

    public void FinishConstruction()
    {
        map.SetChunkMapRoadState(centerPoint, true);
        RemoveFromMap();
    }

    public void CancelConstruction()
    {
        ChunkMapPointContent_RoadDeconstruction deconstruction = new ChunkMapPointContent_RoadDeconstruction(map, pivotPoint, direction, owner, this);
        RemoveFromMap(deconstruction);
        deconstruction.PlaceOnMap();
    }

    public override void PlaceOnMap()
    {
        base.PlaceOnMap();
        owner.townCenterBuilding.villagerTasks.Add(buildingTask);
        constructionProgress.onValueChangeImmediate += OnValueChanged_ConstructionProgress;

        foreach (Resource resource in inventory.resources.Values)
            resource.amount.onValueChangeImmediate += OnValueChanged_Inventory_ResourceAmount;
    }

    private void OnValueChanged_Inventory_ResourceAmount(int oldValue, int newValue)
    {
        int resourcesInStorage = 0;
        foreach (ResourceCostRecord resourceCost in record.ResourceCosts)
        {
            Resource resource = inventory.resources[resourceCost.Resource.Identifier];
            resourcesInStorage += Mathf.Min(resource.amount.value, resourceCost.Amount);
        }

        resourcesProgress.value = (float)resourcesInStorage / totalAmountResourcesNeeded;
    }

    public override void RemoveFromMap(ChunkMapPointContent replacement = null)
    {
        owner.townCenterBuilding.villagerTasks.Remove(buildingTask);
        base.RemoveFromMap(replacement);
        constructionProgress.onValueChange -= OnValueChanged_ConstructionProgress;

        foreach (Resource resource in inventory.resources.Values)
            resource.amount.onValueChange -= OnValueChanged_Inventory_ResourceAmount;
    }
}