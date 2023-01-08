// Decompiled with JetBrains decompiler
// Type: PrioritizeRowTableColumn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class PrioritizeRowTableColumn : TableColumn
{
  public object userData;
  private Action<object, int> onChangePriority;
  private Func<object, int, string> onHoverWidget;

  public PrioritizeRowTableColumn(
    object user_data,
    Action<object, int> on_change_priority,
    Func<object, int, string> on_hover_widget)
    : base((Action<IAssignableIdentity, GameObject>) null, (Comparison<IAssignableIdentity>) null)
  {
    this.userData = user_data;
    this.onChangePriority = on_change_priority;
    this.onHoverWidget = on_hover_widget;
  }

  public override GameObject GetMinionWidget(GameObject parent) => this.GetWidget(parent);

  public override GameObject GetDefaultWidget(GameObject parent) => this.GetWidget(parent);

  public override GameObject GetHeaderWidget(GameObject parent) => Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.PrioritizeRowHeaderWidget, parent, true);

  private GameObject GetWidget(GameObject parent)
  {
    GameObject widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.PrioritizeRowWidget, parent, true);
    HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
    this.ConfigureButton(component, "UpButton", 1, widget_go);
    this.ConfigureButton(component, "DownButton", -1, widget_go);
    return widget_go;
  }

  private void ConfigureButton(
    HierarchyReferences refs,
    string ref_id,
    int delta,
    GameObject widget_go)
  {
    KButton reference = refs.GetReference(ref_id) as KButton;
    reference.onClick += (System.Action) (() => this.onChangePriority((object) widget_go, delta));
    ToolTip component = ((Component) reference).GetComponent<ToolTip>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.OnToolTip = (Func<string>) (() => this.onHoverWidget((object) widget_go, delta));
  }
}
