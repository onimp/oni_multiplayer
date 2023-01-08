// Decompiled with JetBrains decompiler
// Type: ElementEmitter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ElementEmitter : SimComponent
{
  [SerializeField]
  public ElementConverter.OutputElement outputElement;
  [SerializeField]
  public float emissionFrequency = 1f;
  [SerializeField]
  public byte emitRange = 1;
  [SerializeField]
  public float maxPressure = 1f;
  private Guid statusHandle = Guid.Empty;
  public bool showDescriptor = true;
  private HandleVector<Game.CallbackInfo>.Handle onBlockedHandle = HandleVector<Game.CallbackInfo>.InvalidHandle;
  private HandleVector<Game.CallbackInfo>.Handle onUnblockedHandle = HandleVector<Game.CallbackInfo>.InvalidHandle;

  public bool isEmitterBlocked { get; private set; }

  protected override void OnSpawn()
  {
    this.onBlockedHandle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(new System.Action(this.OnEmitterBlocked), true));
    this.onUnblockedHandle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(new System.Action(this.OnEmitterUnblocked), true));
    base.OnSpawn();
  }

  protected override void OnCleanUp()
  {
    Game.Instance.ManualReleaseHandle(this.onBlockedHandle);
    Game.Instance.ManualReleaseHandle(this.onUnblockedHandle);
    base.OnCleanUp();
  }

  public void SetEmitting(bool emitting) => this.SetSimActive(emitting);

  protected override void OnSimActivate()
  {
    int game_cell = Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), (int) this.outputElement.outputElementOffset.x, (int) this.outputElement.outputElementOffset.y);
    if (this.outputElement.elementHash != (SimHashes) 0 && (double) this.outputElement.massGenerationRate > 0.0 && (double) this.emissionFrequency > 0.0)
    {
      float emit_temperature = (double) this.outputElement.minOutputTemperature == 0.0 ? ((Component) this).GetComponent<PrimaryElement>().Temperature : this.outputElement.minOutputTemperature;
      SimMessages.ModifyElementEmitter(this.simHandle, game_cell, (int) this.emitRange, this.outputElement.elementHash, this.emissionFrequency, this.outputElement.massGenerationRate, emit_temperature, this.maxPressure, this.outputElement.addedDiseaseIdx, this.outputElement.addedDiseaseCount);
    }
    if (!this.showDescriptor)
      return;
    this.statusHandle = ((Component) this).GetComponent<KSelectable>().ReplaceStatusItem(this.statusHandle, Db.Get().BuildingStatusItems.ElementEmitterOutput, (object) this);
  }

  protected override void OnSimDeactivate()
  {
    SimMessages.ModifyElementEmitter(this.simHandle, Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), (int) this.outputElement.outputElementOffset.x, (int) this.outputElement.outputElementOffset.y), (int) this.emitRange, SimHashes.Vacuum, 0.0f, 0.0f, 0.0f, 0.0f, byte.MaxValue, 0);
    if (!this.showDescriptor)
      return;
    this.statusHandle = ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle);
  }

  public void ForceEmit(float mass, byte disease_idx, int disease_count, float temperature = -1f)
  {
    if ((double) mass <= 0.0)
      return;
    float temperature1 = (double) temperature > 0.0 ? temperature : this.outputElement.minOutputTemperature;
    Element elementByHash = ElementLoader.FindElementByHash(this.outputElement.elementHash);
    if (elementByHash.IsGas || elementByHash.IsLiquid)
      SimMessages.AddRemoveSubstance(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), this.outputElement.elementHash, CellEventLogger.Instance.ElementConsumerSimUpdate, mass, temperature1, disease_idx, disease_count);
    else if (elementByHash.IsSolid)
      elementByHash.substance.SpawnResource(Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), new Vector3(0.0f, 0.5f, 0.0f)), mass, temperature1, disease_idx, disease_count, forceTemperature: true);
    PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, ElementLoader.FindElementByHash(this.outputElement.elementHash).name, ((Component) this).gameObject.transform);
  }

  private void OnEmitterBlocked()
  {
    this.isEmitterBlocked = true;
    this.Trigger(1615168894, (object) this);
  }

  private void OnEmitterUnblocked()
  {
    this.isEmitterBlocked = false;
    this.Trigger(-657992955, (object) this);
  }

  protected override void OnSimRegister(
    HandleVector<Game.ComplexCallbackInfo<int>>.Handle cb_handle)
  {
    Game.Instance.simComponentCallbackManager.GetItem(cb_handle);
    SimMessages.AddElementEmitter(this.maxPressure, cb_handle.index, this.onBlockedHandle.index, this.onUnblockedHandle.index);
  }

  protected override void OnSimUnregister() => ElementEmitter.StaticUnregister(this.simHandle);

  private static void StaticUnregister(int sim_handle)
  {
    Debug.Assert(Sim.IsValidHandle(sim_handle));
    SimMessages.RemoveElementEmitter(-1, sim_handle);
  }

  private void OnDrawGizmosSelected()
  {
    int cell = Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), (int) this.outputElement.outputElementOffset.x, (int) this.outputElement.outputElementOffset.y);
    Gizmos.color = Color.green;
    Gizmos.DrawSphere(Vector3.op_Addition(Vector3.op_Addition(Grid.CellToPos(cell), Vector3.op_Division(Vector3.right, 2f)), Vector3.op_Division(Vector3.up, 2f)), 0.2f);
  }

  protected override Action<int> GetStaticUnregister() => new Action<int>(ElementEmitter.StaticUnregister);
}
