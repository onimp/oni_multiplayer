// Decompiled with JetBrains decompiler
// Type: RanchedStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;

public class RanchedStates : 
  GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>
{
  private RanchedStates.RanchStates ranch;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.ranch;
    this.root.Exit("AbandonedRanchStation", (StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback) (smi =>
    {
      RanchedStates.Instance instance = smi;
      instance.GetRanchStation()?.Abandon(instance.Monitor);
    }));
    this.ranch.Enter(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.SubscribeToRancherStateChanges)).EventHandler(GameHashes.RanchStationNoLongerAvailable, new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.OnRanchStationNotAvailable)).BehaviourComplete(GameTags.Creatures.WantsToGetRanched, true).Exit(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.UnsubscribeFromRancherStateChanges)).Exit(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.ClearLayerOverride));
    this.ranch.Cheer.ToggleStatusItem((string) CREATURES.STATUSITEMS.EXCITED_TO_GET_RANCHED.NAME, (string) CREATURES.STATUSITEMS.EXCITED_TO_GET_RANCHED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).Enter("FaceRancher", (StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback) (smi => smi.GetComponent<Facing>().Face(TransformExtensions.GetPosition(smi.GetRanchStation().transform)))).PlayAnim("excited_loop").OnAnimQueueComplete(this.ranch.Cheer.Pst);
    this.ranch.Cheer.Pst.ScheduleGoTo(0.2f, (StateMachine.BaseState) this.ranch.Move);
    this.ranch.Move.DefaultState((GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State) this.ranch.Move.MoveToRanch).Enter("Speedup", (StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback) (smi => smi.GetComponent<Navigator>().defaultSpeed = smi.OriginalSpeed * 1.25f)).ToggleStatusItem((string) CREATURES.STATUSITEMS.EXCITED_TO_GET_RANCHED.NAME, (string) CREATURES.STATUSITEMS.EXCITED_TO_GET_RANCHED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).Exit("RestoreSpeed", (StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback) (smi => smi.GetComponent<Navigator>().defaultSpeed = smi.OriginalSpeed));
    this.ranch.Move.MoveToRanch.EnterTransition((GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State) this.ranch.Move.WaitInLine, GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.Not(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.Transition.ConditionCallback(RanchedStates.IsCrittersTurn))).MoveTo(new Func<RanchedStates.Instance, int>(RanchedStates.GetRanchNavTarget), (GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State) this.ranch.Move.WaitInLine);
    this.ranch.Move.WaitInLine.EnterTransition(this.ranch.Ranching, new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.Transition.ConditionCallback(RanchedStates.IsCrittersTurn)).Enter(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.MoveToWaitPosition)).EventHandler(GameHashes.DestinationReached, new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.Wait));
    this.ranch.Ranching.Enter(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.GetOnTable)).Enter("SetCreatureAtRanchingStation", (StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback) (smi =>
    {
      smi.GetRanchStation().MessageCreatureArrived(smi);
      smi.AnimController.SetSceneLayer(Grid.SceneLayer.BuildingUse);
    })).EventTransition(GameHashes.RanchingComplete, this.ranch.Wavegoodbye).ToggleStatusItem((string) CREATURES.STATUSITEMS.GETTING_RANCHED.NAME, (string) CREATURES.STATUSITEMS.GETTING_RANCHED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.ranch.Wavegoodbye.Enter(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.ClearLayerOverride)).OnAnimQueueComplete(this.ranch.Runaway).ToggleStatusItem((string) CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.NAME, (string) CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.ranch.Runaway.MoveTo(new Func<RanchedStates.Instance, int>(RanchedStates.GetRunawayCell)).ToggleStatusItem((string) CREATURES.STATUSITEMS.IDLE.NAME, (string) CREATURES.STATUSITEMS.IDLE.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
  }

  private static void ClearLayerOverride(RanchedStates.Instance smi) => smi.AnimController.SetSceneLayer(Grid.SceneLayer.Creatures);

  private static RanchStation.Instance GetRanchStation(RanchedStates.Instance smi) => smi.GetRanchStation();

  private static GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State GetInitialRanchState(
    RanchedStates.Instance smi)
  {
    return !RanchedStates.IsCrittersTurn(smi) ? (GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State) smi.sm.ranch.Move.WaitInLine : (GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State) smi.sm.ranch.Cheer;
  }

  private static void OnRanchStationNotAvailable(RanchedStates.Instance smi) => smi.GoTo((StateMachine.BaseState) null);

  private static void GetOnTable(RanchedStates.Instance smi)
  {
    Navigator navigator = smi.Get<Navigator>();
    if (navigator.IsValidNavType(NavType.Floor))
      navigator.SetCurrentNavType(NavType.Floor);
    smi.Get<Facing>().SetFacing(false);
  }

  private static bool IsCrittersTurn(RanchedStates.Instance smi)
  {
    RanchStation.Instance ranchStation = RanchedStates.GetRanchStation(smi);
    return ranchStation != null && ranchStation.IsRancherReady && ranchStation.TryGetRanched(smi);
  }

  private static int GetRanchNavTarget(RanchedStates.Instance smi)
  {
    RanchStation.Instance ranchStation = RanchedStates.GetRanchStation(smi);
    return smi.ModifyNavTargetForCritter(ranchStation.GetRanchNavTarget());
  }

  private static void MoveToWaitPosition(RanchedStates.Instance smi) => smi.EnterQueue();

  private static void Wait(RanchedStates.Instance smi)
  {
    RanchStation.Instance targetRanchStation = smi.Monitor.TargetRanchStation;
    smi.Monitor.NavComponent.IsFacingLeft = (double) targetRanchStation.transform.position.x - (double) smi.transform.position.x < 0.0;
    smi.AnimController.Queue(smi.def.StartWaitingAnim);
    smi.AnimController.Play(smi.def.WaitingAnim, (KAnim.PlayMode) 0);
  }

  private static void SubscribeToRancherStateChanges(RanchedStates.Instance smi)
  {
    RanchStation.Instance ranchStation = smi.GetRanchStation();
    if (ranchStation == null)
      return;
    ranchStation.RancherStateChanged += new System.Action<RanchStation.Instance>(smi.OnRancherStateChanged);
    if (!ranchStation.IsRancherReady)
      return;
    smi.OnRancherStateChanged(ranchStation);
  }

  private static void UnsubscribeFromRancherStateChanges(RanchedStates.Instance smi)
  {
    RanchStation.Instance ranchStation = smi.GetRanchStation();
    if (ranchStation == null)
      return;
    ranchStation.RancherStateChanged -= new System.Action<RanchStation.Instance>(smi.OnRancherStateChanged);
  }

  private static int GetRunawayCell(RanchedStates.Instance smi)
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(smi.transform));
    int i = Grid.OffsetCell(cell, 2, 0);
    if (Grid.Solid[i])
      i = Grid.OffsetCell(cell, -2, 0);
    return i;
  }

  public class Def : StateMachine.BaseDef
  {
    public HashedString StartWaitingAnim = HashedString.op_Implicit("queue_pre");
    public HashedString WaitingAnim = HashedString.op_Implicit("queue_loop");
    public HashedString EndWaitingAnim = HashedString.op_Implicit("queue_pst");
    public int WaitCellOffset = 1;

    public bool IsQueueAnim(HashedString anim) => HashedString.op_Equality(this.StartWaitingAnim, anim) | HashedString.op_Equality(this.WaitingAnim, anim) | HashedString.op_Equality(this.EndWaitingAnim, anim);
  }

  public new class Instance : 
    GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.GameInstance
  {
    public float OriginalSpeed;
    private int waitCell;
    private KBatchedAnimController animController;
    private RanchableMonitor.Instance ranchMonitor;

    public RanchableMonitor.Instance Monitor
    {
      get
      {
        if (this.ranchMonitor == null)
          this.ranchMonitor = this.GetSMI<RanchableMonitor.Instance>();
        return this.ranchMonitor;
      }
    }

    public KBatchedAnimController AnimController => this.animController;

    public Instance(Chore<RanchedStates.Instance> chore, RanchedStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      this.animController = this.GetComponent<KBatchedAnimController>();
      this.OriginalSpeed = this.Monitor.NavComponent.defaultSpeed;
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.WantsToGetRanched);
    }

    public override void StartSM()
    {
      base.StartSM();
      this.animController.onAnimComplete += new KAnimControllerBase.KAnimEvent(this.OnAnimComplete);
    }

    public override void StopSM(string reason)
    {
      base.StopSM(reason);
      this.animController.onAnimComplete -= new KAnimControllerBase.KAnimEvent(this.OnAnimComplete);
    }

    public RanchStation.Instance GetRanchStation() => this.Monitor != null ? this.Monitor.TargetRanchStation : (RanchStation.Instance) null;

    public void EnterQueue()
    {
      this.InitializeWaitCell();
      this.Monitor.NavComponent.GoTo(this.waitCell);
    }

    public void ExitQueue()
    {
      if (!RanchedStates.IsCrittersTurn(this))
        return;
      if ((!this.animController.HasAnimation(this.def.EndWaitingAnim) ? 0 : (HashedString.op_Inequality(this.animController.currentAnim, this.def.EndWaitingAnim) ? 1 : 0)) != 0 && this.def.IsQueueAnim(this.animController.currentAnim))
        this.animController.Play(this.def.EndWaitingAnim);
      else
        this.GoTo((StateMachine.BaseState) this.sm.ranch.Move.MoveToRanch);
    }

    public void AbandonRanchStation()
    {
      if (this.Monitor.TargetRanchStation == null || this.status == StateMachine.Status.Failed)
        return;
      this.StopSM("Abandoned Ranch");
    }

    public int ModifyNavTargetForCritter(int navCell) => this.smi.HasTag(GameTags.Creatures.Flyer) ? Grid.CellAbove(navCell) : navCell;

    private void InitializeWaitCell()
    {
      if (this.Monitor == null)
        return;
      int cell1 = 0;
      Extents stationExtents = this.Monitor.TargetRanchStation.StationExtents;
      int cell2 = this.ModifyNavTargetForCritter(Grid.XYToCell(stationExtents.x, stationExtents.y));
      int num1 = 0;
      int hitDistance1;
      if (Grid.Raycast(cell2, new Vector2I(-1, 0), out hitDistance1, this.def.WaitCellOffset, Grid.BuildFlags.FakeFloor | Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.CritterImpassable))
      {
        num1 = 1 + this.def.WaitCellOffset - hitDistance1;
        cell1 = this.ModifyNavTargetForCritter(Grid.XYToCell(stationExtents.x + 1, stationExtents.y));
      }
      int num2 = 0;
      int hitDistance2;
      if (num1 != 0 && Grid.Raycast(cell1, new Vector2I(1, 0), out hitDistance2, this.def.WaitCellOffset, Grid.BuildFlags.FakeFloor | Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.CritterImpassable))
        num2 = this.def.WaitCellOffset - hitDistance2;
      int num3 = (this.def.WaitCellOffset - num1) * -1;
      if (num1 == this.def.WaitCellOffset)
        num3 = 1 + this.def.WaitCellOffset - num2;
      CellOffset offset;
      // ISSUE: explicit constructor call
      ((CellOffset) ref offset).\u002Ector(num3, 0);
      this.waitCell = Grid.OffsetCell(cell2, offset);
    }

    public void OnRancherStateChanged(RanchStation.Instance ranch)
    {
      if (!(this.smi.GetCurrentState() is RanchedStates.IRanchStatesCallbacks currentState))
        return;
      currentState.OnRancherStateChanged(this);
    }

    private void OnAnimComplete(HashedString completedAnim)
    {
      if (!(this.smi.GetCurrentState() is RanchedStates.IRanchStatesCallbacks currentState))
        return;
      currentState.OnAnimComplete(this, completedAnim);
    }
  }

  public class RanchStates : 
    GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State,
    RanchedStates.IRanchStatesCallbacks
  {
    public RanchedStates.CheerStates Cheer;
    public RanchedStates.MoveStates Move;
    public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State Ranching;
    public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State Wavegoodbye;
    public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State Runaway;

    public void OnRancherStateChanged(RanchedStates.Instance smi)
    {
      if (RanchedStates.IsCrittersTurn(smi))
        smi.GoTo((StateMachine.BaseState) smi.sm.ranch.Cheer);
      else
        smi.GoTo((StateMachine.BaseState) smi.sm.ranch.Move.WaitInLine);
    }

    public void OnAnimComplete(RanchedStates.Instance smi, HashedString completedAnim)
    {
    }
  }

  public class CheerStates : 
    GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State
  {
    public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State Cheer;
    public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State Pst;
  }

  public class MoveStates : 
    GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State
  {
    public RanchedStates.MoveState MoveToRanch;
    public RanchedStates.WaitState WaitInLine;
  }

  public interface IRanchStatesCallbacks
  {
    void OnRancherStateChanged(RanchedStates.Instance smi);

    void OnAnimComplete(RanchedStates.Instance smi, HashedString completedAnim);
  }

  public class MoveState : 
    GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State,
    RanchedStates.IRanchStatesCallbacks
  {
    public void OnRancherStateChanged(RanchedStates.Instance smi) => smi.GoTo((StateMachine.BaseState) this.sm.ranch.Move.WaitInLine);

    public void OnAnimComplete(RanchedStates.Instance smi, HashedString completedAnim)
    {
    }
  }

  public class WaitState : 
    GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State,
    RanchedStates.IRanchStatesCallbacks
  {
    public void OnRancherStateChanged(RanchedStates.Instance smi) => smi.ExitQueue();

    public void OnAnimComplete(RanchedStates.Instance smi, HashedString completedAnim)
    {
      if (!HashedString.op_Equality(completedAnim, smi.def.EndWaitingAnim))
        return;
      smi.ExitQueue();
    }
  }
}
