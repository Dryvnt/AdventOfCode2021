using System.Linq;
using Day16;
using Xunit;

namespace Day16Test;

public class Test
{
    [Theory]
    [InlineData("D2FE28", 2021)]
    public void LiteralPacket(string input, ulong expected)
    {
        var reader = new BitReader(input);
        var packet = Parsing.ReadPacket(reader);
        Assert.IsType<LiteralPacket>(packet);
        var p = (LiteralPacket)packet;
        Assert.Equal(expected, p.Literal);
    }

    [Theory]
    [InlineData("8A004A801A8002F478", 16)]
    [InlineData("620080001611562C8802118E34", 12)]
    [InlineData("C0015000016115A2E0802F182340", 23)]
    [InlineData("A0016C880162017C3686B18A3D4780", 31)]
    public void VersionSum(string input, uint expected)
    {
        var reader = new BitReader(input);
        var packet = Parsing.ReadPacket(reader);
        var versionSum = Parsing.SumVersionNumbers(packet);

        Assert.Equal(expected, versionSum);
    }

    [Theory]
    [InlineData("C200B40A82", 3)]
    [InlineData("04005AC33890", 54)]
    [InlineData("880086C3E88112", 7)]
    [InlineData("CE00C43D881120", 9)]
    [InlineData("D8005AC2A8F0", 1)]
    [InlineData("F600BC2D8F", 0)]
    [InlineData("9C005AC2F8F0", 0)]
    [InlineData("9C0141080250320F1802104A08", 1)]
    public void Value(string input, ulong expected)
    {
        var reader = new BitReader(input);
        var packet = Parsing.ReadPacket(reader);
        var value = packet.Value();
        Assert.Equal(expected, value);
    }

    [Fact]
    public void ProductsEqualThemselves()
    {
        var literal = new LiteralPacket(3, 10);
        var product = new OperatorPacket(3, PacketType.Product, new Packet[] { literal }.ToList());

        Assert.Equal(10UL, product.Value());
    }

    [Theory]
    [InlineData("0", "00")]
    [InlineData("012", "0120")]
    [InlineData("ABC", "ABC0")]
    [InlineData("A", "A0")]
    [InlineData("01\n", "01")]
    [InlineData("A\n", "A0")]
    public void BitReaderInputRobustness(string input, string expected)
    {
        var reader = new BitReader(input);
        var expectedReader = new BitReader(expected);

        var length = input.Length * 4;
        Assert.Equal(expectedReader.ReadBits(length), reader.ReadBits(length));
    }
}