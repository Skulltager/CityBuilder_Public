using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOverviewUISegment_AssignedVillagersAdjustable : DataDrivenUI<ChunkMapPointContent_Building>
{
    [SerializeField] private Button assignVillagerButton;
    [SerializeField] private Button unassignVillagerButton;

    [SerializeField] private TextMeshProUGUI assignedVillagersCountText;

    private void Awake()
    {
        assignVillagerButton.onClick.AddListener(OnPress_AssignVillagerButton);
        unassignVillagerButton.onClick.AddListener(OnPress_UnassignVillagerButton);
    }

    private void OnPress_AssignVillagerButton()
    {
        data.owner.AssignVillagerToBuilding(data);
    }

    private void OnPress_UnassignVillagerButton()
    {
        data.owner.UnassignVillagerFromBuilding(data);
    }

    protected override void OnValueChanged_Data(ChunkMapPointContent_Building oldValue, ChunkMapPointContent_Building newValue)
    {
        if (oldValue != null)
        {
            oldValue.owner.townCenterBuilding.assignedVillagersCount.onValueChange -= OnValueChanged_TownCenter_AssignedVillagersCount;
            oldValue.assignedVillagersCount.onValueChange -= OnValueChanged_AssignedVillagersCount;
        }

        if (newValue != null)
        {
            newValue.owner.townCenterBuilding.assignedVillagersCount.onValueChange += OnValueChanged_TownCenter_AssignedVillagersCount;
            newValue.assignedVillagersCount.onValueChangeImmediate += OnValueChanged_AssignedVillagersCount;
        }
    }

    private void OnValueChanged_TownCenter_AssignedVillagersCount(int oldValue, int newValue)
    {
        SetAssignVillagerButtonInteractable();
    }

    private void OnValueChanged_AssignedVillagersCount(int oldValue, int newValue)
    {
        assignedVillagersCountText.text = newValue.ToString();
        SetAssignVillagerButtonInteractable();
        SetUnassignVillagerButtonInteractable();
    }

    private void SetAssignVillagerButtonInteractable()
    {
        if (data.assignedVillagersCount.value == data.record.MaximumAssignedVillagers)
        {
            assignVillagerButton.interactable = false;
            return;
        }

        if (data.owner.townCenterBuilding.assignedVillagersCount.value == 0)
        {
            assignVillagerButton.interactable = false;
            return;
        }

        assignVillagerButton.interactable = true;
    }

    private void SetUnassignVillagerButtonInteractable()
    {
        if (data.assignedVillagersCount.value == 0)
        {
            unassignVillagerButton.interactable = false;
            return;
        }

        unassignVillagerButton.interactable = true;
    }

    private void OnDestroy()
    {
        assignVillagerButton.onClick.RemoveListener(OnPress_AssignVillagerButton);
        unassignVillagerButton.onClick.RemoveListener(OnPress_UnassignVillagerButton);
    }
}
