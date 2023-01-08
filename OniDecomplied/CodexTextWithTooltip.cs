// Decompiled with JetBrains decompiler
// Type: CodexTextWithTooltip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodexTextWithTooltip : CodexWidget<CodexTextWithTooltip>
{
  public string text { get; set; }

  public string tooltip { get; set; }

  public CodexTextStyle style { get; set; }

  public string stringKey
  {
    set => this.text = StringEntry.op_Implicit(Strings.Get(value));
    get => "--> " + (this.text ?? "NULL");
  }

  public CodexTextWithTooltip() => this.style = CodexTextStyle.Body;

  public CodexTextWithTooltip(string text, string tooltip, CodexTextStyle style = CodexTextStyle.Body)
  {
    this.text = text;
    this.style = style;
    this.tooltip = tooltip;
  }

  public void ConfigureLabel(
    LocText label,
    Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
  {
    ((Component) label).gameObject.SetActive(true);
    label.AllowLinks = this.style == CodexTextStyle.Body;
    label.textStyleSetting = textStyles[this.style];
    ((TMP_Text) label).text = this.text;
    label.ApplySettings();
  }

  public void ConfigureTooltip(ToolTip tooltip) => tooltip.SetSimpleTooltip(this.tooltip);

  public override void Configure(
    GameObject contentGameObject,
    Transform displayPane,
    Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
  {
    this.ConfigureLabel(contentGameObject.GetComponent<LocText>(), textStyles);
    this.ConfigureTooltip(contentGameObject.GetComponent<ToolTip>());
    this.ConfigurePreferredLayout(contentGameObject);
  }
}
