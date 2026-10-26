using OfficeMan.Core;
using OfficeMan.Core.Content;

namespace OfficeMan.Tests;

/// <summary>
/// Other people's own day: offstage storylets, what each person is doing, and being stopped on the way out.
/// Uses a tiny story of its own, so the engine is tested apart from the game's content.
/// </summary>
[Collection("content")]   // the ink compiler isn't thread-safe
public class OffstageTests
{
    private const string Phases = """
        { "firstPhase": "evening", "phases": [
          { "id": "evening", "title": "Test", "mode": "hub", "day": 0, "start": "18:00", "end": "20:00", "next": "end" } ] }
        """;

    private const string Externals = """
        EXTERNAL set_doing(who, what)
        EXTERNAL doing(who, what)
        EXTERNAL feel(who, delta)
        EXTERNAL feels(who)

        """;

    private const string Room = """

        === look
        # phase: evening
        # kind: hub
        # choice: Look around.
        # repeat
        # duration: 20
        { doing("magda", "river"):
            Magda's gone.
        - else:
            Magda's here.
        }
        -> DONE

        === find_magda
        # phase: evening
        # kind: hub
        # choice: Go down to the river.
        # when: doing(magda, river) and feels(magda) < 0
        Found her.
        -> DONE

        === leave
        # phase: evening
        # kind: exit
        # choice: Leave.
        Bye.
        -> DONE
        """;

    [Fact]
    public void PeopleMoveOnTheirOwnAndTheWorldFollows()
    {
        var game = Game.New(Load(Externals + """
            === magda_goes
            # phase: evening
            # kind: offstage
            # at: 18:30
            ~ set_doing("magda", "river")
            ~ feel("magda", -2)
            MAGDA: I'm fine. #heard
            -> DONE
            """ + Room), seed: 1);

        Assert.DoesNotContain(game.Current.Choices, c => c.Text == "Go down to the river.");
        game.Choose(0);   // 18:00 → 18:20, too early
        Assert.Equal("", game.State.Character("magda").Doing);

        game.Choose(0);   // 18:20 → 18:40: she goes while Teo is looking the other way
        Assert.Contains(game.Current.Lines, l => l.Text == "Magda's here.");
        Assert.Contains(game.Current.Lines, l => l.Text == "MAGDA: I'm fine." && l.Tags.Contains("heard"));
        Assert.Equal("river", game.State.Character("magda").Doing);
        Assert.Equal(-2, game.State.Character("magda").Feels);
        Assert.Contains(game.Current.Choices, c => c.Text == "Go down to the river.");
        Assert.Equal(1, game.State.Plays("magda_goes"));
    }

    [Fact]
    public void OffstageSurvivesSaveAndLoad()
    {
        var content = Load(Externals + """
            === magda_goes
            # phase: evening
            # kind: offstage
            # at: 18:10
            ~ set_doing("magda", "river")
            ~ feel("magda", -1)
            -> DONE
            """ + Room);
        var game = Game.New(content, seed: 3);
        game.Choose(0);
        var restored = Game.Load(content, SaveGame.FromJson(game.Save().ToJson()));
        Assert.Equal("river", restored.State.Character("magda").Doing);
        Assert.Equal(-1, restored.State.Character("magda").Feels);
    }

    [Fact]
    public void OffstageCantSpeakToTeo()
    {
        var game = Game.New(Load(Externals + """
            === magda_goes
            # phase: evening
            # kind: offstage
            # at: 18:10
            Magda goes down to the river.
            -> DONE
            """ + Room), seed: 1);
        Assert.Throws<ContentException>(() => game.Choose(0));
    }

    [Fact]
    public void OffstageCantOfferChoices()
    {
        var game = Game.New(Load(Externals + """
            === magda_goes
            # phase: evening
            # kind: offstage
            # at: 18:10
            * [Follow her.]
            -> DONE
            """ + Room), seed: 1);
        Assert.Throws<ContentException>(() => game.Choose(0));
    }

    [Fact]
    public void OffstageCantRepeat()
    {
        var e = Assert.Throws<ContentException>(() => Load(Externals + """
            === magda_goes
            # phase: evening
            # kind: offstage
            # repeat
            ~ feel("magda", 1)
            -> DONE
            """ + Room));
        Assert.Contains("can't repeat", e.Message);
    }

    [Fact]
    public void SomeoneCanTalkHimOutOfLeaving()
    {
        var game = Game.New(Load("""
            EXTERNAL stay()
            EXTERNAL played(storylet)

            === look
            # phase: evening
            # kind: hub
            # choice: Look around.
            # repeat
            Nothing.
            -> DONE

            === leave
            # phase: evening
            # kind: exit
            # choice: Go home.
            # repeat
            # duration: 30
            { played("leave") == 0:
                -> stopped
            }
            I go.
            -> DONE

            = stopped
            KORNEL: You're going? I'm reading at eight.
            * [Stay a bit longer.]
                ~ stay()
            * [Go anyway.]
            - -> DONE
            """), seed: 1);

        int leave = game.Current.Choices.ToList().FindIndex(c => c.Text == "Go home.");
        game.Choose(leave);
        game.Choose(0);   // stay
        Assert.Equal("evening", game.State.PhaseId);
        Assert.Equal(18 * 60, game.State.Minute);   // the walk home didn't happen
        Assert.Contains(game.Current.Choices, c => c.Text == "Go home.");

        game.Choose(game.Current.Choices.ToList().FindIndex(c => c.Text == "Go home."));
        Assert.True(game.IsOver);
    }

    [Fact]
    public void ASingleChoiceJustHappens()
    {
        var game = Game.New(Load("""
            === wake
            # phase: evening
            # kind: hub
            # choice: Wake up.
            I lie there.
            * [Get up.]
            - I get up.
            -> DONE

            === leave
            # phase: evening
            # kind: exit
            # choice: Leave.
            Bye.
            -> DONE
            """), seed: 1);

        game.Choose(game.Current.Choices.ToList().FindIndex(c => c.Text == "Wake up."));
        Assert.Equal(PresentationKind.Hub, game.Current.Kind);   // no stop at "Get up."
        Assert.Contains(game.Current.Lines, l => l.Text == "I lie there.");
        Assert.Contains(game.Current.Lines, l => l.Text == "I get up.");
    }

    private static ContentBundle Load(string ink)
    {
        string dir = Path.Combine(Path.GetTempPath(), "officeman-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(dir, "ink"));
        try
        {
            File.WriteAllText(Path.Combine(dir, "ink", "main.ink"), ink);
            File.WriteAllText(Path.Combine(dir, "phases.json"), Phases);
            return ContentBundle.Load(dir);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }
}
