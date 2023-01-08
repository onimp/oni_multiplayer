// Decompiled with JetBrains decompiler
// Type: ModuleGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ModuleGenerator : Generator
{
  private Clustercraft clustercraft;
  private Guid poweringStatusItemHandle;
  private Guid notPoweringStatusItemHandle;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.connectedTags = new Tag[0];
    this.IsVirtual = true;
  }

  protected override void OnSpawn()
  {
    CraftModuleInterface craftInterface = ((Component) this).GetComponent<RocketModuleCluster>().CraftInterface;
    this.VirtualCircuitKey = (object) craftInterface;
    this.clustercraft = ((Component) craftInterface).GetComponent<Clustercraft>();
    Game.Instance.electricalConduitSystem.AddToVirtualNetworks(this.VirtualCircuitKey, (object) this, true);
    base.OnSpawn();
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    Game.Instance.electricalConduitSystem.RemoveFromVirtualNetworks(this.VirtualCircuitKey, (object) this, true);
  }

  public override bool IsProducingPower() => this.clustercraft.IsFlightInProgress();

  public override void EnergySim200ms(float dt)
  {
    base.EnergySim200ms(dt);
    if (this.IsProducingPower())
    {
      this.GenerateJoules(this.WattageRating * dt);
      if (!(this.poweringStatusItemHandle == Guid.Empty))
        return;
      this.poweringStatusItemHandle = this.selectable.ReplaceStatusItem(this.notPoweringStatusItemHandle, Db.Get().BuildingStatusItems.ModuleGeneratorPowered, (object) this);
      this.notPoweringStatusItemHandle = Guid.Empty;
    }
    else
    {
      if (!(this.notPoweringStatusItemHandle == Guid.Empty))
        return;
      this.notPoweringStatusItemHandle = this.selectable.ReplaceStatusItem(this.poweringStatusItemHandle, Db.Get().BuildingStatusItems.ModuleGeneratorNotPowered, (object) this);
      this.poweringStatusItemHandle = Guid.Empty;
    }
  }
}
