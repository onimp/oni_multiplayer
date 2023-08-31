using System;
using System.Reflection;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Screens.UserMenu;

[Serializable]
public class ClickUserMenuButton : MultiplayerCommand {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(ClickUserMenuButton));

    private GameObjectReference reference;
    private Type actionDeclaringType;
    private string actionName;

    public ClickUserMenuButton(GameObject gameObject, System.Action action) {
        reference = gameObject.GetMultiplayerReference();
        actionDeclaringType = action.Method.DeclaringType!;
        actionName = action.Method.Name;
    }

    public override void Execute(Runtime runtime) {
        try {
            var methodInfo = actionDeclaringType.GetMethod(
                actionName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                null,
                new Type[] { },
                new ParameterModifier[] { }
            );
            methodInfo?.Invoke(reference.GetComponent(actionDeclaringType), new object[] { });
        } catch (Exception e) {
            log.Error(e.ToString());
        }
    }
}
