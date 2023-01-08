// Decompiled with JetBrains decompiler
// Type: BabyMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class BabyMonitor : 
  GameStateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>
{
  public GameStateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>.State baby;
  public GameStateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>.State spawnadult;
  public Effect babyEffect;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.baby;
    this.root.Enter(new StateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>.State.Callback(BabyMonitor.AddBabyEffect));
    this.baby.Transition(this.spawnadult, new StateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>.Transition.ConditionCallback(BabyMonitor.IsReadyToSpawnAdult), (UpdateRate) 7);
    this.spawnadult.ToggleBehaviour(GameTags.Creatures.Behaviours.GrowUpBehaviour, (StateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>.Transition.ConditionCallback) (smi => true));
    this.babyEffect = new Effect("IsABaby", (string) CREATURES.MODIFIERS.BABY.NAME, (string) CREATURES.MODIFIERS.BABY.TOOLTIP, 0.0f, true, false, false);
    this.babyEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, -0.9f, (string) CREATURES.MODIFIERS.BABY.NAME, true));
    this.babyEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 5f, (string) CREATURES.MODIFIERS.BABY.NAME));
  }

  private static void AddBabyEffect(BabyMonitor.Instance smi) => smi.Get<Effects>().Add(smi.sm.babyEffect, false);

  private static bool IsReadyToSpawnAdult(BabyMonitor.Instance smi)
  {
    AmountInstance amountInstance = Db.Get().Amounts.Age.Lookup(smi.gameObject);
    float num = smi.def.adultThreshold;
    if (GenericGameSettings.instance.acceleratedLifecycle)
      num = 0.005f;
    return (double) amountInstance.value > (double) num;
  }

  public class Def : StateMachine.BaseDef
  {
    public Tag adultPrefab;
    public string onGrowDropID;
    public bool forceAdultNavType;
    public float adultThreshold = 5f;
  }

  public new class Instance : 
    GameStateMachine<BabyMonitor, BabyMonitor.Instance, IStateMachineTarget, BabyMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, BabyMonitor.Def def)
      : base(master, def)
    {
    }

    public void SpawnAdult()
    {
      Vector3 position = TransformExtensions.GetPosition(this.smi.transform);
      position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
      GameObject go = Util.KInstantiate(Assets.GetPrefab(this.smi.def.adultPrefab), position);
      go.SetActive(true);
      go.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim(HashedString.op_Implicit("growup_pst"));
      if (this.smi.def.onGrowDropID != null)
        Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(this.smi.def.onGrowDropID)), position).SetActive(true);
      foreach (AmountInstance amount in (Modifications<Amount, AmountInstance>) this.gameObject.GetAmounts())
      {
        AmountInstance amountInstance = amount.amount.Lookup(go);
        if (amountInstance != null)
        {
          float num = amount.value / amount.GetMax();
          amountInstance.value = num * amountInstance.GetMax();
        }
      }
      if (!this.smi.def.forceAdultNavType)
      {
        Navigator component = this.smi.GetComponent<Navigator>();
        go.GetComponent<Navigator>().SetCurrentNavType(component.CurrentNavType);
      }
      EventExtensions.Trigger(go, -2027483228, (object) this.gameObject);
      KSelectable component1 = this.gameObject.GetComponent<KSelectable>();
      if (Object.op_Inequality((Object) SelectTool.Instance, (Object) null) && Object.op_Inequality((Object) SelectTool.Instance.selected, (Object) null) && Object.op_Equality((Object) SelectTool.Instance.selected, (Object) component1))
        SelectTool.Instance.Select(go.GetComponent<KSelectable>());
      TracesExtesions.DeleteObject(this.smi.gameObject);
    }
  }
}
