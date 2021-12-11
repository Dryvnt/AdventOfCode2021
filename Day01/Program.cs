var lines = File.ReadAllLines("input");

var last = int.MaxValue;
var nLarger = 0;

foreach (var line in lines)
{
    var current = int.Parse(line);
    if (current > last) nLarger += 1;

    last = current;
}

Console.WriteLine($"Part 1: {nLarger}");

var windows = lines.Zip(lines.Skip(1), lines.Skip(2));

last = int.MaxValue;
nLarger = 0;
foreach (var window in windows)
{
    var a = int.Parse(window.First);
    var b = int.Parse(window.Second);
    var c = int.Parse(window.Third);
    var current = a + b + c;
    if (current > last) nLarger += 1;
    last = current;
}

Console.WriteLine($"Part 2: {nLarger}");