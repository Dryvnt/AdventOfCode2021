using System.Numerics;

int PlayDeterministic(Player player1, Player player2, int goal)
{
    var rolls = 0;
    while (true)
    {
        var r1 = rolls * 3 + 6;
        rolls += 3;

        player1 = player1.Move(r1);
        if (player1.Score >= goal) return rolls * player2.Score;

        var r2 = rolls * 3 + 6;
        rolls += 3;

        player2 = player2.Move(r2);
        if (player2.Score >= goal) return rolls * player1.Score;
    }
}

var input = File.ReadAllLines("input");

var positions = input.Select(l => int.Parse(l.Split(' ').Last())).ToList();
var player1 = new Player(positions[0] - 1);
var player2 = new Player(positions[1] - 1);

Console.WriteLine($"Part 1: {PlayDeterministic(player1, player2, 1000)}");

var memoizer = new MemoizedDirac();
var part2 = memoizer.PlayDirac(player1, player2, 21);

Console.WriteLine($"Part 2: {new[] { part2.Player1Wins, part2.Player2Wins }.Max()}");
Console.WriteLine($"We explored {memoizer.Count} unique universes to find this answer!");
Console.WriteLine(part2);

internal class MemoizedDirac
{
    private readonly Dictionary<(Player, Player, int), DiracResult> _cache = new();

    private readonly RollInformation[] _rolls;

    public MemoizedDirac(int sides = 3)
    {
        _rolls = RollInformation.GenerateTable(sides);
    }

    public int Count => _cache.Count;

    public DiracResult PlayDirac(Player player1, Player player2, int goal)
    {
        if (_cache.TryGetValue((player1, player2, goal), out var cache)) return cache;

        var player1Wins = new BigInteger(0);
        var player2Wins = new BigInteger(0);

        foreach (var (roll, count) in _rolls)
        {
            var afterRoll = player1.Move(roll);
            if (afterRoll.Score >= goal)
            {
                player1Wins += count;
                continue;
            }

            var minScore = Math.Min(afterRoll.Score, player2.Score);

            // Note: players are flipped in sub-game!
            var subGameResult = PlayDirac(player2.SubScore(minScore), afterRoll.SubScore(minScore), goal - minScore);

            player1Wins += subGameResult.Player2Wins * count;
            player2Wins += subGameResult.Player1Wins * count;
        }

        var result = new DiracResult(player1Wins, player2Wins);

        _cache[(player1, player2, goal)] = result;

        return result;
    }
}

internal readonly record struct Player(int Position, int Score = 0)
{
    public Player Move(int roll)
    {
        var newPos = (Position + roll) % 10;
        var newScore = Score + newPos + 1;

        return new Player(newPos, newScore);
    }

    public Player SubScore(int sub)
    {
        return this with
        {
            Score = Score - sub,
        };
    }
}

// 64 bit integers are enough for baseline problem, but BigIntegers are necessary
// if we want to play around with more dice faces and/or higher score goals
internal readonly record struct DiracResult(BigInteger Player1Wins, BigInteger Player2Wins);

internal readonly record struct RollInformation(int Roll, BigInteger Count)
{
    // This is entirely unnecessary. We could (and did!) just hardcode this table for sides=3
    // But I like having it, and it's my code. You can't stop me! >:)
    public static RollInformation[] GenerateTable(int sides)
    {
        var sums = new Dictionary<int, BigInteger>
        {
            [0] = 1L,
        };

        for (var i = 0; i < sides; i++)
        {
            var newSums = new Dictionary<int, BigInteger>();

            foreach (var (oldSum, oldCount) in sums)
            {
                for (var face = 1; face <= sides; face++)
                {
                    var newSum = oldSum + face;
                    newSums.TryGetValue(newSum, out var count);
                    newSums[newSum] = oldCount + count;
                }
            }

            sums = newSums;
        }

        return sums.Select(kvp => new RollInformation(kvp.Key, kvp.Value)).ToArray();
    }
}