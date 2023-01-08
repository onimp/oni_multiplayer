// Decompiled with JetBrains decompiler
// Type: ContentContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization.Converters;
using System.Collections.Generic;
using UnityEngine;

public class ContentContainer
{
  public GameObject go;

  public ContentContainer() => this.content = new List<ICodexWidget>();

  public ContentContainer(List<ICodexWidget> content, ContentContainer.ContentLayout contentLayout)
  {
    this.content = content;
    this.contentLayout = contentLayout;
  }

  public List<ICodexWidget> content { get; set; }

  public string lockID { get; set; }

  [StringEnumConverter]
  public ContentContainer.ContentLayout contentLayout { get; set; }

  public bool showBeforeGeneratedContent { get; set; }

  public enum ContentLayout
  {
    Vertical,
    Horizontal,
    Grid,
    GridTwoColumn,
    GridTwoColumnTall,
  }
}
