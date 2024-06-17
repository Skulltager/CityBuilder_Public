using SheetCodes;

public class TotalResourceData
{
    public readonly ResourcesRecord record;
    public readonly EventVariable<TotalResourceData, int> totalUndesiredAmount;
    public readonly EventVariable<TotalResourceData, int> currentMissingAmount;

    public TotalResourceData(ResourcesRecord record)
    {
        this.record = record;
        totalUndesiredAmount = new EventVariable<TotalResourceData, int>(this, 0);
        currentMissingAmount = new EventVariable<TotalResourceData, int>(this, 0);
    }
}