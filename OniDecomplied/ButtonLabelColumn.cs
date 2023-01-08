// Decompiled with JetBrains decompiler
// Type: ButtonLabelColumn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ButtonLabelColumn : LabelTableColumn
{
  private Action<GameObject> on_click_action;
  private Action<GameObject> on_double_click_action;
  private bool whiteText;

  public ButtonLabelColumn(
    Action<IAssignableIdentity, GameObject> on_load_action,
    Func<IAssignableIdentity, GameObject, string> get_value_action,
    Action<GameObject> on_click_action,
    Action<GameObject> on_double_click_action,
    Comparison<IAssignableIdentity> sort_comparison,
    Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip,
    Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip,
    bool whiteText = false)
    : base(on_load_action, get_value_action, sort_comparison, on_tooltip, on_sort_tooltip)
  {
    this.on_click_action = on_click_action;
    this.on_double_click_action = on_double_click_action;
    this.whiteText = whiteText;
  }

  public override GameObject GetDefaultWidget(GameObject parent)
  {
    GameObject widget_go = Util.KInstantiateUI(this.whiteText ? Assets.UIPrefabs.TableScreenWidgets.ButtonLabelWhite : Assets.UIPrefabs.TableScreenWidgets.ButtonLabel, parent, true);
    if (this.on_click_action != null)
      widget_go.GetComponent<KButton>().onClick += (System.Action) (() => this.on_click_action(widget_go));
    if (this.on_double_click_action != null)
      widget_go.GetComponent<KButton>().onDoubleClick += (System.Action) (() => this.on_double_click_action(widget_go));
    return widget_go;
  }

  public override GameObject GetHeaderWidget(GameObject parent) => base.GetHeaderWidget(parent);

  public override GameObject GetMinionWidget(GameObject parent)
  {
    GameObject widget_go = Util.KInstantiateUI(this.whiteText ? Assets.UIPrefabs.TableScreenWidgets.ButtonLabelWhite : Assets.UIPrefabs.TableScreenWidgets.ButtonLabel, parent, true);
    ToolTip tt = widget_go.GetComponent<ToolTip>();
    tt.OnToolTip = (Func<string>) (() => this.GetTooltip(tt));
    if (this.on_click_action != null)
      widget_go.GetComponent<KButton>().onClick += (System.Action) (() => this.on_click_action(widget_go));
    if (this.on_double_click_action != null)
      widget_go.GetComponent<KButton>().onDoubleClick += (System.Action) (() => this.on_double_click_action(widget_go));
    return widget_go;
  }
}
