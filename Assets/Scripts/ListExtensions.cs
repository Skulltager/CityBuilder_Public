using System;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list, Random random)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void Shuffle<T>(this IList<T> list, int count)
    {
        int n = count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    public static T GetRandomItem<T>(this IList<T> list, Random random)
    {
        int randomIndex = random.Next(list.Count);
        return list[randomIndex];
    }

    public static T GetRandomItem<T>(this IList<T> list)
    {
        int randomIndex = UnityEngine.Random.Range(0, list.Count);
        return list[randomIndex];
    }

    public static bool TryFind<T>(this IList<T> source, Func<T, bool> predicate, out T item)
    {
        item = source.FirstOrDefault(predicate);
        return item != null;
    }
}
