// Decompiled with JetBrains decompiler
// Type: Door
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/Workable/Door")]
public class Door : Workable, ISaveLoadable, ISim200ms, INavDoor
{
  [MyCmpReq]
  private Operational operational;
  [MyCmpGet]
  private Rotatable rotatable;
  [MyCmpReq]
  private KBatchedAnimController animController;
  [MyCmpReq]
  public Building building;
  [MyCmpGet]
  private EnergyConsumer consumer;
  [MyCmpAdd]
  private LoopingSounds loopingSounds;
  public Orientation verticalOrientation;
  [SerializeField]
  public bool hasComplexUserControls;
  [SerializeField]
  public float unpoweredAnimSpeed = 0.25f;
  [SerializeField]
  public float poweredAnimSpeed = 1f;
  [SerializeField]
  public Door.DoorType doorType;
  [SerializeField]
  public bool allowAutoControl = true;
  [SerializeField]
  public string doorClosingSoundEventName;
  [SerializeField]
  public string doorOpeningSoundEventName;
  private string doorClosingSound;
  private string doorOpeningSound;
  private static readonly HashedString SOUND_POWERED_PARAMETER = HashedString.op_Implicit("doorPowered");
  private static readonly HashedString SOUND_PROGRESS_PARAMETER = HashedString.op_Implicit("doorProgress");
  [Serialize]
  private bool hasBeenUnsealed;
  [Serialize]
  private Door.ControlState controlState;
  private bool on;
  private bool do_melt_check;
  private int openCount;
  private Door.ControlState requestedState;
  private Chore changeStateChore;
  private Door.Controller.Instance controller;
  private LoggerFSS log;
  private const float REFRESH_HACK_DELAY = 1f;
  private bool doorOpenLiquidRefreshHack;
  private float doorOpenLiquidRefreshTime;
  private static readonly EventSystem.IntraObjectHandler<Door> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Door>((Action<Door, object>) ((component, data) => component.OnCopySettings(data)));
  public static readonly HashedString OPEN_CLOSE_PORT_ID = new HashedString("DoorOpenClose");
  private static readonly KAnimFile[] OVERRIDE_ANIMS = new KAnimFile[1]
  {
    Assets.GetAnim(HashedString.op_Implicit("anim_use_remote_kanim"))
  };
  private static readonly EventSystem.IntraObjectHandler<Door> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Door>((Action<Door, object>) ((component, data) => component.OnOperationalChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<Door> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<Door>((Action<Door, object>) ((component, data) => component.OnLogicValueChanged(data)));
  private bool applyLogicChange;

  private void OnCopySettings(object data)
  {
    Door component = ((GameObject) data).GetComponent<Door>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.QueueStateChange(component.requestedState);
  }

  public Door() => this.SetOffsetTable(OffsetGroups.InvertedStandardTable);

  public Door.ControlState CurrentState => this.controlState;

  public Door.ControlState RequestedState => this.requestedState;

  public bool ShouldBlockFallingSand => this.rotatable.GetOrientation() != this.verticalOrientation;

  public bool isSealed => this.controller.sm.isSealed.Get(this.controller);

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.overrideAnims = Door.OVERRIDE_ANIMS;
    this.synchronizeAnims = false;
    this.SetWorkTime(3f);
    if (!string.IsNullOrEmpty(this.doorClosingSoundEventName))
      this.doorClosingSound = GlobalAssets.GetSound(this.doorClosingSoundEventName);
    if (!string.IsNullOrEmpty(this.doorOpeningSoundEventName))
      this.doorOpeningSound = GlobalAssets.GetSound(this.doorOpeningSoundEventName);
    this.Subscribe<Door>(-905833192, Door.OnCopySettingsDelegate);
  }

  private Door.ControlState GetNextState(Door.ControlState wantedState) => (Door.ControlState) ((int) (wantedState + 1) % 3);

  private static bool DisplacesGas(Door.DoorType type) => type != Door.DoorType.Internal;

  protected override void OnSpawn()
  {
    base.OnSpawn();
    if (Object.op_Inequality((Object) ((Component) this).GetComponent<KPrefabID>(), (Object) null))
      this.log = new LoggerFSS(nameof (Door), 35);
    if (!this.allowAutoControl && this.controlState == Door.ControlState.Auto)
      this.controlState = Door.ControlState.Locked;
    StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
    HandleVector<int>.Handle handle = structureTemperatures.GetHandle(((Component) this).gameObject);
    if (Door.DisplacesGas(this.doorType))
      structureTemperatures.Bypass(handle);
    this.controller = new Door.Controller.Instance(this);
    this.controller.StartSM();
    if (this.doorType == Door.DoorType.Sealed && !this.hasBeenUnsealed)
      this.Seal();
    this.UpdateDoorSpeed(this.operational.IsOperational);
    this.Subscribe<Door>(-592767678, Door.OnOperationalChangedDelegate);
    this.Subscribe<Door>(824508782, Door.OnOperationalChangedDelegate);
    this.Subscribe<Door>(-801688580, Door.OnLogicValueChangedDelegate);
    this.requestedState = this.CurrentState;
    this.ApplyRequestedControlState(true);
    int num1 = this.rotatable.GetOrientation() == Orientation.Neutral ? this.building.Def.WidthInCells * (this.building.Def.HeightInCells - 1) : 0;
    int num2 = this.rotatable.GetOrientation() == Orientation.Neutral ? this.building.Def.WidthInCells : this.building.Def.HeightInCells;
    for (int index = 0; index != num2; ++index)
    {
      int placementCell = this.building.PlacementCells[num1 + index];
      Grid.FakeFloor.Add(placementCell);
      Pathfinding.Instance.AddDirtyNavGridCell(placementCell);
    }
    List<int> intList = new List<int>();
    foreach (int placementCell in this.building.PlacementCells)
    {
      Grid.HasDoor[placementCell] = true;
      if (this.rotatable.IsRotated)
      {
        intList.Add(Grid.CellAbove(placementCell));
        intList.Add(Grid.CellBelow(placementCell));
      }
      else
      {
        intList.Add(Grid.CellLeft(placementCell));
        intList.Add(Grid.CellRight(placementCell));
      }
      SimMessages.SetCellProperties(placementCell, (byte) 8);
      if (Door.DisplacesGas(this.doorType))
        Grid.RenderedByWorld[placementCell] = false;
    }
  }

  protected override void OnCleanUp()
  {
    this.UpdateDoorState(true);
    List<int> intList = new List<int>();
    foreach (int placementCell in this.building.PlacementCells)
    {
      SimMessages.ClearCellProperties(placementCell, (byte) 12);
      Grid.RenderedByWorld[placementCell] = Grid.Element[placementCell].substance.renderedByWorld;
      Grid.FakeFloor.Remove(placementCell);
      if (Grid.Element[placementCell].IsSolid)
        SimMessages.ReplaceAndDisplaceElement(placementCell, SimHashes.Vacuum, CellEventLogger.Instance.DoorOpen, 0.0f);
      Pathfinding.Instance.AddDirtyNavGridCell(placementCell);
      if (this.rotatable.IsRotated)
      {
        intList.Add(Grid.CellAbove(placementCell));
        intList.Add(Grid.CellBelow(placementCell));
      }
      else
      {
        intList.Add(Grid.CellLeft(placementCell));
        intList.Add(Grid.CellRight(placementCell));
      }
    }
    foreach (int placementCell in this.building.PlacementCells)
    {
      Grid.HasDoor[placementCell] = false;
      Game.Instance.SetDupePassableSolid(placementCell, false, Grid.Solid[placementCell]);
      Grid.CritterImpassable[placementCell] = false;
      Grid.DupeImpassable[placementCell] = false;
      Pathfinding.Instance.AddDirtyNavGridCell(placementCell);
    }
    base.OnCleanUp();
  }

  public void Seal() => this.controller.sm.isSealed.Set(true, this.controller);

  public void OrderUnseal() => this.controller.GoTo((StateMachine.BaseState) this.controller.sm.Sealed.awaiting_unlock);

  private void RefreshControlState()
  {
    switch (this.controlState)
    {
      case Door.ControlState.Auto:
        this.controller.sm.isLocked.Set(false, this.controller);
        break;
      case Door.ControlState.Opened:
        this.controller.sm.isLocked.Set(false, this.controller);
        break;
      case Door.ControlState.Locked:
        this.controller.sm.isLocked.Set(true, this.controller);
        break;
    }
    this.Trigger(279163026, (object) this.controlState);
    this.SetWorldState();
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.CurrentDoorControlState, (object) this);
  }

  private void OnOperationalChanged(object data)
  {
    bool isOperational = this.operational.IsOperational;
    if (isOperational == this.on)
      return;
    this.UpdateDoorSpeed(isOperational);
    if (this.on && ((Component) this).GetComponent<KPrefabID>().HasTag(GameTags.Transition))
      this.SetActive(true);
    else
      this.SetActive(false);
  }

  private void UpdateDoorSpeed(bool powered)
  {
    this.on = powered;
    this.UpdateAnimAndSoundParams(powered);
    float positionPercent = this.animController.GetPositionPercent();
    this.animController.Play(this.animController.CurrentAnim.hash, this.animController.PlayMode);
    this.animController.SetPositionPercent(positionPercent);
  }

  private void UpdateAnimAndSoundParams(bool powered)
  {
    if (powered)
    {
      this.animController.PlaySpeedMultiplier = this.poweredAnimSpeed;
      if (this.doorClosingSound != null)
        this.loopingSounds.UpdateFirstParameter(this.doorClosingSound, Door.SOUND_POWERED_PARAMETER, 1f);
      if (this.doorOpeningSound == null)
        return;
      this.loopingSounds.UpdateFirstParameter(this.doorOpeningSound, Door.SOUND_POWERED_PARAMETER, 1f);
    }
    else
    {
      this.animController.PlaySpeedMultiplier = this.unpoweredAnimSpeed;
      if (this.doorClosingSound != null)
        this.loopingSounds.UpdateFirstParameter(this.doorClosingSound, Door.SOUND_POWERED_PARAMETER, 0.0f);
      if (this.doorOpeningSound == null)
        return;
      this.loopingSounds.UpdateFirstParameter(this.doorOpeningSound, Door.SOUND_POWERED_PARAMETER, 0.0f);
    }
  }

  private void SetActive(bool active)
  {
    if (!this.operational.IsOperational)
      return;
    this.operational.SetActive(active);
  }

  private void SetWorldState()
  {
    int[] placementCells = this.building.PlacementCells;
    bool is_door_open = this.IsOpen();
    this.SetPassableState(is_door_open, (IList<int>) placementCells);
    this.SetSimState(is_door_open, (IList<int>) placementCells);
  }

  private void SetPassableState(bool is_door_open, IList<int> cells)
  {
    for (int index = 0; index < cells.Count; ++index)
    {
      int cell = cells[index];
      switch (this.doorType)
      {
        case Door.DoorType.Pressure:
        case Door.DoorType.ManualPressure:
        case Door.DoorType.Sealed:
          Grid.CritterImpassable[cell] = this.controlState != Door.ControlState.Opened;
          bool solid = !is_door_open;
          bool passable = this.controlState != Door.ControlState.Locked;
          Game.Instance.SetDupePassableSolid(cell, passable, solid);
          if (this.controlState == Door.ControlState.Opened)
          {
            this.doorOpenLiquidRefreshHack = true;
            this.doorOpenLiquidRefreshTime = 1f;
            break;
          }
          break;
        case Door.DoorType.Internal:
          Grid.CritterImpassable[cell] = this.controlState != Door.ControlState.Opened;
          Grid.DupeImpassable[cell] = this.controlState == Door.ControlState.Locked;
          break;
      }
      Pathfinding.Instance.AddDirtyNavGridCell(cell);
    }
  }

  private void SetSimState(bool is_door_open, IList<int> cells)
  {
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    float mass = component.Mass / (float) cells.Count;
    for (int index = 0; index < cells.Count; ++index)
    {
      int cell = cells[index];
      switch (this.doorType)
      {
        case Door.DoorType.Pressure:
        case Door.DoorType.ManualPressure:
        case Door.DoorType.Sealed:
          World.Instance.groundRenderer.MarkDirty(cell);
          if (is_door_open)
          {
            HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(new System.Action(this.OnSimDoorOpened)));
            SimMessages.Dig(cell, handle.index, true);
            if (this.ShouldBlockFallingSand)
            {
              SimMessages.ClearCellProperties(cell, (byte) 4);
              break;
            }
            SimMessages.SetCellProperties(cell, (byte) 4);
            break;
          }
          HandleVector<Game.CallbackInfo>.Handle handle1 = Game.Instance.callbackManager.Add(new Game.CallbackInfo(new System.Action(this.OnSimDoorClosed)));
          float temperature = component.Temperature;
          if ((double) temperature <= 0.0)
            temperature = component.Temperature;
          SimMessages.ReplaceAndDisplaceElement(cell, component.ElementID, CellEventLogger.Instance.DoorClose, mass, temperature, callbackIdx: handle1.index);
          SimMessages.SetCellProperties(cell, (byte) 4);
          break;
      }
    }
  }

  private void UpdateDoorState(bool cleaningUp)
  {
    foreach (int placementCell in this.building.PlacementCells)
    {
      if (Grid.IsValidCell(placementCell))
        Grid.Foundation[placementCell] = !cleaningUp;
    }
  }

  public void QueueStateChange(Door.ControlState nextState)
  {
    this.requestedState = this.requestedState == nextState ? this.controlState : nextState;
    if (this.requestedState == this.controlState)
    {
      if (this.changeStateChore == null)
        return;
      this.changeStateChore.Cancel("Change state");
      this.changeStateChore = (Chore) null;
      ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState);
    }
    else if (DebugHandler.InstantBuildMode)
    {
      this.controlState = this.requestedState;
      this.RefreshControlState();
      this.OnOperationalChanged((object) null);
      ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState);
      this.Open();
      this.Close();
    }
    else
    {
      if (this.changeStateChore != null)
        this.changeStateChore.Cancel("Change state");
      ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState, (object) this);
      this.changeStateChore = (Chore) new WorkChore<Door>(Db.Get().ChoreTypes.Toggle, (IStateMachineTarget) this, only_when_operational: false);
    }
  }

  private void OnSimDoorOpened()
  {
    if (Object.op_Equality((Object) this, (Object) null) || !Door.DisplacesGas(this.doorType))
      return;
    StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
    structureTemperatures.UnBypass(structureTemperatures.GetHandle(((Component) this).gameObject));
    this.do_melt_check = false;
  }

  private void OnSimDoorClosed()
  {
    if (Object.op_Equality((Object) this, (Object) null) || !Door.DisplacesGas(this.doorType))
      return;
    StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
    structureTemperatures.Bypass(structureTemperatures.GetHandle(((Component) this).gameObject));
    this.do_melt_check = true;
  }

  protected override void OnCompleteWork(Worker worker)
  {
    base.OnCompleteWork(worker);
    this.changeStateChore = (Chore) null;
    this.ApplyRequestedControlState();
  }

  public void Open()
  {
    if (this.openCount == 0 && Door.DisplacesGas(this.doorType))
    {
      StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
      HandleVector<int>.Handle handle = structureTemperatures.GetHandle(((Component) this).gameObject);
      if (handle.IsValid() && structureTemperatures.IsBypassed(handle))
      {
        int[] placementCells = this.building.PlacementCells;
        float num1 = 0.0f;
        int num2 = 0;
        for (int index = 0; index < placementCells.Length; ++index)
        {
          int i = placementCells[index];
          if ((double) Grid.Mass[i] > 0.0)
          {
            ++num2;
            num1 += Grid.Temperature[i];
          }
        }
        if (num2 > 0)
        {
          float num3 = num1 / (float) placementCells.Length;
          PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
          KCrashReporter.Assert((double) num3 > 0.0, "Door has calculated an invalid temperature");
          double num4 = (double) num3;
          component.Temperature = (float) num4;
        }
      }
    }
    ++this.openCount;
    switch (this.controlState)
    {
      case Door.ControlState.Auto:
      case Door.ControlState.Opened:
        this.controller.sm.isOpen.Set(true, this.controller);
        break;
    }
  }

  public void Close()
  {
    this.openCount = Mathf.Max(0, this.openCount - 1);
    if (this.openCount == 0 && Door.DisplacesGas(this.doorType))
    {
      StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
      HandleVector<int>.Handle handle = structureTemperatures.GetHandle(((Component) this).gameObject);
      PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
      if (handle.IsValid() && !structureTemperatures.IsBypassed(handle))
      {
        float temperature = ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) structureTemperatures).GetPayload(handle).Temperature;
        component.Temperature = temperature;
      }
    }
    switch (this.controlState)
    {
      case Door.ControlState.Auto:
        if (this.openCount != 0)
          break;
        this.controller.sm.isOpen.Set(false, this.controller);
        Game.Instance.userMenu.Refresh(((Component) this).gameObject);
        break;
      case Door.ControlState.Locked:
        this.controller.sm.isOpen.Set(false, this.controller);
        break;
    }
  }

  public bool IsOpen() => this.controller.IsInsideState((StateMachine.BaseState) this.controller.sm.open) || this.controller.IsInsideState((StateMachine.BaseState) this.controller.sm.closedelay) || this.controller.IsInsideState((StateMachine.BaseState) this.controller.sm.closeblocked);

  private void ApplyRequestedControlState(bool force = false)
  {
    if (this.requestedState == this.controlState && !force)
      return;
    this.controlState = this.requestedState;
    this.RefreshControlState();
    this.OnOperationalChanged((object) null);
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ChangeDoorControlState);
    this.Trigger(1734268753, (object) this);
    if (force)
      return;
    this.Open();
    this.Close();
  }

  public void OnLogicValueChanged(object data)
  {
    LogicValueChanged logicValueChanged = (LogicValueChanged) data;
    if (HashedString.op_Inequality(logicValueChanged.portID, Door.OPEN_CLOSE_PORT_ID))
      return;
    int newValue = logicValueChanged.newValue;
    if (this.changeStateChore != null)
    {
      this.changeStateChore.Cancel("Change state");
      this.changeStateChore = (Chore) null;
    }
    this.requestedState = LogicCircuitNetwork.IsBitActive(0, newValue) ? Door.ControlState.Opened : Door.ControlState.Locked;
    this.applyLogicChange = true;
  }

  public void Sim200ms(float dt)
  {
    if (Object.op_Equality((Object) this, (Object) null))
      return;
    if (this.doorOpenLiquidRefreshHack)
    {
      this.doorOpenLiquidRefreshTime -= dt;
      if ((double) this.doorOpenLiquidRefreshTime <= 0.0)
      {
        this.doorOpenLiquidRefreshHack = false;
        foreach (int placementCell in this.building.PlacementCells)
          Pathfinding.Instance.AddDirtyNavGridCell(placementCell);
      }
    }
    if (this.applyLogicChange)
    {
      this.applyLogicChange = false;
      this.ApplyRequestedControlState();
    }
    if (!this.do_melt_check)
      return;
    StructureTemperatureComponents structureTemperatures = GameComps.StructureTemperatures;
    HandleVector<int>.Handle handle = structureTemperatures.GetHandle(((Component) this).gameObject);
    if (!handle.IsValid() || !structureTemperatures.IsBypassed(handle))
      return;
    foreach (int placementCell in this.building.PlacementCells)
    {
      if (!Grid.Solid[placementCell])
      {
        Util.KDestroyGameObject((Component) this);
        break;
      }
    }
  }

  [SpecialName]
  bool INavDoor.get_isSpawned() => this.isSpawned;

  public enum DoorType
  {
    Pressure,
    ManualPressure,
    Internal,
    Sealed,
  }

  public enum ControlState
  {
    Auto,
    Opened,
    Locked,
    NumStates,
  }

  public class Controller : GameStateMachine<Door.Controller, Door.Controller.Instance, Door>
  {
    public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State open;
    public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State opening;
    public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State closed;
    public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State closing;
    public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State closedelay;
    public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State closeblocked;
    public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State locking;
    public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State locked;
    public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State unlocking;
    public Door.Controller.SealedStates Sealed;
    public StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.BoolParameter isOpen;
    public StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.BoolParameter isLocked;
    public StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.BoolParameter isBlocked;
    public StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.BoolParameter isSealed;
    public StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.BoolParameter sealDirectionRight;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      default_state = (StateMachine.BaseState) this.closed;
      this.root.Update("RefreshIsBlocked", (Action<Door.Controller.Instance, float>) ((smi, dt) => smi.RefreshIsBlocked())).ParamTransition<bool>((StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.Parameter<bool>) this.isSealed, this.Sealed.closed, GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.IsTrue);
      this.closeblocked.PlayAnim("open").ParamTransition<bool>((StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.Parameter<bool>) this.isOpen, this.open, GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.IsTrue).ParamTransition<bool>((StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.Parameter<bool>) this.isBlocked, this.closedelay, GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.IsFalse);
      this.closedelay.PlayAnim("open").ScheduleGoTo(0.5f, (StateMachine.BaseState) this.closing).ParamTransition<bool>((StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.Parameter<bool>) this.isOpen, this.open, GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.IsTrue).ParamTransition<bool>((StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.Parameter<bool>) this.isBlocked, this.closeblocked, GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.IsTrue);
      this.closing.ParamTransition<bool>((StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.Parameter<bool>) this.isBlocked, this.closeblocked, GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.IsTrue).ToggleTag(GameTags.Transition).ToggleLoopingSound("Closing loop", (Func<Door.Controller.Instance, string>) (smi => smi.master.doorClosingSound), (Func<Door.Controller.Instance, bool>) (smi => !string.IsNullOrEmpty(smi.master.doorClosingSound))).Enter("SetParams", (StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State.Callback) (smi => smi.master.UpdateAnimAndSoundParams(smi.master.on))).Update((Action<Door.Controller.Instance, float>) ((smi, dt) =>
      {
        if (smi.master.doorClosingSound == null)
          return;
        smi.master.loopingSounds.UpdateSecondParameter(smi.master.doorClosingSound, Door.SOUND_PROGRESS_PARAMETER, smi.Get<KBatchedAnimController>().GetPositionPercent());
      }), (UpdateRate) 4).Enter("SetActive", (StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State.Callback) (smi => smi.master.SetActive(true))).Exit("SetActive", (StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State.Callback) (smi => smi.master.SetActive(false))).PlayAnim("closing").OnAnimQueueComplete(this.closed);
      this.open.PlayAnim("open").ParamTransition<bool>((StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.Parameter<bool>) this.isOpen, this.closeblocked, GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.IsFalse).Enter("SetWorldStateOpen", (StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State.Callback) (smi => smi.master.SetWorldState()));
      this.closed.PlayAnim("closed").ParamTransition<bool>((StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.Parameter<bool>) this.isOpen, this.opening, GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.IsTrue).ParamTransition<bool>((StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.Parameter<bool>) this.isLocked, this.locking, GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.IsTrue).Enter("SetWorldStateClosed", (StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State.Callback) (smi => smi.master.SetWorldState()));
      this.locking.PlayAnim("locked_pre").OnAnimQueueComplete(this.locked).Enter("SetWorldStateClosed", (StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State.Callback) (smi => smi.master.SetWorldState()));
      this.locked.PlayAnim("locked").ParamTransition<bool>((StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.Parameter<bool>) this.isLocked, this.unlocking, GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.IsFalse);
      this.unlocking.PlayAnim("locked_pst").OnAnimQueueComplete(this.closed);
      this.opening.ToggleTag(GameTags.Transition).ToggleLoopingSound("Opening loop", (Func<Door.Controller.Instance, string>) (smi => smi.master.doorOpeningSound), (Func<Door.Controller.Instance, bool>) (smi => !string.IsNullOrEmpty(smi.master.doorOpeningSound))).Enter("SetParams", (StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State.Callback) (smi => smi.master.UpdateAnimAndSoundParams(smi.master.on))).Update((Action<Door.Controller.Instance, float>) ((smi, dt) =>
      {
        if (smi.master.doorOpeningSound == null)
          return;
        smi.master.loopingSounds.UpdateSecondParameter(smi.master.doorOpeningSound, Door.SOUND_PROGRESS_PARAMETER, smi.Get<KBatchedAnimController>().GetPositionPercent());
      }), (UpdateRate) 4).Enter("SetActive", (StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State.Callback) (smi => smi.master.SetActive(true))).Exit("SetActive", (StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State.Callback) (smi => smi.master.SetActive(false))).PlayAnim("opening").OnAnimQueueComplete(this.open);
      this.Sealed.Enter((StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State.Callback) (smi =>
      {
        OccupyArea component = ((Component) smi.master).GetComponent<OccupyArea>();
        for (int index = 0; index < component.OccupiedCellsOffsets.Length; ++index)
          Grid.PreventFogOfWarReveal[Grid.OffsetCell(Grid.PosToCell(((Component) smi.master).gameObject), component.OccupiedCellsOffsets[index])] = false;
        smi.sm.isLocked.Set(true, smi);
        smi.master.controlState = Door.ControlState.Locked;
        smi.master.RefreshControlState();
        if (!((Component) smi.master).GetComponent<Unsealable>().facingRight)
          return;
        ((Component) smi.master).GetComponent<KBatchedAnimController>().FlipX = true;
      })).Enter("SetWorldStateClosed", (StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State.Callback) (smi => smi.master.SetWorldState())).Exit((StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State.Callback) (smi =>
      {
        smi.sm.isLocked.Set(false, smi);
        ((Component) smi.master).GetComponent<AccessControl>().controlEnabled = true;
        smi.master.controlState = Door.ControlState.Opened;
        smi.master.RefreshControlState();
        smi.sm.isOpen.Set(true, smi);
        smi.sm.isLocked.Set(false, smi);
        smi.sm.isSealed.Set(false, smi);
      }));
      this.Sealed.closed.PlayAnim("sealed", (KAnim.PlayMode) 1);
      this.Sealed.awaiting_unlock.ToggleChore((Func<Door.Controller.Instance, Chore>) (smi => this.CreateUnsealChore(smi, true)), this.Sealed.chore_pst);
      this.Sealed.chore_pst.Enter((StateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State.Callback) (smi =>
      {
        smi.master.hasBeenUnsealed = true;
        if (((Component) smi.master).GetComponent<Unsealable>().unsealed)
        {
          smi.GoTo((StateMachine.BaseState) this.opening);
          FogOfWarMask.ClearMask(Grid.CellRight(Grid.PosToCell(((Component) smi.master).gameObject)));
          FogOfWarMask.ClearMask(Grid.CellLeft(Grid.PosToCell(((Component) smi.master).gameObject)));
        }
        else
          smi.GoTo((StateMachine.BaseState) this.Sealed.closed);
      }));
    }

    private Chore CreateUnsealChore(Door.Controller.Instance smi, bool approach_right) => (Chore) new WorkChore<Unsealable>(Db.Get().ChoreTypes.Toggle, (IStateMachineTarget) smi.master);

    public class SealedStates : 
      GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State
    {
      public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State closed;
      public Door.Controller.SealedStates.AwaitingUnlock awaiting_unlock;
      public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State chore_pst;

      public class AwaitingUnlock : 
        GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State
      {
        public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State awaiting_arrival;
        public GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.State unlocking;
      }
    }

    public new class Instance : 
      GameStateMachine<Door.Controller, Door.Controller.Instance, Door, object>.GameInstance
    {
      public Instance(Door door)
        : base(door)
      {
      }

      public void RefreshIsBlocked()
      {
        bool flag = false;
        foreach (int placementCell in ((Component) this.master).GetComponent<Building>().PlacementCells)
        {
          if (Object.op_Inequality((Object) Grid.Objects[placementCell, 40], (Object) null))
          {
            flag = true;
            break;
          }
        }
        this.sm.isBlocked.Set(flag, this.smi);
      }
    }
  }
}
