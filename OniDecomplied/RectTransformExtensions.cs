// Decompiled with JetBrains decompiler
// Type: RectTransformExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class RectTransformExtensions
{
  public static RectTransform Fill(this RectTransform rectTransform)
  {
    rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
    rectTransform.anchorMax = new Vector2(1f, 1f);
    rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
    rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
    return rectTransform;
  }

  public static RectTransform Fill(this RectTransform rectTransform, Padding padding)
  {
    rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
    rectTransform.anchorMax = new Vector2(1f, 1f);
    rectTransform.anchoredPosition = new Vector2(padding.left, padding.bottom);
    rectTransform.sizeDelta = new Vector2(-padding.right, -padding.top);
    return rectTransform;
  }

  public static RectTransform Pivot(this RectTransform rectTransform, float x, float y)
  {
    rectTransform.pivot = new Vector2(x, y);
    return rectTransform;
  }

  public static RectTransform Pivot(this RectTransform rectTransform, Vector2 pivot)
  {
    rectTransform.pivot = pivot;
    return rectTransform;
  }
}
