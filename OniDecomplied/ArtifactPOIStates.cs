// Decompiled with JetBrains decompiler
// Type: ArtifactPOIStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ArtifactPOIStates")]
public class ArtifactPOIStates : 
  GameStateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>
{
  public GameStateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.State idle;
  public GameStateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.State recharging;
  public StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.FloatParameter poiCharge = new StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.FloatParameter(1f);

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    default_state = (StateMachine.BaseState) this.idle;
    this.root.Enter((StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.State.Callback) (smi =>
    {
      if (smi.configuration != null && !HashedString.op_Equality(smi.configuration.typeId, HashedString.Invalid))
        return;
      smi.configuration = smi.GetComponent<ArtifactPOIConfigurator>().MakeConfiguration();
      smi.PickNewArtifactToHarvest();
      smi.poiCharge = 1f;
    }));
    this.idle.ParamTransition<float>((StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.Parameter<float>) this.poiCharge, this.recharging, (StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.Parameter<float>.Callback) ((smi, f) => (double) f < 1.0));
    this.recharging.ParamTransition<float>((StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.Parameter<float>) this.poiCharge, this.idle, (StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.Parameter<float>.Callback) ((smi, f) => (double) f >= 1.0)).EventHandler(GameHashes.NewDay, (Func<ArtifactPOIStates.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), (StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.State.Callback) (smi => smi.RechargePOI(600f)));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.GameInstance,
    IGameObjectEffectDescriptor
  {
    [Serialize]
    public ArtifactPOIConfigurator.ArtifactPOIInstanceConfiguration configuration;
    [Serialize]
    private float _poiCharge;
    [Serialize]
    public string artifactToHarvest;
    [Serialize]
    private int numHarvests;

    public float poiCharge
    {
      get => this._poiCharge;
      set
      {
        this._poiCharge = value;
        double num = (double) this.smi.sm.poiCharge.Set(value, this.smi);
      }
    }

    public Instance(IStateMachineTarget target, ArtifactPOIStates.Def def)
      : base(target, def)
    {
    }

    public void PickNewArtifactToHarvest()
    {
      if (this.numHarvests <= 0 && !string.IsNullOrEmpty(this.configuration.GetArtifactID()))
      {
        this.artifactToHarvest = this.configuration.GetArtifactID();
        ArtifactSelector.Instance.ReserveArtifactID(this.artifactToHarvest);
      }
      else
        this.artifactToHarvest = ArtifactSelector.Instance.GetUniqueArtifactID(ArtifactType.Space);
    }

    public string GetArtifactToHarvest()
    {
      if (!this.CanHarvestArtifact())
        return (string) null;
      if (string.IsNullOrEmpty(this.artifactToHarvest))
        this.PickNewArtifactToHarvest();
      return this.artifactToHarvest;
    }

    public void HarvestArtifact()
    {
      if (!this.CanHarvestArtifact())
        return;
      ++this.numHarvests;
      this.poiCharge = 0.0f;
      this.artifactToHarvest = (string) null;
      this.PickNewArtifactToHarvest();
    }

    public void RechargePOI(float dt) => this.DeltaPOICharge(dt / this.configuration.GetRechargeTime());

    public float RechargeTimeRemaining() => (float) Mathf.CeilToInt((float) (((double) this.configuration.GetRechargeTime() - (double) this.configuration.GetRechargeTime() * (double) this.poiCharge) / 600.0)) * 600f;

    public void DeltaPOICharge(float delta)
    {
      this.poiCharge += delta;
      this.poiCharge = Mathf.Min(1f, this.poiCharge);
    }

    public bool CanHarvestArtifact() => (double) this.poiCharge >= 1.0;

    public List<Descriptor> GetDescriptors(GameObject go) => new List<Descriptor>();
  }
}
