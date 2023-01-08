// Decompiled with JetBrains decompiler
// Type: BalloonStandCellSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class BalloonStandCellSensor : Sensor
{
  private MinionBrain brain;
  private Navigator navigator;
  private int cell;
  private int standCell;

  public BalloonStandCellSensor(Sensors sensors)
    : base(sensors)
  {
    this.navigator = this.GetComponent<Navigator>();
    this.brain = this.GetComponent<MinionBrain>();
  }

  public override void Update()
  {
    this.cell = Grid.InvalidCell;
    int num1 = int.MaxValue;
    ListPool<int[], BalloonStandCellSensor>.PooledList pooledList = ListPool<int[], BalloonStandCellSensor>.Allocate();
    int num2 = 50;
    foreach (int mingleCell in Game.Instance.mingleCellTracker.mingleCells)
    {
      if (this.brain.IsCellClear(mingleCell))
      {
        int navigationCost = this.navigator.GetNavigationCost(mingleCell);
        if (navigationCost != -1)
        {
          if (mingleCell == Grid.InvalidCell || navigationCost < num1)
          {
            this.cell = mingleCell;
            num1 = navigationCost;
          }
          if (navigationCost < num2)
          {
            int cell1 = Grid.CellRight(mingleCell);
            int cell2 = Grid.CellRight(cell1);
            int cell3 = Grid.CellLeft(mingleCell);
            int cell4 = Grid.CellLeft(cell3);
            CavityInfo cavityForCell1 = Game.Instance.roomProber.GetCavityForCell(this.cell);
            CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(cell4);
            CavityInfo cavityForCell3 = Game.Instance.roomProber.GetCavityForCell(cell2);
            if (cavityForCell1 != null)
            {
              if (cavityForCell3 != null && HandleVector<int>.Handle.op_Equality(cavityForCell3.handle, cavityForCell1.handle) && this.navigator.NavGrid.NavTable.IsValid(cell1) && this.navigator.NavGrid.NavTable.IsValid(cell2))
                ((List<int[]>) pooledList).Add(new int[2]
                {
                  mingleCell,
                  cell2
                });
              if (cavityForCell2 != null && HandleVector<int>.Handle.op_Equality(cavityForCell2.handle, cavityForCell1.handle) && this.navigator.NavGrid.NavTable.IsValid(cell3) && this.navigator.NavGrid.NavTable.IsValid(cell4))
                ((List<int[]>) pooledList).Add(new int[2]
                {
                  mingleCell,
                  cell4
                });
            }
          }
        }
      }
    }
    if (((List<int[]>) pooledList).Count > 0)
    {
      int[] numArray = ((List<int[]>) pooledList)[Random.Range(0, ((List<int[]>) pooledList).Count)];
      this.cell = numArray[0];
      this.standCell = numArray[1];
    }
    else if (Components.Telepads.Count > 0)
    {
      Telepad telepad = Components.Telepads.Items[0];
      if (Object.op_Equality((Object) telepad, (Object) null) || !((Component) telepad).GetComponent<Operational>().IsOperational)
        return;
      int cell5 = Grid.CellLeft(Grid.PosToCell(TransformExtensions.GetPosition(telepad.transform)));
      int cell6 = Grid.CellRight(cell5);
      int cell7 = Grid.CellRight(cell6);
      CavityInfo cavityForCell4 = Game.Instance.roomProber.GetCavityForCell(cell5);
      CavityInfo cavityForCell5 = Game.Instance.roomProber.GetCavityForCell(cell7);
      if (cavityForCell4 != null && cavityForCell5 != null && this.navigator.NavGrid.NavTable.IsValid(cell5) && this.navigator.NavGrid.NavTable.IsValid(cell6) && this.navigator.NavGrid.NavTable.IsValid(cell7))
      {
        this.cell = cell5;
        this.standCell = cell7;
      }
    }
    pooledList.Recycle();
  }

  public int GetCell() => this.cell;

  public int GetStandCell() => this.standCell;
}
