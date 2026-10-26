using OfficeMan.Core;
using OfficeMan.Core.World;

namespace OfficeMan.Tests;

[Collection("content")]
public class TraitTests(ContentFixture fixture)
{
    [Fact]
    public void EveryLifeStartsFromTheBaselineWithOneOrTwoNudges()
    {
        for (int seed = 1; seed <= 300; seed++)
        {
            var s = new WorldState { Seed = seed };
            Teo.StartingTraits(s);
            int moved = 0;
            foreach (string t in Teo.Traits)
            {
                int d = s.Get("trait_" + t) - Teo.Baseline[t];
                Assert.InRange(s.Get("trait_" + t), Teo.TraitMin, Teo.TraitMax);
                Assert.InRange(d, -1, 1);
                if (d != 0) moved++;
            }
            Assert.InRange(moved, 1, 2);
        }
    }

    [Fact]
    public void TheSameSeedGivesTheSameMan()
    {
        var a = new WorldState { Seed = 42 };
        var b = new WorldState { Seed = 42 };
        Teo.StartingTraits(a);
        Teo.StartingTraits(b);
        foreach (string t in Teo.Traits) Assert.Equal(a.Get("trait_" + t), b.Get("trait_" + t));
    }

    [Fact]
    public void ALastingShiftHappensOncePerReasonAndOnlyIfSomethingChanges()
    {
        var s = new WorldState { Seed = 1 };
        s.Set("trait_nerve", 1);
        Assert.True(Teo.Shift(s, "nerve", 1, "stood_up"));
        Assert.Equal(2, s.Get("trait_nerve"));
        Assert.False(Teo.Shift(s, "nerve", 1, "stood_up"));                // once per reason
        Assert.False(Teo.Shift(s, "nerve", 1, "another_reason"));         // already at the bold end
        Assert.Single(s.TraitShifts);
    }

    [Fact]
    public void AtMostOneLastingShiftADay()
    {
        var s = new WorldState { Seed = 1 };
        Assert.True(Teo.Shift(s, "nerve", 1, "stood_up"));
        Assert.False(Teo.Shift(s, "warmth", 1, "stood_by_kornel"));       // same day
        Assert.Equal(0, s.Get("trait_warmth"));
        s.Day = 1;
        Assert.False(Teo.Shift(s, "warmth", 1, "stood_by_kornel"));       // that moment has passed
        Assert.True(Teo.Shift(s, "honesty", 1, "told_magda"));            // a new day, a new moment
        Assert.Equal(2, s.TraitShifts.Count);
    }

    [Fact]
    public void DrinkMakesHimBolderForTheMoment()
    {
        var s = new WorldState { Seed = 1 };
        Teo.StartingTraits(s);
        int sober = Teo.Nerve(s), trait = s.Get("trait_nerve");
        s.Set("alcohol", 6);
        Assert.True(Teo.Nerve(s) > sober);
        Assert.Equal(trait, s.Get("trait_nerve"));   // the trait itself is untouched
    }

    [Fact]
    public void HindsightNamesTheCauseSometimes()
    {
        string[] hindsight = { "zhivka talking", "wasn't invited", "kind of man who says it", "I always wait", "Careful takes energy", "knew it would", "That's also a Tuesday" };
        int seen = 0;
        for (int seed = 1; seed <= 200; seed++)
        {
            var game = Game.New(fixture.Content, seed);
            int guard = 0;
            while (!game.IsOver && guard++ < 2000)
            {
                seen += game.Current.Lines.Count(l => hindsight.Any(h => l.Text.Contains(h)));
                var choices = game.Current.Choices;
                int pick = choices.ToList().FindIndex(c => c.Tags.Contains("bold") || c.Tags.Contains("drink"));
                game.Choose(pick >= 0 && Rng.Range(seed, $"h:{guard}", 0, 3) > 0 ? pick : Rng.Range(seed, $"t:{guard}", 0, choices.Count));
            }
        }
        Assert.True(seen > 0, "no hindsight line was ever shown");
    }

    [Fact]
    public void TheEndSaysInWordsWhoTeoWas()
    {
        for (int seed = 1; seed <= 40; seed++)
        {
            var game = Game.New(fixture.Content, seed);
            int guard = 0;
            while (!game.IsOver && guard++ < 2000) game.Choose(Rng.Range(seed, $"t:{guard}", 0, game.Current.Choices.Count));
            Assert.True(game.IsOver);
            Assert.InRange(game.Current.Today.Count, 1, 3);
            Assert.All(game.Current.Today, line => Assert.DoesNotContain("—", line));
        }
    }
}
