namespace Day16;

public record Packet(uint Version)
{
    public virtual ulong Value()
    {
        throw new NotImplementedException();
    }
}

public record LiteralPacket(uint Version, ulong Literal) : Packet(Version)
{
    public override ulong Value()
    {
        return Literal;
    }

    public override string ToString()
    {
        return Literal.ToString();
    }
}

public record OperatorPacket(uint Version, PacketType Type, List<Packet> SubPackets) : Packet(Version)
{
    public override ulong Value()
    {
        return Type switch
        {
            PacketType.Sum => SubPackets.Select(p => p.Value()).Aggregate(0UL, (a, b) => a + b),
            PacketType.Product => SubPackets.Select(p => p.Value()).Aggregate(1UL, (a, b) => a * b),
            PacketType.Minimum => SubPackets.Select(p => p.Value()).Min(),
            PacketType.Maximum => SubPackets.Select(p => p.Value()).Max(),
            PacketType.GreaterThan => SubPackets[0].Value() > SubPackets[1].Value() ? 1UL : 0UL,
            PacketType.LessThan => SubPackets[0].Value() < SubPackets[1].Value() ? 1UL : 0UL,
            PacketType.EqualTo => SubPackets[0].Value() == SubPackets[1].Value() ? 1UL : 0UL,
            PacketType.Literal => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public override string ToString()
    {
        return Type switch
        {
            PacketType.Sum => $"({string.Join(" + ", SubPackets)})",
            PacketType.Product => $"({string.Join(" * ", SubPackets)})",
            PacketType.Minimum => $"Min({string.Join(", ", SubPackets)})",
            PacketType.Maximum => $"Max({string.Join(", ", SubPackets)})",
            PacketType.GreaterThan => $"({SubPackets[0]} > {SubPackets[1]})",
            PacketType.LessThan => $"({SubPackets[0]} < {SubPackets[1]})",
            PacketType.EqualTo => $"({SubPackets[0]} == {SubPackets[1]})",
            PacketType.Literal => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}

public enum PacketType
{
    Sum = 0,
    Product = 1,
    Minimum = 2,
    Maximum = 3,
    Literal = 4,
    GreaterThan = 5,
    LessThan = 6,
    EqualTo = 7,
}