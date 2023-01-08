// Decompiled with JetBrains decompiler
// Type: SingleCheckboxSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;

public class SingleCheckboxSideScreen : SideScreenContent
{
  public KToggle toggle;
  public KImage toggleCheckMark;
  public LocText label;
  private ICheckboxControl target;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.toggle.onValueChanged += new Action<bool>(this.OnValueChanged);
  }

  public override bool IsValidForTarget(GameObject target) => target.GetComponent<ICheckboxControl>() != null || target.GetSMI<ICheckboxControl>() != null;

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    if (Object.op_Equality((Object) target, (Object) null))
    {
      Debug.LogError((object) "The target object provided was null");
    }
    else
    {
      this.target = target.GetComponent<ICheckboxControl>();
      if (this.target == null)
        this.target = target.GetSMI<ICheckboxControl>();
      if (this.target == null)
      {
        Debug.LogError((object) "The target provided does not have an ICheckboxControl component");
      }
      else
      {
        ((TMP_Text) this.label).text = this.target.CheckboxLabel;
        ((Component) ((Component) this.toggle).transform.parent).GetComponent<ToolTip>().SetSimpleTooltip(this.target.CheckboxTooltip);
        this.titleKey = this.target.CheckboxTitleKey;
        this.toggle.isOn = this.target.GetCheckboxValue();
        ((Behaviour) this.toggleCheckMark).enabled = this.toggle.isOn;
      }
    }
  }

  public override void ClearTarget()
  {
    base.ClearTarget();
    this.target = (ICheckboxControl) null;
  }

  private void OnValueChanged(bool value)
  {
    this.target.SetCheckboxValue(value);
    ((Behaviour) this.toggleCheckMark).enabled = value;
  }
}
