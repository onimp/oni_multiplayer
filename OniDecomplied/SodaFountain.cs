// Decompiled with JetBrains decompiler
// Type: SodaFountain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SodaFountain : 
  StateMachineComponent<SodaFountain.StatesInstance>,
  IGameObjectEffectDescriptor
{
  public string specificEffect;
  public string trackingEffect;
  public Tag ingredientTag;
  public float ingredientMassPerUse;
  public float waterMassPerUse;

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
    Effect.AddModifierDescriptions(((Component) this).gameObject, descs, this.specificEffect, true);
    this.AddRequirementDesc(descs, this.ingredientTag, this.ingredientMassPerUse);
    this.AddRequirementDesc(descs, GameTags.Water, this.waterMassPerUse);
    return descs;
  }

  public class States : 
    GameStateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain>
  {
    private GameStateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.State unoperational;
    private GameStateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.State operational;
    private SodaFountain.States.ReadyStates ready;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.unoperational;
      this.unoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational);
      this.operational.PlayAnim("off").TagTransition(GameTags.Operational, this.unoperational, true).Transition((GameStateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.State) this.ready, new StateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.Transition.ConditionCallback(this.IsReady)).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.State) this.ready, new StateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.Transition.ConditionCallback(this.IsReady));
      this.ready.TagTransition(GameTags.Operational, this.unoperational, true).DefaultState(this.ready.idle).ToggleChore(new Func<SodaFountain.StatesInstance, Chore>(this.CreateChore), this.operational);
      this.ready.idle.Transition(this.operational, GameStateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.Not(new StateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.Transition.ConditionCallback(this.IsReady))).EventTransition(GameHashes.OnStorageChange, this.operational, GameStateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.Not(new StateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.Transition.ConditionCallback(this.IsReady))).WorkableStartTransition((Func<SodaFountain.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<SodaFountainWorkable>()), this.ready.working);
      this.ready.working.PlayAnim("working_pre").WorkableStopTransition((Func<SodaFountain.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<SodaFountainWorkable>()), this.ready.post);
      this.ready.post.PlayAnim("working_pst").OnAnimQueueComplete((GameStateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.State) this.ready);
    }

    private Chore CreateChore(SodaFountain.StatesInstance smi)
    {
      Workable component = (Workable) ((Component) smi.master).GetComponent<SodaFountainWorkable>();
      WorkChore<SodaFountainWorkable> chore = new WorkChore<SodaFountainWorkable>(Db.Get().ChoreTypes.Relax, (IStateMachineTarget) component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
      chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, (object) component);
      return (Chore) chore;
    }

    private bool IsReady(SodaFountain.StatesInstance smi)
    {
      PrimaryElement primaryElement = smi.GetComponent<Storage>().FindPrimaryElement(SimHashes.Water);
      return !Object.op_Equality((Object) primaryElement, (Object) null) && (double) primaryElement.Mass >= (double) smi.master.waterMassPerUse && (double) smi.GetComponent<Storage>().GetAmountAvailable(smi.master.ingredientTag) >= (double) smi.master.ingredientMassPerUse;
    }

    public class ReadyStates : 
      GameStateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.State
    {
      public GameStateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.State idle;
      public GameStateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.State working;
      public GameStateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.State post;
    }
  }

  public class StatesInstance : 
    GameStateMachine<SodaFountain.States, SodaFountain.StatesInstance, SodaFountain, object>.GameInstance
  {
    public StatesInstance(SodaFountain smi)
      : base(smi)
    {
    }
  }
}
