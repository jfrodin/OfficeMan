using OfficeMan.Core;

namespace OfficeMan.Tests;

public class RngTests
{
    [Fact]
    public void SameSeedAndKeyGiveSameResult()
    {
        Assert.Equal(Rng.Hash(42, "check:x:0"), Rng.Hash(42, "check:x:0"));
        Assert.NotEqual(Rng.Hash(42, "check:x:0"), Rng.Hash(42, "check:x:1"));
        Assert.NotEqual(Rng.Hash(42, "check:x:0"), Rng.Hash(43, "check:x:0"));
    }

    [Fact]
    public void RangeStaysInBoundsAndCoversThem()
    {
        var seen = new HashSet<int>();
        for (int i = 0; i < 1000; i++)
        {
            int v = Rng.Range(7, $"k{i}", -1, 2);
            Assert.InRange(v, -1, 1);
            seen.Add(v);
        }
        Assert.Equal(3, seen.Count);
    }

    [Fact]
    public void PercentIsRoughlyCalibrated()
    {
        int hits = Enumerable.Range(0, 10_000).Count(i => Rng.Percent(1, $"p{i}", 30));
        Assert.InRange(hits, 2700, 3300);
    }

    [Fact]
    public void WeightedNeverPicksZeroWeight()
    {
        for (int i = 0; i < 1000; i++)
            Assert.NotEqual(1, Rng.Weighted(3, $"w{i}", new[] { 1, 0, 5 }));
        Assert.Equal(-1, Rng.Weighted(3, "none", new[] { 0, 0 }));
    }
}
