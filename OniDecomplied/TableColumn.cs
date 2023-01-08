// Decompiled with JetBrains decompiler
// Type: TableColumn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class TableColumn : IRender1000ms
{
  public Action<IAssignableIdentity, GameObject> on_load_action;
  public Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip;
  public Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip;
  public Comparison<IAssignableIdentity> sort_comparer;
  public Dictionary<TableRow, GameObject> widgets_by_row = new Dictionary<TableRow, GameObject>();
  public string scrollerID;
  public TableScreen screen;
  public MultiToggle column_sort_toggle;
  private Func<bool> revealed;
  protected bool dirty;

  public bool isRevealed => this.revealed == null || this.revealed();

  public TableColumn(
    Action<IAssignableIdentity, GameObject> on_load_action,
    Comparison<IAssignableIdentity> sort_comparison,
    Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip = null,
    Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip = null,
    Func<bool> revealed = null,
    bool should_refresh_columns = false,
    string scrollerID = "")
  {
    this.on_load_action = on_load_action;
    this.sort_comparer = sort_comparison;
    this.on_tooltip = on_tooltip;
    this.on_sort_tooltip = on_sort_tooltip;
    this.revealed = revealed;
    this.scrollerID = scrollerID;
    if (!should_refresh_columns)
      return;
    SimAndRenderScheduler.instance.Add((object) this, false);
  }

  protected string GetTooltip(ToolTip tool_tip_instance)
  {
    GameObject gameObject = ((Component) tool_tip_instance).gameObject;
    HierarchyReferences component = ((Component) tool_tip_instance).GetComponent<HierarchyReferences>();
    if (Object.op_Inequality((Object) component, (Object) null) && component.HasReference("Widget"))
      gameObject = component.GetReference("Widget").gameObject;
    TableRow tableRow = (TableRow) null;
    foreach (KeyValuePair<TableRow, GameObject> keyValuePair in this.widgets_by_row)
    {
      if (Object.op_Equality((Object) keyValuePair.Value, (Object) gameObject))
      {
        tableRow = keyValuePair.Key;
        break;
      }
    }
    if (Object.op_Inequality((Object) tableRow, (Object) null) && this.on_tooltip != null)
      this.on_tooltip(tableRow.GetIdentity(), gameObject, tool_tip_instance);
    return "";
  }

  protected string GetSortTooltip(ToolTip sort_tooltip_instance)
  {
    GameObject gameObject = ((Component) ((KMonoBehaviour) sort_tooltip_instance).transform.parent).gameObject;
    TableRow tableRow = (TableRow) null;
    foreach (KeyValuePair<TableRow, GameObject> keyValuePair in this.widgets_by_row)
    {
      if (Object.op_Equality((Object) keyValuePair.Value, (Object) gameObject))
      {
        tableRow = keyValuePair.Key;
        break;
      }
    }
    if (Object.op_Inequality((Object) tableRow, (Object) null) && this.on_sort_tooltip != null)
      this.on_sort_tooltip(tableRow.GetIdentity(), gameObject, sort_tooltip_instance);
    return "";
  }

  public bool isDirty => this.dirty;

  public bool ContainsWidget(GameObject widget) => this.widgets_by_row.ContainsValue(widget);

  public virtual GameObject GetMinionWidget(GameObject parent)
  {
    Debug.LogError((object) "Table Column has no Widget prefab");
    return (GameObject) null;
  }

  public virtual GameObject GetHeaderWidget(GameObject parent)
  {
    Debug.LogError((object) "Table Column has no Widget prefab");
    return (GameObject) null;
  }

  public virtual GameObject GetDefaultWidget(GameObject parent)
  {
    Debug.LogError((object) "Table Column has no Widget prefab");
    return (GameObject) null;
  }

  public void Render1000ms(float dt) => this.MarkDirty();

  public void MarkDirty(GameObject triggering_obj = null, TableScreen.ResultValues triggering_object_state = TableScreen.ResultValues.False) => this.dirty = true;

  public void MarkClean() => this.dirty = false;
}
