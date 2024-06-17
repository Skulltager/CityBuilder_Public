
using SheetCodes;
using System;
using System.Collections.Generic;

public abstract class Inventory
{
    public readonly Dictionary<ResourcesIdentifier, Resource> resources;
    public readonly EventVariable<Inventory, int> totalAmount;
    public readonly EventVariable<Inventory, int> totalMaxAmount;
    public readonly EventVariable<Inventory, int> totalDesiredAmount;
    public readonly EventVariable<Inventory, bool> softDesires;
    public abstract bool isFull { get; }
    public abstract int capacityRemaining { get; }
    public event Action onItemsDelivered;

    public Inventory(bool softDesires)
    {
        resources = new Dictionary<ResourcesIdentifier, Resource>();
        totalAmount = new EventVariable<Inventory, int>(this, 0);
        totalMaxAmount = new EventVariable<Inventory, int>(this, 0);
        totalDesiredAmount = new EventVariable<Inventory, int>(this, 0);
        this.softDesires = new EventVariable<Inventory, bool>(this, softDesires);

        ResourcesIdentifier[] resourceIdentifiers = Enum.GetValues(typeof(ResourcesIdentifier)) as ResourcesIdentifier[];
        foreach(ResourcesIdentifier identifier in resourceIdentifiers)
        {
            if (identifier == ResourcesIdentifier.None)
                continue;

            ResourcesRecord record = identifier.GetRecord();

            Resource resource = new Resource(this, record, 0);
            resources.Add(identifier, resource);
            resource.amount.onValueChange += OnValueChanged_Resource_Amount;
            resource.maxAmount.onValueChange += OnValueChanged_Resource_MaxAmount;
            resource.currentDesiredAmount.onValueChange += OnValueChanged_Resource_DesiredAmount;
        }
    }

    public void TriggerOnItemsDelivered()
    {
        if (onItemsDelivered != null)
            onItemsDelivered();
    }

    private void OnValueChanged_Resource_Amount(int oldValue, int newValue)
    {
        totalAmount.value += newValue - oldValue;
    }

    private void OnValueChanged_Resource_MaxAmount(int oldValue, int newValue)
    {
        totalMaxAmount.value += newValue - oldValue;
    }

    private void OnValueChanged_Resource_DesiredAmount(int oldValue, int newValue)
    {
        totalDesiredAmount.value += newValue - oldValue;
    }

    public void AddResources(ResourcesIdentifier identifier, int amount)
    {
        resources[identifier].amount.value += amount;
    }

    public void UnreserveAndRemoveResources(ResourcesIdentifier identifier, int amount)
    {
        Resource resource = resources[identifier];
        resource.amount.value -= amount;
        resource.reservedAmount.value -= amount;
    }

    public void CopyInventoryTo(Inventory other)
    {
        foreach(Resource resource in resources.Values)
        {
            if (resource.amount.eventStackValue == 0)
                continue;

            Resource otherResource = other.resources[resource.record.Identifier];
            otherResource.amount.value += resource.amount.eventStackValue;
        }
    }

    public abstract bool HasEnoughInventorySize(int amount);
}