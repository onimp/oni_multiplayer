namespace MultiplayerMod.Core.Extensions;

public static class ArraysExtensions {

    private static readonly uint[] hexLookup = CreateHexLookup();

    private static uint[] CreateHexLookup() {
        const int byteCardinality = byte.MaxValue + 1;
        var result = new uint[byteCardinality];
        for (var i = 0; i < byteCardinality; i++) {
            var value = $"{i:x2}";
            result[i] = value[0] + (uint) (value[1] << 16);
        }
        return result;
    }

    public static string ToHexString(this byte[] bytes) {
        var result = new char[bytes.Length * 2];
        for (var i = 0; i < bytes.Length; i++) {
            var value = hexLookup[bytes[i]];
            result[2 * i] = (char) value;
            result[2 * i + 1] = (char) (value >> 16);
        }
        return new string(result);
    }

}
