namespace Day18;

public class Number : IEquatable<Number>
{
    private string _underlying;

    public Number(string underlying)
    {
        _underlying = underlying;
    }

    public bool Equals(Number? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(_underlying, other._underlying, StringComparison.InvariantCulture);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Number)obj);
    }

    public override int GetHashCode()
    {
        return StringComparer.InvariantCulture.GetHashCode(_underlying);
    }

    public static bool operator ==(Number? left, Number? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Number? left, Number? right)
    {
        return !Equals(left, right);
    }

    private bool FindExplode(out int start, out int middle, out int end)
    {
        var depth = 0;
        start = -1;
        middle = -1;
        end = -1;

        for (var i = 0; i < _underlying.Length; i++)
        {
            var c = _underlying[i];
            switch (c)
            {
                case '[':
                    depth += 1;
                    if (depth > 4) start = i;
                    break;
                case ',':
                    middle = i;
                    break;
                case ']' when depth > 4:
                    end = i;
                    return true;
                case ']':
                    depth -= 1;
                    break;
            }
        }

        return false;
    }

    private string ExplodeLeft(int offset, long value)
    {
        var numberEnd = -1;
        var numberStart = -1;

        for (var i = offset - 1; i >= 0; i--)
        {
            var c = _underlying[i];
            if ("0123456789".Contains(c))
            {
                numberStart = i;
                if (numberEnd == -1) numberEnd = i;
            }
            else if (numberEnd != -1) break;
        }

        if (numberStart == -1) return _underlying[..offset];

        var number = long.Parse(_underlying[numberStart..(numberEnd + 1)]);
        return $"{_underlying[..numberStart]}{number + value}{_underlying[(numberEnd + 1)..offset]}";
    }

    private string ExplodeRight(int offset, long value)
    {
        var numberStart = -1;
        var numberEnd = -1;

        for (var i = offset; i < _underlying.Length; i++)
        {
            var c = _underlying[i];
            if ("0123456789".Contains(c))
            {
                numberEnd = i;
                if (numberStart == -1) numberStart = i;
            }
            else if (numberStart != -1) break;
        }

        if (numberStart == -1) return _underlying[offset..];
        var number = long.Parse(_underlying[numberStart..(numberEnd + 1)]);
        var s = $"{_underlying[offset..numberStart]}{number + value}{_underlying[(numberEnd + 1)..]}";
        return s;
    }

    private bool Explode()
    {
        if (!FindExplode(out var start, out var middle, out var end)) return false;


        var leftString = _underlying[(start + 1)..middle];
        var rightString = _underlying[(middle + 1)..end];
        var left = long.Parse(leftString);
        var right = long.Parse(rightString);

        _underlying = $"{ExplodeLeft(start, left)}0{ExplodeRight(end + 1, right)}";
        return true;
    }

    private bool Split()
    {
        if (!FindSplit(out var start, out var end)) return false;

        var number = long.Parse(_underlying[start..(end + 1)]);

        var left = number / 2;
        var right = number - left;

        _underlying = $"{_underlying[..start]}[{left},{right}]{_underlying[(end + 1)..]}";

        return true;
    }

    private bool FindSplit(out int start, out int end)
    {
        start = -1;
        end = -1;

        for (var i = 0; i < _underlying.Length; i++)
        {
            var c = _underlying[i];
            if ("0123456789".Contains(c))
            {
                end = i;
                if (start == -1) start = i;
            }
            else if (start != -1)
            {
                if (end - start > 0) return true;
                start = -1;
                end = -1;
            }
        }

        return false;
    }

    public void Reduce()
    {
        while (true)
        {
            var e = Explode();
            if (e) continue;

            var s = Split();
            if (s) continue;

            break;
        }
    }

    public long Magnitude()
    {
        return Ast.Parse(_underlying).Magnitude();
    }

    public static Number operator +(Number a, Number b)
    {
        var num = new Number($"[{a},{b}]");
        num.Reduce();
        return num;
    }

    public override string ToString()
    {
        return _underlying;
    }
}