// Decompiled with JetBrains decompiler
// Type: TreeFilterableSideScreenRow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TreeFilterableSideScreenRow")]
public class TreeFilterableSideScreenRow : KMonoBehaviour
{
  public bool visualDirty;
  [SerializeField]
  private LocText elementName;
  [SerializeField]
  private GameObject elementGroup;
  [SerializeField]
  private MultiToggle checkBoxToggle;
  [SerializeField]
  private MultiToggle arrowToggle;
  [SerializeField]
  private KImage bgImg;
  private List<Tag> subTags = new List<Tag>();
  private List<TreeFilterableSideScreenElement> rowElements = new List<TreeFilterableSideScreenElement>();
  private TreeFilterableSideScreen parent;

  public TreeFilterableSideScreen Parent
  {
    get => this.parent;
    set => this.parent = value;
  }

  public TreeFilterableSideScreenRow.State GetState()
  {
    bool flag1 = false;
    bool flag2 = false;
    foreach (TreeFilterableSideScreenElement rowElement in this.rowElements)
    {
      if (this.parent.GetElementTagAcceptedState(rowElement.GetElementTag()))
        flag1 = true;
      else
        flag2 = true;
    }
    if (flag1 && !flag2)
      return TreeFilterableSideScreenRow.State.On;
    if (!flag1 & flag2)
      return TreeFilterableSideScreenRow.State.Off;
    if (flag1 & flag2)
      return TreeFilterableSideScreenRow.State.Mixed;
    return this.rowElements.Count <= 0 ? TreeFilterableSideScreenRow.State.Off : TreeFilterableSideScreenRow.State.On;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.checkBoxToggle.onClick += (System.Action) (() =>
    {
      switch (this.GetState())
      {
        case TreeFilterableSideScreenRow.State.Off:
        case TreeFilterableSideScreenRow.State.Mixed:
          this.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.On);
          break;
        case TreeFilterableSideScreenRow.State.On:
          this.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.Off);
          break;
      }
    });
  }

  protected virtual void OnCmpEnable()
  {
    base.OnCmpEnable();
    this.SetArrowToggleState(this.GetState() != 0);
  }

  protected virtual void OnCmpDisable()
  {
    this.SetArrowToggleState(false);
    this.rowElements.ForEach((Action<TreeFilterableSideScreenElement>) (row => row.OnSelectionChanged -= new Action<Tag, bool>(this.OnElementSelectionChanged)));
    base.OnCmpDisable();
  }

  protected virtual void OnCleanUp() => base.OnCleanUp();

  public void UpdateCheckBoxVisualState()
  {
    this.checkBoxToggle.ChangeState((int) this.GetState());
    this.visualDirty = false;
  }

  public void ChangeCheckBoxState(TreeFilterableSideScreenRow.State newState)
  {
    switch (newState)
    {
      case TreeFilterableSideScreenRow.State.Off:
        this.rowElements.ForEach((Action<TreeFilterableSideScreenElement>) (re => re.SetCheckBox(false)));
        break;
      case TreeFilterableSideScreenRow.State.On:
        this.rowElements.ForEach((Action<TreeFilterableSideScreenElement>) (re => re.SetCheckBox(true)));
        break;
    }
    this.visualDirty = true;
  }

  private void ArrowToggleClicked()
  {
    this.SetArrowToggleState(this.arrowToggle.CurrentState != 1);
    this.UpdateArrowToggleState();
  }

  private void SetArrowToggleState(bool state)
  {
    this.arrowToggle.ChangeState(state ? 1 : 0);
    this.UpdateArrowToggleState();
  }

  private void UpdateArrowToggleState()
  {
    bool flag = this.arrowToggle.CurrentState != 0;
    this.elementGroup.SetActive(flag);
    ((Behaviour) this.bgImg).enabled = flag;
  }

  private void ArrowToggleDisabledClick() => KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));

  private void OnElementSelectionChanged(Tag t, bool state)
  {
    if (state)
      this.parent.AddTag(t);
    else
      this.parent.RemoveTag(t);
    this.visualDirty = true;
  }

  public void SetElement(Tag mainElementTag, bool state, Dictionary<Tag, bool> filterMap)
  {
    this.subTags.Clear();
    this.rowElements.Clear();
    ((TMP_Text) this.elementName).text = mainElementTag.ProperName();
    ((Behaviour) this.bgImg).enabled = false;
    string str = string.Format((string) UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.CATEGORYBUTTONTOOLTIP, (object) mainElementTag.ProperName());
    ((Component) this.checkBoxToggle).GetComponent<ToolTip>().SetSimpleTooltip(str);
    if (filterMap.Count == 0)
    {
      if (this.elementGroup.activeInHierarchy)
        this.elementGroup.SetActive(false);
      this.arrowToggle.onClick = new System.Action(this.ArrowToggleDisabledClick);
      this.arrowToggle.ChangeState(0);
    }
    else
    {
      this.arrowToggle.onClick = new System.Action(this.ArrowToggleClicked);
      this.arrowToggle.ChangeState(0);
      foreach (KeyValuePair<Tag, bool> filter in filterMap)
      {
        TreeFilterableSideScreenElement freeElement = this.parent.elementPool.GetFreeElement(this.elementGroup, true);
        freeElement.Parent = this.parent;
        freeElement.SetTag(filter.Key);
        freeElement.SetCheckBox(filter.Value);
        freeElement.OnSelectionChanged += new Action<Tag, bool>(this.OnElementSelectionChanged);
        freeElement.SetCheckBox(this.parent.IsTagAllowed(filter.Key));
        this.rowElements.Add(freeElement);
        this.subTags.Add(filter.Key);
      }
    }
    this.UpdateCheckBoxVisualState();
  }

  public enum State
  {
    Off,
    Mixed,
    On,
  }
}
