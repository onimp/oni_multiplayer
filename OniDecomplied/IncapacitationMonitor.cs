// Decompiled with JetBrains decompiler
// Type: IncapacitationMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class IncapacitationMonitor : 
  GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance>
{
  public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State healthy;
  public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State start_recovery;
  public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State incapacitated;
  public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State die;
  private StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter bleedOutStamina = new StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter(120f);
  private StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter baseBleedOutSpeed = new StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter(1f);
  private StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter baseStaminaRecoverSpeed = new StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter(1f);
  private StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter maxBleedOutStamina = new StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter(120f);

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.healthy;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.healthy.TagTransition(GameTags.CaloriesDepleted, this.incapacitated).TagTransition(GameTags.HitPointsDepleted, this.incapacitated).TagTransition(GameTags.HitByHighEnergyParticle, this.incapacitated).TagTransition(GameTags.RadiationSicknessIncapacitation, this.incapacitated).Update((System.Action<IncapacitationMonitor.Instance, float>) ((smi, dt) => smi.RecoverStamina(dt, smi)));
    this.start_recovery.TagTransition(new Tag[2]
    {
      GameTags.CaloriesDepleted,
      GameTags.HitPointsDepleted
    }, this.healthy, true);
    this.incapacitated.EventTransition(GameHashes.IncapacitationRecovery, this.start_recovery).ToggleTag(GameTags.Incapacitated).ToggleRecurringChore((Func<IncapacitationMonitor.Instance, Chore>) (smi => (Chore) new BeIncapacitatedChore(smi.master))).ParamTransition<float>((StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.bleedOutStamina, this.die, GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.IsLTEZero).ToggleUrge(Db.Get().Urges.BeIncapacitated).Enter((StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.master.Trigger(-1506500077, (object) null))).Update((System.Action<IncapacitationMonitor.Instance, float>) ((smi, dt) => smi.Bleed(dt, smi)));
    this.die.Enter((StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.master.gameObject.GetSMI<DeathMonitor.Instance>().Kill(smi.GetCauseOfIncapacitation())));
  }

  public new class Instance : 
    GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Instance(IStateMachineTarget master)
      : base(master)
    {
      Health component = master.GetComponent<Health>();
      if (!Object.op_Implicit((Object) component))
        return;
      component.CanBeIncapacitated = true;
    }

    public void Bleed(float dt, IncapacitationMonitor.Instance smi)
    {
      double num = (double) smi.sm.bleedOutStamina.Delta(dt * -smi.sm.baseBleedOutSpeed.Get(smi), smi);
    }

    public void RecoverStamina(float dt, IncapacitationMonitor.Instance smi)
    {
      double num = (double) smi.sm.bleedOutStamina.Delta(Mathf.Min(dt * smi.sm.baseStaminaRecoverSpeed.Get(smi), smi.sm.maxBleedOutStamina.Get(smi) - smi.sm.bleedOutStamina.Get(smi)), smi);
    }

    public float GetBleedLifeTime(IncapacitationMonitor.Instance smi) => Mathf.Floor(smi.sm.bleedOutStamina.Get(smi) / smi.sm.baseBleedOutSpeed.Get(smi));

    public Death GetCauseOfIncapacitation()
    {
      KPrefabID component = this.GetComponent<KPrefabID>();
      if (component.HasTag(GameTags.HitByHighEnergyParticle))
        return Db.Get().Deaths.HitByHighEnergyParticle;
      if (component.HasTag(GameTags.RadiationSicknessIncapacitation))
        return Db.Get().Deaths.Radiation;
      if (component.HasTag(GameTags.CaloriesDepleted))
        return Db.Get().Deaths.Starvation;
      return component.HasTag(GameTags.HitPointsDepleted) ? Db.Get().Deaths.Slain : Db.Get().Deaths.Generic;
    }
  }
}
