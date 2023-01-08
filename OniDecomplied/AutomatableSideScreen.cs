// Decompiled with JetBrains decompiler
// Type: AutomatableSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class AutomatableSideScreen : SideScreenContent
{
  public KToggle allowManualToggle;
  public KImage allowManualToggleCheckMark;
  public GameObject content;
  private GameObject target;
  public LocText DescriptionText;
  private Automatable targetAutomatable;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((Component) ((Component) this.allowManualToggle).transform.parent).GetComponent<ToolTip>().SetSimpleTooltip((string) UI.UISIDESCREENS.AUTOMATABLE_SIDE_SCREEN.ALLOWMANUALBUTTONTOOLTIP);
    this.allowManualToggle.onValueChanged += new Action<bool>(this.OnAllowManualChanged);
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<Automatable>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    if (Object.op_Equality((Object) target, (Object) null))
    {
      Debug.LogError((object) "The target object provided was null");
    }
    else
    {
      this.targetAutomatable = target.GetComponent<Automatable>();
      if (Object.op_Equality((Object) this.targetAutomatable, (Object) null))
      {
        Debug.LogError((object) "The target provided does not have an Automatable component");
      }
      else
      {
        this.allowManualToggle.isOn = !this.targetAutomatable.GetAutomationOnly();
        ((Behaviour) this.allowManualToggleCheckMark).enabled = this.allowManualToggle.isOn;
      }
    }
  }

  private void OnAllowManualChanged(bool value)
  {
    this.targetAutomatable.SetAutomationOnly(!value);
    ((Behaviour) this.allowManualToggleCheckMark).enabled = value;
  }
}
