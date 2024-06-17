
using UnityEngine;
using UnityEngine.UI;

public class BuildingDeconstructionResourceIndicator : DataDrivenUI<Resource>
{
    [SerializeField] private Image resourceIcon;
    [SerializeField] private Image desiredAmountFillImage;

    protected override void OnValueChanged_Data(Resource oldValue, Resource newValue)
    {
        if (oldValue != null)
        {
            oldValue.startingAmount.onValueChange -= OnValueChanged_StartingAmount;
            oldValue.amount.onValueChange -= OnValueChanged_Amount;
        }

        if (newValue != null)
        {
            newValue.startingAmount.onValueChange += OnValueChanged_StartingAmount;
            newValue.amount.onValueChange += OnValueChanged_Amount;
            resourceIcon.sprite = newValue.record.Sprite;
            SetDesiredAmountFillImage();
        }
    }

    private void OnValueChanged_StartingAmount(int oldValue, int newVAlue)
    {
        SetDesiredAmountFillImage();
    }

    private void OnValueChanged_Amount(int oldValue, int newValue)
    {
        SetDesiredAmountFillImage();
    }

    private void SetDesiredAmountFillImage()
    {
        desiredAmountFillImage.fillAmount = (float)data.amount.value / data.startingAmount.value ;
    }
}
