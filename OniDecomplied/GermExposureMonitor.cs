// Decompiled with JetBrains decompiler
// Type: GermExposureMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GermExposureMonitor : 
  GameStateMachine<GermExposureMonitor, GermExposureMonitor.Instance>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.serializable = StateMachine.SerializeType.Never;
    this.root.Update((System.Action<GermExposureMonitor.Instance, float>) ((smi, dt) => smi.OnInhaleExposureTick(dt)), (UpdateRate) 6, true).EventHandler(GameHashes.EatCompleteEater, (GameStateMachine<GermExposureMonitor, GermExposureMonitor.Instance, IStateMachineTarget, object>.GameEvent.Callback) ((smi, obj) => smi.OnEatComplete(obj))).EventHandler(GameHashes.SicknessAdded, (GameStateMachine<GermExposureMonitor, GermExposureMonitor.Instance, IStateMachineTarget, object>.GameEvent.Callback) ((smi, data) => smi.OnSicknessAdded(data))).EventHandler(GameHashes.SicknessCured, (GameStateMachine<GermExposureMonitor, GermExposureMonitor.Instance, IStateMachineTarget, object>.GameEvent.Callback) ((smi, data) => smi.OnSicknessCured(data))).EventHandler(GameHashes.SleepFinished, (StateMachine<GermExposureMonitor, GermExposureMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.OnSleepFinished()));
  }

  public static float GetContractionChance(float rating) => (float) (0.5 - 0.5 * Math.Tanh(0.25 * (double) rating));

  public enum ExposureState
  {
    None,
    Contact,
    Exposed,
    Contracted,
    Sick,
  }

  public class ExposureStatusData
  {
    public ExposureType exposure_type;
    public GermExposureMonitor.Instance owner;
  }

  [SerializationConfig]
  public new class Instance : 
    GameStateMachine<GermExposureMonitor, GermExposureMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    [Serialize]
    public Dictionary<HashedString, GermExposureMonitor.Instance.DiseaseSourceInfo> lastDiseaseSources;
    [Serialize]
    public Dictionary<HashedString, float> lastExposureTime;
    private Dictionary<HashedString, GermExposureMonitor.Instance.InhaleTickInfo> inhaleExposureTick;
    private Sicknesses sicknesses;
    private PrimaryElement primaryElement;
    private Traits traits;
    [Serialize]
    private Dictionary<string, GermExposureMonitor.ExposureState> exposureStates = new Dictionary<string, GermExposureMonitor.ExposureState>();
    [Serialize]
    private Dictionary<string, float> exposureTiers = new Dictionary<string, float>();
    private Dictionary<string, Guid> statusItemHandles = new Dictionary<string, Guid>();
    private Dictionary<string, Guid> contactStatusItemHandles = new Dictionary<string, Guid>();

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.sicknesses = master.GetComponent<MinionModifiers>().sicknesses;
      this.primaryElement = master.GetComponent<PrimaryElement>();
      this.traits = master.GetComponent<Traits>();
      this.lastDiseaseSources = new Dictionary<HashedString, GermExposureMonitor.Instance.DiseaseSourceInfo>();
      this.lastExposureTime = new Dictionary<HashedString, float>();
      this.inhaleExposureTick = new Dictionary<HashedString, GermExposureMonitor.Instance.InhaleTickInfo>();
      GameClock.Instance.Subscribe(-722330267, new System.Action<object>(this.OnNightTime));
      this.GetComponent<OxygenBreather>().onSimConsume += new System.Action<Sim.MassConsumedCallback>(this.OnAirConsumed);
    }

    public override void StartSM()
    {
      base.StartSM();
      this.RefreshStatusItems();
    }

    public override void StopSM(string reason)
    {
      GameClock.Instance.Unsubscribe(-722330267, new System.Action<object>(this.OnNightTime));
      foreach (ExposureType exposureType in GERM_EXPOSURE.TYPES)
      {
        Guid guid;
        this.statusItemHandles.TryGetValue(exposureType.germ_id, out guid);
        guid = this.GetComponent<KSelectable>().RemoveStatusItem(guid);
      }
      base.StopSM(reason);
    }

    public void OnEatComplete(object obj)
    {
      Edible edible = (Edible) obj;
      HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(((Component) edible).gameObject);
      if (!HandleVector<int>.Handle.op_Inequality(handle, HandleVector<int>.InvalidHandle))
        return;
      DiseaseHeader header = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) GameComps.DiseaseContainers).GetHeader(handle);
      if (header.diseaseIdx == byte.MaxValue)
        return;
      Klei.AI.Disease disease = Db.Get().Diseases[(int) header.diseaseIdx];
      float num = edible.unitsConsumed / (edible.unitsConsumed + edible.Units);
      int count = Mathf.CeilToInt((float) header.diseaseCount * num);
      GameComps.DiseaseContainers.ModifyDiseaseCount(handle, -count);
      KPrefabID component = ((Component) edible).GetComponent<KPrefabID>();
      this.InjectDisease(disease, count, component.PrefabID(), Sickness.InfectionVector.Digestion);
    }

    public void OnAirConsumed(Sim.MassConsumedCallback mass_cb_info)
    {
      if (mass_cb_info.diseaseIdx == byte.MaxValue)
        return;
      this.InjectDisease(Db.Get().Diseases[(int) mass_cb_info.diseaseIdx], mass_cb_info.diseaseCount, ElementLoader.elements[(int) mass_cb_info.elemIdx].tag, Sickness.InfectionVector.Inhalation);
    }

    public void OnInhaleExposureTick(float dt)
    {
      foreach (KeyValuePair<HashedString, GermExposureMonitor.Instance.InhaleTickInfo> keyValuePair in this.inhaleExposureTick)
      {
        if (keyValuePair.Value.inhaled)
        {
          keyValuePair.Value.inhaled = false;
          ++keyValuePair.Value.ticks;
        }
        else
          keyValuePair.Value.ticks = Mathf.Max(0, keyValuePair.Value.ticks - 1);
      }
    }

    public void TryInjectDisease(
      byte disease_idx,
      int count,
      Tag source,
      Sickness.InfectionVector vector)
    {
      if (disease_idx == byte.MaxValue)
        return;
      this.InjectDisease(Db.Get().Diseases[(int) disease_idx], count, source, vector);
    }

    public float GetGermResistance() => Db.Get().Attributes.GermResistance.Lookup(this.gameObject).GetTotalValue();

    public float GetResistanceToExposureType(ExposureType exposureType, float overrideExposureTier = -1f)
    {
      float num1 = overrideExposureTier;
      if ((double) num1 == -1.0)
        num1 = this.GetExposureTier(exposureType.germ_id);
      float num2 = Mathf.Clamp(num1, 1f, 3f);
      float num3 = GERM_EXPOSURE.EXPOSURE_TIER_RESISTANCE_BONUSES[(int) num2 - 1];
      float totalValue = Db.Get().Attributes.GermResistance.Lookup(this.gameObject).GetTotalValue();
      return (float) exposureType.base_resistance + totalValue + num3;
    }

    public int AssessDigestedGerms(ExposureType exposure_type, int count)
    {
      int exposureThreshold = exposure_type.exposure_threshold;
      return MathUtil.Clamp(1, 3, count / exposureThreshold);
    }

    public bool AssessInhaledGerms(ExposureType exposure_type)
    {
      GermExposureMonitor.Instance.InhaleTickInfo inhaleTickInfo;
      this.inhaleExposureTick.TryGetValue(HashedString.op_Implicit(exposure_type.germ_id), out inhaleTickInfo);
      if (inhaleTickInfo == null)
      {
        inhaleTickInfo = new GermExposureMonitor.Instance.InhaleTickInfo();
        this.inhaleExposureTick[HashedString.op_Implicit(exposure_type.germ_id)] = inhaleTickInfo;
      }
      if (inhaleTickInfo.inhaled)
        return false;
      float exposureTier = this.GetExposureTier(exposure_type.germ_id);
      inhaleTickInfo.inhaled = true;
      return inhaleTickInfo.ticks >= GERM_EXPOSURE.INHALE_TICK_THRESHOLD[(int) exposureTier];
    }

    public void InjectDisease(
      Klei.AI.Disease disease,
      int count,
      Tag source,
      Sickness.InfectionVector vector)
    {
      foreach (ExposureType exposureType in GERM_EXPOSURE.TYPES)
      {
        if (HashedString.op_Equality(disease.id, HashedString.op_Implicit(exposureType.germ_id)) && count > exposureType.exposure_threshold && this.HasMinExposurePeriodElapsed(exposureType.germ_id) && this.IsExposureValidForTraits(exposureType))
        {
          Sickness sickness = exposureType.sickness_id != null ? Db.Get().Sicknesses.Get(exposureType.sickness_id) : (Sickness) null;
          if (sickness == null || sickness.infectionVectors.Contains(vector))
          {
            GermExposureMonitor.ExposureState exposureState = this.GetExposureState(exposureType.germ_id);
            float exposureTier = this.GetExposureTier(exposureType.germ_id);
            switch (exposureState)
            {
              case GermExposureMonitor.ExposureState.None:
              case GermExposureMonitor.ExposureState.Contact:
                float contractionChance1 = GermExposureMonitor.GetContractionChance(this.GetResistanceToExposureType(exposureType));
                this.SetExposureState(exposureType.germ_id, GermExposureMonitor.ExposureState.Contact);
                if ((double) contractionChance1 > 0.0)
                {
                  this.lastDiseaseSources[disease.id] = new GermExposureMonitor.Instance.DiseaseSourceInfo(source, vector, contractionChance1, TransformExtensions.GetPosition(this.transform));
                  if (exposureType.infect_immediately)
                  {
                    this.InfectImmediately(exposureType);
                    continue;
                  }
                  bool flag1 = true;
                  bool flag2 = vector == Sickness.InfectionVector.Inhalation;
                  bool flag3 = vector == Sickness.InfectionVector.Digestion;
                  int tier = 1;
                  if (flag2)
                    flag1 = this.AssessInhaledGerms(exposureType);
                  if (flag3)
                    tier = this.AssessDigestedGerms(exposureType, count);
                  if (flag1)
                  {
                    if (flag2)
                      this.inhaleExposureTick[HashedString.op_Implicit(exposureType.germ_id)].ticks = 0;
                    this.SetExposureState(exposureType.germ_id, GermExposureMonitor.ExposureState.Exposed);
                    this.SetExposureTier(exposureType.germ_id, (float) tier);
                    float amount = Mathf.Clamp01(contractionChance1);
                    GermExposureTracker.Instance.AddExposure(exposureType, amount);
                    continue;
                  }
                  continue;
                }
                continue;
              case GermExposureMonitor.ExposureState.Exposed:
                if ((double) exposureTier < 3.0)
                {
                  float contractionChance2 = GermExposureMonitor.GetContractionChance(this.GetResistanceToExposureType(exposureType));
                  if ((double) contractionChance2 > 0.0)
                  {
                    this.lastDiseaseSources[disease.id] = new GermExposureMonitor.Instance.DiseaseSourceInfo(source, vector, contractionChance2, TransformExtensions.GetPosition(this.transform));
                    if (!exposureType.infect_immediately)
                    {
                      bool flag4 = true;
                      bool flag5 = vector == Sickness.InfectionVector.Inhalation;
                      bool flag6 = vector == Sickness.InfectionVector.Digestion;
                      int num = 1;
                      if (flag5)
                        flag4 = this.AssessInhaledGerms(exposureType);
                      if (flag6)
                        num = this.AssessDigestedGerms(exposureType, count);
                      if (flag4)
                      {
                        if (flag5)
                          this.inhaleExposureTick[HashedString.op_Implicit(exposureType.germ_id)].ticks = 0;
                        this.SetExposureTier(exposureType.germ_id, this.GetExposureTier(exposureType.germ_id) + (float) num);
                        float amount = Mathf.Clamp01(GermExposureMonitor.GetContractionChance(this.GetResistanceToExposureType(exposureType)) - contractionChance2);
                        GermExposureTracker.Instance.AddExposure(exposureType, amount);
                        continue;
                      }
                      continue;
                    }
                    continue;
                  }
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
      }
      this.RefreshStatusItems();
    }

    public GermExposureMonitor.ExposureState GetExposureState(string germ_id)
    {
      GermExposureMonitor.ExposureState exposureState;
      this.exposureStates.TryGetValue(germ_id, out exposureState);
      return exposureState;
    }

    public float GetExposureTier(string germ_id)
    {
      float num = 1f;
      this.exposureTiers.TryGetValue(germ_id, out num);
      return Mathf.Clamp(num, 1f, 3f);
    }

    public void SetExposureState(string germ_id, GermExposureMonitor.ExposureState exposure_state)
    {
      this.exposureStates[germ_id] = exposure_state;
      this.RefreshStatusItems();
    }

    public void SetExposureTier(string germ_id, float tier)
    {
      tier = Mathf.Clamp(tier, 0.0f, 3f);
      this.exposureTiers[germ_id] = tier;
      this.RefreshStatusItems();
    }

    public void ContractGerms(string germ_id)
    {
      DebugUtil.DevAssert(this.GetExposureState(germ_id) == GermExposureMonitor.ExposureState.Exposed, "Duplicant is contracting a sickness but was never exposed to it!", (Object) null);
      this.SetExposureState(germ_id, GermExposureMonitor.ExposureState.Contracted);
    }

    public void OnSicknessAdded(object sickness_instance_data)
    {
      SicknessInstance sicknessInstance = (SicknessInstance) sickness_instance_data;
      foreach (ExposureType exposureType in GERM_EXPOSURE.TYPES)
      {
        if (exposureType.sickness_id == sicknessInstance.Sickness.Id)
          this.SetExposureState(exposureType.germ_id, GermExposureMonitor.ExposureState.Sick);
      }
    }

    public void OnSicknessCured(object sickness_instance_data)
    {
      SicknessInstance sicknessInstance = (SicknessInstance) sickness_instance_data;
      foreach (ExposureType exposureType in GERM_EXPOSURE.TYPES)
      {
        if (exposureType.sickness_id == sicknessInstance.Sickness.Id)
          this.SetExposureState(exposureType.germ_id, GermExposureMonitor.ExposureState.None);
      }
    }

    private bool IsExposureValidForTraits(ExposureType exposure_type)
    {
      if (exposure_type.required_traits != null && exposure_type.required_traits.Count > 0)
      {
        foreach (string requiredTrait in exposure_type.required_traits)
        {
          if (!this.traits.HasTrait(requiredTrait))
            return false;
        }
      }
      if (exposure_type.excluded_traits != null && exposure_type.excluded_traits.Count > 0)
      {
        foreach (string excludedTrait in exposure_type.excluded_traits)
        {
          if (this.traits.HasTrait(excludedTrait))
            return false;
        }
      }
      if (exposure_type.excluded_effects != null && exposure_type.excluded_effects.Count > 0)
      {
        Effects component = this.master.GetComponent<Effects>();
        foreach (string excludedEffect in exposure_type.excluded_effects)
        {
          if (component.HasEffect(excludedEffect))
            return false;
        }
      }
      return true;
    }

    private bool HasMinExposurePeriodElapsed(string germ_id)
    {
      float num;
      this.lastExposureTime.TryGetValue(HashedString.op_Implicit(germ_id), out num);
      return (double) num == 0.0 || (double) GameClock.Instance.GetTime() - (double) num > 540.0;
    }

    private void RefreshStatusItems()
    {
      foreach (ExposureType exposureType in GERM_EXPOSURE.TYPES)
      {
        Guid guid1;
        this.contactStatusItemHandles.TryGetValue(exposureType.germ_id, out guid1);
        Guid guid2;
        this.statusItemHandles.TryGetValue(exposureType.germ_id, out guid2);
        GermExposureMonitor.ExposureState exposureState = this.GetExposureState(exposureType.germ_id);
        if (guid2 == Guid.Empty && (exposureState == GermExposureMonitor.ExposureState.Exposed || exposureState == GermExposureMonitor.ExposureState.Contracted) && !string.IsNullOrEmpty(exposureType.sickness_id))
          guid2 = this.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.ExposedToGerms, (object) new GermExposureMonitor.ExposureStatusData()
          {
            exposure_type = exposureType,
            owner = this
          });
        else if (guid2 != Guid.Empty && exposureState != GermExposureMonitor.ExposureState.Exposed && exposureState != GermExposureMonitor.ExposureState.Contracted)
          guid2 = this.GetComponent<KSelectable>().RemoveStatusItem(guid2);
        this.statusItemHandles[exposureType.germ_id] = guid2;
        if (guid1 == Guid.Empty && exposureState == GermExposureMonitor.ExposureState.Contact)
        {
          if (!string.IsNullOrEmpty(exposureType.sickness_id))
            guid1 = this.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.ContactWithGerms, (object) new GermExposureMonitor.ExposureStatusData()
            {
              exposure_type = exposureType,
              owner = this
            });
        }
        else if (guid1 != Guid.Empty && exposureState != GermExposureMonitor.ExposureState.Contact)
          guid1 = this.GetComponent<KSelectable>().RemoveStatusItem(guid1);
        this.contactStatusItemHandles[exposureType.germ_id] = guid1;
      }
    }

    private void OnNightTime(object data) => this.UpdateReports();

    private void UpdateReports() => ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseStatus, (float) this.primaryElement.DiseaseCount, StringFormatter.Replace((string) UI.ENDOFDAYREPORT.NOTES.GERMS, "{0}", this.master.name), this.master.gameObject.GetProperName());

    public void InfectImmediately(ExposureType exposure_type)
    {
      if (exposure_type.infection_effect != null)
        this.master.GetComponent<Effects>().Add(exposure_type.infection_effect, true);
      if (exposure_type.sickness_id == null)
        return;
      string lastDiseaseSource = this.GetLastDiseaseSource(exposure_type.germ_id);
      this.sicknesses.Infect(new SicknessExposureInfo(exposure_type.sickness_id, lastDiseaseSource));
    }

    public void OnSleepFinished()
    {
      foreach (ExposureType exposureType in GERM_EXPOSURE.TYPES)
      {
        if (!exposureType.infect_immediately && exposureType.sickness_id != null)
        {
          int exposureState = (int) this.GetExposureState(exposureType.germ_id);
          if (exposureState == 2)
            this.SetExposureState(exposureType.germ_id, GermExposureMonitor.ExposureState.None);
          if (exposureState == 3)
          {
            this.SetExposureState(exposureType.germ_id, GermExposureMonitor.ExposureState.Sick);
            string lastDiseaseSource = this.GetLastDiseaseSource(exposureType.germ_id);
            this.sicknesses.Infect(new SicknessExposureInfo(exposureType.sickness_id, lastDiseaseSource));
          }
          this.SetExposureTier(exposureType.germ_id, 0.0f);
        }
      }
    }

    public string GetLastDiseaseSource(string id)
    {
      GermExposureMonitor.Instance.DiseaseSourceInfo diseaseSourceInfo;
      string lastDiseaseSource;
      if (this.lastDiseaseSources.TryGetValue(HashedString.op_Implicit(id), out diseaseSourceInfo))
      {
        switch (diseaseSourceInfo.vector)
        {
          case Sickness.InfectionVector.Contact:
            lastDiseaseSource = (string) DUPLICANTS.DISEASES.INFECTIONSOURCES.SKIN;
            break;
          case Sickness.InfectionVector.Digestion:
            lastDiseaseSource = string.Format((string) DUPLICANTS.DISEASES.INFECTIONSOURCES.FOOD, (object) diseaseSourceInfo.sourceObject.ProperName());
            break;
          case Sickness.InfectionVector.Inhalation:
            lastDiseaseSource = string.Format((string) DUPLICANTS.DISEASES.INFECTIONSOURCES.AIR, (object) diseaseSourceInfo.sourceObject.ProperName());
            break;
          default:
            lastDiseaseSource = (string) DUPLICANTS.DISEASES.INFECTIONSOURCES.UNKNOWN;
            break;
        }
      }
      else
        lastDiseaseSource = (string) DUPLICANTS.DISEASES.INFECTIONSOURCES.UNKNOWN;
      return lastDiseaseSource;
    }

    public Vector3 GetLastExposurePosition(string germ_id)
    {
      GermExposureMonitor.Instance.DiseaseSourceInfo diseaseSourceInfo;
      return this.lastDiseaseSources.TryGetValue(HashedString.op_Implicit(germ_id), out diseaseSourceInfo) ? diseaseSourceInfo.position : TransformExtensions.GetPosition(this.transform);
    }

    public float GetExposureWeight(string id)
    {
      float exposureTier = this.GetExposureTier(id);
      GermExposureMonitor.Instance.DiseaseSourceInfo diseaseSourceInfo;
      return this.lastDiseaseSources.TryGetValue(HashedString.op_Implicit(id), out diseaseSourceInfo) ? diseaseSourceInfo.factor * exposureTier : 0.0f;
    }

    [Serializable]
    public class DiseaseSourceInfo
    {
      public Tag sourceObject;
      public Sickness.InfectionVector vector;
      public float factor;
      public Vector3 position;

      public DiseaseSourceInfo(
        Tag sourceObject,
        Sickness.InfectionVector vector,
        float factor,
        Vector3 position)
      {
        this.sourceObject = sourceObject;
        this.vector = vector;
        this.factor = factor;
        this.position = position;
      }
    }

    public class InhaleTickInfo
    {
      public bool inhaled;
      public int ticks;
    }
  }
}
