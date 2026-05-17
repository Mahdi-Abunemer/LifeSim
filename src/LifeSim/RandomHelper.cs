using System;
using System.Collections.Generic;

namespace LifeSim;

public static class RandomHelper
{
    private static readonly Random _random = new();

    public static int Next(int min, int max) => _random.Next(min, max);

    public static double NextDouble() => _random.NextDouble();

    public static T? Pick<T>(this IList<T> list) 
        => list.Count == 0 
        ? default 
        : list[_random.Next(0, list.Count)];

    public static bool Chance(double probability) => NextDouble() < probability;
}
