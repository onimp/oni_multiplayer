// Decompiled with JetBrains decompiler
// Type: CodexCollapsibleHeader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodexCollapsibleHeader : CodexWidget<CodexCollapsibleHeader>
{
  private ContentContainer contents;
  private string label;

  public CodexCollapsibleHeader(string label, ContentContainer contents)
  {
    this.label = label;
    this.contents = contents;
  }

  public override void Configure(
    GameObject contentGameObject,
    Transform displayPane,
    Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
  {
    HierarchyReferences component = contentGameObject.GetComponent<HierarchyReferences>();
    LocText reference1 = component.GetReference<LocText>("Label");
    ((TMP_Text) reference1).text = this.label;
    reference1.textStyleSetting = textStyles[CodexTextStyle.Subtitle];
    reference1.ApplySettings();
    MultiToggle reference2 = component.GetReference<MultiToggle>("ExpandToggle");
    reference2.ChangeState(1);
    reference2.onClick = (System.Action) (() => this.ToggleCategoryOpen(contentGameObject, !this.contents.go.activeSelf));
  }

  private void ToggleCategoryOpen(GameObject header, bool open)
  {
    header.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("ExpandToggle").ChangeState(open ? 1 : 0);
    this.contents.go.SetActive(open);
  }
}
