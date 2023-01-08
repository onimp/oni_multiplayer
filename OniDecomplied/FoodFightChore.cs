// Decompiled with JetBrains decompiler
// Type: FoodFightChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class FoodFightChore : Chore<FoodFightChore.StatesInstance>
{
  public static readonly Chore.Precondition EdibleIsNotNull = new Chore.Precondition()
  {
    id = nameof (EdibleIsNotNull),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.EDIBLE_IS_NOT_NULL,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => Object.op_Inequality((Object) null, (Object) ((Component) context.consumerState.consumer).GetSMI<RationMonitor.Instance>().GetEdible()))
  };

  public FoodFightChore(IStateMachineTarget master, GameObject locator)
    : base(Db.Get().ChoreTypes.Party, master, master.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.high, report_type: ReportManager.ReportType.PersonalTime)
  {
    this.smi = new FoodFightChore.StatesInstance(this, locator);
    this.showAvailabilityInHoverText = false;
    this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
    this.AddPrecondition(FoodFightChore.EdibleIsNotNull);
  }

  public override void Begin(Chore.Precondition.Context context)
  {
    if (Object.op_Equality((Object) context.consumerState.consumer, (Object) null))
    {
      Debug.LogError((object) "FOODFIGHTCHORE null context.consumer");
    }
    else
    {
      RationMonitor.Instance smi = ((Component) context.consumerState.consumer).GetSMI<RationMonitor.Instance>();
      if (smi == null)
      {
        Debug.LogError((object) "FOODFIGHTCHORE null RationMonitor.Instance");
      }
      else
      {
        Edible edible = smi.GetEdible();
        if (Object.op_Equality((Object) ((Component) edible).gameObject, (Object) null))
          Debug.LogError((object) "FOODFIGHTCHORE null edible.gameObject");
        else if (this.smi == null)
          Debug.LogError((object) "FOODFIGHTCHORE null smi");
        else if (this.smi.sm == null)
          Debug.LogError((object) "FOODFIGHTCHORE null smi.sm");
        else if (this.smi.sm.ediblesource == null)
        {
          Debug.LogError((object) "FOODFIGHTCHORE null smi.sm.ediblesource");
        }
        else
        {
          this.smi.sm.ediblesource.Set(((Component) edible).gameObject, this.smi, false);
          KCrashReporter.Assert((double) edible.FoodInfo.CaloriesPerUnit > 0.0, ((Component) edible).GetProperName() + " has invalid calories per unit. Will result in NaNs");
          float num1 = 0.5f;
          KCrashReporter.Assert((double) num1 > 0.0, "FoodFightChore is requesting an invalid amount of food");
          double num2 = (double) this.smi.sm.requestedfoodunits.Set(num1, this.smi);
          this.smi.sm.eater.Set(context.consumerState.gameObject, this.smi, false);
          base.Begin(context);
        }
      }
    }
  }

  public class StatesInstance : 
    GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.GameInstance
  {
    private int locatorCell;

    public StatesInstance(FoodFightChore master, GameObject locator)
      : base(master)
    {
      this.sm.locator.Set(locator, this.smi, false);
    }

    public void UpdateAttackTarget()
    {
      int num = 0;
      MinionIdentity minionIdentity;
      do
      {
        ++num;
        minionIdentity = Components.LiveMinionIdentities[Random.Range(0, Components.LiveMinionIdentities.Count)];
        if (num > 30)
        {
          minionIdentity = (MinionIdentity) null;
          break;
        }
      }
      while (Components.LiveMinionIdentities.Count > 1 && (Object.op_Inequality((Object) this.sm.attackableTarget.Get(this.smi), (Object) null) && Object.op_Equality((Object) ((Component) minionIdentity).gameObject, (Object) this.sm.attackableTarget.Get(this.smi).gameObject) || Game.Instance.roomProber.GetRoomOfGameObject(((Component) minionIdentity).gameObject) == null || Game.Instance.roomProber.GetRoomOfGameObject(((Component) minionIdentity).gameObject).roomType != Db.Get().RoomTypes.MessHall && Game.Instance.roomProber.GetRoomOfGameObject(((Component) minionIdentity).gameObject).roomType != Db.Get().RoomTypes.GreatHall || Object.op_Equality((Object) ((Component) minionIdentity).gameObject, (Object) this.smi.master.gameObject) || Game.Instance.roomProber.GetRoomOfGameObject(((Component) minionIdentity).gameObject) != Game.Instance.roomProber.GetRoomOfGameObject(this.smi.master.gameObject)));
      if (Object.op_Equality((Object) minionIdentity, (Object) null))
        this.smi.GoTo((StateMachine.BaseState) this.sm.end);
      else
        this.smi.sm.attackableTarget.Set((KMonoBehaviour) ((Component) minionIdentity).GetComponent<AttackableBase>(), this.smi);
    }
  }

  public class States : 
    GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore>
  {
    public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.TargetParameter eater;
    public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.TargetParameter ediblesource;
    public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.TargetParameter ediblechunk;
    public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.TargetParameter attackableTarget;
    public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.FloatParameter requestedfoodunits;
    public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.FloatParameter actualfoodunits;
    public StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.TargetParameter locator;
    public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State waitForParticipants;
    public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State emoteRoar;
    public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.ApproachSubState<IApproachable> moveToArena;
    public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.FetchSubState fetch;
    public FoodFightChore.States.AttackStates fight;
    public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State end;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.fetch;
      this.Target(this.eater);
      this.root.ToggleAnims("anim_loco_run_angry_kanim");
      this.fetch.InitializeStates(this.eater, this.ediblesource, this.ediblechunk, this.requestedfoodunits, this.actualfoodunits, (GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State) this.moveToArena).ToggleAnims("anim_loco_run_angry_kanim");
      this.moveToArena.InitializeStates(this.eater, this.locator, this.waitForParticipants);
      this.waitForParticipants.Enter((StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State.Callback) (smi => smi.master.GetComponent<Facing>().SetFacing((double) Game.Instance.roomProber.GetRoomOfGameObject(smi.master.gameObject).cavity.GetCenter().x <= (double) smi.master.transform.position.x))).ToggleAnims("anim_rage_kanim").PlayAnim("idle_pre").QueueAnim("idle_default", true).ScheduleGoTo(30f, (StateMachine.BaseState) this.emoteRoar).EventTransition(GameHashes.GameplayEventCommence, this.emoteRoar);
      this.emoteRoar.Enter("ChooseTarget", (StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State.Callback) (smi => smi.UpdateAttackTarget())).ToggleAnims("anim_rage_kanim").PlayAnim("rage_pre").QueueAnim("rage_loop").QueueAnim("rage_pst").OnAnimQueueComplete((GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State) this.fight);
      this.fight.DefaultState((GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State) this.fight.moveto);
      this.fight.moveto.InitializeStates(this.eater, this.attackableTarget, this.fight.throwFood, tactic: NavigationTactics.Range_3_ProhibitOverlap);
      this.fight.throwFood.Enter((StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State.Callback) (smi =>
      {
        smi.master.GetComponent<Facing>().Face(this.attackableTarget.Get(smi).transform.position.x);
        GameObject gameObject1 = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(FoodCometConfig.ID)), Vector3.op_Addition(smi.master.transform.position, Vector3.up));
        gameObject1.SetActive(true);
        Comet comet = gameObject1.GetComponent<Comet>();
        Vector3 vector3 = Vector3.op_Subtraction(this.attackableTarget.Get(smi).transform.position, smi.master.transform.position);
        ((Vector3) ref vector3).Normalize();
        comet.Velocity = Vector2.op_Implicit(Vector3.op_Multiply(vector3, 5f));
        comet.OnImpact += (System.Action) (() =>
        {
          GameObject gameObject2 = Grid.Objects[Grid.PosToCell((KMonoBehaviour) comet), 0];
          if (!Object.op_Inequality((Object) gameObject2, (Object) null))
            return;
          if (Random.Range(0, 100) > 75)
          {
            FleeChore fleeChore = new FleeChore(gameObject2.GetComponent<IStateMachineTarget>(), smi.master.gameObject);
          }
          else
            EventExtensions.Trigger(gameObject2, 508119890, (object) null);
        });
        GameObject first = smi.master.GetComponent<Storage>().FindFirst(GameTags.Edible);
        if (Object.op_Inequality((Object) first, (Object) null))
        {
          Edible component = first.GetComponent<Edible>();
          float num = Math.Min(200000f, component.Calories);
          component.Calories -= num;
          ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, -num, StringFormatter.Replace((string) UI.ENDOFDAYREPORT.NOTES.FOODFIGHT_CONTEXT, "{0}", ((Component) component).GetProperName()));
          if ((double) component.Calories <= 0.0)
          {
            Util.KDestroyGameObject(first);
            smi.GoTo((StateMachine.BaseState) this.end);
          }
          else
            smi.GoTo((StateMachine.BaseState) this.emoteRoar);
        }
        else
          smi.GoTo((StateMachine.BaseState) this.end);
      }));
      this.end.Enter((StateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State.Callback) (smi =>
      {
        Util.KDestroyGameObject(this.ediblechunk.Get(smi));
        smi.StopSM("complete");
      }));
    }

    public class AttackStates : 
      GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State
    {
      public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.ApproachSubState<AttackableBase> moveto;
      public GameStateMachine<FoodFightChore.States, FoodFightChore.StatesInstance, FoodFightChore, object>.State throwFood;
    }
  }
}
