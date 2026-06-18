using System;
using System.Linq;

namespace MultiplayerMod.Multiplayer.Compatibility;

[Serializable]
public class CompatibilityValidationResult {

    public CompatibilityMismatch[] Mismatches { get; }
    public bool Compatible => Mismatches.Length == 0;

    public CompatibilityValidationResult(CompatibilityMismatch[] mismatches) {
        Mismatches = mismatches;
    }

    public string ToUserMessage() {
        if (Compatible)
            return "Compatible";

        return string.Join("\n", Mismatches.Select(it => $"- {it.Message}"));
    }

}
