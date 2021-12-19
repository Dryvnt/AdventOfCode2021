// It really feels like today's task wants matrix math
// Oh well

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

ScannerCollection ReduceCollections(IReadOnlyCollection<ScannerData> scannerData)
{
    var collection = new ScannerCollection(scannerData.First());

    // Pre-transform data
    var transforms = new Dictionary<int, List<ScannerData>>();
    foreach (var s in scannerData.Skip(1))
    {
        var l = new List<ScannerData>();

        foreach (var f in Enum.GetValues<Facing>())
        {
            for (var rot = 0; rot < 4; rot++)
            {
                var transformedBeacons = s.Beacons.Select(b => b.CorrectFacing(f).MultipleRotate(rot)).ToHashSet();
                l.Add(new ScannerData(s.Id, transformedBeacons));
            }
        }

        transforms[s.Id] = l;
    }

    var partialOverlaps = new Dictionary<ScannerData, Dictionary<Coordinate, int>>();

    var newBeacons = collection.Beacons.ToHashSet();

    while (transforms.Any())
    {
        foreach (var (data, counts) in partialOverlaps)
        {
            foreach (var adjust in counts.Keys)
            {
                var i = data.CountOverlaps(newBeacons, adjust);
            }
        }

        foreach (var b in newBeacons)
        {
            foreach (var d in transforms.Values.SelectMany(l => l))
            {
                foreach (var toMove in d.Beacons)
                {
                    var adjust = b - toMove;
                    var overlaps = d.CountOverlaps(newBeacons, adjust);

                    if (partialOverlaps.ContainsKey(d) && partialOverlaps[d].ContainsKey(adjust))
                    {
                        partialOverlaps[d][adjust] += overlaps;
                        continue;
                    }

                    if (overlaps < 2) continue;

                    partialOverlaps[d] = new Dictionary<Coordinate, int>
                    {
                        [adjust] = overlaps,
                    };
                }
            }
        }

        newBeacons.Clear();

        var toRemove = new HashSet<int>();

        foreach (var (data, adjust, count) in partialOverlaps.SelectMany(p =>
                     p.Value.Select(pp => (p.Key, pp.Key, pp.Value))))
        {
            if (count < 12) continue;

            Console.WriteLine($"Merging {data.Id}");
            var adjusted = data.Adjust(adjust);

            var spill = collection.MergeWith(data, adjust);

            newBeacons.UnionWith(spill);
            toRemove.Add(data.Id);
        }

        Debug.Assert(newBeacons.Any());

        foreach (var id in toRemove)
        {
            foreach (var d in transforms[id]) partialOverlaps.Remove(d);

            transforms.Remove(id);
        }
    }

    return collection;
}

var input = File.ReadAllText("input");
const string pattern = @"--- scanner ([0-9]+) ---\n((.+\n)+)";

var scannerData = new List<ScannerData>();
foreach (Match m in Regex.Matches(input, pattern))
{
    var id = int.Parse(m.Groups[1].Value);
    var beaconsString = m.Groups[2].Value;
    var beacons = beaconsString.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).Select(l =>
    {
        var values = l.Split(',').Select(int.Parse).ToArray();
        return new Coordinate(values[0], values[1], values[2]);
    }).ToHashSet();

    scannerData.Add(new ScannerData(id, beacons));
}

var collection = ReduceCollections(scannerData);

Console.WriteLine($"Part 1: {collection.Beacons.Count}");

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


internal class ScannerCollection
{
    public ScannerCollection(ScannerData scanner0)
    {
        Scanners = new List<Scanner>
        {
            new(scanner0.Id, new Coordinate(0, 0, 0)),
        };
        Beacons = scanner0.Beacons.ToHashSet();
    }

    public List<Scanner> Scanners { get; init; }
    public HashSet<Coordinate> Beacons { get; init; }

    // returns new coordinates that weren't in set before
    public HashSet<Coordinate> MergeWith(ScannerData other, Coordinate adjust)
    {
        Scanners.Add(new Scanner(other.Id, adjust));
        var adjusted = other.Adjust(adjust).ToHashSet();
        var spill = adjusted.Except(Beacons).ToHashSet();
        Beacons.UnionWith(adjusted);
        return spill;
    }

    public override string ToString()
    {
        var s = new StringBuilder();
        s.Append("Scanner Collection\n");
        foreach (var scanner in Scanners) s.Append($"\t{scanner}\n");

        var beaconStrings = Beacons.OrderBy(b => b).Select(b => $"\tBeacon {b}");
        s.Append(string.Join("\n", beaconStrings));
        return s.ToString();
    }

    public void Deconstruct(out List<Scanner> Scanners, out HashSet<Coordinate> Beacons)
    {
        Scanners = this.Scanners;
        Beacons = this.Beacons;
    }
}

internal class ScannerData
{
    public ScannerData(int id, IReadOnlySet<Coordinate> beacons)
    {
        Id = id;
        Beacons = beacons;
    }

    public int Id { get; }
    public IReadOnlySet<Coordinate> Beacons { get; }

    public int CountOverlaps(IEnumerable<Coordinate> other, Coordinate adjustBy)
    {
        return Adjust(adjustBy).Intersect(other).Count();
    }

    public IEnumerable<Coordinate> Adjust(Coordinate by)
    {
        return Beacons.Select(b => b + by);
    }

    public override string ToString()
    {
        return $"Scanner {Id}";
    }
}

internal readonly record struct Scanner(int Id, Coordinate Position)
{
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

internal readonly record struct Coordinate(int X, int Y, int Z) : IComparable<Coordinate>, IComparable
{
    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        return obj is Coordinate other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(Coordinate)}");
    }

    public int CompareTo(Coordinate other)
    {
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