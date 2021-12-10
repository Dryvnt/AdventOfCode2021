using System.Collections.Immutable;

var input = File.ReadAllLines("input");

var part1 = 0;
var part2 = 0;

foreach (var line in input)
{
    var data = line.Split(" | ");
    var segments = data.First().Split(' ').Select(s => s.ToImmutableSortedSet()).ToList();
    var digits = data.Last().Split(' ').Select(s => s.ToImmutableSortedSet()).ToList();

    var one = segments.Single(s => s.Count == 2);
    var four = segments.Single(s => s.Count == 4);
    var seven = segments.Single(s => s.Count == 3);
    var eight = segments.Single(s => s.Count == 7);

    var part1Filter = new[] { one, four, seven, eight };
    part1 += digits.Count(v => part1Filter.Any(s => s.SetEquals(v)));

    var two = segments.Single(s => s.Count == 5 && s.Except(four).Count == 3);
    var three = segments.Single(s => s.Count == 5 && one.IsSubsetOf(s));
    var five = segments.Single(s => s.Count == 5 && !s.SetEquals(three) && !s.SetEquals(two));

    var e = two.Except(three);
    var nine = segments.Single(s => s.Count == 6 && s.Union(e).SetEquals(eight));
    var six = segments.Single(s => s.Count == 6 && s.Intersect(one).Count == 1);
    var zero = segments.Single(s => s.Count == 6 && !s.SetEquals(nine) && !s.SetEquals(six));


    var segmentValues = new Dictionary<string, int>
    {
        [string.Join("", zero)] = 0,
        [string.Join("", one)] = 1,
        [string.Join("", two)] = 2,
        [string.Join("", three)] = 3,
        [string.Join("", four)] = 4,
        [string.Join("", five)] = 5,
        [string.Join("", six)] = 6,
        [string.Join("", seven)] = 7,
        [string.Join("", eight)] = 8,
        [string.Join("", nine)] = 9,
    };

    var total = 0;
    for (var i = 0; i < digits.Count; i++)
    {
        var power = (int)Math.Pow(10, i);
        var digit = digits[digits.Count - i - 1];
        var value = segmentValues[string.Join("", digit)];
        total += power * value;
    }

    part2 += total;
}

Console.WriteLine($"Part 1: {part1}");
Console.WriteLine($"Part 2: {part2}");