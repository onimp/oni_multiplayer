// Decompiled with JetBrains decompiler
// Type: PresUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

public static class PresUtil
{
  public static Promise MoveAndFade(
    RectTransform rect,
    Vector2 targetAnchoredPosition,
    float targetAlpha,
    float duration,
    Easing.EasingFn easing = null)
  {
    CanvasGroup canvasGroup = Util.FindOrAddComponent<CanvasGroup>((Component) rect);
    return Util.FindOrAddComponent<CoroutineRunner>((Component) rect).Run((IEnumerator) Updater.Parallel(Updater.Ease((Action<float>) (f => canvasGroup.alpha = f), canvasGroup.alpha, targetAlpha, duration, easing), Updater.Ease((Action<Vector2>) (v2 => rect.anchoredPosition = v2), rect.anchoredPosition, targetAnchoredPosition, duration, easing)));
  }

  public static Promise OffsetFromAndFade(
    RectTransform rect,
    Vector2 offset,
    float targetAlpha,
    float duration,
    Easing.EasingFn easing = null)
  {
    Vector2 anchoredPosition = rect.anchoredPosition;
    return PresUtil.MoveAndFade(rect, Vector2.op_Addition(offset, anchoredPosition), targetAlpha, duration, easing);
  }

  public static Promise OffsetToAndFade(
    RectTransform rect,
    Vector2 offset,
    float targetAlpha,
    float duration,
    Easing.EasingFn easing = null)
  {
    Vector2 anchoredPosition = rect.anchoredPosition;
    rect.anchoredPosition = Vector2.op_Addition(offset, anchoredPosition);
    return PresUtil.MoveAndFade(rect, anchoredPosition, targetAlpha, duration, easing);
  }
}
