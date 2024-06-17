
using TMPro;
using UnityEngine;

public class BuildingOverviewUI_GatheringHut : DataDrivenUI<ChunkMapPointContent_Building_GatheringHut>
{
    [SerializeField] private BuildingVillagerAssignButton assignButton;
    [SerializeField] private BuildingVillagerUnassignButton unassignButton;
    [SerializeField] private TextMeshProUGUI assignedVillagersCountText;
    [SerializeField] private BuildingOverviewUISegment_Inventory inventoryOverview;
    [SerializeField] private BuildingOverviewUISegment_GatherableResources gatherableResources;
    [SerializeField] private CanvasGroup canvasGroup;

    protected override void OnValueChanged_Data(ChunkMapPointContent_Building_GatheringHut oldValue, ChunkMapPointContent_Building_GatheringHut newValue)
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
            inventoryOverview.data = newValue.inventory;
            gatherableResources.data = newValue.gatherableResources;
            SetAssignedVillagersCountText();
            canvasGroup.Show();
        }
        else
        {
            inventoryOverview.data = null;
            assignButton.data = null;
            unassignButton.data = null;
            gatherableResources.data = null;

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