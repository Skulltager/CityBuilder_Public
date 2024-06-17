using System.Collections.Generic;
using System;

public class WeightedRandomizer<T>
{
    private readonly List<WeightedItem> items;
    private readonly Random random;

    private int totalWeight;

    public int count => items.Count;

    public WeightedRandomizer()
    {
        random = new Random((int)(UnityEngine.Random.value * int.MaxValue));
        items = new List<WeightedItem>();
    }

    public WeightedRandomizer(int seed)
    {
        random = new Random(seed);
        items = new List<WeightedItem>();
    }

    public WeightedRandomizer(Random random)
    {
        this.random = random;
        items = new List<WeightedItem>();
    }

    public void AddItem(T item, int weight)
    {
        WeightedItem weightedItem = items.Find(i => i.item.Equals(item));
        totalWeight += weight;
        if (weightedItem == null)
        {
            weightedItem = new WeightedItem(item, weight);
            items.Add(weightedItem);
        }
        else
            weightedItem.weight += weight;
    }

    public void RemoveItem(T item)
    {
        WeightedItem weightedItem = items.Find(i => i.item.Equals(item));
        totalWeight -= weightedItem.weight;
        items.Remove(weightedItem);
    }

    public void RemoveItemWeight(T item, int weight)
    {
        WeightedItem weightedItem = items.Find(i => i.item.Equals(item));
        if (weightedItem.weight <= weight)
        {
            totalWeight -= weightedItem.weight;
            items.Remove(weightedItem);
        }
        else
        {
            weightedItem.weight -= weight;
            totalWeight -= weight;
        }
    }

    public void SetItemWeight(T item, int weight)
    {
        WeightedItem weightedItem = items.Find(i => i.item.Equals(item));
        if (weightedItem == null)
        {
            totalWeight += weight;
            weightedItem = new WeightedItem(item, weight);
            items.Add(weightedItem);
        }
        else
        {
            totalWeight += weight - weightedItem.weight;
            weightedItem.weight = weight;
        }
    }

    public void Clear()
    {
        totalWeight = 0;
        items.Clear();
    }

    public T GetRandomItem()
    {
        int weightedValue = random.Next(0, totalWeight);
        for (int i = 0; i < items.Count; i++)
        {
            if (weightedValue < items[i].weight)
                return items[i].item;

            weightedValue -= items[i].weight;
        }

        if (items.Count >= 1)
            return items[0].item;

        throw new ArgumentNullException("Weighted randomizer couldn't return an item");
    }

    private class WeightedItem
    {
        public readonly T item;
        public int weight;

        public WeightedItem(T item, int weight)
        {
            this.item = item;
            this.weight = weight;
        }
    }
}
