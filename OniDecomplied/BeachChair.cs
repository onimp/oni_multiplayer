// Decompiled with JetBrains decompiler
// Type: BeachChair
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BeachChair : 
  StateMachineComponent<BeachChair.StatesInstance>,
  IGameObjectEffectDescriptor
{
  public string specificEffectUnlit;
  public string specificEffectLit;
  public string trackingEffect;
  public const float LIT_RATIO_FOR_POSITIVE_EFFECT = 0.75f;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  protected override void OnCleanUp() => base.OnCleanUp();

  public static void AddModifierDescriptions(
    List<Descriptor> descs,
    string effect_id,
    bool high_lux)
  {
    Effect effect = Db.Get().effects.Get(effect_id);
    LocString locString1 = high_lux ? BUILDINGS.PREFABS.BEACHCHAIR.LIGHTEFFECT_HIGH : BUILDINGS.PREFABS.BEACHCHAIR.LIGHTEFFECT_LOW;
    LocString locString2 = high_lux ? BUILDINGS.PREFABS.BEACHCHAIR.LIGHTEFFECT_HIGH_TOOLTIP : BUILDINGS.PREFABS.BEACHCHAIR.LIGHTEFFECT_LOW_TOOLTIP;
    foreach (AttributeModifier selfModifier in effect.SelfModifiers)
    {
      Descriptor descriptor;
      // ISSUE: explicit constructor call
      ((Descriptor) ref descriptor).\u002Ector(locString1.Replace("{attrib}", StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".NAME"))).Replace("{amount}", selfModifier.GetFormattedString()).Replace("{lux}", GameUtil.GetFormattedLux(10000)), locString2.Replace("{attrib}", StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".NAME"))).Replace("{amount}", selfModifier.GetFormattedString()).Replace("{lux}", GameUtil.GetFormattedLux(10000)), (Descriptor.DescriptorType) 1, false);
      ((Descriptor) ref descriptor).IncreaseIndent();
      descs.Add(descriptor);
    }
  }

  List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
  {
    List<Descriptor> descs = new List<Descriptor>();
    descs.Add(new Descriptor((string) UI.BUILDINGEFFECTS.RECREATION, (string) UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, (Descriptor.DescriptorType) 1, false));
    BeachChair.AddModifierDescriptions(descs, this.specificEffectLit, true);
    BeachChair.AddModifierDescriptions(descs, this.specificEffectUnlit, false);
    return descs;
  }

  public void SetLit(bool v) => this.smi.sm.lit.Set(v, this.smi);

  public void SetWorker(Worker worker) => this.smi.sm.worker.Set((KMonoBehaviour) worker, this.smi);

  public class States : GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair>
  {
    public StateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.BoolParameter lit;
    public StateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.TargetParameter worker;
    private GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State inoperational;
    private BeachChair.States.ReadyStates ready;
    private HashedString[] UNLIT_PST_ANIMS = new HashedString[2]
    {
      HashedString.op_Implicit("working_unlit_pst"),
      HashedString.op_Implicit("working_pst")
    };
    private HashedString[] LIT_PST_ANIMS = new HashedString[2]
    {
      HashedString.op_Implicit("working_lit_pst"),
      HashedString.op_Implicit("working_pst")
    };
    private string[] SILLY_ANIMS = new string[3]
    {
      "working_lit_loop1",
      "working_lit_loop2",
      "working_lit_loop3"
    };

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.inoperational;
      this.inoperational.PlayAnim("off").TagTransition(GameTags.Operational, (GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State) this.ready).ToggleMainStatusItem(Db.Get().BuildingStatusItems.MissingRequirements);
      this.ready.TagTransition(GameTags.Operational, this.inoperational, true).DefaultState(this.ready.idle).ToggleChore(new Func<BeachChair.StatesInstance, Chore>(this.CreateChore), this.inoperational).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working);
      this.ready.idle.PlayAnim("on", (KAnim.PlayMode) 0).WorkableStartTransition((Func<BeachChair.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<BeachChairWorkable>()), this.ready.working_pre);
      this.ready.working_pre.PlayAnim("working_pre").QueueAnim("working_loop", true).Target(this.worker).PlayAnim("working_pre").EventHandler(GameHashes.AnimQueueComplete, (StateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State.Callback) (smi =>
      {
        if (this.lit.Get(smi))
          smi.GoTo((StateMachine.BaseState) this.ready.working_lit);
        else
          smi.GoTo((StateMachine.BaseState) this.ready.working_unlit);
      }));
      this.ready.working_unlit.DefaultState(this.ready.working_unlit.working).Enter((StateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State.Callback) (smi =>
      {
        BeachChairWorkable component = ((Component) smi.master).GetComponent<BeachChairWorkable>();
        HashedString[] unlitPstAnims;
        HashedString[] hashedStringArray = unlitPstAnims = this.UNLIT_PST_ANIMS;
        component.workingPstFailed = unlitPstAnims;
        component.workingPstComplete = hashedStringArray;
      })).ToggleStatusItem(Db.Get().BuildingStatusItems.TanningLightInsufficient).WorkableStopTransition((Func<BeachChair.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<BeachChairWorkable>()), this.ready.post).Target(this.worker).PlayAnim("working_unlit_pre");
      this.ready.working_unlit.working.ParamTransition<bool>((StateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.Parameter<bool>) this.lit, this.ready.working_unlit.post, GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.IsTrue).Target(this.worker).QueueAnim("working_unlit_loop", true);
      this.ready.working_unlit.post.Target(this.worker).PlayAnim("working_unlit_pst").EventHandler(GameHashes.AnimQueueComplete, (StateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State.Callback) (smi =>
      {
        if (this.lit.Get(smi))
          smi.GoTo((StateMachine.BaseState) this.ready.working_lit);
        else
          smi.GoTo((StateMachine.BaseState) this.ready.working_unlit.working);
      }));
      this.ready.working_lit.DefaultState(this.ready.working_lit.working).Enter((StateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State.Callback) (smi =>
      {
        BeachChairWorkable component = ((Component) smi.master).GetComponent<BeachChairWorkable>();
        HashedString[] litPstAnims;
        HashedString[] hashedStringArray = litPstAnims = this.LIT_PST_ANIMS;
        component.workingPstFailed = litPstAnims;
        component.workingPstComplete = hashedStringArray;
      })).ToggleStatusItem(Db.Get().BuildingStatusItems.TanningLightSufficient).WorkableStopTransition((Func<BeachChair.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<BeachChairWorkable>()), this.ready.post).Target(this.worker).PlayAnim("working_lit_pre");
      this.ready.working_lit.working.ParamTransition<bool>((StateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.Parameter<bool>) this.lit, this.ready.working_lit.post, GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.IsFalse).Target(this.worker).QueueAnim("working_lit_loop", true).ScheduleGoTo((Func<BeachChair.StatesInstance, float>) (smi => Random.Range(5f, 15f)), (StateMachine.BaseState) this.ready.working_lit.silly);
      this.ready.working_lit.silly.ParamTransition<bool>((StateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.Parameter<bool>) this.lit, this.ready.working_lit.post, GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.IsFalse).Target(this.worker).PlayAnim((Func<BeachChair.StatesInstance, string>) (smi => this.SILLY_ANIMS[Random.Range(0, this.SILLY_ANIMS.Length)])).OnAnimQueueComplete(this.ready.working_lit.working);
      this.ready.working_lit.post.Target(this.worker).PlayAnim("working_lit_pst").EventHandler(GameHashes.AnimQueueComplete, (StateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State.Callback) (smi =>
      {
        if (!this.lit.Get(smi))
          smi.GoTo((StateMachine.BaseState) this.ready.working_unlit);
        else
          smi.GoTo((StateMachine.BaseState) this.ready.working_lit.working);
      }));
      this.ready.post.PlayAnim("working_pst").Exit((StateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State.Callback) (smi => this.worker.Set((KMonoBehaviour) null, smi))).OnAnimQueueComplete((GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State) this.ready);
    }

    private Chore CreateChore(BeachChair.StatesInstance smi)
    {
      Workable component = (Workable) ((Component) smi.master).GetComponent<BeachChairWorkable>();
      WorkChore<BeachChairWorkable> chore = new WorkChore<BeachChairWorkable>(Db.Get().ChoreTypes.Relax, (IStateMachineTarget) component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
      chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, (object) component);
      return (Chore) chore;
    }

    public class LitWorkingStates : 
      GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State
    {
      public GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State working;
      public GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State silly;
      public GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State post;
    }

    public class WorkingStates : 
      GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State
    {
      public GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State working;
      public GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State post;
    }

    public class ReadyStates : 
      GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State
    {
      public GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State idle;
      public GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State working_pre;
      public BeachChair.States.WorkingStates working_unlit;
      public BeachChair.States.LitWorkingStates working_lit;
      public GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.State post;
    }
  }

  public class StatesInstance : 
    GameStateMachine<BeachChair.States, BeachChair.StatesInstance, BeachChair, object>.GameInstance
  {
    public StatesInstance(BeachChair smi)
      : base(smi)
    {
    }
  }
}
