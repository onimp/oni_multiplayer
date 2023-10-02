using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerMod.Multiplayer.Components;

public class CursorComponent : MonoBehaviour {

    private Camera camera = null!;
    private Image imageComponent = null!;
    private TextMeshProUGUI textComponent = null!;

    private bool initialized;

    public readonly SmoothCursor CursorWithinWorld = new SmoothCursor();
    public readonly SmoothCursor CursorWithinScreen = new SmoothCursor();
    public string PlayerName { get; set; } = null!;

    public string? ScreenName { get; set; }
    public Type? ScreenType { get; set; }

    private void OnEnable() {
        var cursorTexture = Assets.GetTexture("cursor_arrow");

        CreateCursorGameObject(cursorTexture);
        CreateTextGameObject(cursorTexture);

        var parent = GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
        gameObject.SetLayerRecursively(LayerMask.NameToLayer("UI"));
        gameObject.transform.SetParent(parent.transform, false);

        camera = GameScreenManager.Instance.GetCamera(GameScreenManager.UIRenderTarget.ScreenSpaceCamera);

        initialized = true;
    }

    private void CreateCursorGameObject(Texture2D cursorTexture) {
        var imageGameObject = new GameObject { transform = { parent = gameObject.transform } };
        var rectTransform = imageGameObject.AddComponent<RectTransform>();
        rectTransform.transform.parent = imageGameObject.transform;
        rectTransform.sizeDelta = new Vector2(cursorTexture.width, cursorTexture.height);
        rectTransform.pivot = new Vector2(0, 1); // Align to top left corner.

        imageComponent = imageGameObject.AddComponent<Image>();
        imageComponent.sprite = Sprite.Create(
            cursorTexture,
            new Rect(0, 0, cursorTexture.width, cursorTexture.height),
            Vector2.zero
        );
        imageComponent.raycastTarget = false;
    }

    private void CreateTextGameObject(Texture2D cursorTexture) {
        var textGameObject = new GameObject { transform = { parent = gameObject.transform } };

        var rectTransform = textGameObject.AddComponent<RectTransform>();
        rectTransform.transform.parent = textGameObject.transform;
        rectTransform.sizeDelta = new Vector2(50, 50);
        rectTransform.pivot = new Vector2(0, 1); // Align to top left corner.
        rectTransform.position = new Vector3(cursorTexture.width, -cursorTexture.height, 0);

        textComponent = textGameObject.AddComponent<TextMeshProUGUI>();
        textComponent.fontSize = 14;
        textComponent.font = Localization.FontAsset;
        textComponent.color = Color.white;
        textComponent.raycastTarget = false;
        textComponent.enableWordWrapping = false;
    }

    private void Update() {
        if (!initialized)
            return;

        var currentScreen = KScreenManager.Instance.screenStack.FirstOrDefault(screen => screen.mouseOver);
        var otherClientScreen =
            KScreenManager.Instance.screenStack.FirstOrDefault(screen => screen.GetType() == ScreenType);
        var isOnTheSameScreen = currentScreen?.GetType() == ScreenType;

        // If we see a screen where other player is - show cursor within that screen.
        var showScreenOrWorldCursor =
            CursorWithinScreen.CurrentPosition != null && (otherClientScreen?.isActive ?? false);
        gameObject.transform.position = showScreenOrWorldCursor
            ? ScreenToWorld(otherClientScreen!, (Vector3) CursorWithinScreen.CurrentPosition!)
            : camera.WorldToScreenPoint((Vector3) CursorWithinWorld.CurrentPosition!);
        textComponent.text = PlayerName + (isOnTheSameScreen ? "" : $" ({ScreenName ?? "World"})");
        imageComponent.color = textComponent.color = isOnTheSameScreen ? Color.white : new Color(1, 1, 1, 0.5f);
    }

    private static Vector3 ScreenToWorld(KScreen screen, Vector3 pos) {
        var screenRectTransform = screen.transform as RectTransform;
        if (screenRectTransform == null) return Vector3.zero;

        return new Vector2(
            screenRectTransform.position.x + pos.x * screenRectTransform.rect.width,
            screenRectTransform.position.y + pos.y * screenRectTransform.rect.height
        );
    }

    public class SmoothCursor {
        private record TimedCursor(Vector2? Position, long Time);

        private TimedCursor previous = null!;
        private TimedCursor current = null!;

        public void SetPosition(Vector2? position) {
            var ticks = System.DateTime.Now.Ticks;
            previous = new TimedCursor(position, ticks);
            current = new TimedCursor(position, ticks);
        }

        public void Trace(Vector2? position) {
            previous = current;
            current = new TimedCursor(position, System.DateTime.Now.Ticks);
        }

        public Vector3? CurrentPosition {
            get {
                if (previous.Position == null) return current.Position;
                if (current.Position == null) return null;

                float updateDelta = current.Time - previous.Time;
                var timeDiff = (System.DateTime.Now.Ticks - current.Time) / updateDelta;
                return Vector2.Lerp((Vector2) previous.Position, (Vector2) current.Position, timeDiff);
            }
        }
    }
}
