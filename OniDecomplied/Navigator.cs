// Decompiled with JetBrains decompiler
// Type: Navigator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Navigator : StateMachineComponent<Navigator.StatesInstance>, ISaveLoadableDetails
{
  public bool DebugDrawPath;
  [MyCmpAdd]
  public PathProber PathProber;
  [MyCmpAdd]
  private Facing facing;
  [MyCmpGet]
  public AnimEventHandler animEventHandler;
  public float defaultSpeed = 1f;
  public TransitionDriver transitionDriver;
  public string NavGridName;
  public bool updateProber;
  public int maxProbingRadius;
  public PathFinder.PotentialPath.Flags flags;
  private LoggerFSS log;
  public Dictionary<NavType, int> distanceTravelledByNavType;
  public Grid.SceneLayer sceneLayer = Grid.SceneLayer.Move;
  private PathFinderAbilities abilities;
  [MyCmpReq]
  private KSelectable selectable;
  [NonSerialized]
  public PathFinder.Path path;
  public NavType CurrentNavType;
  private int AnchorCell;
  private KPrefabID targetLocator;
  private int reservedCell = NavigationReservations.InvalidReservation;
  private NavTactic tactic;
  public Navigator.PathProbeTask pathProbeTask;
  private static readonly EventSystem.IntraObjectHandler<Navigator> OnDefeatedDelegate = new EventSystem.IntraObjectHandler<Navigator>((Action<Navigator, object>) ((component, data) => component.OnDefeated(data)));
  private static readonly EventSystem.IntraObjectHandler<Navigator> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Navigator>((Action<Navigator, object>) ((component, data) => component.OnRefreshUserMenu(data)));
  private static readonly EventSystem.IntraObjectHandler<Navigator> OnSelectObjectDelegate = new EventSystem.IntraObjectHandler<Navigator>((Action<Navigator, object>) ((component, data) => component.OnSelectObject(data)));
  private static readonly EventSystem.IntraObjectHandler<Navigator> OnStoreDelegate = new EventSystem.IntraObjectHandler<Navigator>((Action<Navigator, object>) ((component, data) => component.OnStore(data)));
  public bool executePathProbeTaskAsync;

  public bool IsFacingLeft
  {
    get => this.facing.GetFacing();
    set => this.facing.SetFacing(value);
  }

  public KMonoBehaviour target { get; set; }

  public CellOffset[] targetOffsets { get; private set; }

  public NavGrid NavGrid { get; private set; }

  public void Serialize(BinaryWriter writer)
  {
    byte currentNavType = (byte) this.CurrentNavType;
    writer.Write(currentNavType);
    writer.Write(this.distanceTravelledByNavType.Count);
    foreach (KeyValuePair<NavType, int> keyValuePair in this.distanceTravelledByNavType)
    {
      byte key = (byte) keyValuePair.Key;
      writer.Write(key);
      writer.Write(keyValuePair.Value);
    }
  }

  public void Deserialize(IReader reader)
  {
    NavType navType = (NavType) reader.ReadByte();
    if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 11))
    {
      int num1 = reader.ReadInt32();
      for (int index = 0; index < num1; ++index)
      {
        NavType key = (NavType) reader.ReadByte();
        int num2 = reader.ReadInt32();
        if (this.distanceTravelledByNavType.ContainsKey(key))
          this.distanceTravelledByNavType[key] = num2;
      }
    }
    bool flag = false;
    foreach (NavType validNavType in this.NavGrid.ValidNavTypes)
    {
      if (validNavType == navType)
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      return;
    this.CurrentNavType = navType;
  }

  protected virtual void OnPrefabInit()
  {
    this.transitionDriver = new TransitionDriver(this);
    this.targetLocator = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(TargetLocator.ID)), (GameObject) null, (string) null).GetComponent<KPrefabID>();
    ((Component) this.targetLocator).gameObject.SetActive(true);
    this.log = new LoggerFSS(nameof (Navigator), 35);
    this.simRenderLoadBalance = true;
    this.autoRegisterSimRender = false;
    this.NavGrid = Pathfinding.Instance.GetNavGrid(this.NavGridName);
    ((Component) this).GetComponent<PathProber>().SetValidNavTypes(this.NavGrid.ValidNavTypes, this.maxProbingRadius);
    this.distanceTravelledByNavType = new Dictionary<NavType, int>();
    for (int key = 0; key < 11; ++key)
      this.distanceTravelledByNavType.Add((NavType) key, 0);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<Navigator>(1623392196, Navigator.OnDefeatedDelegate);
    this.Subscribe<Navigator>(-1506500077, Navigator.OnDefeatedDelegate);
    this.Subscribe<Navigator>(493375141, Navigator.OnRefreshUserMenuDelegate);
    this.Subscribe<Navigator>(-1503271301, Navigator.OnSelectObjectDelegate);
    this.Subscribe<Navigator>(856640610, Navigator.OnStoreDelegate);
    if (this.updateProber)
      SimAndRenderScheduler.instance.Add((object) this, false);
    this.pathProbeTask = new Navigator.PathProbeTask(this);
    this.SetCurrentNavType(this.CurrentNavType);
    this.SubscribeUnstuckFunctions();
  }

  private void SubscribeUnstuckFunctions()
  {
    if (this.CurrentNavType != NavType.Tube)
      return;
    GameScenePartitioner.Instance.AddGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[1], new Action<int, object>(this.OnBuildingTileChanged));
  }

  private void UnsubscribeUnstuckFunctions() => GameScenePartitioner.Instance.RemoveGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[1], new Action<int, object>(this.OnBuildingTileChanged));

  private void OnBuildingTileChanged(int cell, object building)
  {
    if (this.CurrentNavType != NavType.Tube || building != null || !(this.smi != null & cell == Grid.PosToCell((KMonoBehaviour) this)))
      return;
    this.SetCurrentNavType(NavType.Floor);
    this.UnsubscribeUnstuckFunctions();
  }

  protected override void OnCleanUp()
  {
    this.UnsubscribeUnstuckFunctions();
    base.OnCleanUp();
  }

  public bool IsMoving() => this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.normal.moving);

  public bool GoTo(int cell, CellOffset[] offsets = null)
  {
    if (offsets == null)
      offsets = new CellOffset[1];
    TransformExtensions.SetPosition(((KMonoBehaviour) this.targetLocator).transform, Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
    return this.GoTo((KMonoBehaviour) this.targetLocator, offsets, NavigationTactics.ReduceTravelDistance);
  }

  public bool GoTo(int cell, CellOffset[] offsets, NavTactic tactic)
  {
    if (offsets == null)
      offsets = new CellOffset[1];
    TransformExtensions.SetPosition(((KMonoBehaviour) this.targetLocator).transform, Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
    return this.GoTo((KMonoBehaviour) this.targetLocator, offsets, tactic);
  }

  public void UpdateTarget(int cell) => TransformExtensions.SetPosition(((KMonoBehaviour) this.targetLocator).transform, Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));

  public bool GoTo(KMonoBehaviour target, CellOffset[] offsets, NavTactic tactic)
  {
    if (tactic == null)
      tactic = NavigationTactics.ReduceTravelDistance;
    this.smi.GoTo((StateMachine.BaseState) this.smi.sm.normal.moving);
    this.smi.sm.moveTarget.Set(((Component) target).gameObject, this.smi, false);
    this.tactic = tactic;
    this.target = target;
    this.targetOffsets = offsets;
    this.ClearReservedCell();
    this.AdvancePath();
    return this.IsMoving();
  }

  public void BeginTransition(NavGrid.Transition transition)
  {
    this.transitionDriver.EndTransition();
    this.smi.GoTo((StateMachine.BaseState) this.smi.sm.normal.moving);
    this.transitionDriver.BeginTransition(this, transition, this.defaultSpeed);
  }

  private bool ValidatePath(ref PathFinder.Path path, out bool atNextNode)
  {
    atNextNode = false;
    bool flag = false;
    if (path.IsValid())
    {
      int cell = Grid.PosToCell(this.target);
      flag = (this.reservedCell != NavigationReservations.InvalidReservation && this.CanReach(this.reservedCell)) & Grid.IsCellOffsetOf(this.reservedCell, cell, this.targetOffsets);
    }
    if (flag)
    {
      int cell = Grid.PosToCell((KMonoBehaviour) this);
      flag = ((cell != path.nodes[0].cell ? (false ? 1 : 0) : (this.CurrentNavType == path.nodes[0].navType ? 1 : 0)) | ((atNextNode = cell == path.nodes[1].cell && this.CurrentNavType == path.nodes[1].navType) ? 1 : 0)) != 0;
    }
    return flag && PathFinder.ValidatePath(this.NavGrid, this.GetCurrentAbilities(), ref path);
  }

  public void AdvancePath(bool trigger_advance = true)
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    if (Object.op_Equality((Object) this.target, (Object) null))
    {
      this.Trigger(-766531887, (object) null);
      this.Stop();
    }
    else if (cell == this.reservedCell && this.CurrentNavType != NavType.Tube)
    {
      this.Stop(true);
    }
    else
    {
      bool atNextNode;
      int num = !this.ValidatePath(ref this.path, out atNextNode) ? 1 : 0;
      if (atNextNode)
        this.path.nodes.RemoveAt(0);
      if (num != 0)
      {
        this.SetReservedCell(this.tactic.GetCellPreferences(Grid.PosToCell(this.target), this.targetOffsets, this));
        if (this.reservedCell == NavigationReservations.InvalidReservation)
        {
          this.Stop();
        }
        else
        {
          PathFinder.PotentialPath potential_path = new PathFinder.PotentialPath(cell, this.CurrentNavType, this.flags);
          PathFinder.UpdatePath(this.NavGrid, this.GetCurrentAbilities(), potential_path, (PathFinderQuery) PathFinderQueries.cellQuery.Reset(this.reservedCell), ref this.path);
        }
      }
      if (this.path.IsValid())
      {
        this.BeginTransition(this.NavGrid.transitions[(int) this.path.nodes[1].transitionId]);
        this.distanceTravelledByNavType[this.CurrentNavType] = Mathf.Max(this.distanceTravelledByNavType[this.CurrentNavType] + 1, this.distanceTravelledByNavType[this.CurrentNavType]);
      }
      else if (this.path.HasArrived())
      {
        this.Stop(true);
      }
      else
      {
        this.ClearReservedCell();
        this.Stop();
      }
    }
    if (!trigger_advance)
      return;
    this.Trigger(1347184327, (object) null);
  }

  public NavGrid.Transition GetNextTransition() => this.NavGrid.transitions[(int) this.path.nodes[1].transitionId];

  public void Stop(bool arrived_at_destination = false, bool play_idle = true)
  {
    this.target = (KMonoBehaviour) null;
    this.targetOffsets = (CellOffset[]) null;
    this.path.Clear();
    this.smi.sm.moveTarget.Set((KMonoBehaviour) null, this.smi);
    this.transitionDriver.EndTransition();
    if (play_idle)
    {
      HashedString idleAnim = this.NavGrid.GetIdleAnim(this.CurrentNavType);
      ((Component) this).GetComponent<KAnimControllerBase>().Play(idleAnim, (KAnim.PlayMode) 0);
    }
    if (arrived_at_destination)
    {
      this.smi.GoTo((StateMachine.BaseState) this.smi.sm.normal.arrived);
    }
    else
    {
      if (this.smi.GetCurrentState() != this.smi.sm.normal.moving)
        return;
      this.ClearReservedCell();
      this.smi.GoTo((StateMachine.BaseState) this.smi.sm.normal.failed);
    }
  }

  private void SimEveryTick(float dt)
  {
    if (!this.IsMoving())
      return;
    this.transitionDriver.UpdateTransition(dt);
  }

  public void Sim4000ms(float dt) => this.UpdateProbe(true);

  public void UpdateProbe(bool forceUpdate = false)
  {
    if (!forceUpdate && this.executePathProbeTaskAsync)
      return;
    this.pathProbeTask.Update();
    this.pathProbeTask.Run((object) null);
  }

  public void DrawPath()
  {
    if (!((Component) this).gameObject.activeInHierarchy || !this.IsMoving())
      return;
    NavPathDrawer.Instance.DrawPath(((Component) this).GetComponent<KAnimControllerBase>().GetPivotSymbolPosition(), this.path);
  }

  public void Pause(string reason) => this.smi.sm.isPaused.Set(true, this.smi);

  public void Unpause(string reason) => this.smi.sm.isPaused.Set(false, this.smi);

  private void OnDefeated(object data)
  {
    this.ClearReservedCell();
    this.Stop(play_idle: false);
  }

  private void ClearReservedCell()
  {
    if (this.reservedCell == NavigationReservations.InvalidReservation)
      return;
    NavigationReservations.Instance.RemoveOccupancy(this.reservedCell);
    this.reservedCell = NavigationReservations.InvalidReservation;
  }

  private void SetReservedCell(int cell)
  {
    this.ClearReservedCell();
    this.reservedCell = cell;
    NavigationReservations.Instance.AddOccupancy(cell);
  }

  public int GetReservedCell() => this.reservedCell;

  public int GetAnchorCell() => this.AnchorCell;

  public bool IsValidNavType(NavType nav_type) => this.NavGrid.HasNavTypeData(nav_type);

  public void SetCurrentNavType(NavType nav_type)
  {
    this.CurrentNavType = nav_type;
    this.AnchorCell = NavTypeHelper.GetAnchorCell(nav_type, Grid.PosToCell((KMonoBehaviour) this));
    NavGrid.NavTypeData navTypeData = this.NavGrid.GetNavTypeData(this.CurrentNavType);
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    Vector2 one = Vector2.one;
    if (navTypeData.flipX)
      one.x = -1f;
    if (navTypeData.flipY)
      one.y = -1f;
    Matrix2x3 matrix2x3 = Matrix2x3.op_Multiply(Matrix2x3.op_Multiply(Matrix2x3.Translate(Vector2.op_Multiply(navTypeData.animControllerOffset, 200f)), Matrix2x3.Rotate(navTypeData.rotation)), Matrix2x3.Scale(one));
    component.navMatrix = matrix2x3;
  }

  private void OnRefreshUserMenu(object data)
  {
    if (((Component) this).gameObject.HasTag(GameTags.Dead))
      return;
    Game.Instance.userMenu.AddButton(((Component) this).gameObject, Object.op_Inequality((Object) NavPathDrawer.Instance.GetNavigator(), (Object) this) ? new KIconButtonMenu.ButtonInfo("action_navigable_regions", (string) UI.USERMENUACTIONS.DRAWPATHS.NAME, new System.Action(this.OnDrawPaths), tooltipText: ((string) UI.USERMENUACTIONS.DRAWPATHS.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_navigable_regions", (string) UI.USERMENUACTIONS.DRAWPATHS.NAME_OFF, new System.Action(this.OnDrawPaths), tooltipText: ((string) UI.USERMENUACTIONS.DRAWPATHS.TOOLTIP_OFF)), 0.1f);
    Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("action_follow_cam", (string) UI.USERMENUACTIONS.FOLLOWCAM.NAME, new System.Action(this.OnFollowCam), tooltipText: ((string) UI.USERMENUACTIONS.FOLLOWCAM.TOOLTIP)), 0.3f);
  }

  private void OnFollowCam()
  {
    if (Object.op_Equality((Object) CameraController.Instance.followTarget, (Object) this.transform))
      CameraController.Instance.ClearFollowTarget();
    else
      CameraController.Instance.SetFollowTarget(this.transform);
  }

  private void OnDrawPaths()
  {
    if (Object.op_Inequality((Object) NavPathDrawer.Instance.GetNavigator(), (Object) this))
      NavPathDrawer.Instance.SetNavigator(this);
    else
      NavPathDrawer.Instance.ClearNavigator();
  }

  private void OnSelectObject(object data) => NavPathDrawer.Instance.ClearNavigator();

  public void OnStore(object data)
  {
    if ((data is Storage ? 1 : (data != null ? ((bool) data ? 1 : 0) : 0)) == 0)
      return;
    this.Stop();
  }

  public PathFinderAbilities GetCurrentAbilities()
  {
    this.abilities.Refresh();
    return this.abilities;
  }

  public void SetAbilities(PathFinderAbilities abilities) => this.abilities = abilities;

  public bool CanReach(IApproachable approachable) => this.CanReach(approachable.GetCell(), approachable.GetOffsets());

  public bool CanReach(int cell, CellOffset[] offsets)
  {
    foreach (CellOffset offset in offsets)
    {
      if (this.CanReach(Grid.OffsetCell(cell, offset)))
        return true;
    }
    return false;
  }

  public bool CanReach(int cell) => this.GetNavigationCost(cell) != -1;

  public int GetNavigationCost(int cell) => Grid.IsValidCell(cell) ? this.PathProber.GetCost(cell) : -1;

  public int GetNavigationCostIgnoreProberOffset(int cell, CellOffset[] offsets) => this.PathProber.GetNavigationCostIgnoreProberOffset(cell, offsets);

  public int GetNavigationCost(int cell, CellOffset[] offsets)
  {
    int navigationCost1 = -1;
    int length = offsets.Length;
    for (int index = 0; index < length; ++index)
    {
      int navigationCost2 = this.GetNavigationCost(Grid.OffsetCell(cell, offsets[index]));
      if (navigationCost2 != -1 && (navigationCost1 == -1 || navigationCost2 < navigationCost1))
        navigationCost1 = navigationCost2;
    }
    return navigationCost1;
  }

  public int GetNavigationCost(IApproachable approachable) => this.GetNavigationCost(approachable.GetCell(), approachable.GetOffsets());

  public void RunQuery(PathFinderQuery query)
  {
    PathFinder.PotentialPath potential_path = new PathFinder.PotentialPath(Grid.PosToCell((KMonoBehaviour) this), this.CurrentNavType, this.flags);
    PathFinder.Run(this.NavGrid, this.GetCurrentAbilities(), potential_path, query);
  }

  public void SetFlags(PathFinder.PotentialPath.Flags new_flags) => this.flags |= new_flags;

  public void ClearFlags(PathFinder.PotentialPath.Flags new_flags) => this.flags &= ~new_flags;

  public class ActiveTransition
  {
    public int x;
    public int y;
    public bool isLooping;
    public NavType start;
    public NavType end;
    public HashedString preAnim;
    public HashedString anim;
    public float speed;
    public float animSpeed = 1f;
    public Func<bool> isCompleteCB;
    public NavGrid.Transition navGridTransition;

    public void Init(NavGrid.Transition transition, float default_speed)
    {
      this.x = transition.x;
      this.y = transition.y;
      this.isLooping = transition.isLooping;
      this.start = transition.start;
      this.end = transition.end;
      this.preAnim = HashedString.op_Implicit(transition.preAnim);
      this.anim = HashedString.op_Implicit(transition.anim);
      this.speed = default_speed;
      this.animSpeed = transition.animSpeed;
      this.navGridTransition = transition;
    }

    public void Copy(Navigator.ActiveTransition other)
    {
      this.x = other.x;
      this.y = other.y;
      this.isLooping = other.isLooping;
      this.start = other.start;
      this.end = other.end;
      this.preAnim = other.preAnim;
      this.anim = other.anim;
      this.speed = other.speed;
      this.animSpeed = other.animSpeed;
      this.navGridTransition = other.navGridTransition;
    }
  }

  public class StatesInstance : 
    GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.GameInstance
  {
    public StatesInstance(Navigator master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator>
  {
    public StateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.TargetParameter moveTarget;
    public StateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.BoolParameter isPaused = new StateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.BoolParameter(false);
    public Navigator.States.NormalStates normal;
    public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State paused;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.normal.stopped;
      this.saveHistory = true;
      this.normal.ParamTransition<bool>((StateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.Parameter<bool>) this.isPaused, this.paused, GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.IsTrue).Update("NavigatorProber", (Action<Navigator.StatesInstance, float>) ((smi, dt) => smi.master.Sim4000ms(dt)), (UpdateRate) 7);
      this.normal.moving.Enter((StateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State.Callback) (smi => smi.Trigger(1027377649, (object) GameHashes.ObjectMovementWakeUp))).Update("UpdateNavigator", (Action<Navigator.StatesInstance, float>) ((smi, dt) => smi.master.SimEveryTick(dt)), (UpdateRate) 3, true).Exit((StateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State.Callback) (smi => smi.Trigger(1027377649, (object) GameHashes.ObjectMovementSleep)));
      this.normal.arrived.TriggerOnEnter(GameHashes.DestinationReached).GoTo(this.normal.stopped);
      this.normal.failed.TriggerOnEnter(GameHashes.NavigationFailed).GoTo(this.normal.stopped);
      this.normal.stopped.Enter((StateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State.Callback) (smi => smi.master.SubscribeUnstuckFunctions())).DoNothing().Exit((StateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State.Callback) (smi => smi.master.UnsubscribeUnstuckFunctions()));
      this.paused.ParamTransition<bool>((StateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.Parameter<bool>) this.isPaused, (GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State) this.normal, GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.IsFalse);
    }

    public class NormalStates : 
      GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State
    {
      public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State moving;
      public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State arrived;
      public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State failed;
      public GameStateMachine<Navigator.States, Navigator.StatesInstance, Navigator, object>.State stopped;
    }
  }

  public struct PathProbeTask : IWorkItem<object>
  {
    private int cell;
    private Navigator navigator;

    public PathProbeTask(Navigator navigator)
    {
      this.navigator = navigator;
      this.cell = -1;
    }

    public void Update()
    {
      this.cell = Grid.PosToCell((KMonoBehaviour) this.navigator);
      this.navigator.abilities.Refresh();
    }

    public void Run(object sharedData) => this.navigator.PathProber.UpdateProbe(this.navigator.NavGrid, this.cell, this.navigator.CurrentNavType, this.navigator.abilities, this.navigator.flags);
  }
}
