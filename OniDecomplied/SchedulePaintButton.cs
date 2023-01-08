// Decompiled with JetBrains decompiler
// Type: SchedulePaintButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SchedulePaintButton")]
public class SchedulePaintButton : KMonoBehaviour
{
  [SerializeField]
  private LocText label;
  [SerializeField]
  private ImageToggleState toggleState;
  [SerializeField]
  private MultiToggle toggle;
  [SerializeField]
  private ToolTip toolTip;

  public ScheduleGroup group { get; private set; }

  public void SetGroup(
    ScheduleGroup group,
    Dictionary<string, ColorStyleSetting> styles,
    Action<SchedulePaintButton> onClick)
  {
    this.group = group;
    if (styles.ContainsKey(group.Id))
      this.toggleState.SetColorStyle(styles[group.Id]);
    ((TMP_Text) this.label).text = group.Name;
    this.toggle.onClick += (System.Action) (() => onClick(this));
    this.toolTip.SetSimpleTooltip(group.GetTooltip());
    ((Object) ((Component) this).gameObject).name = "PaintButton_" + group.Id;
  }

  public void SetToggle(bool on) => this.toggle.ChangeState(on ? 1 : 0);
}
