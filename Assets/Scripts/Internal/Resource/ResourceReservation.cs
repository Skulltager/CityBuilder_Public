
using SheetCodes;

public class ResourceReservation
{
    public readonly Villager villager;
    public readonly Inventory fromInventory;
    public readonly ResourcesRecord record;
    public readonly int amount;

    private readonly Resource fromResource;

    public ResourceReservation(Villager villager, Inventory fromInventory, ResourcesRecord record, int amount)
    {
        this.villager = villager;
        this.fromInventory = fromInventory;
        this.record = record;
        this.amount = amount;
        fromResource = fromInventory.resources[record.Identifier];
    }

    public void StartPickup()
    {
        fromResource.reservedAmount.value += amount;
    }

    public void CancelPickup()
    {
        fromResource.reservedAmount.value -= amount;
    }

    public void PickUpDelivery()
    {
        villager.inventory.value = new ResourceCount(record, amount);
        fromResource.amount.value -= amount;
        fromResource.reservedAmount.value -= amount;
    }
}