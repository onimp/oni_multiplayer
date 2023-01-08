// Decompiled with JetBrains decompiler
// Type: NestingPoopState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

internal class NestingPoopState : 
  GameStateMachine<NestingPoopState, NestingPoopState.Instance, IStateMachineTarget, NestingPoopState.Def>
{
  public GameStateMachine<NestingPoopState, NestingPoopState.Instance, IStateMachineTarget, NestingPoopState.Def>.State goingtopoop;
  public GameStateMachine<NestingPoopState, NestingPoopState.Instance, IStateMachineTarget, NestingPoopState.Def>.State pooping;
  public GameStateMachine<NestingPoopState, NestingPoopState.Instance, IStateMachineTarget, NestingPoopState.Def>.State behaviourcomplete;
  public GameStateMachine<NestingPoopState, NestingPoopState.Instance, IStateMachineTarget, NestingPoopState.Def>.State failedtonest;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.goingtopoop;
    this.goingtopoop.MoveTo((Func<NestingPoopState.Instance, int>) (smi => smi.GetPoopPosition()), this.pooping, this.failedtonest);
    this.failedtonest.Enter((StateMachine<NestingPoopState, NestingPoopState.Instance, IStateMachineTarget, NestingPoopState.Def>.State.Callback) (smi => smi.SetLastPoopCell())).GoTo(this.pooping);
    this.pooping.Enter((StateMachine<NestingPoopState, NestingPoopState.Instance, IStateMachineTarget, NestingPoopState.Def>.State.Callback) (smi => smi.master.GetComponent<Facing>().SetFacing(Grid.PosToCell(smi.master.gameObject) > smi.targetPoopCell))).ToggleStatusItem((string) CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, (string) CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).PlayAnim("poop").OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.Enter((StateMachine<NestingPoopState, NestingPoopState.Instance, IStateMachineTarget, NestingPoopState.Def>.State.Callback) (smi => smi.SetLastPoopCell())).PlayAnim("idle_loop", (KAnim.PlayMode) 0).BehaviourComplete(GameTags.Creatures.Poop);
  }

  public class Def : StateMachine.BaseDef
  {
    public Tag nestingPoopElement = Tag.Invalid;

    public Def(Tag tag) => this.nestingPoopElement = tag;
  }

  public new class Instance : 
    GameStateMachine<NestingPoopState, NestingPoopState.Instance, IStateMachineTarget, NestingPoopState.Def>.GameInstance
  {
    [Serialize]
    private int lastPoopCell = -1;
    public int targetPoopCell = -1;
    private Tag currentlyPoopingElement = Tag.Invalid;

    public Instance(Chore<NestingPoopState.Instance> chore, NestingPoopState.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.Poop);
    }

    private static bool IsValidNestingCell(int cell, object arg)
    {
      if (!Grid.IsValidCell(cell) || Grid.Solid[cell] || !Grid.Solid[Grid.CellBelow(cell)])
        return false;
      return NestingPoopState.Instance.IsValidPoopFromCell(cell, true) || NestingPoopState.Instance.IsValidPoopFromCell(cell, false);
    }

    private static bool IsValidPoopFromCell(int cell, bool look_left)
    {
      if (look_left)
      {
        int num1 = Grid.CellDownLeft(cell);
        int num2 = Grid.CellLeft(cell);
        return Grid.IsValidCell(num1) && Grid.Solid[num1] && Grid.IsValidCell(num2) && !Grid.Solid[num2];
      }
      int num3 = Grid.CellDownRight(cell);
      int num4 = Grid.CellRight(cell);
      return Grid.IsValidCell(num3) && Grid.Solid[num3] && Grid.IsValidCell(num4) && !Grid.Solid[num4];
    }

    public int GetPoopPosition()
    {
      this.targetPoopCell = this.GetTargetPoopCell();
      List<Direction> directionList = new List<Direction>();
      if (NestingPoopState.Instance.IsValidPoopFromCell(this.targetPoopCell, true))
        directionList.Add(Direction.Left);
      if (NestingPoopState.Instance.IsValidPoopFromCell(this.targetPoopCell, false))
        directionList.Add(Direction.Right);
      if (directionList.Count > 0)
      {
        int cellInDirection = Grid.GetCellInDirection(this.targetPoopCell, directionList[Random.Range(0, directionList.Count)]);
        if (Grid.IsValidCell(cellInDirection))
          return cellInDirection;
      }
      if (Grid.IsValidCell(this.targetPoopCell))
        return this.targetPoopCell;
      if (!Grid.IsValidCell(Grid.PosToCell((StateMachine.Instance) this)))
        Debug.LogWarning((object) "This is bad, how is Mole occupying an invalid cell?");
      return Grid.PosToCell((StateMachine.Instance) this);
    }

    private int GetTargetPoopCell()
    {
      this.currentlyPoopingElement = this.smi.GetSMI<CreatureCalorieMonitor.Instance>().stomach.GetNextPoopEntry();
      int cell = GameUtil.FloodFillFind<object>(new Func<int, object, bool>(NestingPoopState.Instance.IsValidNestingCell), (object) null, !Tag.op_Equality(this.currentlyPoopingElement, this.smi.def.nestingPoopElement) || !Tag.op_Inequality(this.smi.def.nestingPoopElement, Tag.Invalid) || this.lastPoopCell == -1 ? Grid.PosToCell((StateMachine.Instance) this) : this.lastPoopCell, 8, false, true);
      if (cell == -1)
      {
        CellOffset[] cellOffsetArray = new CellOffset[5]
        {
          new CellOffset(0, 0),
          new CellOffset(-1, 0),
          new CellOffset(1, 0),
          new CellOffset(-1, -1),
          new CellOffset(1, -1)
        };
        cell = Grid.OffsetCell(this.lastPoopCell, cellOffsetArray[Random.Range(0, cellOffsetArray.Length)]);
        for (int index = Grid.CellAbove(cell); Grid.IsValidCell(index) && Grid.Solid[index]; index = Grid.CellAbove(cell))
          cell = index;
      }
      return cell;
    }

    public void SetLastPoopCell()
    {
      if (!Tag.op_Equality(this.currentlyPoopingElement, this.smi.def.nestingPoopElement))
        return;
      this.lastPoopCell = Grid.PosToCell((StateMachine.Instance) this);
    }
  }
}
