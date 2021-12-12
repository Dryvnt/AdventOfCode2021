using System.Diagnostics;

int AllPaths(Dictionary<string, HashSet<string>> dictionary, Part part)
{
    var stack = new Stack<SearchEntry>();

    var specials = new[] { (string?)null };

    if (part == Part.Two)
        specials = specials
            .Concat(dictionary.Keys.Where(n => n.ToLowerInvariant() == n).Except(new[] { "start", "end" })).ToArray();

    var startPath = new[] { "start" }.ToList();
    var allLegalVertices = dictionary.Keys.Except(new[] { "start" }).ToHashSet();
    stack.Push(new SearchEntry("start", startPath, allLegalVertices, new HashSet<string>(), false));

    var list = new List<List<string>>();

    while (stack.Any())
    {
        var v = stack.Pop();

        if (v.Node == "end")
        {
            list.Add(v.Path);
            continue;
        }


        var neighbors = dictionary[v.Node].Where(v.LegalMoves.Contains);

        foreach (var n in neighbors)
        {
            var newLegal = v.LegalMoves;
            var exceptionMade = v.Part2ExceptionMade;
            var newUsed = v.UsedSmall;
            if (n.ToLowerInvariant() == n)
            {
                switch (part)
                {
                    case Part.One:
                    case Part.Two when exceptionMade:
                        newLegal = newLegal.ToHashSet();
                        newLegal.Remove(n);
                        break;
                    case Part.Two:
                    {
                        if (v.Path.Contains(n))
                        {
                            newLegal = newLegal.Except(newUsed).ToHashSet();
                            exceptionMade = true;
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(part), part, null);
                }

                newUsed = newUsed.ToHashSet();
                newUsed.Add(n);
            }

            var newPath = v.Path.ToList();
            newPath.Add(n);
            stack.Push(new SearchEntry(n, newPath, newLegal, newUsed, exceptionMade));
        }
    }

    return list.Count;
}

var input = File.ReadAllLines("input");

var adjacencyDict = new Dictionary<string, HashSet<string>>();

foreach (var line in input)
{
    var split = line.Split("-");

    var from = split[0];
    var to = split[1];

    if (!adjacencyDict.ContainsKey(from)) adjacencyDict[from] = new HashSet<string>();
    if (!adjacencyDict.ContainsKey(to)) adjacencyDict[to] = new HashSet<string>();

    adjacencyDict[from].Add(to);
    adjacencyDict[to].Add(from);
}

var c = new Stopwatch();
c.Start();
Console.WriteLine($"Part 1: {AllPaths(adjacencyDict, Part.One)}");
Console.WriteLine($"Part 2: {AllPaths(adjacencyDict, Part.Two)}");
c.Stop();
Console.WriteLine($"Elapsed time: {c.Elapsed}");

// I benchmarked it, and the non-immutable data structures are faster than the immutable ones,
// despite the immutable ergonomics being more intuitive for this sort of "progressively build
// the data structure down the tree" workflow. Oh well.

internal record SearchEntry(string Node, List<string> Path, HashSet<string> LegalMoves, HashSet<string> UsedSmall,
    bool Part2ExceptionMade);

internal enum Part
{
    One,
    Two,
}