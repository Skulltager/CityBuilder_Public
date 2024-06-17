
using SheetCodes;

public class ResourceCount
{
    public readonly ResourcesRecord record;
    public readonly EventVariable<ResourceCount, int> amount;

    public ResourceCount(ResourcesRecord record, int amount)
    {
        this.record = record;
        this.amount = new EventVariable<ResourceCount, int>(this, amount);
    }
}
