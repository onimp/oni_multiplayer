using System;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.State;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Multiplayer.Commands.Screens.UserMenu;

[Serializable]
public class ClickUserMenuButton : IMultiplayerCommand {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(ClickUserMenuButton));

    private MultiplayerId multiplayerId;
    private Type actionDeclaringType;
    private string actionName;

    public ClickUserMenuButton(GameObject gameObject, System.Action action) {
        multiplayerId = gameObject.GetComponent<MultiplayerInstance>().Id;
        actionDeclaringType = action.Method.DeclaringType;
        actionName = action.Method.Name;
    }

    public void Execute() {
        var gameObject = MultiplayerGame.Objects[multiplayerId];
        if (gameObject == null) return;

        var actionObject = gameObject.GetComponent(actionDeclaringType);
        if (actionObject == null) return;

        try {
            var methodInfo = actionDeclaringType.GetMethod(
                actionName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                null,
                new Type[] { },
                new ParameterModifier[] { }
            );
            methodInfo?.Invoke(actionObject, new object[] { });
        } catch (Exception e) {
            log.Error(e.ToString);
        }
    }
}
