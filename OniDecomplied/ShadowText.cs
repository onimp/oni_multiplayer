// Decompiled with JetBrains decompiler
// Type: ShadowText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ShadowText : ShadowRect
{
  private UnityEngine.UI.Text shadowText;
  private UnityEngine.UI.Text mainText;

  protected override void MatchRect()
  {
    if (Object.op_Equality((Object) this.RectMain, (Object) null) || Object.op_Equality((Object) this.RectShadow, (Object) null))
      return;
    if (Object.op_Equality((Object) this.shadowText, (Object) null))
      this.shadowText = ((Component) this.RectShadow).GetComponent<UnityEngine.UI.Text>();
    if (Object.op_Equality((Object) this.mainText, (Object) null))
      this.mainText = ((Component) this.RectMain).GetComponent<UnityEngine.UI.Text>();
    if (Object.op_Equality((Object) this.shadowText, (Object) null) || Object.op_Equality((Object) this.mainText, (Object) null))
      return;
    if (Object.op_Inequality((Object) this.shadowText.font, (Object) this.mainText.font))
      this.shadowText.font = this.mainText.font;
    if (this.shadowText.fontSize != this.mainText.fontSize)
      this.shadowText.fontSize = this.mainText.fontSize;
    if (this.shadowText.alignment != this.mainText.alignment)
      this.shadowText.alignment = this.mainText.alignment;
    if ((double) this.shadowText.lineSpacing != (double) this.mainText.lineSpacing)
      this.shadowText.lineSpacing = this.mainText.lineSpacing;
    string str = Regex.Replace(this.mainText.text, "\\</?color\\b.*?\\>", string.Empty);
    if (this.shadowText.text != str)
      this.shadowText.text = str;
    if (Color.op_Inequality(((Graphic) this.shadowText).color, this.shadowColor))
      ((Graphic) this.shadowText).color = this.shadowColor;
    if (this.shadowText.horizontalOverflow != this.mainText.horizontalOverflow)
      this.shadowText.horizontalOverflow = this.mainText.horizontalOverflow;
    if (this.shadowText.verticalOverflow != this.mainText.verticalOverflow)
      this.shadowText.verticalOverflow = this.mainText.verticalOverflow;
    base.MatchRect();
  }
}
