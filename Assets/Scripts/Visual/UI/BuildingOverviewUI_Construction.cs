using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOverviewUI_Construction : DataDrivenUI<ChunkMapPointContent_BuildingConstruction>
{
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private BuildingOverviewUISegment_Inventory inventoryOverview;
    [SerializeField] private Image constructionProgressFillImage;
    [SerializeField] private BuildingOverviewUISegment_TaskPriority taskPriorityOverview;
    [SerializeField] private CanvasGroup canvasGroup;

    protected override void OnValueChanged_Data(ChunkMapPointContent_BuildingConstruction oldValue, ChunkMapPointContent_BuildingConstruction newValue)
    {
        if (oldValue != null)
        {
            oldValue.constructionProgress.onValueChange -= OnValueChanged_ConstructionProgress;
        }

        if (newValue != null)
        {
            buildingNameText.text = string.Format("{0} (Construction)", newValue.record.Name);
            inventoryOverview.data = newValue.inventory;
            taskPriorityOverview.data = newValue.buildingTask;
            newValue.constructionProgress.onValueChangeImmediate += OnValueChanged_ConstructionProgress;
            canvasGroup.Show();
        }
        else
        {
            inventoryOverview.data = null;
            taskPriorityOverview.data = null;
            canvasGroup.Hide();
        }
    }

    private void OnValueChanged_ConstructionProgress(float oldValue, float newValue)
    {
        constructionProgressFillImage.fillAmount = newValue;
    }
}
