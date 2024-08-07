﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;

namespace MultiplayerMod.Test.Core.Patch.Compatibility;

public static class PatchesCompatibilityMetadata {

    /// <summary>
    /// Generated metadata, see <see cref="PatchesCompatibility.Generate"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
    private static readonly List<MethodMetadata> methodsMetadata = [
        new(typeof(AttackChore.States), "InitializeStates(StateMachine.BaseState&)", "76d0303d40ba0af708b454459541a2c0620c7abb"),
        new(typeof(ThreatMonitor), "InitializeStates(StateMachine.BaseState&)", "fe07b4523742ada76bdfadf86e281046171db20d"),
        new(typeof(IdleChore.States), "InitializeStates(StateMachine.BaseState&)", "e72be5f2d127b7a3a2362cb51c37c77e89e8a4df"),
        new(typeof(IdleMonitor), "InitializeStates(StateMachine.BaseState&)", "1a64a103a806e7a88f9e51134681bfd9d821b01a"),
        new(typeof(MoveToSafetyChore.States), "InitializeStates(StateMachine.BaseState&)", "17d5fdb636b49e3942fb13457dcb63d38801a363"),
        new(typeof(SafeCellMonitor), "InitializeStates(StateMachine.BaseState&)", "ba30939582cc4532763ffb2250a03d175f2a6db7"),
        new(typeof(IdleStates), "InitializeStates(StateMachine.BaseState&)", "5b018cf4bf9b0df92febb4d544f9163cf30c18b6")
    ];

    public static readonly HashAlgorithm HashAlgorithm = SHA1.Create();

    public static readonly Dictionary<Type, Dictionary<string, MethodMetadata>> Metadata;

    static PatchesCompatibilityMetadata() {
        Metadata = methodsMetadata.GroupBy(it => it.Type)
            .ToDictionary(
                it => it.Key,
                grouping => grouping.ToDictionary(it => it.Signature, it => it)
            );
    }

    public record MethodMetadata(Type Type, string Signature, string Hash);

}
