using Day22.Model;

namespace Day22;

public record struct Instruction(bool On, Cuboid Cuboid)
{
    public static Instruction Parse(string input)
    {
        var split = input.Split(' ');
        var on = split[0] == "on";
        var cuboid = Cuboid.Parse(split[1]);

        return new Instruction(on, cuboid);
    }
}