using System.Text;

string ApplyRules(string template, IReadOnlyDictionary<string, string> rules)
{
    var s = new StringBuilder();

    for (var i = 0; i < template.Length - 1; i++)
    {
        s.Append(template[i]);

        var window = template[i..(i + 2)];

        if (rules.TryGetValue(window, out var value)) s.Append(value);
    }

    s.Append(template.Last());

    return s.ToString();
}

// Kept as an example of what NOT to do if you want good runtime :)
int NaiveApplyAndCount(string template, IReadOnlyDictionary<string, string> rules, int iterations)
{
    for (var i = 0; i < iterations; i++) template = ApplyRules(template, rules);

    var counts = template.GroupBy(c => c).Select(g => g.Count()).ToList();

    return counts.Max() - counts.Min();
}

Dictionary<string, long> CountPairs(string template)
{
    var pairCounts = new Dictionary<string, long>();
    for (var i = 0; i < template.Length - 1; i++)
    {
        var window = template[i..(i + 2)];

        pairCounts.TryGetValue(window, out var c);

        pairCounts[window] = c + 1;
    }

    return pairCounts;
}

Dictionary<char, long> CountChars(Dictionary<string, long> dictionary, char lastChar)
{
    var charCounts = new Dictionary<char, long>();
    foreach (var (pair, count) in dictionary)
    {
        charCounts.TryGetValue(pair[0], out var i);
        charCounts[pair[0]] = i + count;
    }

    charCounts[lastChar] += 1;

    return charCounts;
}

Dictionary<string, long> Apply(Dictionary<string, long> oldCounts, IReadOnlyDictionary<string, string[]> rules,
    ref string lastPair)
{
    var newCounts = new Dictionary<string, long>(oldCounts);
    foreach (var (oldPair, oldCount) in oldCounts)
    {
        rules.TryGetValue(oldPair, out var newPairs);

        if (newPairs is null) continue;

        foreach (var pair in newPairs)
        {
            newCounts.TryGetValue(pair, out var newCount);
            newCounts[pair] = newCount + oldCount;
        }

        newCounts[oldPair] -= oldCount;
        if (newCounts[oldPair] == 0) newCounts.Remove(oldPair);

        if (oldPair == lastPair) lastPair = newPairs[1];
    }

    return newCounts;
}

long ApplyAndCount(string template, IReadOnlyDictionary<string, string[]> rules, int iterations)
{
    var pairCounts = CountPairs(template);

    var lastPair = template[^2..];
    for (var i = 0; i < iterations; i++) pairCounts = Apply(pairCounts, rules, ref lastPair);

    var charCounts = CountChars(pairCounts, lastPair[1]);

    return charCounts.Values.Max() - charCounts.Values.Min();
}

Dictionary<string, string[]> ParseRules(IEnumerable<string> rules)
{
    var dict = new Dictionary<string, string[]>();
    foreach (var line in rules)
    {
        var split = line.Split(" -> ");
        var see = split[0];
        var leftMiddle = string.Join("", see[0], split[1]);
        var middleRight = string.Join("", split[1], see[1]);

        dict[see] = new[] { leftMiddle, middleRight };
    }

    return dict;
}

var input = File.ReadAllLines("input");

var template = input[0];

var part1Rules = input.Skip(2)
    .Select(line => line.Split(" -> "))
    .ToDictionary(split => split[0], split => split[1]);
Console.WriteLine($"Part 1: {NaiveApplyAndCount(template, part1Rules, 10)}");

var part2Rules = ParseRules(input.Skip(2));
Console.WriteLine($"Part 2: {ApplyAndCount(template, part2Rules, 40)}");