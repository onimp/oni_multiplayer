// Decompiled with JetBrains decompiler
// Type: EspressoMachine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EspressoMachine : 
  StateMachineComponent<EspressoMachine.StatesInstance>,
  IGameObjectEffectDescriptor
{
  public const string SPECIFIC_EFFECT = "Espresso";
  public const string TRACKING_EFFECT = "RecentlyRecDrink";
  public static Tag INGREDIENT_TAG = new Tag("SpiceNut");
  public static float INGREDIENT_MASS_PER_USE = 1f;
  public static float WATER_MASS_PER_USE = 1f;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule)), (object) null, (SchedulerGroup) null);
  }

  protected override void OnCleanUp() => base.OnCleanUp();

  private void AddRequirementDesc(List<Descriptor> descs, Tag tag, float mass)
  {
    string str = tag.ProperName();
    Descriptor descriptor = new Descriptor();
    ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, (object) str, (object) GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}")), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, (object) str, (object) GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}")), (Descriptor.DescriptorType) 0);
    descs.Add(descriptor);
  }

  List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
  {
    List<Descriptor> descs = new List<Descriptor>();
    Descriptor descriptor = new Descriptor();
    ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.RECREATION, (string) UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, (Descriptor.DescriptorType) 1);
    descs.Add(descriptor);
    Effect.AddModifierDescriptions(((Component) this).gameObject, descs, "Espresso", true);
    this.AddRequirementDesc(descs, EspressoMachine.INGREDIENT_TAG, EspressoMachine.INGREDIENT_MASS_PER_USE);
    this.AddRequirementDesc(descs, GameTags.Water, EspressoMachine.WATER_MASS_PER_USE);
    return descs;
  }

  public class States : 
    GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine>
  {
    private GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State unoperational;
    private GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State operational;
    private EspressoMachine.States.ReadyStates ready;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.unoperational;
      this.unoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational);
      this.operational.PlayAnim("off").TagTransition(GameTags.Operational, this.unoperational, true).Transition((GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State) this.ready, new StateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.Transition.ConditionCallback(this.IsReady)).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State) this.ready, new StateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.Transition.ConditionCallback(this.IsReady));
      this.ready.TagTransition(GameTags.Operational, this.unoperational, true).DefaultState(this.ready.idle).ToggleChore(new Func<EspressoMachine.StatesInstance, Chore>(this.CreateChore), this.operational);
      this.ready.idle.PlayAnim("on", (KAnim.PlayMode) 0).WorkableStartTransition((Func<EspressoMachine.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<EspressoMachineWorkable>()), this.ready.working).Transition(this.operational, GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.Not(new StateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.Transition.ConditionCallback(this.IsReady))).EventTransition(GameHashes.OnStorageChange, this.operational, GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.Not(new StateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.Transition.ConditionCallback(this.IsReady)));
      this.ready.working.PlayAnim("working_pre").QueueAnim("working_loop", true).WorkableStopTransition((Func<EspressoMachine.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<EspressoMachineWorkable>()), this.ready.post);
      this.ready.post.PlayAnim("working_pst").OnAnimQueueComplete((GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State) this.ready);
    }

    private Chore CreateChore(EspressoMachine.StatesInstance smi)
    {
      Workable component = (Workable) ((Component) smi.master).GetComponent<EspressoMachineWorkable>();
      WorkChore<EspressoMachineWorkable> chore = new WorkChore<EspressoMachineWorkable>(Db.Get().ChoreTypes.Relax, (IStateMachineTarget) component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
      chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, (object) component);
      return (Chore) chore;
    }

    private bool IsReady(EspressoMachine.StatesInstance smi)
    {
      PrimaryElement primaryElement = smi.GetComponent<Storage>().FindPrimaryElement(SimHashes.Water);
      return !Object.op_Equality((Object) primaryElement, (Object) null) && (double) primaryElement.Mass >= (double) EspressoMachine.WATER_MASS_PER_USE && (double) smi.GetComponent<Storage>().GetAmountAvailable(EspressoMachine.INGREDIENT_TAG) >= (double) EspressoMachine.INGREDIENT_MASS_PER_USE;
    }

    public class ReadyStates : 
      GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State
    {
      public GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State idle;
      public GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State working;
      public GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.State post;
    }
  }

  public class StatesInstance : 
    GameStateMachine<EspressoMachine.States, EspressoMachine.StatesInstance, EspressoMachine, object>.GameInstance
  {
    public StatesInstance(EspressoMachine smi)
      : base(smi)
    {
    }
  }
}
