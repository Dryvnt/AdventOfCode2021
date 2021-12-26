namespace Day24;

internal record Instruction
{
    private static Register ParseRegister(string r)
    {
        return r switch
        {
            "w" => Register.W,
            "x" => Register.X,
            "y" => Register.Y,
            "z" => Register.Z,
            _ => throw new ArgumentOutOfRangeException(nameof(r), r, null),
        };
    }

    public static Instruction Parse(string line)
    {
        var words = line.Split(' ');
        var r1 = ParseRegister(words[1]);

        if (words.Length == 2)
        {
            return words[0] switch
            {
                "inp" => new Inp(r1),
                _ => throw new ArgumentOutOfRangeException(nameof(line), line, null),
            };
        }

        var hasLiteral = long.TryParse(words[2], out var literal);
        if (hasLiteral)
        {
            return words[0] switch
            {
                "add" => new AddLiteral(r1, literal),
                "mul" => new MulLiteral(r1, literal),
                "div" => new DivLiteral(r1, literal),
                "mod" => new ModLiteral(r1, literal),
                "eql" => new EqlLiteral(r1, literal),
                _ => throw new ArgumentOutOfRangeException(nameof(line), line, null),
            };
        }

        var r2 = ParseRegister(words[2]);

        return words[0] switch
        {
            "add" => new Add(r1, r2),
            "mul" => new Mul(r1, r2),
            "div" => new Div(r1, r2),
            "mod" => new Mod(r1, r2),
            "eql" => new Eql(r1, r2),
            _ => throw new ArgumentOutOfRangeException(nameof(line), line, null),
        };
    }
}

internal record Inp(Register A) : Instruction;

internal record Add(Register A, Register B) : Instruction;

internal record Mul(Register A, Register B) : Instruction;

internal record Div(Register A, Register B) : Instruction;

internal record Mod(Register A, Register B) : Instruction;

internal record Eql(Register A, Register B) : Instruction;

internal record AddLiteral(Register A, long Literal) : Instruction;

internal record MulLiteral(Register A, long Literal) : Instruction;

internal record DivLiteral(Register A, long Literal) : Instruction;

internal record ModLiteral(Register A, long Literal) : Instruction;

internal record EqlLiteral(Register A, long Literal) : Instruction;

internal enum Register
{
    X = 0,
    Y = 1,
    Z = 2,
    W = 3,
}