using System.Diagnostics;
using System.Text;

var input = File.ReadAllLines("input");

var table = input.First().Trim().Select(c => c switch
{
    '#' => true,
    '.' => false,
    _ => throw new ArgumentOutOfRangeException(nameof(c), c, null),
}).ToArray();

Debug.Assert(table.Length == 512);

var image = Image.Parse(table, input[2..]);

Console.WriteLine($"Part 1: {image.Enhance(table).Enhance(table).CountOnes()}");

var enhanced = Enumerable.Range(0, 50).Aggregate(image, (i, _) => i.Enhance(table));

Console.WriteLine($"Part 2: {enhanced.CountOnes()}");

internal readonly record struct Image(bool[,] Inner, bool Outside)
{
    private bool Index(int x, int y)
    {
        if (y < 0 || y >= Inner.GetLength(1) || x < 0 || x >= Inner.GetLength(0)) return Outside;
        return Inner[x, y];
    }

    private ushort GetValue(int atX, int atY)
    {
        var value = 0;
        for (var y = atY - 1; y <= atY + 1; y++)
        {
            for (var x = atX - 1; x <= atX + 1; x++)
            {
                var c = Index(x, y);

                value |= c ? 1 : 0;
                value <<= 1;
            }
        }

        value >>= 1;

        return (ushort)value;
    }

    public long CountOnes()
    {
        if (Outside) return long.MaxValue;

        return Inner.Cast<bool>().Select(b => b ? 1L : 0L).Sum();
    }

    public Image Trim()
    {
        var leftMargin = -1;
        var rightMargin = 0;
        for (var x = 0; x < Inner.GetLength(0); x++)
        {
            for (var y = 0; y < Inner.GetLength(1); y++)
            {
                if (Inner[x, y] == Outside) continue;

                if (leftMargin == -1) leftMargin = x;
                rightMargin = x + 1;
            }
        }


        var topMargin = -1;
        var bottomMargin = 0;
        for (var y = 0; y < Inner.GetLength(1); y++)
        {
            for (var x = 0; x < Inner.GetLength(0); x++)
            {
                if (Inner[x, y] == Outside) continue;

                if (topMargin == -1) topMargin = y;
                bottomMargin = y + 1;
            }
        }

        var newWidth = rightMargin - leftMargin;
        var newHeight = bottomMargin - topMargin;
        var newInner = new bool[newWidth, newHeight];

        for (var x = 0; x < newInner.GetLength(0); x++)
        {
            for (var y = 0; y < newInner.GetLength(1); y++) newInner[x, y] = Inner[x + leftMargin, y + topMargin];
        }

        return new Image(newInner, Outside);
    }

    public Image Enhance(bool[] table)
    {
        var newWidth = Inner.GetLength(0) + 2;
        var newHeight = Inner.GetLength(1) + 2;
        var newImage = new bool[newWidth, newHeight];

        for (var y = 0; y < newHeight; y++)
        for (var x = 0; x < newWidth; x++)
            newImage[x, y] = table[GetValue(x - 1, y - 1)];

        var newOutside = Outside ? table[511] : table[0];

        return new Image(newImage, newOutside).Trim();
    }

    public static Image Parse(bool[] table, string[] input)
    {
        var height = input.Length;
        var width = input[0].Length;
        var image = new bool[width, height];
        for (var y = 0; y < height; y++)
        {
            var line = input[y];
            for (var x = 0; x < width; x++)
            {
                var c = line[x];
                image[x, y] = c == '#';
            }
        }

        return new Image(image, false);
    }

    private static char CharMap(bool value)
    {
        return value ? '#' : '.';
    }

    public override string ToString()
    {
        var s = new StringBuilder();

        s.Append($"Outside: {CharMap(Outside)}\n");
        for (var y = 0; y < Inner.GetLength(1); y++)
        {
            for (var x = 0; x < Inner.GetLength(0); x++) s.Append(CharMap(Index(x, y)));

            s.Append('\n');
        }

        return s.ToString();
    }
}