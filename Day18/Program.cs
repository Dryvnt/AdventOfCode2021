using Day18;

var input = File.ReadAllLines("input");

var numbers = input.Select(l => new Number(l)).ToHashSet();

var sum = numbers.Skip(1).Aggregate(numbers.First(), (current, n) => current + n);

Console.WriteLine($"Part 1: {sum.Magnitude()}");

// O(n^2) oh well
var max = numbers.Aggregate(0L,
    (current, a) => numbers
        .Except(new[] { a })
        .Select(b => (a + b).Magnitude())
        .Prepend(current).Max());

Console.WriteLine($"Part 2: {max}");