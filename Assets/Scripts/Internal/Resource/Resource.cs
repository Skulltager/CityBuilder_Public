using SheetCodes;
using UnityEngine;

public class Resource
{
    public readonly Inventory inventory;
    public readonly ResourcesRecord record;
    public readonly EventVariable<Resource, int> amount;
    public readonly EventVariable<Resource, int> reservedAmount; // Items reserved for pickup
    public readonly EventVariable<Resource, int> incomingAmount; // Items incoming for delivery
    public readonly EventVariable<Resource, int> currentDesiredAmount; // Desired amount of items
    public readonly EventVariable<Resource, int> totalDesiredAmount; // Desired amount of items but not actually requesting (used for crafting queues)
    public readonly EventVariable<Resource, int> totalUndesiredAmount; // Undesired amount of items
    public readonly EventVariable<Resource, int> lockedAmount; // Items locked for use for crafting
    public readonly EventVariable<Resource, int> unreservedAmount; // Items available for usage
    public readonly EventVariable<Resource, int> currentMissingAmount; // Items away from the current desired amount not including incoming amount
    public readonly EventVariable<Resource, int> missingAmount; // Items away from the total desired amount including incoming amount
    public readonly EventVariable<Resource, int> availableAmount; // Items available for pickup
    public readonly EventVariable<Resource, int> maxAmount; // The maximum amount of items this can get to (used to prevent overflow of storage)
    public readonly EventVariable<Resource, int> startingAmount; // Only used for deconstruction indicators

    public Resource(Inventory inventory, ResourcesRecord record, int amount = 0)
    {
        this.inventory = inventory;
        this.record = record;

        this.amount = new EventVariable<Resource, int>(this, amount, true);
        reservedAmount = new EventVariable<Resource, int>(this, 0, true);
        incomingAmount = new EventVariable<Resource, int>(this, 0, true);
        currentDesiredAmount = new EventVariable<Resource, int>(this, 0, true);
        lockedAmount = new EventVariable<Resource, int>(this, 0, true);
        totalDesiredAmount = new EventVariable<Resource, int>(this, 0, true);

        unreservedAmount = new EventVariable<Resource, int>(this, 0, true);
        missingAmount = new EventVariable<Resource, int>(this, 0, true);
        availableAmount = new EventVariable<Resource, int>(this, 0, true);
        maxAmount = new EventVariable<Resource, int>(this, 0, true);
        currentMissingAmount = new EventVariable<Resource, int>(this, 0, true);
        totalUndesiredAmount = new EventVariable<Resource, int>(this, 0, true);
        startingAmount = new EventVariable<Resource, int>(this, 0, true);

        this.amount.onValueChange += OnValueChanged_Amount;
        reservedAmount.onValueChange += OnValueChanged_ReservedAmount;
        incomingAmount.onValueChange += OnValueChanged_IncomingAmount;
        currentDesiredAmount.onValueChange += OnValueChanged_DesiredAmount;
        lockedAmount.onValueChange += OnValueChanged_LockedAmount;
        totalDesiredAmount.onValueChange += OnValueChanged_TotalDesiredAmount;
    }

    private void OnValueChanged_Amount(int oldValue, int newValue)
    {
        SetUnreservedAmount();
        SetMissingAmount();
        SetAvailableAmount();
        SetMaxAmount();
        SetCurrentMissingAmount();
        SetTotalUndesiredAmount();
    }

    private void OnValueChanged_ReservedAmount(int oldValue, int newValue)
    {
        SetUnreservedAmount();
        SetMissingAmount();
        SetAvailableAmount();
        SetCurrentMissingAmount();
    }

    private void OnValueChanged_IncomingAmount(int oldValue, int newValue)
    {
        SetMissingAmount();
        SetMaxAmount();
    }

    private void OnValueChanged_DesiredAmount(int oldValue, int newValue)
    {
        SetMissingAmount();
        SetAvailableAmount();
    }

    private void OnValueChanged_LockedAmount(int oldValue, int newValue)
    {
        SetUnreservedAmount();
        SetAvailableAmount();
    }

    private void OnValueChanged_TotalDesiredAmount(int oldValue, int newValue)
    {
        SetTotalUndesiredAmount();
        SetCurrentMissingAmount();
    }

    private void SetUnreservedAmount()
    {
        unreservedAmount.value = amount.value - reservedAmount.value - lockedAmount.value;
    }

    private void SetAvailableAmount()
    {
        availableAmount.value = amount.value - reservedAmount.value - currentDesiredAmount.value - lockedAmount.value;
    }

    private void SetMissingAmount()
    {
        missingAmount.value = currentDesiredAmount.value - (amount.value - reservedAmount.value + incomingAmount.value);
    }

    private void SetCurrentMissingAmount()
    {
        currentMissingAmount.value = totalDesiredAmount.value - (amount.value - reservedAmount.value);
    }

    private void SetMaxAmount()
    {
        maxAmount.value = amount.value + incomingAmount.value;
    }

    private void SetTotalUndesiredAmount()
    {
        totalUndesiredAmount.value = Mathf.Max(0, amount.value - totalDesiredAmount.value);
    }
}