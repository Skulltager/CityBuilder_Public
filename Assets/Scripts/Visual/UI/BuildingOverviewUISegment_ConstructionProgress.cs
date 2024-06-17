using UnityEngine;
using UnityEngine.UI;

public class BuildingOverviewUISegment_ConstructionProgress : DataDrivenUI<ChunkMapPointContent_BuildingConstruction>
{
    [SerializeField] private Image constructionProgressFillImage;

    protected override void OnValueChanged_Data(ChunkMapPointContent_BuildingConstruction oldValue, ChunkMapPointContent_BuildingConstruction newValue)
    {
        if (oldValue != null)
        {
            oldValue.constructionProgress.onValueChange -= OnValueChanged_DeconstructionProgress;
        }

        if (newValue != null)
        {
            newValue.constructionProgress.onValueChangeImmediate += OnValueChanged_DeconstructionProgress;
        }
    }

    private void OnValueChanged_DeconstructionProgress(float oldValue, float newValue)
    {
        constructionProgressFillImage.fillAmount = newValue;
    }
}