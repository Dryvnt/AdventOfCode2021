void Print(int[,] map, ICollection<Coord>? solution = null)
{
    for (var y = 0; y < map.GetLength(1); y++)
    {
        for (var x = 0; x < map.GetLength(0); x++)
        {
            var color = solution?.Contains(new Coord(x, y)) ?? false;
            if (color) Console.BackgroundColor = ConsoleColor.White;
            Console.Write(map[x, y]);
            if (color) Console.ResetColor();
        }

        Console.WriteLine();
    }
}

int[,] BuildMap(IReadOnlyCollection<string> input)
{
    var ints = new int[input.First().Length, input.Count];

    foreach (var (line, y) in input.Select((line, i) => (line, i)))
    foreach (var (c, x) in line.Select((c, i) => (c, i)))
        ints[x, y] = int.Parse(c.ToString());

    return ints;
}

int[,] BuildLargerMap(int[,] smallMap, int factor = 5)
{
    var largeMap = new int[smallMap.GetLength(0) * factor, smallMap.GetLength(1) * factor];
    for (var x = 0; x < largeMap.GetLength(0); x++)
    {
        for (var y = 0; y < largeMap.GetLength(1); y++)
        {
            var i = x / smallMap.GetLength(0);
            var j = y / smallMap.GetLength(1);
            var smallX = x - i * smallMap.GetLength(0);
            var smallY = y - j * smallMap.GetLength(1);

            var newNum = smallMap[smallX, smallY] + i + j;
            while (newNum > 9) newNum -= 9;

            largeMap[x, y] = newNum;
        }
    }

    return largeMap;
}

IEnumerable<Coord> Neighbors(int[,] map, Coord coord)
{
    var neighbors =
        new (int X, int Y)[] { (-1, 0), (1, 0), (0, -1), (0, 1) }.Select(o => new Coord(coord.X + o.X, coord.Y + o.Y));

    foreach (var n in neighbors)
    {
        if (n.X < 0 || n.X >= map.GetLength(0)) continue;
        if (n.Y < 0 || n.Y >= map.GetLength(1)) continue;
        yield return n;
    }
}

IEnumerable<Coord> Dijkstra(int[,] map, Coord start, Coord goal)
{
    // Follows pseudocode almost to the letter, except we use a seen set instead of changing priority of elements
    // https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm#Using_a_priority_queue

    var dist = new Dictionary<Coord, int>();
    var prev = new Dictionary<Coord, Coord>();

    var q = new PriorityQueue<Coord, int>();

    // Length of the path that visits every element, plus 1
    var pseudoInfinity = map.Cast<int>().Sum() + 1;

    dist[start] = 0;

    for (var y = 0; y < map.GetLength(1); y++)
    {
        for (var x = 0; x < map.GetLength(0); x++)
        {
            var v = new Coord(x, y);
            if (v != start) dist[v] = pseudoInfinity;

            q.Enqueue(v, dist[v]);
        }
    }

    var seen = new HashSet<Coord>();
    while (q.Count > 0)
    {
        var u = q.Dequeue();

        if (seen.Contains(u)) continue;
        seen.Add(u);

        foreach (var v in Neighbors(map, u))
        {
            var alt = dist[u] + map[u.X, u.Y];

            if (alt >= dist[v]) continue;

            dist[v] = alt;
            prev[v] = u;
            q.Enqueue(v, alt);
        }
    }

    var cursor = goal;
    yield return cursor;
    while (cursor != start)
    {
        cursor = prev[cursor];
        yield return cursor;
    }
}


int CalculateTotalCost(IEnumerable<Coord> solution, int[,] map)
{
    return solution.Select(c => map[c.X, c.Y]).Sum() - map[0, 0];
}


var input = File.ReadAllLines("input");

var map = BuildMap(input);

var start = new Coord(0, 0);
var goal = new Coord(map.GetLength(0) - 1, map.GetLength(1) - 1);

var solution = Dijkstra(map, start, goal);

Console.WriteLine($"Part 1: {CalculateTotalCost(solution, map)}");

var largeMap = BuildLargerMap(map);

var largeGoal = new Coord(largeMap.GetLength(0) - 1, largeMap.GetLength(1) - 1);
var largeSolution = Dijkstra(largeMap, start, largeGoal);

Console.WriteLine($"Part 2: {CalculateTotalCost(largeSolution, largeMap)}");

// Helper alternative to tuples
internal record struct Coord(int X, int Y);