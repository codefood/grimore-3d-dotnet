using System;
using System.Collections.Generic;
using System.Linq;

namespace Grimore;

public static class EnumerableExtensions
{
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> input, Action<T> action)
    {
        var enumerable = input as T[] ?? input.ToArray();
        foreach (var element in enumerable)
        {
            action(element);
        }

        return enumerable;
    }
}