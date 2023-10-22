using System;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerMod.Multiplayer.UI.Overlays;

public class MultiplayerStatusOverlay {

    public static string Text {
        get => overlay?.text ?? "";
        set {
            if (overlay == null)
                return;

            overlay.text = value;
            overlay.textComponent.text = value;
        }
    }

    private LocText textComponent = null!;
    private string text = "";

    [InjectDependency, UsedImplicitly]
    private UnityTaskScheduler scheduler = null!;

    private RectTransform rect = null!;

    // ReSharper disable once InconsistentNaming
    private Func<float> GetScale = null!;

    private static MultiplayerStatusOverlay? overlay;

    private MultiplayerStatusOverlay() {
        SceneManager.sceneLoaded += OnPostLoadScene;
        ScreenResize.Instance.OnResize += OnResize;
        Dependencies.Get<IDependencyInjector>().Inject(this);
        CreateOverlay();
    }

    private void CreateOverlay() {
        LoadingOverlay.Load(() => { });
        textComponent = LoadingOverlay.instance.GetComponentInChildren<LocText>();
        textComponent.alignment = TextAlignmentOptions.Top;
        textComponent.margin = new Vector4(0, -21.0f, 0, 0);
        textComponent.text = text;

        GetScale = LoadingOverlay.instance.GetComponentInParent<KCanvasScaler>().GetCanvasScale;

        rect = textComponent.gameObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(Screen.width / GetScale(), 0);
    }

    private void OnResize() => rect.sizeDelta = new Vector2(Screen.width / GetScale(), 0);

    private void Dispose() {
        SceneManager.sceneLoaded -= OnPostLoadScene;
        ScreenResize.Instance.OnResize -= OnResize;
        LoadingOverlay.Clear();
    }

    private void OnPostLoadScene(Scene scene, LoadSceneMode mode) => scheduler.Run(CreateOverlay);

    public static void Show(string text) {
        overlay ??= new MultiplayerStatusOverlay();
        Text = text;
    }

    public static void Close() {
        overlay?.Dispose();
        overlay = null;
    }

}
