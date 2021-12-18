namespace Day18;

internal record Ast(long? Value, (Ast Left, Ast Right)? Tree)
{
    public long Magnitude()
    {
        if (Value.HasValue) return Value.Value;
        if (Tree.HasValue) return Tree.Value.Left.Magnitude() * 3 + Tree.Value.Right.Magnitude() * 2;
        throw new NotImplementedException();
    }

    internal static Ast Parse(string input)
    {
        if (long.TryParse(input, out var earlyExitValue)) return new Ast(earlyExitValue, null);

        var astStack = new Stack<Ast>();

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            switch (c)
            {
                case '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                    var subString = input[i..(i + 1)];
                    if (long.TryParse(subString, out var value)) astStack.Push(new Ast(value, null));
                    break;
                case ']':
                    var right = astStack.Pop();
                    var left = astStack.Pop();
                    astStack.Push(new Ast(null, (left, right)));
                    break;
            }
        }

        return astStack.Pop();
    }
}