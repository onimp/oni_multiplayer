// Decompiled with JetBrains decompiler
// Type: Floodable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Floodable")]
public class Floodable : KMonoBehaviour
{
  [MyCmpReq]
  private Building building;
  [MyCmpReq]
  private PrimaryElement primaryElement;
  [MyCmpGet]
  private SimCellOccupier simCellOccupier;
  [MyCmpReq]
  private Operational operational;
  public static Operational.Flag notFloodedFlag = new Operational.Flag("not_flooded", Operational.Flag.Type.Functional);
  private bool isFlooded;
  private HandleVector<int>.Handle partitionerEntry;

  public bool IsFlooded => this.isFlooded;

  public BuildingDef Def => this.building.Def;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.partitionerEntry = GameScenePartitioner.Instance.Add("Floodable.OnSpawn", (object) ((Component) this).gameObject, this.building.GetExtents(), GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnElementChanged));
    this.OnElementChanged((object) null);
  }

  private void OnElementChanged(object data)
  {
    bool flag = false;
    for (int index = 0; index < this.building.PlacementCells.Length; ++index)
    {
      if (Grid.IsSubstantialLiquid(this.building.PlacementCells[index]))
      {
        flag = true;
        break;
      }
    }
    if (flag == this.isFlooded)
      return;
    this.isFlooded = flag;
    this.operational.SetFlag(Floodable.notFloodedFlag, !this.isFlooded);
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.Flooded, this.isFlooded, (object) this);
  }

  protected virtual void OnCleanUp() => GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
}
