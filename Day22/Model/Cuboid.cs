using System.Runtime.CompilerServices;

namespace Day22.Model;

public readonly record struct Cuboid(Range X, Range Y, Range Z)
{
    public long Size => X.Size * Y.Size * Z.Size;

    public static Cuboid Parse(string input)
    {
        var split = input.Split(',');
        var x = Range.Parse(split[0]);
        var y = Range.Parse(split[1]);
        var z = Range.Parse(split[2]);

        return new Cuboid(x, y, z);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public bool Overlaps(Cuboid other)
    {
        var ((xMin, xMax), (yMin, yMax), (zMin, zMax)) = other;
        var xOverlaps = xMin < X.Max && X.Min < xMax;
        var yOverlaps = yMin < Y.Max && Y.Min < yMax;
        var zOverlaps = zMin < Z.Max && Z.Min < zMax;
        return xOverlaps && yOverlaps && zOverlaps;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Cuboid GetOverlap(Cuboid other)
    {
        var xMin = Math.Max(X.Min, other.X.Min);
        var xMax = Math.Min(X.Max, other.X.Max);
        var yMin = Math.Max(Y.Min, other.Y.Min);
        var yMax = Math.Min(Y.Max, other.Y.Max);
        var zMin = Math.Max(Z.Min, other.Z.Min);
        var zMax = Math.Min(Z.Max, other.Z.Max);

        return new Cuboid(new Range(xMin, xMax), new Range(yMin, yMax), new Range(zMin, zMax));
    }

    public (Cuboid, Cuboid) SplitX(int x)
    {
        return (
            this with
            {
                X = new Range(X.Min, x),
            },
            this with
            {
                X = new Range(x, X.Max),
            });
    }

    public (Cuboid, Cuboid) SplitY(int y)
    {
        return (
            this with
            {
                Y = new Range(Y.Min, y),
            },
            this with
            {
                Y = new Range(y, Y.Max),
            });
    }

    public (Cuboid, Cuboid) SplitZ(int z)
    {
        return (
            this with
            {
                Z = new Range(Z.Min, z),
            },
            this with
            {
                Z = new Range(z, Z.Max),
            });
    }
}