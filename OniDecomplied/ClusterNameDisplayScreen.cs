// Decompiled with JetBrains decompiler
// Type: ClusterNameDisplayScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClusterNameDisplayScreen : KScreen
{
  public static ClusterNameDisplayScreen Instance;
  public GameObject nameAndBarsPrefab;
  [SerializeField]
  private Color selectedColor;
  [SerializeField]
  private Color defaultColor;
  private List<ClusterNameDisplayScreen.Entry> m_entries = new List<ClusterNameDisplayScreen.Entry>();
  private List<KCollider2D> workingList = new List<KCollider2D>();

  public static void DestroyInstance() => ClusterNameDisplayScreen.Instance = (ClusterNameDisplayScreen) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ClusterNameDisplayScreen.Instance = this;
  }

  protected virtual void OnSpawn() => base.OnSpawn();

  public void AddNewEntry(ClusterGridEntity representedObject)
  {
    if (this.GetEntry(representedObject) != null)
      return;
    ClusterNameDisplayScreen.Entry entry = new ClusterNameDisplayScreen.Entry();
    entry.grid_entity = representedObject;
    GameObject gameObject = Util.KInstantiateUI(this.nameAndBarsPrefab, ((Component) this).gameObject, true);
    entry.display_go = gameObject;
    ((Object) gameObject).name = ((Object) representedObject).name + " cluster overlay";
    entry.Name = ((Object) representedObject).name;
    entry.refs = gameObject.GetComponent<HierarchyReferences>();
    entry.bars_go = ((Component) entry.refs.GetReference<RectTransform>("Bars")).gameObject;
    this.m_entries.Add(entry);
    if (!Object.op_Inequality((Object) ((Component) representedObject).GetComponent<KSelectable>(), (Object) null))
      return;
    this.UpdateName(representedObject);
    this.UpdateBars(representedObject);
  }

  private void LateUpdate()
  {
    if (App.isLoading || App.IsExiting)
      return;
    int count = this.m_entries.Count;
    int index = 0;
    while (index < count)
    {
      if (Object.op_Inequality((Object) this.m_entries[index].grid_entity, (Object) null) && ClusterMapScreen.GetRevealLevel(this.m_entries[index].grid_entity) == ClusterRevealLevel.Visible)
      {
        Transform entityNameTarget = ClusterMapScreen.Instance.GetGridEntityNameTarget(this.m_entries[index].grid_entity);
        if (Object.op_Inequality((Object) entityNameTarget, (Object) null))
        {
          Vector3 position = TransformExtensions.GetPosition(entityNameTarget);
          ((Transform) this.m_entries[index].display_go.GetComponent<RectTransform>()).SetPositionAndRotation(position, Quaternion.identity);
          this.m_entries[index].display_go.SetActive(this.m_entries[index].grid_entity.IsVisible && this.m_entries[index].grid_entity.ShowName());
        }
        else if (this.m_entries[index].display_go.activeSelf)
          this.m_entries[index].display_go.SetActive(false);
        this.UpdateBars(this.m_entries[index].grid_entity);
        if (Object.op_Inequality((Object) this.m_entries[index].bars_go, (Object) null))
        {
          this.m_entries[index].bars_go.GetComponentsInChildren<KCollider2D>(false, this.workingList);
          foreach (KCollider2D working in this.workingList)
            working.MarkDirty();
        }
        ++index;
      }
      else
      {
        Object.Destroy((Object) this.m_entries[index].display_go);
        --count;
        this.m_entries[index] = this.m_entries[count];
      }
    }
    this.m_entries.RemoveRange(count, this.m_entries.Count - count);
  }

  public void UpdateName(ClusterGridEntity representedObject)
  {
    ClusterNameDisplayScreen.Entry entry = this.GetEntry(representedObject);
    if (entry == null)
      return;
    KSelectable component = ((Component) representedObject).GetComponent<KSelectable>();
    ((Object) entry.display_go).name = component.GetProperName() + " cluster overlay";
    LocText componentInChildren = entry.display_go.GetComponentInChildren<LocText>();
    if (!Object.op_Inequality((Object) componentInChildren, (Object) null))
      return;
    ((TMP_Text) componentInChildren).text = component.GetProperName();
  }

  private void UpdateBars(ClusterGridEntity representedObject)
  {
    ClusterNameDisplayScreen.Entry entry = this.GetEntry(representedObject);
    if (entry == null)
      return;
    GenericUIProgressBar componentInChildren = entry.bars_go.GetComponentInChildren<GenericUIProgressBar>(true);
    if (entry.grid_entity.ShowProgressBar())
    {
      if (!((Component) componentInChildren).gameObject.activeSelf)
        ((Component) componentInChildren).gameObject.SetActive(true);
      componentInChildren.SetFillPercentage(entry.grid_entity.GetProgress());
    }
    else
    {
      if (!((Component) componentInChildren).gameObject.activeSelf)
        return;
      ((Component) componentInChildren).gameObject.SetActive(false);
    }
  }

  private ClusterNameDisplayScreen.Entry GetEntry(ClusterGridEntity entity) => this.m_entries.Find((Predicate<ClusterNameDisplayScreen.Entry>) (entry => Object.op_Equality((Object) entry.grid_entity, (Object) entity)));

  private class Entry
  {
    public string Name;
    public ClusterGridEntity grid_entity;
    public GameObject display_go;
    public GameObject bars_go;
    public HierarchyReferences refs;
  }
}
