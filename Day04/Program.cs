using System.Diagnostics;

var lines = File.ReadAllLines("input");

var numbers = lines.First().Split(",").Select(int.Parse).ToList();

var boards = new List<Board>();

var magic = string.Join(" ", lines.Skip(1).Select(l => l.Any() ? l : "\n")).Split("\n")
    .Select(line => line.Split(" ").Where(s => s.Any()).Select(int.Parse)).Select(nums => new Board(nums));

boards.AddRange(magic);

Board? firstBingo = null;
Board? lastBingo = null;
var magicTwo = numbers
    .SelectMany(_ => boards.Where(b => !b.Bingo()), (n, b) => new { n, b })
    .Where(p => p.b.Mark(p.n))
    .Select(p => p.b);

foreach (var b in magicTwo)
{
    firstBingo ??= b;
    lastBingo = b;
}

Debug.Assert(firstBingo is not null);
Debug.Assert(lastBingo is not null);

firstBingo.Print();
lastBingo.Print();

internal class Board
{
    private readonly bool[] _marked = new bool[5 * 5];
    private readonly List<int> _numbers;
    private int _bingoNum;

    private bool _bingoSeen;

    public Board(IEnumerable<int> nums)
    {
        _numbers = nums.ToList();
    }

    private int Score => _numbers.Where((_, i) => !_marked[i]).Sum() * _bingoNum;

    public bool Mark(int num)
    {
        if (_bingoSeen) return _bingoSeen;

        for (var i = 0; i < _numbers.Count; i++)
        {
            if (_numbers[i] == num)
                _marked[i] = true;
        }

        if (!Bingo()) return false;

        _bingoSeen = true;
        _bingoNum = num;
        return true;
    }

    public bool Bingo()
    {
        // Shortcut
        if (_bingoSeen) return _bingoSeen;

        // Check rows
        for (var row = 0; row < 5; row++)
        {
            if (Enumerable.Range(0, 5).Select(i => _marked[row * 5 + i]).All(b => b))
                return true;
        }

        // Check columns
        for (var col = 0; col < 5; col++)
        {
            if (Enumerable.Range(0, 5).Select(i => _marked[i * 5 + col]).All(b => b))
                return true;
        }

        return false;
    }

    public void Print()
    {
        if (_bingoSeen) Console.WriteLine($"Score: {Score}, Winning Number: {_bingoNum}");
        for (var y = 0; y < 5; y++)
        {
            for (var x = 0; x < 5; x++)
            {
                var i = y * 5 + x;
                var n = _numbers[y * 5 + x];
                var s = n.ToString();

                if (_marked[i]) Console.BackgroundColor = ConsoleColor.White;
                Console.Write(s.PadLeft(3, ' '));
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }
}