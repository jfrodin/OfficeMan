namespace OfficeMan.Core;

/// <summary>
/// Stateless, keyed randomness. Every roll is derived from the run seed and a key,
/// so reloading a save and making the same choice reproduces the same luck.
/// </summary>
public static class Rng
{
    public static ulong Hash(int seed, string key)
    {
        const ulong fnvOffset = 14695981039346656037UL;
        const ulong fnvPrime = 1099511628211UL;

        ulong h = fnvOffset;
        foreach (char c in key)
        {
            h ^= c;
            h *= fnvPrime;
        }
        return Mix(h ^ Mix((ulong)(uint)seed + 0x9E3779B97F4A7C15UL));
    }

    /// <summary>Uniform value in [0, 1).</summary>
    public static double Unit(int seed, string key) => (Hash(seed, key) >> 11) * (1.0 / (1UL << 53));

    /// <summary>Uniform integer in [minInclusive, maxExclusive).</summary>
    public static int Range(int seed, string key, int minInclusive, int maxExclusive)
    {
        if (maxExclusive <= minInclusive) return minInclusive;
        return minInclusive + (int)(Hash(seed, key) % (ulong)(maxExclusive - minInclusive));
    }

    public static bool Percent(int seed, string key, int percent) => Unit(seed, key) * 100.0 < percent;

    /// <summary>Picks an index with probability proportional to its weight. Returns -1 if all weights are zero.</summary>
    public static int Weighted(int seed, string key, IReadOnlyList<int> weights)
    {
        int total = 0;
        foreach (int w in weights) total += Math.Max(0, w);
        if (total == 0) return -1;

        int roll = Range(seed, key, 0, total);
        for (int i = 0; i < weights.Count; i++)
        {
            roll -= Math.Max(0, weights[i]);
            if (roll < 0) return i;
        }
        return weights.Count - 1;
    }

    private static ulong Mix(ulong z)
    {
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        return z ^ (z >> 31);
    }
}
