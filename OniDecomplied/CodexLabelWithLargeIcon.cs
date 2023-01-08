// Decompiled with JetBrains decompiler
// Type: CodexLabelWithLargeIcon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexLabelWithLargeIcon : CodexLabelWithIcon
{
  public string linkID { get; set; }

  public CodexLabelWithLargeIcon()
  {
  }

  public CodexLabelWithLargeIcon(
    string text,
    CodexTextStyle style,
    Tuple<Sprite, Color> coloredSprite,
    string targetEntrylinkID)
    : base(text, style, coloredSprite, 128, 128)
  {
    this.icon = new CodexImage(128, 128, coloredSprite);
    this.label = new CodexText(text, style);
    this.linkID = targetEntrylinkID;
  }

  public override void Configure(
    GameObject contentGameObject,
    Transform displayPane,
    Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
  {
    this.icon.ConfigureImage(contentGameObject.GetComponentsInChildren<Image>()[1]);
    if (this.icon.preferredWidth != -1 && this.icon.preferredHeight != -1)
    {
      LayoutElement component = ((Component) contentGameObject.GetComponentsInChildren<Image>()[1]).GetComponent<LayoutElement>();
      component.minWidth = (float) this.icon.preferredHeight;
      component.minHeight = (float) this.icon.preferredWidth;
      component.preferredHeight = (float) this.icon.preferredHeight;
      component.preferredWidth = (float) this.icon.preferredWidth;
    }
    this.label.text = STRINGS.UI.StripLinkFormatting(this.label.text);
    this.label.ConfigureLabel(contentGameObject.GetComponentInChildren<LocText>(), textStyles);
    contentGameObject.GetComponent<KButton>().ClearOnClick();
    contentGameObject.GetComponent<KButton>().onClick += (System.Action) (() => ManagementMenu.Instance.codexScreen.ChangeArticle(this.linkID, targetPosition: new Vector3()));
  }
}
