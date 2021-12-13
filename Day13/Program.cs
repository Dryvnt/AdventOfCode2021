void Print(bool[,] grid, Fold? f = null)
{
    for (var y = 0; y < grid.GetLength(1); y++)
    {
        if (f is not null && f.Axis == Axis.Y && f.Pos == y)
            Console.WriteLine("".PadLeft(grid.GetLength(0), '-'));
        else
        {
            for (var x = 0; x < grid.GetLength(0); x++)
            {
                var c = grid[x, y] ? '#' : '.';
                if (f is not null && f.Axis == Axis.X && f.Pos == x) c = '-';
                Console.Write(c);
            }

            Console.WriteLine();
        }
    }
}

bool[,] Transpose(bool[,] grid)
{
    var t = new bool[grid.GetLength(1), grid.GetLength(0)];
    for (var x = 0; x < grid.GetLength(0); x++)
    {
        for (var y = 0; y < grid.GetLength(1); y++) t[y, x] = grid[x, y];
    }

    return t;
}

bool[,] DoFold(bool[,] grid, Fold f)
{
    // Always ensure we're folding across the vertical axis
    if (f.Axis is Axis.X) grid = Transpose(grid);

    var folded = new bool[grid.GetLength(0), f.Pos];

    for (var y = 0; y < folded.GetLength(1); y++)
    {
        for (var x = 0; x < folded.GetLength(0); x++)
        {
            folded[x, y] |= grid[x, y];
            folded[x, y] |= grid[x, grid.GetLength(1) - y - 1];
        }
    }

    // If we rotated around vertical, rotate back to horizontal
    if (f.Axis is Axis.X) folded = Transpose(folded);

    return folded;
}

var input = File.ReadAllLines("input");

var coords = new List<Coord>();
var folds = new List<Fold>();

foreach (var line in input)
{
    if (line.Contains(','))
    {
        var i = line.Split(',').Select(int.Parse).ToArray();
        coords.Add(new Coord(i[0], i[1]));
    }

    if (line.StartsWith("fold along "))
    {
        var l = line[11..];
        var a = l[0] == 'x' ? Axis.X : Axis.Y;
        var i = int.Parse(l[2..]);
        folds.Add(new Fold(a, i));
    }
}

var maxX = coords.Select(c => c.X).Max();
var maxY = coords.Select(c => c.Y).Max();

var grid = new bool[maxX + 1, maxY + 1];
foreach (var (x, y) in coords) grid[x, y] = true;

var count = DoFold(grid, folds.First()).Cast<bool>().Count(b => b);

Console.WriteLine($"Part 1: {count}");

grid = folds.Aggregate(grid, DoFold);

Console.WriteLine("Part 2:");
Print(grid);

internal record struct Coord(int X, int Y);

internal record Fold(Axis Axis, int Pos);

internal enum Axis
{
    X = 0,
    Y = 1,
}