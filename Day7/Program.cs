using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var input = File.ReadAllText("input");
var positions = input.Split(',').Select(int.Parse).ToList();

var (part1Target, part1Cost) = CalculateCost(positions, i => i);
Console.WriteLine($"Part one: {new { Target = part1Target, Cost = part1Cost }}");

// Cost function: f(0) = 0, f(x) = x + f(x - 1)
// https://www.wolframalpha.com/input/?i=f%280%29+%3D+0%2C+f%28x%29+%3D+x+%2B+f%28x-1%29
// f(x) = 1/2 x * (x + 1)
var (part2Target, part2Cost) = CalculateCost(positions, i => 0.5 * i * (i + 1));
Console.WriteLine($"Part two: {new { Target = part2Target, Cost = part2Cost }}");

static (int, double) CalculateCost(ICollection<int> positions, Func<int, double> costFunction)
{
    var minCost = double.PositiveInfinity;
    var minTarget = 0;

    // We can do some smart binary search stuff.
    // Analyze two middle inputs, discard half the list in the direction the cost is rising
    // Repeat until one element left in list.
    // But there the input is so small, a simple linear search is more than fast enough.

    foreach (var target in Enumerable.Range(positions.Min(), positions.Max()))
    {
        var distances = positions.Select(p => Math.Abs(p - target));
        var cost = distances.Select(costFunction).Sum();

        if (cost >= minCost) continue;

        minCost = cost;
        minTarget = target;
    }

    return (minTarget, minCost);
}