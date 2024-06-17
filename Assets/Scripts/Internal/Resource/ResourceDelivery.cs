
using SheetCodes;

public class ResourceDelivery
{
    public readonly Villager villager;
    public readonly Inventory toInventory;
    public readonly ResourcesRecord record;
    public readonly int amount;

    private readonly Resource toResource;

    public ResourceDelivery(Villager villager, Inventory toInventory, ResourcesRecord record, int amount)
    {
        this.villager = villager;
        this.toInventory = toInventory;
        this.record = record;
        this.amount = amount;

        toResource = toInventory.resources[record.Identifier];
    }

    public void StartDelivery()
    {
        toResource.incomingAmount.value += amount;
    }

    public void CancelDelivery()
    {
        toResource.incomingAmount.value -= amount;
    }

    public void DeliverDelivery()
    {
        villager.inventory.value = null;
        toResource.amount.value += amount;
        toResource.incomingAmount.value -= amount;
        toInventory.TriggerOnItemsDelivered();
    }
}