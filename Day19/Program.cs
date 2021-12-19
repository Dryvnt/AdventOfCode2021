// It really feels like today's task wants matrix math
// Oh well

using System.Text;
using System.Text.RegularExpressions;

ScannerCollection ReduceCollections(IEnumerable<Scanner> scanners)
{
    var scannerCollections = scanners.Select(s => new ScannerCollection(new List<Scanner> { s })).ToList();

    while (scannerCollections.Count > 1)
    {
        var progressMade = false;

        for (var i = 0; i < scannerCollections.Count - 1; i++)
        {
            for (var j = i + 1; j < scannerCollections.Count; j++)
            {
                var known = scannerCollections[i];
                var toMerge = scannerCollections[j];
                Console.WriteLine(
                    $"Try {string.Join(",", known.Scanners.Select(s => s.Id))} <- {string.Join(",", toMerge.Scanners.Select(s => s.Id))}");
                var merged = known.MergeWith(toMerge);
                if (merged is null) continue;

                Console.WriteLine("\tIt worked!");
                progressMade = true;
                scannerCollections[i] = merged;
                scannerCollections.RemoveAt(j);
                break;
            }

            if (progressMade) break;
        }

        if (progressMade) continue;

        //foreach (var s in scannerCollections) Console.WriteLine(s);

        throw new NotImplementedException("heck");
    }

    return scannerCollections.First();
}

var input = File.ReadAllText("input");
const string pattern = @"--- scanner ([0-9]+) ---\n((.+\n)+)";

var scanners = new List<Scanner>();
foreach (Match m in Regex.Matches(input, pattern))
{
    var id = int.Parse(m.Groups[1].Value);
    var beaconsString = m.Groups[2].Value;
    var beacons = beaconsString.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).Select(l =>
    {
        var values = l.Split(',').Select(int.Parse).ToArray();
        return new Coordinate(values[0], values[1], values[2]);
    }).ToHashSet();

    scanners.Add(new Scanner(id, new Coordinate(0, 0, 0), beacons));
}

var collection = ReduceCollections(scanners);

Console.WriteLine($"Part 1: {collection.KnownBeacons().Count}");

var maxDistance = 0;
for (var i = 0; i < collection.Scanners.Count - 1; i++)
{
    for (var j = i + 1; j < collection.Scanners.Count; j++)
    {
        var distance = (collection.Scanners[i].Position - collection.Scanners[j].Position).ManhattenDistance();
        if (distance > maxDistance) maxDistance = distance;
    }
}

Console.WriteLine($"Part 2: {maxDistance}");


internal record ScannerCollection(List<Scanner> Scanners)
{
    private HashSet<Coordinate>? _knownBeacons;

    public HashSet<Coordinate> KnownBeacons()
    {
        if (_knownBeacons is not null) return _knownBeacons;

        var knownBeacons = new HashSet<Coordinate>();
        foreach (var beacons in Scanners.Select(s => s.Beacons)) knownBeacons.UnionWith(beacons);

        _knownBeacons = knownBeacons;
        return _knownBeacons;
    }

    // other's positions are adjusted
    public ScannerCollection? MergeWith(ScannerCollection other)
    {
        var knownBeacons = KnownBeacons();
        foreach (var f in Enum.GetValues<Facing>())
        {
            for (var rotations = 1; rotations <= 4; rotations++)
            {
                var correctedOther = other.CorrectFacing(f, rotations);
                var correctedKnown = correctedOther.KnownBeacons();
                foreach (var reference in knownBeacons)
                {
                    foreach (var adjust in correctedKnown)
                    {
                        var adjustBy = reference - adjust;
                        var adjustedOther = correctedOther.Adjust(adjustBy);
                        var adjustedKnown = adjustedOther.KnownBeacons();

                        var overlap = knownBeacons.Intersect(adjustedKnown).ToHashSet();

                        if (overlap.Count is > 2 and < 12)
                        {
                            //Console.WriteLine($"Almost: {overlap.Count}");
                            //Console.WriteLine(adjustedOther);
                            //Console.WriteLine(string.Join("\n", overlap));
                            //Console.WriteLine($"{reference} - {adjust} = {adjustBy}");
                        }

                        if (overlap.Count < 12) continue;

                        var allScanners = Scanners.Concat(adjustedOther.Scanners).ToList();
                        return new ScannerCollection(allScanners);
                    }
                }
            }
        }

        return null;
    }

    private ScannerCollection Adjust(Coordinate by)
    {
        return new ScannerCollection(Scanners.Select(s => s.Adjust(by)).ToList());
    }

    public ScannerCollection CorrectFacing(Facing f, int rotations)
    {
        return new ScannerCollection(Scanners.Select(s => s.CorrectFacing(f, rotations)).ToList());
    }

    public override string ToString()
    {
        var s = new StringBuilder();
        s.Append("Scanner Collection\n");
        foreach (var scanner in Scanners) s.Append($"\t{scanner}\n");

        var beaconStrings = KnownBeacons().OrderBy(b => b).Select(b => $"\tBeacon {b}");
        s.Append(string.Join("\n", beaconStrings));
        return s.ToString();
    }
}

internal record Scanner(int Id, Coordinate Position, HashSet<Coordinate> Beacons)
{
    public Scanner CorrectFacing(Facing f, int rotations)
    {
        var transformedPosition = Position.CorrectFacing(f).MultipleRotate(rotations);
        var transformedBeacons = Beacons.Select(b => b.CorrectFacing(f).MultipleRotate(rotations)).ToHashSet();
        return new Scanner(Id, transformedPosition, transformedBeacons);
    }

    public Scanner Adjust(Coordinate by)
    {
        var adjustedPosition = Position + by;
        var adjustedSet = Beacons.Select(b => b + by).ToHashSet();

        return new Scanner(Id, adjustedPosition, adjustedSet);
    }

    public override string ToString()
    {
        return $"{nameof(Scanner)} {Id}, {Position}";
    }
}

internal enum Facing
{
    X,
    Y,
    Z,
    NegativeX,
    NegativeY,
    NegativeZ,
}

internal record Coordinate(int X, int Y, int Z) : IComparable<Coordinate>, IComparable
{
    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is Coordinate other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(Coordinate)}");
    }

    public int CompareTo(Coordinate? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var xComparison = X.CompareTo(other.X);
        if (xComparison != 0) return xComparison;
        var yComparison = Y.CompareTo(other.Y);
        if (yComparison != 0) return yComparison;
        return Z.CompareTo(other.Z);
    }

    public int ManhattenDistance()
    {
        return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
    }

    // Face towards X
    public Coordinate CorrectFacing(Facing currentFacing)
    {
        return currentFacing switch
        {
            Facing.X => this,
            Facing.Y => this with
            {
                X = Y,
                Y = -X,
            },
            Facing.Z => this with
            {
                X = Z,
                Z = -X,
            },
            Facing.NegativeX => this with
            {
                X = -X,
                Y = -Y,
            },
            Facing.NegativeY => this with
            {
                X = -Y,
                Y = X,
            },
            Facing.NegativeZ => this with
            {
                X = -Z,
                Z = X,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(currentFacing), currentFacing, null),
        };
    }

    // Continues facing X
    public Coordinate RotateClockwise()
    {
        return this with
        {
            Z = Y,
            Y = -Z,
        };
    }

    public Coordinate MultipleRotate(int n)
    {
        return Enumerable.Range(0, n).Aggregate(this, (b, _) => b.RotateClockwise());
    }

    public static Coordinate operator -(Coordinate a, Coordinate b)
    {
        return new Coordinate(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static Coordinate operator +(Coordinate a, Coordinate b)
    {
        return new Coordinate(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
}