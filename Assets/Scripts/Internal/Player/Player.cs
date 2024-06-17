using SheetCodes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public readonly PlayerResourceTracker resourceTracker;
    public readonly ChunkRegion playerRegion;
    public readonly EventList<ChunkMapPointContent_BuildingBase> buildings;
    public readonly EventList<Villager> villagers;
    public readonly Dictionary<BuildingIdentifier, PlayerBuildingLimits> buildingLimits;

    public ChunkMapPointContent_Building_TownCenter townCenterBuilding;

    public Player(ChunkRegion playerRegion)
    {
        this.playerRegion = playerRegion;
        playerRegion.owner = this;
        buildings = new EventList<ChunkMapPointContent_BuildingBase>();
        villagers = new EventList<Villager>();
        resourceTracker = new PlayerResourceTracker(this);
        buildingLimits = new Dictionary<BuildingIdentifier, PlayerBuildingLimits>();

        BuildingIdentifier[] buildingIdentifiers = Enum.GetValues(typeof(BuildingIdentifier)) as BuildingIdentifier[];
        foreach(BuildingIdentifier buildingIdentifier in buildingIdentifiers)
        {
            if (buildingIdentifier == BuildingIdentifier.None)
                continue;

            BuildingRecord buildingRecord = buildingIdentifier.GetRecord();
            PlayerBuildingLimits buildingLimit = new PlayerBuildingLimits(buildingRecord);
            buildingLimit.limit.value = buildingRecord.StartingLimit;
            buildingLimits.Add(buildingIdentifier, buildingLimit);

        }

        buildings.onAdd += OnAdd_Building_WaitForTownCenter;

        buildings.onAdd += OnAdd_Building;
        buildings.onRemove += OnRemove_Building;
    }

    private void OnAdd_Building_WaitForTownCenter(ChunkMapPointContent_BuildingBase building)
    {
        if (building.record.Identifier != BuildingIdentifier.TownCenter)
            return;

        buildings.onAdd -= OnAdd_Building_WaitForTownCenter;
        townCenterBuilding = building as ChunkMapPointContent_Building_TownCenter;

        foreach (Villager villager in villagers)
            OnAdd_Villager(villager);

        villagers.onAdd += OnAdd_Villager;
        villagers.onRemove += OnRemove_Villager;
    }

    private void OnAdd_Building(ChunkMapPointContent_BuildingBase building)
    {
        buildingLimits[building.record.Identifier].placedAmount.value++;
    }

    private void OnRemove_Building(ChunkMapPointContent_BuildingBase building)
    {
        buildingLimits[building.record.Identifier].placedAmount.value--;
    }

    private void OnAdd_Villager(Villager villager)
    {
        villager.assignedBuilding.value = townCenterBuilding;
        villager.Show();
    }

    private void OnRemove_Villager(Villager villager)
    {
        villager.assignedBuilding.value = null;
        villager.Hide();
    }

    public void Dispose()
    {
        foreach(Villager villager in villagers)
            villager.Hide();

        villagers.onAdd -= OnAdd_Villager;
        villagers.onRemove -= OnRemove_Villager;
    }

    public bool TryGetResourceReservationLocation(Inventory inventory, Villager villager, int bonusCarryCapacity, out ResourceReservationLocation resourceReservationLocation)
    {
        Point startPoint = villager.point.value;

        ChunkMapPointContent_BuildingBase closestBuilding = default;
        int closestBuildingDistance = int.MaxValue;
        int resourceCount = 0;
        Resource closestBuildingResource = default;

        foreach(ChunkMapPointContent_BuildingBase building in buildings)
        {
            if (building.inventory == inventory)
                continue;

            if (!building.allowResourcePickup)
                continue;

            if (building.inventory.softDesires.value)
            {
                foreach (Resource resource in building.inventory.resources.Values)
                {
                    Resource localResource;
                    if (!inventory.resources.TryGetValue(resource.record.Identifier, out localResource))
                        continue;

                    int resourceTransferable = Mathf.Min(localResource.missingAmount.value, resource.unreservedAmount.value, villager.carryCapacity + bonusCarryCapacity, inventory.capacityRemaining);
                    if (resourceTransferable < resourceCount)
                        continue;

                    int distance = startPoint.DistanceToPoint(building.centerPoint);

                    if (resourceTransferable == resourceCount && distance >= closestBuildingDistance)
                        continue;

                    resourceCount = resourceTransferable;
                    closestBuildingDistance = distance;
                    closestBuilding = building;
                    closestBuildingResource = resource;
                }
            }
            else
            {
                foreach (Resource resource in building.inventory.resources.Values)
                {
                    Resource localResource;
                    if (!inventory.resources.TryGetValue(resource.record.Identifier, out localResource))
                        continue;

                    int resourceTransferable = Mathf.Min(localResource.missingAmount.value, resource.availableAmount.value, villager.carryCapacity + bonusCarryCapacity, inventory.capacityRemaining);
                    if (resourceTransferable < resourceCount)
                        continue;

                    int distance = startPoint.DistanceToPoint(building.centerPoint);

                    if (resourceTransferable == resourceCount && distance >= closestBuildingDistance)
                        continue;

                    resourceCount = resourceTransferable;
                    closestBuildingDistance = distance;
                    closestBuilding = building;
                    closestBuildingResource = resource;
                }
            }
        }

        if (resourceCount == 0)
        {
            resourceReservationLocation = default;
            return false;
        }

        resourceReservationLocation = new ResourceReservationLocation(closestBuilding, closestBuildingResource.record, resourceCount);
        return true;
    }


    public bool TryGetResourceDeliveryLocation(Inventory inventory, Villager villager, int bonusCarryCapacity, out ResourceReservationLocation resourceReservationLocation)
    {
        Point startPoint = villager.point.value;

        ChunkMapPointContent_BuildingBase closestBuilding = default;
        int closestBuildingDistance = int.MaxValue;
        int resourceCount = 0;
        Resource closestBuildingResource = default;

        foreach (ChunkMapPointContent_BuildingBase building in buildings)
        {
            if (building.inventory == inventory)
                continue;

            foreach (Resource resource in building.inventory.resources.Values)
            {
                Resource localResource;
                if (!inventory.resources.TryGetValue(resource.record.Identifier, out localResource))
                    continue;


                int resourceTransferable = Mathf.Min(localResource.unreservedAmount.value, resource.missingAmount.value, villager.carryCapacity + bonusCarryCapacity, inventory.capacityRemaining);
                if (resourceTransferable < resourceCount)
                    continue;

                int distance = startPoint.DistanceToPoint(building.centerPoint);

                if (resourceTransferable == resourceCount && distance >= closestBuildingDistance)
                    continue;

                resourceCount = resourceTransferable;
                closestBuildingDistance = distance;
                closestBuilding = building;
                closestBuildingResource = resource;
            }
        }

        if (resourceCount == 0)
        {
            resourceReservationLocation = default;
            return false;
        }

        resourceReservationLocation = new ResourceReservationLocation(closestBuilding, closestBuildingResource.record, resourceCount);
        return true;
    }

    public ChunkMapPointContent_BuildingBase GetResourceDumpingLocation(Villager villager)
    {
        Point startPoint = villager.point.value;

        ChunkMapPointContent_BuildingBase closestBuilding = default;
        int closestBuildingDistance = int.MaxValue;

        foreach (ChunkMapPointContent_BuildingBase building in buildings)
        {
            if (building.inventory.capacityRemaining < villager.inventory.value.amount.value)
                continue;

            Resource resource;
            if (!building.inventory.resources.TryGetValue(villager.inventory.value.record.Identifier, out resource))
                continue;

            if (resource.missingAmount.value <= villager.inventory.value.amount.value)
                continue;

            int distance = startPoint.DistanceToPoint(building.centerPoint);
            if (distance > closestBuildingDistance)
                continue;

            closestBuildingDistance = distance;
            closestBuilding = building;
        }

        if (closestBuilding == null)
        {
            foreach (ChunkMapPointContent_BuildingBase building in buildings)
            {
                if (!building.allowAnyResourceDumping)
                    continue;

                if (!building.inventory.HasEnoughInventorySize(villager.inventory.value.amount.value))
                    continue;

                int distance = startPoint.DistanceToPoint(building.centerPoint);
                if (distance > closestBuildingDistance)
                    continue;

                closestBuildingDistance = distance;
                closestBuilding = building;
            }
        }

        return closestBuilding;
    }

    public void AssignVillagerToBuilding(ChunkMapPointContent_Building building)
    {
        Villager closestVillager = default;
        int closestDistance = int.MaxValue;

        foreach(Villager villager in townCenterBuilding.assignedVillagers)
        {
            int distance = villager.point.value.DistanceToPoint(building.centerPoint);
            if (distance >= closestDistance)
                continue;

            closestVillager = villager;
            closestDistance = distance;
        }

        closestVillager.assignedBuilding.value = building;
    }

    public void TryAssignMaxVillagersToBuilding(ChunkMapPointContent_Building building)
    {
        int lowerLimit = Mathf.Max(0, townCenterBuilding.assignedVillagers.Count - building.record.MaximumAssignedVillagers);

        for (int i = townCenterBuilding.assignedVillagers.Count - 1; i >= lowerLimit; i--)
            townCenterBuilding.assignedVillagers[i].assignedBuilding.value = building;
    }

    public void UnassignVillagerFromBuilding(ChunkMapPointContent_Building building)
    {
        Villager villager = building.assignedVillagers[0];
        villager.assignedBuilding.value = townCenterBuilding;
    }

    public void UnassignAllVillagersFromBuilding(ChunkMapPointContent_Building building)
    {
        for(int i = building.assignedVillagers.Count - 1; i >= 0; i--)
            building.assignedVillagers[i].assignedBuilding.value = townCenterBuilding;
    }
}