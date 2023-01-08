// Decompiled with JetBrains decompiler
// Type: CategoryEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class CategoryEntry : CodexEntry
{
  public List<CodexEntry> entriesInCategory = new List<CodexEntry>();

  public bool largeFormat { get; set; }

  public bool sort { get; set; }

  public CategoryEntry(
    string category,
    List<ContentContainer> contentContainers,
    string name,
    List<CodexEntry> entriesInCategory,
    bool largeFormat,
    bool sort)
    : base(category, contentContainers, name)
  {
    this.entriesInCategory = entriesInCategory;
    this.largeFormat = largeFormat;
    this.sort = sort;
  }
}
