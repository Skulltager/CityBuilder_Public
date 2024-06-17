using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOverviewUI_Deconstruction : DataDrivenUI<ChunkMapPointContent_BuildingDeconstruction>
{
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private BuildingOverviewUISegment_Inventory inventoryOverview;
    [SerializeField] private Image deconstructionProgressFillImage;
    [SerializeField] private BuildingOverviewUISegment_TaskPriority taskPriorityOverview;
    [SerializeField] private Button cancelDeconstructionButton;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        cancelDeconstructionButton.onClick.AddListener(OnPress_CancelDeconstructionButton);
    }

    private void OnPress_CancelDeconstructionButton()
    {
        data.CancelDeconstruction();
    }

    protected override void OnValueChanged_Data(ChunkMapPointContent_BuildingDeconstruction oldValue, ChunkMapPointContent_BuildingDeconstruction newValue)
    {
        if (oldValue != null)
        {
            oldValue.deconstructionProgress.onValueChange -= OnValueChanged_DeconstructionProgress;
        }

        if (newValue != null)
        {
            buildingNameText.text = string.Format("{0} (Deconstruction)", newValue.record.Name);
            inventoryOverview.data = newValue.inventory;
            taskPriorityOverview.data = newValue.buildingTask;
            newValue.deconstructionProgress.onValueChangeImmediate += OnValueChanged_DeconstructionProgress;
            canvasGroup.Show();
        }
        else
        {
            inventoryOverview.data = null;
            taskPriorityOverview.data = null;
            canvasGroup.Hide();
        }
    }

    private void OnValueChanged_DeconstructionProgress(float oldValue, float newValue)
    {
        deconstructionProgressFillImage.fillAmount = 1 - newValue;
    }

    private void OnDestroy()
    {
        cancelDeconstructionButton.onClick.RemoveListener(OnPress_CancelDeconstructionButton);
    }
}
