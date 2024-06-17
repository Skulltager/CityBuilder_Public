
using TMPro;
using UnityEngine;

public class BuildingOverviewUI_Warehouse : DataDrivenUI<ChunkMapPointContent_Building_Warehouse>
{
    [SerializeField] private BuildingVillagerAssignButton assignButton;
    [SerializeField] private BuildingVillagerUnassignButton unassignButton;
    [SerializeField] private TextMeshProUGUI assignedVillagersCountText;
    [SerializeField] private BuildingOverviewUISegment_InventoryCapacity inventoryLimitOverview;
    [SerializeField] private BuildingOverviewUISegment_InventoryAdjustable inventoryOverview;
    [SerializeField] private CanvasGroup canvasGroup;


    protected override void OnValueChanged_Data(ChunkMapPointContent_Building_Warehouse oldValue, ChunkMapPointContent_Building_Warehouse newValue)
    {
        if (oldValue != null)
        {
            oldValue.assignedVillagersCount.onValueChange -= OnValueChanged_AssignedVillager;
        }

        if (newValue != null)
        {
            assignButton.data = newValue;
            unassignButton.data = newValue;

            newValue.assignedVillagersCount.onValueChange += OnValueChanged_AssignedVillager;
            inventoryLimitOverview.data = newValue.inventory as Inventory_Limited;
            SetAssignedVillagersCountText();
            canvasGroup.Show();
        }
        else
        {
            inventoryOverview.data = null;
            inventoryLimitOverview.data = null;
            assignButton.data = null;
            unassignButton.data = null;
            canvasGroup.Hide();
        }
    }

    private void OnValueChanged_AssignedVillager(int oldValue, int newValue)
    {
        SetAssignedVillagersCountText();
    }

    private void SetAssignedVillagersCountText()
    {
        assignedVillagersCountText.text = string.Format("x {0}", data.assignedVillagersCount.value.ToString());
    }
}
