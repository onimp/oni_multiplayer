// Decompiled with JetBrains decompiler
// Type: MechanicalSurfboard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MechanicalSurfboard : 
  StateMachineComponent<MechanicalSurfboard.StatesInstance>,
  IGameObjectEffectDescriptor
{
  public string specificEffect;
  public string trackingEffect;
  public float waterSpillRateKG;
  public float minOperationalWaterKG;
  public string[] interactAnims = new string[3]
  {
    "anim_interacts_mechanical_surfboard_kanim",
    "anim_interacts_mechanical_surfboard2_kanim",
    "anim_interacts_mechanical_surfboard3_kanim"
  };

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  protected override void OnCleanUp() => base.OnCleanUp();

  List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
  {
    List<Descriptor> descs = new List<Descriptor>();
    Element elementByHash = ElementLoader.FindElementByHash(SimHashes.Water);
    descs.Add(new Descriptor((string) UI.BUILDINGEFFECTS.RECREATION, (string) UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, (Descriptor.DescriptorType) 1, false));
    Effect.AddModifierDescriptions(((Component) this).gameObject, descs, this.specificEffect, true);
    descs.Add(new Descriptor(BUILDINGS.PREFABS.MECHANICALSURFBOARD.WATER_REQUIREMENT.Replace("{element}", elementByHash.name).Replace("{amount}", GameUtil.GetFormattedMass(this.minOperationalWaterKG)), BUILDINGS.PREFABS.MECHANICALSURFBOARD.WATER_REQUIREMENT_TOOLTIP.Replace("{element}", elementByHash.name).Replace("{amount}", GameUtil.GetFormattedMass(this.minOperationalWaterKG)), (Descriptor.DescriptorType) 0, false));
    List<Descriptor> descriptorList = descs;
    Descriptor descriptor1 = new Descriptor(BUILDINGS.PREFABS.MECHANICALSURFBOARD.LEAK_REQUIREMENT.Replace("{amount}", GameUtil.GetFormattedMass(this.waterSpillRateKG, GameUtil.TimeSlice.PerSecond)), BUILDINGS.PREFABS.MECHANICALSURFBOARD.LEAK_REQUIREMENT_TOOLTIP.Replace("{amount}", GameUtil.GetFormattedMass(this.waterSpillRateKG, GameUtil.TimeSlice.PerSecond)), (Descriptor.DescriptorType) 0, false);
    Descriptor descriptor2 = ((Descriptor) ref descriptor1).IncreaseIndent();
    descriptorList.Add(descriptor2);
    return descs;
  }

  public class States : 
    GameStateMachine<MechanicalSurfboard.States, MechanicalSurfboard.StatesInstance, MechanicalSurfboard>
  {
    private GameStateMachine<MechanicalSurfboard.States, MechanicalSurfboard.StatesInstance, MechanicalSurfboard, object>.State inoperational;
    private GameStateMachine<MechanicalSurfboard.States, MechanicalSurfboard.StatesInstance, MechanicalSurfboard, object>.State operational;
    private MechanicalSurfboard.States.ReadyStates ready;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.inoperational;
      this.inoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational).ToggleMainStatusItem(Db.Get().BuildingStatusItems.MissingRequirements);
      this.operational.PlayAnim("off").TagTransition(GameTags.Operational, this.inoperational, true).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<MechanicalSurfboard.States, MechanicalSurfboard.StatesInstance, MechanicalSurfboard, object>.State) this.ready, new StateMachine<MechanicalSurfboard.States, MechanicalSurfboard.StatesInstance, MechanicalSurfboard, object>.Transition.ConditionCallback(this.IsReady)).ToggleMainStatusItem(Db.Get().BuildingStatusItems.GettingReady);
      this.ready.TagTransition(GameTags.Operational, this.inoperational, true).DefaultState(this.ready.idle).ToggleChore(new Func<MechanicalSurfboard.StatesInstance, Chore>(this.CreateChore), this.operational).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working);
      this.ready.idle.PlayAnim("on", (KAnim.PlayMode) 0).WorkableStartTransition((Func<MechanicalSurfboard.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<MechanicalSurfboardWorkable>()), this.ready.working).EventTransition(GameHashes.OnStorageChange, this.operational, GameStateMachine<MechanicalSurfboard.States, MechanicalSurfboard.StatesInstance, MechanicalSurfboard, object>.Not(new StateMachine<MechanicalSurfboard.States, MechanicalSurfboard.StatesInstance, MechanicalSurfboard, object>.Transition.ConditionCallback(this.IsReady)));
      this.ready.working.PlayAnim("working_pre").QueueAnim("working_loop", true).WorkableStopTransition((Func<MechanicalSurfboard.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<MechanicalSurfboardWorkable>()), this.ready.post);
      this.ready.post.PlayAnim("working_pst").OnAnimQueueComplete((GameStateMachine<MechanicalSurfboard.States, MechanicalSurfboard.StatesInstance, MechanicalSurfboard, object>.State) this.ready);
    }

    private Chore CreateChore(MechanicalSurfboard.StatesInstance smi)
    {
      Workable component = (Workable) ((Component) smi.master).GetComponent<MechanicalSurfboardWorkable>();
      WorkChore<MechanicalSurfboardWorkable> chore = new WorkChore<MechanicalSurfboardWorkable>(Db.Get().ChoreTypes.Relax, (IStateMachineTarget) component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
      chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, (object) component);
      return (Chore) chore;
    }

    private bool IsReady(MechanicalSurfboard.StatesInstance smi)
    {
      PrimaryElement primaryElement = smi.GetComponent<Storage>().FindPrimaryElement(SimHashes.Water);
      return !Object.op_Equality((Object) primaryElement, (Object) null) && (double) primaryElement.Mass >= (double) smi.master.minOperationalWaterKG;
    }

    public class ReadyStates : 
      GameStateMachine<MechanicalSurfboard.States, MechanicalSurfboard.StatesInstance, MechanicalSurfboard, object>.State
    {
      public GameStateMachine<MechanicalSurfboard.States, MechanicalSurfboard.StatesInstance, MechanicalSurfboard, object>.State idle;
      public GameStateMachine<MechanicalSurfboard.States, MechanicalSurfboard.StatesInstance, MechanicalSurfboard, object>.State working;
      public GameStateMachine<MechanicalSurfboard.States, MechanicalSurfboard.StatesInstance, MechanicalSurfboard, object>.State post;
    }
  }

  public class StatesInstance : 
    GameStateMachine<MechanicalSurfboard.States, MechanicalSurfboard.StatesInstance, MechanicalSurfboard, object>.GameInstance
  {
    public StatesInstance(MechanicalSurfboard smi)
      : base(smi)
    {
    }
  }
}
