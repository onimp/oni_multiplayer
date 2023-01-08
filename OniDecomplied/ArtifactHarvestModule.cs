// Decompiled with JetBrains decompiler
// Type: ArtifactHarvestModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ArtifactHarvestModule : 
  GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>
{
  public StateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.BoolParameter canHarvest;
  public GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.State grounded;
  public ArtifactHarvestModule.NotGroundedStates not_grounded;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.grounded;
    this.root.Enter((StateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.State.Callback) (smi => smi.CheckIfCanHarvest()));
    this.grounded.TagTransition(GameTags.RocketNotOnGround, (GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.State) this.not_grounded);
    this.not_grounded.DefaultState(this.not_grounded.not_harvesting).EventHandler(GameHashes.ClusterLocationChanged, (Func<ArtifactHarvestModule.StatesInstance, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), (StateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.State.Callback) (smi => smi.CheckIfCanHarvest())).EventHandler(GameHashes.OnStorageChange, (StateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.State.Callback) (smi => smi.CheckIfCanHarvest())).TagTransition(GameTags.RocketNotOnGround, this.grounded, true);
    this.not_grounded.not_harvesting.PlayAnim("loaded").ParamTransition<bool>((StateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.Parameter<bool>) this.canHarvest, this.not_grounded.harvesting, GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.IsTrue);
    this.not_grounded.harvesting.PlayAnim("deploying").Update((System.Action<ArtifactHarvestModule.StatesInstance, float>) ((smi, dt) => smi.HarvestFromPOI(dt)), (UpdateRate) 7).ParamTransition<bool>((StateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.Parameter<bool>) this.canHarvest, this.not_grounded.not_harvesting, GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.IsFalse);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class NotGroundedStates : 
    GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.State
  {
    public GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.State not_harvesting;
    public GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.State harvesting;
  }

  public class StatesInstance : 
    GameStateMachine<ArtifactHarvestModule, ArtifactHarvestModule.StatesInstance, IStateMachineTarget, ArtifactHarvestModule.Def>.GameInstance
  {
    [MyCmpReq]
    private Storage storage;
    [MyCmpReq]
    private SingleEntityReceptacle receptacle;

    public StatesInstance(IStateMachineTarget master, ArtifactHarvestModule.Def def)
      : base(master, def)
    {
    }

    public void HarvestFromPOI(float dt)
    {
      ClusterGridEntity atCurrentLocation = ((Component) this.GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>().GetPOIAtCurrentLocation();
      if (Util.IsNullOrDestroyed((object) atCurrentLocation))
        return;
      ArtifactPOIStates.Instance smi = ((Component) atCurrentLocation).GetSMI<ArtifactPOIStates.Instance>();
      if (!Object.op_Implicit((Object) ((Component) atCurrentLocation).GetComponent<ArtifactPOIClusterGridEntity>()) && !Object.op_Implicit((Object) ((Component) atCurrentLocation).GetComponent<HarvestablePOIClusterGridEntity>()) || Util.IsNullOrDestroyed((object) smi))
        return;
      bool flag = false;
      string artifactToHarvest = smi.GetArtifactToHarvest();
      if (artifactToHarvest == null)
        return;
      GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(artifactToHarvest)), this.transform.position);
      gameObject.SetActive(true);
      this.receptacle.ForceDeposit(gameObject);
      this.storage.Store(gameObject);
      smi.HarvestArtifact();
      if (smi.configuration.DestroyOnHarvest())
        flag = true;
      if (!flag)
        return;
      TracesExtesions.DeleteObject(((Component) atCurrentLocation).gameObject);
    }

    public bool CheckIfCanHarvest()
    {
      Clustercraft component = ((Component) this.GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>();
      if (Object.op_Equality((Object) component, (Object) null))
        return false;
      ClusterGridEntity atCurrentLocation = component.GetPOIAtCurrentLocation();
      if (Object.op_Inequality((Object) atCurrentLocation, (Object) null) && (Object.op_Implicit((Object) ((Component) atCurrentLocation).GetComponent<ArtifactPOIClusterGridEntity>()) || Object.op_Implicit((Object) ((Component) atCurrentLocation).GetComponent<HarvestablePOIClusterGridEntity>())))
      {
        ArtifactPOIStates.Instance smi = ((Component) atCurrentLocation).GetSMI<ArtifactPOIStates.Instance>();
        if (smi != null && smi.CanHarvestArtifact() && Object.op_Equality((Object) this.receptacle.Occupant, (Object) null))
        {
          this.sm.canHarvest.Set(true, this);
          return true;
        }
      }
      this.sm.canHarvest.Set(false, this);
      return false;
    }
  }
}
