// Decompiled with JetBrains decompiler
// Type: PrioritizationGroupTableColumn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class PrioritizationGroupTableColumn : TableColumn
{
  public object userData;
  private Action<object, int> onChangePriority;
  private Func<object, string> onHoverWidget;
  private Func<object, string> onHoverHeaderOptionSelector;
  private Action<object> onSortClicked;
  private Func<object, string> onSortHovered;

  public PrioritizationGroupTableColumn(
    object user_data,
    Action<IAssignableIdentity, GameObject> on_load_action,
    Action<object, int> on_change_priority,
    Func<object, string> on_hover_widget,
    Action<object, int> on_change_header_priority,
    Func<object, string> on_hover_header_option_selector,
    Action<object> on_sort_clicked,
    Func<object, string> on_sort_hovered)
    : base(on_load_action, (Comparison<IAssignableIdentity>) null)
  {
    this.userData = user_data;
    this.onChangePriority = on_change_priority;
    this.onHoverWidget = on_hover_widget;
    this.onHoverHeaderOptionSelector = on_hover_header_option_selector;
    this.onSortClicked = on_sort_clicked;
    this.onSortHovered = on_sort_hovered;
  }

  public override GameObject GetMinionWidget(GameObject parent) => this.GetWidget(parent);

  public override GameObject GetDefaultWidget(GameObject parent) => this.GetWidget(parent);

  private GameObject GetWidget(GameObject parent)
  {
    GameObject widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.PriorityGroupSelector, parent, true);
    OptionSelector component = widget_go.GetComponent<OptionSelector>();
    component.Initialize((object) widget_go);
    component.OnChangePriority = (Action<object, int>) ((widget, delta) => this.onChangePriority(widget, delta));
    ToolTip[] componentsInChildren = ((Component) widget_go.transform).GetComponentsInChildren<ToolTip>();
    if (componentsInChildren != null)
    {
      foreach (ToolTip toolTip in componentsInChildren)
        toolTip.OnToolTip = (Func<string>) (() => this.onHoverWidget((object) widget_go));
    }
    return widget_go;
  }

  public override GameObject GetHeaderWidget(GameObject parent)
  {
    GameObject widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.PriorityGroupSelectorHeader, parent, true);
    HierarchyReferences component1 = widget_go.GetComponent<HierarchyReferences>();
    LayoutElement component2 = ((Component) widget_go.GetComponentInChildren<LocText>()).GetComponent<LayoutElement>();
    double num1;
    float num2 = (float) (num1 = 63.0);
    component2.minWidth = (float) num1;
    component2.preferredWidth = num2;
    Component reference = component1.GetReference("Label");
    ((Graphic) reference.GetComponent<LocText>()).raycastTarget = true;
    ToolTip component3 = reference.GetComponent<ToolTip>();
    if (Object.op_Inequality((Object) component3, (Object) null))
      component3.OnToolTip = (Func<string>) (() => this.onHoverWidget((object) widget_go));
    MultiToggle componentInChildren = widget_go.GetComponentInChildren<MultiToggle>(true);
    this.column_sort_toggle = componentInChildren;
    componentInChildren.onClick += (System.Action) (() => this.onSortClicked((object) widget_go));
    ToolTip component4 = ((Component) componentInChildren).GetComponent<ToolTip>();
    if (Object.op_Inequality((Object) component4, (Object) null))
      component4.OnToolTip = (Func<string>) (() => this.onSortHovered((object) widget_go));
    ToolTip component5 = ((Component) (component1.GetReference("PrioritizeButton") as KButton)).GetComponent<ToolTip>();
    if (Object.op_Inequality((Object) component5, (Object) null))
      component5.OnToolTip = (Func<string>) (() => this.onHoverHeaderOptionSelector((object) widget_go));
    return widget_go;
  }
}
