using OfficeMan.Core.Conditions;

namespace OfficeMan.Tests;

public class ConditionTests
{
    private sealed class Ctx : IConditionContext
    {
        public Dictionary<string, int> Values { get; } = new();
        public HashSet<string> Memories { get; } = new();

        public int Value(string name) => Values.TryGetValue(name, out int v) ? v : 0;

        public int Call(string function, IReadOnlyList<string> args) => function switch
        {
            "remembers" => Memories.Contains($"{args[0]}:{args[1]}") ? 1 : 0,
            "played" => Value("played_" + args[0]),
            _ => throw new FormatException(function),
        };
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("drunk >= 2", true)]
    [InlineData("drunk > 2", false)]
    [InlineData("!drunk", false)]
    [InlineData("not missing", true)]
    [InlineData("drunk == 2 && mood < 0", true)]
    [InlineData("drunk == 3 || mood < 0", true)]
    [InlineData("drunk == 3 or mood > 0", false)]
    [InlineData("drunk + mood == 1", true)]
    [InlineData("-mood == 1", true)]
    [InlineData("(drunk >= 2 and mood > 0) or remembers(magda, kissed)", true)]
    [InlineData("remembers(magda, rude)", false)]
    [InlineData("played(\"wake\") > 0", true)]
    [InlineData("time >= 21:30", true)]
    [InlineData("time < 21:30", false)]
    public void Evaluates(string source, bool expected)
    {
        var ctx = new Ctx();
        ctx.Values["drunk"] = 2;
        ctx.Values["mood"] = -1;
        ctx.Values["time"] = 21 * 60 + 45;
        ctx.Values["played_wake"] = 1;
        ctx.Memories.Add("magda:kissed");

        Assert.Equal(expected, Condition.Parse(source).IsTrue(ctx));
    }

    [Fact]
    public void AndBindsTighterThanOr()
    {
        var ctx = new Ctx();
        // 1 || (0 && 0) is true; (1 || 0) && 0 would be false.
        ctx.Values["a"] = 1;
        Assert.True(Condition.Parse("a || b && c").IsTrue(ctx));
    }

    [Theory]
    [InlineData("drunk >=")]
    [InlineData("(drunk")]
    [InlineData("drunk $ 2")]
    [InlineData("remembers(magda, )x")]
    public void RejectsMalformed(string source) =>
        Assert.Throws<FormatException>(() => Condition.Parse(source));
}
