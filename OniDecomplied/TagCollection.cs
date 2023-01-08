// Decompiled with JetBrains decompiler
// Type: TagCollection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class TagCollection : IReadonlyTags
{
  private HashSet<int> tags = new HashSet<int>();

  public TagCollection()
  {
  }

  public TagCollection(int[] initialTags)
  {
    for (int index = 0; index < initialTags.Length; ++index)
      this.tags.Add(initialTags[index]);
  }

  public TagCollection(string[] initialTags)
  {
    for (int index = 0; index < initialTags.Length; ++index)
      this.tags.Add(Hash.SDBMLower(initialTags[index]));
  }

  public TagCollection(TagCollection initialTags)
  {
    if (initialTags == null || initialTags.tags == null)
      return;
    this.tags.UnionWith((IEnumerable<int>) initialTags.tags);
  }

  public TagCollection Append(TagCollection others)
  {
    foreach (int tag in others.tags)
      this.tags.Add(tag);
    return this;
  }

  public void AddTag(string tag) => this.tags.Add(Hash.SDBMLower(tag));

  public void AddTag(int tag) => this.tags.Add(tag);

  public void RemoveTag(string tag) => this.tags.Remove(Hash.SDBMLower(tag));

  public void RemoveTag(int tag) => this.tags.Remove(tag);

  public bool HasTag(string tag) => this.tags.Contains(Hash.SDBMLower(tag));

  public bool HasTag(int tag) => this.tags.Contains(tag);

  public bool HasTags(int[] searchTags)
  {
    for (int index = 0; index < searchTags.Length; ++index)
    {
      if (!this.tags.Contains(searchTags[index]))
        return false;
    }
    return true;
  }
}
