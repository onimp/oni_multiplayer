// Decompiled with JetBrains decompiler
// Type: HarvestablePOIStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HarvestablePOIStates")]
public class HarvestablePOIStates : 
  GameStateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>
{
  public GameStateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.State idle;
  public GameStateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.State recharging;
  public StateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.FloatParameter poiCapacity = new StateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.FloatParameter(1f);

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    default_state = (StateMachine.BaseState) this.idle;
    this.root.Enter((StateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.State.Callback) (smi =>
    {
      if (smi.configuration != null && !HashedString.op_Equality(smi.configuration.typeId, HashedString.Invalid))
        return;
      smi.configuration = smi.GetComponent<HarvestablePOIConfigurator>().MakeConfiguration();
      smi.poiCapacity = Random.Range(0.0f, smi.configuration.GetMaxCapacity());
    }));
    this.idle.ParamTransition<float>((StateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.Parameter<float>) this.poiCapacity, this.recharging, (StateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.Parameter<float>.Callback) ((smi, f) => (double) f < (double) smi.configuration.GetMaxCapacity()));
    this.recharging.EventHandler(GameHashes.NewDay, (Func<HarvestablePOIStates.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), (StateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.State.Callback) (smi => smi.RechargePOI(600f))).ParamTransition<float>((StateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.Parameter<float>) this.poiCapacity, this.idle, (StateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.Parameter<float>.Callback) ((smi, f) => (double) f >= (double) smi.configuration.GetMaxCapacity()));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.GameInstance,
    IGameObjectEffectDescriptor
  {
    [Serialize]
    public HarvestablePOIConfigurator.HarvestablePOIInstanceConfiguration configuration;
    [Serialize]
    private float _poiCapacity;

    public float poiCapacity
    {
      get => this._poiCapacity;
      set
      {
        this._poiCapacity = value;
        double num = (double) this.smi.sm.poiCapacity.Set(value, this.smi);
      }
    }

    public Instance(IStateMachineTarget target, HarvestablePOIStates.Def def)
      : base(target, def)
    {
    }

    public void RechargePOI(float dt)
    {
      float num = dt / this.configuration.GetRechargeTime();
      this.DeltaPOICapacity(this.configuration.GetMaxCapacity() * num);
    }

    public void DeltaPOICapacity(float delta)
    {
      this.poiCapacity += delta;
      this.poiCapacity = Mathf.Min(this.configuration.GetMaxCapacity(), this.poiCapacity);
    }

    public bool POICanBeHarvested() => (double) this.poiCapacity > 0.0;

    public List<Descriptor> GetDescriptors(GameObject go)
    {
      List<Descriptor> descriptors = new List<Descriptor>();
      foreach (KeyValuePair<SimHashes, float> elementsWithWeight in this.configuration.GetElementsWithWeights())
      {
        SimHashes key = elementsWithWeight.Key;
        string str = ElementLoader.FindElementByHash(key).tag.ProperName();
        descriptors.Add(new Descriptor(string.Format((string) UI.SPACEDESTINATIONS.HARVESTABLE_POI.POI_PRODUCTION, (object) str), string.Format((string) UI.SPACEDESTINATIONS.HARVESTABLE_POI.POI_PRODUCTION_TOOLTIP, (object) key.ToString()), (Descriptor.DescriptorType) 1, false));
      }
      descriptors.Add(new Descriptor(string.Format("{0}/{1}", (object) GameUtil.GetFormattedMass(this.poiCapacity), (object) GameUtil.GetFormattedMass(this.configuration.GetMaxCapacity())), "Capacity", (Descriptor.DescriptorType) 1, false));
      return descriptors;
    }
  }
}
