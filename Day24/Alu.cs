using System.Runtime.CompilerServices;

namespace Day24;

internal readonly record struct Alu(long X, long Y, long Z, long W)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Alu StepInp(Instruction instruction, long w)
    {
        return instruction switch
        {
            Inp inp => new Alu(X, Y, Z, w),
            _ => throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private Alu Set(Register r, long value)
    {
        return r switch
        {
            Register.X => this with { X = value },
            Register.Y => this with { Y = value },
            Register.Z => this with { Z = value },
            Register.W => this with { W = value },
            _ => throw new ArgumentOutOfRangeException(nameof(r), r, null),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private long Get(Register r)
    {
        return r switch
        {
            Register.X => X,
            Register.Y => Y,
            Register.Z => Z,
            Register.W => W,
            _ => throw new ArgumentOutOfRangeException(nameof(r), r, null),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public Alu Step(Instruction instruction)
    {
        return instruction switch
        {
            Add add => Set(add.A, Get(add.A) + Get(add.B)),
            Mul mul => Set(mul.A, Get(mul.A) * Get(mul.B)),
            Div div => Set(div.A, Get(div.A) / Get(div.B)),
            Mod mod => Set(mod.A, Get(mod.A) % Get(mod.B)),
            Eql eql => Set(eql.A, Get(eql.A) == Get(eql.B) ? 1 : 0),
            AddLiteral addLiteral => Set(addLiteral.A, Get(addLiteral.A) + addLiteral.Literal),
            MulLiteral mulLiteral => Set(mulLiteral.A, Get(mulLiteral.A) * mulLiteral.Literal),
            DivLiteral divLiteral => Set(divLiteral.A, Get(divLiteral.A) / divLiteral.Literal),
            ModLiteral modLiteral => Set(modLiteral.A, Get(modLiteral.A) % modLiteral.Literal),
            EqlLiteral eqlLiteral => Set(eqlLiteral.A, Get(eqlLiteral.A) == eqlLiteral.Literal ? 1 : 0),
            _ => throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null),
        };
    }
}