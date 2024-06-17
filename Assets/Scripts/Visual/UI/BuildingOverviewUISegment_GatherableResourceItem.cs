using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOverviewUISegment_GatherableResourceItem : DataDrivenUI<ResourceCount>
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image resourceImage;

    protected override void OnValueChanged_Data(ResourceCount oldValue, ResourceCount newValue)
    {
        if(oldValue != null)
        {
            oldValue.amount.onValueChange -= OnValueChanged_Amount;
        }

        if (newValue != null)
        {
            newValue.amount.onValueChangeImmediate += OnValueChanged_Amount;
            resourceImage.sprite = newValue.record.Sprite;
        }
    }

    private void OnValueChanged_Amount(int oldValue, int newValue)
    {
        if (newValue == 0)
        {
            canvasGroup.Hide();
            layoutElement.ignoreLayout = true;
        }
        else
        {
            canvasGroup.Show();
            layoutElement.ignoreLayout = false;
            amountText.text = newValue.ToString();
        }
    }
}