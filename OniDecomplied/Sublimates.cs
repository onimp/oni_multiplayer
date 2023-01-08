// Decompiled with JetBrains decompiler
// Type: Sublimates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/Sublimates")]
public class Sublimates : KMonoBehaviour, ISim200ms
{
  [MyCmpReq]
  private PrimaryElement primaryElement;
  [MyCmpReq]
  private KSelectable selectable;
  [SerializeField]
  public SpawnFXHashes spawnFXHash;
  public bool decayStorage;
  [SerializeField]
  public Sublimates.Info info;
  [Serialize]
  private float sublimatedMass;
  private HandleVector<int>.Handle flowAccumulator = HandleVector<int>.InvalidHandle;
  private Sublimates.EmitState lastEmitState = ~Sublimates.EmitState.Emitting;
  private static readonly EventSystem.IntraObjectHandler<Sublimates> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<Sublimates>((Action<Sublimates, object>) ((component, data) => component.OnAbsorb(data)));
  private static readonly EventSystem.IntraObjectHandler<Sublimates> OnSplitFromChunkDelegate = new EventSystem.IntraObjectHandler<Sublimates>((Action<Sublimates, object>) ((component, data) => component.OnSplitFromChunk(data)));

  public float Temperature => this.primaryElement.Temperature;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<Sublimates>(-2064133523, Sublimates.OnAbsorbDelegate);
    this.Subscribe<Sublimates>(1335436905, Sublimates.OnSplitFromChunkDelegate);
    this.simRenderLoadBalance = true;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.flowAccumulator = Game.Instance.accumulators.Add("EmittedMass", (KMonoBehaviour) this);
    this.RefreshStatusItem(Sublimates.EmitState.Emitting);
  }

  protected virtual void OnCleanUp()
  {
    this.flowAccumulator = Game.Instance.accumulators.Remove(this.flowAccumulator);
    base.OnCleanUp();
  }

  private void OnAbsorb(object data)
  {
    Pickupable pickupable = (Pickupable) data;
    if (!Object.op_Inequality((Object) pickupable, (Object) null))
      return;
    Sublimates component = ((Component) pickupable).GetComponent<Sublimates>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.sublimatedMass += component.sublimatedMass;
  }

  private void OnSplitFromChunk(object data)
  {
    Pickupable pickupable = data as Pickupable;
    PrimaryElement component1 = ((Component) pickupable).GetComponent<PrimaryElement>();
    Sublimates component2 = ((Component) pickupable).GetComponent<Sublimates>();
    if (Object.op_Equality((Object) component2, (Object) null))
      return;
    float mass1 = this.primaryElement.Mass;
    float mass2 = component1.Mass;
    float num1 = mass1 / (mass2 + mass1);
    this.sublimatedMass = component2.sublimatedMass * num1;
    float num2 = 1f - num1;
    component2.sublimatedMass *= num2;
  }

  public void Sim200ms(float dt)
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    if (!Grid.IsValidCell(cell))
      return;
    bool flag = ((Component) this).HasTag(GameTags.Sealed);
    Pickupable component = ((Component) this).GetComponent<Pickupable>();
    Storage cmp = Object.op_Inequality((Object) component, (Object) null) ? component.storage : (Storage) null;
    if (flag && !this.decayStorage || flag && Object.op_Inequality((Object) cmp, (Object) null) && ((Component) cmp).HasTag(GameTags.CorrosionProof))
      return;
    if ((double) this.primaryElement.Temperature <= (double) ElementLoader.FindElementByHash(this.info.sublimatedElement).lowTemp)
    {
      this.RefreshStatusItem(Sublimates.EmitState.BlockedOnTemperature);
    }
    else
    {
      float num1 = Grid.Mass[cell];
      if ((double) num1 < (double) this.info.maxDestinationMass)
      {
        float mass1 = this.primaryElement.Mass;
        if ((double) mass1 > 0.0)
        {
          float num2 = Mathf.Min(Mathf.Max(this.info.sublimationRate, this.info.sublimationRate * Mathf.Pow(mass1, this.info.massPower)) * dt, mass1);
          this.sublimatedMass += num2;
          float num3 = mass1 - num2;
          if ((double) this.sublimatedMass <= (double) this.info.minSublimationAmount)
            return;
          float num4 = this.sublimatedMass / this.primaryElement.Mass;
          byte diseaseIdx;
          int disease_count;
          if (this.info.diseaseIdx == byte.MaxValue)
          {
            diseaseIdx = this.primaryElement.DiseaseIdx;
            disease_count = (int) ((double) this.primaryElement.DiseaseCount * (double) num4);
            this.primaryElement.ModifyDiseaseCount(-disease_count, "Sublimates.SimUpdate");
          }
          else
          {
            float num5 = this.sublimatedMass / this.info.sublimationRate;
            diseaseIdx = this.info.diseaseIdx;
            disease_count = (int) ((double) this.info.diseaseCount * (double) num5);
          }
          float mass2 = Mathf.Min(this.sublimatedMass, this.info.maxDestinationMass - num1);
          if ((double) mass2 > 0.0)
          {
            this.Emit(cell, mass2, this.primaryElement.Temperature, diseaseIdx, disease_count);
            this.sublimatedMass = Mathf.Max(0.0f, this.sublimatedMass - mass2);
            this.primaryElement.Mass = Mathf.Max(0.0f, this.primaryElement.Mass - mass2);
            this.UpdateStorage();
            this.RefreshStatusItem(Sublimates.EmitState.Emitting);
            if (!flag || !this.decayStorage || !Object.op_Inequality((Object) cmp, (Object) null))
              return;
            cmp.Trigger(-794517298, (object) new BuildingHP.DamageSourceInfo()
            {
              damage = 1,
              source = (string) BUILDINGS.DAMAGESOURCES.CORROSIVE_ELEMENT,
              popString = (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CORROSIVE_ELEMENT,
              fullDamageEffectName = "smoke_damage_kanim"
            });
          }
          else
            this.RefreshStatusItem(Sublimates.EmitState.BlockedOnPressure);
        }
        else if ((double) this.sublimatedMass > 0.0)
        {
          float mass3 = Mathf.Min(this.sublimatedMass, this.info.maxDestinationMass - num1);
          if ((double) mass3 > 0.0)
          {
            this.Emit(cell, mass3, this.primaryElement.Temperature, this.primaryElement.DiseaseIdx, this.primaryElement.DiseaseCount);
            this.sublimatedMass = Mathf.Max(0.0f, this.sublimatedMass - mass3);
            this.primaryElement.Mass = Mathf.Max(0.0f, this.primaryElement.Mass - mass3);
            this.UpdateStorage();
            this.RefreshStatusItem(Sublimates.EmitState.Emitting);
          }
          else
            this.RefreshStatusItem(Sublimates.EmitState.BlockedOnPressure);
        }
        else
        {
          if (this.primaryElement.KeepZeroMassObject)
            return;
          Util.KDestroyGameObject(((Component) this).gameObject);
        }
      }
      else
        this.RefreshStatusItem(Sublimates.EmitState.BlockedOnPressure);
    }
  }

  private void UpdateStorage()
  {
    Pickupable component = ((Component) this).GetComponent<Pickupable>();
    if (!Object.op_Inequality((Object) component, (Object) null) || !Object.op_Inequality((Object) component.storage, (Object) null))
      return;
    component.storage.Trigger(-1697596308, (object) ((Component) this).gameObject);
  }

  private void Emit(int cell, float mass, float temperature, byte disease_idx, int disease_count)
  {
    SimMessages.AddRemoveSubstance(cell, this.info.sublimatedElement, CellEventLogger.Instance.SublimatesEmit, mass, temperature, disease_idx, disease_count);
    Game.Instance.accumulators.Accumulate(this.flowAccumulator, mass);
    if (this.spawnFXHash == SpawnFXHashes.None)
      return;
    TransformExtensions.GetPosition(this.transform).z = Grid.GetLayerZ(Grid.SceneLayer.Front);
    Game.Instance.SpawnFX(this.spawnFXHash, TransformExtensions.GetPosition(this.transform), 0.0f);
  }

  public float AvgFlowRate() => Game.Instance.accumulators.GetAverageRate(this.flowAccumulator);

  private void RefreshStatusItem(Sublimates.EmitState newEmitState)
  {
    if (newEmitState == this.lastEmitState)
      return;
    switch (newEmitState)
    {
      case Sublimates.EmitState.Emitting:
        if (this.info.sublimatedElement == SimHashes.Oxygen)
        {
          this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.EmittingOxygenAvg, (object) this);
          break;
        }
        this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.EmittingGasAvg, (object) this);
        break;
      case Sublimates.EmitState.BlockedOnPressure:
        this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.EmittingBlockedHighPressure, (object) this);
        break;
      case Sublimates.EmitState.BlockedOnTemperature:
        this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.EmittingBlockedLowTemperature, (object) this);
        break;
    }
    this.lastEmitState = newEmitState;
  }

  [Serializable]
  public struct Info
  {
    public float sublimationRate;
    public float minSublimationAmount;
    public float maxDestinationMass;
    public float massPower;
    public byte diseaseIdx;
    public int diseaseCount;
    [HashedEnum]
    public SimHashes sublimatedElement;

    public Info(
      float rate,
      float min_amount,
      float max_destination_mass,
      float mass_power,
      SimHashes element,
      byte disease_idx = 255,
      int disease_count = 0)
    {
      this.sublimationRate = rate;
      this.minSublimationAmount = min_amount;
      this.maxDestinationMass = max_destination_mass;
      this.massPower = mass_power;
      this.sublimatedElement = element;
      this.diseaseIdx = disease_idx;
      this.diseaseCount = disease_count;
    }
  }

  private enum EmitState
  {
    Emitting,
    BlockedOnPressure,
    BlockedOnTemperature,
  }
}
