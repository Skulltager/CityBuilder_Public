
public class Inventory_Limited : Inventory
{
    public override bool isFull => totalMaxAmount.value >= capacity.value;
    public override int capacityRemaining => capacity.value - totalMaxAmount.value;

    public readonly EventVariable<Inventory_Limited, int> capacity;

    public Inventory_Limited(int capacity, bool softDesires)
        :base(softDesires)
    {
        this.capacity = new EventVariable<Inventory_Limited, int>(this, capacity);
    }

    public override bool HasEnoughInventorySize(int amount)
    {
        return totalAmount.value + amount <= capacity.value;
    }
}