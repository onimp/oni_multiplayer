// Decompiled with JetBrains decompiler
// Type: SicknessMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SicknessMonitor : GameStateMachine<SicknessMonitor, SicknessMonitor.Instance>
{
  public GameStateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.State healthy;
  public SicknessMonitor.SickStates sick;
  public GameStateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.State post;
  public GameStateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.State post_nocheer;
  private static readonly HashedString SickPostKAnim = HashedString.op_Implicit("anim_cheer_kanim");
  private static readonly HashedString[] SickPostAnims = new HashedString[3]
  {
    HashedString.op_Implicit("cheer_pre"),
    HashedString.op_Implicit("cheer_loop"),
    HashedString.op_Implicit("cheer_pst")
  };

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    default_state = (StateMachine.BaseState) this.healthy;
    this.healthy.EventTransition(GameHashes.SicknessAdded, (GameStateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.State) this.sick, (StateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsSick()));
    this.sick.DefaultState(this.sick.minor).EventTransition(GameHashes.SicknessCured, this.post_nocheer, (StateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsSick())).ToggleThought(Db.Get().Thoughts.GotInfected);
    this.sick.minor.EventTransition(GameHashes.SicknessAdded, this.sick.major, (StateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.HasMajorDisease()));
    this.sick.major.EventTransition(GameHashes.SicknessCured, this.sick.minor, (StateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.HasMajorDisease())).ToggleUrge(Db.Get().Urges.RestDueToDisease).Update("AutoAssignClinic", (System.Action<SicknessMonitor.Instance, float>) ((smi, dt) => smi.AutoAssignClinic()), (UpdateRate) 7).Exit((StateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.UnassignClinic()));
    this.post_nocheer.Enter((StateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      new SicknessCuredFX.Instance(smi.master, new Vector3(0.0f, 0.0f, -0.1f)).StartSM();
      if (smi.IsSleepingOrSleepSchedule())
        smi.GoTo((StateMachine.BaseState) this.healthy);
      else
        smi.GoTo((StateMachine.BaseState) this.post);
    }));
    this.post.ToggleChore((Func<SicknessMonitor.Instance, Chore>) (smi => (Chore) new EmoteChore(smi.master, Db.Get().ChoreTypes.EmoteHighPriority, SicknessMonitor.SickPostKAnim, SicknessMonitor.SickPostAnims, (KAnim.PlayMode) 1)), this.healthy);
  }

  public class SickStates : 
    GameStateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.State minor;
    public GameStateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.State major;
  }

  public new class Instance : 
    GameStateMachine<SicknessMonitor, SicknessMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    private Sicknesses sicknesses;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.sicknesses = master.GetComponent<MinionModifiers>().sicknesses;
    }

    private string OnGetToolTip(List<Notification> notifications, object data) => (string) DUPLICANTS.STATUSITEMS.HASDISEASE.TOOLTIP;

    public bool IsSick() => this.sicknesses.Count > 0;

    public bool HasMajorDisease()
    {
      foreach (ModifierInstance<Sickness> sickness in (Modifications<Sickness, SicknessInstance>) this.sicknesses)
      {
        if (sickness.modifier.severity >= Sickness.Severity.Major)
          return true;
      }
      return false;
    }

    public void AutoAssignClinic()
    {
      Ownables soleOwner = this.sm.masterTarget.Get(this.smi).GetComponent<MinionIdentity>().GetSoleOwner();
      AssignableSlot clinic = Db.Get().AssignableSlots.Clinic;
      AssignableSlotInstance slot = soleOwner.GetSlot(clinic);
      if (slot == null || Object.op_Inequality((Object) slot.assignable, (Object) null))
        return;
      soleOwner.AutoAssignSlot(clinic);
    }

    public void UnassignClinic() => this.sm.masterTarget.Get(this.smi).GetComponent<MinionIdentity>().GetSoleOwner().GetSlot(Db.Get().AssignableSlots.Clinic)?.Unassign();

    public bool IsSleepingOrSleepSchedule()
    {
      Schedulable component1 = this.GetComponent<Schedulable>();
      if (Object.op_Inequality((Object) component1, (Object) null) && component1.IsAllowed(Db.Get().ScheduleBlockTypes.Sleep))
        return true;
      KPrefabID component2 = this.GetComponent<KPrefabID>();
      return Object.op_Inequality((Object) component2, (Object) null) && component2.HasTag(GameTags.Asleep);
    }
  }
}
