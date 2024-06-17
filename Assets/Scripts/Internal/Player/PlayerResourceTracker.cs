
using SheetCodes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourceTracker
{
    public readonly Player player;
    public readonly Dictionary<ResourcesIdentifier, TotalResourceData> totalResourceData;
    
    public PlayerResourceTracker(Player player)
    {
        ResourcesIdentifier[] resourcesIdentifiers = Enum.GetValues(typeof(ResourcesIdentifier)) as ResourcesIdentifier[];
        totalResourceData = new Dictionary<ResourcesIdentifier, TotalResourceData>();
        foreach(ResourcesIdentifier identifier in resourcesIdentifiers)
        {
            if (identifier == ResourcesIdentifier.None)
                continue;

            ResourcesRecord record = identifier.GetRecord();
            totalResourceData.Add(identifier, new TotalResourceData(record));
        }

        player.villagers.onAdd += OnAdd_Villager;
        player.villagers.onRemove += OnRemove_Villager;

        player.buildings.onAdd += OnAdd_Building;
        player.buildings.onRemove += OnRemove_Building;

        foreach (Villager villager in player.villagers)
            OnAdd_Villager(villager);
       
        foreach (ChunkMapPointContent_BuildingBase building in player.buildings)
            OnAdd_Building(building);
    }

    private void OnAdd_Villager(Villager villager)
    {
        villager.inventory.onValueChangeImmediate += OnValueChanged_Villager_Inventory;
    }

    private void OnRemove_Villager(Villager villager)
    {
        villager.inventory.onValueChangeImmediate -= OnValueChanged_Villager_Inventory;
    }

    private void OnValueChanged_Villager_Inventory(ResourceCount oldValue, ResourceCount newValue)
    {
        if (oldValue != null)
        {
            oldValue.amount.onValueChangeImmediateSource -= OnValueChanged_Villager_Inventory_Amount;
        }

        if(newValue != null)
        {
            newValue.amount.onValueChangeImmediateSource += OnValueChanged_Villager_Inventory_Amount;
        }
    }

    private void OnValueChanged_Villager_Inventory_Amount(ResourceCount source, int oldValue, int newValue)
    {
        totalResourceData[source.record.Identifier].totalUndesiredAmount.value += newValue - oldValue;
    }

    private void OnAdd_Building(ChunkMapPointContent_BuildingBase building)
    {
        foreach(Resource resource in building.inventory.resources.Values)
        {
            resource.totalUndesiredAmount.onValueChangeImmediateSource += OnValueChanged_BuildingResource_UndesiredAmount;
            resource.currentMissingAmount.onValueChangeImmediateSource += OnValueChanged_BuildingResource_CurrentMissingAmount;
        }
    }

    private void OnRemove_Building(ChunkMapPointContent_BuildingBase building)
    {
        foreach (Resource resource in building.inventory.resources.Values)
        {
            resource.totalUndesiredAmount.onValueChangeImmediateSource -= OnValueChanged_BuildingResource_UndesiredAmount;
            resource.currentMissingAmount.onValueChangeImmediateSource -= OnValueChanged_BuildingResource_CurrentMissingAmount;
        }
    }

    private void OnValueChanged_BuildingResource_UndesiredAmount(Resource source, int oldValue, int newValue)
    {
        totalResourceData[source.record.Identifier].totalUndesiredAmount.value += newValue - oldValue;
    }


    private void OnValueChanged_BuildingResource_CurrentMissingAmount(Resource source, int oldValue, int newValue)
    {
        totalResourceData[source.record.Identifier].currentMissingAmount.value += Mathf.Max(0, newValue) - Mathf.Max(0, oldValue);
    }
}