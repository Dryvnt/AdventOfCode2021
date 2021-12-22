namespace Day22.Model.Reactor;

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

            Cuboid first;
            Cuboid second;
            if (overlapping.X.Min < overlap.X.Min)
            {
                (first, second) = overlapping.SplitX(overlap.X.Min);
            }
            else if (overlap.X.Max < overlapping.X.Max)
            {
                (second, first) = overlapping.SplitX(overlap.X.Max);
            }
            else if (overlapping.Y.Min < overlap.Y.Min)
            {
                (first, second) = overlapping.SplitY(overlap.Y.Min);
            }
            else if (overlap.Y.Max < overlapping.Y.Max)
            {
                (second, first) = overlapping.SplitY(overlap.Y.Max);
            }
            else if (overlapping.Z.Min < overlap.Z.Min)
            {
                (first, second) = overlapping.SplitZ(overlap.Z.Min);
            }
            else if (overlap.Z.Max < overlapping.Z.Max)
            {
                (second, first) = overlapping.SplitZ(overlap.Z.Max);
            }
            else throw new NotImplementedException();

            _cuboids.Add(first);
            overlapSet.Push(second);
        }

        if (instruction.On) _cuboids.Add(cuboid);
    }
}