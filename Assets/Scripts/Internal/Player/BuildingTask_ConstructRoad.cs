using System.Collections.Generic;
using System.Linq;

public class BuildingTask_ConstructRoad : BuildingTask
{
    public readonly new ChunkMapPointContent_RoadConstruction building;

    public BuildingTask_ConstructRoad(ChunkMapPointContent_RoadConstruction building, int targetVillagers, int maxVillager)
        : base(building, targetVillagers, maxVillager)
    {
        this.building = building;
    }

    public override bool TryReserveVillagerTaskData(Villager villager)
    {
        if (TryStartTask_GetConstructionResources(villager))
            return true;

        if (TryReserveVillagerTaskData_Construction(villager))
            return true;

        return false;
    }

    #region Get Construction Resources
    private bool TryStartTask_GetConstructionResources(Villager villager)
    {
        if (building.resourcesProgress.value == 1)
            return false;

        return TryStartTask_GetDesiredResources(villager);
    }

    #endregion

    #region Construct Building
    private bool TryReserveVillagerTaskData_Construction(Villager villager)
    {
        if (building.resourcesProgress.value < 1)
            return false;

        if (!building.villagerTaskPoints.Any(i => i.enabled.value))
            return false;

        StartTask_ConstructBuilding_MoveToConstruction(villager);
        return true;
    }

    private void StartTask_ConstructBuilding_MoveToConstruction(Villager villager)
    {
        List<ChunkMapPointContent> dependencies = new List<ChunkMapPointContent>() { building };
        string taskDescription = string.Format("Moving to {0} to help with construction", building.name);
        BuildingTaskData_MoveToLocation taskData = new BuildingTaskData_MoveToLocation(dependencies, building.villagerTaskPoints, building.contentTaskLocation, true, taskDescription);
        VillagerSubTask_MoveToLocation task = new VillagerSubTask_MoveToLocation(villager, this, taskData);
        task.finishedCallback = FinishTask_ConstructBuilding_MoveToConstruction;

        villager.queuedTasks.Add(task);
    }

    private void FinishTask_ConstructBuilding_MoveToConstruction(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        StartTask_ConstructBuilding_Build(villager, taskDataOld.pathData.taskPoint);
    }

    private void StartTask_ConstructBuilding_Build(Villager villager, VillagerTaskPoint taskPoint)
    {
        List<ChunkMapPointContent> dependencies = new List<ChunkMapPointContent>() { building };
        string taskDescription = string.Format("Helping with the construction of {0}", building.name);
        BuildingTaskData_ConstructRoad taskData = new BuildingTaskData_ConstructRoad(dependencies, building, taskPoint, taskDescription);
        VillagerSubTask_ConstructRoad task = new VillagerSubTask_ConstructRoad(villager, this, taskData);

        villager.queuedTasks.Add(task);
    }
    #endregion
}