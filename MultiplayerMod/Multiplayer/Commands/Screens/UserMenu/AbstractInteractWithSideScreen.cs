using System;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Game.UI.Screens.Events;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Commands.Screens.UserMenu;

[Serializable]
public abstract class AbstractInteractWithSideScreen : IMultiplayerCommand {

    private readonly Type sideScreenType;
    private readonly string fieldName;
    private readonly MultiplayerReference multiplayerReference;

    protected AbstractInteractWithSideScreen(SideScreenEvents.SideScreenFieldArgs sideScreenFieldArgs) {
        sideScreenType = sideScreenFieldArgs.SideScreenContent.GetType();
        fieldName = sideScreenFieldArgs.FieldInfo.Name;
        multiplayerReference = sideScreenFieldArgs.Target.GetMultiplayerReference();
    }

    protected abstract void HandleValue(object fieldValue);

    public void Execute() {
        // Necessary for Click and other input methods go through.
        var initialHasFocus = KInputManager.hasFocus;
        KInputManager.hasFocus = true;
        var field = sideScreenType.GetField(
            fieldName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
        );
        var value = field!.GetValue(SideScreenContent);
        HandleValue(value);
        KInputManager.hasFocus = initialHasFocus;
    }

    private SideScreenContent SideScreenContent {
        get {
            var sideScreen =
                DetailsScreen.Instance.sideScreens.Single(screen => screen.screenPrefab.GetType() == sideScreenType);
            var instance = Util.KInstantiateUI<SideScreenContent>(sideScreen.screenPrefab.gameObject);
            var target = multiplayerReference.GetGameObject();
            if (target != null) {
                instance.SetTarget(target);
            }
            CallMethod(instance, "OnPrefabInit");
            CallMethod(instance, "OnSpawn");
            return instance;
        }
    }

    private void CallMethod(SideScreenContent sideScreenContent, string methodName) {
        var methodInfo = sideScreenType.GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
            null,
            new Type[] { },
            new ParameterModifier[] { }
        );
        methodInfo?.Invoke(sideScreenContent, new object[] { });
    }

}
