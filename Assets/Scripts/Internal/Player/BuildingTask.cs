
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingTask
{
    public BuildingTaskFillRate fillRate
    {
        get
        {
            if (assignedVillagers < targetVillagers)
                return BuildingTaskFillRate.NeedMore;

            if (assignedVillagers < maxVillagers)
                return BuildingTaskFillRate.DesiredAmount;

            return BuildingTaskFillRate.Full;
        }
    }

    public readonly ChunkMapPointContent_BuildingBase building;
    public readonly int maxVillagers;
    public readonly int targetVillagers;
    public readonly EventVariable<BuildingTask, BuildingTaskPriority> priority;
    public int assignedVillagers;

    public BuildingTask(ChunkMapPointContent_BuildingBase building, int targetVillagers, int maxVillagers)
    {
        this.building = building;
        this.priority = new EventVariable<BuildingTask, BuildingTaskPriority>(this, BuildingTaskPriority.Medium);
        this.targetVillagers = targetVillagers;
        this.maxVillagers = maxVillagers;
    }

    #region Get Resources
    protected bool TryStartTask_GetDesiredResources(Villager villager, int bonusCarryCapacity = 0)
    {
        ResourceReservationLocation resourceReservationLocation;
        if (!villager.owner.TryGetResourceReservationLocation(building.inventory, villager, bonusCarryCapacity, out resourceReservationLocation))
            return false;

        StartTask_GetDesiredResources(villager, resourceReservationLocation);
        return true;
    }

    private void StartTask_GetDesiredResources(Villager villager, ResourceReservationLocation resourceReservationLocation)
    {
        ResourceDeliveryReservation deliveryReservation = new ResourceDeliveryReservation(villager, resourceReservationLocation.building, building, resourceReservationLocation.record, resourceReservationLocation.amount);
        deliveryReservation.StartPickup();

        List<ChunkMapPointContent> dependencies = new List<ChunkMapPointContent>() { building, resourceReservationLocation.building };

        string taskDescription = string.Format("Picking up {0} {1} from {2} for delivery to {3}", deliveryReservation.amount, deliveryReservation.record.Name, deliveryReservation.fromBuilding.name, deliveryReservation.toBuilding.name);
        BuildingTaskData_MoveToLocation taskData = new BuildingTaskData_MoveToLocation(dependencies, deliveryReservation.fromBuilding.villagerTaskPoints, deliveryReservation.fromBuilding.contentTaskLocation, false, taskDescription, deliveryReservation);
        VillagerSubTask_MoveToLocation task = new VillagerSubTask_MoveToLocation(villager, this, taskData);
        task.canceledCallback = CancelTask_GetDesiredResources_PickUpResources;
        task.finishedCallback = FinishTask_GetDesiredResources_PickUpResources;

        villager.queuedTasks.Add(task);
    }

    private void CancelTask_GetDesiredResources_PickUpResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDeliveryReservation deliveryReservation = (ResourceDeliveryReservation)taskDataOld.extraData;
        deliveryReservation.CancelPickup();
    }

    private void FinishTask_GetDesiredResources_PickUpResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDeliveryReservation deliveryReservation = (ResourceDeliveryReservation)taskDataOld.extraData;
        deliveryReservation.PickUpDelivery();

        StartTask_GetDesiredResources_DeliverResources(villager, taskDataOld.dependencies, deliveryReservation);
    }

    private void StartTask_GetDesiredResources_DeliverResources(Villager villager, List<ChunkMapPointContent> dependencies, ResourceDeliveryReservation deliveryReservation)
    {
        string taskDescription = string.Format("Delivering {0} {1} to {2}", deliveryReservation.amount, deliveryReservation.record.Name, deliveryReservation.toBuilding.name);
        BuildingTaskData_MoveToLocation taskData = new BuildingTaskData_MoveToLocation(dependencies, deliveryReservation.toBuilding.villagerTaskPoints, deliveryReservation.toBuilding.contentTaskLocation, false, taskDescription, deliveryReservation);
        VillagerSubTask_MoveToLocation task = new VillagerSubTask_MoveToLocation(villager, this, taskData);
        task.canceledCallback = CancelTask_GetDesiredResources_DeliverResources;
        task.finishedCallback = FinishTask_GetDesiredResources_DeliverResources;
        villager.queuedTasks.Add(task);
    }

    private void CancelTask_GetDesiredResources_DeliverResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDeliveryReservation deliveryReservation = (ResourceDeliveryReservation)taskDataOld.extraData;
        deliveryReservation.CancelDelivery();
    }

    private void FinishTask_GetDesiredResources_DeliverResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDeliveryReservation deliveryReservation = (ResourceDeliveryReservation)taskDataOld.extraData;
        deliveryReservation.DeliverDelivery();
    }
    #endregion

    #region Deliver Resources
    protected bool TryStartTask_DeliverResources(Villager villager, int bonusCarryCapacity = 0)
    {
        ResourceReservationLocation resourceReservationLocation;
        if (!villager.owner.TryGetResourceDeliveryLocation(building.inventory, villager, bonusCarryCapacity, out resourceReservationLocation))
            return false;

        StartTask_GetDeliveryResources(villager, resourceReservationLocation);
        return true;
    }

    private void StartTask_GetDeliveryResources(Villager villager, ResourceReservationLocation resourceReservationLocation)
    {
        ResourceDeliveryReservation deliveryReservation = new ResourceDeliveryReservation(villager, building, resourceReservationLocation.building, resourceReservationLocation.record, resourceReservationLocation.amount);
        deliveryReservation.StartPickup();

        List<ChunkMapPointContent> dependencies = new List<ChunkMapPointContent>() { building, resourceReservationLocation.building };

        string taskDescription = string.Format("Picking up {0} {1} from {2} for delivery to {3}", deliveryReservation.amount, deliveryReservation.record.Name, deliveryReservation.fromBuilding.name, deliveryReservation.toBuilding.name);
        BuildingTaskData_MoveToLocation taskData = new BuildingTaskData_MoveToLocation(dependencies, deliveryReservation.fromBuilding.villagerTaskPoints, deliveryReservation.fromBuilding.contentTaskLocation, false, taskDescription, deliveryReservation);
        VillagerSubTask_MoveToLocation task = new VillagerSubTask_MoveToLocation(villager, this, taskData);
        task.canceledCallback = CancelTask_DeliverResources_PickUpResources;
        task.finishedCallback = FinishTask_DeliverResources_PickUpResources;

        villager.queuedTasks.Add(task);
    }

    private void CancelTask_DeliverResources_PickUpResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDeliveryReservation deliveryReservation = (ResourceDeliveryReservation)taskDataOld.extraData;
        deliveryReservation.CancelPickup();
    }

    private void FinishTask_DeliverResources_PickUpResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDeliveryReservation deliveryReservation = (ResourceDeliveryReservation)taskDataOld.extraData;
        deliveryReservation.PickUpDelivery();

        StartTask_DeliverResources_DeliverResources(villager, taskDataOld.dependencies, deliveryReservation);
    }

    private void StartTask_DeliverResources_DeliverResources(Villager villager, List<ChunkMapPointContent> dependencies, ResourceDeliveryReservation deliveryReservation)
    {
        string taskDescription = string.Format("Delivering {0} {1} to {2}", deliveryReservation.amount, deliveryReservation.record.Name, deliveryReservation.toBuilding.name);
        BuildingTaskData_MoveToLocation taskData = new BuildingTaskData_MoveToLocation(dependencies, deliveryReservation.toBuilding.villagerTaskPoints, deliveryReservation.toBuilding.contentTaskLocation, false, taskDescription, deliveryReservation);
        VillagerSubTask_MoveToLocation task = new VillagerSubTask_MoveToLocation(villager, this, taskData);
        task.canceledCallback = CancelTask_DeliverResources_DeliverResources;
        task.finishedCallback = FinishTask_DeliverResources_DeliverResources;
        villager.queuedTasks.Add(task);
    }

    private void CancelTask_DeliverResources_DeliverResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDeliveryReservation deliveryReservation = (ResourceDeliveryReservation)taskDataOld.extraData;
        deliveryReservation.CancelDelivery();
    }

    private void FinishTask_DeliverResources_DeliverResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDeliveryReservation deliveryReservation = (ResourceDeliveryReservation)taskDataOld.extraData;
        deliveryReservation.DeliverDelivery();
    }
    #endregion

    #region Remove Resources
    protected bool TryStartTask_RemoveUndesiredResources(Villager villager, int bonusCarryCapacity = 0)
    {
        Resource highestResource = default;
        int highestOverflowAmount = 0;

        foreach (Resource resource in building.inventory.resources.Values)
        {
            int overflowAmount = Mathf.Min(-resource.currentMissingAmount.value, villager.carryCapacity + bonusCarryCapacity);

            if (overflowAmount <= highestOverflowAmount)
                continue;

            highestOverflowAmount = overflowAmount;
            highestResource = resource;
        }

        if (highestOverflowAmount == 0)
            return false;

        ResourceReservation reservation = new ResourceReservation(villager, building.inventory, highestResource.record, highestOverflowAmount);
        StartTask_RemoveUndesiredResources(villager, reservation);
        return true;
    }

    private void StartTask_RemoveUndesiredResources(Villager villager, ResourceReservation reservation)
    {
        List<ChunkMapPointContent> dependencies = new List<ChunkMapPointContent>() { building};
        string taskDescription = string.Format("Picking up {0} {1} from {2}", reservation.amount, reservation.record.Name, building.name);
        BuildingTaskData_MoveToLocation taskData = new BuildingTaskData_MoveToLocation(dependencies, building.villagerTaskPoints, building.contentTaskLocation, false, taskDescription, reservation);
        VillagerSubTask_MoveToLocation task = new VillagerSubTask_MoveToLocation(villager, this, taskData);
        task.canceledCallback = CancelTask_RemoveUndesiredResources_MoveToLocation;
        task.finishedCallback = FinishTask_RemoveUndesiredResources_MoveToLocation;
        reservation.StartPickup();

        villager.queuedTasks.Add(task);
    }

    private void CancelTask_RemoveUndesiredResources_MoveToLocation(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceReservation reservation = (ResourceReservation)taskDataOld.extraData;
        reservation.CancelPickup();
    }

    private void FinishTask_RemoveUndesiredResources_MoveToLocation(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceReservation reservation = (ResourceReservation)taskDataOld.extraData;
        reservation.PickUpDelivery();
    }
    #endregion

    #region Offload Resources
    protected bool TryStartTask_OffloadResources(Villager villager, int bonusCarryCapacity = 0)
    {
        Resource highestResource = default;
        int highestOverflowAmount = 0;

        foreach (Resource resource in building.inventory.resources.Values)
        {
            int overflowAmount = Mathf.Min(-resource.currentMissingAmount.value, villager.carryCapacity + bonusCarryCapacity);

            if (overflowAmount <= highestOverflowAmount)
                continue;

            highestOverflowAmount = overflowAmount;
            highestResource = resource;
        }

        if (highestOverflowAmount == 0)
            return false;

        StartTask_OffloadResources(villager, highestResource, highestOverflowAmount);
        return true;
    }

    private void StartTask_OffloadResources(Villager villager, Resource resource, int amount)
    {
        ResourceDeliveryReservation deliveryReservation = new ResourceDeliveryReservation(villager, building, building.owner.townCenterBuilding, resource.record, amount);
        deliveryReservation.StartPickup();

        List<ChunkMapPointContent> dependencies = new List<ChunkMapPointContent>() { building, building.owner.townCenterBuilding };

        string taskDescription = string.Format("Delivering {0} {1} to {2}", deliveryReservation.amount, deliveryReservation.record.Name, deliveryReservation.toBuilding.name);
        BuildingTaskData_MoveToLocation taskData = new BuildingTaskData_MoveToLocation(dependencies, deliveryReservation.fromBuilding.villagerTaskPoints, deliveryReservation.fromBuilding.contentTaskLocation, false, taskDescription, deliveryReservation);
        VillagerSubTask_MoveToLocation task = new VillagerSubTask_MoveToLocation(villager, this, taskData);
        task.canceledCallback = CancelTask_OffloadResources_PickUpResources;
        task.finishedCallback = FinishTask_OffloadResources_PickUpResources;

        villager.queuedTasks.Add(task);
    }

    private void CancelTask_OffloadResources_PickUpResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDeliveryReservation deliveryReservation = (ResourceDeliveryReservation)taskDataOld.extraData;
        deliveryReservation.CancelPickup();
    }

    private void FinishTask_OffloadResources_PickUpResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDeliveryReservation deliveryReservation = (ResourceDeliveryReservation)taskDataOld.extraData;
        deliveryReservation.PickUpDelivery();

        StartTask_OffloadResources_DeliverResources(villager, taskDataOld.dependencies, deliveryReservation);
    }

    private void StartTask_OffloadResources_DeliverResources(Villager villager, List<ChunkMapPointContent> dependencies, ResourceDeliveryReservation deliveryReservation)
    {
        string taskDescription = string.Format("Delivering {0} {1} to {2}", deliveryReservation.amount, deliveryReservation.record.Name, deliveryReservation.toBuilding.name);
        BuildingTaskData_MoveToLocation taskData = new BuildingTaskData_MoveToLocation(dependencies, deliveryReservation.toBuilding.villagerTaskPoints, deliveryReservation.toBuilding.contentTaskLocation, false, taskDescription, deliveryReservation);
        VillagerSubTask_MoveToLocation task = new VillagerSubTask_MoveToLocation(villager, this, taskData);
        task.canceledCallback = CancelTask_OffloadResources_DeliverResources;
        task.finishedCallback = FinishTask_OffloadResources_DeliverResources;
        villager.queuedTasks.Add(task);
    }

    private void CancelTask_OffloadResources_DeliverResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDeliveryReservation deliveryReservation = (ResourceDeliveryReservation)taskDataOld.extraData;
        deliveryReservation.CancelDelivery();
    }

    private void FinishTask_OffloadResources_DeliverResources(Villager villager, BuildingTaskData taskDataRaw)
    {
        BuildingTaskData_MoveToLocation taskDataOld = (BuildingTaskData_MoveToLocation)taskDataRaw;
        ResourceDeliveryReservation deliveryReservation = (ResourceDeliveryReservation)taskDataOld.extraData;
        deliveryReservation.DeliverDelivery();
    }
    #endregion

    public abstract bool TryReserveVillagerTaskData(Villager villager);
}