// Decompiled with JetBrains decompiler
// Type: Sauna
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Sauna : StateMachineComponent<Sauna.StatesInstance>, IGameObjectEffectDescriptor
{
  public string specificEffect;
  public string trackingEffect;
  public float steamPerUseKG;
  public float waterOutputTemp;
  public static readonly Operational.Flag sufficientSteam = new Operational.Flag(nameof (sufficientSteam), Operational.Flag.Type.Requirement);

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
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
    descs.Add(new Descriptor((string) UI.BUILDINGEFFECTS.RECREATION, (string) UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, (Descriptor.DescriptorType) 1, false));
    Effect.AddModifierDescriptions(((Component) this).gameObject, descs, this.specificEffect, true);
    Element elementByHash1 = ElementLoader.FindElementByHash(SimHashes.Steam);
    descs.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, (object) elementByHash1.name, (object) GameUtil.GetFormattedMass(this.steamPerUseKG, floatFormat: "{0:0.##}")), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, (object) elementByHash1.name, (object) GameUtil.GetFormattedMass(this.steamPerUseKG, floatFormat: "{0:0.##}")), (Descriptor.DescriptorType) 0, false));
    Element elementByHash2 = ElementLoader.FindElementByHash(SimHashes.Water);
    descs.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTEMITTEDPERUSE, (object) elementByHash2.name, (object) GameUtil.GetFormattedMass(this.steamPerUseKG, floatFormat: "{0:0.##}")), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTEDPERUSE, (object) elementByHash2.name, (object) GameUtil.GetFormattedMass(this.steamPerUseKG, floatFormat: "{0:0.##}")), (Descriptor.DescriptorType) 1, false));
    return descs;
  }

  public class States : GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna>
  {
    private GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.State inoperational;
    private GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.State operational;
    private Sauna.States.ReadyStates ready;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.inoperational;
      this.inoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.operational).ToggleMainStatusItem(Db.Get().BuildingStatusItems.MissingRequirements);
      this.operational.TagTransition(GameTags.Operational, this.inoperational, true).ToggleMainStatusItem(Db.Get().BuildingStatusItems.GettingReady).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.State) this.ready, new StateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.Transition.ConditionCallback(this.IsReady));
      this.ready.TagTransition(GameTags.Operational, this.inoperational, true).DefaultState(this.ready.idle).ToggleChore(new Func<Sauna.StatesInstance, Chore>(this.CreateChore), this.inoperational).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working);
      this.ready.idle.WorkableStartTransition((Func<Sauna.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<SaunaWorkable>()), this.ready.working).EventTransition(GameHashes.OnStorageChange, this.operational, GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.Not(new StateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.Transition.ConditionCallback(this.IsReady)));
      this.ready.working.WorkableCompleteTransition((Func<Sauna.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<SaunaWorkable>()), this.ready.idle).WorkableStopTransition((Func<Sauna.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<SaunaWorkable>()), this.ready.idle);
    }

    private Chore CreateChore(Sauna.StatesInstance smi)
    {
      Workable component = (Workable) ((Component) smi.master).GetComponent<SaunaWorkable>();
      WorkChore<SaunaWorkable> chore = new WorkChore<SaunaWorkable>(Db.Get().ChoreTypes.Relax, (IStateMachineTarget) component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
      chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, (object) component);
      return (Chore) chore;
    }

    private bool IsReady(Sauna.StatesInstance smi)
    {
      PrimaryElement primaryElement = smi.GetComponent<Storage>().FindPrimaryElement(SimHashes.Steam);
      return Object.op_Inequality((Object) primaryElement, (Object) null) && (double) primaryElement.Mass >= (double) smi.master.steamPerUseKG;
    }

    public class ReadyStates : 
      GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.State
    {
      public GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.State idle;
      public GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.State working;
    }
  }

  public class StatesInstance : 
    GameStateMachine<Sauna.States, Sauna.StatesInstance, Sauna, object>.GameInstance
  {
    public StatesInstance(Sauna smi)
      : base(smi)
    {
    }
  }
}
