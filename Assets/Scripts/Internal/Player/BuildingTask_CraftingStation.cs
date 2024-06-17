
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingTask_CraftingStation : BuildingTask
{
    public readonly new ChunkMapPointContent_Building_CraftingStation building;

    public BuildingTask_CraftingStation(ChunkMapPointContent_Building_CraftingStation building, int targetVillagers, int maxVillagers)
        :base(building, targetVillagers, maxVillagers)
    {
        this.building = building;
    }

    public override bool TryReserveVillagerTaskData(Villager villager)
    {
        if (TryStartTask_CraftItem(villager))
            return true;

        if(building.insideEnterExitPoints.Any(i => i.globalPoint == villager.point.value))
        {
            if (TryStartTask_EmptyInventory(villager))
                return true;

            if (TryStartTask_CollectCraftingMaterials(villager))
                return true;
        }
        else
        {
            if (TryStartTask_CollectCraftingMaterials(villager))
                return true;

            if (TryStartTask_EmptyInventory(villager))
                return true;
        }

        return false;
    }

    #region Craft Item

    private bool TryStartTask_CraftItem(Villager villager)
    {
        CraftingSlot craftingSlot;
        if (!building.craftingSlots.TryFind(i => i.resourcesAssigned.value && !i.villagerAssigned, out craftingSlot))
            return false;

        StartTask_CraftItem_MoveToBuilding(villager, craftingSlot);
        return true;
    }

    private void StartTask_CraftItem_MoveToBuilding(Villager villager, CraftingSlot craftingSlot)
    {
        List<ChunkMapPointContent> dependencies = new List<ChunkMapPointContent>() { building };
        string taskDescription = string.Format("Moving to {0} to craft {1}", building.name, building.record.CraftingRecipes.CraftedResource.Name);
        BuildingTaskData_MoveToLocation taskData = new BuildingTaskData_MoveToLocation(dependencies, building.villagerTaskPoints, building.contentTaskLocation, false, taskDescription, craftingSlot);
        VillagerSubTask_MoveToLocation task = new VillagerSubTask_MoveToLocation(villager, this, taskData);
        craftingSlot.villagerAssigned = true;
        task.canceledCallback = CancelTask_CraftItem_MoveToBuilding;
        task.finishedCallback = FinishTask_CraftItem_MoveToBuilding;

        villager.queuedTasks.Add(task);
    }

    private void CancelTask_CraftItem_MoveToBuilding(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        CraftingSlot craftingSlot = (CraftingSlot)taskDataOld.extraData;
        craftingSlot.villagerAssigned = false;
    }

    private void FinishTask_CraftItem_MoveToBuilding(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        CraftingSlot craftingSlot = (CraftingSlot)taskDataOld.extraData;
        StartTask_CraftItem_Craft(villager, craftingSlot);
    }

    private void StartTask_CraftItem_Craft(Villager villager, CraftingSlot craftingSlot)
    {
        List<ChunkMapPointContent> dependencies = new List<ChunkMapPointContent>() { building };
        string taskDescription = string.Format("Crafing {0}", building.record.CraftingRecipes.CraftedResource.Name);
        BuildingTaskData_CraftItem taskData = new BuildingTaskData_CraftItem(dependencies, building, craftingSlot, taskDescription);
        VillagerSubTask_CraftItem task = new VillagerSubTask_CraftItem(villager, this, taskData);
        task.canceledCallback = CancelTask_CraftItem_Craft;
        task.finishedCallback = FinishTask_CraftItem_Craft;
        villager.queuedTasks.Add(task);
    }

    private void CancelTask_CraftItem_Craft(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_CraftItem taskDataOld = (BuildingTaskData_CraftItem)taskDataRaw;
        taskDataOld.craftingSlot.villagerAssigned = false;
    }

    private void FinishTask_CraftItem_Craft(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_CraftItem taskDataOld = (BuildingTaskData_CraftItem)taskDataRaw;
        taskDataOld.craftingSlot.villagerAssigned = false;
    }
    #endregion

    #region Collect Crafting Materials

    private bool TryStartTask_CollectCraftingMaterials(Villager villager)
    {
        return TryStartTask_GetDesiredResources(villager);
    }

    #endregion

    #region Empty Inventory

    private bool TryStartTask_EmptyInventory(Villager villager)
    {
        return TryStartTask_RemoveUndesiredResources(villager);
    }
    #endregion
}