using System;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Chores;

[Serializable]
public class CreateHostChore : MultiplayerCommand {

    public readonly MultiplayerId ChoreId;
    public readonly Type ChoreType;
    public readonly object?[] Args;

    public CreateHostChore(CreateNewChoreArgs args) {
        ChoreId = args.ChoreId;
        ChoreType = args.ChoreType;
        Args = SpecialWrap(args.Args);
        Args = Args.Select(WrapObject).ToArray();
    }

    public override void Execute(MultiplayerCommandContext context) {
        var args = Args.Select(UnWrapObject).ToArray();
        args = SpecialUnWrap(args);

        var chore = (Chore) ChoreType.GetConstructors()[0].Invoke(args);
        chore.Register(ChoreId);
    }

    private object?[] SpecialWrap(object?[] args) {
        if (ChoreType == typeof(MoveChore)) {
            args[2] = ((Func<MoveChore.StatesInstance, int>) args[2]!).Invoke(null!);
        }
        return args;
    }

    private object?[] SpecialUnWrap(object?[] args) {
        if (ChoreType == typeof(MoveChore)) {
            var targetCell = (int) args[2]!;
            args[2] = new Func<MoveChore.StatesInstance, int>(_ => targetCell);
        }
        return args;
    }

    private object? WrapObject(object? obj) {
        return obj switch {
            GameObject gameObject => gameObject.GetReference(),
            KMonoBehaviour kMonoBehaviour => kMonoBehaviour.GetReference(),
            Delegate action => new DelegateRef(action.GetType(), WrapObject(action.Target), action.Method),
            _ => obj
        };
    }

    private object? UnWrapObject(object? obj) {
        return obj switch {
            GameObjectReference gameObjectReference => gameObjectReference.GetGameObject(),
            ComponentReference reference => reference.GetComponent(),
            DelegateRef delegateRef => Delegate.CreateDelegate(
                delegateRef.DelegateType,
                UnWrapObject(delegateRef.Target),
                delegateRef.MethodInfo
            ),
            _ => obj
        };
    }

    [Serializable]
    public record DelegateRef(
        Type DelegateType,
        object? Target,
        MethodInfo MethodInfo
    );
}
