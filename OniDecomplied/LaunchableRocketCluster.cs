// Decompiled with JetBrains decompiler
// Type: LaunchableRocketCluster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class LaunchableRocketCluster : 
  StateMachineComponent<LaunchableRocketCluster.StatesInstance>,
  ILaunchableRocket
{
  [Serialize]
  private int takeOffLocation;
  private GameObject soundSpeakerObject;

  public IList<Ref<RocketModuleCluster>> parts => ((Component) this).GetComponent<RocketModuleCluster>().CraftInterface.ClusterModules;

  public bool isLanding { get; private set; }

  public float rocketSpeed { get; private set; }

  public LaunchableRocketRegisterType registerType => LaunchableRocketRegisterType.Clustercraft;

  public GameObject LaunchableGameObject => ((Component) this).gameObject;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  public List<GameObject> GetEngines()
  {
    List<GameObject> engines = new List<GameObject>();
    foreach (Ref<RocketModuleCluster> part in (IEnumerable<Ref<RocketModuleCluster>>) this.parts)
    {
      if (Object.op_Implicit((Object) ((Component) part.Get()).GetComponent<RocketEngineCluster>()))
        engines.Add(((Component) part.Get()).gameObject);
    }
    return engines;
  }

  private int GetRocketHeight()
  {
    int rocketHeight = 0;
    foreach (Ref<RocketModuleCluster> part in (IEnumerable<Ref<RocketModuleCluster>>) this.parts)
      rocketHeight += ((Component) part.Get()).GetComponent<Building>().Def.HeightInCells;
    return rocketHeight;
  }

  private float InitialFlightAnimOffsetForLanding()
  {
    int cell = Grid.PosToCell(((Component) this).gameObject);
    return (float) ((double) ClusterManager.Instance.GetWorld((int) Grid.WorldIdx[cell]).maximumBounds.y - (double) TransformExtensions.GetPosition(((Component) this).gameObject.transform).y + (double) this.GetRocketHeight() + 100.0);
  }

  public class StatesInstance : 
    GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.GameInstance
  {
    private float takeoffAccelPowerInv;
    private float constantVelocityPhase_maxSpeed;

    private float heightLaunchSpeedRatio => Mathf.Pow((float) this.master.GetRocketHeight(), TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().heightSpeedPower) * TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().heightSpeedFactor;

    public float DistanceAboveGround
    {
      get => this.sm.distanceAboveGround.Get(this);
      set
      {
        double num = (double) this.sm.distanceAboveGround.Set(value, this);
      }
    }

    public StatesInstance(LaunchableRocketCluster master)
      : base(master)
    {
      this.takeoffAccelPowerInv = 1f / TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower;
    }

    public void SetMissionState(Spacecraft.MissionState state)
    {
      Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
      SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(((Component) this.master).GetComponent<LaunchConditionManager>()).SetState(state);
    }

    public Spacecraft.MissionState GetMissionState()
    {
      Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
      return SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(((Component) this.master).GetComponent<LaunchConditionManager>()).state;
    }

    public bool IsGrounded() => ((Component) ((Component) this.smi.master).GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>().Status == Clustercraft.CraftStatus.Grounded;

    public bool IsNotSpaceBound()
    {
      Clustercraft component = ((Component) ((Component) this.smi.master).GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>();
      return component.Status == Clustercraft.CraftStatus.Grounded || component.Status == Clustercraft.CraftStatus.Landing;
    }

    public bool IsNotGroundBound()
    {
      Clustercraft component = ((Component) ((Component) this.smi.master).GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>();
      return component.Status == Clustercraft.CraftStatus.Launching || component.Status == Clustercraft.CraftStatus.InFlight;
    }

    public void SetupLaunch()
    {
      this.master.isLanding = false;
      this.master.rocketSpeed = 0.0f;
      double num1 = (double) this.sm.warmupTimeRemaining.Set(5f, this);
      double num2 = (double) this.sm.distanceAboveGround.Set(0.0f, this);
      if (Object.op_Equality((Object) this.master.soundSpeakerObject, (Object) null))
      {
        this.master.soundSpeakerObject = new GameObject("rocketSpeaker");
        this.master.soundSpeakerObject.transform.SetParent(((Component) this.master).gameObject.transform);
      }
      foreach (Ref<RocketModuleCluster> part in (IEnumerable<Ref<RocketModuleCluster>>) this.master.parts)
      {
        if (part != null)
        {
          this.master.takeOffLocation = Grid.PosToCell(((Component) this.master).gameObject);
          part.Get().Trigger(-1277991738, (object) ((Component) this.master).gameObject);
        }
      }
      CraftModuleInterface craftInterface = ((Component) this.master).GetComponent<RocketModuleCluster>().CraftInterface;
      if (Object.op_Inequality((Object) craftInterface, (Object) null))
      {
        craftInterface.Trigger(-1277991738, (object) ((Component) this.master).gameObject);
        WorldContainer component = ((Component) craftInterface).GetComponent<WorldContainer>();
        List<MinionIdentity> worldItems = Components.MinionIdentities.GetWorldItems(component.id);
        MinionMigrationEventArgs migrationEventArgs = new MinionMigrationEventArgs()
        {
          prevWorldId = component.id,
          targetWorldId = component.id
        };
        foreach (MinionIdentity minionIdentity in worldItems)
        {
          migrationEventArgs.minionId = minionIdentity;
          Game.Instance.Trigger(586301400, (object) migrationEventArgs);
        }
      }
      Game.Instance.Trigger(-1277991738, (object) this.gameObject);
      this.constantVelocityPhase_maxSpeed = 0.0f;
    }

    public void LaunchLoop(float dt)
    {
      this.master.isLanding = false;
      if ((double) this.constantVelocityPhase_maxSpeed == 0.0)
      {
        float num = Mathf.Pow((float) ((double) Mathf.Pow(TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().maxAccelerationDistance, this.takeoffAccelPowerInv) * (double) this.heightLaunchSpeedRatio - 0.032999999821186066) / this.heightLaunchSpeedRatio, TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower);
        this.constantVelocityPhase_maxSpeed = (float) (((double) TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().maxAccelerationDistance - (double) num) / 0.032999999821186066);
      }
      if ((double) this.sm.warmupTimeRemaining.Get(this) > 0.0)
      {
        double num1 = (double) this.sm.warmupTimeRemaining.Delta(-dt, this);
      }
      else if ((double) this.DistanceAboveGround < (double) TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().maxAccelerationDistance)
      {
        float num2 = Mathf.Pow(this.DistanceAboveGround, this.takeoffAccelPowerInv) * this.heightLaunchSpeedRatio + dt;
        this.DistanceAboveGround = Mathf.Pow(num2 / this.heightLaunchSpeedRatio, TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower);
        this.master.rocketSpeed = (float) (((double) this.DistanceAboveGround - (double) Mathf.Pow((num2 - 0.033f) / this.heightLaunchSpeedRatio, TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower)) / 0.032999999821186066);
      }
      else
      {
        this.master.rocketSpeed = this.constantVelocityPhase_maxSpeed;
        this.DistanceAboveGround += this.master.rocketSpeed * dt;
      }
      this.UpdateSoundSpeakerObject();
      if (this.UpdatePartsAnimPositionsAndDamage() != 0)
        return;
      this.smi.GoTo((StateMachine.BaseState) this.sm.not_grounded.space);
    }

    public void FinalizeLaunch()
    {
      this.master.rocketSpeed = 0.0f;
      this.DistanceAboveGround = this.sm.distanceToSpace.Get(this.smi);
      foreach (Ref<RocketModuleCluster> part in (IEnumerable<Ref<RocketModuleCluster>>) this.master.parts)
      {
        if (part != null && !Object.op_Equality((Object) part.Get(), (Object) null))
        {
          RocketModuleCluster rocketModuleCluster = part.Get();
          ((Component) rocketModuleCluster).GetComponent<KBatchedAnimController>().Offset = Vector3.op_Multiply(Vector3.up, this.DistanceAboveGround);
          ((Component) rocketModuleCluster).GetComponent<KBatchedAnimController>().enabled = false;
          ((Component) rocketModuleCluster).GetComponent<RocketModule>().MoveToSpace();
        }
      }
      ((Component) ((Component) this.smi.master).GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>().SetCraftStatus(Clustercraft.CraftStatus.InFlight);
    }

    public void SetupLanding()
    {
      this.DistanceAboveGround = this.master.InitialFlightAnimOffsetForLanding();
      double num = (double) this.sm.warmupTimeRemaining.Set(2f, this);
      this.master.isLanding = true;
      this.master.rocketSpeed = 0.0f;
      this.constantVelocityPhase_maxSpeed = 0.0f;
    }

    public void LandingLoop(float dt)
    {
      this.master.isLanding = true;
      if ((double) this.constantVelocityPhase_maxSpeed == 0.0)
        this.constantVelocityPhase_maxSpeed = (float) (((double) Mathf.Pow((float) ((double) Mathf.Pow(TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().maxAccelerationDistance, this.takeoffAccelPowerInv) * (double) this.heightLaunchSpeedRatio - 0.032999999821186066) / this.heightLaunchSpeedRatio, TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower) - (double) TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().maxAccelerationDistance) / 0.032999999821186066);
      if ((double) this.DistanceAboveGround > (double) TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().maxAccelerationDistance)
      {
        this.master.rocketSpeed = this.constantVelocityPhase_maxSpeed;
        this.DistanceAboveGround += this.master.rocketSpeed * dt;
      }
      else if ((double) this.DistanceAboveGround > 1.0 / 400.0)
      {
        float num = Mathf.Pow(this.DistanceAboveGround, this.takeoffAccelPowerInv) * this.heightLaunchSpeedRatio - dt;
        this.DistanceAboveGround = Mathf.Pow(num / this.heightLaunchSpeedRatio, TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower);
        this.master.rocketSpeed = (float) (((double) this.DistanceAboveGround - (double) Mathf.Pow((num - 0.033f) / this.heightLaunchSpeedRatio, TuningData<LaunchableRocketCluster.StatesInstance.Tuning>.Get().takeoffAccelPower)) / 0.032999999821186066);
      }
      else if ((double) this.sm.warmupTimeRemaining.Get(this) > 0.0)
      {
        double num = (double) this.sm.warmupTimeRemaining.Delta(-dt, this);
        this.DistanceAboveGround = 0.0f;
      }
      this.UpdateSoundSpeakerObject();
      this.UpdatePartsAnimPositionsAndDamage();
    }

    public void FinalizeLanding()
    {
      this.GetComponent<KSelectable>().IsSelectable = true;
      this.master.rocketSpeed = 0.0f;
      this.DistanceAboveGround = 0.0f;
      foreach (Ref<RocketModuleCluster> part in (IEnumerable<Ref<RocketModuleCluster>>) this.smi.master.parts)
      {
        if (part != null && !Object.op_Equality((Object) part.Get(), (Object) null))
          ((Component) part.Get()).GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
      }
      ((Component) ((Component) this.smi.master).GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>().SetCraftStatus(Clustercraft.CraftStatus.Grounded);
    }

    private void UpdateSoundSpeakerObject()
    {
      if (Object.op_Equality((Object) this.master.soundSpeakerObject, (Object) null))
      {
        this.master.soundSpeakerObject = new GameObject("rocketSpeaker");
        this.master.soundSpeakerObject.transform.SetParent(this.gameObject.transform);
      }
      TransformExtensions.SetLocalPosition(this.master.soundSpeakerObject.transform, Vector3.op_Multiply(this.DistanceAboveGround, Vector3.up));
    }

    public int UpdatePartsAnimPositionsAndDamage(bool doDamage = true)
    {
      int myWorldId = this.gameObject.GetMyWorldId();
      if (myWorldId == -1)
        return 0;
      LaunchPad currentPad = ((Component) this.smi.master).GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad;
      if (Object.op_Inequality((Object) currentPad, (Object) null))
        myWorldId = currentPad.GetMyWorldId();
      int num = 0;
      foreach (Ref<RocketModuleCluster> part in (IEnumerable<Ref<RocketModuleCluster>>) this.master.parts)
      {
        if (part != null)
        {
          RocketModuleCluster rocketModuleCluster = part.Get();
          KBatchedAnimController component = ((Component) rocketModuleCluster).GetComponent<KBatchedAnimController>();
          component.Offset = Vector3.op_Multiply(Vector3.up, this.DistanceAboveGround);
          Vector3 positionIncludingOffset = component.PositionIncludingOffset;
          int cell = Grid.PosToCell(positionIncludingOffset);
          bool flag1 = Grid.IsValidCell(cell);
          bool flag2 = flag1 && (int) Grid.WorldIdx[cell] == myWorldId;
          if (component.enabled != flag2)
            component.enabled = flag2;
          if (doDamage & flag1)
          {
            ++num;
            LaunchableRocketCluster.States.DoWorldDamage(((Component) rocketModuleCluster).gameObject, positionIncludingOffset, myWorldId);
          }
        }
      }
      return num;
    }

    public class Tuning : TuningData<LaunchableRocketCluster.StatesInstance.Tuning>
    {
      public float takeoffAccelPower = 4f;
      public float maxAccelerationDistance = 25f;
      public float warmupTime = 5f;
      public float heightSpeedPower = 0.5f;
      public float heightSpeedFactor = 4f;
      public int maxAccelHeight = 40;
    }
  }

  public class States : 
    GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster>
  {
    public StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.FloatParameter warmupTimeRemaining;
    public StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.FloatParameter distanceAboveGround;
    public StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.FloatParameter distanceToSpace;
    public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State grounded;
    public LaunchableRocketCluster.States.NotGroundedStates not_grounded;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.grounded;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.grounded.EventTransition(GameHashes.DoLaunchRocket, this.not_grounded.launch_setup).EnterTransition(this.not_grounded.launch_loop, (StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.Transition.ConditionCallback) (smi => smi.IsNotGroundBound())).Enter((StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State.Callback) (smi => smi.FinalizeLanding()));
      this.not_grounded.launch_setup.Enter((StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State.Callback) (smi =>
      {
        smi.SetupLaunch();
        double num = (double) this.distanceToSpace.Set((float) ConditionFlightPathIsClear.PadTopEdgeDistanceToCeilingEdge(((Component) ((Component) smi.master).gameObject.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad).gameObject), smi);
        smi.GoTo((StateMachine.BaseState) this.not_grounded.launch_loop);
      }));
      this.not_grounded.launch_loop.EventTransition(GameHashes.DoReturnRocket, this.not_grounded.landing_setup).Enter((StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State.Callback) (smi => smi.UpdatePartsAnimPositionsAndDamage(false))).Update((Action<LaunchableRocketCluster.StatesInstance, float>) ((smi, dt) => smi.LaunchLoop(dt)), (UpdateRate) 3).ParamTransition<float>((StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.Parameter<float>) this.distanceAboveGround, this.not_grounded.space, (StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.Parameter<float>.Callback) ((smi, p) => (double) p >= (double) this.distanceToSpace.Get(smi))).TriggerOnEnter(GameHashes.StartRocketLaunch).Exit((StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State.Callback) (smi => smi.gameObject.GetMyWorld().RevealSurface()));
      this.not_grounded.space.EnterTransition(this.not_grounded.landing_setup, (StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.Transition.ConditionCallback) (smi => smi.IsNotSpaceBound())).EventTransition(GameHashes.DoReturnRocket, this.not_grounded.landing_setup).Enter((StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State.Callback) (smi => smi.FinalizeLaunch()));
      this.not_grounded.landing_setup.Enter((StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State.Callback) (smi =>
      {
        smi.SetupLanding();
        smi.GoTo((StateMachine.BaseState) this.not_grounded.landing_loop);
      }));
      this.not_grounded.landing_loop.Enter((StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State.Callback) (smi => smi.UpdatePartsAnimPositionsAndDamage(false))).Update((Action<LaunchableRocketCluster.StatesInstance, float>) ((smi, dt) => smi.LandingLoop(dt)), (UpdateRate) 3).ParamTransition<float>((StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.Parameter<float>) this.distanceAboveGround, this.not_grounded.land, new StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.Parameter<float>.Callback(this.IsFullyLanded<float>)).ParamTransition<float>((StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.Parameter<float>) this.warmupTimeRemaining, this.not_grounded.land, new StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.Parameter<float>.Callback(this.IsFullyLanded<float>));
      this.not_grounded.land.TriggerOnEnter(GameHashes.RocketTouchDown).Enter((StateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State.Callback) (smi =>
      {
        foreach (Ref<RocketModuleCluster> part in (IEnumerable<Ref<RocketModuleCluster>>) smi.master.parts)
        {
          if (part != null && !Object.op_Equality((Object) part.Get(), (Object) null))
            part.Get().Trigger(-887025858, (object) smi.gameObject);
        }
        CraftModuleInterface craftInterface = ((Component) smi.master).GetComponent<RocketModuleCluster>().CraftInterface;
        if (Object.op_Inequality((Object) craftInterface, (Object) null))
        {
          craftInterface.Trigger(-887025858, (object) smi.gameObject);
          WorldContainer component = ((Component) craftInterface).GetComponent<WorldContainer>();
          List<MinionIdentity> worldItems = Components.MinionIdentities.GetWorldItems(component.id);
          MinionMigrationEventArgs migrationEventArgs = new MinionMigrationEventArgs()
          {
            prevWorldId = component.id,
            targetWorldId = component.id
          };
          foreach (MinionIdentity minionIdentity in worldItems)
          {
            migrationEventArgs.minionId = minionIdentity;
            Game.Instance.Trigger(586301400, (object) migrationEventArgs);
          }
        }
        Game.Instance.Trigger(-887025858, (object) smi.gameObject);
        smi.GoTo((StateMachine.BaseState) this.grounded);
      }));
    }

    public bool IsFullyLanded<T>(LaunchableRocketCluster.StatesInstance smi, T p) => (double) this.distanceAboveGround.Get(smi) <= 1.0 / 400.0 && (double) this.warmupTimeRemaining.Get(smi) <= 0.0;

    public static void DoWorldDamage(GameObject part, Vector3 apparentPosition, int actualWorld)
    {
      OccupyArea component1 = part.GetComponent<OccupyArea>();
      component1.UpdateOccupiedArea();
      foreach (CellOffset occupiedCellsOffset in component1.OccupiedCellsOffsets)
      {
        int index = Grid.OffsetCell(Grid.PosToCell(apparentPosition), occupiedCellsOffset);
        if (Grid.IsValidCell(index) && (int) Grid.WorldIdx[index] == (int) Grid.WorldIdx[actualWorld])
        {
          if (Grid.Solid[index])
          {
            double num = (double) WorldDamage.Instance.ApplyDamage(index, 10000f, index, (string) BUILDINGS.DAMAGESOURCES.ROCKET, (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET);
          }
          else if (Grid.FakeFloor[index])
          {
            GameObject go = Grid.Objects[index, 39];
            if (Object.op_Inequality((Object) go, (Object) null) && go.HasTag(GameTags.GantryExtended))
            {
              BuildingHP component2 = go.GetComponent<BuildingHP>();
              if (Object.op_Inequality((Object) component2, (Object) null))
                EventExtensions.Trigger(go, -794517298, (object) new BuildingHP.DamageSourceInfo()
                {
                  damage = component2.MaxHitPoints,
                  source = (string) BUILDINGS.DAMAGESOURCES.ROCKET,
                  popString = (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET
                });
            }
          }
        }
      }
    }

    public class NotGroundedStates : 
      GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State
    {
      public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State launch_setup;
      public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State launch_loop;
      public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State space;
      public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State landing_setup;
      public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State landing_loop;
      public GameStateMachine<LaunchableRocketCluster.States, LaunchableRocketCluster.StatesInstance, LaunchableRocketCluster, object>.State land;
    }
  }
}
