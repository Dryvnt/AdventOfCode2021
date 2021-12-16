namespace Day16;

public static class Parsing
{
    private static List<Packet> ReadSubTotalBits(BitReader reader)
    {
        var packets = new List<Packet>();
        var toRead = reader.ReadBits(15);
        var readSoFar = reader.TotalBitsRead;
        while (reader.TotalBitsRead != readSoFar + toRead)
        {
            if (reader.TotalBitsRead > readSoFar + toRead) throw new NotImplementedException();
            var subPacket = ReadPacket(reader);
            packets.Add(subPacket);
        }

        return packets;
    }

    private static List<Packet> ReadSubTotalPackets(BitReader reader)
    {
        var packets = new List<Packet>();
        var toRead = reader.ReadBits(11);
        for (var i = 0; i < toRead; i++)
        {
            var subPacket = ReadPacket(reader);
            packets.Add(subPacket);
        }

        return packets;
    }

    private static List<Packet> ReadSubPackets(BitReader reader)
    {
        var operatorLengthType = (OperatorLengthType)reader.ReadBit();
        return operatorLengthType switch
        {
            OperatorLengthType.TotalBits => ReadSubTotalBits(reader),
            OperatorLengthType.TotalPackets => ReadSubTotalPackets(reader),
            _ => throw new NotImplementedException(), // This will never happen?
        };
    }

    private static ulong ReadLiteral(BitReader reader)
    {
        var value = 0UL;
        var keepGoing = true;
        while (keepGoing)
        {
            keepGoing = reader.ReadBit() == 1;
            value |= reader.ReadBits(4);
            if (keepGoing) value <<= 4;
        }

        return value;
    }


    public static Packet ReadPacket(BitReader reader)
    {
        var version = reader.ReadBits(3);
        var type = (PacketType)reader.ReadBits(3);

        return type switch
        {
            PacketType.Literal => new LiteralPacket(version, ReadLiteral(reader)),
            _ => new OperatorPacket(version, type, ReadSubPackets(reader)),
        };
    }

    public static uint SumVersionNumbers(Packet p)
    {
        return p is not OperatorPacket(_, _, var subPackets)
            ? p.Version
            : subPackets.Select(SumVersionNumbers).Aggregate(p.Version, (a, b) => a + b);
    }

    private enum OperatorLengthType
    {
        TotalBits = 0b0,
        TotalPackets = 0b1,
    }
}