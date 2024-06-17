
using SheetCodes;

public class PlayerBuildingLimits
{
    public readonly BuildingRecord record;
    public readonly EventVariable<PlayerBuildingLimits, int> limit;
    public readonly EventVariable<PlayerBuildingLimits, int> placedAmount;
    public readonly EventVariable<PlayerBuildingLimits, int> amountLeft;

    public PlayerBuildingLimits(BuildingRecord record)
    {
        this.record = record;
        limit = new EventVariable<PlayerBuildingLimits, int>(this, 0);
        placedAmount = new EventVariable<PlayerBuildingLimits, int>(this, 0);
        amountLeft = new EventVariable<PlayerBuildingLimits, int>(this, 0);

        limit.onValueChange += OnValueChanged_Limit;
        placedAmount.onValueChange += OnValueChanged_PlacedAmount;
    }

    private void OnValueChanged_Limit(int oldValue, int newValue)
    {
        SetAmountLeft();
    }

    private void OnValueChanged_PlacedAmount(int oldValue, int newValue)
    {
        SetAmountLeft();
    }

    private void SetAmountLeft()
    {
        amountLeft.value = limit.value - placedAmount.value;
    }
}