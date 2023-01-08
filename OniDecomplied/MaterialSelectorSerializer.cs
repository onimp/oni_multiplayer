// Decompiled with JetBrains decompiler
// Type: MaterialSelectorSerializer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/MaterialSelectorSerializer")]
public class MaterialSelectorSerializer : KMonoBehaviour
{
  [Serialize]
  private List<Dictionary<Tag, Tag>> previouslySelectedElements;
  [Serialize]
  private List<Dictionary<Tag, Tag>>[] previouslySelectedElementsPerWorld;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.previouslySelectedElementsPerWorld != null)
      return;
    this.previouslySelectedElementsPerWorld = new List<Dictionary<Tag, Tag>>[(int) ClusterManager.INVALID_WORLD_IDX];
    if (this.previouslySelectedElements == null)
      return;
    foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
      this.previouslySelectedElementsPerWorld[worldContainer.id] = this.previouslySelectedElements.ConvertAll<Dictionary<Tag, Tag>>((Converter<Dictionary<Tag, Tag>, Dictionary<Tag, Tag>>) (input => new Dictionary<Tag, Tag>((IDictionary<Tag, Tag>) input)));
    this.previouslySelectedElements = (List<Dictionary<Tag, Tag>>) null;
  }

  public void WipeWorldSelectionData(int worldID) => this.previouslySelectedElementsPerWorld[worldID] = (List<Dictionary<Tag, Tag>>) null;

  public void SetSelectedElement(int worldID, int selectorIndex, Tag recipe, Tag element)
  {
    if (this.previouslySelectedElementsPerWorld[worldID] == null)
      this.previouslySelectedElementsPerWorld[worldID] = new List<Dictionary<Tag, Tag>>();
    List<Dictionary<Tag, Tag>> dictionaryList = this.previouslySelectedElementsPerWorld[worldID];
    while (dictionaryList.Count <= selectorIndex)
      dictionaryList.Add(new Dictionary<Tag, Tag>());
    dictionaryList[selectorIndex][recipe] = element;
  }

  public Tag GetPreviousElement(int worldID, int selectorIndex, Tag recipe)
  {
    Tag invalid = Tag.Invalid;
    if (this.previouslySelectedElementsPerWorld[worldID] == null)
      return invalid;
    List<Dictionary<Tag, Tag>> dictionaryList = this.previouslySelectedElementsPerWorld[worldID];
    if (dictionaryList.Count <= selectorIndex)
      return invalid;
    dictionaryList[selectorIndex].TryGetValue(recipe, out invalid);
    return invalid;
  }
}
