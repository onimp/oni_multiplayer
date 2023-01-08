// Decompiled with JetBrains decompiler
// Type: EarlyBird
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class EarlyBird : StateMachineComponent<EarlyBird.StatesInstance>
{
  [MyCmpReq]
  private KPrefabID kPrefabID;
  private AttributeModifier[] attributeModifiers;

  protected virtual void OnSpawn()
  {
    this.attributeModifiers = new AttributeModifier[11]
    {
      new AttributeModifier("Construction", TUNING.TRAITS.EARLYBIRD_MODIFIER, (string) DUPLICANTS.TRAITS.EARLYBIRD.NAME),
      new AttributeModifier("Digging", TUNING.TRAITS.EARLYBIRD_MODIFIER, (string) DUPLICANTS.TRAITS.EARLYBIRD.NAME),
      new AttributeModifier("Machinery", TUNING.TRAITS.EARLYBIRD_MODIFIER, (string) DUPLICANTS.TRAITS.EARLYBIRD.NAME),
      new AttributeModifier("Athletics", TUNING.TRAITS.EARLYBIRD_MODIFIER, (string) DUPLICANTS.TRAITS.EARLYBIRD.NAME),
      new AttributeModifier("Learning", TUNING.TRAITS.EARLYBIRD_MODIFIER, (string) DUPLICANTS.TRAITS.EARLYBIRD.NAME),
      new AttributeModifier("Cooking", TUNING.TRAITS.EARLYBIRD_MODIFIER, (string) DUPLICANTS.TRAITS.EARLYBIRD.NAME),
      new AttributeModifier("Art", TUNING.TRAITS.EARLYBIRD_MODIFIER, (string) DUPLICANTS.TRAITS.EARLYBIRD.NAME),
      new AttributeModifier("Strength", TUNING.TRAITS.EARLYBIRD_MODIFIER, (string) DUPLICANTS.TRAITS.EARLYBIRD.NAME),
      new AttributeModifier("Caring", TUNING.TRAITS.EARLYBIRD_MODIFIER, (string) DUPLICANTS.TRAITS.EARLYBIRD.NAME),
      new AttributeModifier("Botanist", TUNING.TRAITS.EARLYBIRD_MODIFIER, (string) DUPLICANTS.TRAITS.EARLYBIRD.NAME),
      new AttributeModifier("Ranching", TUNING.TRAITS.EARLYBIRD_MODIFIER, (string) DUPLICANTS.TRAITS.EARLYBIRD.NAME)
    };
    this.smi.StartSM();
  }

  public void ApplyModifiers()
  {
    Attributes attributes = ((Component) this).gameObject.GetAttributes();
    for (int index = 0; index < this.attributeModifiers.Length; ++index)
    {
      AttributeModifier attributeModifier = this.attributeModifiers[index];
      attributes.Add(attributeModifier);
    }
  }

  public void RemoveModifiers()
  {
    Attributes attributes = ((Component) this).gameObject.GetAttributes();
    for (int index = 0; index < this.attributeModifiers.Length; ++index)
    {
      AttributeModifier attributeModifier = this.attributeModifiers[index];
      attributes.Remove(attributeModifier);
    }
  }

  public class StatesInstance : 
    GameStateMachine<EarlyBird.States, EarlyBird.StatesInstance, EarlyBird, object>.GameInstance
  {
    public StatesInstance(EarlyBird master)
      : base(master)
    {
    }

    public bool IsMorning() => !Object.op_Equality((Object) ScheduleManager.Instance, (Object) null) && !Tag.op_Equality(this.master.kPrefabID.PrefabTag, GameTags.MinionSelectPreview) && Schedule.GetBlockIdx() < TUNING.TRAITS.EARLYBIRD_SCHEDULEBLOCK;
  }

  public class States : GameStateMachine<EarlyBird.States, EarlyBird.StatesInstance, EarlyBird>
  {
    public GameStateMachine<EarlyBird.States, EarlyBird.StatesInstance, EarlyBird, object>.State idle;
    public GameStateMachine<EarlyBird.States, EarlyBird.StatesInstance, EarlyBird, object>.State early;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.root.TagTransition(GameTags.Dead, (GameStateMachine<EarlyBird.States, EarlyBird.StatesInstance, EarlyBird, object>.State) null);
      this.idle.Transition(this.early, (StateMachine<EarlyBird.States, EarlyBird.StatesInstance, EarlyBird, object>.Transition.ConditionCallback) (smi => smi.IsMorning()));
      this.early.Enter("Morning", (StateMachine<EarlyBird.States, EarlyBird.StatesInstance, EarlyBird, object>.State.Callback) (smi => smi.master.ApplyModifiers())).Exit("NotMorning", (StateMachine<EarlyBird.States, EarlyBird.StatesInstance, EarlyBird, object>.State.Callback) (smi => smi.master.RemoveModifiers())).ToggleStatusItem(Db.Get().DuplicantStatusItems.EarlyMorning).ToggleExpression(Db.Get().Expressions.Happy).Transition(this.idle, (StateMachine<EarlyBird.States, EarlyBird.StatesInstance, EarlyBird, object>.Transition.ConditionCallback) (smi => !smi.IsMorning()));
    }
  }
}
