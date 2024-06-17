using System;
using System.Collections.Generic;

public class UnorganizedList<T>
{
    private readonly List<T> list;
    public int Count { private set; get; }

    public UnorganizedList()
    {
        list = new List<T>();
        Count = 0;
    }

    public void Add(T item)
    {
        if (list.Count == Count)
        {
            Count++;
            list.Add(item);
            return;
        }

        list[Count++] = item;
    }

    public void Remove(T item)
    {
        int index = list.IndexOf(item, 0, Count);
        if (index == Count - 1)
        {
            Count--;
            return;
        }

        list[index] = list[--Count];
    }

    public void RemoveAt(int index)
    {
        if (index == Count - 1)
        {
            Count--;
            return;
        }

        list[index] = list[--Count];
    }

    public T GetItem(int index)
    {
        return list[index];
    }

    public void Shuffle()
    {
        list.Shuffle(Count);
    }

    public void Shuffle(Random random)
    {
        list.Shuffle(random, Count);
    }
}