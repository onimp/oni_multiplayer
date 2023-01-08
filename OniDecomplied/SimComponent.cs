// Decompiled with JetBrains decompiler
// Type: SimComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Diagnostics;
using UnityEngine;

public abstract class SimComponent : KMonoBehaviour, ISim200ms
{
  [SerializeField]
  protected int simHandle = -1;
  private bool simActive = true;
  private bool dirty = true;

  public bool IsSimActive => this.simActive;

  protected virtual void OnSimRegister(
    HandleVector<Game.ComplexCallbackInfo<int>>.Handle cb_handle)
  {
  }

  protected virtual void OnSimRegistered()
  {
  }

  protected virtual void OnSimActivate()
  {
  }

  protected virtual void OnSimDeactivate()
  {
  }

  protected virtual void OnSimUnregister()
  {
  }

  protected abstract Action<int> GetStaticUnregister();

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.SimRegister();
  }

  protected virtual void OnCleanUp()
  {
    this.SimUnregister();
    base.OnCleanUp();
  }

  public void SetSimActive(bool active)
  {
    this.simActive = active;
    this.dirty = true;
  }

  public void Sim200ms(float dt)
  {
    if (!Sim.IsValidHandle(this.simHandle))
      return;
    this.UpdateSimState();
  }

  private void UpdateSimState()
  {
    if (!this.dirty)
      return;
    this.dirty = false;
    if (this.simActive)
      this.OnSimActivate();
    else
      this.OnSimDeactivate();
  }

  private void SimRegister()
  {
    if (!this.isSpawned || this.simHandle != -1)
      return;
    this.simHandle = -2;
    Action<int> static_unregister = this.GetStaticUnregister();
    this.OnSimRegister(Game.Instance.simComponentCallbackManager.Add((Action<int, object>) ((handle, data) => SimComponent.OnSimRegistered(this, handle, static_unregister)), (object) this, "SimComponent.SimRegister"));
  }

  private void SimUnregister()
  {
    if (Sim.IsValidHandle(this.simHandle))
      this.OnSimUnregister();
    this.simHandle = -1;
  }

  private static void OnSimRegistered(
    SimComponent instance,
    int handle,
    Action<int> static_unregister)
  {
    if (Object.op_Inequality((Object) instance, (Object) null))
    {
      instance.simHandle = handle;
      instance.OnSimRegistered();
    }
    else
      static_unregister(handle);
  }

  [Conditional("ENABLE_LOGGER")]
  protected void Log(string msg)
  {
  }
}
