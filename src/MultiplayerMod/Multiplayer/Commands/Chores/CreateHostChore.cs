using System;
using System.Linq;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Chores;

[Serializable]
public class CreateHostChore : MultiplayerCommand {

    private readonly Type choreType;
    private readonly object?[] args;

    public CreateHostChore(CreateNewChoreArgs args) {
        this.choreType = args.ChoreType;
        this.args = args.Args.Select(
            obj =>
                obj switch {
                    GameObject gameObject => gameObject.GetGridReference(),
                    KMonoBehaviour kMonoBehaviour => kMonoBehaviour.GetReference(),
                    _ => obj
                }
        ).ToArray();
    }

    public override void Execute(MultiplayerCommandContext context) {
        var args = this.args.Select(
            arg =>
                arg switch {
                    GameObjectReference gameObjectReference => gameObjectReference.GetGameObject(),
                    ComponentReference reference => reference.GetComponent(),
                    _ => arg
                }
        ).ToArray();

        Activator.CreateInstance(choreType, args);
    }
}
