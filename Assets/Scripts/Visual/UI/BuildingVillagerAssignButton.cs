using UnityEngine;
using UnityEngine.UI;

public class BuildingVillagerAssignButton : DataDrivenUI<ChunkMapPointContent_Building>
{
    [SerializeField] private Button assignButton;

    private void Awake()
    {
        assignButton.onClick.AddListener(OnPress_AssignButton);
    }

    protected override void OnValueChanged_Data(ChunkMapPointContent_Building oldValue, ChunkMapPointContent_Building newValue)
    {
        if (oldValue != null)
        {
            oldValue.assignedVillagersCount.onValueChange -= OnValueChanged_AssignedVillagers;
            oldValue.owner.townCenterBuilding.assignedVillagersCount.onValueChange -= OnValueChanged_UnassignedVillagers;
        }

        if(newValue != null)
        {
            newValue.assignedVillagersCount.onValueChange += OnValueChanged_AssignedVillagers;
            newValue.owner.townCenterBuilding.assignedVillagersCount.onValueChange += OnValueChanged_UnassignedVillagers;
            SetAssignButtonInteractable();
        }
    }

    private void OnValueChanged_AssignedVillagers(int oldValue, int newValue)
    {
        SetAssignButtonInteractable();
    }

    private void OnValueChanged_UnassignedVillagers(int oldValue, int newValue)
    {
        SetAssignButtonInteractable();
    }

    private void SetAssignButtonInteractable()
    {
        if (data.assignedVillagersCount.value == data.record.MaximumAssignedVillagers)
        {
            assignButton.interactable = false;
            return;
        }

        if (data.owner.townCenterBuilding.assignedVillagersCount.value == 0)
        {
            assignButton.interactable = false;
            return;
        }

        assignButton.interactable = true;
    }

    private void OnPress_AssignButton()
    {
        data.owner.AssignVillagerToBuilding(data);
    }

    private void OnDestroy()
    {
        assignButton.onClick.RemoveListener(OnPress_AssignButton);
    }

}