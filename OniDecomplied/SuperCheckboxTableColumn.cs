// Decompiled with JetBrains decompiler
// Type: SuperCheckboxTableColumn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class SuperCheckboxTableColumn : CheckboxTableColumn
{
  public GameObject prefab_super_checkbox = Assets.UIPrefabs.TableScreenWidgets.SuperCheckbox_Horizontal;
  public CheckboxTableColumn[] columns_affected;

  public SuperCheckboxTableColumn(
    CheckboxTableColumn[] columns_affected,
    Action<IAssignableIdentity, GameObject> on_load_action,
    Func<IAssignableIdentity, GameObject, TableScreen.ResultValues> get_value_action,
    Action<GameObject> on_press_action,
    Action<GameObject, TableScreen.ResultValues> set_value_action,
    Comparison<IAssignableIdentity> sort_comparison,
    Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip)
    : base(on_load_action, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip, (Action<IAssignableIdentity, GameObject, ToolTip>) null)
  {
    this.columns_affected = columns_affected;
  }

  public override GameObject GetDefaultWidget(GameObject parent)
  {
    GameObject widget_go = Util.KInstantiateUI(this.prefab_super_checkbox, parent, true);
    if (Object.op_Inequality((Object) widget_go.GetComponent<ToolTip>(), (Object) null))
      widget_go.GetComponent<ToolTip>().OnToolTip = (Func<string>) (() => this.GetTooltip(widget_go.GetComponent<ToolTip>()));
    widget_go.GetComponent<MultiToggle>().onClick += (System.Action) (() => this.on_press_action(widget_go));
    return widget_go;
  }

  public override GameObject GetHeaderWidget(GameObject parent)
  {
    GameObject widget_go = Util.KInstantiateUI(this.prefab_super_checkbox, parent, true);
    if (Object.op_Inequality((Object) widget_go.GetComponent<ToolTip>(), (Object) null))
      widget_go.GetComponent<ToolTip>().OnToolTip = (Func<string>) (() => this.GetTooltip(widget_go.GetComponent<ToolTip>()));
    widget_go.GetComponent<MultiToggle>().onClick += (System.Action) (() => this.on_press_action(widget_go));
    return widget_go;
  }

  public override GameObject GetMinionWidget(GameObject parent)
  {
    GameObject widget_go = Util.KInstantiateUI(this.prefab_super_checkbox, parent, true);
    if (Object.op_Inequality((Object) widget_go.GetComponent<ToolTip>(), (Object) null))
      widget_go.GetComponent<ToolTip>().OnToolTip = (Func<string>) (() => this.GetTooltip(widget_go.GetComponent<ToolTip>()));
    widget_go.GetComponent<MultiToggle>().onClick += (System.Action) (() => this.on_press_action(widget_go));
    return widget_go;
  }
}
