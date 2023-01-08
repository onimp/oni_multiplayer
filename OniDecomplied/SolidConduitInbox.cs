// Decompiled with JetBrains decompiler
// Type: SolidConduitInbox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;

[SerializationConfig]
public class SolidConduitInbox : StateMachineComponent<SolidConduitInbox.SMInstance>, ISim1000ms
{
  [MyCmpReq]
  private Operational operational;
  [MyCmpReq]
  private SolidConduitDispenser dispenser;
  [MyCmpAdd]
  private Storage storage;
  private FilteredStorage filteredStorage;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.filteredStorage = new FilteredStorage((KMonoBehaviour) this, (Tag[]) null, (IUserControlledCapacity) null, false, Db.Get().ChoreTypes.StorageFetch);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.filteredStorage.FilterChanged();
    this.smi.StartSM();
  }

  protected override void OnCleanUp() => base.OnCleanUp();

  public void Sim1000ms(float dt)
  {
    if (this.operational.IsOperational && this.dispenser.IsDispensing)
      this.operational.SetActive(true);
    else
      this.operational.SetActive(false);
  }

  public class SMInstance : 
    GameStateMachine<SolidConduitInbox.States, SolidConduitInbox.SMInstance, SolidConduitInbox, object>.GameInstance
  {
    public SMInstance(SolidConduitInbox master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<SolidConduitInbox.States, SolidConduitInbox.SMInstance, SolidConduitInbox>
  {
    public GameStateMachine<SolidConduitInbox.States, SolidConduitInbox.SMInstance, SolidConduitInbox, object>.State off;
    public SolidConduitInbox.States.ReadyStates on;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.off;
      this.root.DoNothing();
      this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, (GameStateMachine<SolidConduitInbox.States, SolidConduitInbox.SMInstance, SolidConduitInbox, object>.State) this.on, (StateMachine<SolidConduitInbox.States, SolidConduitInbox.SMInstance, SolidConduitInbox, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational));
      this.on.DefaultState(this.on.idle).EventTransition(GameHashes.OperationalChanged, this.off, (StateMachine<SolidConduitInbox.States, SolidConduitInbox.SMInstance, SolidConduitInbox, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational));
      this.on.idle.PlayAnim("on").EventTransition(GameHashes.ActiveChanged, this.on.working, (StateMachine<SolidConduitInbox.States, SolidConduitInbox.SMInstance, SolidConduitInbox, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsActive));
      this.on.working.PlayAnim("working_pre").QueueAnim("working_loop", true).EventTransition(GameHashes.ActiveChanged, this.on.post, (StateMachine<SolidConduitInbox.States, SolidConduitInbox.SMInstance, SolidConduitInbox, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsActive));
      this.on.post.PlayAnim("working_pst").OnAnimQueueComplete((GameStateMachine<SolidConduitInbox.States, SolidConduitInbox.SMInstance, SolidConduitInbox, object>.State) this.on);
    }

    public class ReadyStates : 
      GameStateMachine<SolidConduitInbox.States, SolidConduitInbox.SMInstance, SolidConduitInbox, object>.State
    {
      public GameStateMachine<SolidConduitInbox.States, SolidConduitInbox.SMInstance, SolidConduitInbox, object>.State idle;
      public GameStateMachine<SolidConduitInbox.States, SolidConduitInbox.SMInstance, SolidConduitInbox, object>.State working;
      public GameStateMachine<SolidConduitInbox.States, SolidConduitInbox.SMInstance, SolidConduitInbox, object>.State post;
    }
  }
}
