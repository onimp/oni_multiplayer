// Decompiled with JetBrains decompiler
// Type: AggressiveChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class AggressiveChore : Chore<AggressiveChore.StatesInstance>
{
  public AggressiveChore(IStateMachineTarget target, Action<Chore> on_complete = null)
    : base(Db.Get().ChoreTypes.StressActingOut, target, target.GetComponent<ChoreProvider>(), false, on_complete, master_priority_class: PriorityScreen.PriorityClass.compulsory)
  {
    this.smi = new AggressiveChore.StatesInstance(this, target.gameObject);
  }

  public override void Cleanup() => base.Cleanup();

  public void PunchWallDamage(float dt)
  {
    if (!Grid.Solid[this.smi.sm.wallCellToBreak] || Grid.StrengthInfo[this.smi.sm.wallCellToBreak] >= (byte) 100)
      return;
    double num = (double) WorldDamage.Instance.ApplyDamage(this.smi.sm.wallCellToBreak, 0.06f * dt, this.smi.sm.wallCellToBreak, (string) BUILDINGS.DAMAGESOURCES.MINION_DESTRUCTION, (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.MINION_DESTRUCTION);
  }

  public class StatesInstance : 
    GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.GameInstance
  {
    public StatesInstance(AggressiveChore master, GameObject breaker)
      : base(master)
    {
      this.sm.breaker.Set(breaker, this.smi, false);
    }

    public void FindBreakable()
    {
      Navigator navigator = this.GetComponent<Navigator>();
      int num = int.MaxValue;
      Breakable breakable1 = (Breakable) null;
      if (Random.Range(0, 100) >= 50)
      {
        foreach (Breakable breakable2 in Components.Breakables.Items)
        {
          if (!Object.op_Equality((Object) breakable2, (Object) null) && !breakable2.isBroken())
          {
            int navigationCost = navigator.GetNavigationCost((IApproachable) breakable2);
            if (navigationCost != -1 && navigationCost < num)
            {
              num = navigationCost;
              breakable1 = breakable2;
            }
          }
        }
      }
      if (Object.op_Equality((Object) breakable1, (Object) null))
      {
        this.sm.moveToWallTarget.Set(GameUtil.FloodFillFind<object>((Func<int, object, bool>) ((cell, arg) => !Grid.Solid[cell] && navigator.CanReach(cell) && (Grid.IsValidCell(Grid.CellLeft(cell)) && Grid.Solid[Grid.CellLeft(cell)] || Grid.IsValidCell(Grid.CellRight(cell)) && Grid.Solid[Grid.CellRight(cell)] || Grid.IsValidCell(Grid.OffsetCell(cell, 1, 1)) && Grid.Solid[Grid.OffsetCell(cell, 1, 1)] || Grid.IsValidCell(Grid.OffsetCell(cell, -1, 1)) && Grid.Solid[Grid.OffsetCell(cell, -1, 1)])), (object) null, Grid.PosToCell(((Component) navigator).gameObject), 128, true, true), this.smi);
        this.GoTo((StateMachine.BaseState) this.sm.move_notarget);
      }
      else
      {
        this.sm.breakable.Set((KMonoBehaviour) breakable1, this.smi);
        this.GoTo((StateMachine.BaseState) this.sm.move_target);
      }
    }
  }

  public class States : 
    GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore>
  {
    public StateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.TargetParameter breaker;
    public StateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.TargetParameter breakable;
    public StateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.IntParameter moveToWallTarget;
    public int wallCellToBreak;
    public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.ApproachSubState<Breakable> move_target;
    public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State move_notarget;
    public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State findbreakable;
    public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State noTarget;
    public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State breaking;
    public AggressiveChore.States.BreakingWall breaking_wall;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.findbreakable;
      this.Target(this.breaker);
      this.root.ToggleAnims("anim_loco_destructive_kanim");
      this.noTarget.Enter((StateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State.Callback) (smi => smi.StopSM("complete/no more food")));
      this.findbreakable.Enter("FindBreakable", (StateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State.Callback) (smi => smi.FindBreakable()));
      this.move_notarget.MoveTo((Func<AggressiveChore.StatesInstance, int>) (smi => smi.sm.moveToWallTarget.Get(smi)), (GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State) this.breaking_wall, this.noTarget);
      this.move_target.InitializeStates(this.breaker, this.breakable, this.breaking, this.findbreakable).ToggleStatusItem(Db.Get().DuplicantStatusItems.LashingOut);
      this.breaking_wall.DefaultState(this.breaking_wall.Pre).Enter((StateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State.Callback) (smi =>
      {
        int cell = Grid.PosToCell(smi.master.gameObject);
        if (Grid.Solid[Grid.OffsetCell(cell, 1, 0)])
        {
          smi.sm.masterTarget.Get<KAnimControllerBase>(smi).AddAnimOverrides(Assets.GetAnim(HashedString.op_Implicit("anim_out_of_reach_destructive_low_kanim")));
          this.wallCellToBreak = Grid.OffsetCell(cell, 1, 0);
        }
        else if (Grid.Solid[Grid.OffsetCell(cell, -1, 0)])
        {
          smi.sm.masterTarget.Get<KAnimControllerBase>(smi).AddAnimOverrides(Assets.GetAnim(HashedString.op_Implicit("anim_out_of_reach_destructive_low_kanim")));
          this.wallCellToBreak = Grid.OffsetCell(cell, -1, 0);
        }
        else if (Grid.Solid[Grid.OffsetCell(cell, 1, 1)])
        {
          smi.sm.masterTarget.Get<KAnimControllerBase>(smi).AddAnimOverrides(Assets.GetAnim(HashedString.op_Implicit("anim_out_of_reach_destructive_high_kanim")));
          this.wallCellToBreak = Grid.OffsetCell(cell, 1, 1);
        }
        else if (Grid.Solid[Grid.OffsetCell(cell, -1, 1)])
        {
          smi.sm.masterTarget.Get<KAnimControllerBase>(smi).AddAnimOverrides(Assets.GetAnim(HashedString.op_Implicit("anim_out_of_reach_destructive_high_kanim")));
          this.wallCellToBreak = Grid.OffsetCell(cell, -1, 1);
        }
        smi.master.GetComponent<Facing>().Face(Grid.CellToPos(this.wallCellToBreak));
      })).Exit((StateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State.Callback) (smi =>
      {
        smi.sm.masterTarget.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(Assets.GetAnim(HashedString.op_Implicit("anim_out_of_reach_destructive_high_kanim")));
        smi.sm.masterTarget.Get<KAnimControllerBase>(smi).RemoveAnimOverrides(Assets.GetAnim(HashedString.op_Implicit("anim_out_of_reach_destructive_low_kanim")));
      }));
      this.breaking_wall.Pre.PlayAnim("working_pre").OnAnimQueueComplete(this.breaking_wall.Loop);
      this.breaking_wall.Loop.ScheduleGoTo(26f, (StateMachine.BaseState) this.breaking_wall.Pst).Update("PunchWallDamage", (Action<AggressiveChore.StatesInstance, float>) ((smi, dt) => smi.master.PunchWallDamage(dt)), (UpdateRate) 6).Enter((StateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State.Callback) (smi => smi.Play("working_loop", (KAnim.PlayMode) 0))).Update((Action<AggressiveChore.StatesInstance, float>) ((smi, dt) =>
      {
        if (Grid.Solid[smi.sm.wallCellToBreak])
          return;
        smi.GoTo((StateMachine.BaseState) this.breaking_wall.Pst);
      }));
      this.breaking_wall.Pst.QueueAnim("working_pst").OnAnimQueueComplete(this.noTarget);
      this.breaking.ToggleWork<Breakable>(this.breakable, (GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State) null, (GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State) null, (Func<AggressiveChore.StatesInstance, bool>) null);
    }

    public class BreakingWall : 
      GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State
    {
      public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State Pre;
      public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State Loop;
      public GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State Pst;
    }
  }
}
