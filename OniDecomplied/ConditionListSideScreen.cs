// Decompiled with JetBrains decompiler
// Type: ConditionListSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionListSideScreen : SideScreenContent
{
  public GameObject rowPrefab;
  public GameObject rowContainer;
  [Tooltip("This list is indexed by the ProcessCondition.Status enum")]
  public static Color readyColor = Color.black;
  public static Color failedColor = Color.red;
  public static Color warningColor = new Color(1f, 0.3529412f, 0.0f, 1f);
  private IProcessConditionSet targetConditionSet;
  private Dictionary<ProcessCondition, GameObject> rows = new Dictionary<ProcessCondition, GameObject>();

  public override bool IsValidForTarget(GameObject target) => false;

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    if (!Object.op_Inequality((Object) target, (Object) null))
      return;
    this.targetConditionSet = target.GetComponent<IProcessConditionSet>();
  }

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    if (!show)
      return;
    this.Refresh();
  }

  private void Refresh()
  {
    bool flag = false;
    List<ProcessCondition> conditionSet = this.targetConditionSet.GetConditionSet(ProcessCondition.ProcessConditionType.All);
    foreach (ProcessCondition key in conditionSet)
    {
      if (!this.rows.ContainsKey(key))
      {
        flag = true;
        break;
      }
    }
    foreach (KeyValuePair<ProcessCondition, GameObject> row in this.rows)
    {
      if (!conditionSet.Contains(row.Key))
      {
        flag = true;
        break;
      }
    }
    if (flag)
      this.Rebuild();
    foreach (KeyValuePair<ProcessCondition, GameObject> row in this.rows)
      ConditionListSideScreen.SetRowState(row.Value, row.Key);
  }

  public static void SetRowState(GameObject row, ProcessCondition condition)
  {
    HierarchyReferences component = row.GetComponent<HierarchyReferences>();
    ProcessCondition.Status condition1 = condition.EvaluateCondition();
    ((TMP_Text) component.GetReference<LocText>("Label")).text = condition.GetStatusMessage(condition1);
    switch (condition1)
    {
      case ProcessCondition.Status.Failure:
        ((Graphic) component.GetReference<LocText>("Label")).color = ConditionListSideScreen.failedColor;
        ((Graphic) component.GetReference<Image>("Box")).color = ConditionListSideScreen.failedColor;
        break;
      case ProcessCondition.Status.Warning:
        ((Graphic) component.GetReference<LocText>("Label")).color = ConditionListSideScreen.warningColor;
        ((Graphic) component.GetReference<Image>("Box")).color = ConditionListSideScreen.warningColor;
        break;
      case ProcessCondition.Status.Ready:
        ((Graphic) component.GetReference<LocText>("Label")).color = ConditionListSideScreen.readyColor;
        ((Graphic) component.GetReference<Image>("Box")).color = ConditionListSideScreen.readyColor;
        break;
    }
    ((Component) component.GetReference<Image>("Check")).gameObject.SetActive(condition1 == ProcessCondition.Status.Ready);
    ((Component) component.GetReference<Image>("Dash")).gameObject.SetActive(false);
    row.GetComponent<ToolTip>().SetSimpleTooltip(condition.GetStatusTooltip(condition1));
  }

  private void Rebuild()
  {
    this.ClearRows();
    this.BuildRows();
  }

  private void ClearRows()
  {
    foreach (KeyValuePair<ProcessCondition, GameObject> row in this.rows)
      Util.KDestroyGameObject(row.Value);
    this.rows.Clear();
  }

  private void BuildRows()
  {
    foreach (ProcessCondition condition in this.targetConditionSet.GetConditionSet(ProcessCondition.ProcessConditionType.All))
    {
      if (condition.ShowInUI())
      {
        GameObject gameObject = Util.KInstantiateUI(this.rowPrefab, this.rowContainer, true);
        this.rows.Add(condition, gameObject);
      }
    }
  }
}
