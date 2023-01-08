// Decompiled with JetBrains decompiler
// Type: DeliverFoodChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class DeliverFoodChore : Chore<DeliverFoodChore.StatesInstance>
{
  public DeliverFoodChore(IStateMachineTarget target)
    : base(Db.Get().ChoreTypes.DeliverFood, target, target.GetComponent<ChoreProvider>(), false)
  {
    this.smi = new DeliverFoodChore.StatesInstance(this);
    this.AddPrecondition(ChorePreconditions.instance.IsChattable, (object) target);
  }

  public override void Begin(Chore.Precondition.Context context)
  {
    double num = (double) this.smi.sm.requestedrationcount.Set(this.smi.GetComponent<StateMachineController>().GetSMI<RationMonitor.Instance>().GetRationsRemaining(), this.smi);
    this.smi.sm.ediblesource.Set((KMonoBehaviour) context.consumerState.gameObject.GetComponent<Sensors>().GetSensor<ClosestEdibleSensor>().GetEdible(), this.smi);
    this.smi.sm.deliverypoint.Set(this.gameObject, this.smi, false);
    this.smi.sm.deliverer.Set(context.consumerState.gameObject, this.smi, false);
    base.Begin(context);
  }

  public class StatesInstance : 
    GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.GameInstance
  {
    public StatesInstance(DeliverFoodChore master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore>
  {
    public StateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.TargetParameter deliverer;
    public StateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.TargetParameter ediblesource;
    public StateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.TargetParameter ediblechunk;
    public StateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.TargetParameter deliverypoint;
    public StateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.FloatParameter requestedrationcount;
    public StateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.FloatParameter actualrationcount;
    public GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.FetchSubState fetch;
    public GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.ApproachSubState<Chattable> movetodeliverypoint;
    public GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.DropSubState drop;
    public GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.State success;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.fetch;
      this.fetch.InitializeStates(this.deliverer, this.ediblesource, this.ediblechunk, this.requestedrationcount, this.actualrationcount, (GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.State) this.movetodeliverypoint);
      this.movetodeliverypoint.InitializeStates(this.deliverer, this.deliverypoint, (GameStateMachine<DeliverFoodChore.States, DeliverFoodChore.StatesInstance, DeliverFoodChore, object>.State) this.drop);
      this.drop.InitializeStates(this.deliverer, this.ediblechunk, this.deliverypoint, this.success);
      this.success.ReturnSuccess();
    }
  }
}
