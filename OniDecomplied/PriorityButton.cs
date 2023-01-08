// Decompiled with JetBrains decompiler
// Type: PriorityButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/PriorityButton")]
public class PriorityButton : KMonoBehaviour
{
  public KToggle toggle;
  public LocText text;
  public ToolTip tooltip;
  [MyCmpGet]
  private ImageToggleState its;
  public ColorStyleSetting normalStyle;
  public ColorStyleSetting highStyle;
  public bool playSelectionSound = true;
  public Action<PrioritySetting> onClick;
  private PrioritySetting _priority;

  public PrioritySetting priority
  {
    get => this._priority;
    set
    {
      this._priority = value;
      if (!Object.op_Inequality((Object) this.its, (Object) null))
        return;
      this.its.colorStyleSetting = this.priority.priority_class != PriorityScreen.PriorityClass.high ? this.normalStyle : this.highStyle;
      this.its.RefreshColorStyle();
      this.its.ResetColor();
    }
  }

  protected virtual void OnPrefabInit() => this.toggle.onClick += new System.Action(this.OnClick);

  private void OnClick()
  {
    if (this.playSelectionSound)
      PriorityScreen.PlayPriorityConfirmSound(this.priority);
    if (this.onClick == null)
      return;
    this.onClick(this.priority);
  }
}
