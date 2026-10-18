namespace OfficeMan.Core.Conditions;

/// <summary>What a condition can ask about the world.</summary>
public interface IConditionContext
{
    /// <summary>Value of a named quality or derived stat. Unknown names are 0.</summary>
    int Value(string name);

    /// <summary>Evaluates a named function such as <c>remembers(magda, rude)</c>. Arguments are raw identifiers.</summary>
    int Call(string function, IReadOnlyList<string> args);
}

/// <summary>
/// A compiled condition expression, as written in storylet tags:
/// <c># when: drunk >= 2 &amp;&amp; !remembers(magda, rude) &amp;&amp; time &lt; 21:30</c>
/// </summary>
/// <remarks>
/// Grammar: integers, time literals (<c>HH:MM</c> → minutes), identifiers, function calls with
/// identifier arguments, unary <c>! -</c>, binary <c>+ - &lt; &lt;= &gt; &gt;= == != &amp;&amp; ||</c>, parentheses.
/// <c>and</c>, <c>or</c>, <c>not</c> are accepted as word forms. Truth is non-zero.
/// </remarks>
public sealed class Condition
{
    private readonly Func<IConditionContext, int> _eval;

    public string Source { get; }

    private Condition(string source, Func<IConditionContext, int> eval)
    {
        Source = source;
        _eval = eval;
    }

    public static readonly Condition Always = new("true", _ => 1);

    public int Evaluate(IConditionContext ctx) => _eval(ctx);

    public bool IsTrue(IConditionContext ctx) => _eval(ctx) != 0;

    public static Condition Parse(string source)
    {
        if (string.IsNullOrWhiteSpace(source)) return Always;
        var parser = new Parser(source);
        var eval = parser.ParseExpression(0);
        parser.ExpectEnd();
        return new Condition(source.Trim(), eval);
    }

    public override string ToString() => Source;

    private sealed class Parser
    {
        private readonly List<Token> _tokens;
        private readonly string _source;
        private int _pos;

        public Parser(string source)
        {
            _source = source;
            _tokens = Tokenize(source);
        }

        private Token Peek => _tokens[_pos];

        private Token Next() => _tokens[_pos++];

        public void ExpectEnd()
        {
            if (Peek.Kind != TokenKind.End) throw Error($"unexpected '{Peek.Text}'");
        }

        private static int Precedence(string op) => op switch
        {
            "||" => 1,
            "&&" => 2,
            "==" or "!=" => 3,
            "<" or "<=" or ">" or ">=" => 4,
            "+" or "-" => 5,
            _ => 0,
        };

        public Func<IConditionContext, int> ParseExpression(int minPrecedence)
        {
            var left = ParseUnary();
            while (Peek.Kind == TokenKind.Op && Precedence(Peek.Text) > minPrecedence)
            {
                string op = Next().Text;
                var right = ParseExpression(Precedence(op));
                var l = left;
                left = op switch
                {
                    "||" => c => (l(c) != 0 || right(c) != 0) ? 1 : 0,
                    "&&" => c => (l(c) != 0 && right(c) != 0) ? 1 : 0,
                    "==" => c => l(c) == right(c) ? 1 : 0,
                    "!=" => c => l(c) != right(c) ? 1 : 0,
                    "<" => c => l(c) < right(c) ? 1 : 0,
                    "<=" => c => l(c) <= right(c) ? 1 : 0,
                    ">" => c => l(c) > right(c) ? 1 : 0,
                    ">=" => c => l(c) >= right(c) ? 1 : 0,
                    "+" => c => l(c) + right(c),
                    "-" => c => l(c) - right(c),
                    _ => throw Error($"unknown operator '{op}'"),
                };
            }
            return left;
        }

        private Func<IConditionContext, int> ParseUnary()
        {
            if (Peek.Kind == TokenKind.Op && Peek.Text == "!")
            {
                Next();
                var inner = ParseUnary();
                return c => inner(c) == 0 ? 1 : 0;
            }
            if (Peek.Kind == TokenKind.Op && Peek.Text == "-")
            {
                Next();
                var inner = ParseUnary();
                return c => -inner(c);
            }
            return ParsePrimary();
        }

        private Func<IConditionContext, int> ParsePrimary()
        {
            var token = Next();
            switch (token.Kind)
            {
                case TokenKind.Number:
                    int n = token.Number;
                    return _ => n;

                case TokenKind.LParen:
                    var inner = ParseExpression(0);
                    if (Next().Kind != TokenKind.RParen) throw Error("missing ')'");
                    return inner;

                case TokenKind.Ident:
                    string name = token.Text;
                    if (name == "true") return _ => 1;
                    if (name == "false") return _ => 0;
                    if (Peek.Kind != TokenKind.LParen) return c => c.Value(name);

                    Next(); // (
                    var args = new List<string>();
                    while (Peek.Kind != TokenKind.RParen)
                    {
                        var arg = Next();
                        if (arg.Kind != TokenKind.Ident && arg.Kind != TokenKind.Number)
                            throw Error($"function arguments must be names, got '{arg.Text}'");
                        args.Add(arg.Text);
                        if (Peek.Kind == TokenKind.Comma) Next();
                        else if (Peek.Kind != TokenKind.RParen) throw Error("expected ',' or ')'");
                    }
                    Next(); // )
                    var frozen = args.ToArray();
                    return c => c.Call(name, frozen);

                default:
                    throw Error($"unexpected '{token.Text}'");
            }
        }

        private FormatException Error(string message) =>
            new($"Condition \"{_source}\": {message}");

        private static List<Token> Tokenize(string s)
        {
            var tokens = new List<Token>();
            int i = 0;
            while (i < s.Length)
            {
                char c = s[i];
                if (char.IsWhiteSpace(c)) { i++; continue; }

                if (char.IsDigit(c))
                {
                    int start = i;
                    while (i < s.Length && char.IsDigit(s[i])) i++;
                    int value = int.Parse(s.AsSpan(start, i - start));
                    if (i + 1 < s.Length && s[i] == ':' && char.IsDigit(s[i + 1]))
                    {
                        int mStart = i + 1;
                        int j = mStart;
                        while (j < s.Length && char.IsDigit(s[j])) j++;
                        value = value * 60 + int.Parse(s.AsSpan(mStart, j - mStart));
                        i = j;
                    }
                    tokens.Add(new Token(TokenKind.Number, s[start..i], value));
                    continue;
                }

                if (char.IsLetter(c) || c == '_')
                {
                    int start = i;
                    while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i] == '_' || s[i] == '.')) i++;
                    string word = s[start..i];
                    switch (word)
                    {
                        case "and": tokens.Add(new Token(TokenKind.Op, "&&")); break;
                        case "or": tokens.Add(new Token(TokenKind.Op, "||")); break;
                        case "not": tokens.Add(new Token(TokenKind.Op, "!")); break;
                        default: tokens.Add(new Token(TokenKind.Ident, word)); break;
                    }
                    continue;
                }

                if (c == '"')
                {
                    // Quoted names are accepted as plain identifiers: played("x") == played(x).
                    int end = s.IndexOf('"', i + 1);
                    if (end < 0) throw new FormatException($"Condition \"{s}\": unterminated quote");
                    tokens.Add(new Token(TokenKind.Ident, s[(i + 1)..end]));
                    i = end + 1;
                    continue;
                }

                if (c == '(') { tokens.Add(new Token(TokenKind.LParen, "(")); i++; continue; }
                if (c == ')') { tokens.Add(new Token(TokenKind.RParen, ")")); i++; continue; }
                if (c == ',') { tokens.Add(new Token(TokenKind.Comma, ",")); i++; continue; }

                string two = i + 1 < s.Length ? s.Substring(i, 2) : "";
                if (two is "&&" or "||" or "==" or "!=" or "<=" or ">=")
                {
                    tokens.Add(new Token(TokenKind.Op, two));
                    i += 2;
                    continue;
                }
                if (c is '<' or '>' or '!' or '+' or '-')
                {
                    tokens.Add(new Token(TokenKind.Op, c.ToString()));
                    i++;
                    continue;
                }

                throw new FormatException($"Condition \"{s}\": unexpected character '{c}'");
            }
            tokens.Add(new Token(TokenKind.End, "<end>"));
            return tokens;
        }
    }

    private enum TokenKind { Number, Ident, Op, LParen, RParen, Comma, End }

    private readonly record struct Token(TokenKind Kind, string Text, int Number = 0);
}
