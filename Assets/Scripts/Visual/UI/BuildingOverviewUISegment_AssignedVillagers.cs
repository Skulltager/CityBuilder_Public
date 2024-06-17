using TMPro;
using UnityEngine;

public class BuildingOverviewUISegment_AssignedVillagers : DataDrivenUI<ChunkMapPointContent_Building>
{
    [SerializeField] private TextMeshProUGUI assignedVillagersCountText;

    protected override void OnValueChanged_Data(ChunkMapPointContent_Building oldValue, ChunkMapPointContent_Building newValue)
    {
        if (oldValue != null)
        {
            oldValue.assignedVillagersCount.onValueChange -= OnValueChanged_AssignedVillagersCount;
        }

        if (newValue != null)
        {
            newValue.assignedVillagersCount.onValueChangeImmediate += OnValueChanged_AssignedVillagersCount;
        }
    }

    private void OnValueChanged_AssignedVillagersCount(int oldValue, int newValue)
    {
        assignedVillagersCountText.text = newValue.ToString();
    }
}
