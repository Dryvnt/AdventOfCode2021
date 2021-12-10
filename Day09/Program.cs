// See https://aka.ms/new-console-template for more information

void PrintArray(int[,] array, bool[,]? markMap)
{
    for (var y = 0; y < array.GetLength(1); y++)
    {
        for (var x = 0; x < array.GetLength(0); x++)
        {
            if (markMap is not null && markMap[x, y])
                Console.BackgroundColor = ConsoleColor.White;
            Console.Write(array[x, y]);
            Console.ResetColor();
        }

        Console.WriteLine();
    }
}

var input = File.ReadAllLines("input");
var yMax = input.Length;
var xMax = input.Select(s => s.Length).Max();


var heightMap = new int[xMax, yMax];

for (var y = 0; y < yMax; y++)
{
    var line = input[y];
    for (var x = 0; x < xMax; x++) heightMap[x, y] = int.Parse(line.Substring(x, 1));
}

var coordinates = new List<(int X, int Y)>();
for (var x = 0; x < xMax; x++)
for (var y = 0; y < yMax; y++)
    coordinates.Add((x, y));


bool InBounds(int x, int y)
{
    return x >= 0 && x < xMax && y >= 0 && y < yMax;
}

IEnumerable<(int X, int Y)> GetNeighbors(int x, int y)
{
    var neighbors = new (int X, int Y)[]
    {
        (x - 1, y),
        (x + 1, y),
        (x, y - 1),
        (x, y + 1),
    };

    return neighbors.Where(xy => InBounds(xy.X, xy.Y));
}

var lowPointMap = new bool[xMax, yMax];
foreach (var (x, y) in coordinates)
{
    var height = heightMap[x, y];
    var neighbors = GetNeighbors(x, y);
    if (neighbors.All(n => heightMap[n.X, n.Y] > height))
        lowPointMap[x, y] = true;
}

var risk = 0;
foreach (var (x, y) in coordinates)
{
    if (lowPointMap[x, y])
        risk += heightMap[x, y] + 1;
}

Console.WriteLine($"Part 1: {risk}");

var explored = new bool[xMax, yMax];
foreach (var (x, y) in coordinates)
{
    if (heightMap[x, y] == 9)
        explored[x, y] = true;
}

var basins = new List<bool[,]>();
foreach (var (i, j) in coordinates)
{
    if (explored[i, j]) continue;
    // We haven't explored here yet: New basin!
    var basin = new bool[xMax, yMax];
    var stack = new Stack<(int X, int Y)>();
    stack.Push((i, j));

    while (stack.Any())
    {
        var (x, y) = stack.Pop();
        basin[x, y] = true;
        explored[x, y] = true;
        foreach (var neighbor in GetNeighbors(x, y).Where(xy => !explored[xy.X, xy.Y])) stack.Push(neighbor);
    }

    basins.Add(basin);
}

var basinSizes = basins
    .Select(b => (Basin: b, Size: coordinates.Count(xy => b[xy.X, xy.Y])))
    .OrderBy(tuple => -tuple.Size)
    .ToList();

var part2 = basinSizes.Take(3).Aggregate(1, (i, tuple) => i * tuple.Size);

Console.WriteLine($"Part 2: {part2}");