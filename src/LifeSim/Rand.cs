using System;
using System.Collections.Generic;

namespace LifeSim;

public static class Rand
{
    private static readonly Random Random = new();

    public static int Next(int min, int max) => Random.Next(min, max);

    public static double NextDouble() => Random.NextDouble();

    public static T? Pick<T>(this IList<T> list) => list.Count == 0 ? default : list[Random.Next(0, list.Count)];

    public static bool Chance(double p) => NextDouble() < p;
}
