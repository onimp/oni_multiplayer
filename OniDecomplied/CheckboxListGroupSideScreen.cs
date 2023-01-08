// Decompiled with JetBrains decompiler
// Type: CheckboxListGroupSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckboxListGroupSideScreen : SideScreenContent
{
  public const int DefaultCheckboxListSideScreenSortOrder = 20;
  private ObjectPool<CheckboxListGroupSideScreen.CheckboxContainer> checkboxContainerPool;
  private GameObjectPool checkboxPool;
  [SerializeField]
  private GameObject checkboxGroupPrefab;
  [SerializeField]
  private GameObject checkboxPrefab;
  [SerializeField]
  private RectTransform groupParent;
  [SerializeField]
  private RectTransform checkboxParent;
  [SerializeField]
  private LocText descriptionLabel;
  private List<ICheckboxListGroupControl> targets;
  private GameObject currentBuildTarget;
  private int uiRefreshSubHandle = -1;
  private List<CheckboxListGroupSideScreen.CheckboxContainer> activeChecklistGroups = new List<CheckboxListGroupSideScreen.CheckboxContainer>();

  private CheckboxListGroupSideScreen.CheckboxContainer InstantiateCheckboxContainer() => new CheckboxListGroupSideScreen.CheckboxContainer(Util.KInstantiateUI(this.checkboxGroupPrefab, ((Component) this.groupParent).gameObject, true).GetComponent<HierarchyReferences>());

  private GameObject InstantiateCheckbox() => Util.KInstantiateUI(this.checkboxPrefab, ((Component) this.checkboxParent).gameObject, false);

  protected virtual void OnSpawn()
  {
    this.checkboxPrefab.SetActive(false);
    this.checkboxGroupPrefab.SetActive(false);
    base.OnSpawn();
  }

  public override bool IsValidForTarget(GameObject target)
  {
    ICheckboxListGroupControl[] components = target.GetComponents<ICheckboxListGroupControl>();
    if (components != null)
    {
      foreach (ICheckboxListGroupControl listGroupControl in components)
      {
        if (listGroupControl.SidescreenEnabled())
          return true;
      }
    }
    foreach (ICheckboxListGroupControl listGroupControl in target.GetAllSMI<ICheckboxListGroupControl>())
    {
      if (listGroupControl.SidescreenEnabled())
        return true;
    }
    return false;
  }

  public override int GetSideScreenSortOrder() => this.targets == null ? 20 : this.targets[0].CheckboxSideScreenSortOrder();

  public override void SetTarget(GameObject target)
  {
    if (Object.op_Equality((Object) target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      this.targets = target.GetAllSMI<ICheckboxListGroupControl>();
      this.targets.AddRange((IEnumerable<ICheckboxListGroupControl>) target.GetComponents<ICheckboxListGroupControl>());
      this.Rebuild(target);
      this.uiRefreshSubHandle = KMonoBehaviourExtensions.Subscribe(this.currentBuildTarget, 1980521255, new Action<object>(this.Refresh));
    }
  }

  public override void ClearTarget()
  {
    if (this.uiRefreshSubHandle != -1 && Object.op_Inequality((Object) this.currentBuildTarget, (Object) null))
    {
      KMonoBehaviourExtensions.Unsubscribe(this.currentBuildTarget, this.uiRefreshSubHandle);
      this.uiRefreshSubHandle = -1;
    }
    this.ReleaseContainers(this.activeChecklistGroups.Count);
  }

  public override string GetTitle() => this.targets != null && this.targets.Count > 0 && this.targets[0] != null ? this.targets[0].Title : base.GetTitle();

  private void Rebuild(GameObject buildTarget)
  {
    if (this.checkboxContainerPool == null)
    {
      this.checkboxContainerPool = new ObjectPool<CheckboxListGroupSideScreen.CheckboxContainer>(new Func<CheckboxListGroupSideScreen.CheckboxContainer>(this.InstantiateCheckboxContainer));
      this.checkboxPool = new GameObjectPool(new Func<GameObject>(this.InstantiateCheckbox));
    }
    if (Object.op_Equality((Object) buildTarget, (Object) this.currentBuildTarget))
    {
      this.Refresh();
    }
    else
    {
      this.currentBuildTarget = buildTarget;
      ((Behaviour) this.descriptionLabel).enabled = !Util.IsNullOrWhiteSpace(this.targets[0].Description);
      if (!Util.IsNullOrWhiteSpace(this.targets[0].Description))
        ((TMP_Text) this.descriptionLabel).SetText(this.targets[0].Description);
      foreach (ICheckboxListGroupControl target in this.targets)
      {
        foreach (ICheckboxListGroupControl.ListGroup group in target.GetData())
        {
          CheckboxListGroupSideScreen.CheckboxContainer instance = this.checkboxContainerPool.GetInstance();
          this.InitContainer(target, group, instance);
        }
      }
    }
  }

  [ContextMenu("Force refresh")]
  private void Test() => this.Refresh();

  private void Refresh(object data = null)
  {
    int num1 = 0;
    foreach (ICheckboxListGroupControl target in this.targets)
    {
      foreach (ICheckboxListGroupControl.ListGroup group in target.GetData())
      {
        if (++num1 > this.activeChecklistGroups.Count)
          this.InitContainer(target, group, this.checkboxContainerPool.GetInstance());
        CheckboxListGroupSideScreen.CheckboxContainer activeChecklistGroup = this.activeChecklistGroups[num1 - 1];
        if (group.resolveTitleCallback != null)
          ((TMP_Text) activeChecklistGroup.container.GetReference<LocText>("Text")).SetText(group.resolveTitleCallback(group.title));
        int num2 = 0;
        foreach (ICheckboxListGroupControl.CheckboxItem checkboxItem in group.checkboxItems)
        {
          ++num2;
          ((Behaviour) activeChecklistGroup.checkboxUIItems[num2 - 1].GetReference<Image>("Check")).enabled = checkboxItem.isOn;
        }
      }
    }
    this.ReleaseContainers(this.activeChecklistGroups.Count - num1);
  }

  private void ReleaseContainers(int count)
  {
    int count1 = this.activeChecklistGroups.Count;
    for (int index1 = 1; index1 <= count; ++index1)
    {
      int index2 = count1 - index1;
      CheckboxListGroupSideScreen.CheckboxContainer activeChecklistGroup = this.activeChecklistGroups[index2];
      this.activeChecklistGroups.RemoveAt(index2);
      for (int index3 = activeChecklistGroup.checkboxUIItems.Count - 1; index3 >= 0; --index3)
      {
        GameObject gameObject = ((Component) activeChecklistGroup.checkboxUIItems[index3]).gameObject;
        activeChecklistGroup.checkboxUIItems.RemoveAt(index3);
        gameObject.SetActive(false);
        gameObject.transform.SetParent((Transform) this.checkboxParent);
        this.checkboxPool.ReleaseInstance(gameObject);
      }
      ((Component) activeChecklistGroup.container).gameObject.SetActive(false);
      this.checkboxContainerPool.ReleaseInstance(activeChecklistGroup);
    }
  }

  private void InitContainer(
    ICheckboxListGroupControl target,
    ICheckboxListGroupControl.ListGroup group,
    CheckboxListGroupSideScreen.CheckboxContainer groupUI)
  {
    this.activeChecklistGroups.Add(groupUI);
    ((Component) groupUI.container).gameObject.SetActive(true);
    string str = group.title;
    if (group.resolveTitleCallback != null)
      str = group.resolveTitleCallback(str);
    ((TMP_Text) groupUI.container.GetReference<LocText>("Text")).SetText(str);
    foreach (ICheckboxListGroupControl.CheckboxItem checkboxItem in group.checkboxItems)
    {
      ICheckboxListGroupControl.CheckboxItem item = checkboxItem;
      HierarchyReferences component = this.checkboxPool.GetInstance().GetComponent<HierarchyReferences>();
      groupUI.checkboxUIItems.Add(component);
      component.transform.SetParent(groupUI.container.transform);
      ((Component) component).gameObject.SetActive(true);
      ((TMP_Text) component.GetReference<LocText>("Text")).SetText(item.text);
      ((Behaviour) component.GetReference<Image>("Check")).enabled = item.isOn;
      ToolTip reference = component.GetReference<ToolTip>("Tooltip");
      reference.SetSimpleTooltip(item.tooltip);
      reference.refreshWhileHovering = item.resolveTooltipCallback != null;
      reference.OnToolTip = (Func<string>) (() => item.resolveTooltipCallback == null ? item.tooltip : item.resolveTooltipCallback(item.tooltip, (object) target));
    }
  }

  public class CheckboxContainer
  {
    public HierarchyReferences container;
    public List<HierarchyReferences> checkboxUIItems = new List<HierarchyReferences>();

    public CheckboxContainer(HierarchyReferences container) => this.container = container;
  }
}
