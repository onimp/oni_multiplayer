// Decompiled with JetBrains decompiler
// Type: FloorSwitchActivator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FloorSwitchActivator")]
public class FloorSwitchActivator : KMonoBehaviour
{
  [MyCmpReq]
  private PrimaryElement primaryElement;
  private bool registered;
  private HandleVector<int>.Handle partitionerEntry;
  private int last_cell_occupied = -1;

  public PrimaryElement PrimaryElement => this.primaryElement;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Register();
    this.OnCellChange();
  }

  protected virtual void OnCleanUp()
  {
    this.Unregister();
    base.OnCleanUp();
  }

  private void OnCellChange()
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    GameScenePartitioner.Instance.UpdatePosition(this.partitionerEntry, cell);
    if (Grid.IsValidCell(this.last_cell_occupied) && cell != this.last_cell_occupied)
      this.NotifyChanged(this.last_cell_occupied);
    this.NotifyChanged(cell);
    this.last_cell_occupied = cell;
  }

  private void NotifyChanged(int cell) => GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.floorSwitchActivatorChangedLayer, (object) this);

  protected virtual void OnCmpEnable()
  {
    base.OnCmpEnable();
    this.Register();
  }

  protected virtual void OnCmpDisable()
  {
    this.Unregister();
    base.OnCmpDisable();
  }

  private void Register()
  {
    if (this.registered)
      return;
    this.partitionerEntry = GameScenePartitioner.Instance.Add("FloorSwitchActivator.Register", (object) this, Grid.PosToCell((KMonoBehaviour) this), GameScenePartitioner.Instance.floorSwitchActivatorLayer, (Action<object>) null);
    Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange), "FloorSwitchActivator.Register");
    this.registered = true;
  }

  private void Unregister()
  {
    if (!this.registered)
      return;
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange));
    if (this.last_cell_occupied > -1)
      this.NotifyChanged(this.last_cell_occupied);
    this.registered = false;
  }
}
