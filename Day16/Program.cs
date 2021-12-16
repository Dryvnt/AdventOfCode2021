using Day16;

var input = File.ReadAllText("input");

var reader = new BitReader(input);
var packet = Parsing.ReadPacket(reader);

Console.WriteLine(packet);

Console.WriteLine($"Part 1: {Parsing.SumVersionNumbers(packet)}");
Console.WriteLine($"Part 2: {packet.Value()}");