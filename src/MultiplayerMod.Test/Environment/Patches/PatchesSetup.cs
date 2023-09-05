using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MultiplayerMod.Core.Extensions;

namespace MultiplayerMod.Test.Environment.Patches;

public static class PatchesSetup {

    public static void Install(Harmony harmony, List<Type> patches) =>
        patches.ForEach(it => harmony.CreateClassProcessor(it).Patch());

    public static void Uninstall(Harmony harmony) {
        foreach (var methodBase in harmony.GetPatchedMethods()) {
            if (!methodBase.HasMethodBody())
                continue;

            var patches = Harmony.GetPatchInfo(methodBase);
            patches.Prefixes
                .Union(patches.Postfixes)
                .Union(patches.Transpilers)
                .Union(patches.Finalizers)
                .Where(it => it.owner == harmony.Id)
                .ForEach(it => harmony.Unpatch(methodBase, it.PatchMethod));
        }
    }

}
