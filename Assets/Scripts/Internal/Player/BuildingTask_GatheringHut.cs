
using System.Collections.Generic;
using System.Linq;

public class BuildingTask_GatheringHut : BuildingTask
{
    public const int BONUS_CARRY_CAPACITY = 10;

    public readonly new ChunkMapPointContent_Building_GatheringHut building;
    public readonly List<VillagerTaskPoint> worldResourceTaskPoints;

    public BuildingTask_GatheringHut(ChunkMapPointContent_Building_GatheringHut building, int targetVillagers, int maxVillager)
        : base(building, targetVillagers, maxVillager)
    {
        this.building = building;
        worldResourceTaskPoints = new List<VillagerTaskPoint>();
    }

    public override bool TryReserveVillagerTaskData(Villager villager)
    {
        if (building.inventory.totalAmount.value >= villager.carryCapacity + BONUS_CARRY_CAPACITY)
        {
            if (TryStartTask_EmptyGatheringHut(villager))
                return true;

            if (TryStartTask_HarvestResources(villager))
                return true;
        }
        else
        {
            if (TryStartTask_HarvestResources(villager))
                return true;

            if (TryStartTask_EmptyGatheringHut(villager))
                return true;
        }

        return false;
    }

    #region Empty Gathering Hut
    private bool TryStartTask_EmptyGatheringHut(Villager villager)
    {
        return TryStartTask_RemoveUndesiredResources(villager, BONUS_CARRY_CAPACITY);
    }
    #endregion

    #region Harvest Resources Task
    private bool TryStartTask_HarvestResources(Villager villager)
    {
        if (!worldResourceTaskPoints.Any(i => i.enabled.value))
            return false;

        StartTask_HarvestResources_MoveToResource(villager);
        return true;
    }

    private void StartTask_HarvestResources_MoveToResource(Villager villager)
    {
        List<ChunkMapPointContent> dependencies = new List<ChunkMapPointContent>() { building };
        string taskDescription = string.Format("Moving to x to gather y");
        BuildingTaskData_MoveToLocation taskData = new BuildingTaskData_MoveToLocation(dependencies, worldResourceTaskPoints, ContentTaskLocation.Edge, true, taskDescription);
        VillagerSubTask_MoveToLocation task = new VillagerSubTask_MoveToLocation(villager, this, taskData);
        task.finishedCallback = FinishTask_HarvestResources_MoveToResource;
        taskData.pathCalculatedCallback = TaskEvent_HarvestResources_MoveToResource_PathCalculated;
        villager.queuedTasks.Add(task);
    }

    private void TaskEvent_HarvestResources_MoveToResource_PathCalculated(Villager villager, BuildingTaskData_MoveToLocation taskData)
    {
        ChunkMapPointContent_WorldResource worldResource = taskData.pathData.taskPoint.owner as ChunkMapPointContent_WorldResource;
        taskData.taskDescription = string.Format("Moving to {0} to gather {1}", worldResource.name, worldResource.record.ResourceDrops.Name);
    }

    private void FinishTask_HarvestResources_MoveToResource(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation previousTaskData = (BuildingTaskData_MoveToLocation) taskDataRaw;
        ChunkMapPointContent_WorldResource worldResource = previousTaskData.pathData.taskPoint.owner as ChunkMapPointContent_WorldResource;
        StartTask_HarvestResource_Gather(villager, worldResource, previousTaskData.pathData.taskPoint);
    }

    private void StartTask_HarvestResource_Gather(Villager villager, ChunkMapPointContent_WorldResource worldResource, VillagerTaskPoint taskPoint)
    {
        List<ChunkMapPointContent> dependencies = new List<ChunkMapPointContent>() { building };
        string taskDescription = string.Format("Gathering {0} from {1}", worldResource.record.ResourceDrops.Name, worldResource.name);
        BuildingTaskData_HarvestResources taskData = new BuildingTaskData_HarvestResources(dependencies, worldResource, taskPoint, (CardinalDirection)taskPoint.extraData, taskDescription);
        VillagerSubTask_HarvestResource task = new VillagerSubTask_HarvestResource(villager, this, taskData);
        task.finishedCallback = FinishTask_HarvestResources_Gather;

        villager.queuedTasks.Add(task);
    }

    private void FinishTask_HarvestResources_Gather(Villager villager, BuildingTaskData taskDataRaw)
    {
        //Didn't manage to gather anything before the worldresource disappeared
        if (villager.inventory.value == null)
            return;

        BuildingTaskData_HarvestResources taskDataOld = (BuildingTaskData_HarvestResources) taskDataRaw;
        ResourceDelivery delivery = new ResourceDelivery(villager, building.inventory, taskDataOld.worldResource.record.ResourceDrops, villager.inventory.value.amount.value);
        delivery.StartDelivery();
        StartTask_HarvestResources_DeliverResources(villager, delivery);
    }

    private void StartTask_HarvestResources_DeliverResources(Villager villager, ResourceDelivery delivery)
    {
        List<ChunkMapPointContent> dependencies = new List<ChunkMapPointContent>() { building };
        string taskDescription = string.Format("Delivering {0} {1} to {2}", villager.inventory.value.amount.value, villager.inventory.value.record.Name, building.name);
        BuildingTaskData_MoveToLocation taskData = new BuildingTaskData_MoveToLocation(dependencies, building.villagerTaskPoints, building.contentTaskLocation, false, taskDescription, delivery);
        VillagerSubTask_MoveToLocation task = new VillagerSubTask_MoveToLocation(villager, this, taskData);
        task.canceledCallback = CancelTask_HarvestResources_DeliverResources;
        task.finishedCallback = FinishTask_HarvestResources_DeliverResources;
        villager.queuedTasks.Add(task);
    }

    private void CancelTask_HarvestResources_DeliverResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDelivery delivery = (ResourceDelivery)taskDataOld.extraData;
        delivery.CancelDelivery();
    }

    private void FinishTask_HarvestResources_DeliverResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDelivery delivery = (ResourceDelivery)taskDataOld.extraData;
        delivery.DeliverDelivery();
    }
    #endregion
}