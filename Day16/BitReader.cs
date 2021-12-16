namespace Day16;

public class BitReader
{
    private readonly byte[] data;
    private int cursor;
    private int subCursor;

    public int TotalBitsRead;

    public BitReader(string input)
    {
        input = input.Trim();

        if (input.Length % 2 == 1) input = $"{input}0";

        data = new byte[input.Length / 2];
        for (var i = 0; i < data.Length; i++)
        {
            var byteString = input.Substring(i * 2, 2);
            data[i] = Convert.ToByte(byteString, 16);
        }
    }

    private void IncrementCursor()
    {
        subCursor += 1;

        if (subCursor <= 7) return;

        subCursor = 0;
        cursor += 1;
    }

    public uint ReadBit()
    {
        TotalBitsRead += 1;

        if (cursor >= data.Length) return 0;

        var value = (byte)((data[cursor] >> (7 - subCursor)) & 1);
        IncrementCursor();
        return value;
    }

    // This could be implemented in a much smarter way where we read
    // multiple bits at once when n > 1, but that's complicated and this
    // is perfectly fine for these purposes.
    public uint ReadBits(int n)
    {
        if (n == 0) return 0;
        var value = ReadBit();
        for (var i = 1; i < n; i++)
        {
            value <<= 1;
            value |= ReadBit();
        }

        return value;
    }
}