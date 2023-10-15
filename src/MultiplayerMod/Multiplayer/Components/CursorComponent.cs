using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MultiplayerMod.Multiplayer.Components;

public class CursorComponent : MonoBehaviour {

    // Perhaps we should provide a setting to enable this feature later on.
    private const bool ENABLE_CURSORS_RELATIVE_TO_GUI = false;

    private Camera camera = null!;
    private Image imageComponent = null!;
    private TextMeshProUGUI textComponent = null!;

    private bool initialized;

    public readonly SmoothCursor CursorWithinWorld = new();
    public readonly SmoothCursor CursorWithinScreen = new();
    public string PlayerName { get; set; } = null!;

    public string? ScreenName { get; set; }
    public Type? ScreenType { get; set; }

    private void OnEnable() {
        var parent = GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);

        var cursorTexture = Assets.GetTexture("cursor_arrow");
        var canvas = CreateCanvas(parent);
        CreateCursorGameObject(gameObject, cursorTexture);
        CreateTextGameObject(gameObject, cursorTexture);

        gameObject.transform.SetParent(canvas.transform, false);
        gameObject.SetLayerRecursively(LayerMask.NameToLayer("UI"));

        camera = GameScreenManager.Instance.GetCamera(GameScreenManager.UIRenderTarget.ScreenSpaceCamera);

        initialized = true;
    }

    private GameObject CreateCanvas(GameObject parent) {
        var canvasGameObject = new GameObject { transform = { parent = parent.transform } };
        var canvas = canvasGameObject.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 100;
        return canvasGameObject;
    }

    private void CreateCursorGameObject(GameObject parent, Texture2D cursorTexture) {
        var imageGameObject = new GameObject { transform = { parent = parent.transform } };
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

    private void CreateTextGameObject(GameObject parent, Texture2D cursorTexture) {
        var textGameObject = new GameObject { transform = { parent = parent.transform } };

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

        var otherClientScreen =
            KScreenManager.Instance.screenStack.FirstOrDefault(screen => screen.GetType() == ScreenType);

        // If we see a screen where other player is - show cursor within that screen.
        var showScreenOrWorldCursor =
            CursorWithinScreen.CurrentPosition != null && (otherClientScreen?.isActive ?? false);
        gameObject.transform.position = showScreenOrWorldCursor && ENABLE_CURSORS_RELATIVE_TO_GUI
            ? ScreenToWorld(otherClientScreen!, (Vector3) CursorWithinScreen.CurrentPosition!)
            : camera.WorldToScreenPoint((Vector3) CursorWithinWorld.CurrentPosition!);
        var screenUnderCursor = FindScreenUnderCursor(gameObject.transform.position);
        var showScreeName = screenUnderCursor?.GetType() != ScreenType;
        textComponent.text = PlayerName + (showScreeName ? $" ({ScreenName ?? "World"})" : "");
        imageComponent.color = textComponent.color = showScreeName ? new Color(1, 1, 1, 0.5f) : Color.white;
    }

    private KScreen? FindScreenUnderCursor(Vector2 cursor) {
        var eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem == null)
            return null;

        var eventData = new PointerEventData(eventSystem) {
            position = cursor
        };

        var results = new List<RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);
        if (results.Count == 0) return null;

        return KScreenManager.Instance.screenStack.FirstOrDefault(
            screen => IsParentOf(screen.gameObject, results[0].gameObject)
        );
    }

    private static bool IsParentOf(GameObject potentialParent, GameObject potentialChild) {
        var current = potentialChild.transform;
        while (current != null) {
            if (current.gameObject == potentialParent) {
                return true;
            }
            current = current.parent;
        }

        return false;
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
