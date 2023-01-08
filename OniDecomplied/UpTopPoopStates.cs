// Decompiled with JetBrains decompiler
// Type: UpTopPoopStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class UpTopPoopStates : 
  GameStateMachine<UpTopPoopStates, UpTopPoopStates.Instance, IStateMachineTarget, UpTopPoopStates.Def>
{
  public GameStateMachine<UpTopPoopStates, UpTopPoopStates.Instance, IStateMachineTarget, UpTopPoopStates.Def>.State goingtopoop;
  public GameStateMachine<UpTopPoopStates, UpTopPoopStates.Instance, IStateMachineTarget, UpTopPoopStates.Def>.State pooping;
  public GameStateMachine<UpTopPoopStates, UpTopPoopStates.Instance, IStateMachineTarget, UpTopPoopStates.Def>.State behaviourcomplete;
  public StateMachine<UpTopPoopStates, UpTopPoopStates.Instance, IStateMachineTarget, UpTopPoopStates.Def>.IntParameter targetCell;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.goingtopoop;
    this.root.Enter("SetTarget", (StateMachine<UpTopPoopStates, UpTopPoopStates.Instance, IStateMachineTarget, UpTopPoopStates.Def>.State.Callback) (smi => this.targetCell.Set(smi.GetSMI<GasAndLiquidConsumerMonitor.Instance>().targetCell, smi)));
    this.goingtopoop.MoveTo((Func<UpTopPoopStates.Instance, int>) (smi => smi.GetPoopCell()), this.pooping, this.pooping);
    this.pooping.PlayAnim("poop").ToggleStatusItem((string) CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, (string) CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.PlayAnim("idle_loop", (KAnim.PlayMode) 0).BehaviourComplete(GameTags.Creatures.Poop);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<UpTopPoopStates, UpTopPoopStates.Instance, IStateMachineTarget, UpTopPoopStates.Def>.GameInstance
  {
    public Instance(Chore<UpTopPoopStates.Instance> chore, UpTopPoopStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.Poop);
    }

    public int GetPoopCell()
    {
      int num = this.master.gameObject.GetComponent<Navigator>().maxProbingRadius - 1;
      int cell = Grid.PosToCell(this.gameObject);
      for (int index = Grid.OffsetCell(cell, 0, 1); num > 0 && Grid.IsValidCell(index) && !Grid.Solid[index] && !this.IsClosedDoor(index); index = Grid.OffsetCell(cell, 0, 1))
      {
        --num;
        cell = index;
      }
      return cell;
    }

    public bool IsClosedDoor(int cellAbove)
    {
      if (!Grid.HasDoor[cellAbove])
        return false;
      Door component = Grid.Objects[cellAbove, 1].GetComponent<Door>();
      return Object.op_Inequality((Object) component, (Object) null) && component.CurrentState != Door.ControlState.Opened;
    }
  }
}
