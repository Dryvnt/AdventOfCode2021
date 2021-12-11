var lines = File.ReadAllLines("input");

var allNumbers = lines.Select(l => Convert.ToInt32(l, 2)).ToList();

int CountOnes(IEnumerable<int> numbers, int bitPosition)
{
    var bitConst = 1 << bitPosition;
    return numbers.Select(n => (n & bitConst) >> bitPosition).Sum();
}

int BitsToNumber(IEnumerable<int> bits)
{
    var bitsString = string.Join("", bits.Select(b => b == 0 ? "0" : "1").Reverse());
    var number = Convert.ToInt32(bitsString, 2);
    return number;
}

var counts = Enumerable.Range(0, 12).Select(i => CountOnes(allNumbers, i)).ToList();

var mostCommonBits = counts.Select(c => c > allNumbers.Count / 2 ? 1 : 0).ToList();
var gamma = BitsToNumber(mostCommonBits);
var epsilon = ~gamma & 0b11_1111_1111;

Console.WriteLine($"Part 1 - Gamma: {gamma}, Epsilon: {epsilon}, Answer: {epsilon * gamma}");

int LifeSupportSelector(IList<int> candidates, LifeSupportVariant variant)
{
    // In the description, the "first" bit is to the left, and moves to the right.
    // So traverse bits in machine-opposite order
    var discriminantBit = 11;
    while (candidates.Count > 1)
    {
        var ones = CountOnes(candidates, discriminantBit);
        var zeros = candidates.Count - ones;

        int criteria;
        switch (variant)
        {
            case LifeSupportVariant.Oxygen:
                criteria = ones >= zeros ? 1 : 0;
                break;
            case LifeSupportVariant.Co2:
                criteria = zeros <= ones ? 0 : 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(variant), variant, null);
        }

        candidates = candidates.Where(n => ((n >> discriminantBit) & 1) == criteria)
            .ToList();
        discriminantBit -= 1;
    }

    return candidates.First();
}

var oxygen = LifeSupportSelector(allNumbers, LifeSupportVariant.Oxygen);
var co2 = LifeSupportSelector(allNumbers, LifeSupportVariant.Co2);

Console.WriteLine($"Part 2 - Oxygen: {oxygen}, CO2: {co2}, Answer: {oxygen * co2}");

internal enum LifeSupportVariant
{
    Oxygen,
    Co2,
}