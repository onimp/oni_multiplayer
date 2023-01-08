// Decompiled with JetBrains decompiler
// Type: ArtifactModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ArtifactModule : SingleEntityReceptacle, IRenderEveryTick
{
  [MyCmpReq]
  private KBatchedAnimController animController;
  [MyCmpReq]
  private RocketModuleCluster module;
  private Clustercraft craft;

  protected override void OnSpawn()
  {
    this.craft = ((Component) this.module.CraftInterface).GetComponent<Clustercraft>();
    if (this.craft.Status == Clustercraft.CraftStatus.InFlight && Object.op_Inequality((Object) this.occupyingObject, (Object) null))
      this.occupyingObject.SetActive(false);
    base.OnSpawn();
    this.Subscribe(705820818, new Action<object>(this.OnEnterSpace));
    this.Subscribe(-1165815793, new Action<object>(this.OnExitSpace));
  }

  public void RenderEveryTick(float dt) => this.ArtifactTrackModulePosition();

  private void ArtifactTrackModulePosition()
  {
    this.occupyingObjectRelativePosition = Vector3.op_Addition(Vector3.op_Addition(this.animController.Offset, Vector3.op_Multiply(Vector3.up, 0.5f)), new Vector3(0.0f, 0.0f, -1f));
    if (!Object.op_Inequality((Object) this.occupyingObject, (Object) null))
      return;
    this.PositionOccupyingObject();
  }

  private void OnEnterSpace(object data)
  {
    if (!Object.op_Inequality((Object) this.occupyingObject, (Object) null))
      return;
    this.occupyingObject.SetActive(false);
  }

  private void OnExitSpace(object data)
  {
    if (!Object.op_Inequality((Object) this.occupyingObject, (Object) null))
      return;
    this.occupyingObject.SetActive(true);
  }
}
