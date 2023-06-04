using System;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Core.Logging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Multiplayer.Commands.Screens.UserMenu;

[Serializable]
public class ClickUserMenuButton : IMultiplayerCommand {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(ClickUserMenuButton));

    private int instanceId;
    private Type actionDeclaringType;
    private string actionName;

    public ClickUserMenuButton(GameObject gameObject, System.Action action) {
        instanceId = gameObject.GetComponent<KPrefabID>().InstanceID;
        actionDeclaringType = action.Method.DeclaringType;
        actionName = action.Method.Name;
    }

    public void Execute() {
        var kPrefabID = Object.FindObjectsOfType<KPrefabID>().FirstOrDefault(a => a.InstanceID == instanceId);
        if (kPrefabID == null) return;

        var actionObject = kPrefabID.GetComponent(actionDeclaringType);
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
