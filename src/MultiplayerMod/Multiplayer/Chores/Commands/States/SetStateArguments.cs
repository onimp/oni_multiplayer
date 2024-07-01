using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Game.Chores.States;
using MultiplayerMod.Game.NameOf;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.States;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Chores.Commands.States;

[Serializable]
public class SetStateArguments : MultiplayerCommand {

    public readonly MultiplayerId ChoreId;

    public readonly Dictionary<int, object?> Args;

    public SetStateArguments(ChoreTransitStateArgs transitData) {
        ChoreId = transitData.Chore.MultiplayerId();
        Args = SpecialWrap(transitData.Chore, transitData.Args);
        Args = Args.ToDictionary(a => a.Key, a => ArgumentUtils.WrapObject(a.Value));
    }

    public override void Execute(MultiplayerCommandContext context) {
        var args = Args.ToDictionary(a => a.Key, a => ArgumentUtils.UnWrapObject(a.Value));
        var chore = context.Multiplayer.Objects.Get<Chore>(ChoreId)!;
        var smi = context.Runtime.Dependencies.Get<StatesManager>().GetSmi(chore);

        if (smi is RecoverBreathChore.StatesInstance recoverSmi) {
            var locator = recoverSmi.sm.locator.Get(recoverSmi);
            if (locator == null) {
                locator = ChoreHelpers.CreateLocator("RecoverBreathLocator", Vector3.zero);
                recoverSmi.sm.locator.Set(locator, recoverSmi);
            }
            locator.transform.position = (Vector3) args.Single().Value!;
            return;
        }

        foreach (var (parameterIndex, value) in args) {
            var parameterContext = smi.parameterContexts[parameterIndex];
            parameterContext.GetType().GetMethod(nameof(StateMachineMemberReference.Parameter.Context.Set))!.Invoke(
                parameterContext,
                new[] { value, smi, false }
            );
        }
    }

    private Dictionary<int, object?> SpecialWrap(Chore chore, Dictionary<int, object?> args) {
        if (chore.GetType() == typeof(RecoverBreathChore)) {
            var t = args.Single();
            return new Dictionary<int, object?> { { t.Key, ((GameObject) t.Value!).transform.position } };
        }
        return args;
    }
}
