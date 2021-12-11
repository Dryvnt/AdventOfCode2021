using System.Diagnostics;

var inputLines = File.ReadAllLines("input");

var lines = new List<Line>();

foreach (var input in inputLines)
{
    var split = input.Split(" -> ");
    var fromData = split.First().Split(",").Select(int.Parse).ToList();
    var toData = split.Last().Split(",").Select(int.Parse).ToList();

    var from = new Coordinate(fromData.First(), fromData.Last());
    var to = new Coordinate(toData.First(), toData.Last());

    lines.Add(new Line(from, to));
}

var maxXy = lines.SelectMany(l => new[] { l.From.X, l.To.X, l.From.Y, l.To.Y }).Max();

// Part 1
var straightLines = lines.Where(l => l.From.X == l.To.X || l.From.Y == l.To.Y).ToList();
var grid1 = new Grid(maxXy + 1);
foreach (var line in straightLines) grid1.Mark(line);
//grid1.Print();
Console.WriteLine($"Part 1: {grid1.NumOverlaps}");

// Part 2
var grid2 = new Grid(maxXy + 1);
foreach (var line in lines) grid2.Mark(line);
//grid2.Print();
Console.WriteLine($"Part 2: {grid2.NumOverlaps}");

internal record Coordinate(int X, int Y);

internal record Line(Coordinate From, Coordinate To);

internal class Grid
{
    private readonly List<List<int>> _grid = new();
    private readonly int _size;

    public Grid(int size)
    {
        _size = size;
        for (var i = 0; i < _size; i++)
        {
            _grid.Add(new List<int>(size));
            for (var j = 0; j < _size; j++) _grid[i].Add(0);
        }
    }

    public int NumOverlaps => _grid.SelectMany(l => l).Count(i => i > 1);

    public void Mark(Line line)
    {
        var cursor = line.From;
        var target = line.To;
        var done = false;
        while (!done)
        {
            _grid[cursor.X][cursor.Y] += 1;

            var touched = false;
            if (cursor.X != target.X)
            {
                touched = true;
                var inc = cursor.X < target.X ? 1 : -1;
                cursor = cursor with { X = cursor.X + inc };
            }

            if (cursor.Y != target.Y)
            {
                touched = true;
                var inc = cursor.Y < target.Y ? 1 : -1;
                cursor = cursor with { Y = cursor.Y + inc };
            }

            if (touched) continue;

            Debug.Assert(cursor == target);
            done = true;
        }
    }

    public void Print()
    {
        for (var y = 0; y < _size; y++)
        {
            for (var x = 0; x < _size; x++) Console.Write(_grid[x][y]);
            Console.WriteLine();
        }
    }
}