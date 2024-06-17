
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverviewUI_WorldResource : DataDrivenUI<ChunkMapPointContent_WorldResource>
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI providedResourceNameText;
    [SerializeField] private TextMeshProUGUI chargesText;
    [SerializeField] private Image chargesLeftFillImage;
    [SerializeField] private Image resourceImage;
    [SerializeField] private Color maxChargesLeftColor;
    [SerializeField] private Color noChargesLeftColor;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.Hide();
    }

    protected override void OnValueChanged_Data(ChunkMapPointContent_WorldResource oldValue, ChunkMapPointContent_WorldResource newValue)
    {
        if(oldValue != null)
        {
            oldValue.timesHarvestableRemaining.onValueChange -= OnValueChanged_TimesHarvestableRemaining;
            oldValue.maximumTimesHarvestable.onValueChange -= OnValueChanged_MaximumTimesHarvestable;
        }

        if (newValue != null)
        {
            newValue.timesHarvestableRemaining.onValueChange += OnValueChanged_TimesHarvestableRemaining;
            newValue.maximumTimesHarvestable.onValueChange += OnValueChanged_MaximumTimesHarvestable;
            resourceImage.sprite = newValue.record.ResourceDrops.Sprite;
            nameText.text = newValue.record.Name;
            providedResourceNameText.text = newValue.record.ResourceDrops.Name;
            SetChargesFillImage();

            canvasGroup.Show();
        }
        else
        {
            canvasGroup.Hide();
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
        chargesText.text = string.Format("{0}/{1}", data.timesHarvestableRemaining.value, data.maximumTimesHarvestable.value);
    }
}