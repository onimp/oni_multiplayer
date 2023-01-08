// Decompiled with JetBrains decompiler
// Type: CodexWidget`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CodexWidget<SubClass> : ICodexWidget
{
  public int preferredWidth { get; set; }

  public int preferredHeight { get; set; }

  protected CodexWidget()
  {
    this.preferredWidth = -1;
    this.preferredHeight = -1;
  }

  protected CodexWidget(int preferredWidth, int preferredHeight)
  {
    this.preferredWidth = preferredWidth;
    this.preferredHeight = preferredHeight;
  }

  public abstract void Configure(
    GameObject contentGameObject,
    Transform displayPane,
    Dictionary<CodexTextStyle, TextStyleSetting> textStyles);

  protected void ConfigurePreferredLayout(GameObject contentGameObject)
  {
    LayoutElement componentInChildren = contentGameObject.GetComponentInChildren<LayoutElement>();
    componentInChildren.preferredHeight = (float) this.preferredHeight;
    componentInChildren.preferredWidth = (float) this.preferredWidth;
  }
}
