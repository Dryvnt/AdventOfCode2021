using System.Text.RegularExpressions;

int SimulateMaxY(int vel, int steps)
{
    var y = 0;
    var max = y;
    for (var i = 0; i < steps; i++)
    {
        y += vel;
        vel -= 1;
        if (y > max) max = y;
    }

    return max;
}

IEnumerable<Feasible> FindFeasibleX(TargetArea target)
{
    for (var initialVel = 1; initialVel <= target.XMax; initialVel++)
    {
        var vel = initialVel;
        var x = 0;
        var steps = 0;
        var hitStart = 0;
        var standstill = false;
        var window = 0;
        while (vel != 0)
        {
            steps++;

            x += vel;
            vel -= Math.Sign(vel);

            if (x < target.XMin || x > target.XMax) continue;

            if (vel == 0) standstill = true;
            if (hitStart == 0) hitStart = steps;
            else window += 1;
        }

        if (hitStart != 0)
            yield return new Feasible(initialVel, hitStart, standstill ? int.MaxValue : hitStart + window);
    }
}

IEnumerable<Feasible> FindFeasibleY(TargetArea target)
{
    for (var initialVel = target.YMin; initialVel <= 1000; initialVel++)
    {
        var vel = initialVel;
        var y = 0;
        var steps = 0;
        var hitStart = 0;
        var hitStop = 0;
        while (!(vel < 0 && y < target.YMin))
        {
            steps++;

            y += vel;
            vel -= 1;


            if (y < target.YMin || y > target.YMax) continue;

            if (hitStart == 0) hitStart = steps;
            hitStop = steps;
        }

        if (hitStart != 0) yield return new Feasible(initialVel, hitStart, hitStop);
    }
}

var input = File.ReadAllLines("input");
var target = TargetArea.Parse(input.First());

var feasibleX = FindFeasibleX(target).ToList();
var feasibleY = FindFeasibleY(target).ToList();

var highestY = feasibleY.MaxBy(f => f.Vel);
var someX = feasibleX.First(f => f.HitStart <= highestY.HitStart && f.HitEnd <= highestY.HitEnd);
Console.WriteLine($"Part 1: {SimulateMaxY(highestY.Vel, highestY.HitStart)} ({someX.Vel}, {highestY.Vel})");

var configurations = new HashSet<Configuration>();
foreach (var (velY, yStart, yEnd) in feasibleY)
{
    foreach (var (velX, xStart, xEnd) in feasibleX)
    {
        if (yEnd < xStart || xEnd < yStart) continue;

        configurations.Add(new Configuration(velX, velY));
    }
}

Console.WriteLine($"Part 2: {configurations.Count}");

internal readonly record struct TargetArea(int XMin, int XMax, int YMin, int YMax)
{
    public static TargetArea Parse(string input)
    {
        var match = Regex.Match(input, @"x=(-?[0-9]+)..(-?[0-9]+), y=(-?[0-9]+)..(-?[0-9]+)");
        var targetValues = match.Groups.Values.Skip(1).Select(g => int.Parse(g.Value)).ToList();

        return new TargetArea(targetValues[0], targetValues[1], targetValues[2], targetValues[3]);
    }
}

internal readonly record struct Feasible(int Vel, int HitStart, int HitEnd);

internal readonly record struct Configuration(int VelX, int VelY);