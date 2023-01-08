// Decompiled with JetBrains decompiler
// Type: RangedAttackable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class RangedAttackable : AttackableBase
{
  protected override void OnPrefabInit() => base.OnPrefabInit();

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.preferUnreservedCell = true;
    this.SetOffsetTable(OffsetGroups.InvertedStandardTable);
  }

  public new int GetCell() => Grid.PosToCell((KMonoBehaviour) this);

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = new Color(0.0f, 0.5f, 0.5f, 0.15f);
    foreach (CellOffset offset in this.GetOffsets())
      Gizmos.DrawCube(Vector3.op_Addition(new Vector3(0.5f, 0.5f, 0.0f), Grid.CellToPos(Grid.OffsetCell(Grid.PosToCell(((Component) this).gameObject), offset))), Vector3.one);
  }
}
