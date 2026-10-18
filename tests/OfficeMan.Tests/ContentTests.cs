using OfficeMan.Core;
using OfficeMan.Core.Content;

namespace OfficeMan.Tests;

/// <summary>Loads the real game content once and plays it.</summary>
public class ContentFixture
{
    public ContentBundle Content { get; } = ContentBundle.Load(ContentLocator.Find());
}

public class ContentTests(ContentFixture fixture) : IClassFixture<ContentFixture>
{
    private readonly ContentBundle _content = fixture.Content;

    [Fact]
    public void ContentCompilesWithoutWarnings()
    {
        Assert.NotEmpty(_content.Storylets);
        Assert.Empty(_content.Warnings);
    }

    [Fact]
    public void EveryHubPhaseCanBeLeft()
    {
        foreach (var phase in _content.Schedule.Phases.Where(p => p.Mode == PhaseMode.Hub))
            Assert.Contains(_content.Storylets, s => s.InPhase(phase.Id) && s.Kind == StoryletKind.Exit);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void RandomLivesFinishWithoutCrashesOrDeadEnds(int policy)
    {
        for (int seed = 1; seed <= 150; seed++)
        {
            var game = Play(seed, policy, untilDecision: int.MaxValue);
            Assert.True(game.IsOver, $"seed {seed} did not finish");
            Assert.Equal(0, game.DeadEnds);
        }
    }

    [Fact]
    public void SameSeedAndChoicesGiveSameLife()
    {
        var a = Play(77, policy: 1, untilDecision: int.MaxValue);
        var b = Play(77, policy: 1, untilDecision: int.MaxValue);
        Assert.Equal(a.State.History, b.State.History);
        Assert.Equal(a.State.Headlines, b.State.Headlines);
    }

    [Theory]
    [InlineData(5, 3)]
    [InlineData(9, 20)]
    [InlineData(31, 40)]
    public void SaveAndLoadResumeTheSameLife(int seed, int saveAt)
    {
        // Play to a point, save, then finish both the original and the restored game identically.
        var original = Play(seed, policy: 2, untilDecision: saveAt);
        string json = original.Save().ToJson();
        var restored = Game.Load(_content, SaveGame.FromJson(json));

        Assert.Equal(original.Current.Choices.Select(c => c.Text), restored.Current.Choices.Select(c => c.Text));

        int n = saveAt;
        while (!original.IsOver)
        {
            int pick = Pick(seed, policy: 2, n++, original.Current.Choices.Count);
            original.Choose(pick);
            restored.Choose(pick);
        }
        Assert.True(restored.IsOver);
        Assert.Equal(original.State.History, restored.State.History);
        Assert.Equal(original.State.Headlines, restored.State.Headlines);
    }

    private Game Play(int seed, int policy, int untilDecision)
    {
        var game = Game.New(_content, seed);
        for (int n = 0; n < untilDecision && !game.IsOver; n++)
        {
            Assert.True(n < 2000, "runaway game");
            game.Choose(Pick(seed, policy, n, game.Current.Choices.Count));
        }
        return game;
    }

    /// <summary>0 = always first, 1 = always last, 2 = seeded random.</summary>
    private static int Pick(int seed, int policy, int n, int count) => policy switch
    {
        0 => 0,
        1 => count - 1,
        _ => Rng.Range(seed, $"test:{n}", 0, count),
    };
}
