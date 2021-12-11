void Print(int[,] octopuses)
{
    for (var y = 0; y < octopuses.GetLength(1); y++)
    {
        for (var x = 0; x < octopuses.GetLength(0); x++)
        {
            var highlight = octopuses[x, y] == 0;
            if (highlight) Console.BackgroundColor = ConsoleColor.White;

            Console.Write(octopuses[x, y]);

            if (highlight) Console.ResetColor();
        }

        Console.WriteLine();
    }
}

int Step(ref int[,] octopuses)
{
    var flashing = new Stack<Coord>();
    var flashed = new HashSet<Coord>();

    var maxX = octopuses.GetLength(0);
    var maxY = octopuses.GetLength(1);

    // Helper functions :)
    bool InBounds(Coord coord)
    {
        var (x, y) = coord;
        return 0 <= x && x < maxX && 0 <= y && y < maxY;
    }

    void AgeOctopus(Coord coord, ref int[,] o, Stack<Coord> stack)
    {
        o[coord.X, coord.Y] += 1;
        if (o[coord.X, coord.Y] > 9) stack.Push(coord);
    }


    // Age all octopuses
    var coords = Enumerable.Range(0, maxX).SelectMany(x => Enumerable.Range(0, maxY).Select(y => new Coord(x, y)));
    foreach (var coord in coords) AgeOctopus(coord, ref octopuses, flashing);

    // Flash until all flashing is done
    while (flashing.Any())
    {
        var o = flashing.Pop();

        // We can instead just not add things to flashing that are already in flashing or flashed,
        // but this is plenty fast already and the code is simpler this way.
        if (flashed.Contains(o)) continue;
        flashed.Add(o);

        var offsets = new[] { 1, 0, -1 };
        var neighbors = offsets
            .SelectMany(x => offsets.Select(y => new Coord(o.X + x, o.Y + y)))
            .Where(InBounds);

        foreach (var neighbor in neighbors) AgeOctopus(neighbor, ref octopuses, flashing);
    }

    // Octopuses that flash get set to 0
    foreach (var (x, y) in flashed) octopuses[x, y] = 0;

    return flashed.Count;
}

int[,] ReadInput(IReadOnlyList<string> lines)
{
    var octopuses = new int[lines[0].Length, lines.Count];

    for (var y = 0; y < lines.Count; y++)
    {
        var line = lines[y];
        for (var x = 0; x < line.Length; x++) octopuses[x, y] = int.Parse(line[x..(x + 1)]);
    }

    return octopuses;
}

var input = File.ReadAllLines("input");


var part1 = ReadInput(input);
var totalFlashes = 0;
for (var step = 0; step < 100; step++) totalFlashes += Step(ref part1);

Console.WriteLine($"Part 1: {totalFlashes}");

var part2 = ReadInput(input);
var stepCount = 0;
while (true)
{
    stepCount += 1;
    var flashes = Step(ref part2);
    if (flashes == part2.Length) break;
}

Console.WriteLine($"Part 2: {stepCount}");

internal record Coord(int X, int Y);