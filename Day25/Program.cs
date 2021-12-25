using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Text;

var input = File.ReadAllLines("input");

var s = new Seabed(input);

var steps = 1;

while (s.Step() != 0)
{
    steps += 1;
}

Console.WriteLine($"Part 1: {steps}");

internal class Seabed
{
    private int _maxX = 0;
    private int _maxY = 0;
    private Dictionary<Vector2, char> _map = new();

    private record struct Vector2(int X, int Y);

    public int StepEast()
    {
        var newMap = new Dictionary<Vector2, char>();

        var moved = 0;
        
        foreach (var (pos, c) in _map.Where(kvp => kvp.Value is not '>'))
            newMap[pos] = c;
        foreach (var (pos, c) in _map.Where(kvp => kvp.Value is '>'))
        {
            var moveTo = new Vector2(pos.X + 1, pos.Y);
            if (moveTo.X > _maxX)
            {
                moveTo = moveTo with { X = 0 };
            }

            if (_map.ContainsKey(moveTo))
            {
                newMap[pos] = c;
            }
            else
            {
                newMap[moveTo] = c;
                moved += 1;
            }
        }
        
        Debug.Assert(newMap.Count == _map.Count);
        _map = newMap;

        return moved;
    }
    
    public int StepSouth()
    {
        var newMap = new Dictionary<Vector2, char>();

        var moved = 0;

        foreach (var (pos, c) in _map.Where(kvp => kvp.Value is not 'v'))
            newMap[pos] = c;
        foreach (var (pos, c) in _map.Where(kvp => kvp.Value is 'v'))
        {
            var moveTo = new Vector2(pos.X, pos.Y + 1);
            if (moveTo.Y > _maxY)
            {
                moveTo = moveTo with { Y = 0 };
            }

            if (_map.ContainsKey(moveTo))
            {
                newMap[pos] = c;
            }
            else
            {
                newMap[moveTo] = c;
                moved += 1;
            }
        }
        
        Debug.Assert(newMap.Count == _map.Count);
        _map = newMap;

        return moved;
    }

    public int Step()
    {
        var e = StepEast();
        var s = StepSouth();
        return e + s;

    }
    
    public Seabed(IReadOnlyList<string> input)
    {
        for (var y = 0; y < input.Count; y++)
        {
            if (y > _maxY) _maxY = y;
            
            var line = input[y];
            for(var x = 0; x < line.Length; x++)
            {
                if (x > _maxX) _maxX = x;
                
                var c = line[x];
                if (c is '.') continue;
                
                Debug.Assert(c is '>' or 'v');
                _map[new Vector2(x, y)] = c;
            }
        }
    }

    public override string ToString()
    {
        var s = new StringBuilder();
        for (var y = 0; y <= _maxY; y++)
        {
            for (var x = 0; x <= _maxX; x++)
            {
                s.Append(_map.TryGetValue(new Vector2(x, y), out var c) ? c : '.');
            }

            s.AppendLine();
        }

        return s.ToString();
    }
}
