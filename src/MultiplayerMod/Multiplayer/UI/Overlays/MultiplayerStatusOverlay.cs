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

    // When set, the overlay text is rebuilt from this supplier every frame (see MultiplayerStatusOverlayTicker),
    // so live content such as the per-player hard-sync timers keeps ticking.
    private Func<string>? textSupplier;

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

        AttachTicker();
    }

    // The wrapper object survives scene reloads, but ONI's LoadingOverlay GameObject is torn down on a
    // scene transition and only rebuilt by the scheduler.Run(CreateOverlay) queued in OnPostLoadScene.
    // If Show() lands in that window (e.g. LoadWorld arriving mid-transition when joining off the main
    // menu) LoadingOverlay.instance is null, so rebuild the surface before touching it.
    private void EnsureSurface() {
        if (LoadingOverlay.instance == null)
            CreateOverlay();
        else
            AttachTicker();
    }

    // Drive a per-frame refresh from the supplier. The overlay object survives scene reloads but the
    // underlying LoadingOverlay GameObject is recreated, so (re)attach the ticker whenever we rebuild.
    private void AttachTicker() {
        if (textSupplier == null || LoadingOverlay.instance == null)
            return;

        var host = LoadingOverlay.instance.gameObject;
        var ticker = host.GetComponent<MultiplayerStatusOverlayTicker>() ?? host.AddComponent<MultiplayerStatusOverlayTicker>();
        ticker.Supplier = textSupplier;
        ticker.Apply = value => Text = value;
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
        overlay.textSupplier = null;
        overlay.EnsureSurface();
        Text = text;
    }

    // Show a live overlay whose text is rebuilt every frame from the supplier.
    public static void Show(Func<string> textSupplier) {
        overlay ??= new MultiplayerStatusOverlay();
        overlay.textSupplier = textSupplier;
        overlay.EnsureSurface();
        Text = textSupplier();
    }

    public static void Close() {
        overlay?.Dispose();
        overlay = null;
    }

}
