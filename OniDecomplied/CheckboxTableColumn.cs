// Decompiled with JetBrains decompiler
// Type: CheckboxTableColumn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class CheckboxTableColumn : TableColumn
{
  public GameObject prefab_header_portrait_checkbox = Assets.UIPrefabs.TableScreenWidgets.TogglePortrait;
  public GameObject prefab_checkbox = Assets.UIPrefabs.TableScreenWidgets.Checkbox;
  public Action<GameObject> on_press_action;
  public Action<GameObject, TableScreen.ResultValues> on_set_action;
  public Func<IAssignableIdentity, GameObject, TableScreen.ResultValues> get_value_action;

  public CheckboxTableColumn(
    Action<IAssignableIdentity, GameObject> on_load_action,
    Func<IAssignableIdentity, GameObject, TableScreen.ResultValues> get_value_action,
    Action<GameObject> on_press_action,
    Action<GameObject, TableScreen.ResultValues> set_value_action,
    Comparison<IAssignableIdentity> sort_comparer,
    Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip,
    Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip,
    Func<bool> revealed = null)
    : base(on_load_action, sort_comparer, on_tooltip, on_sort_tooltip, revealed)
  {
    this.get_value_action = get_value_action;
    this.on_press_action = on_press_action;
    this.on_set_action = set_value_action;
  }

  public override GameObject GetMinionWidget(GameObject parent)
  {
    GameObject widget_go = Util.KInstantiateUI(this.prefab_checkbox, parent, true);
    if (Object.op_Inequality((Object) widget_go.GetComponent<ToolTip>(), (Object) null))
      widget_go.GetComponent<ToolTip>().OnToolTip = (Func<string>) (() => this.GetTooltip(widget_go.GetComponent<ToolTip>()));
    widget_go.GetComponent<MultiToggle>().onClick += (System.Action) (() => this.on_press_action(widget_go));
    return widget_go;
  }

  public override GameObject GetDefaultWidget(GameObject parent)
  {
    GameObject widget_go = Util.KInstantiateUI(this.prefab_checkbox, parent, true);
    if (Object.op_Inequality((Object) widget_go.GetComponent<ToolTip>(), (Object) null))
      widget_go.GetComponent<ToolTip>().OnToolTip = (Func<string>) (() => this.GetTooltip(widget_go.GetComponent<ToolTip>()));
    widget_go.GetComponent<MultiToggle>().onClick += (System.Action) (() => this.on_press_action(widget_go));
    return widget_go;
  }

  public override GameObject GetHeaderWidget(GameObject parent)
  {
    ToolTip tooltip = (ToolTip) null;
    GameObject widget_go = Util.KInstantiateUI(this.prefab_header_portrait_checkbox, parent, true);
    tooltip = widget_go.GetComponent<ToolTip>();
    HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
    if (Object.op_Equality((Object) tooltip, (Object) null) && Object.op_Inequality((Object) component, (Object) null) && component.HasReference("ToolTip"))
      tooltip = component.GetReference("ToolTip") as ToolTip;
    tooltip.OnToolTip = (Func<string>) (() => this.GetTooltip(tooltip));
    (component.GetReference("Toggle") as MultiToggle).onClick += (System.Action) (() => this.on_press_action(widget_go));
    MultiToggle componentInChildren = widget_go.GetComponentInChildren<MultiToggle>();
    componentInChildren.onClick += (System.Action) (() =>
    {
      this.screen.SetSortComparison(this.sort_comparer, (TableColumn) this);
      this.screen.SortRows();
    });
    ToolTip sort_tooltip = ((Component) componentInChildren).GetComponent<ToolTip>();
    sort_tooltip.OnToolTip = (Func<string>) (() => this.GetSortTooltip(sort_tooltip));
    this.column_sort_toggle = widget_go.GetComponentInChildren<MultiToggle>();
    return widget_go;
  }
}
