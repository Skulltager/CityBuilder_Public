
public class Inventory_Unlimited : Inventory
{
    public override bool isFull => false;
    public override int capacityRemaining => int.MaxValue;

    public Inventory_Unlimited(bool softDesires)
        : base(softDesires) { }

    public override bool HasEnoughInventorySize(int amount)
    {
        return true;
    }
}