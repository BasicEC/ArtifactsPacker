namespace ArtifactsPacker.Services;

public static class HexConverter
{
    public static string Convert(byte[] bytes)
    {
        Span<char> hex = stackalloc char[bytes.Length * 2];
        var j = 0;
        foreach (var b in bytes)
        {
            Convert(b, out hex[j++], out hex[j++]);
        }

        return new string(hex);
    }
    
    private static void Convert(byte @byte, out char left, out char right)
    {
        var rightValue = @byte & 0x0f;
        var leftValue = (@byte & 0xf0) >> 4;
        left = ConvertPart(leftValue);
        right = ConvertPart(rightValue);
    }

    private static char ConvertPart(int part) =>
        part switch
        {
            0 => '0',
            1 => '1',
            2 => '2',
            3 => '3',
            4 => '4',
            5 => '5',
            6 => '6',
            7 => '7',
            8 => '8',
            9 => '9',
            10 => 'a',
            11 => 'b',
            12 => 'c',
            13 => 'd',
            14 => 'e',
            15 => 'f',
            _ => throw new ArgumentOutOfRangeException(nameof(part))
        };
}
