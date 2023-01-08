// Decompiled with JetBrains decompiler
// Type: CodexImageLayoutMB
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CodexImageLayoutMB : UIBehaviour
{
  public RectTransform rectTransform;
  public LayoutElement layoutElement;
  public Image image;

  protected virtual void OnRectTransformDimensionsChange()
  {
    base.OnRectTransformDimensionsChange();
    if (this.image.preserveAspect && Object.op_Inequality((Object) this.image.sprite, (Object) null) && Object.op_Implicit((Object) this.image.sprite))
    {
      Rect rect = this.image.sprite.rect;
      double height = (double) ((Rect) ref rect).height;
      rect = this.image.sprite.rect;
      double width = (double) ((Rect) ref rect).width;
      this.layoutElement.preferredHeight = (float) (height / width) * this.rectTransform.sizeDelta.x;
      this.layoutElement.minHeight = this.layoutElement.preferredHeight;
    }
    else
    {
      this.layoutElement.preferredHeight = -1f;
      this.layoutElement.preferredWidth = -1f;
      this.layoutElement.minHeight = -1f;
      this.layoutElement.minWidth = -1f;
      this.layoutElement.flexibleHeight = -1f;
      this.layoutElement.flexibleWidth = -1f;
      this.layoutElement.ignoreLayout = false;
    }
  }
}
