// Decompiled with JetBrains decompiler
// Type: CometDetector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System.Collections.Generic;
using UnityEngine;

public class CometDetector : 
  GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>
{
  public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State off;
  public CometDetector.OnStates on;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.off;
    this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, (GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State) this.on, (StateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational)).Update("Scan Sky", (System.Action<CometDetector.Instance, float>) ((smi, dt) => smi.ScanSky(false)), (UpdateRate) 7);
    this.on.DefaultState(this.on.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.DetectorScanning).Enter("ToggleActive", (StateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(true))).Exit("ToggleActive", (StateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(false)));
    this.on.pre.PlayAnim("on_pre").OnAnimQueueComplete(this.on.loop);
    this.on.loop.PlayAnim("on", (KAnim.PlayMode) 0).EventTransition(GameHashes.OperationalChanged, this.on.pst, (StateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational)).TagTransition(GameTags.Detecting, (GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State) this.on.working).Enter("UpdateLogic", (StateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State.Callback) (smi => smi.UpdateDetectionState(smi.HasTag(GameTags.Detecting), false))).Update("Scan Sky", (System.Action<CometDetector.Instance, float>) ((smi, dt) => smi.ScanSky(false)));
    this.on.pst.PlayAnim("on_pst").OnAnimQueueComplete(this.off);
    this.on.working.DefaultState(this.on.working.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.IncomingMeteors).Enter("UpdateLogic", (StateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State.Callback) (smi => smi.SetLogicSignal(true))).Exit("UpdateLogic", (StateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State.Callback) (smi => smi.SetLogicSignal(false))).Update("Scan Sky", (System.Action<CometDetector.Instance, float>) ((smi, dt) => smi.ScanSky(true)));
    this.on.working.pre.PlayAnim("detect_pre").OnAnimQueueComplete(this.on.working.loop);
    this.on.working.loop.PlayAnim("detect_loop", (KAnim.PlayMode) 0).EventTransition(GameHashes.OperationalChanged, this.on.working.pst, (StateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational)).EventTransition(GameHashes.ActiveChanged, this.on.working.pst, (StateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsActive)).TagTransition(GameTags.Detecting, this.on.working.pst, true);
    this.on.working.pst.PlayAnim("detect_pst").OnAnimQueueComplete(this.on.loop).Enter("Reroll", (StateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State.Callback) (smi => smi.RerollAccuracy()));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class OnStates : 
    GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State
  {
    public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State pre;
    public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State loop;
    public CometDetector.WorkingStates working;
    public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State pst;
  }

  public class WorkingStates : 
    GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State
  {
    public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State pre;
    public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State loop;
    public GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.State pst;
  }

  public new class Instance : 
    GameStateMachine<CometDetector, CometDetector.Instance, IStateMachineTarget, CometDetector.Def>.GameInstance
  {
    public bool ShowWorkingStatus;
    private const float BEST_WARNING_TIME = 200f;
    private const float WORST_WARNING_TIME = 1f;
    private const float VARIANCE = 50f;
    private const int MAX_DISH_COUNT = 6;
    private const int INTERFERENCE_RADIUS = 15;
    [Serialize]
    private float nextAccuracy;
    [Serialize]
    private Ref<LaunchConditionManager> targetCraft;
    private DetectorNetwork.Def detectorNetworkDef;
    private DetectorNetwork.Instance detectorNetwork;
    private List<GameplayEventInstance> meteorShowers = new List<GameplayEventInstance>();

    public Instance(IStateMachineTarget master, CometDetector.Def def)
      : base(master, def)
    {
      this.detectorNetworkDef = new DetectorNetwork.Def();
      this.detectorNetworkDef.interferenceRadius = 15;
      this.detectorNetworkDef.worstWarningTime = 1f;
      this.detectorNetworkDef.bestWarningTime = 200f;
      this.detectorNetworkDef.bestNetworkSize = 6;
      this.targetCraft = new Ref<LaunchConditionManager>();
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
      if (Object.op_Equality((Object) this.targetCraft.Get(), (Object) null))
      {
        ((Component) SaveGame.Instance).GetComponent<GameplayEventManager>().GetActiveEventsOfType<MeteorShowerEvent>(this.GetMyWorldId(), ref this.meteorShowers);
        float num = float.MaxValue;
        foreach (GameplayEventInstance meteorShower in this.meteorShowers)
        {
          if (meteorShower.smi is MeteorShowerEvent.StatesInstance smi)
            num = Mathf.Min(num, smi.TimeUntilNextShower());
        }
        this.meteorShowers.Clear();
        this.UpdateDetectionState((double) num < (double) detectTime, expectedDetectionForState);
      }
      else
      {
        Spacecraft conditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.targetCraft.Get());
        if (conditionManager.state == Spacecraft.MissionState.Destroyed)
        {
          this.targetCraft.Set((LaunchConditionManager) null);
          this.UpdateDetectionState(false, expectedDetectionForState);
        }
        else if (conditionManager.state == Spacecraft.MissionState.Launching || conditionManager.state == Spacecraft.MissionState.WaitingToLand || conditionManager.state == Spacecraft.MissionState.Landing || conditionManager.state == Spacecraft.MissionState.Underway && (double) conditionManager.GetTimeLeft() <= (double) detectTime)
          this.UpdateDetectionState(true, expectedDetectionForState);
        else
          this.UpdateDetectionState(false, expectedDetectionForState);
      }
    }

    public void RerollAccuracy() => this.nextAccuracy = Random.value;

    public void SetLogicSignal(bool on) => this.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, on ? 1 : 0);

    public float GetDetectTime()
    {
      MathUtil.MinMax detectTimeRange = this.detectorNetwork.GetDetectTimeRange();
      return ((MathUtil.MinMax) ref detectTimeRange).Lerp(this.nextAccuracy);
    }

    public void SetTargetCraft(LaunchConditionManager target) => this.targetCraft.Set(target);

    public LaunchConditionManager GetTargetCraft() => this.targetCraft.Get();
  }
}
