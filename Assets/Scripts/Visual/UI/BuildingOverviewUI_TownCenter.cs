
using TMPro;
using UnityEngine;

public class BuildingOverviewUI_TownCenter : DataDrivenUI<ChunkMapPointContent_Building_TownCenter>
{
    [SerializeField] private TextMeshProUGUI assignedVillagersCountText;
    [SerializeField] private BuildingOverviewUISegment_Inventory inventoryOverview;
    [SerializeField] private CanvasGroup canvasGroup;

    protected override void OnValueChanged_Data(ChunkMapPointContent_Building_TownCenter oldValue, ChunkMapPointContent_Building_TownCenter newValue)
    {
        if (oldValue != null)
        {
            oldValue.assignedVillagersCount.onValueChange -= OnValueChanged_AssignedVillager;
        }

        if (newValue != null)
        {
            newValue.assignedVillagersCount.onValueChange += OnValueChanged_AssignedVillager;
            inventoryOverview.data = newValue.inventory;
            SetAssignedVillagersCountText();
            canvasGroup.Show();
        }
        else
        {
            inventoryOverview.data = null;
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
