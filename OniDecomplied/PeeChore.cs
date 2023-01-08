// Decompiled with JetBrains decompiler
// Type: PeeChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PeeChore : Chore<PeeChore.StatesInstance>
{
  public PeeChore(IStateMachineTarget target)
    : base(Db.Get().ChoreTypes.Pee, target, target.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.compulsory)
  {
    this.smi = new PeeChore.StatesInstance(this, target.gameObject);
  }

  public class StatesInstance : 
    GameStateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore, object>.GameInstance
  {
    public Notification stressfullyEmptyingBladder = new Notification((string) DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_NAME, NotificationType.Bad, (Func<List<Notification>, object, string>) ((notificationList, data) => (string) DUPLICANTS.STATUSITEMS.STRESSFULLYEMPTYINGBLADDER.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false)));
    public AmountInstance bladder;
    private AmountInstance bodyTemperature;

    public StatesInstance(PeeChore master, GameObject worker)
      : base(master)
    {
      this.bladder = Db.Get().Amounts.Bladder.Lookup(worker);
      this.bodyTemperature = Db.Get().Amounts.Temperature.Lookup(worker);
      this.sm.worker.Set(worker, this.smi, false);
    }

    public bool IsDonePeeing() => (double) this.bladder.value <= 0.0;

    public void SpawnDirtyWater(float dt)
    {
      int cell = Grid.PosToCell(this.sm.worker.Get<KMonoBehaviour>(this.smi));
      byte index = Db.Get().Diseases.GetIndex(HashedString.op_Implicit("FoodPoisoning"));
      float num = dt * -this.bladder.GetDelta() / this.bladder.GetMax();
      if ((double) num <= 0.0)
        return;
      float mass = 2f * num;
      Equippable equippable = this.GetComponent<SuitEquipper>().IsWearingAirtightSuit();
      if (Object.op_Inequality((Object) equippable, (Object) null))
        ((Component) equippable).GetComponent<Storage>().AddLiquid(SimHashes.DirtyWater, mass, this.bodyTemperature.value, index, Mathf.CeilToInt(100000f * num));
      else
        SimMessages.AddRemoveSubstance(cell, SimHashes.DirtyWater, CellEventLogger.Instance.Vomit, mass, this.bodyTemperature.value, index, Mathf.CeilToInt(100000f * num));
    }
  }

  public class States : GameStateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore>
  {
    public StateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore, object>.TargetParameter worker;
    public GameStateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore, object>.State running;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.running;
      this.Target(this.worker);
      this.running.ToggleAnims("anim_expel_kanim").ToggleEffect("StressfulyEmptyingBladder").DoNotification((Func<PeeChore.StatesInstance, Notification>) (smi => smi.stressfullyEmptyingBladder)).DoReport(ReportManager.ReportType.ToiletIncident, (Func<PeeChore.StatesInstance, float>) (smi => 1f), (Func<PeeChore.StatesInstance, string>) (smi => this.masterTarget.Get(smi).GetProperName())).DoTutorial(Tutorial.TutorialMessages.TM_Mopping).Transition((GameStateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore, object>.State) null, (StateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore, object>.Transition.ConditionCallback) (smi => smi.IsDonePeeing())).Update("SpawnDirtyWater", (Action<PeeChore.StatesInstance, float>) ((smi, dt) => smi.SpawnDirtyWater(dt))).PlayAnim("working_loop", (KAnim.PlayMode) 0).ToggleTag(GameTags.MakingMess).Enter((StateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore, object>.State.Callback) (smi =>
      {
        if (!Sim.IsRadiationEnabled() || (double) smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).value <= 0.0)
          return;
        smi.master.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads);
      })).Exit((StateMachine<PeeChore.States, PeeChore.StatesInstance, PeeChore, object>.State.Callback) (smi =>
      {
        if (!Sim.IsRadiationEnabled())
          return;
        smi.master.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads);
        AmountInstance amountInstance = smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance.Id);
        RadiationMonitor.Instance smi1 = smi.master.gameObject.GetSMI<RadiationMonitor.Instance>();
        if (smi1 == null)
          return;
        float num1 = Math.Min(amountInstance.value, 100f * smi1.difficultySettingMod);
        double num2 = (double) smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.RadiationBalance.Id).ApplyDelta(-num1);
        if ((double) num1 < 1.0)
          return;
        PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, Mathf.FloorToInt(num1).ToString() + (string) UI.UNITSUFFIXES.RADIATION.RADS, smi.master.transform);
      }));
    }
  }
}
