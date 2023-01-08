// Decompiled with JetBrains decompiler
// Type: ClusterCometDetector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System.Collections.Generic;
using UnityEngine;

public class ClusterCometDetector : 
  GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>
{
  public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State off;
  public ClusterCometDetector.OnStates on;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.off;
    this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, (GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State) this.on, (StateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational)).Update("Scan Sky", (System.Action<ClusterCometDetector.Instance, float>) ((smi, dt) => smi.ScanSky(false)), (UpdateRate) 7);
    this.on.DefaultState(this.on.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.DetectorScanning).Enter("ToggleActive", (StateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(true))).Exit("ToggleActive", (StateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(false)));
    this.on.pre.PlayAnim("on_pre").OnAnimQueueComplete(this.on.loop);
    this.on.loop.PlayAnim("on", (KAnim.PlayMode) 0).EventTransition(GameHashes.OperationalChanged, this.on.pst, (StateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational)).TagTransition(GameTags.Detecting, (GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State) this.on.working).Enter("UpdateLogic", (StateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State.Callback) (smi => smi.UpdateDetectionState(smi.HasTag(GameTags.Detecting), false))).Update("Scan Sky", (System.Action<ClusterCometDetector.Instance, float>) ((smi, dt) => smi.ScanSky(false)));
    this.on.pst.PlayAnim("on_pst").OnAnimQueueComplete(this.off);
    this.on.working.DefaultState(this.on.working.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.IncomingMeteors).Enter("UpdateLogic", (StateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State.Callback) (smi => smi.SetLogicSignal(true))).Exit("UpdateLogic", (StateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State.Callback) (smi => smi.SetLogicSignal(false))).Update("Scan Sky", (System.Action<ClusterCometDetector.Instance, float>) ((smi, dt) => smi.ScanSky(true)));
    this.on.working.pre.PlayAnim("detect_pre").OnAnimQueueComplete(this.on.working.loop);
    this.on.working.loop.PlayAnim("detect_loop", (KAnim.PlayMode) 0).EventTransition(GameHashes.OperationalChanged, this.on.working.pst, (StateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational)).EventTransition(GameHashes.ActiveChanged, this.on.working.pst, (StateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsActive)).TagTransition(GameTags.Detecting, this.on.working.pst, true);
    this.on.working.pst.PlayAnim("detect_pst").OnAnimQueueComplete(this.on.loop).Enter("Reroll", (StateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State.Callback) (smi => smi.RerollAccuracy()));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class OnStates : 
    GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State
  {
    public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State pre;
    public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State loop;
    public ClusterCometDetector.WorkingStates working;
    public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State pst;
  }

  public class WorkingStates : 
    GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State
  {
    public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State pre;
    public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State loop;
    public GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.State pst;
  }

  public new class Instance : 
    GameStateMachine<ClusterCometDetector, ClusterCometDetector.Instance, IStateMachineTarget, ClusterCometDetector.Def>.GameInstance
  {
    public bool ShowWorkingStatus;
    private const float BEST_WARNING_TIME = 200f;
    private const float WORST_WARNING_TIME = 1f;
    private const float VARIANCE = 50f;
    private const int MAX_DISH_COUNT = 6;
    private const int INTERFERENCE_RADIUS = 15;
    [Serialize]
    private ClusterCometDetector.Instance.ClusterCometDetectorState detectorState;
    [Serialize]
    private float nextAccuracy;
    [Serialize]
    private Ref<Clustercraft> targetCraft;
    private DetectorNetwork.Def detectorNetworkDef;
    private DetectorNetwork.Instance detectorNetwork;
    private List<GameplayEventInstance> meteorShowers = new List<GameplayEventInstance>();

    public Instance(IStateMachineTarget master, ClusterCometDetector.Def def)
      : base(master, def)
    {
      this.detectorNetworkDef = new DetectorNetwork.Def();
      this.detectorNetworkDef.interferenceRadius = 15;
      this.detectorNetworkDef.worstWarningTime = 1f;
      this.detectorNetworkDef.bestWarningTime = 200f;
      this.detectorNetworkDef.bestNetworkSize = 6;
      this.RerollAccuracy();
    }

    public override void StartSM()
    {
      if (this.detectorNetwork == null)
        this.detectorNetwork = (DetectorNetwork.Instance) this.detectorNetworkDef.CreateSMI(this.master);
      this.detectorNetwork.StartSM();
      base.StartSM();
    }

    public override void StopSM(string reason)
    {
      base.StopSM(reason);
      this.detectorNetwork.StopSM(reason);
    }

    public void UpdateDetectionState(bool currentDetection, bool expectedDetectionForState)
    {
      KPrefabID component = this.GetComponent<KPrefabID>();
      if (currentDetection)
        component.AddTag(GameTags.Detecting, false);
      else
        component.RemoveTag(GameTags.Detecting);
      if (currentDetection != expectedDetectionForState)
        return;
      this.SetLogicSignal(currentDetection);
    }

    public void ScanSky(bool expectedDetectionForState)
    {
      float detectTime = this.GetDetectTime();
      int myWorldId = this.GetMyWorldId();
      if (this.GetDetectorState() == ClusterCometDetector.Instance.ClusterCometDetectorState.MeteorShower)
      {
        ((Component) SaveGame.Instance).GetComponent<GameplayEventManager>().GetActiveEventsOfType<MeteorShowerEvent>(myWorldId, ref this.meteorShowers);
        float num = float.MaxValue;
        foreach (GameplayEventInstance meteorShower in this.meteorShowers)
        {
          if (meteorShower.smi is MeteorShowerEvent.StatesInstance smi)
            num = Mathf.Min(num, smi.TimeUntilNextShower());
        }
        this.meteorShowers.Clear();
        this.UpdateDetectionState((double) num < (double) detectTime, expectedDetectionForState);
      }
      if (this.GetDetectorState() == ClusterCometDetector.Instance.ClusterCometDetectorState.BallisticObject)
      {
        float num1 = float.MaxValue;
        foreach (ClusterTraveler clusterTraveler in Components.ClusterTravelers)
        {
          int num2 = clusterTraveler.IsTraveling() ? 1 : 0;
          bool flag = Object.op_Inequality((Object) ((Component) clusterTraveler).GetComponent<Clustercraft>(), (Object) null);
          if (num2 != 0 && !flag && clusterTraveler.GetDestinationWorldID() == myWorldId)
            num1 = Mathf.Min(num1, clusterTraveler.TravelETA());
        }
        this.UpdateDetectionState((double) num1 < (double) detectTime, expectedDetectionForState);
      }
      if (this.GetDetectorState() != ClusterCometDetector.Instance.ClusterCometDetectorState.Rocket || this.targetCraft == null)
        return;
      Clustercraft clustercraft = this.targetCraft.Get();
      if (Util.IsNullOrDestroyed((object) clustercraft))
        return;
      ClusterTraveler component = ((Component) clustercraft).GetComponent<ClusterTraveler>();
      bool currentDetection = false;
      if (clustercraft.Status != Clustercraft.CraftStatus.Grounded)
      {
        bool flag1 = component.GetDestinationWorldID() == myWorldId;
        bool flag2 = component.IsTraveling();
        bool move = clustercraft.HasResourcesToMove();
        float num = component.TravelETA();
        currentDetection = flag1 & flag2 & move && (double) num < (double) detectTime || !flag2 & flag1 && clustercraft.Status == Clustercraft.CraftStatus.Landing;
        if (!currentDetection)
        {
          ClusterGridEntity adjacentAsteroid = clustercraft.GetAdjacentAsteroid();
          currentDetection = (Object.op_Inequality((Object) adjacentAsteroid, (Object) null) ? ClusterUtil.GetAsteroidWorldIdAtLocation(adjacentAsteroid.Location) : (int) ClusterManager.INVALID_WORLD_IDX) == myWorldId && clustercraft.Status == Clustercraft.CraftStatus.Launching;
        }
      }
      this.UpdateDetectionState(currentDetection, expectedDetectionForState);
    }

    public void RerollAccuracy() => this.nextAccuracy = Random.value;

    public void SetLogicSignal(bool on) => this.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, on ? 1 : 0);

    public float GetDetectTime()
    {
      MathUtil.MinMax detectTimeRange = this.detectorNetwork.GetDetectTimeRange();
      return ((MathUtil.MinMax) ref detectTimeRange).Lerp(this.nextAccuracy);
    }

    public void SetDetectorState(
      ClusterCometDetector.Instance.ClusterCometDetectorState newState)
    {
      this.detectorState = newState;
    }

    public ClusterCometDetector.Instance.ClusterCometDetectorState GetDetectorState() => this.detectorState;

    public void SetClustercraftTarget(Clustercraft target)
    {
      if (Object.op_Implicit((Object) target))
        this.targetCraft = new Ref<Clustercraft>(target);
      else
        this.targetCraft = (Ref<Clustercraft>) null;
    }

    public Clustercraft GetClustercraftTarget() => this.targetCraft?.Get();

    public enum ClusterCometDetectorState
    {
      MeteorShower,
      BallisticObject,
      Rocket,
    }
  }
}
