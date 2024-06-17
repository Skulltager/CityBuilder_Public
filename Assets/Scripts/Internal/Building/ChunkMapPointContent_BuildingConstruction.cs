
using SheetCodes;
using UnityEngine;

public class ChunkMapPointContent_BuildingConstruction : ChunkMapPointContent_BuildingBase
{
    public override ChunkMapPointContentType contentType => ChunkMapPointContentType.BuildingConstruction;
    public override ContentTaskLocation contentTaskLocation => ContentTaskLocation.Edge;

    public readonly EventVariable<ChunkMapPointContent_BuildingConstruction, float> resourcesProgress;
    public readonly EventVariable<ChunkMapPointContent_BuildingConstruction, float> constructionProgress;
    public readonly BuildingTask_ConstructBuilding buildingTask;
    public readonly int totalAmountResourcesNeeded;

    public ChunkMapPointContent_BuildingConstruction(Map map, Point centerPoint, CardinalDirection direction, BuildingRecord record, Player owner)
        : base(map, centerPoint, direction, record, owner, new Inventory_Unlimited(false))
    {
        buildingTask = new BuildingTask_ConstructBuilding(this, 100, 100);
        constructionProgress = new EventVariable<ChunkMapPointContent_BuildingConstruction, float>(this, 0);
        resourcesProgress = new EventVariable<ChunkMapPointContent_BuildingConstruction, float>(this, 0);

        foreach (ResourceCostRecord resourceCost in record.ResourceCosts)
        {
            totalAmountResourcesNeeded += resourceCost.Amount;
            inventory.resources[resourceCost.Resource.Identifier].currentDesiredAmount.value = resourceCost.Amount;
            inventory.resources[resourceCost.Resource.Identifier].totalDesiredAmount.value = resourceCost.Amount;
        }
    }

    public ChunkMapPointContent_BuildingConstruction(Map map, Point centerPoint, CardinalDirection direction, BuildingRecord record, Player owner, ChunkMapPointContent_BuildingDeconstruction deconstruction)
        : base(map, centerPoint, direction, record, owner, new Inventory_Unlimited(false))
    {
        buildingTask = new BuildingTask_ConstructBuilding(this, 100, 100);
        constructionProgress = new EventVariable<ChunkMapPointContent_BuildingConstruction, float>(this, 0);
        resourcesProgress = new EventVariable<ChunkMapPointContent_BuildingConstruction, float>(this, 0);

        deconstruction.inventory.CopyInventoryTo(inventory);

        foreach (ResourceCostRecord resourceCost in record.ResourceCosts)
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
        ChunkMapPointContent_Building building = CreateBuilding();
        RemoveFromMap(building);
        building.PlaceOnMap();
    }

    public void CancelConstruction()
    {
        ChunkMapPointContent_BuildingDeconstruction deconstruction = new ChunkMapPointContent_BuildingDeconstruction(map, pivotPoint, direction, record, owner, this);
        RemoveFromMap(deconstruction);
        deconstruction.PlaceOnMap();
    }

    public override void PlaceOnMap()
    {
        base.PlaceOnMap();
        owner.townCenterBuilding.villagerTasks.Add(buildingTask);
        constructionProgress.onValueChangeImmediate += OnValueChanged_ConstructionProgress;

        foreach(Resource resource in inventory.resources.Values)
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

        resourcesProgress.value = (float) resourcesInStorage / totalAmountResourcesNeeded;
    }

    public override void RemoveFromMap(ChunkMapPointContent replacement = null)
    {
        owner.townCenterBuilding.villagerTasks.Remove(buildingTask);
        base.RemoveFromMap(replacement);
        constructionProgress.onValueChange -= OnValueChanged_ConstructionProgress;

        foreach (Resource resource in inventory.resources.Values)
            resource.amount.onValueChange -= OnValueChanged_Inventory_ResourceAmount;

    }

    private ChunkMapPointContent_Building CreateBuilding()
    {
        switch(record.Identifier)
        {
            case BuildingIdentifier.GatheringHut:
            case BuildingIdentifier.MiningCamp:
            case BuildingIdentifier.WoodcuttersHut:
                return new ChunkMapPointContent_Building_GatheringHut(map, pivotPoint, direction, record, owner, this);
            case BuildingIdentifier.TownCenter:
                return new ChunkMapPointContent_Building_TownCenter(map, pivotPoint, direction, owner, this);
            case BuildingIdentifier.House:
                return new ChunkMapPointContent_Building_House(map, pivotPoint, direction, owner, this);
            case BuildingIdentifier.Warehouse:
                return new ChunkMapPointContent_Building_Warehouse(map, pivotPoint, direction, owner, this);
            case BuildingIdentifier.CraftingStation:
                return new ChunkMapPointContent_Building_CraftingStation(map, pivotPoint, direction, owner, this);
        }

        throw new System.Exception(string.Format("CreateBuilding is missing implementation for {0}", record.Identifier));
    }
}