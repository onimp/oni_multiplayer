using System;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Multiplayer.Objects;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Screens.UserMenu;

[Serializable]
public class InteractWithSideScreen : IMultiplayerCommand {

    private readonly Type sideScreenType;
    private readonly string fieldName;
    private readonly MultiplayerReference multiplayerReference;

    public InteractWithSideScreen(SideScreenContent sideScreen, GameObject target, FieldInfo field) {
        sideScreenType = sideScreen.GetType();
        fieldName = field.Name;
        multiplayerReference = target.GetMultiplayerReference();
    }

    public void Execute() {
        // Necessary for Click and other input methods go through.
        var initialHasFocus = KInputManager.hasFocus;
        KInputManager.hasFocus = true;
        var field = sideScreenType.GetField(
            fieldName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
        );
        var value = field!.GetValue(SideScreenContent);
        switch (value) {
            case KToggle toggle:
                toggle.Click();
                break;
            case KButton button:
                button.SignalClick(KKeyCode.Mouse2);
                break;

            // TODO handle other cases.
        }
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
            callMethod(instance, "OnPrefabInit");
            callMethod(instance, "OnSpawn");
            return instance;
        }
    }

    private void callMethod(SideScreenContent sideScreenContent, string methodName) {
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
