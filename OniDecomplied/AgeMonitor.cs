// Decompiled with JetBrains decompiler
// Type: AgeMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using UnityEngine;

public class AgeMonitor : 
  GameStateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>
{
  public GameStateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>.State alive;
  public GameStateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>.State time_to_die;
  private AttributeModifier aging;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.alive;
    this.alive.ToggleAttributeModifier("Aging", (Func<AgeMonitor.Instance, AttributeModifier>) (smi => this.aging)).Transition(this.time_to_die, new StateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>.Transition.ConditionCallback(AgeMonitor.TimeToDie), (UpdateRate) 6).Update(new System.Action<AgeMonitor.Instance, float>(AgeMonitor.UpdateOldStatusItem), (UpdateRate) 6);
    this.time_to_die.Enter(new StateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>.State.Callback(AgeMonitor.Die));
    this.aging = new AttributeModifier(Db.Get().Amounts.Age.deltaAttribute.Id, 1f / 600f, (string) CREATURES.MODIFIERS.AGE.NAME);
  }

  private static void Die(AgeMonitor.Instance smi) => smi.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Generic);

  private static bool TimeToDie(AgeMonitor.Instance smi) => (double) smi.age.value >= (double) smi.age.GetMax();

  private static void UpdateOldStatusItem(AgeMonitor.Instance smi, float dt)
  {
    KSelectable component = smi.GetComponent<KSelectable>();
    bool show = (double) smi.age.value > (double) smi.age.GetMax() * 0.89999997615814209;
    smi.oldStatusGuid = component.ToggleStatusItem(Db.Get().CreatureStatusItems.Old, smi.oldStatusGuid, show, (object) smi);
  }

  public class Def : StateMachine.BaseDef
  {
    public float maxAgePercentOnSpawn = 0.75f;

    public override void Configure(GameObject prefab) => prefab.AddOrGet<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Age.Id);
  }

  public new class Instance : 
    GameStateMachine<AgeMonitor, AgeMonitor.Instance, IStateMachineTarget, AgeMonitor.Def>.GameInstance
  {
    public AmountInstance age;
    public Guid oldStatusGuid;

    public Instance(IStateMachineTarget master, AgeMonitor.Def def)
      : base(master, def)
    {
      this.age = Db.Get().Amounts.Age.Lookup(this.gameObject);
      this.Subscribe(1119167081, (System.Action<object>) (data => this.RandomizeAge()));
    }

    public void RandomizeAge()
    {
      this.age.value = Random.value * this.age.GetMax() * this.def.maxAgePercentOnSpawn;
      AmountInstance amountInstance = Db.Get().Amounts.Fertility.Lookup(this.gameObject);
      if (amountInstance == null)
        return;
      amountInstance.value = (float) ((double) this.age.value / (double) this.age.GetMax() * (double) amountInstance.GetMax() * 1.75);
      amountInstance.value = Mathf.Min(amountInstance.value, amountInstance.GetMax() * 0.9f);
    }

    public float CyclesUntilDeath => this.age.GetMax() - this.age.value;
  }
}
