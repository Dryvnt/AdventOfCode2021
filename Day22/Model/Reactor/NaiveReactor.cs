namespace Day22.Model.Reactor;

// Useful as a comparison in tests
public class NaiveReactor : IReactor
{
    private readonly HashSet<Vector3> _grid = new();

    public long CountOn()
    {
        return _grid.Count;
    }

    public void Mark(Instruction instruction)
    {
        var (on, cuboid) = instruction;
        var ((xMin, xMax), (yMin, yMax), (zMin, zMax)) = cuboid;
        for (var x = xMin; x < xMax; x++)
        for (var y = yMin; y < yMax; y++)
        for (var z = zMin; z < zMax; z++)
        {
            var v = new Vector3(x, y, z);
            if (on) _grid.Add(v);
            else _grid.Remove(v);
        }
    }
}