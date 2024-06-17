
using SheetCodes;

public class ResourceDeliveryReservation
{
    public readonly Villager villager;
    public readonly ChunkMapPointContent_BuildingBase fromBuilding;
    public readonly ChunkMapPointContent_BuildingBase toBuilding;
    public readonly ResourcesRecord record;
    public readonly int amount;

    private readonly Resource fromResource;
    private readonly Resource toResource;

    public ResourceDeliveryReservation(Villager villager, ChunkMapPointContent_BuildingBase fromBuilding, ChunkMapPointContent_BuildingBase toBuilding, ResourcesRecord record, int amount)
    {
        this.villager = villager;
        this.fromBuilding = fromBuilding;
        this.toBuilding = toBuilding;
        this.record = record;
        this.amount = amount;
        fromResource = fromBuilding.inventory.resources[record.Identifier];
        toResource = toBuilding.inventory.resources[record.Identifier];
    }

    public void StartPickup()
    {
        fromResource.reservedAmount.value += amount;
        toResource.incomingAmount.value += amount;
    }

    public void CancelPickup()
    {
        fromResource.reservedAmount.value -= amount;
        toResource.incomingAmount.value -= amount;
    }

    public void PickUpDelivery()
    {
        villager.inventory.value = new ResourceCount(record, amount);
        fromResource.amount.value -= amount;
        fromResource.reservedAmount.value -= amount;
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
        toBuilding.inventory.TriggerOnItemsDelivered();
    }
}
