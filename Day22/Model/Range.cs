namespace Day22;

public readonly record struct Range(int Min, int Max)
{
    public long Size => Math.Max(0, Max - Min);

    public static Range Parse(string input)
    {
        var split = input[2..].Split("..");
        var min = int.Parse(split[0]);
        var max = int.Parse(split[1]) + 1;

        return new Range(min, max);
    }

    public bool Contains(int c)
    {
        return Min <= c && c < Max;
    }
}