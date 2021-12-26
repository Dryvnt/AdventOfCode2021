using System.Diagnostics;
using Day24;

// I solved this day with math (See notes.txt),
// but I wanted to write code anyway, so I ported someone else's solution.
// https://www.reddit.com/r/adventofcode/comments/rnejv5/2021_day_24_solutions/hpsq6zq/
// Note: this solution is very inefficient, takes a very long time, and uses a lot of memory.
// Orders of magnitude slower runtime and memory usage vs. original implementation in Rust
// This is clearly a solution NOT intended for C#, but it's easy to implement, so :)
(long, long) BruteForceStateSpace(IEnumerable<Instruction> instructions)
{
    var watch = new Stopwatch();
    watch.Start();

    var states = new List<SearchEntry>
    {
        new(new Alu(), new MinMax(0, 0)),
    };

    var digit = 0;

    foreach (var instruction in instructions)
    {
        if (instruction is Inp)
        {
            Console.WriteLine($"{watch.Elapsed}\tBranching");
            var newStates = new Dictionary<Alu, MinMax>();

            foreach (var (alu, (min, max)) in states)
            {
                for (var w = 1; w <= 9; w++)
                {
                    var newState = alu.StepInp(instruction, w);
                    var newMin = min * 10 + w;
                    var newMax = max * 10 + w;

                    if (newStates.TryGetValue(newState, out var e))
                        newStates[newState] = new MinMax(Math.Min(newMin, e.Min), Math.Max(newMax, e.Max));
                    else
                        newStates[newState] = new MinMax(newMin, newMax);
                }
            }

            states = newStates.AsParallel().Select(kvp => new SearchEntry(kvp.Key, kvp.Value)).ToList();

            digit++;
            Console.WriteLine(
                $"{watch.Elapsed} Digit {digit.ToString().PadRight(2, ' ')} - Processing {states.Count} states");
        }
        else
        {
            for (var i = 0; i < states.Count; i++)
            {
                var e = states[i];
                states[i] = e with { Alu = e.Alu.Step(instruction) };
            }
        }
    }

    var minInput = states.Where(e => e.Alu.Z == 0).Min(e => e.MinMax.Min);
    var maxInput = states.Where(e => e.Alu.Z == 0).Max(e => e.MinMax.Max);

    return (minInput, maxInput);
}

var input = File.ReadAllLines("input");
var instructions = input.Select(Instruction.Parse).ToList();

var (part2, part1) = BruteForceStateSpace(instructions);
Console.WriteLine($"Part 1: {part1}");
Console.WriteLine($"Part 2: {part2}");

internal record struct MinMax(long Min, long Max);

internal record struct SearchEntry(Alu Alu, MinMax MinMax);