// Decompiled with JetBrains decompiler
// Type: Desalinator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class Desalinator : StateMachineComponent<Desalinator.StatesInstance>
{
  [MyCmpGet]
  private Operational operational;
  private ManualDeliveryKG[] deliveryComponents;
  [MyCmpReq]
  private Storage storage;
  [Serialize]
  public float maxSalt = 1000f;
  [Serialize]
  private float _storageLeft = 1000f;
  private ElementConverter[] converters;
  private static readonly EventSystem.IntraObjectHandler<Desalinator> OnConduitConnectionChangedDelegate = new EventSystem.IntraObjectHandler<Desalinator>((Action<Desalinator, object>) ((component, data) => component.OnConduitConnectionChanged(data)));

  public float SaltStorageLeft
  {
    get => this._storageLeft;
    set
    {
      this._storageLeft = value;
      double num = (double) this.smi.sm.saltStorageLeft.Set(value, this.smi);
    }
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.deliveryComponents = ((Component) this).GetComponents<ManualDeliveryKG>();
    this.OnConduitConnectionChanged((object) ((Component) this).GetComponent<ConduitConsumer>().IsConnected);
    this.Subscribe<Desalinator>(-2094018600, Desalinator.OnConduitConnectionChangedDelegate);
    this.smi.StartSM();
  }

  private void OnConduitConnectionChanged(object data)
  {
    bool pause = (bool) data;
    foreach (ManualDeliveryKG deliveryComponent in this.deliveryComponents)
    {
      Element element = ElementLoader.GetElement(deliveryComponent.RequestedItemTag);
      if (element != null && element.IsLiquid)
        deliveryComponent.Pause(pause, "pipe connected");
    }
  }

  private void OnRefreshUserMenu(object data)
  {
    if (this.smi.GetCurrentState() == this.smi.sm.full || !this.smi.HasSalt || this.smi.emptyChore != null)
      return;
    Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("status_item_desalinator_needs_emptying", (string) UI.USERMENUACTIONS.EMPTYDESALINATOR.NAME, (System.Action) (() => this.smi.GoTo((StateMachine.BaseState) this.smi.sm.earlyEmpty)), tooltipText: ((string) UI.USERMENUACTIONS.CLEANTOILET.TOOLTIP)));
  }

  private bool CheckCanConvert()
  {
    if (this.converters == null)
      this.converters = ((Component) this).GetComponents<ElementConverter>();
    for (int index = 0; index < this.converters.Length; ++index)
    {
      if (this.converters[index].CanConvertAtAll())
        return true;
    }
    return false;
  }

  private bool CheckEnoughMassToConvert()
  {
    if (this.converters == null)
      this.converters = ((Component) this).GetComponents<ElementConverter>();
    for (int index = 0; index < this.converters.Length; ++index)
    {
      if (this.converters[index].HasEnoughMassToStartConverting())
        return true;
    }
    return false;
  }

  public class StatesInstance : 
    GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.GameInstance
  {
    public Chore emptyChore;

    public StatesInstance(Desalinator smi)
      : base(smi)
    {
    }

    public bool HasSalt => this.master.storage.Has(ElementLoader.FindElementByHash(SimHashes.Salt).tag);

    public bool IsFull() => (double) this.master.SaltStorageLeft <= 0.0;

    public bool IsSaltRemoved() => !this.HasSalt;

    public void CreateEmptyChore()
    {
      if (this.emptyChore != null)
        this.emptyChore.Cancel("dupe");
      DesalinatorWorkableEmpty component = ((Component) this.master).GetComponent<DesalinatorWorkableEmpty>();
      this.emptyChore = (Chore) new WorkChore<DesalinatorWorkableEmpty>(Db.Get().ChoreTypes.EmptyDesalinator, (IStateMachineTarget) component, on_complete: new Action<Chore>(this.OnEmptyComplete), ignore_building_assignment: true);
    }

    public void CancelEmptyChore()
    {
      if (this.emptyChore == null)
        return;
      this.emptyChore.Cancel("Cancelled");
      this.emptyChore = (Chore) null;
    }

    private void OnEmptyComplete(Chore chore)
    {
      this.emptyChore = (Chore) null;
      Tag tag = GameTagExtensions.Create(SimHashes.Salt);
      ListPool<GameObject, Desalinator>.PooledList result = ListPool<GameObject, Desalinator>.Allocate();
      this.master.storage.Find(tag, (List<GameObject>) result);
      foreach (GameObject go in (List<GameObject>) result)
        this.master.storage.Drop(go, true);
      result.Recycle();
    }

    public void UpdateStorageLeft() => this.master.SaltStorageLeft = this.master.maxSalt - this.master.storage.GetMassAvailable(GameTagExtensions.Create(SimHashes.Salt));
  }

  public class States : GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator>
  {
    public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State off;
    public Desalinator.States.OnStates on;
    public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State full;
    public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State fullWaitingForEmpty;
    public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State earlyEmpty;
    public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State earlyWaitingForEmpty;
    public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State empty;
    private static readonly HashedString[] FULL_ANIMS = new HashedString[2]
    {
      HashedString.op_Implicit("working_pst"),
      HashedString.op_Implicit(nameof (off))
    };
    public StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.FloatParameter saltStorageLeft = new StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.FloatParameter(0.0f);

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.off;
      this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, (GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State) this.on, (StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.Transition.ConditionCallback) (smi => smi.master.operational.IsOperational));
      this.on.PlayAnim("on").EventTransition(GameHashes.OperationalChanged, this.off, (StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.Transition.ConditionCallback) (smi => !smi.master.operational.IsOperational)).DefaultState(this.on.waiting);
      this.on.waiting.EventTransition(GameHashes.OnStorageChange, this.on.working_pre, (StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.Transition.ConditionCallback) (smi => smi.master.CheckEnoughMassToConvert()));
      this.on.working_pre.PlayAnim("working_pre").OnAnimQueueComplete(this.on.working);
      this.on.working.Enter((StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State.Callback) (smi => smi.master.operational.SetActive(true))).QueueAnim("working_loop", true).EventTransition(GameHashes.OnStorageChange, this.on.working_pst, (StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.Transition.ConditionCallback) (smi => !smi.master.CheckCanConvert())).ParamTransition<float>((StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.Parameter<float>) this.saltStorageLeft, this.full, (StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.Parameter<float>.Callback) ((smi, p) => smi.IsFull())).EventHandler(GameHashes.OnStorageChange, (StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State.Callback) (smi => smi.UpdateStorageLeft())).Exit((StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State.Callback) (smi => smi.master.operational.SetActive(false)));
      this.on.working_pst.PlayAnim("working_pst").OnAnimQueueComplete(this.on.waiting);
      this.earlyEmpty.PlayAnims((Func<Desalinator.StatesInstance, HashedString[]>) (smi => Desalinator.States.FULL_ANIMS)).OnAnimQueueComplete(this.earlyWaitingForEmpty);
      this.earlyWaitingForEmpty.Enter((StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State.Callback) (smi => smi.CreateEmptyChore())).Exit((StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State.Callback) (smi => smi.CancelEmptyChore())).EventTransition(GameHashes.OnStorageChange, this.empty, (StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.Transition.ConditionCallback) (smi => smi.IsSaltRemoved()));
      this.full.PlayAnims((Func<Desalinator.StatesInstance, HashedString[]>) (smi => Desalinator.States.FULL_ANIMS)).OnAnimQueueComplete(this.fullWaitingForEmpty);
      this.fullWaitingForEmpty.Enter((StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State.Callback) (smi => smi.CreateEmptyChore())).Exit((StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State.Callback) (smi => smi.CancelEmptyChore())).ToggleMainStatusItem(Db.Get().BuildingStatusItems.DesalinatorNeedsEmptying).EventTransition(GameHashes.OnStorageChange, this.empty, (StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.Transition.ConditionCallback) (smi => smi.IsSaltRemoved()));
      this.empty.PlayAnim("off").Enter("ResetStorage", (StateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State.Callback) (smi => smi.master.SaltStorageLeft = smi.master.maxSalt)).GoTo(this.on.waiting);
    }

    public class OnStates : 
      GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State
    {
      public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State waiting;
      public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State working_pre;
      public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State working;
      public GameStateMachine<Desalinator.States, Desalinator.StatesInstance, Desalinator, object>.State working_pst;
    }
  }
}
