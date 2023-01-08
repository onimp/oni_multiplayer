// Decompiled with JetBrains decompiler
// Type: SocialChoreTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class SocialChoreTracker
{
  public Func<int, Chore> CreateChoreCB;
  public int choreCount;
  private GameObject owner;
  private CellOffset[] choreOffsets;
  private Chore[] chores;
  private HandleVector<int>.Handle validNavCellChangedPartitionerEntry;
  private bool updating;

  public SocialChoreTracker(GameObject owner, CellOffset[] chore_offsets)
  {
    this.owner = owner;
    this.choreOffsets = chore_offsets;
    this.chores = new Chore[this.choreOffsets.Length];
    Extents extents = new Extents(Grid.PosToCell(owner), this.choreOffsets);
    this.validNavCellChangedPartitionerEntry = GameScenePartitioner.Instance.Add("PrintingPodSocialize", (object) owner, extents, GameScenePartitioner.Instance.validNavCellChangedLayer, new Action<object>(this.OnCellChanged));
  }

  public void Update(bool update = true)
  {
    if (this.updating)
      return;
    this.updating = true;
    int num = 0;
    for (int index = 0; index < this.choreOffsets.Length; ++index)
    {
      CellOffset choreOffset = this.choreOffsets[index];
      Chore chore = this.chores[index];
      if ((!update || num >= this.choreCount ? 0 : (this.IsOffsetValid(choreOffset) ? 1 : 0)) != 0)
      {
        ++num;
        if (chore == null || chore.isComplete)
          this.chores[index] = this.CreateChoreCB != null ? this.CreateChoreCB(index) : (Chore) null;
      }
      else if (chore != null)
      {
        chore.Cancel("locator invalidated");
        this.chores[index] = (Chore) null;
      }
    }
    this.updating = false;
  }

  private void OnCellChanged(object data)
  {
    if (!this.owner.HasTag(GameTags.Operational))
      return;
    this.Update();
  }

  public void Clear()
  {
    GameScenePartitioner.Instance.Free(ref this.validNavCellChangedPartitionerEntry);
    this.Update(false);
  }

  private bool IsOffsetValid(CellOffset offset)
  {
    int cell = Grid.OffsetCell(Grid.PosToCell(this.owner), offset);
    return GameNavGrids.FloorValidator.IsWalkableCell(cell, Grid.CellBelow(cell), true);
  }
}
