using System;
using System.Collections.Generic;

public class NonScalingList<T>
{
    public readonly List<T> internalList;
    public int Count { private set; get; }

    public NonScalingList()
    {
        internalList = new List<T>();
        Count = 0;
    }

    public void Add(T item)
    {
        if (internalList.Count == Count)
        {
            Count++;
            internalList.Add(item);
            return;
        }

        internalList[Count++] = item;
    }

    public void Remove(T item)
    {
        int index = internalList.IndexOf(item, 0, Count);
        RemoveAt(index);
    }

    public void RemoveAt(int index)
    {
        for (int i = index; i < Count - 1; i++)
            internalList[i] = internalList[i + 1];

        Count--;
    }

    public void Clear()
    {
        Count = 0;
    }

    public void Shuffle()
    {
        internalList.Shuffle(Count);
    }

    public T this[int i]
    {
        get { return internalList[i]; }
        set { internalList[i] = value; }
    }

    public int FindIndex(Predicate<T> predicate)
    {
        return internalList.FindIndex(0, Count, predicate);
    }

    public int FindIndex(int startIndex, Predicate<T> predicate)
    {
        return internalList.FindIndex(startIndex, Count - startIndex, predicate);
    }

    public void Insert(int index, T item)
    {
        if (internalList.Count == Count)
            internalList.Add(default);

        for(int i = Count - 1; i >= index; i--)
            internalList[i + 1] = internalList[i];

        internalList[index] = item;
        Count++;
    }
}