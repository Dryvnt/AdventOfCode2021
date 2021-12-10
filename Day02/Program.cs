// See https://aka.ms/new-console-template for more information

var lines = await File.ReadAllLinesAsync("input");

var depth = 0;
var horizontal = 0;
foreach (var line in lines)
{
    var words = line.Split(" ");
    var direction = words[0];
    var distance = int.Parse(words[1]);
    switch (words[0])
    {
        case "forward":
            horizontal += distance;
            break;
        case "down":
            depth += distance;
            break;
        case "up":
            depth -= distance;
            break;
    }
}

Console.WriteLine($"Part one - Depth: {depth}, Horizontal: {horizontal}, Answer: {depth * horizontal}");

depth = 0;
horizontal = 0;
var aim = 0;
foreach (var line in lines)
{
    var words = line.Split(" ");
    var direction = words[0];
    var distance = int.Parse(words[1]);
    switch (words[0])
    {
        case "forward":
            horizontal += distance;
            depth += aim * distance;
            break;
        case "down":
            aim += distance;
            break;
        case "up":
            aim -= distance;
            break;
    }
}

Console.WriteLine($"Part Two - Depth: {depth}, Horizontal: {horizontal}, Aim: {aim}, Answer: {depth * horizontal}");