using System;

namespace MultiplayerMod.Core.Extensions;

public static class NumberExtensions {

    public static readonly string Digits = "0123456789abcdefghijklmnopqrstuvwxyz";

    private static string LongToString(ulong value, int radix) {
        if (radix is < 2 or > 36)
            throw new ArgumentException("Radix must be from 2 to 36", nameof(radix));
        var result = "";
        do
            result = Digits[(int)(value % (byte)radix)] + result;
        while ((value /= (byte)radix) != 0);
        return result;
    }

    public static string ToString(this long value, int radix) => LongToString((ulong)value, radix);

}
