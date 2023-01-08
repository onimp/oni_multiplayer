// Decompiled with JetBrains decompiler
// Type: UnityMouseCatcherUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class UnityMouseCatcherUI
{
  private static Canvas m_instance_canvas;

  public static Canvas ManifestCanvas()
  {
    if (Object.op_Inequality((Object) UnityMouseCatcherUI.m_instance_canvas, (Object) null) && Object.op_Implicit((Object) UnityMouseCatcherUI.m_instance_canvas))
      return UnityMouseCatcherUI.m_instance_canvas;
    GameObject gameObject1 = new GameObject("UnityMouseCatcherUI Canvas");
    Object.DontDestroyOnLoad((Object) gameObject1);
    Canvas canvas = gameObject1.AddComponent<Canvas>();
    canvas.renderMode = (RenderMode) 0;
    canvas.sortingOrder = (int) short.MaxValue;
    canvas.pixelPerfect = false;
    UnityMouseCatcherUI.m_instance_canvas = canvas;
    gameObject1.AddComponent<GraphicRaycaster>();
    GameObject gameObject2 = new GameObject("ImGui Consume Input", new System.Type[1]
    {
      typeof (RectTransform)
    });
    gameObject2.transform.SetParent(gameObject1.transform, false);
    RectTransform component = gameObject2.GetComponent<RectTransform>();
    component.anchorMin = Vector2.zero;
    component.anchorMax = Vector2.one;
    component.sizeDelta = Vector2.zero;
    component.anchoredPosition = Vector2.zero;
    Image image = gameObject2.AddComponent<Image>();
    image.sprite = Resources.Load<Sprite>("1x1_white");
    ((Graphic) image).color = new Color(1f, 1f, 1f, 0.0f);
    ((Graphic) image).raycastTarget = true;
    return UnityMouseCatcherUI.m_instance_canvas;
  }

  public static void SetEnabled(bool is_enabled)
  {
    Canvas canvas = UnityMouseCatcherUI.ManifestCanvas();
    if (((Component) canvas).gameObject.activeSelf != is_enabled)
      ((Component) canvas).gameObject.SetActive(is_enabled);
    if (((Behaviour) canvas).enabled == is_enabled)
      return;
    ((Behaviour) canvas).enabled = is_enabled;
  }
}
