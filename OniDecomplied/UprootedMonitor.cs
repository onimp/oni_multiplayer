// Decompiled with JetBrains decompiler
// Type: UprootedMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UprootedMonitor")]
public class UprootedMonitor : KMonoBehaviour
{
  private int position;
  [Serialize]
  public bool canBeUprooted = true;
  [Serialize]
  private bool uprooted;
  public CellOffset[] monitorCells = new CellOffset[1]
  {
    new CellOffset(0, -1)
  };
  private List<HandleVector<int>.Handle> partitionerEntries = new List<HandleVector<int>.Handle>();
  private static readonly EventSystem.IntraObjectHandler<UprootedMonitor> OnUprootedDelegate = new EventSystem.IntraObjectHandler<UprootedMonitor>((Action<UprootedMonitor, object>) ((component, data) =>
  {
    if (component.uprooted)
      return;
    ((Component) component).GetComponent<KPrefabID>().AddTag(GameTags.Uprooted, false);
    component.uprooted = true;
    component.Trigger(-216549700, (object) null);
  }));

  public bool IsUprooted => this.uprooted || ((Component) this).GetComponent<KPrefabID>().HasTag(GameTags.Uprooted);

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<UprootedMonitor>(-216549700, UprootedMonitor.OnUprootedDelegate);
    this.position = Grid.PosToCell(((Component) this).gameObject);
    foreach (CellOffset monitorCell in this.monitorCells)
    {
      int cell = Grid.OffsetCell(this.position, monitorCell);
      if (Grid.IsValidCell(this.position) && Grid.IsValidCell(cell))
        this.partitionerEntries.Add(GameScenePartitioner.Instance.Add("UprootedMonitor.OnSpawn", (object) ((Component) this).gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnGroundChanged)));
      this.OnGroundChanged((object) null);
    }
  }

  protected virtual void OnCleanUp()
  {
    foreach (HandleVector<int>.Handle partitionerEntry in this.partitionerEntries)
      GameScenePartitioner.Instance.Free(ref partitionerEntry);
    base.OnCleanUp();
  }

  public bool CheckTileGrowable() => !this.canBeUprooted || !this.uprooted && this.IsSuitableFoundation(this.position);

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
    if (!this.CheckTileGrowable())
      this.uprooted = true;
    if (!this.uprooted)
      return;
    ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.Uprooted, false);
    this.Trigger(-216549700, (object) null);
  }
}
