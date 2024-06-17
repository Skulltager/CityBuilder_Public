using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOverviewUISegment_InventoryAdjustableItem : DataDrivenUI<Resource>
{
    private const int DESIRED_AMOUNT_ADJUSTMENT_VALUE = 10;

    [SerializeField] private Image resourceIcon;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Button increaseDesiredAmountButton;
    [SerializeField] private Button decreaseDesiredAmountButton;

    private void Awake()
    {
        increaseDesiredAmountButton.onClick.AddListener(OnPress_IncreaseDesiredAmountButton);
        decreaseDesiredAmountButton.onClick.AddListener(OnPress_DecreaseDesiredAmountButton);
    }

    private void OnPress_IncreaseDesiredAmountButton()
    {
        Inventory_Limited inventory = data.inventory as Inventory_Limited;
        int desiredAmountIncrease = Mathf.Min(inventory.capacity.value - data.inventory.totalDesiredAmount.value, DESIRED_AMOUNT_ADJUSTMENT_VALUE);
        data.currentDesiredAmount.value += desiredAmountIncrease;
        data.totalDesiredAmount.value += desiredAmountIncrease;
    }

    private void OnPress_DecreaseDesiredAmountButton()
    {
        int newDesiredAmount = Mathf.Max(data.currentDesiredAmount.value - DESIRED_AMOUNT_ADJUSTMENT_VALUE, 0);
        data.currentDesiredAmount.value = newDesiredAmount;
        data.totalDesiredAmount.value = newDesiredAmount;
    }

    private void SetIncreaseDesiredAmountButtonInteractable()
    {
        Inventory_Limited inventory = data.inventory as Inventory_Limited;
        if(data.inventory.totalDesiredAmount.value == inventory.capacity.value)
        {
            increaseDesiredAmountButton.interactable = false;
            return;
        }

        increaseDesiredAmountButton.interactable = true;
    }

    private void SetDecreaseDesiredAmountButtonInteractable()
    {
        if (data.inventory.totalDesiredAmount.value == 0)
        {
            decreaseDesiredAmountButton.interactable = false;
            return;
        }

        if (data.currentDesiredAmount.value == 0)
        {
            decreaseDesiredAmountButton.interactable = false;
            return;
        }


        decreaseDesiredAmountButton.interactable = true;
    }

    protected override void OnValueChanged_Data(Resource oldValue, Resource newValue)
    {
        if (oldValue != null)
        {
            Inventory_Limited inventory = oldValue.inventory as Inventory_Limited;
            oldValue.amount.onValueChange -= OnValueChanged_Amount;
            oldValue.reservedAmount.onValueChange -= OnValueChanged_ReservedAmount;
            oldValue.incomingAmount.onValueChange -= OnValueChanged_IncomingAmount;
            oldValue.currentDesiredAmount.onValueChange -= OnValueChanged_CurrentDesiredAmount;
            inventory.totalDesiredAmount.onValueChange -= OnValueChanged_Inventory_TotalDesiredAmount;
            inventory.capacity.onValueChange -= OnValueChanged_Inventory_Capacity;
        }

        if (newValue != null)
        {
            Inventory_Limited inventory = newValue.inventory as Inventory_Limited;

            newValue.amount.onValueChange += OnValueChanged_Amount;
            newValue.reservedAmount.onValueChange += OnValueChanged_ReservedAmount;
            newValue.incomingAmount.onValueChange += OnValueChanged_IncomingAmount;
            newValue.currentDesiredAmount.onValueChange += OnValueChanged_CurrentDesiredAmount;
            inventory.totalDesiredAmount.onValueChange += OnValueChanged_Inventory_TotalDesiredAmount;
            inventory.capacity.onValueChange += OnValueChanged_Inventory_Capacity;
            resourceIcon.sprite = newValue.record.Sprite;
            SetAmountText();
            SetDecreaseDesiredAmountButtonInteractable();
            SetIncreaseDesiredAmountButtonInteractable();
        }
    }

    private void OnValueChanged_Inventory_Capacity(int oldValue, int newValue)
    {
        SetDecreaseDesiredAmountButtonInteractable();
        SetIncreaseDesiredAmountButtonInteractable();
    }

    private void OnValueChanged_Inventory_TotalDesiredAmount(int oldValue, int newValue)
    {
        SetDecreaseDesiredAmountButtonInteractable();
        SetIncreaseDesiredAmountButtonInteractable();
    }

    private void OnValueChanged_Amount(int oldValue, int newValue)
    {
        SetAmountText();
    }

    private void OnValueChanged_IncomingAmount(int oldValue, int newValue)
    {
        SetAmountText();
    }

    private void OnValueChanged_CurrentDesiredAmount(int oldValue, int newValue)
    {
        SetAmountText();
    }

    private void OnValueChanged_ReservedAmount(int oldValue, int newValue)
    {
        SetAmountText();
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

    private void OnDestroy()
    {
        increaseDesiredAmountButton.onClick.RemoveListener(OnPress_IncreaseDesiredAmountButton);
        decreaseDesiredAmountButton.onClick.RemoveListener(OnPress_DecreaseDesiredAmountButton);
    }
}