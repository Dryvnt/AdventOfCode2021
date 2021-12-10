var input = File.ReadAllText("input");

var data = input.Split(",").Select(int.Parse).ToList();
var ages = data.GroupBy(d => d).Select(g => new { Age = g.Key, Count = g.Count() }).ToList();

var pop = new Population();
foreach (var d in ages) pop.Set(d.Age, d.Count);

pop.Tick(80);
Console.WriteLine($"Part 1: {pop.Count}");
pop.Tick(256 - 80);
Console.WriteLine($"Part 2: {pop.Count}");

internal class Population
{
    private readonly List<long> _fish = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    public long Count => _fish.Sum();

    public void Set(int age, int amount)
    {
        _fish[age] = amount;
    }

    private void _tick()
    {
        // How many fish are done aging
        var newFish = _fish[0];

        // Age everything that isn't done aging
        for (var i = 1; i < 9; i++) _fish[i - 1] = _fish[i];

        // Fish that gave birth get age 6
        _fish[6] += newFish;

        // New fish are age 8
        _fish[8] = newFish;
    }

    public void Tick(int n)
    {
        for (var i = 0; i < n; i++) _tick();
    }
}