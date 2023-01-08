// Decompiled with JetBrains decompiler
// Type: MingleCellSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class MingleCellSensor : Sensor
{
  private MinionBrain brain;
  private Navigator navigator;
  private int cell;

  public MingleCellSensor(Sensors sensors)
    : base(sensors)
  {
    this.navigator = this.GetComponent<Navigator>();
    this.brain = this.GetComponent<MinionBrain>();
  }

  public override void Update()
  {
    this.cell = Grid.InvalidCell;
    int num1 = int.MaxValue;
    ListPool<int, MingleCellSensor>.PooledList pooledList = ListPool<int, MingleCellSensor>.Allocate();
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
            ((List<int>) pooledList).Add(mingleCell);
        }
      }
    }
    if (((List<int>) pooledList).Count > 0)
      this.cell = ((List<int>) pooledList)[Random.Range(0, ((List<int>) pooledList).Count)];
    pooledList.Recycle();
  }

  public int GetCell() => this.cell;
}
