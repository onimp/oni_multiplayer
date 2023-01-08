// Decompiled with JetBrains decompiler
// Type: BansheeChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BansheeChore : Chore<BansheeChore.StatesInstance>
{
  private const string audienceEffectName = "WailedAt";

  public BansheeChore(
    ChoreType chore_type,
    IStateMachineTarget target,
    Notification notification,
    Action<Chore> on_complete = null)
    : base(Db.Get().ChoreTypes.BansheeWail, target, target.GetComponent<ChoreProvider>(), false, on_complete, master_priority_class: PriorityScreen.PriorityClass.compulsory)
  {
    this.smi = new BansheeChore.StatesInstance(this, target.gameObject, notification);
  }

  public class StatesInstance : 
    GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.GameInstance
  {
    public Notification notification;

    public StatesInstance(BansheeChore master, GameObject wailer, Notification notification)
      : base(master)
    {
      this.sm.wailer.Set(wailer, this.smi, false);
      this.notification = notification;
    }

    public void FindAudience()
    {
      Navigator component = this.GetComponent<Navigator>();
      int worldId = (int) Grid.WorldIdx[Grid.PosToCell(this.gameObject)];
      int num1 = int.MaxValue;
      int num2 = Grid.InvalidCell;
      List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(worldId);
      for (int index = 0; index < worldItems.Count; ++index)
      {
        if (!Util.IsNullOrDestroyed((object) worldItems[index]) && !Object.op_Equality((Object) ((Component) worldItems[index]).gameObject, (Object) this.gameObject))
        {
          int cell = Grid.PosToCell((KMonoBehaviour) worldItems[index]);
          if (component.CanReach(cell) && !((Component) worldItems[index]).GetComponent<Effects>().HasEffect("WailedAt"))
          {
            int navigationCost = component.GetNavigationCost(cell);
            if (navigationCost < num1)
            {
              num1 = navigationCost;
              num2 = cell;
            }
          }
        }
      }
      if (num2 == Grid.InvalidCell)
        num2 = this.FindIdleCell();
      this.sm.targetWailLocation.Set(num2, this.smi);
      this.GoTo((StateMachine.BaseState) this.sm.moveToAudience);
    }

    public int FindIdleCell()
    {
      Navigator component = this.smi.master.GetComponent<Navigator>();
      MinionPathFinderAbilities currentAbilities = (MinionPathFinderAbilities) component.GetCurrentAbilities();
      currentAbilities.SetIdleNavMaskEnabled(true);
      IdleCellQuery query = PathFinderQueries.idleCellQuery.Reset(this.GetComponent<MinionBrain>(), Random.Range(30, 90));
      component.RunQuery((PathFinderQuery) query);
      currentAbilities.SetIdleNavMaskEnabled(false);
      return query.GetResultCell();
    }

    public void BotherAudience(float dt)
    {
      if ((double) dt <= 0.0)
        return;
      int cell1 = Grid.PosToCell(this.smi.master.gameObject);
      int worldId = (int) Grid.WorldIdx[cell1];
      foreach (MinionIdentity worldItem in Components.LiveMinionIdentities.GetWorldItems(worldId))
      {
        if (!Util.IsNullOrDestroyed((object) worldItem) && !Object.op_Equality((Object) ((Component) worldItem).gameObject, (Object) this.smi.master.gameObject))
        {
          int cell2 = Grid.PosToCell((KMonoBehaviour) worldItem);
          if (Grid.GetCellDistance(cell1, Grid.PosToCell((KMonoBehaviour) worldItem)) <= TUNING.STRESS.BANSHEE_WAIL_RADIUS)
          {
            HashSet<int> outputCells = new HashSet<int>();
            Grid.CollectCellsInLine(cell1, cell2, outputCells);
            bool flag = false;
            foreach (int i in outputCells)
            {
              if (Grid.Solid[i])
              {
                flag = true;
                break;
              }
            }
            if (!flag && !((Component) worldItem).GetComponent<Effects>().HasEffect("WailedAt"))
            {
              ((Component) worldItem).GetComponent<Effects>().Add("WailedAt", true);
              ((Component) worldItem).GetSMI<ThreatMonitor.Instance>().ClearMainThreat();
              FleeChore fleeChore = new FleeChore(((Component) worldItem).GetComponent<IStateMachineTarget>(), this.smi.master.gameObject);
            }
          }
        }
      }
    }
  }

  public class States : 
    GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore>
  {
    public StateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.TargetParameter wailer;
    public StateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.IntParameter targetWailLocation;
    public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State findAudience;
    public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State moveToAudience;
    public BansheeChore.States.Wail wail;
    public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State recover;
    public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State delay;
    public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State wander;
    public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State complete;
    private Effect wailPreEffect;
    private Effect wailRecoverEffect;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.findAudience;
      this.Target(this.wailer);
      this.wailPreEffect = new Effect("BansheeWailing", (string) DUPLICANTS.MODIFIERS.BANSHEE_WAILING.NAME, (string) DUPLICANTS.MODIFIERS.BANSHEE_WAILING.TOOLTIP, 0.0f, true, false, true);
      this.wailPreEffect.Add(new AttributeModifier("AirConsumptionRate", 7.5f));
      Db.Get().effects.Add(this.wailPreEffect);
      this.wailRecoverEffect = new Effect("BansheeWailingRecovery", (string) DUPLICANTS.MODIFIERS.BANSHEE_WAILING_RECOVERY.NAME, (string) DUPLICANTS.MODIFIERS.BANSHEE_WAILING_RECOVERY.TOOLTIP, 0.0f, true, false, true);
      this.wailRecoverEffect.Add(new AttributeModifier("AirConsumptionRate", 1f));
      Db.Get().effects.Add(this.wailRecoverEffect);
      this.findAudience.Enter("FindAudience", (StateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State.Callback) (smi => smi.FindAudience())).ToggleAnims("anim_loco_banshee_kanim");
      this.moveToAudience.MoveTo((Func<BansheeChore.StatesInstance, int>) (smi => smi.sm.targetWailLocation.Get(smi)), (GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State) this.wail, this.delay).ToggleAnims("anim_loco_banshee_kanim");
      this.wail.defaultState = (StateMachine.BaseState) this.wail.pre.DoNotification((Func<BansheeChore.StatesInstance, Notification>) (smi => smi.notification));
      this.wail.pre.ToggleAnims("anim_banshee_kanim").PlayAnim("working_pre").ToggleEffect((Func<BansheeChore.StatesInstance, Effect>) (smi => this.wailPreEffect)).OnAnimQueueComplete(this.wail.loop);
      this.wail.loop.ToggleAnims("anim_banshee_kanim").Enter((StateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State.Callback) (smi =>
      {
        smi.Play("working_loop", (KAnim.PlayMode) 0);
        AcousticDisturbance.Emit((object) smi.master.gameObject, TUNING.STRESS.BANSHEE_WAIL_RADIUS);
      })).ScheduleGoTo(5f, (StateMachine.BaseState) this.wail.pst).Update((Action<BansheeChore.StatesInstance, float>) ((smi, dt) => smi.BotherAudience(dt)));
      this.wail.pst.ToggleAnims("anim_banshee_kanim").QueueAnim("working_pst").EventHandlerTransition(GameHashes.AnimQueueComplete, this.recover, (Func<BansheeChore.StatesInstance, object, bool>) ((smi, data) => true)).ScheduleGoTo(3f, (StateMachine.BaseState) this.recover);
      this.recover.ToggleEffect((Func<BansheeChore.StatesInstance, Effect>) (smi => this.wailRecoverEffect)).ToggleAnims("anim_emotes_default_kanim").QueueAnim("breathe_pre").QueueAnim("breathe_loop").QueueAnim("breathe_loop").QueueAnim("breathe_loop").QueueAnim("breathe_pst").OnAnimQueueComplete(this.complete);
      this.delay.ScheduleGoTo(1f, (StateMachine.BaseState) this.wander);
      this.wander.MoveTo((Func<BansheeChore.StatesInstance, int>) (smi => smi.FindIdleCell()), this.findAudience, this.findAudience).ToggleAnims("anim_loco_banshee_kanim");
      this.complete.Enter((StateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State.Callback) (smi => smi.StopSM("complete")));
    }

    public class Wail : 
      GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State
    {
      public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State pre;
      public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State loop;
      public GameStateMachine<BansheeChore.States, BansheeChore.StatesInstance, BansheeChore, object>.State pst;
    }
  }
}
