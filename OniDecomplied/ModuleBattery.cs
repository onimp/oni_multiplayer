// Decompiled with JetBrains decompiler
// Type: ModuleBattery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ModuleBattery : Battery
{
  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.connectedTags = new Tag[0];
    this.IsVirtual = true;
  }

  protected override void OnSpawn()
  {
    this.VirtualCircuitKey = (object) ((Component) this).GetComponent<RocketModuleCluster>().CraftInterface;
    base.OnSpawn();
    this.meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
  }
}
