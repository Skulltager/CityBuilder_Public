using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOverviewUISegment_ResourcesLeft : DataDrivenUI<ChunkMapPointContent_WorldResource>
{
    [SerializeField] private TextMeshProUGUI providedResourceNameText;
    [SerializeField] private TextMeshProUGUI chargesText;
    [SerializeField] private Image chargesLeftFillImage;
    [SerializeField] private Image resourceImage;
    [SerializeField] private Color maxChargesLeftColor;
    [SerializeField] private Color noChargesLeftColor;

    protected override void OnValueChanged_Data(ChunkMapPointContent_WorldResource oldValue, ChunkMapPointContent_WorldResource newValue)
    {
        if (oldValue != null)
        {
            oldValue.timesHarvestableRemaining.onValueChange -= OnValueChanged_TimesHarvestableRemaining;
            oldValue.maximumTimesHarvestable.onValueChange -= OnValueChanged_MaximumTimesHarvestable;
        }

        if (newValue != null)
        {
            newValue.timesHarvestableRemaining.onValueChange += OnValueChanged_TimesHarvestableRemaining;
            newValue.maximumTimesHarvestable.onValueChange += OnValueChanged_MaximumTimesHarvestable;
            resourceImage.sprite = newValue.record.ResourceDrops.Sprite;
            providedResourceNameText.text = newValue.record.ResourceDrops.Name;
            SetChargesFillImage();
        }
    }

    private void OnValueChanged_TimesHarvestableRemaining(int oldValue, int newValue)
    {
        SetChargesFillImage();
    }

    private void OnValueChanged_MaximumTimesHarvestable(int oldValue, int newValue)
    {
        SetChargesFillImage();
    }

    private void SetChargesFillImage()
    {
        float factor = (float)data.timesHarvestableRemaining.value / data.maximumTimesHarvestable.value;
        chargesLeftFillImage.fillAmount = factor;
        chargesLeftFillImage.color = Color.Lerp(noChargesLeftColor, maxChargesLeftColor, factor);
        chargesText.text = string.Format("{0}", data.timesHarvestableRemaining.value);
    }
}