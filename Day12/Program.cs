using System.Collections.Immutable;

bool Part1Condition(SearchEntry entry)
{
    var (node, path) = entry;
    if (node.ToLowerInvariant() != node) return true;

    var seen = new HashSet<string>();

    foreach (var n in path.Where(n => n == n.ToLowerInvariant()))
    {
        if (seen.Contains(n)) return false;

        seen.Add(n);
    }

    return true;
}

bool Part2Condition(SearchEntry entry)
{
    var (node, path) = entry;
    if (node.ToLowerInvariant() != node) return true;

    var seen = new HashSet<string>();

    var special = (string?)null;

    foreach (var n in path.Where(n => n == n.ToLowerInvariant()))
    {
        if (seen.Contains(n))
        {
            if (n is "start" or "end") return false;
            if (special is null) special = n;
            else return false;
        }

        seen.Add(n);
    }

    return true;
}

// This is a very brute-force-y solution.
// I'm sure there's a smarter way to do it, but at this point it's late and I'm tired.
int DoPart(IDictionary<string, HashSet<string>> adjacencyDict, Func<SearchEntry, bool> conditionFunction)
{
    var allPaths = new List<ImmutableList<string>>();

    var stack = new Stack<SearchEntry>();
    stack.Push(new SearchEntry("start", new[] { "start" }.ToImmutableList()));
    while (stack.Any())
    {
        var v = stack.Pop();

        if (v.Node == "end")
        {
            allPaths.Add(v.Path);
            continue;
        }

        var neighbors = adjacencyDict[v.Node];

        var neighborEntries = neighbors
            .Select(n => new SearchEntry(n, v.Path.Add(n)))
            .Where(conditionFunction);

        foreach (var newEntry in neighborEntries) stack.Push(newEntry);
    }

    return allPaths.Count;
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

Console.WriteLine($"Part 1: {DoPart(adjacencyDict, Part1Condition)}");
Console.WriteLine($"Part 2: {DoPart(adjacencyDict, Part2Condition)}");

internal record SearchEntry(string Node, ImmutableList<string> Path);