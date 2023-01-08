// Decompiled with JetBrains decompiler
// Type: TakeMedicineChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class TakeMedicineChore : Chore<TakeMedicineChore.StatesInstance>
{
  private Pickupable pickupable;
  private MedicinalPillWorkable medicine;
  public static readonly Chore.Precondition CanCure = new Chore.Precondition()
  {
    id = nameof (CanCure),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.CAN_CURE,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => ((TakeMedicineChore) data).medicine.CanBeTakenBy(context.consumerState.gameObject))
  };
  public static readonly Chore.Precondition IsConsumptionPermitted = new Chore.Precondition()
  {
    id = nameof (IsConsumptionPermitted),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_CONSUMPTION_PERMITTED,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      TakeMedicineChore takeMedicineChore = (TakeMedicineChore) data;
      ConsumableConsumer consumableConsumer1 = context.consumerState.consumableConsumer;
      if (Object.op_Equality((Object) consumableConsumer1, (Object) null))
        return true;
      ConsumableConsumer consumableConsumer2 = consumableConsumer1;
      Tag tag = ((Component) takeMedicineChore.medicine).PrefabID();
      string name = ((Tag) ref tag).Name;
      return consumableConsumer2.IsPermitted(name);
    })
  };

  public TakeMedicineChore(MedicinalPillWorkable master)
    : base(Db.Get().ChoreTypes.TakeMedicine, (IStateMachineTarget) master, (ChoreProvider) null, false, master_priority_class: PriorityScreen.PriorityClass.personalNeeds)
  {
    this.medicine = master;
    this.pickupable = ((Component) this.medicine).GetComponent<Pickupable>();
    this.smi = new TakeMedicineChore.StatesInstance(this);
    this.AddPrecondition(ChorePreconditions.instance.CanPickup, (object) this.pickupable);
    this.AddPrecondition(TakeMedicineChore.CanCure, (object) this);
    this.AddPrecondition(TakeMedicineChore.IsConsumptionPermitted, (object) this);
    this.AddPrecondition(ChorePreconditions.instance.IsNotARobot, (object) this);
  }

  public override void Begin(Chore.Precondition.Context context)
  {
    this.smi.sm.source.Set(((Component) this.pickupable).gameObject, this.smi, false);
    double num = (double) this.smi.sm.requestedpillcount.Set(1f, this.smi);
    this.smi.sm.eater.Set(context.consumerState.gameObject, this.smi, false);
    base.Begin(context);
    TakeMedicineChore takeMedicineChore = new TakeMedicineChore(this.medicine);
  }

  public class StatesInstance : 
    GameStateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.GameInstance
  {
    public StatesInstance(TakeMedicineChore master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore>
  {
    public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.TargetParameter eater;
    public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.TargetParameter source;
    public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.TargetParameter chunk;
    public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.FloatParameter requestedpillcount;
    public StateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.FloatParameter actualpillcount;
    public GameStateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.FetchSubState fetch;
    public GameStateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.State takemedicine;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.fetch;
      this.Target(this.eater);
      this.fetch.InitializeStates(this.eater, this.source, this.chunk, this.requestedpillcount, this.actualpillcount, this.takemedicine);
      this.takemedicine.ToggleAnims("anim_eat_floor_kanim").ToggleTag(GameTags.TakingMedicine).ToggleWork("TakeMedicine", (Action<TakeMedicineChore.StatesInstance>) (smi =>
      {
        MedicinalPillWorkable medicinalPillWorkable = this.chunk.Get<MedicinalPillWorkable>(smi);
        this.eater.Get<Worker>(smi).StartWork(new Worker.StartWorkInfo((Workable) medicinalPillWorkable));
      }), (Func<TakeMedicineChore.StatesInstance, bool>) (smi => Object.op_Inequality((Object) this.chunk.Get<MedicinalPill>(smi), (Object) null)), (GameStateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.State) null, (GameStateMachine<TakeMedicineChore.States, TakeMedicineChore.StatesInstance, TakeMedicineChore, object>.State) null);
    }
  }
}
