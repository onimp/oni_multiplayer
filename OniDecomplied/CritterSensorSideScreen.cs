// Decompiled with JetBrains decompiler
// Type: CritterSensorSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class CritterSensorSideScreen : SideScreenContent
{
  public LogicCritterCountSensor targetSensor;
  public KToggle countCrittersToggle;
  public KToggle countEggsToggle;
  public KImage crittersCheckmark;
  public KImage eggsCheckmark;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.countCrittersToggle.onClick += new System.Action(this.ToggleCritters);
    this.countEggsToggle.onClick += new System.Action(this.ToggleEggs);
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<LogicCritterCountSensor>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.targetSensor = target.GetComponent<LogicCritterCountSensor>();
    ((Behaviour) this.crittersCheckmark).enabled = this.targetSensor.countCritters;
    ((Behaviour) this.eggsCheckmark).enabled = this.targetSensor.countEggs;
  }

  private void ToggleCritters()
  {
    this.targetSensor.countCritters = !this.targetSensor.countCritters;
    ((Behaviour) this.crittersCheckmark).enabled = this.targetSensor.countCritters;
  }

  private void ToggleEggs()
  {
    this.targetSensor.countEggs = !this.targetSensor.countEggs;
    ((Behaviour) this.eggsCheckmark).enabled = this.targetSensor.countEggs;
  }
}
