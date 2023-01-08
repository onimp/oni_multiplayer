// Decompiled with JetBrains decompiler
// Type: MinionVitalsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class MinionVitalsScreen : TargetScreen
{
  public MinionVitalsPanel panel;

  public override bool IsValidForTarget(GameObject target) => Object.op_Implicit((Object) target.GetComponent<MinionIdentity>());

  public virtual void ScreenUpdate(bool topLevel) => base.ScreenUpdate(topLevel);

  public override void OnSelectTarget(GameObject target)
  {
    this.panel.selectedEntity = target;
    this.panel.Refresh();
  }

  public override void OnDeselectTarget(GameObject target)
  {
  }

  protected virtual void OnActivate()
  {
    base.OnActivate();
    if (Object.op_Equality((Object) this.panel, (Object) null))
      this.panel = ((Component) this).GetComponent<MinionVitalsPanel>();
    this.panel.Init();
  }
}
