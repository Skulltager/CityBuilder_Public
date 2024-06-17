using UnityEngine;
using UnityEngine.UI;

public class BuildingOverviewUISegment_DeconstructionProgress : DataDrivenUI<ChunkMapPointContent_BuildingDeconstruction>
{
    [SerializeField] private Image deconstructionProgressFillImage;

    protected override void OnValueChanged_Data(ChunkMapPointContent_BuildingDeconstruction oldValue, ChunkMapPointContent_BuildingDeconstruction newValue)
    {
        if (oldValue != null)
        {
            oldValue.deconstructionProgress.onValueChange -= OnValueChanged_DeconstructionProgress;
        }

        if (newValue != null)
        {
            newValue.deconstructionProgress.onValueChangeImmediate += OnValueChanged_DeconstructionProgress;
        }
    }

    private void OnValueChanged_DeconstructionProgress(float oldValue, float newValue)
    {
        deconstructionProgressFillImage.fillAmount = 1 - newValue;
    }
}