using System;
using System.Collections.Generic;
using System.Diagnostics;
using HarmonyLib;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Patch.Generics;

namespace MultiplayerMod.Core.Patch;

public static class StackTraceExtensions {

    private const int routerClassStackSize = 3;

    public static IEnumerable<string> Format(this StackTrace stackTrace) {
        var frames = stackTrace.GetFrames() ?? throw new Exception("Unable to get stack frames");
        var methods = new List<string>();
        var index = 1;
        while (index < frames.Length) {
            var frame = frames[index];
            if (Harmony.GetOriginalMethodFromStackframe(frame).DeclaringType == typeof(HarmonyGenericsRouter)) {
                index += routerClassStackSize;
                continue;
            }
            var method = Harmony.GetOriginalMethodFromStackframe(frame);
            var name = method.DeclaringType!.GetSignature();
            if (name is not ("MethodBase" or "MonoMethod")) {
                var ilOffset = $"0x{frame.GetILOffset():x4}";
                var sourceLine = $"{frame.GetFileName() ?? "N/A"}:{frame.GetFileLineNumber()}";
                methods.Add($"{name}::{method.Name} at [{ilOffset}] ({sourceLine})");
            }

            index++;
        }
        return methods;
    }

}
