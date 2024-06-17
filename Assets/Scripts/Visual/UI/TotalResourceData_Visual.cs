
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TotalResourceData_Visual : DataDrivenUI<TotalResourceData>
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    protected override void OnValueChanged_Data(TotalResourceData oldValue, TotalResourceData newValue)
    {
        if (oldValue != null)
        {
            oldValue.totalUndesiredAmount.onValueChange -= OnValueChanged_TotalAmount;
            oldValue.currentMissingAmount.onValueChange -= OnValueChanged_DesiredAmount;
        }

        if (newValue != null)
        {
            image.sprite = newValue.record.Sprite;
            newValue.totalUndesiredAmount.onValueChange += OnValueChanged_TotalAmount;
            newValue.currentMissingAmount.onValueChange += OnValueChanged_DesiredAmount;
            SetText();
        }
    }

    private void OnValueChanged_TotalAmount(int oldValue, int newValue)
    {
        SetText();
    }

    private void OnValueChanged_DesiredAmount(int oldValue, int newValue)
    {
        SetText();
    }

    private void SetText()
    {
        string amountText = data.totalUndesiredAmount.value.ToString();

        if(data.currentMissingAmount.value > 0)
            amountText += string.Format("<color=\"yellow\"> ({0})", data.currentMissingAmount.value);

        text.text = amountText;
    }
}