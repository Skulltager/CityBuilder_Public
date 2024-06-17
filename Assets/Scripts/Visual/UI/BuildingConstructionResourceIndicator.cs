
using UnityEngine;
using UnityEngine.UI;

public class BuildingConstructionResourceIndicator : DataDrivenUI<Resource>
{
    [SerializeField] private Image resourceIcon;
    [SerializeField] private Image desiredAmountFillImage;

    protected override void OnValueChanged_Data(Resource oldValue, Resource newValue)
    {
        if (oldValue != null)
        {
            oldValue.currentMissingAmount.onValueChange -= OnValueChanged_CurrentMissingAmount;
            oldValue.currentDesiredAmount.onValueChange -= OnValueChanged_CurrentDesiredAmount;
        }

        if (newValue != null)
        {
            newValue.currentMissingAmount.onValueChange += OnValueChanged_CurrentMissingAmount;
            newValue.currentDesiredAmount.onValueChange += OnValueChanged_CurrentDesiredAmount;
            resourceIcon.sprite = newValue.record.Sprite;
            SetDesiredAmountFillImage();
        }
    }

    private void OnValueChanged_CurrentMissingAmount(int oldValue, int newVAlue)
    {
        SetDesiredAmountFillImage();
    }

    private void OnValueChanged_CurrentDesiredAmount(int oldValue, int newValue)
    {
        SetDesiredAmountFillImage();
    }

    private void SetDesiredAmountFillImage()
    {
        desiredAmountFillImage.fillAmount = 1 - ((float)data.currentMissingAmount.value / data.currentDesiredAmount.value);
    }
}
