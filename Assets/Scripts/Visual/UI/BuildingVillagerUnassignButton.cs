using UnityEngine;
using UnityEngine.UI;

public class BuildingVillagerUnassignButton : DataDrivenUI<ChunkMapPointContent_Building>
{
    [SerializeField] private Button unassignButton;

    private void Awake()
    {
        unassignButton.onClick.AddListener(OnPress_UnassignButton);
    }

    protected override void OnValueChanged_Data(ChunkMapPointContent_Building oldValue, ChunkMapPointContent_Building newValue)
    {
        if (oldValue != null)
        {
            oldValue.assignedVillagersCount.onValueChange -= OnValueChanged_AssignedVillagers;
        }

        if (newValue != null)
        {
            newValue.assignedVillagersCount.onValueChange += OnValueChanged_AssignedVillagers;
            SetAssignButtonInteractable();
        }
    }

    private void OnValueChanged_AssignedVillagers(int oldValue, int newValue)
    {
        SetAssignButtonInteractable();
    }

    private void SetAssignButtonInteractable()
    {
        if (data.assignedVillagersCount.value == 0)
        {
            unassignButton.interactable = false;
            return;
        }

        unassignButton.interactable = true;
    }

    private void OnPress_UnassignButton()
    {
        data.owner.UnassignVillagerFromBuilding(data);
    }

    private void OnDestroy()
    {
        unassignButton.onClick.RemoveListener(OnPress_UnassignButton);
    }

}