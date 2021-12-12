using System.Collections.Immutable;
using System.Diagnostics;

int NumberPaths(Dictionary<Node, HashSet<Node>> dictionary, Part part)
{
    var stack = new Stack<SearchEntry>();

    var start = dictionary.Keys.Single(n => n.Name is "start");
    var end = dictionary.Keys.Single(n => n.Name is "end");

    var startPath = new[] { start }.ToImmutableList();
    var allLegalVertices = dictionary.Keys.Where(n => n != start).ToImmutableHashSet();
    stack.Push(new SearchEntry(start, startPath, allLegalVertices, false));

    var list = new List<ImmutableList<Node>>();

    // Classic depth-first search :)
    while (stack.Any())
    {
        var v = stack.Pop();

        if (v.Node == end)
        {
            list.Add(v.Path);
            continue;
        }


        var neighbors = dictionary[v.Node].Where(v.LegalMoves.Contains);

        foreach (var n in neighbors)
        {
            var newLegal = v.LegalMoves;
            var exceptionMade = v.Part2ExceptionMade;
            if (n.Small)
            {
                switch (part)
                {
                    case Part.One:
                    case Part.Two when exceptionMade:
                        newLegal = newLegal.Remove(n);
                        break;
                    case Part.Two:
                    {
                        if (v.Path.Contains(n))
                        {
                            newLegal = newLegal.Except(v.Path.Where(c => c.Small));
                            exceptionMade = true;
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(part), part, null);
                }
            }

            stack.Push(new SearchEntry(n, v.Path.Add(n), newLegal, exceptionMade));
        }
    }

    return list.Count;
}

var input = File.ReadAllLines("input");

var adjacencyDict = new Dictionary<Node, HashSet<Node>>();

foreach (var line in input)
{
    var split = line.Split("-");

    var fromString = split[0];
    var from = new Node(fromString, fromString == fromString.ToLowerInvariant());
    var toString = split[1];
    var to = new Node(toString, toString == toString.ToLowerInvariant());

    if (!adjacencyDict.ContainsKey(from)) adjacencyDict[from] = new HashSet<Node>();
    if (!adjacencyDict.ContainsKey(to)) adjacencyDict[to] = new HashSet<Node>();

    adjacencyDict[from].Add(to);
    adjacencyDict[to].Add(from);
}

// I benchmarked it, and the non-immutable data structures are faster than the
// immutable ones by a ~30%, but the ergonomics of immutable data structures fit
// the "build the data structures as we build the search tree" approach better
// I've satisfied my own curiosity w.r.t. speed and I prefer the cleaner approach since it's fast enough.
var c = new Stopwatch();
c.Start();
for (var i = 0; i < 1; i++)
{
    Console.WriteLine($"Part 1: {NumberPaths(adjacencyDict, Part.One)}");
    Console.WriteLine($"Part 2: {NumberPaths(adjacencyDict, Part.Two)}");
}

c.Stop();
Console.WriteLine($"Elapsed time: {c.Elapsed}");

internal record SearchEntry(Node Node, ImmutableList<Node> Path, ImmutableHashSet<Node> LegalMoves,
    bool Part2ExceptionMade);

internal record Node(string Name, bool Small);

internal enum Part
{
    One,
    Two,
}