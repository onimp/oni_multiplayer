using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerMod.Multiplayer.Components;

public class CursorComponent : MonoBehaviour {

    private record TimedCursor(Vector2 Position, long Time);

    private TimedCursor previous = null!;
    private TimedCursor current = null!;

    private Camera camera = null!;
    private TextMeshProUGUI textComponent = null!;

    private bool initialized;

    public Vector3 Position {
        set {
            var ticks = System.DateTime.Now.Ticks;
            previous = new TimedCursor(value, ticks);
            current = new TimedCursor(value, ticks);
        }
    }

    public string CursorText { get; set; } = null!;

    public void Trace(Vector2 position) {
        previous = current;
        current = new TimedCursor(position, System.DateTime.Now.Ticks);
    }

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

        var imageComponent = imageGameObject.AddComponent<Image>();
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

        gameObject.transform.position = camera.WorldToScreenPoint(GetCurrentPosition());
        textComponent.text = CursorText;
    }

    private Vector2 GetCurrentPosition() {
        float updateDelta = current.Time - previous.Time;
        var timeDiff = (System.DateTime.Now.Ticks - current.Time) / updateDelta;
        return Vector2.Lerp(previous.Position, current.Position, timeDiff);
    }
}
