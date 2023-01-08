// Decompiled with JetBrains decompiler
// Type: SubmersionMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SubmersionMonitor")]
public class SubmersionMonitor : KMonoBehaviour, IGameObjectEffectDescriptor, IWiltCause, ISim1000ms
{
  private int position;
  private bool dry;
  protected float cellLiquidThreshold = 0.2f;
  private Extents extents;
  private HandleVector<int>.Handle partitionerEntry;

  public bool Dry => this.dry;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.OnMove();
    this.CheckDry();
    Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnMove), "SubmersionMonitor.OnSpawn");
  }

  private void OnMove()
  {
    this.position = Grid.PosToCell(((Component) this).gameObject);
    if (this.partitionerEntry.IsValid())
    {
      GameScenePartitioner.Instance.UpdatePosition(this.partitionerEntry, this.position);
    }
    else
    {
      Vector2I xy = Grid.PosToXY(TransformExtensions.GetPosition(this.transform));
      this.partitionerEntry = GameScenePartitioner.Instance.Add("DrowningMonitor.OnSpawn", (object) ((Component) this).gameObject, new Extents(xy.x, xy.y, 1, 2), GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnLiquidChanged));
    }
    this.CheckDry();
  }

  private void OnDrawGizmosSelected()
  {
  }

  protected virtual void OnCleanUp()
  {
    Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transform, new System.Action(this.OnMove));
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    base.OnCleanUp();
  }

  public void Configure(float _maxStamina, float _staminaRegenRate, float _cellLiquidThreshold = 0.95f) => this.cellLiquidThreshold = _cellLiquidThreshold;

  public void Sim1000ms(float dt) => this.CheckDry();

  private void CheckDry()
  {
    if (!this.IsCellSafe())
    {
      if (this.dry)
        return;
      this.dry = true;
      this.Trigger(-2057657673, (object) null);
    }
    else
    {
      if (!this.dry)
        return;
      this.dry = false;
      this.Trigger(1555379996, (object) null);
    }
  }

  public bool IsCellSafe()
  {
    int cell = Grid.PosToCell(((Component) this).gameObject);
    return Grid.IsValidCell(cell) && Grid.IsSubstantialLiquid(cell, this.cellLiquidThreshold);
  }

  private void OnLiquidChanged(object data) => this.CheckDry();

  WiltCondition.Condition[] IWiltCause.Conditions => new WiltCondition.Condition[1]
  {
    WiltCondition.Condition.DryingOut
  };

  public string WiltStateString => this.Dry ? Db.Get().CreatureStatusItems.DryingOut.resolveStringCallback((string) CREATURES.STATUSITEMS.DRYINGOUT.NAME, (object) this) : "";

  public void SetIncapacitated(bool state)
  {
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    descriptors.Add(new Descriptor((string) UI.GAMEOBJECTEFFECTS.REQUIRES_SUBMERSION, (string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_SUBMERSION, (Descriptor.DescriptorType) 0, false));
    return descriptors;
  }
}
