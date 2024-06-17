
using System.Collections.Generic;
using UnityEngine;

public class BuildingOverviewUISegment_InventoryAdjustable : DataDrivenUI<Inventory_Limited>
{
    [SerializeField] private BuildingOverviewUISegment_InventoryAdjustableItem inventoryItemPrefab;
    [SerializeField] private BuildingOverviewUISegment_InventoryCapacity inventoryCapacity;
    [SerializeField] private RectTransform inventoryItemsContainer;

    private readonly List<BuildingOverviewUISegment_InventoryAdjustableItem> inventoryItemInstances;

    private BuildingOverviewUISegment_InventoryAdjustable()
    {
        inventoryItemInstances = new List<BuildingOverviewUISegment_InventoryAdjustableItem>();
    }

    protected override void OnValueChanged_Data(Inventory_Limited oldValue, Inventory_Limited newValue)
    {
        if (oldValue != null)
        {
            foreach (BuildingOverviewUISegment_InventoryAdjustableItem instance in inventoryItemInstances)
                GameObject.Destroy(instance.gameObject);

            inventoryItemInstances.Clear();
        }

        if (newValue != null)
        {
            inventoryCapacity.data = newValue;
            foreach (Resource resource in newValue.resources.Values)
            {
                BuildingOverviewUISegment_InventoryAdjustableItem instance = GameObject.Instantiate(inventoryItemPrefab, inventoryItemsContainer);
                instance.data = resource;
                inventoryItemInstances.Add(instance);
            }
        }
        else
        {
            inventoryCapacity.data = null;
        }
    }
}