// Decompiled with JetBrains decompiler
// Type: ImageAspectRatioFitter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof (RectTransform))]
[DisallowMultipleComponent]
public class ImageAspectRatioFitter : AspectRatioFitter
{
  [SerializeField]
  private Image targetImage;

  private void UpdateAspectRatio()
  {
    Rect rect = this.targetImage.sprite.rect;
    double width = (double) ((Rect) ref rect).width;
    rect = this.targetImage.sprite.rect;
    double height = (double) ((Rect) ref rect).height;
    this.aspectRatio = (float) (width / height);
  }

  protected virtual void OnTransformParentChanged()
  {
    this.UpdateAspectRatio();
    base.OnTransformParentChanged();
  }

  protected virtual void OnRectTransformDimensionsChange()
  {
    this.UpdateAspectRatio();
    base.OnRectTransformDimensionsChange();
  }
}
