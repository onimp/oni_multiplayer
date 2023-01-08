// Decompiled with JetBrains decompiler
// Type: ObjectDispenser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDispenser : Switch, IUserControlledCapacity
{
  public static readonly HashedString PORT_ID = HashedString.op_Implicit(nameof (ObjectDispenser));
  private LoggerFS log;
  public CellOffset dropOffset;
  [MyCmpReq]
  private Building building;
  [MyCmpReq]
  private Storage storage;
  [MyCmpGet]
  private Rotatable rotatable;
  private ObjectDispenser.Instance smi;
  private static StatusItem infoStatusItem;
  [Serialize]
  private float userMaxCapacity = float.PositiveInfinity;
  protected FilteredStorage filteredStorage;
  private static readonly EventSystem.IntraObjectHandler<ObjectDispenser> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ObjectDispenser>((Action<ObjectDispenser, object>) ((component, data) => component.OnCopySettings(data)));

  public virtual float UserMaxCapacity
  {
    get => Mathf.Min(this.userMaxCapacity, ((Component) this).GetComponent<Storage>().capacityKg);
    set
    {
      this.userMaxCapacity = value;
      this.filteredStorage.FilterChanged();
    }
  }

  public float AmountStored => ((Component) this).GetComponent<Storage>().MassStored();

  public float MinCapacity => 0.0f;

  public float MaxCapacity => ((Component) this).GetComponent<Storage>().capacityKg;

  public bool WholeValues => false;

  public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

  protected override void OnPrefabInit() => this.Initialize();

  protected void Initialize()
  {
    base.OnPrefabInit();
    this.log = new LoggerFS(nameof (ObjectDispenser), 35);
    this.filteredStorage = new FilteredStorage((KMonoBehaviour) this, (Tag[]) null, (IUserControlledCapacity) this, false, Db.Get().ChoreTypes.StorageFetch);
    this.Subscribe<ObjectDispenser>(-905833192, ObjectDispenser.OnCopySettingsDelegate);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.smi = new ObjectDispenser.Instance(this, this.IsSwitchedOn);
    this.smi.StartSM();
    if (ObjectDispenser.infoStatusItem == null)
    {
      ObjectDispenser.infoStatusItem = new StatusItem("ObjectDispenserAutomationInfo", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      ObjectDispenser.infoStatusItem.resolveStringCallback = new Func<string, object, string>(ObjectDispenser.ResolveInfoStatusItemString);
    }
    this.filteredStorage.FilterChanged();
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(ObjectDispenser.infoStatusItem, true, (object) this.smi);
  }

  protected virtual void OnCleanUp()
  {
    this.filteredStorage.CleanUp();
    base.OnCleanUp();
  }

  private void OnCopySettings(object data)
  {
    GameObject gameObject = (GameObject) data;
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return;
    ObjectDispenser component = gameObject.GetComponent<ObjectDispenser>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    this.UserMaxCapacity = component.UserMaxCapacity;
  }

  public void DropHeldItems()
  {
    while (this.storage.Count > 0)
    {
      GameObject gameObject = this.storage.Drop(this.storage.items[0], true);
      if (Object.op_Inequality((Object) this.rotatable, (Object) null))
      {
        Transform transform = gameObject.transform;
        Vector3 position = TransformExtensions.GetPosition(this.transform);
        CellOffset rotatedCellOffset = this.rotatable.GetRotatedCellOffset(this.dropOffset);
        Vector3 vector3_1 = ((CellOffset) ref rotatedCellOffset).ToVector3();
        Vector3 vector3_2 = Vector3.op_Addition(position, vector3_1);
        TransformExtensions.SetPosition(transform, vector3_2);
      }
      else
        TransformExtensions.SetPosition(gameObject.transform, Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), ((CellOffset) ref this.dropOffset).ToVector3()));
    }
    this.smi.GetMaster().GetComponent<Storage>().DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
  }

  protected override void Toggle() => base.Toggle();

  protected override void OnRefreshUserMenu(object data)
  {
    if (this.smi.IsAutomated())
      return;
    base.OnRefreshUserMenu(data);
  }

  private static string ResolveInfoStatusItemString(string format_str, object data)
  {
    ObjectDispenser.Instance instance = (ObjectDispenser.Instance) data;
    return string.Format((string) (instance.IsAutomated() ? BUILDING.STATUSITEMS.OBJECTDISPENSER.AUTOMATION_CONTROL : BUILDING.STATUSITEMS.OBJECTDISPENSER.MANUAL_CONTROL), (object) (string) (instance.IsOpened ? BUILDING.STATUSITEMS.OBJECTDISPENSER.OPENED : BUILDING.STATUSITEMS.OBJECTDISPENSER.CLOSED));
  }

  public class States : 
    GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser>
  {
    public GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.State load_item;
    public GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.State load_item_pst;
    public GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.State drop_item;
    public GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.State idle;
    public StateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.BoolParameter should_open;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.idle.PlayAnim("on").EventHandler(GameHashes.OnStorageChange, (StateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.State.Callback) (smi => smi.UpdateState())).ParamTransition<bool>((StateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.Parameter<bool>) this.should_open, this.drop_item, (StateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.Parameter<bool>.Callback) ((smi, p) => p && !((Component) smi.master).GetComponent<Storage>().IsEmpty()));
      this.load_item.PlayAnim("working_load").OnAnimQueueComplete(this.load_item_pst);
      this.load_item_pst.ParamTransition<bool>((StateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.Parameter<bool>) this.should_open, this.idle, (StateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.Parameter<bool>.Callback) ((smi, p) => !p)).ParamTransition<bool>((StateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.Parameter<bool>) this.should_open, this.drop_item, (StateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.Parameter<bool>.Callback) ((smi, p) => p));
      this.drop_item.PlayAnim("working_dispense").OnAnimQueueComplete(this.idle).Exit((StateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.State.Callback) (smi => smi.master.DropHeldItems()));
    }
  }

  public class Instance : 
    GameStateMachine<ObjectDispenser.States, ObjectDispenser.Instance, ObjectDispenser, object>.GameInstance
  {
    private Operational operational;
    public LogicPorts logic;
    public bool logic_on = true;
    private bool manual_on;

    public Instance(ObjectDispenser master, bool manual_start_state)
      : base(master)
    {
      this.manual_on = manual_start_state;
      this.operational = this.GetComponent<Operational>();
      this.logic = this.GetComponent<LogicPorts>();
      this.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
      this.Subscribe(-801688580, new Action<object>(this.OnLogicValueChanged));
      this.smi.sm.should_open.Set(true, this.smi);
    }

    public void UpdateState() => this.smi.GoTo((StateMachine.BaseState) this.sm.load_item);

    public bool IsAutomated() => this.logic.IsPortConnected(ObjectDispenser.PORT_ID);

    public bool IsOpened => !this.IsAutomated() ? this.manual_on : this.logic_on;

    public void SetSwitchState(bool on)
    {
      this.manual_on = on;
      this.UpdateShouldOpen();
    }

    public void SetActive(bool active) => this.operational.SetActive(active);

    private void OnOperationalChanged(object data) => this.UpdateShouldOpen();

    private void OnLogicValueChanged(object data)
    {
      LogicValueChanged logicValueChanged = (LogicValueChanged) data;
      if (HashedString.op_Inequality(logicValueChanged.portID, ObjectDispenser.PORT_ID))
        return;
      this.logic_on = LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue);
      this.UpdateShouldOpen();
    }

    private void UpdateShouldOpen()
    {
      this.SetActive(this.operational.IsOperational);
      if (!this.operational.IsOperational)
        return;
      if (this.IsAutomated())
        this.smi.sm.should_open.Set(this.logic_on, this.smi);
      else
        this.smi.sm.should_open.Set(this.manual_on, this.smi);
    }
  }
}
