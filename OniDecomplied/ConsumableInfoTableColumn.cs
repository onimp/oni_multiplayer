// Decompiled with JetBrains decompiler
// Type: ConsumableInfoTableColumn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;

public class ConsumableInfoTableColumn : CheckboxTableColumn
{
  public IConsumableUIItem consumable_info;
  public Func<GameObject, string> get_header_label;

  public ConsumableInfoTableColumn(
    IConsumableUIItem consumable_info,
    Action<IAssignableIdentity, GameObject> load_value_action,
    Func<IAssignableIdentity, GameObject, TableScreen.ResultValues> get_value_action,
    Action<GameObject> on_press_action,
    Action<GameObject, TableScreen.ResultValues> set_value_action,
    Comparison<IAssignableIdentity> sort_comparison,
    Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip,
    Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip,
    Func<GameObject, string> get_header_label)
    : base(load_value_action, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip, on_sort_tooltip, (Func<bool>) (() => DebugHandler.InstantBuildMode || ConsumerManager.instance.isDiscovered(TagExtensions.ToTag(consumable_info.ConsumableId))))
  {
    this.consumable_info = consumable_info;
    this.get_header_label = get_header_label;
  }

  public override GameObject GetHeaderWidget(GameObject parent)
  {
    GameObject headerWidget = base.GetHeaderWidget(parent);
    if (Object.op_Inequality((Object) headerWidget.GetComponentInChildren<LocText>(), (Object) null))
      ((TMP_Text) headerWidget.GetComponentInChildren<LocText>()).text = this.get_header_label(headerWidget);
    ((Component) headerWidget.GetComponentInChildren<MultiToggle>()).gameObject.SetActive(false);
    return headerWidget;
  }
}
