using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOverviewUISegment_TaskPriority : DataDrivenUI<BuildingTask>
{
    [SerializeField] private Button increasePriorityButton;
    [SerializeField] private Button decreasePriorityButton;

    [SerializeField] private TextMeshProUGUI priorityText;

    private void Awake()
    {
        increasePriorityButton.onClick.AddListener(OnPress_IncreasePriorityButton);
        decreasePriorityButton.onClick.AddListener(OnPress_DecreasePriorityButton);
    }

    private void OnPress_IncreasePriorityButton()
    {
        data.priority.value = data.priority.value.IncreasePriority();
    }

    private void OnPress_DecreasePriorityButton()
    {
        data.priority.value = data.priority.value.DecreasePriority();
    }

    protected override void OnValueChanged_Data(BuildingTask oldValue, BuildingTask newValue)
    {
        if (oldValue != null)
        {
            oldValue.priority.onValueChange -= OnValueChanged_Priority;
        }

        if (newValue != null)
        {
            newValue.priority.onValueChange += OnValueChanged_Priority;
            SetIncreasePriorityButtonInteractable();
            SetDecreasePriorityButtonInteractable();
            SetPriorityText();
        }
    }

    private void OnValueChanged_Priority(BuildingTaskPriority oldValue, BuildingTaskPriority newValue)
    {
        SetIncreasePriorityButtonInteractable();
        SetDecreasePriorityButtonInteractable();
        SetPriorityText();
    }

    private void SetIncreasePriorityButtonInteractable()
    {
        if(data.priority.value == BuildingTaskPriority.VeryHigh)
        {
            increasePriorityButton.interactable = false;
            return;
        }

        increasePriorityButton.interactable = true;
    }

    private void SetDecreasePriorityButtonInteractable()
    {
        if (data.priority.value == BuildingTaskPriority.VeryLow)
        {
            decreasePriorityButton.interactable = false;
            return;
        }

        decreasePriorityButton.interactable = true;
    }

    private void SetPriorityText()
    {
        priorityText.text = string.Format("Priority: {0}", data.priority.value.GetIdentifier());
    }

    private void OnDestroy()
    {
        increasePriorityButton.onClick.RemoveListener(OnPress_IncreasePriorityButton);
        decreasePriorityButton.onClick.RemoveListener(OnPress_DecreasePriorityButton);
    }

}