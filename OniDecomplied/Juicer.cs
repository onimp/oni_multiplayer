// Decompiled with JetBrains decompiler
// Type: Juicer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Juicer : StateMachineComponent<Juicer.StatesInstance>, IGameObjectEffectDescriptor
{
  public string specificEffect;
  public string trackingEffect;
  public Tag[] ingredientTags;
  public float[] ingredientMassesPerUse;
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
    string str1 = tag.ProperName();
    Descriptor descriptor = new Descriptor();
    string str2 = EdiblesManager.GetFoodInfo(((Tag) ref tag).Name) != null ? GameUtil.GetFormattedCaloriesForItem(tag, mass) : GameUtil.GetFormattedMass(mass, massFormat: GameUtil.MetricMassFormat.Kilogram);
    ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, (object) str1, (object) str2), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, (object) str1, (object) str2), (Descriptor.DescriptorType) 0);
    descs.Add(descriptor);
  }

  List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
  {
    List<Descriptor> descs = new List<Descriptor>();
    Descriptor descriptor = new Descriptor();
    ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.RECREATION, (string) UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, (Descriptor.DescriptorType) 1);
    descs.Add(descriptor);
    Effect.AddModifierDescriptions(((Component) this).gameObject, descs, this.specificEffect, true);
    for (int index = 0; index < this.ingredientTags.Length; ++index)
      this.AddRequirementDesc(descs, this.ingredientTags[index], this.ingredientMassesPerUse[index]);
    this.AddRequirementDesc(descs, GameTags.Water, this.waterMassPerUse);
    return descs;
  }

  public class States : GameStateMachine<Juicer.States, Juicer.StatesInstance, Juicer>
  {
    private GameStateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.State unoperational;
    private GameStateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.State operational;
    private Juicer.States.ReadyStates ready;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.unoperational;
      this.unoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational);
      this.operational.PlayAnim("off").TagTransition(GameTags.Operational, this.unoperational, true).Transition((GameStateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.State) this.ready, new StateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.Transition.ConditionCallback(this.IsReady)).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.State) this.ready, new StateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.Transition.ConditionCallback(this.IsReady));
      this.ready.TagTransition(GameTags.Operational, this.unoperational, true).DefaultState(this.ready.idle).ToggleChore(new Func<Juicer.StatesInstance, Chore>(this.CreateChore), this.operational);
      this.ready.idle.Transition(this.operational, GameStateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.Not(new StateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.Transition.ConditionCallback(this.IsReady))).EventTransition(GameHashes.OnStorageChange, this.operational, GameStateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.Not(new StateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.Transition.ConditionCallback(this.IsReady))).PlayAnim("on").WorkableStartTransition((Func<Juicer.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<JuicerWorkable>()), this.ready.working);
      this.ready.working.PlayAnim("working_pre").QueueAnim("working_loop", true).WorkableStopTransition((Func<Juicer.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<JuicerWorkable>()), this.ready.post);
      this.ready.post.PlayAnim("working_pst").OnAnimQueueComplete((GameStateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.State) this.ready);
    }

    private Chore CreateChore(Juicer.StatesInstance smi)
    {
      Workable component = (Workable) ((Component) smi.master).GetComponent<JuicerWorkable>();
      WorkChore<JuicerWorkable> chore = new WorkChore<JuicerWorkable>(Db.Get().ChoreTypes.Relax, (IStateMachineTarget) component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
      chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, (object) component);
      return (Chore) chore;
    }

    private bool IsReady(Juicer.StatesInstance smi)
    {
      PrimaryElement primaryElement = smi.GetComponent<Storage>().FindPrimaryElement(SimHashes.Water);
      if (Object.op_Equality((Object) primaryElement, (Object) null) || (double) primaryElement.Mass < (double) smi.master.waterMassPerUse)
        return false;
      for (int index = 0; index < smi.master.ingredientTags.Length; ++index)
      {
        if ((double) smi.GetComponent<Storage>().GetAmountAvailable(smi.master.ingredientTags[index]) < (double) smi.master.ingredientMassesPerUse[index])
          return false;
      }
      return true;
    }

    public class ReadyStates : 
      GameStateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.State
    {
      public GameStateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.State idle;
      public GameStateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.State working;
      public GameStateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.State post;
    }
  }

  public class StatesInstance : 
    GameStateMachine<Juicer.States, Juicer.StatesInstance, Juicer, object>.GameInstance
  {
    public StatesInstance(Juicer smi)
      : base(smi)
    {
    }
  }
}
