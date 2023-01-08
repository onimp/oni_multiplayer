// Decompiled with JetBrains decompiler
// Type: InhaleStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class InhaleStates : 
  GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>
{
  public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State goingtoeat;
  public InhaleStates.InhalingStates inhaling;
  public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State behaviourcomplete;
  public StateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.IntParameter targetCell;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.goingtoeat;
    this.root.Enter("SetTarget", (StateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State.Callback) (smi => this.targetCell.Set(smi.monitor.targetCell, smi)));
    this.goingtoeat.MoveTo((Func<InhaleStates.Instance, int>) (smi => this.targetCell.Get(smi)), (GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State) this.inhaling).ToggleMainStatusItem(new Func<InhaleStates.Instance, StatusItem>(InhaleStates.GetMovingStatusItem));
    this.inhaling.DefaultState(this.inhaling.inhale).ToggleStatusItem((string) CREATURES.STATUSITEMS.INHALING.NAME, (string) CREATURES.STATUSITEMS.INHALING.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.inhaling.inhale.PlayAnim((Func<InhaleStates.Instance, string>) (smi => smi.def.inhaleAnimPre)).QueueAnim((Func<InhaleStates.Instance, string>) (smi => smi.def.inhaleAnimLoop), true).Update("Consume", (System.Action<InhaleStates.Instance, float>) ((smi, dt) => smi.monitor.Consume(dt))).EventTransition(GameHashes.ElementNoLongerAvailable, this.inhaling.pst).Enter("StartInhaleSound", (StateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State.Callback) (smi => smi.StartInhaleSound())).Exit("StopInhaleSound", (StateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State.Callback) (smi => smi.StopInhaleSound())).ScheduleGoTo((Func<InhaleStates.Instance, float>) (smi => smi.def.inhaleTime), (StateMachine.BaseState) this.inhaling.pst);
    this.inhaling.pst.Transition(this.inhaling.full, (StateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.Transition.ConditionCallback) (smi => smi.def.alwaysPlayPstAnim || InhaleStates.IsFull(smi))).Transition(this.behaviourcomplete, GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.Not(new StateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.Transition.ConditionCallback(InhaleStates.IsFull)));
    this.inhaling.full.QueueAnim((Func<InhaleStates.Instance, string>) (smi => smi.def.inhaleAnimPst)).OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.PlayAnim("idle_loop", (KAnim.PlayMode) 0).BehaviourComplete((Func<InhaleStates.Instance, Tag>) (smi => smi.def.behaviourTag));
  }

  private static StatusItem GetMovingStatusItem(InhaleStates.Instance smi) => smi.def.useStorage ? smi.def.storageStatusItem : Db.Get().CreatureStatusItems.LookingForFood;

  private static bool IsFull(InhaleStates.Instance smi)
  {
    if (smi.def.useStorage)
    {
      if (Object.op_Inequality((Object) smi.storage, (Object) null))
        return smi.storage.IsFull();
    }
    else
    {
      CreatureCalorieMonitor.Instance smi1 = smi.GetSMI<CreatureCalorieMonitor.Instance>();
      if (smi1 != null)
        return (double) smi1.stomach.GetFullness() >= 1.0;
    }
    return false;
  }

  public class Def : StateMachine.BaseDef
  {
    public string inhaleSound;
    public float inhaleTime = 3f;
    public Tag behaviourTag = GameTags.Creatures.WantsToEat;
    public bool useStorage;
    public string inhaleAnimPre = "inhale_pre";
    public string inhaleAnimLoop = "inhale_loop";
    public string inhaleAnimPst = "inhale_pst";
    public bool alwaysPlayPstAnim;
    public StatusItem storageStatusItem = Db.Get().CreatureStatusItems.LookingForGas;
  }

  public new class Instance : 
    GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.GameInstance
  {
    public string inhaleSound;
    [MySmiGet]
    public GasAndLiquidConsumerMonitor.Instance monitor;
    [MyCmpGet]
    public Storage storage;

    public Instance(Chore<InhaleStates.Instance> chore, InhaleStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) def.behaviourTag);
      this.inhaleSound = GlobalAssets.GetSound(def.inhaleSound);
    }

    public void StartInhaleSound()
    {
      LoopingSounds component = this.GetComponent<LoopingSounds>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.StartSound(this.smi.inhaleSound);
    }

    public void StopInhaleSound()
    {
      LoopingSounds component = this.GetComponent<LoopingSounds>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.StopSound(this.smi.inhaleSound);
    }
  }

  public class InhalingStates : 
    GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State
  {
    public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State inhale;
    public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State pst;
    public GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>.State full;
  }
}
