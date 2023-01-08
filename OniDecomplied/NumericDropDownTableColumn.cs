// Decompiled with JetBrains decompiler
// Type: NumericDropDownTableColumn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NumericDropDownTableColumn : TableColumn
{
  public object userData;
  private NumericDropDownTableColumn.ToolTipCallbacks callbacks;
  private Action<GameObject, int> set_value_action;
  private List<TMP_Dropdown.OptionData> options;

  public NumericDropDownTableColumn(
    object user_data,
    List<TMP_Dropdown.OptionData> options,
    Action<IAssignableIdentity, GameObject> on_load_action,
    Action<GameObject, int> set_value_action,
    Comparison<IAssignableIdentity> sort_comparer,
    NumericDropDownTableColumn.ToolTipCallbacks callbacks,
    Func<bool> revealed = null)
    : base(on_load_action, sort_comparer, callbacks.headerTooltip, callbacks.headerSortTooltip, revealed)
  {
    this.userData = user_data;
    this.set_value_action = set_value_action;
    this.options = options;
    this.callbacks = callbacks;
  }

  public override GameObject GetMinionWidget(GameObject parent) => this.GetWidget(parent);

  public override GameObject GetDefaultWidget(GameObject parent) => this.GetWidget(parent);

  private GameObject GetWidget(GameObject parent)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    NumericDropDownTableColumn.\u003C\u003Ec__DisplayClass8_0 cDisplayClass80 = new NumericDropDownTableColumn.\u003C\u003Ec__DisplayClass8_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass80.\u003C\u003E4__this = this;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass80.widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.NumericDropDown, parent, true);
    // ISSUE: reference to a compiler-generated field
    TMP_Dropdown componentInChildren = ((Component) cDisplayClass80.widget_go.transform).GetComponentInChildren<TMP_Dropdown>();
    componentInChildren.options = this.options;
    // ISSUE: method pointer
    ((UnityEvent<int>) componentInChildren.onValueChanged).AddListener(new UnityAction<int>((object) cDisplayClass80, __methodptr(\u003CGetWidget\u003Eb__0)));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    cDisplayClass80.tt = ((Component) cDisplayClass80.widget_go.transform).GetComponentInChildren<ToolTip>();
    // ISSUE: reference to a compiler-generated field
    if (Object.op_Inequality((Object) cDisplayClass80.tt, (Object) null))
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated method
      cDisplayClass80.tt.OnToolTip = new Func<string>(cDisplayClass80.\u003CGetWidget\u003Eb__1);
    }
    // ISSUE: reference to a compiler-generated field
    return cDisplayClass80.widget_go;
  }

  public override GameObject GetHeaderWidget(GameObject parent)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    NumericDropDownTableColumn.\u003C\u003Ec__DisplayClass9_0 cDisplayClass90 = new NumericDropDownTableColumn.\u003C\u003Ec__DisplayClass9_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass90.\u003C\u003E4__this = this;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass90.widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.DropDownHeader, parent, true);
    // ISSUE: reference to a compiler-generated field
    HierarchyReferences component1 = cDisplayClass90.widget_go.GetComponent<HierarchyReferences>();
    Component reference1 = component1.GetReference("Label");
    MultiToggle componentInChildren1 = reference1.GetComponentInChildren<MultiToggle>(true);
    this.column_sort_toggle = componentInChildren1;
    // ISSUE: reference to a compiler-generated method
    componentInChildren1.onClick += new System.Action(cDisplayClass90.\u003CGetHeaderWidget\u003Eb__0);
    ToolTip tt1 = reference1.GetComponent<ToolTip>();
    ((Behaviour) tt1).enabled = true;
    tt1.OnToolTip = (Func<string>) (() =>
    {
      this.callbacks.headerTooltip((IAssignableIdentity) null, widget_go, tt1);
      return "";
    });
    ToolTip tt2 = ((Component) componentInChildren1.transform).GetComponent<ToolTip>();
    tt2.OnToolTip = (Func<string>) (() =>
    {
      this.callbacks.headerSortTooltip((IAssignableIdentity) null, widget_go, tt2);
      return "";
    });
    Component reference2 = component1.GetReference("DropDown");
    TMP_Dropdown componentInChildren2 = reference2.GetComponentInChildren<TMP_Dropdown>();
    componentInChildren2.options = this.options;
    // ISSUE: method pointer
    ((UnityEvent<int>) componentInChildren2.onValueChanged).AddListener(new UnityAction<int>((object) cDisplayClass90, __methodptr(\u003CGetHeaderWidget\u003Eb__3)));
    ToolTip tt3 = reference2.GetComponent<ToolTip>();
    tt3.OnToolTip = (Func<string>) (() =>
    {
      this.callbacks.headerDropdownTooltip((IAssignableIdentity) null, widget_go, tt3);
      return "";
    });
    // ISSUE: reference to a compiler-generated field
    LayoutElement component2 = ((Component) cDisplayClass90.widget_go.GetComponentInChildren<LocText>()).GetComponent<LayoutElement>();
    double num1;
    float num2 = (float) (num1 = 83.0);
    component2.minWidth = (float) num1;
    component2.preferredWidth = num2;
    // ISSUE: reference to a compiler-generated field
    return cDisplayClass90.widget_go;
  }

  public class ToolTipCallbacks
  {
    public Action<IAssignableIdentity, GameObject, ToolTip> headerTooltip;
    public Action<IAssignableIdentity, GameObject, ToolTip> headerSortTooltip;
    public Action<IAssignableIdentity, GameObject, ToolTip> headerDropdownTooltip;
  }
}
