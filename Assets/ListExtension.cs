using System.Collections.Generic;
using System;

public static class ListExtensions
{
    private static readonly Random random = new Random();

    public static T RandomElement<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
        {
            throw new ArgumentException("List is null or empty");
        }

        return list[random.Next(list.Count)];
    }
}