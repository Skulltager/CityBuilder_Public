
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOverviewUISegment_InventoryItem : DataDrivenUI<Resource>
{
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private LayoutElement layoutElement;

    protected override void OnValueChanged_Data(Resource oldValue, Resource newValue)
    {
        if (oldValue != null)
        {
            oldValue.amount.onValueChange -= OnValueChanged_Amount;
            oldValue.currentDesiredAmount.onValueChange -= OnValueChanged_CurrentDesiredAmount;
            oldValue.reservedAmount.onValueChange -= OnValueChanged_ReservedAmount;
            oldValue.incomingAmount.onValueChange -= OnValueChanged_IncomingAmount;
        }

        if (newValue != null)
        {
            newValue.amount.onValueChange += OnValueChanged_Amount;
            newValue.currentDesiredAmount.onValueChange += OnValueChanged_CurrentDesiredAmount;
            newValue.reservedAmount.onValueChange += OnValueChanged_ReservedAmount;
            newValue.incomingAmount.onValueChange += OnValueChanged_IncomingAmount;
            resourceIcon.sprite = newValue.record.Sprite;
            SetAmountText();
            SetVisibility();
        }
    }

    private void OnValueChanged_Amount(int oldValue, int newValue)
    {
        SetAmountText();
        SetVisibility();
    }

    private void OnValueChanged_CurrentDesiredAmount(int oldValue, int newValue)
    {
        SetAmountText();
        SetVisibility();
    }

    private void OnValueChanged_ReservedAmount(int oldValue, int newValue)
    {
        SetAmountText();
        SetVisibility();
    }

    private void OnValueChanged_IncomingAmount(int oldValue, int newValue)
    {
        SetAmountText();
        SetVisibility();
    }

    private void SetAmountText()
    {
        string amountText = data.amount.value.ToString();

        if (data.currentDesiredAmount.value > 0)
            amountText += string.Format("<color=\"black\">/{0}", data.currentDesiredAmount.value);

        if (data.reservedAmount.value > 0)
            amountText += string.Format("<color=\"red\">(-{0})", data.reservedAmount.value);

        if (data.incomingAmount.value > 0)
            amountText += string.Format("<color=\"green\">(+{0})", data.incomingAmount.value);


        this.amountText.text = amountText;
    }

    private void SetVisibility()
    {
        bool visible = data.amount.value > 0 || data.currentDesiredAmount.value > 0 || data.reservedAmount.value > 0 || data.incomingAmount.value > 0;

        if (visible)
        {
            layoutElement.ignoreLayout = false;
            canvasGroup.Show();
        }
        else
        {
            layoutElement.ignoreLayout = true;
            canvasGroup.Hide();
        }
    }
}