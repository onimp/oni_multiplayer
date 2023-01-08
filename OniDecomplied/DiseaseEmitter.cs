// Decompiled with JetBrains decompiler
// Type: DiseaseEmitter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DiseaseEmitter")]
public class DiseaseEmitter : KMonoBehaviour
{
  [Serialize]
  public float emitRate = 1f;
  [Serialize]
  public byte emitRange;
  [Serialize]
  public int emitCount;
  [Serialize]
  public byte[] emitDiseases;
  public int[] simHandles;
  [Serialize]
  private bool enableEmitter;

  public float EmitRate => this.emitRate;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.emitDiseases != null)
    {
      this.simHandles = new int[this.emitDiseases.Length];
      for (int index = 0; index < this.simHandles.Length; ++index)
        this.simHandles[index] = -1;
    }
    this.SimRegister();
  }

  protected virtual void OnCleanUp()
  {
    this.SimUnregister();
    base.OnCleanUp();
  }

  public void SetEnable(bool enable)
  {
    if (this.enableEmitter == enable)
      return;
    this.enableEmitter = enable;
    if (this.enableEmitter)
      this.SimRegister();
    else
      this.SimUnregister();
  }

  private void OnCellChanged()
  {
    if (this.simHandles == null || !this.enableEmitter)
      return;
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    if (!Grid.IsValidCell(cell))
      return;
    for (int index = 0; index < this.emitDiseases.Length; ++index)
    {
      if (Sim.IsValidHandle(this.simHandles[index]))
        SimMessages.ModifyDiseaseEmitter(this.simHandles[index], cell, this.emitRange, this.emitDiseases[index], this.emitRate, this.emitCount);
    }
  }

  private void SimRegister()
  {
    if (this.simHandles == null || !this.enableEmitter)
      return;
    Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChanged), "DiseaseEmitter.Modify");
    for (int index = 0; index < this.simHandles.Length; ++index)
    {
      if (this.simHandles[index] == -1)
      {
        this.simHandles[index] = -2;
        SimMessages.AddDiseaseEmitter(Game.Instance.simComponentCallbackManager.Add(new Action<int, object>(DiseaseEmitter.OnSimRegisteredCallback), (object) this, nameof (DiseaseEmitter)).index);
      }
    }
  }

  private void SimUnregister()
  {
    if (this.simHandles == null)
      return;
    for (int index = 0; index < this.simHandles.Length; ++index)
    {
      if (Sim.IsValidHandle(this.simHandles[index]))
        SimMessages.RemoveDiseaseEmitter(-1, this.simHandles[index]);
      this.simHandles[index] = -1;
    }
    Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transform, new System.Action(this.OnCellChanged));
  }

  private static void OnSimRegisteredCallback(int handle, object data) => ((DiseaseEmitter) data).OnSimRegistered(handle);

  private void OnSimRegistered(int handle)
  {
    bool flag = false;
    if (Object.op_Inequality((Object) this, (Object) null))
    {
      for (int index = 0; index < this.simHandles.Length; ++index)
      {
        if (this.simHandles[index] == -2)
        {
          this.simHandles[index] = handle;
          flag = true;
          break;
        }
      }
      this.OnCellChanged();
    }
    if (flag)
      return;
    SimMessages.RemoveDiseaseEmitter(-1, handle);
  }

  public void SetDiseases(List<Klei.AI.Disease> diseases)
  {
    this.emitDiseases = new byte[diseases.Count];
    for (int index = 0; index < diseases.Count; ++index)
      this.emitDiseases[index] = Db.Get().Diseases.GetIndex(diseases[index].id);
  }
}
