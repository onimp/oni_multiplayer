// Decompiled with JetBrains decompiler
// Type: BeIncapacitatedChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class BeIncapacitatedChore : Chore<BeIncapacitatedChore.StatesInstance>
{
  private static string IncapacitatedDuplicantAnim_pre = "incapacitate_pre";
  private static string IncapacitatedDuplicantAnim_loop = "incapacitate_loop";
  private static string IncapacitatedDuplicantAnim_death = "incapacitate_death";
  private static string IncapacitatedDuplicantAnim_carry = "carry_loop";
  private static string IncapacitatedDuplicantAnim_place = "place";

  public void FindAvailableMedicalBed(Navigator navigator)
  {
    Clinic clinic1 = (Clinic) null;
    AssignableSlot clinic2 = Db.Get().AssignableSlots.Clinic;
    Ownables soleOwner = this.gameObject.GetComponent<MinionIdentity>().GetSoleOwner();
    AssignableSlotInstance slot = soleOwner.GetSlot(clinic2);
    if (Object.op_Equality((Object) slot.assignable, (Object) null))
    {
      Assignable assignable = soleOwner.AutoAssignSlot(clinic2);
      if (Object.op_Inequality((Object) assignable, (Object) null))
        clinic1 = ((Component) assignable).GetComponent<Clinic>();
    }
    else
      clinic1 = ((Component) slot.assignable).GetComponent<Clinic>();
    if (!Object.op_Inequality((Object) clinic1, (Object) null) || !navigator.CanReach((IApproachable) clinic1))
      return;
    this.smi.sm.clinic.Set(((Component) clinic1).gameObject, this.smi, false);
    this.smi.GoTo((StateMachine.BaseState) this.smi.sm.incapacitation_root.rescue.waitingForPickup);
  }

  public GameObject GetChosenClinic() => this.smi.sm.clinic.Get(this.smi);

  public BeIncapacitatedChore(IStateMachineTarget master)
    : base(Db.Get().ChoreTypes.BeIncapacitated, master, master.GetComponent<ChoreProvider>(), master_priority_class: PriorityScreen.PriorityClass.compulsory)
  {
    this.smi = new BeIncapacitatedChore.StatesInstance(this);
  }

  public class StatesInstance : 
    GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.GameInstance
  {
    public StatesInstance(BeIncapacitatedChore master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore>
  {
    public BeIncapacitatedChore.States.IncapacitatedStates incapacitation_root;
    public StateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.TargetParameter clinic;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.root;
      this.root.ToggleAnims("anim_incapacitated_kanim").ToggleStatusItem(Db.Get().DuplicantStatusItems.Incapacitated, (Func<BeIncapacitatedChore.StatesInstance, object>) (smi => (object) smi.master.gameObject.GetSMI<IncapacitationMonitor.Instance>())).Enter((StateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State.Callback) (smi =>
      {
        smi.SetStatus(StateMachine.Status.Failed);
        smi.GoTo((StateMachine.BaseState) this.incapacitation_root.lookingForBed);
      }));
      this.incapacitation_root.EventHandler(GameHashes.Died, (StateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State.Callback) (smi =>
      {
        smi.SetStatus(StateMachine.Status.Failed);
        smi.StopSM("died");
      }));
      this.incapacitation_root.lookingForBed.Update("LookForAvailableClinic", (Action<BeIncapacitatedChore.StatesInstance, float>) ((smi, dt) => smi.master.FindAvailableMedicalBed(smi.master.GetComponent<Navigator>())), (UpdateRate) 6).Enter("PlayAnim", (StateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State.Callback) (smi =>
      {
        smi.sm.clinic.Set((KMonoBehaviour) null, smi);
        smi.Play(BeIncapacitatedChore.IncapacitatedDuplicantAnim_pre);
        smi.Queue(BeIncapacitatedChore.IncapacitatedDuplicantAnim_loop, (KAnim.PlayMode) 0);
      }));
      this.incapacitation_root.rescue.ToggleChore((Func<BeIncapacitatedChore.StatesInstance, Chore>) (smi => (Chore) new RescueIncapacitatedChore((IStateMachineTarget) smi.master, this.masterTarget.Get(smi))), this.incapacitation_root.recovering, this.incapacitation_root.lookingForBed);
      this.incapacitation_root.rescue.waitingForPickup.EventTransition(GameHashes.OnStore, this.incapacitation_root.rescue.carried).Update("LookForAvailableClinic", (Action<BeIncapacitatedChore.StatesInstance, float>) ((smi, dt) =>
      {
        bool flag = false;
        if (Object.op_Equality((Object) smi.sm.clinic.Get(smi), (Object) null))
          flag = true;
        else if (!smi.master.gameObject.GetComponent<Navigator>().CanReach((IApproachable) this.clinic.Get(smi).GetComponent<Clinic>()))
          flag = true;
        else if (!this.clinic.Get(smi).GetComponent<Assignable>().IsAssignedTo(smi.master.GetComponent<IAssignableIdentity>()))
          flag = true;
        if (!flag)
          return;
        smi.GoTo((StateMachine.BaseState) this.incapacitation_root.lookingForBed);
      }), (UpdateRate) 6);
      this.incapacitation_root.rescue.carried.Update("LookForAvailableClinic", (Action<BeIncapacitatedChore.StatesInstance, float>) ((smi, dt) =>
      {
        bool flag = false;
        if (Object.op_Equality((Object) smi.sm.clinic.Get(smi), (Object) null))
          flag = true;
        else if (!this.clinic.Get(smi).GetComponent<Assignable>().IsAssignedTo(smi.master.GetComponent<IAssignableIdentity>()))
          flag = true;
        if (!flag)
          return;
        smi.GoTo((StateMachine.BaseState) this.incapacitation_root.lookingForBed);
      }), (UpdateRate) 6).Enter((StateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State.Callback) (smi => smi.Queue(BeIncapacitatedChore.IncapacitatedDuplicantAnim_carry, (KAnim.PlayMode) 0))).Exit((StateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State.Callback) (smi => smi.Play(BeIncapacitatedChore.IncapacitatedDuplicantAnim_place)));
      this.incapacitation_root.death.PlayAnim(BeIncapacitatedChore.IncapacitatedDuplicantAnim_death).Enter((StateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State.Callback) (smi =>
      {
        smi.SetStatus(StateMachine.Status.Failed);
        smi.StopSM("died");
      }));
      this.incapacitation_root.recovering.ToggleUrge(Db.Get().Urges.HealCritical).Enter((StateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State.Callback) (smi =>
      {
        smi.Trigger(-1256572400);
        smi.SetStatus(StateMachine.Status.Success);
        smi.StopSM("recovering");
      }));
    }

    public class IncapacitatedStates : 
      GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State
    {
      public GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State lookingForBed;
      public BeIncapacitatedChore.States.BeingRescued rescue;
      public GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State death;
      public GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State recovering;
    }

    public class BeingRescued : 
      GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State
    {
      public GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State waitingForPickup;
      public GameStateMachine<BeIncapacitatedChore.States, BeIncapacitatedChore.StatesInstance, BeIncapacitatedChore, object>.State carried;
    }
  }
}
