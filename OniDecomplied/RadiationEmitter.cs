// Decompiled with JetBrains decompiler
// Type: RadiationEmitter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class RadiationEmitter : SimComponent
{
  public bool radiusProportionalToRads;
  [SerializeField]
  public short emitRadiusX = 4;
  [SerializeField]
  public short emitRadiusY = 4;
  [SerializeField]
  public float emitRads = 10f;
  [SerializeField]
  public float emitRate = 1f;
  [SerializeField]
  public float emitSpeed = 1f;
  [SerializeField]
  public float emitDirection;
  [SerializeField]
  public float emitAngle = 360f;
  [SerializeField]
  public RadiationEmitter.RadiationEmitterType emitType;
  [SerializeField]
  public Vector3 emissionOffset = Vector3.zero;

  protected override void OnSpawn()
  {
    Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange), "RadiationEmitter.OnSpawn");
    base.OnSpawn();
  }

  protected override void OnCleanUp()
  {
    Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange));
    base.OnCleanUp();
  }

  public void SetEmitting(bool emitting) => this.SetSimActive(emitting);

  public int GetEmissionCell() => Grid.PosToCell(Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), this.emissionOffset));

  public void Refresh()
  {
    int emissionCell = this.GetEmissionCell();
    if (this.radiusProportionalToRads)
      this.SetRadiusProportionalToRads();
    SimMessages.ModifyRadiationEmitter(this.simHandle, emissionCell, this.emitRadiusX, this.emitRadiusY, this.emitRads, this.emitRate, this.emitSpeed, this.emitDirection, this.emitAngle, this.emitType);
  }

  private void OnCellChange() => this.Refresh();

  private void SetRadiusProportionalToRads()
  {
    this.emitRadiusX = (short) Mathf.Clamp(Mathf.RoundToInt(this.emitRads * 1f), 1, 128);
    this.emitRadiusY = (short) Mathf.Clamp(Mathf.RoundToInt(this.emitRads * 1f), 1, 128);
  }

  protected override void OnSimActivate()
  {
    int emissionCell = this.GetEmissionCell();
    if (this.radiusProportionalToRads)
      this.SetRadiusProportionalToRads();
    SimMessages.ModifyRadiationEmitter(this.simHandle, emissionCell, this.emitRadiusX, this.emitRadiusY, this.emitRads, this.emitRate, this.emitSpeed, this.emitDirection, this.emitAngle, this.emitType);
  }

  protected override void OnSimDeactivate() => SimMessages.ModifyRadiationEmitter(this.simHandle, this.GetEmissionCell(), (short) 0, (short) 0, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, this.emitType);

  protected override void OnSimRegister(
    HandleVector<Game.ComplexCallbackInfo<int>>.Handle cb_handle)
  {
    Game.Instance.simComponentCallbackManager.GetItem(cb_handle);
    int emissionCell = this.GetEmissionCell();
    SimMessages.AddRadiationEmitter(cb_handle.index, emissionCell, (short) 0, (short) 0, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, this.emitType);
  }

  protected override void OnSimUnregister() => RadiationEmitter.StaticUnregister(this.simHandle);

  private static void StaticUnregister(int sim_handle)
  {
    Debug.Assert(Sim.IsValidHandle(sim_handle));
    SimMessages.RemoveRadiationEmitter(-1, sim_handle);
  }

  private void OnDrawGizmosSelected()
  {
    int emissionCell = this.GetEmissionCell();
    Gizmos.color = Color.green;
    Gizmos.DrawSphere(Vector3.op_Addition(Vector3.op_Addition(Grid.CellToPos(emissionCell), Vector3.op_Division(Vector3.right, 2f)), Vector3.op_Division(Vector3.up, 2f)), 0.2f);
  }

  protected override Action<int> GetStaticUnregister() => new Action<int>(RadiationEmitter.StaticUnregister);

  public enum RadiationEmitterType
  {
    Constant,
    Pulsing,
    PulsingAveraged,
    SimplePulse,
    RadialBeams,
    Attractor,
  }
}
