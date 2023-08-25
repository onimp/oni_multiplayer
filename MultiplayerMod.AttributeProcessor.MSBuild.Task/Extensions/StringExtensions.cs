namespace MultiplayerMod.AttributeProcessor.MSBuild.Task.Extensions;

public static class StringExtensions {

    public static string? Decapitalize(this string? value) {
        if (string.IsNullOrEmpty(value) || char.IsLower(value![0]))
            return value;

        return char.ToLower(value[0]) + value.Substring(1);
    }

}
