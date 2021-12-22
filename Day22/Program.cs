using Day22;

var input = File.ReadAllLines("input");

var instructions = input.Select(Instruction.Parse).ToList();

var part1Reactor = new SmartReactor();
foreach (var i in instructions.Where(i =>
             i.Cuboid.X.Min >= -50 && i.Cuboid.X.Max <= 51 &&
             i.Cuboid.Y.Min >= -50 && i.Cuboid.Y.Max <= 51 &&
             i.Cuboid.Z.Min >= -50 && i.Cuboid.Z.Max <= 51)
        ) part1Reactor.Mark(i);

Console.WriteLine($"Part 1: {part1Reactor.CountOn()}");

var part2Reactor = new SmartReactor();
foreach (var i in instructions) part2Reactor.Mark(i);

Console.WriteLine($"Part 2: {part2Reactor.CountOn()}");