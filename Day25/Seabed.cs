using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Day25;

internal class Seabed
{
    private char[,] _map;

    public Seabed(IReadOnlyList<string> input)
    {
        _map = new char[input[0].Length, input.Count];

        for (var y = 0; y < _map.GetLength(1); y++)
        {
            var line = input[y];
            for (var x = 0; x < _map.GetLength(0); x++)
            {
                var c = line[x];

                Debug.Assert(c is '>' or 'v' or '.');
                _map[x, y] = c;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Vector2 Index(int x, int y)
    {
        if (x < 0) x += _map.GetLength(0);
        if (y < 0) y += _map.GetLength(1);

        return new Vector2(x % _map.GetLength(0), y % _map.GetLength(1));
    }

    private int StepEast()
    {
        var moved = 0;

        var buffer = new char[_map.GetLength(0), _map.GetLength(1)];
        Array.Copy(_map, 0, buffer, 0, _map.Length);

        for (var x = 0; x < _map.GetLength(0); x++)
        for (var y = 0; y < _map.GetLength(1); y++)
        {
            var c = _map[x, y];
            if (c != '>') continue;

            var (newX, newY) = Index(x + 1, y);
            var nextC = _map[newX, newY];
            if (nextC != '.') continue;

            moved++;
            buffer[x, y] = '.';
            buffer[newX, newY] = '>';
        }

        _map = buffer;

        return moved;
    }

    private int StepSouth()
    {
        var moved = 0;

        var buffer = new char[_map.GetLength(0), _map.GetLength(1)];
        Array.Copy(_map, 0, buffer, 0, _map.Length);

        for (var x = 0; x < _map.GetLength(0); x++)
        for (var y = 0; y < _map.GetLength(1); y++)
        {
            var c = _map[x, y];
            if (c != 'v') continue;

            var (newX, newY) = Index(x, y + 1);
            var nextC = _map[newX, newY];
            if (nextC != '.') continue;

            moved++;
            buffer[x, y] = '.';
            buffer[newX, newY] = 'v';
        }

        _map = buffer;

        return moved;
    }

    public int Step()
    {
        var e = StepEast();
        var s = StepSouth();

        return e + s;
    }

    public override string ToString()
    {
        var s = new StringBuilder();
        for (var y = 0; y < _map.GetLength(1); y++)
        {
            for (var x = 0; x < _map.GetLength(0); x++) s.Append(_map[x, y]);

            s.AppendLine();
        }

        return s.ToString();
    }

    private record struct Vector2(int X, int Y);
}