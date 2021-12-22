using Day22.Model;
using Day22.Reactor;

namespace Day22;

public class SmartReactor : IReactor
{
    private readonly HashSet<Cuboid> _cuboids = new();

    public long CountOn()
    {
        return _cuboids.Select(c => c.Size).Sum();
    }

    public void Mark(Instruction instruction)
    {
        var cuboid = instruction.Cuboid;

        var overlapSet = new Stack<Cuboid>(_cuboids.Where(c => cuboid.Overlaps(c)));

        while (overlapSet.Any())
        {
            var overlapping = overlapSet.Pop();
            _cuboids.Remove(overlapping);

            var overlap = overlapping.GetOverlap(cuboid);
            if (overlap == overlapping) continue;

            Cuboid[] split;
            int i;
            if (overlapping.X.Min < overlap.X.Min)
            {
                split = overlapping.SplitX(overlap.X.Min);
                i = 0;
            }
            else if (overlap.X.Max < overlapping.X.Max)
            {
                split = overlapping.SplitX(overlap.X.Max);
                i = 1;
            }
            else if (overlapping.Y.Min < overlap.Y.Min)
            {
                split = overlapping.SplitY(overlap.Y.Min);
                i = 0;
            }
            else if (overlap.Y.Max < overlapping.Y.Max)
            {
                split = overlapping.SplitY(overlap.Y.Max);
                i = 1;
            }
            else if (overlapping.Z.Min < overlap.Z.Min)
            {
                split = overlapping.SplitZ(overlap.Z.Min);
                i = 0;
            }
            else if (overlap.Z.Max < overlapping.Z.Max)
            {
                split = overlapping.SplitZ(overlap.Z.Max);
                i = 1;
            }
            else throw new NotImplementedException();

            _cuboids.Add(split[i]);
            overlapSet.Push(split[1 - i]);
        }

        if (instruction.On) _cuboids.Add(cuboid);
    }
}