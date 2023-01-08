// Decompiled with JetBrains decompiler
// Type: ShadowImage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class ShadowImage : ShadowRect
{
  private Image shadowImage;
  private Image mainImage;

  protected override void MatchRect()
  {
    base.MatchRect();
    if (Object.op_Equality((Object) this.RectMain, (Object) null) || Object.op_Equality((Object) this.RectShadow, (Object) null))
      return;
    if (Object.op_Equality((Object) this.shadowImage, (Object) null))
      this.shadowImage = ((Component) this.RectShadow).GetComponent<Image>();
    if (Object.op_Equality((Object) this.mainImage, (Object) null))
      this.mainImage = ((Component) this.RectMain).GetComponent<Image>();
    if (Object.op_Equality((Object) this.mainImage, (Object) null))
    {
      if (!Object.op_Inequality((Object) this.shadowImage, (Object) null))
        return;
      ((Graphic) this.shadowImage).color = Color.clear;
    }
    else
    {
      if (Object.op_Equality((Object) this.shadowImage, (Object) null))
        return;
      if (Object.op_Inequality((Object) this.shadowImage.sprite, (Object) this.mainImage.sprite))
        this.shadowImage.sprite = this.mainImage.sprite;
      if (!Color.op_Inequality(((Graphic) this.shadowImage).color, this.shadowColor))
        return;
      if (Object.op_Inequality((Object) this.shadowImage.sprite, (Object) null))
        ((Graphic) this.shadowImage).color = this.shadowColor;
      else
        ((Graphic) this.shadowImage).color = Color.clear;
    }
  }
}
