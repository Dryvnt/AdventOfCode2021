var input = File.ReadAllLines("input");

var errorScore = 0;
// Long because integer overflow
var completionScores = new List<long>();

foreach (var line in input)
{
    var stack = new Stack<char>();
    var hadError = false;
    foreach (var c in line)
    {
        SyntaxError? syntaxError = null;
        switch (c)
        {
            case '(' or '[' or '{' or '<':
                stack.Push(c);
                break;
            case ')' or ']' or '}' or '>':
                var p = stack.Pop();
                if (c == Pair(p)) break;
                syntaxError = new SyntaxError(p, c);
                break;
            default:
                throw new NotImplementedException();
        }

        if (syntaxError is not null)
        {
            hadError = true;
            errorScore += ErrorScore(syntaxError.Close);
        }
    }

    if (hadError) continue;

    long lineScore = 0;
    while (stack.Any())
    {
        var c = stack.Pop();
        lineScore *= 5;
        lineScore += CompletionScore(Pair(c));
    }

    completionScores.Add(lineScore);
}

// Part 1
Console.WriteLine($"Part 1: {errorScore}");

// Part 2
completionScores.Sort();
Console.WriteLine($"Part 2: {completionScores[completionScores.Count / 2]}");

// Helper functions
char Pair(char c)
{
    return c switch
    {
        '(' => ')',
        '[' => ']',
        '{' => '}',
        '<' => '>',
        _ => throw new NotImplementedException(),
    };
}

int ErrorScore(char c)
{
    return c switch
    {
        ')' => 3,
        ']' => 57,
        '}' => 1197,
        '>' => 25137,
        _ => throw new NotImplementedException(),
    };
}

int CompletionScore(char c)
{
    return c switch
    {
        ')' => 1,
        ']' => 2,
        '}' => 3,
        '>' => 4,
        _ => throw new NotImplementedException(),
    };
}

internal record SyntaxError(char Open, char Close);