// Decompiled with JetBrains decompiler
// Type: FoundationMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FoundationMonitor")]
public class FoundationMonitor : KMonoBehaviour
{
  private int position;
  [Serialize]
  public bool needsFoundation = true;
  [Serialize]
  private bool hasFoundation = true;
  public CellOffset[] monitorCells = new CellOffset[1]
  {
    new CellOffset(0, -1)
  };
  private List<HandleVector<int>.Handle> partitionerEntries = new List<HandleVector<int>.Handle>();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.position = Grid.PosToCell(((Component) this).gameObject);
    foreach (CellOffset monitorCell in this.monitorCells)
    {
      int cell = Grid.OffsetCell(this.position, monitorCell);
      if (Grid.IsValidCell(this.position) && Grid.IsValidCell(cell))
        this.partitionerEntries.Add(GameScenePartitioner.Instance.Add("FoundationMonitor.OnSpawn", (object) ((Component) this).gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnGroundChanged)));
      this.OnGroundChanged((object) null);
    }
  }

  protected virtual void OnCleanUp()
  {
    foreach (HandleVector<int>.Handle partitionerEntry in this.partitionerEntries)
      GameScenePartitioner.Instance.Free(ref partitionerEntry);
    base.OnCleanUp();
  }

  public bool CheckFoundationValid() => !this.needsFoundation || this.IsSuitableFoundation(this.position);

  public bool IsSuitableFoundation(int cell)
  {
    bool flag = true;
    foreach (CellOffset monitorCell in this.monitorCells)
    {
      if (!Grid.IsCellOffsetValid(cell, monitorCell))
        return false;
      int i = Grid.OffsetCell(cell, monitorCell);
      flag = Grid.Solid[i];
      if (!flag)
        break;
    }
    return flag;
  }

  public void OnGroundChanged(object callbackData)
  {
    if (!this.hasFoundation && this.CheckFoundationValid())
    {
      this.hasFoundation = true;
      ((Component) this).GetComponent<KPrefabID>().RemoveTag(GameTags.Creatures.HasNoFoundation);
      this.Trigger(-1960061727, (object) null);
    }
    if (!this.hasFoundation || this.CheckFoundationValid())
      return;
    this.hasFoundation = false;
    ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.Creatures.HasNoFoundation, false);
    this.Trigger(-1960061727, (object) null);
  }
}
