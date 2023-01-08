// Decompiled with JetBrains decompiler
// Type: CreatureHelpers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public static class CreatureHelpers
{
  public static bool isClear(int cell) => Grid.IsValidCell(cell) && !Grid.Solid[cell] && !Grid.IsSubstantialLiquid(cell, 0.9f) && (!Grid.IsValidCell(Grid.CellBelow(cell)) || !Grid.IsLiquid(cell) || !Grid.IsLiquid(Grid.CellBelow(cell)));

  public static int FindNearbyBreathableCell(int currentLocation, SimHashes breathableElement) => currentLocation;

  public static bool cellsAreClear(int[] cells)
  {
    for (int index = 0; index < cells.Length; ++index)
    {
      if (!Grid.IsValidCell(cells[index]) || !CreatureHelpers.isClear(cells[index]))
        return false;
    }
    return true;
  }

  public static Vector3 PositionOfCurrentCell(Vector3 transformPosition) => Grid.CellToPos(Grid.PosToCell(transformPosition));

  public static Vector3 CenterPositionOfCell(int cell) => Vector3.op_Addition(Grid.CellToPos(cell), new Vector3(0.5f, 0.5f, -2f));

  public static void DeselectCreature(GameObject creature)
  {
    KSelectable component = creature.GetComponent<KSelectable>();
    if (!Object.op_Inequality((Object) component, (Object) null) || !Object.op_Equality((Object) SelectTool.Instance.selected, (Object) component))
      return;
    SelectTool.Instance.Select((KSelectable) null);
  }

  public static bool isSwimmable(int cell) => Grid.IsValidCell(cell) && !Grid.Solid[cell] && Grid.IsSubstantialLiquid(cell);

  public static bool isSolidGround(int cell) => Grid.IsValidCell(cell) && Grid.Solid[cell];

  public static void FlipAnim(KAnimControllerBase anim, Vector3 heading)
  {
    if ((double) heading.x < 0.0)
    {
      anim.FlipX = true;
    }
    else
    {
      if ((double) heading.x <= 0.0)
        return;
      anim.FlipX = false;
    }
  }

  public static void FlipAnim(KBatchedAnimController anim, Vector3 heading)
  {
    if ((double) heading.x < 0.0)
    {
      anim.FlipX = true;
    }
    else
    {
      if ((double) heading.x <= 0.0)
        return;
      anim.FlipX = false;
    }
  }

  public static Vector3 GetWalkMoveTarget(Transform transform, Vector2 Heading)
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(transform));
    if ((double) Heading.x == 1.0)
    {
      if (CreatureHelpers.isClear(Grid.CellRight(cell)) && CreatureHelpers.isClear(Grid.CellDownRight(cell)) && CreatureHelpers.isClear(Grid.CellRight(Grid.CellRight(cell))) && !CreatureHelpers.isClear(Grid.PosToCell(Vector3.op_Addition(Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.op_Multiply(Vector3.right, 2f)), Vector3.down))))
        return Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.op_Multiply(Vector3.right, 2f));
      if (CreatureHelpers.cellsAreClear(new int[2]
      {
        Grid.CellRight(cell),
        Grid.CellDownRight(cell)
      }) && !CreatureHelpers.isClear(Grid.CellBelow(Grid.CellDownRight(cell))))
        return Vector3.op_Addition(Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.right), Vector3.down);
      if (CreatureHelpers.cellsAreClear(new int[3]
      {
        Grid.OffsetCell(cell, 1, 0),
        Grid.OffsetCell(cell, 1, -1),
        Grid.OffsetCell(cell, 1, -2)
      }) && !CreatureHelpers.isClear(Grid.OffsetCell(cell, 1, -3)))
        return Vector3.op_Addition(Vector3.op_Addition(Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.right), Vector3.down), Vector3.down);
      if (CreatureHelpers.cellsAreClear(new int[4]
      {
        Grid.OffsetCell(cell, 1, 0),
        Grid.OffsetCell(cell, 1, -1),
        Grid.OffsetCell(cell, 1, -2),
        Grid.OffsetCell(cell, 1, -3)
      }))
        return TransformExtensions.GetPosition(transform);
      if (CreatureHelpers.isClear(Grid.CellRight(cell)))
        return Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.right);
      if (CreatureHelpers.isClear(Grid.CellUpRight(cell)) && !Grid.Solid[Grid.CellAbove(cell)] && Grid.Solid[Grid.CellRight(cell)])
        return Vector3.op_Addition(Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.up), Vector3.right);
      if (!Grid.Solid[Grid.CellAbove(cell)] && !Grid.Solid[Grid.CellAbove(Grid.CellAbove(cell))] && Grid.Solid[Grid.CellAbove(Grid.CellRight(cell))] && CreatureHelpers.isClear(Grid.CellRight(Grid.CellAbove(Grid.CellAbove(cell)))))
        return Vector3.op_Addition(Vector3.op_Addition(Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.up), Vector3.up), Vector3.right);
    }
    if ((double) Heading.x == -1.0)
    {
      if (CreatureHelpers.isClear(Grid.CellLeft(cell)) && CreatureHelpers.isClear(Grid.CellDownLeft(cell)) && CreatureHelpers.isClear(Grid.CellLeft(Grid.CellLeft(cell))) && !CreatureHelpers.isClear(Grid.PosToCell(Vector3.op_Addition(Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.op_Multiply(Vector3.left, 2f)), Vector3.down))))
        return Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.op_Multiply(Vector3.left, 2f));
      if (CreatureHelpers.cellsAreClear(new int[2]
      {
        Grid.CellLeft(cell),
        Grid.CellDownLeft(cell)
      }) && !CreatureHelpers.isClear(Grid.CellBelow(Grid.CellDownLeft(cell))))
        return Vector3.op_Addition(Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.left), Vector3.down);
      if (CreatureHelpers.cellsAreClear(new int[3]
      {
        Grid.OffsetCell(cell, -1, 0),
        Grid.OffsetCell(cell, -1, -1),
        Grid.OffsetCell(cell, -1, -2)
      }) && !CreatureHelpers.isClear(Grid.OffsetCell(cell, -1, -3)))
        return Vector3.op_Addition(Vector3.op_Addition(Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.left), Vector3.down), Vector3.down);
      if (CreatureHelpers.cellsAreClear(new int[4]
      {
        Grid.OffsetCell(cell, -1, 0),
        Grid.OffsetCell(cell, -1, -1),
        Grid.OffsetCell(cell, -1, -2),
        Grid.OffsetCell(cell, -1, -3)
      }))
        return TransformExtensions.GetPosition(transform);
      if (CreatureHelpers.isClear(Grid.CellLeft(Grid.PosToCell(TransformExtensions.GetPosition(transform)))))
        return Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.left);
      if (CreatureHelpers.isClear(Grid.CellUpLeft(cell)) && !Grid.Solid[Grid.CellAbove(cell)] && Grid.Solid[Grid.CellLeft(cell)])
        return Vector3.op_Addition(Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.up), Vector3.left);
      if (!Grid.Solid[Grid.CellAbove(cell)] && !Grid.Solid[Grid.CellAbove(Grid.CellAbove(cell))] && Grid.Solid[Grid.CellAbove(Grid.CellLeft(cell))] && CreatureHelpers.isClear(Grid.CellLeft(Grid.CellAbove(Grid.CellAbove(cell)))))
        return Vector3.op_Addition(Vector3.op_Addition(Vector3.op_Addition(TransformExtensions.GetPosition(transform), Vector3.up), Vector3.up), Vector3.left);
    }
    return TransformExtensions.GetPosition(transform);
  }

  public static bool CrewNearby(Transform transform, int range = 6)
  {
    int cell1 = Grid.PosToCell(((Component) transform).gameObject);
    for (int x = 1; x < range; ++x)
    {
      int cell2 = Grid.OffsetCell(cell1, x, 0);
      int cell3 = Grid.OffsetCell(cell1, -x, 0);
      if (Object.op_Inequality((Object) Grid.Objects[cell2, 0], (Object) null) || Object.op_Inequality((Object) Grid.Objects[cell3, 0], (Object) null))
        return true;
    }
    return false;
  }

  public static bool CheckHorizontalClear(Vector3 startPosition, Vector3 endPosition)
  {
    int cell = Grid.PosToCell(startPosition);
    int num1 = 1;
    if ((double) endPosition.x < (double) startPosition.x)
      num1 = -1;
    float num2 = Mathf.Abs(endPosition.x - startPosition.x);
    for (int index = 0; (double) index < (double) num2; ++index)
    {
      int i = Grid.OffsetCell(cell, index * num1, 0);
      if (Grid.Solid[i])
        return false;
    }
    return true;
  }

  public static GameObject GetFleeTargetLocatorObject(GameObject self, GameObject threat)
  {
    if (Object.op_Equality((Object) threat, (Object) null))
    {
      Debug.LogWarning((object) (((Object) self).name + " is trying to flee, bus has no threats"));
      return (GameObject) null;
    }
    int cell1 = Grid.PosToCell(threat);
    int cell2 = Grid.PosToCell(self);
    Navigator nav = self.GetComponent<Navigator>();
    if (Object.op_Equality((Object) nav, (Object) null))
    {
      Debug.LogWarning((object) (((Object) self).name + " is trying to flee, bus has no navigator component attached."));
      return (GameObject) null;
    }
    HashSet<int> intSet = GameUtil.FloodCollectCells(Grid.PosToCell(self), (Func<int, bool>) (cell => CreatureHelpers.CanFleeTo(cell, nav)));
    int cell3 = -1;
    int num1 = -1;
    foreach (int num2 in intSet)
    {
      if (nav.CanReach(num2) && num2 != cell2)
      {
        int num3 = Grid.GetCellDistance(num2, cell1) - 1;
        if (CreatureHelpers.isInFavoredFleeDirection(num2, cell1, self))
          num3 += 2;
        if (num3 > num1)
        {
          num1 = num3;
          cell3 = num2;
        }
      }
    }
    return cell3 != -1 ? ChoreHelpers.CreateLocator("GoToLocator", Grid.CellToPos(cell3)) : (GameObject) null;
  }

  private static bool isInFavoredFleeDirection(int targetFleeCell, int threatCell, GameObject self) => ((double) Grid.CellToPos(threatCell).x < (double) TransformExtensions.GetPosition(self.transform).x ? 1 : 0) == ((double) Grid.CellToPos(threatCell).x < (double) Grid.CellToPos(targetFleeCell).x ? (true ? 1 : 0) : (false ? 1 : 0));

  private static bool CanFleeTo(int cell, Navigator nav) => nav.CanReach(cell) || nav.CanReach(Grid.OffsetCell(cell, -1, -1)) || nav.CanReach(Grid.OffsetCell(cell, 1, -1)) || nav.CanReach(Grid.OffsetCell(cell, -1, 1)) || nav.CanReach(Grid.OffsetCell(cell, 1, 1));
}
