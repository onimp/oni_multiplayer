// Decompiled with JetBrains decompiler
// Type: IdleCellQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class IdleCellQuery : PathFinderQuery
{
  private MinionBrain brain;
  private int targetCell;
  private int maxCost;

  public IdleCellQuery Reset(MinionBrain brain, int max_cost)
  {
    this.brain = brain;
    this.maxCost = max_cost;
    this.targetCell = Grid.InvalidCell;
    return this;
  }

  public override bool IsMatch(int cell, int parent_cell, int cost)
  {
    SafeCellQuery.SafeFlags flags = SafeCellQuery.GetFlags(cell, this.brain);
    if ((flags & SafeCellQuery.SafeFlags.IsClear) != (SafeCellQuery.SafeFlags) 0 && (flags & SafeCellQuery.SafeFlags.IsNotLadder) != (SafeCellQuery.SafeFlags) 0 && (flags & SafeCellQuery.SafeFlags.IsNotTube) != (SafeCellQuery.SafeFlags) 0 && (flags & SafeCellQuery.SafeFlags.IsBreathable) != (SafeCellQuery.SafeFlags) 0 && (flags & SafeCellQuery.SafeFlags.IsNotLiquid) != (SafeCellQuery.SafeFlags) 0)
      this.targetCell = cell;
    return cost > this.maxCost;
  }

  public override int GetResultCell() => this.targetCell;
}
