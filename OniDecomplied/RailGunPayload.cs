// Decompiled with JetBrains decompiler
// Type: RailGunPayload
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class RailGunPayload : 
  GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>
{
  public StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.IntParameter destinationWorld = new StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.IntParameter(-1);
  public StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.BoolParameter onSurface = new StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.BoolParameter(false);
  public StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.Signal beginTravelling;
  public StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.Signal launch;
  public RailGunPayload.TakeoffStates takeoff;
  public RailGunPayload.TravelStates travel;
  public RailGunPayload.LandingStates landing;
  public RailGunPayload.GroundedStates grounded;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.grounded.idle;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.grounded.DefaultState(this.grounded.idle).Enter((StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State.Callback) (smi => this.onSurface.Set(true, smi))).ToggleMainStatusItem(Db.Get().BuildingStatusItems.RailgunpayloadNeedsEmptying).ToggleTag(GameTags.RailGunPayloadEmptyable).ToggleTag(GameTags.ClusterEntityGrounded).EventHandler(GameHashes.DroppedAll, (StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State.Callback) (smi => smi.OnDroppedAll())).OnSignal(this.launch, (GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State) this.takeoff);
    this.grounded.idle.PlayAnim("idle");
    this.grounded.crater.Enter((StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State.Callback) (smi => smi.animController.randomiseLoopedOffset = true)).Exit((StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State.Callback) (smi => smi.animController.randomiseLoopedOffset = false)).PlayAnim("landed", (KAnim.PlayMode) 0).EventTransition(GameHashes.OnStore, this.grounded.idle);
    this.takeoff.DefaultState(this.takeoff.launch).Enter((StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State.Callback) (smi => this.onSurface.Set(false, smi))).PlayAnim("launching").OnSignal(this.beginTravelling, (GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State) this.travel);
    this.takeoff.launch.Enter((StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State.Callback) (smi => smi.StartTakeoff())).GoTo(this.takeoff.airborne);
    this.takeoff.airborne.Update("Launch", (System.Action<RailGunPayload.StatesInstance, float>) ((smi, dt) => smi.UpdateLaunch(dt)), (UpdateRate) 3);
    this.travel.DefaultState(this.travel.travelling).Enter((StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State.Callback) (smi => this.onSurface.Set(false, smi))).Enter((StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State.Callback) (smi => smi.MoveToSpace())).PlayAnim("idle").ToggleTag(GameTags.EntityInSpace).ToggleMainStatusItem(Db.Get().BuildingStatusItems.InFlight, (Func<RailGunPayload.StatesInstance, object>) (smi => (object) smi.GetComponent<ClusterTraveler>()));
    this.travel.travelling.EventTransition(GameHashes.ClusterDestinationReached, this.travel.transferWorlds);
    this.travel.transferWorlds.Enter((StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State.Callback) (smi => smi.StartLand())).GoTo(this.landing.landing);
    this.landing.DefaultState(this.landing.landing).ParamTransition<bool>((StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.Parameter<bool>) this.onSurface, this.grounded.crater, GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.IsTrue).ParamTransition<int>((StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.Parameter<int>) this.destinationWorld, (GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State) this.takeoff, (StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.Parameter<int>.Callback) ((smi, p) => p != -1)).Enter((StateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State.Callback) (smi => smi.MoveToWorld()));
    this.landing.landing.PlayAnim("falling", (KAnim.PlayMode) 0).UpdateTransition(this.landing.impact, (Func<RailGunPayload.StatesInstance, float, bool>) ((smi, dt) => smi.UpdateLanding(dt))).ToggleGravity(this.landing.impact);
    this.landing.impact.PlayAnim("land").TriggerOnEnter(GameHashes.JettisonCargo).OnAnimQueueComplete(this.grounded.crater);
  }

  public class Def : StateMachine.BaseDef
  {
    public bool attractToBeacons;
    public string clusterAnimSymbolSwapTarget;
    public List<string> randomClusterSymbolSwaps;
    public string worldAnimSymbolSwapTarget;
    public List<string> randomWorldSymbolSwaps;
  }

  public class TakeoffStates : 
    GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State
  {
    public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State launch;
    public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State airborne;
  }

  public class TravelStates : 
    GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State
  {
    public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State travelling;
    public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State transferWorlds;
  }

  public class LandingStates : 
    GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State
  {
    public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State landing;
    public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State impact;
  }

  public class GroundedStates : 
    GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State
  {
    public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State crater;
    public GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.State idle;
  }

  public class StatesInstance : 
    GameStateMachine<RailGunPayload, RailGunPayload.StatesInstance, IStateMachineTarget, RailGunPayload.Def>.GameInstance
  {
    [Serialize]
    public float takeoffVelocity;
    [Serialize]
    private int randomSymbolSwapIndex = -1;
    public KBatchedAnimController animController;

    public StatesInstance(IStateMachineTarget master, RailGunPayload.Def def)
      : base(master, def)
    {
      this.animController = this.GetComponent<KBatchedAnimController>();
      DebugUtil.Assert(def.clusterAnimSymbolSwapTarget == null == (def.worldAnimSymbolSwapTarget == null), "Must specify both or neither symbol swap targets!");
      DebugUtil.Assert(def.randomClusterSymbolSwaps == null && def.randomWorldSymbolSwaps == null || def.randomClusterSymbolSwaps.Count == def.randomWorldSymbolSwaps.Count, "Must specify the same number of swaps for both world and cluster!");
      if (def.clusterAnimSymbolSwapTarget == null || def.worldAnimSymbolSwapTarget == null)
        return;
      if (this.randomSymbolSwapIndex == -1)
      {
        this.randomSymbolSwapIndex = Random.Range(0, def.randomClusterSymbolSwaps.Count);
        Debug.Log((object) string.Format("Rolling a random symbol: {0}", (object) this.randomSymbolSwapIndex), (Object) this.gameObject);
      }
      this.GetComponent<BallisticClusterGridEntity>().SwapSymbolFromSameAnim(def.clusterAnimSymbolSwapTarget, def.randomClusterSymbolSwaps[this.randomSymbolSwapIndex]);
      KAnim.Build.Symbol symbol = this.animController.AnimFiles[0].GetData().build.GetSymbol(KAnimHashedString.op_Implicit(def.randomWorldSymbolSwaps[this.randomSymbolSwapIndex]));
      ((Component) this.animController).GetComponent<SymbolOverrideController>().AddSymbolOverride(HashedString.op_Implicit(def.worldAnimSymbolSwapTarget), symbol);
    }

    public void Launch(AxialI source, AxialI destination)
    {
      this.GetComponent<BallisticClusterGridEntity>().Configure(source, destination);
      this.sm.destinationWorld.Set(ClusterUtil.GetAsteroidWorldIdAtLocation(destination), this);
      this.GoTo((StateMachine.BaseState) this.sm.takeoff);
    }

    public void Travel(AxialI source, AxialI destination)
    {
      this.GetComponent<BallisticClusterGridEntity>().Configure(source, destination);
      this.sm.destinationWorld.Set(ClusterUtil.GetAsteroidWorldIdAtLocation(destination), this);
      this.GoTo((StateMachine.BaseState) this.sm.travel);
    }

    public void StartTakeoff()
    {
      if (!((KComponentManager<FallerComponent>) GameComps.Fallers).Has((object) this.gameObject))
        return;
      ((KGameObjectComponentManager<FallerComponent>) GameComps.Fallers).Remove(this.gameObject);
    }

    public void StartLand()
    {
      WorldContainer world = ClusterManager.Instance.GetWorld(this.sm.destinationWorld.Get(this));
      int cell = Grid.InvalidCell;
      if (this.def.attractToBeacons)
        cell = ClusterManager.Instance.GetLandingBeaconLocation(world.id);
      int num;
      if (cell != Grid.InvalidCell)
      {
        int x;
        Grid.CellToXY(cell, out x, out int _);
        num = Mathf.RoundToInt((float) Random.Range(Mathf.Max(x - 3, (int) world.minimumBounds.x), Mathf.Min(x + 3, (int) world.maximumBounds.x)));
      }
      else
        num = Mathf.RoundToInt(Random.Range(world.minimumBounds.x + 3f, world.maximumBounds.x - 3f));
      Vector3 vector3;
      // ISSUE: explicit constructor call
      ((Vector3) ref vector3).\u002Ector((float) num + 0.5f, world.maximumBounds.y - 1f, Grid.GetLayerZ(Grid.SceneLayer.Front));
      TransformExtensions.SetPosition(this.transform, vector3);
      if (((KComponentManager<FallerComponent>) GameComps.Fallers).Has((object) this.gameObject))
        ((KGameObjectComponentManager<FallerComponent>) GameComps.Fallers).Remove(this.gameObject);
      GameComps.Fallers.Add(this.gameObject, new Vector2(0.0f, -10f));
      this.sm.destinationWorld.Set(-1, this);
    }

    public void UpdateLaunch(float dt)
    {
      if (Object.op_Inequality((Object) this.gameObject.GetMyWorld(), (Object) null))
      {
        TransformExtensions.SetPosition(this.transform, Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), new Vector3(0.0f, this.takeoffVelocity * dt, 0.0f)));
      }
      else
      {
        this.sm.beginTravelling.Trigger(this);
        ClusterGridEntity component = this.GetComponent<ClusterGridEntity>();
        if (!Object.op_Inequality((Object) ClusterGrid.Instance.GetAsteroidAtCell(component.Location), (Object) null))
          return;
        this.GetComponent<ClusterTraveler>().AdvancePathOneStep();
      }
    }

    public bool UpdateLanding(float dt)
    {
      if (Object.op_Inequality((Object) this.gameObject.GetMyWorld(), (Object) null))
      {
        Vector3 position = TransformExtensions.GetPosition(this.transform);
        position.y -= 0.5f;
        int cell = Grid.PosToCell(position);
        if (Grid.IsWorldValidCell(cell) && Grid.IsSolidCell(cell))
          return true;
      }
      return false;
    }

    public void OnDroppedAll() => TracesExtesions.DeleteObject(this.gameObject);

    public bool IsTraveling() => this.IsInsideState((StateMachine.BaseState) this.sm.travel.travelling);

    public void MoveToSpace()
    {
      Pickupable component = this.GetComponent<Pickupable>();
      if (Object.op_Inequality((Object) component, (Object) null))
        component.deleteOffGrid = false;
      TransformExtensions.SetPosition(this.gameObject.transform, new Vector3(-1f, -1f, 0.0f));
    }

    public void MoveToWorld()
    {
      Pickupable component = this.GetComponent<Pickupable>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.deleteOffGrid = true;
    }
  }
}
