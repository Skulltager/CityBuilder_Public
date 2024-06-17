using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingTask_DeconstructBuilding : BuildingTask
{
    public readonly new ChunkMapPointContent_BuildingDeconstruction building;

    public BuildingTask_DeconstructBuilding(ChunkMapPointContent_BuildingDeconstruction building, int targetVillagers, int maxVillager)
        : base(building, targetVillagers, maxVillager)
    {
        this.building = building;
    }

    public override bool TryReserveVillagerTaskData(Villager villager)
    {
        if (TryReserveVillagerTaskData_MoveResources(villager))
            return true;

        if (TryReserveVillagerTaskData_DeconstructBuilding(villager))
            return true;

        return false;
    }

    #region Deconstruct Building
    private bool TryReserveVillagerTaskData_DeconstructBuilding(Villager villager)
    {
        if (building.deconstructionProgress.value >= 1)
            return false;

        if (!building.villagerTaskPoints.Any(i => i.enabled.value))
            return false;

        StartTask_DeconstructBuilding_MoveToConstruction(villager);
        return true;
    }

    private void StartTask_DeconstructBuilding_MoveToConstruction(Villager villager)
    {
        List<ChunkMapPointContent> dependencies = new List<ChunkMapPointContent>() { building };

        string taskDescription = string.Format("Moving to {0} to help with deconstruction", building.name);
        BuildingTaskData_MoveToLocation taskData = new BuildingTaskData_MoveToLocation(dependencies, building.villagerTaskPoints, building.contentTaskLocation, true, taskDescription);
        VillagerSubTask_MoveToLocation task = new VillagerSubTask_MoveToLocation(villager, this, taskData);
        task.finishedCallback = FinishTask_DeconstructBuilding_MoveToDeconstruction;

        villager.queuedTasks.Add(task);
    }

    private void FinishTask_DeconstructBuilding_MoveToDeconstruction(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        StartTask_DeconstructBuilding_Demolish(villager,taskDataOld.dependencies, taskDataOld.pathData.taskPoint);
    }

    private void StartTask_DeconstructBuilding_Demolish(Villager villager, List<ChunkMapPointContent> dependencies, VillagerTaskPoint villagerTaskPoint)
    {
        string taskDescription = string.Format("Helping with the deconstruction of {0}", building.name);
        BuildingTaskData_DeconstructBuilding taskData = new BuildingTaskData_DeconstructBuilding(dependencies, building, villagerTaskPoint, taskDescription);
        VillagerSubTask_DeconstructBuilding task = new VillagerSubTask_DeconstructBuilding(villager, this, taskData);

        villager.queuedTasks.Add(task);
    }

    #endregion

    #region Empty Deconstruction Site

    private bool TryReserveVillagerTaskData_MoveResources(Villager villager)
    {
        if (building.deconstructionProgress.value < 1)
            return false;

        return TryStartTask_RemoveUndesiredResources(villager);
    }
    #endregion
}