// Decompiled with JetBrains decompiler
// Type: NavTactic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class NavTactic
{
  private int _overlapPenalty = 3;
  private int _preferredRange;
  private int _rangePenalty = 2;
  private int _pathCostPenalty = 1;

  public NavTactic(int preferredRange, int rangePenalty = 1, int overlapPenalty = 1, int pathCostPenalty = 1)
  {
    this._overlapPenalty = overlapPenalty;
    this._preferredRange = preferredRange;
    this._rangePenalty = rangePenalty;
    this._pathCostPenalty = pathCostPenalty;
  }

  public int GetCellPreferences(int root, CellOffset[] offsets, Navigator navigator)
  {
    int cellPreferences = NavigationReservations.InvalidReservation;
    int num1 = int.MaxValue;
    for (int index = 0; index < offsets.Length; ++index)
    {
      int num2 = Grid.OffsetCell(root, offsets[index]);
      int num3 = 0 + this._overlapPenalty * NavigationReservations.Instance.GetOccupancyCount(num2) + this._rangePenalty * Mathf.Abs(this._preferredRange - Grid.GetCellDistance(root, num2)) + this._pathCostPenalty * Mathf.Max(navigator.GetNavigationCost(num2), 0);
      if (num3 < num1 && navigator.CanReach(num2))
      {
        num1 = num3;
        cellPreferences = num2;
      }
    }
    return cellPreferences;
  }
}
