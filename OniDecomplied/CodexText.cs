// Decompiled with JetBrains decompiler
// Type: CodexText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodexText : CodexWidget<CodexText>
{
  public string text { get; set; }

  public string messageID { get; set; }

  public CodexTextStyle style { get; set; }

  public string stringKey
  {
    set => this.text = StringEntry.op_Implicit(Strings.Get(value));
    get => "--> " + (this.text ?? "NULL");
  }

  public CodexText() => this.style = CodexTextStyle.Body;

  public CodexText(string text, CodexTextStyle style = CodexTextStyle.Body, string id = null)
  {
    this.text = text;
    this.style = style;
    if (id == null)
      return;
    this.messageID = id;
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

  public override void Configure(
    GameObject contentGameObject,
    Transform displayPane,
    Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
  {
    this.ConfigureLabel(contentGameObject.GetComponent<LocText>(), textStyles);
    this.ConfigurePreferredLayout(contentGameObject);
  }
}
