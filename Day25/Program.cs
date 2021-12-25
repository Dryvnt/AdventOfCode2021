using System.Diagnostics;
using Day25;

var input = File.ReadAllLines("input");

var watch = new Stopwatch();

watch.Start();
var steps = 1;
var s = new Seabed(input);
while (s.Step() != 0) steps += 1;
watch.Stop();

Console.WriteLine($"Part 1: {steps}");
Console.WriteLine($"\tTook {watch.Elapsed}");