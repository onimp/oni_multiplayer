// Decompiled with JetBrains decompiler
// Type: TeleporterWorkableUse
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class TeleporterWorkableUse : Workable
{
  protected override void OnPrefabInit() => base.OnPrefabInit();

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.SetWorkTime(5f);
    this.resetProgressOnStop = true;
  }

  protected override void OnStartWork(Worker worker)
  {
    Teleporter component = ((Component) this).GetComponent<Teleporter>();
    Teleporter teleportTarget = component.FindTeleportTarget();
    component.SetTeleportTarget(teleportTarget);
    TeleportalPad.StatesInstance smi = ((Component) teleportTarget).GetSMI<TeleportalPad.StatesInstance>();
    smi.sm.targetTeleporter.Trigger(smi);
  }

  protected override void OnStopWork(Worker worker)
  {
    TeleportalPad.StatesInstance smi = ((Component) this).GetSMI<TeleportalPad.StatesInstance>();
    smi.sm.doTeleport.Trigger(smi);
  }
}
