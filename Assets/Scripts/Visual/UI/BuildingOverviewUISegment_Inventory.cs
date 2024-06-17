
using System.Collections.Generic;
using UnityEngine;

public class BuildingOverviewUISegment_Inventory : DataDrivenUI<Inventory>
{
    [SerializeField] private BuildingOverviewUISegment_InventoryItem inventoryItemPrefab;
    [SerializeField] private RectTransform inventoryItemsContainer;

    private readonly List<BuildingOverviewUISegment_InventoryItem> inventoryItemInstances;

    private BuildingOverviewUISegment_Inventory()
    {
        inventoryItemInstances = new List<BuildingOverviewUISegment_InventoryItem>();
    }

    protected override void OnValueChanged_Data(Inventory oldValue, Inventory newValue)
    {
        if (oldValue != null)
        {
            foreach (BuildingOverviewUISegment_InventoryItem instance in inventoryItemInstances)
                GameObject.Destroy(instance.gameObject);

            inventoryItemInstances.Clear();
        }

        if (newValue != null)
        {
            foreach (Resource resource in newValue.resources.Values)
            {
                BuildingOverviewUISegment_InventoryItem instance = GameObject.Instantiate(inventoryItemPrefab, inventoryItemsContainer);
                instance.data = resource;
                inventoryItemInstances.Add(instance);
            }
        }
    }
}