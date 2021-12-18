using System;
using System.IO;
using System.Linq;
using System.Text;
using Day18;
using Xunit;
using Xunit.Abstractions;

namespace Day18Test;

public class NumberTest
{
    public NumberTest(ITestOutputHelper output)
    {
        var converter = new Converter(output);
        Console.SetOut(converter);
    }

    public static TheoryData<string[], string> LargeSumExample => new()
    {
        {
            new[]
            {
                "[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]",
                "[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]",
                "[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]",
                "[[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]",
                "[7,[5,[[3,8],[1,4]]]]",
                "[[2,[2,2]],[8,[8,1]]]",
                "[2,9]",
                "[1,[[[9,3],9],[[9,0],[0,7]]]]",
                "[[[5,[7,4]],7],1]",
                "[[[[4,2],2],6],[8,7]]",
            },
            "[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]"
        },
    };

    [Theory]
    [InlineData("[1,2]")]
    [InlineData("[[1,2],3]")]
    [InlineData("[9,[8,7]]")]
    [InlineData("[[1,9],[8,5]]")]
    [InlineData("[[[[1,2],[3,4]],[[5,6],[7,8]]],9]")]
    [InlineData("[[[9,[3,8]],[[0,9],6]],[[[3,7],[4,9]],3]]")]
    [InlineData("[[[[1,3],[5,3]],[[1,3],[8,7]]],[[[4,9],[6,9]],[[8,2],[7,3]]]]")]
    public void ParseNoFail(string input)
    {
        var _ = new Number(input);
    }

    [Theory]
    [InlineData("[1,2]")]
    [InlineData("[[1,2],3]")]
    [InlineData("[9,[8,7]]")]
    [InlineData("[[1,9],[8,5]]")]
    [InlineData("[[[[1,2],[3,4]],[[5,6],[7,8]]],9]")]
    [InlineData("[[[9,[3,8]],[[0,9],6]],[[[3,7],[4,9]],3]]")]
    [InlineData("[[[[1,3],[5,3]],[[1,3],[8,7]]],[[[4,9],[6,9]],[[8,2],[7,3]]]]")]
    public void Equality(string input)
    {
        var a = new Number(input);
        var b = new Number(input);

        Assert.Equal(a, b);
    }

    [Theory]
    [InlineData("[[[[[9,8],1],2],3],4]", "[[[[0,9],2],3],4]")]
    [InlineData("[7,[6,[5,[4,[3,2]]]]]", "[7,[6,[5,[7,0]]]]")]
    [InlineData("[[6,[5,[4,[3,2]]]],1]", "[[6,[5,[7,0]]],3]")]
    [InlineData("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]")]
    [InlineData("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]")]
    public void ReduceExplode(string input, string result)
    {
        var a = new Number(input);
        var expected = new Number(result);

        a.Reduce();

        Assert.Equal(expected, a);
    }

    [Theory]
    [InlineData("[10,0]", "[[5,5],0]")]
    [InlineData("[32,0]", "[[[8,8],[8,8]],0]")]
    [InlineData("[11,0]", "[[5,6],0]")]
    public void ReduceSplit(string input, string result)
    {
        var a = new Number(input);
        var expected = new Number(result);

        a.Reduce();

        Assert.Equal(expected, a);
    }

    [Theory]
    [InlineData("[[[[4,3],4],4],[7,[[8,4],9]]]", "[1,1]", "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]")]
    [InlineData("[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]", "[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]",
        "[[[[7,8],[6,6]],[[6,0],[7,7]]],[[[7,8],[8,8]],[[7,9],[0,6]]]]")]
    public void Addition(string a, string b, string result)
    {
        var aPair = new Number(a);
        var bPair = new Number(b);
        var expected = new Number(result);

        var add = aPair + bPair;

        Assert.Equal(expected, add);
    }


    [Theory]
    [InlineData(new[] { "[1,1]", "[2,2]", "[3,3]", "[4,4]" }, "[[[[1,1],[2,2]],[3,3]],[4,4]]")]
    [InlineData(new[] { "[1,1]", "[2,2]", "[3,3]", "[4,4]", "[5,5]" }, "[[[[3,0],[5,3]],[4,4]],[5,5]]")]
    [InlineData(new[] { "[1,1]", "[2,2]", "[3,3]", "[4,4]", "[5,5]", "[6,6]" }, "[[[[5,0],[7,4]],[5,5]],[6,6]]")]
    [MemberData(nameof(LargeSumExample))]
    public void Sum(string[] values, string result)
    {
        var pairs = values.Select(v => new Number(v)).ToList();
        var sum = pairs.Skip(1).Aggregate(pairs.First(), (current, a) => current + a);

        Assert.Equal(new Number(result), sum);
    }

    [Theory]
    [InlineData("[[1,2],[[3,4],5]]", 143)]
    [InlineData("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]", 1384)]
    [InlineData("[[[[1,1],[2,2]],[3,3]],[4,4]]", 445)]
    [InlineData("[[[[3,0],[5,3]],[4,4]],[5,5]]", 791)]
    [InlineData("[[[[5,0],[7,4]],[5,5]],[6,6]]", 1137)]
    [InlineData("[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]", 3488)]
    [InlineData("[[[[7,8],[6,6]],[[6,0],[7,7]]],[[[7,8],[8,8]],[[7,9],[0,6]]]]", 3993)]
    public void Magnitude(string value, long expected)
    {
        var pair = new Number(value).Magnitude();

        Assert.Equal(expected, pair);
    }

    private class Converter : TextWriter
    {
        private readonly ITestOutputHelper _output;

        public Converter(ITestOutputHelper output)
        {
            _output = output;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void WriteLine(string? message)
        {
            _output.WriteLine(message);
        }

        public override void WriteLine(string format, params object?[] args)
        {
            _output.WriteLine(format, args);
        }

        public override void Write(char value)
        {
            throw new NotSupportedException(
                "This text writer only supports WriteLine(string) and WriteLine(string, params object[]).");
        }
    }
}