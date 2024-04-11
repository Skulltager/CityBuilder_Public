using System.Collections.Generic;

public class UnorganizedList<T>
{
    private readonly List<T> list;
    public int count { private set; get; }

    public UnorganizedList()
    {
        list = new List<T>();
        count = 0;
    }

    public void Add(T item)
    {
        if (list.Count == count)
        {
            count++;
            list.Add(item);
            return;
        }

        list[count++] = item;
    }

    public void Remove(T item)
    {
        int index = list.IndexOf(item, 0, count);
        if (index == count - 1)
        {
            count--;
            return;
        }

        list[index] = list[--count];
    }

    public void RemoveAt(int index)
    {
        if (index == count - 1)
        {
            count--;
            return;
        }

        list[index] = list[--count];
    }

    public T GetItem(int index)
    {
        return list[index];
    }

    public void Shuffle()
    {
        list.Shuffle(count);
    }
}