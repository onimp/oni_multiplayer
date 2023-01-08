// Decompiled with JetBrains decompiler
// Type: CreatureDiseaseCleaner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class CreatureDiseaseCleaner : 
  GameStateMachine<CreatureDiseaseCleaner, CreatureDiseaseCleaner.Instance, IStateMachineTarget, CreatureDiseaseCleaner.Def>
{
  public GameStateMachine<CreatureDiseaseCleaner, CreatureDiseaseCleaner.Instance, IStateMachineTarget, CreatureDiseaseCleaner.Def>.State behaviourcomplete;
  public CreatureDiseaseCleaner.CleaningStates cleaning;
  public StateMachine<CreatureDiseaseCleaner, CreatureDiseaseCleaner.Instance, IStateMachineTarget, CreatureDiseaseCleaner.Def>.Signal cellChangedSignal;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.cleaning;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.CLEANING.NAME, (string) CREATURES.STATUSITEMS.CLEANING.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.cleaning.DefaultState(this.cleaning.clean_pre).ScheduleGoTo((Func<CreatureDiseaseCleaner.Instance, float>) (smi => smi.def.cleanDuration), (StateMachine.BaseState) this.cleaning.clean_pst);
    this.cleaning.clean_pre.PlayAnim("clean_water_pre").OnAnimQueueComplete(this.cleaning.clean);
    this.cleaning.clean.Enter((StateMachine<CreatureDiseaseCleaner, CreatureDiseaseCleaner.Instance, IStateMachineTarget, CreatureDiseaseCleaner.Def>.State.Callback) (smi => smi.EnableDiseaseEmitter())).QueueAnim("clean_water_loop", true).Transition(this.cleaning.clean_pst, (StateMachine<CreatureDiseaseCleaner, CreatureDiseaseCleaner.Instance, IStateMachineTarget, CreatureDiseaseCleaner.Def>.Transition.ConditionCallback) (smi => !smi.GetSMI<CleaningMonitor.Instance>().CanCleanElementState()), (UpdateRate) 6).Exit((StateMachine<CreatureDiseaseCleaner, CreatureDiseaseCleaner.Instance, IStateMachineTarget, CreatureDiseaseCleaner.Def>.State.Callback) (smi => smi.EnableDiseaseEmitter(false)));
    this.cleaning.clean_pst.PlayAnim("clean_water_pst").OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Cleaning);
  }

  public class Def : StateMachine.BaseDef
  {
    public float cleanDuration;

    public Def(float duration) => this.cleanDuration = duration;
  }

  public class CleaningStates : 
    GameStateMachine<CreatureDiseaseCleaner, CreatureDiseaseCleaner.Instance, IStateMachineTarget, CreatureDiseaseCleaner.Def>.State
  {
    public GameStateMachine<CreatureDiseaseCleaner, CreatureDiseaseCleaner.Instance, IStateMachineTarget, CreatureDiseaseCleaner.Def>.State clean_pre;
    public GameStateMachine<CreatureDiseaseCleaner, CreatureDiseaseCleaner.Instance, IStateMachineTarget, CreatureDiseaseCleaner.Def>.State clean;
    public GameStateMachine<CreatureDiseaseCleaner, CreatureDiseaseCleaner.Instance, IStateMachineTarget, CreatureDiseaseCleaner.Def>.State clean_pst;
  }

  public new class Instance : 
    GameStateMachine<CreatureDiseaseCleaner, CreatureDiseaseCleaner.Instance, IStateMachineTarget, CreatureDiseaseCleaner.Def>.GameInstance
  {
    public Instance(Chore<CreatureDiseaseCleaner.Instance> chore, CreatureDiseaseCleaner.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.Cleaning);
    }

    public void EnableDiseaseEmitter(bool enable = true)
    {
      DiseaseEmitter component = this.GetComponent<DiseaseEmitter>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.SetEnable(enable);
    }
  }
}
