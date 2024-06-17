using SheetCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkMapPointContent_Building_CraftingStation : ChunkMapPointContent_Building
{
    public const int CRAFTING_SLOTS = 3;

    public readonly CraftingSlot[] craftingSlots;
    public readonly CraftingSetting[] craftingSettings;
    private readonly BuildingTask_CraftingStation buildingTask;

    public ChunkMapPointContent_Building_CraftingStation(Map map, Point centerPoint, CardinalDirection direction, Player owner, ChunkMapPointContent_BuildingConstruction construction)
        : base(map, centerPoint, direction, BuildingIdentifier.CraftingStation.GetRecord(), owner, construction)
    {
        craftingSlots = new CraftingSlot[CRAFTING_SLOTS];
        craftingSettings = new CraftingSetting[record.CraftingRecipes.RecipeCosts.Length];

        for (int i = 0; i < record.CraftingRecipes.RecipeCosts.Length; i++)
        { 
            CraftingSetting craftingSetting = new CraftingSetting(record.CraftingRecipes.RecipeCosts[i]);
            craftingSetting.selectedCost.onValueChange += OnValueChanged_CraftingSetting_SelectedCost;
            craftingSettings[i] = craftingSetting;
        }

        for (int i = 0; i < craftingSlots.Length; i++)
            craftingSlots[i] = new CraftingSlot(this);

        buildingTask = new BuildingTask_CraftingStation(this, 3, 3);
    }

    public override void PlaceOnMap()
    {
        base.PlaceOnMap();

        inventory.onItemsDelivered += OnEvent_Inventory_ItemsDelivered;
        RecalculateDesiredResourcesAndCraftingSlots();
    }

    private void OnEvent_Inventory_ItemsDelivered()
    {
        RecalculateDesiredResourcesAndCraftingSlots();
    }

    private void OnValueChanged_CraftingSetting_SelectedCost(ResourceCostRecord oldValue, ResourceCostRecord newValue)
    {
        RecalculateDesiredResourcesAndCraftingSlots();
    }

    public void RecalculateDesiredResourcesAndCraftingSlots()
    {
        foreach (CraftingSlot craftingSlot in craftingSlots)
        {
            if (craftingSlot.resourcesAssigned.value)
                continue;
        
            bool anyFound = false;
            foreach (CraftingSetting craftingSetting in craftingSettings)
            {
                if (inventory.resources[craftingSetting.selectedCost.value.Resource.Identifier].unreservedAmount.value >= craftingSetting.selectedCost.value.Amount)
                    continue;
        
                anyFound = true;
                break;
            }
        
            if(anyFound)
                break;

            craftingSlot.resourcesAssigned.value = true;
            for(int i = 0; i < craftingSettings.Length; i++)
            {
                CraftingSetting craftingSetting = craftingSettings[i];
                inventory.resources[craftingSetting.selectedCost.value.Resource.Identifier].lockedAmount.value += craftingSetting.selectedCost.value.Amount;
                craftingSlot.selectedResourceCosts[i] = craftingSetting.selectedCost.value;
            }
        }

        ResourcesIdentifier[] identifiers = Enum.GetValues(typeof(ResourcesIdentifier)) as ResourcesIdentifier[];
        
        Dictionary<ResourcesIdentifier, int> currentDesiredAmounts = new Dictionary<ResourcesIdentifier, int>();
        foreach (ResourcesIdentifier identifier in identifiers)
        {
            if (identifier == ResourcesIdentifier.None)
                continue;
        
            currentDesiredAmounts.Add(identifier, 0);
        }

        int remainingStorageAllowed = record.InventorySize;

        foreach (CraftingSlot craftingSlot in craftingSlots)
        {
            if (!craftingSlot.resourcesAssigned.value)
                continue;

            foreach (ResourceCostRecord resourceCost in craftingSlot.selectedResourceCosts)
            {
                remainingStorageAllowed -= resourceCost.Amount;
                currentDesiredAmounts[resourceCost.Resource.Identifier] += resourceCost.Amount;
            }
        }

        int storagePerItem = 1;
        foreach(CraftingSetting craftingSetting in craftingSettings)
            storagePerItem += craftingSetting.selectedCost.value.Amount;

        int allowedStorageAmount = remainingStorageAllowed / storagePerItem;

        foreach (CraftingSetting craftingSetting in craftingSettings)
            currentDesiredAmounts[craftingSetting.selectedCost.value.Resource.Identifier] += craftingSetting.selectedCost.value.Amount * allowedStorageAmount;

        foreach (KeyValuePair<ResourcesIdentifier, int> keyValuePair in currentDesiredAmounts)
        {
            inventory.resources[keyValuePair.Key].currentDesiredAmount.value = keyValuePair.Value;
            inventory.resources[keyValuePair.Key].totalDesiredAmount.value = keyValuePair.Value;
        }
    }

    public override void RemoveFromMap(ChunkMapPointContent replacement = null)
    {
        base.RemoveFromMap(replacement);

        inventory.onItemsDelivered -= OnEvent_Inventory_ItemsDelivered;
    }

    public override bool TryAssignVillagerTasks(Villager villager)
    {
        if (!buildingTask.TryReserveVillagerTaskData(villager))
            return false;

        return true;
    }
}