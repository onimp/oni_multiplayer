// Decompiled with JetBrains decompiler
// Type: LabelTableColumn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class LabelTableColumn : TableColumn
{
  public Func<IAssignableIdentity, GameObject, string> get_value_action;
  private int widget_width = 128;

  public LabelTableColumn(
    Action<IAssignableIdentity, GameObject> on_load_action,
    Func<IAssignableIdentity, GameObject, string> get_value_action,
    Comparison<IAssignableIdentity> sort_comparison,
    Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip,
    Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip,
    int widget_width = 128,
    bool should_refresh_columns = false)
    : base(on_load_action, sort_comparison, on_tooltip, on_sort_tooltip, should_refresh_columns: should_refresh_columns)
  {
    this.get_value_action = get_value_action;
    this.widget_width = widget_width;
  }

  public override GameObject GetDefaultWidget(GameObject parent)
  {
    GameObject defaultWidget = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Label, parent, true);
    LayoutElement component = ((Component) defaultWidget.GetComponentInChildren<LocText>()).GetComponent<LayoutElement>();
    double widgetWidth;
    float num = (float) (widgetWidth = (double) this.widget_width);
    component.minWidth = (float) widgetWidth;
    component.preferredWidth = num;
    return defaultWidget;
  }

  public override GameObject GetMinionWidget(GameObject parent)
  {
    GameObject minionWidget = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Label, parent, true);
    ToolTip tt = minionWidget.GetComponent<ToolTip>();
    tt.OnToolTip = (Func<string>) (() => this.GetTooltip(tt));
    LayoutElement component = ((Component) minionWidget.GetComponentInChildren<LocText>()).GetComponent<LayoutElement>();
    double widgetWidth;
    float num = (float) (widgetWidth = (double) this.widget_width);
    component.minWidth = (float) widgetWidth;
    component.preferredWidth = num;
    return minionWidget;
  }

  public override GameObject GetHeaderWidget(GameObject parent)
  {
    GameObject widget_go = (GameObject) null;
    widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.LabelHeader, parent, true);
    MultiToggle componentInChildren = widget_go.GetComponentInChildren<MultiToggle>(true);
    this.column_sort_toggle = componentInChildren;
    componentInChildren.onClick += (System.Action) (() =>
    {
      this.screen.SetSortComparison(this.sort_comparer, (TableColumn) this);
      this.screen.SortRows();
    });
    ToolTip tt = widget_go.GetComponent<ToolTip>();
    tt.OnToolTip = (Func<string>) (() =>
    {
      this.on_tooltip((IAssignableIdentity) null, widget_go, tt);
      return "";
    });
    tt = ((Component) widget_go.GetComponentInChildren<MultiToggle>()).GetComponent<ToolTip>();
    tt.OnToolTip = (Func<string>) (() =>
    {
      this.on_sort_tooltip((IAssignableIdentity) null, widget_go, tt);
      return "";
    });
    LayoutElement component = ((Component) widget_go.GetComponentInChildren<LocText>()).GetComponent<LayoutElement>();
    double widgetWidth;
    float num = (float) (widgetWidth = (double) this.widget_width);
    component.minWidth = (float) widgetWidth;
    component.preferredWidth = num;
    return widget_go;
  }
}
