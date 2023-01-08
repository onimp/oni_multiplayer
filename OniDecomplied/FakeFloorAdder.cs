// Decompiled with JetBrains decompiler
// Type: FakeFloorAdder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FakeFloorAdder : KMonoBehaviour
{
  public CellOffset[] floorOffsets;
  public bool initiallyActive = true;
  private bool isActive;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (!this.initiallyActive)
      return;
    this.SetFloor(true);
  }

  public void SetFloor(bool active)
  {
    if (this.isActive == active)
      return;
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    Building component = ((Component) this).GetComponent<Building>();
    foreach (CellOffset floorOffset in this.floorOffsets)
    {
      CellOffset rotatedOffset = component.GetRotatedOffset(floorOffset);
      int num = Grid.OffsetCell(cell, rotatedOffset);
      if (active)
        Grid.FakeFloor.Add(num);
      else
        Grid.FakeFloor.Remove(num);
      Pathfinding.Instance.AddDirtyNavGridCell(num);
    }
    this.isActive = active;
  }

  protected virtual void OnCleanUp()
  {
    this.SetFloor(false);
    base.OnCleanUp();
  }
}
