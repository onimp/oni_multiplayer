// Decompiled with JetBrains decompiler
// Type: FlatTagFilterable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using UnityEngine;

public class FlatTagFilterable : KMonoBehaviour
{
  [Serialize]
  public List<Tag> selectedTags = new List<Tag>();
  public List<Tag> tagOptions = new List<Tag>();
  public string headerText;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    TreeFilterable component = ((Component) this).GetComponent<TreeFilterable>();
    component.filterByStorageCategoriesOnSpawn = false;
    component.UpdateFilters(new HashSet<Tag>((IEnumerable<Tag>) this.selectedTags));
  }

  public void SelectTag(Tag tag, bool state)
  {
    Debug.Assert(this.tagOptions.Contains(tag), (object) ("The tag " + ((Tag) ref tag).Name + " is not valid for this filterable - it must be added to tagOptions"));
    if (state)
    {
      if (!this.selectedTags.Contains(tag))
        this.selectedTags.Add(tag);
    }
    else if (this.selectedTags.Contains(tag))
      this.selectedTags.Remove(tag);
    ((Component) this).GetComponent<TreeFilterable>().UpdateFilters(new HashSet<Tag>((IEnumerable<Tag>) this.selectedTags));
  }

  public void ToggleTag(Tag tag) => this.SelectTag(tag, !this.selectedTags.Contains(tag));

  public string GetHeaderText() => this.headerText;
}
