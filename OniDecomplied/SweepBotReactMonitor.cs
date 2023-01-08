// Decompiled with JetBrains decompiler
// Type: SweepBotReactMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class SweepBotReactMonitor : 
  GameStateMachine<SweepBotReactMonitor, SweepBotReactMonitor.Instance, IStateMachineTarget, SweepBotReactMonitor.Def>
{
  private GameStateMachine<SweepBotReactMonitor, SweepBotReactMonitor.Instance, IStateMachineTarget, SweepBotReactMonitor.Def>.State idle;
  private GameStateMachine<SweepBotReactMonitor, SweepBotReactMonitor.Instance, IStateMachineTarget, SweepBotReactMonitor.Def>.State reactScaryThing;
  private GameStateMachine<SweepBotReactMonitor, SweepBotReactMonitor.Instance, IStateMachineTarget, SweepBotReactMonitor.Def>.State reactFriendlyThing;
  private GameStateMachine<SweepBotReactMonitor, SweepBotReactMonitor.Instance, IStateMachineTarget, SweepBotReactMonitor.Def>.State reactNewOrnament;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.idle;
    this.idle.EventHandler(GameHashes.OccupantChanged, (StateMachine<SweepBotReactMonitor, SweepBotReactMonitor.Instance, IStateMachineTarget, SweepBotReactMonitor.Def>.State.Callback) (smi =>
    {
      if (!Object.op_Inequality((Object) smi.master.gameObject.GetComponent<OrnamentReceptacle>().Occupant, (Object) null))
        return;
      smi.GoTo((StateMachine.BaseState) this.reactNewOrnament);
    })).Update((System.Action<SweepBotReactMonitor.Instance, float>) ((smi, dt) =>
    {
      SweepStates.Instance smi1 = smi.master.gameObject.GetSMI<SweepStates.Instance>();
      int invalidCell = Grid.InvalidCell;
      if (smi1 == null)
        return;
      int cell1 = !smi1.sm.headingRight.Get(smi1) ? Grid.CellLeft(Grid.PosToCell(smi.master.gameObject)) : Grid.CellRight(Grid.PosToCell(smi.master.gameObject));
      bool flag1 = false;
      bool flag2 = false;
      int x;
      int y;
      Grid.CellToXY(Grid.PosToCell((StateMachine.Instance) smi), out x, out y);
      ListPool<ScenePartitionerEntry, SweepBotReactMonitor>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, SweepBotReactMonitor>.Allocate();
      GameScenePartitioner.Instance.GatherEntries(x - 1, y - 1, 3, 3, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>) gathered_entries);
      foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
      {
        Pickupable cmp = partitionerEntry.obj as Pickupable;
        if (!Object.op_Equality((Object) cmp, (Object) null) && !Object.op_Equality((Object) ((Component) cmp).gameObject, (Object) smi.gameObject))
        {
          int cell2 = Grid.PosToCell((KMonoBehaviour) cmp);
          if ((double) Vector3.Distance(smi.gameObject.transform.position, ((Component) cmp).gameObject.transform.position) < (double) Grid.CellSizeInMeters)
          {
            if (Tag.op_Equality(((Component) cmp).PrefabID(), Tag.op_Implicit("SweepBot")) && cell2 == cell1)
            {
              smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim(HashedString.op_Implicit("bump"));
              smi1.sm.headingRight.Set(!smi1.sm.headingRight.Get(smi1), smi1);
              flag1 = true;
            }
            else if (((Component) cmp).HasTag(GameTags.Creature))
              flag2 = true;
          }
        }
      }
      gathered_entries.Recycle();
      if (flag1 || (double) smi.timeinstate <= 10.0 || !Grid.IsValidCell(cell1))
        return;
      if (Object.op_Inequality((Object) Grid.Objects[cell1, 0], (Object) null) && !Grid.Objects[cell1, 0].HasTag(GameTags.Dead))
        smi.GoTo((StateMachine.BaseState) this.reactFriendlyThing);
      else if (smi1.sm.bored.Get(smi1) && Object.op_Inequality((Object) Grid.Objects[cell1, 3], (Object) null))
      {
        smi.GoTo((StateMachine.BaseState) this.reactFriendlyThing);
      }
      else
      {
        if (!flag2)
          return;
        smi.GoTo((StateMachine.BaseState) this.reactScaryThing);
      }
    }), (UpdateRate) 4);
    this.reactScaryThing.Enter((StateMachine<SweepBotReactMonitor, SweepBotReactMonitor.Instance, IStateMachineTarget, SweepBotReactMonitor.Def>.State.Callback) (smi => smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim(HashedString.op_Implicit("react_neg")))).ToggleStatusItem(Db.Get().RobotStatusItems.ReactNegative, (Func<SweepBotReactMonitor.Instance, object>) null, Db.Get().StatusItemCategories.Main).OnAnimQueueComplete(this.idle);
    this.reactFriendlyThing.Enter((StateMachine<SweepBotReactMonitor, SweepBotReactMonitor.Instance, IStateMachineTarget, SweepBotReactMonitor.Def>.State.Callback) (smi => smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim(HashedString.op_Implicit("react_pos")))).ToggleStatusItem(Db.Get().RobotStatusItems.ReactPositive, (Func<SweepBotReactMonitor.Instance, object>) null, Db.Get().StatusItemCategories.Main).OnAnimQueueComplete(this.idle);
    this.reactNewOrnament.Enter((StateMachine<SweepBotReactMonitor, SweepBotReactMonitor.Instance, IStateMachineTarget, SweepBotReactMonitor.Def>.State.Callback) (smi => smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim(HashedString.op_Implicit("react_ornament")))).OnAnimQueueComplete(this.idle).ToggleStatusItem(Db.Get().RobotStatusItems.ReactPositive);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<SweepBotReactMonitor, SweepBotReactMonitor.Instance, IStateMachineTarget, SweepBotReactMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, SweepBotReactMonitor.Def def)
      : base(master, def)
    {
    }
  }
}
