// Decompiled with JetBrains decompiler
// Type: GeyserConfigurator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/GeyserConfigurator")]
public class GeyserConfigurator : KMonoBehaviour
{
  private static List<GeyserConfigurator.GeyserType> geyserTypes;
  public HashedString presetType;
  public float presetMin;
  public float presetMax = 1f;

  public static GeyserConfigurator.GeyserType FindType(HashedString typeId)
  {
    GeyserConfigurator.GeyserType type = (GeyserConfigurator.GeyserType) null;
    if (HashedString.op_Inequality(typeId, HashedString.Invalid))
      type = GeyserConfigurator.geyserTypes.Find((Predicate<GeyserConfigurator.GeyserType>) (t => HashedString.op_Equality(HashedString.op_Implicit(t.id), typeId)));
    if (type == null)
      Debug.LogError((object) string.Format("Tried finding a geyser with id {0} but it doesn't exist!", (object) typeId.ToString()));
    return type;
  }

  public GeyserConfigurator.GeyserInstanceConfiguration MakeConfiguration() => this.CreateRandomInstance(this.presetType, this.presetMin, this.presetMax);

  private GeyserConfigurator.GeyserInstanceConfiguration CreateRandomInstance(
    HashedString typeId,
    float min,
    float max)
  {
    KRandom randomSource = new KRandom(SaveLoader.Instance.clusterDetailSave.globalWorldSeed + (int) TransformExtensions.GetPosition(this.transform).x + (int) TransformExtensions.GetPosition(this.transform).y);
    return new GeyserConfigurator.GeyserInstanceConfiguration()
    {
      typeId = typeId,
      rateRoll = this.Roll(randomSource, min, max),
      iterationLengthRoll = this.Roll(randomSource, 0.0f, 1f),
      iterationPercentRoll = this.Roll(randomSource, min, max),
      yearLengthRoll = this.Roll(randomSource, 0.0f, 1f),
      yearPercentRoll = this.Roll(randomSource, min, max)
    };
  }

  private float Roll(KRandom randomSource, float min, float max) => (float) (randomSource.NextDouble() * ((double) max - (double) min)) + min;

  public enum GeyserShape
  {
    Gas,
    Liquid,
    Molten,
  }

  public class GeyserType
  {
    public string id;
    public HashedString idHash;
    public SimHashes element;
    public GeyserConfigurator.GeyserShape shape;
    public float temperature;
    public float minRatePerCycle;
    public float maxRatePerCycle;
    public float maxPressure;
    public SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid;
    public float minIterationLength;
    public float maxIterationLength;
    public float minIterationPercent;
    public float maxIterationPercent;
    public float minYearLength;
    public float maxYearLength;
    public float minYearPercent;
    public float maxYearPercent;
    public float geyserTemperature;
    public string DlcID;
    public const string BLANK_ID = "Blank";
    public const SimHashes BLANK_ELEMENT = SimHashes.Void;
    public const string BLANK_DLCID = "";

    public GeyserType(
      string id,
      SimHashes element,
      GeyserConfigurator.GeyserShape shape,
      float temperature,
      float minRatePerCycle,
      float maxRatePerCycle,
      float maxPressure,
      float minIterationLength = 60f,
      float maxIterationLength = 1140f,
      float minIterationPercent = 0.1f,
      float maxIterationPercent = 0.9f,
      float minYearLength = 15000f,
      float maxYearLength = 135000f,
      float minYearPercent = 0.4f,
      float maxYearPercent = 0.8f,
      float geyserTemperature = 372.15f,
      string DlcID = "")
    {
      this.id = id;
      this.idHash = HashedString.op_Implicit(id);
      this.element = element;
      this.shape = shape;
      this.temperature = temperature;
      this.minRatePerCycle = minRatePerCycle;
      this.maxRatePerCycle = maxRatePerCycle;
      this.maxPressure = maxPressure;
      this.minIterationLength = minIterationLength;
      this.maxIterationLength = maxIterationLength;
      this.minIterationPercent = minIterationPercent;
      this.maxIterationPercent = maxIterationPercent;
      this.minYearLength = minYearLength;
      this.maxYearLength = maxYearLength;
      this.minYearPercent = minYearPercent;
      this.maxYearPercent = maxYearPercent;
      this.DlcID = DlcID;
      this.geyserTemperature = geyserTemperature;
      if (GeyserConfigurator.geyserTypes == null)
        GeyserConfigurator.geyserTypes = new List<GeyserConfigurator.GeyserType>();
      GeyserConfigurator.geyserTypes.Add(this);
    }

    public GeyserConfigurator.GeyserType AddDisease(SimUtil.DiseaseInfo diseaseInfo)
    {
      this.diseaseInfo = diseaseInfo;
      return this;
    }

    public GeyserType()
    {
      this.id = "Blank";
      this.element = SimHashes.Void;
      this.temperature = 0.0f;
      this.minRatePerCycle = 0.0f;
      this.maxRatePerCycle = 0.0f;
      this.maxPressure = 0.0f;
      this.minIterationLength = 0.0f;
      this.maxIterationLength = 0.0f;
      this.minIterationPercent = 0.0f;
      this.maxIterationPercent = 0.0f;
      this.minYearLength = 0.0f;
      this.maxYearLength = 0.0f;
      this.minYearPercent = 0.0f;
      this.maxYearPercent = 0.0f;
      this.geyserTemperature = 0.0f;
      this.DlcID = "";
    }
  }

  [Serializable]
  public class GeyserInstanceConfiguration
  {
    public HashedString typeId;
    public float rateRoll;
    public float iterationLengthRoll;
    public float iterationPercentRoll;
    public float yearLengthRoll;
    public float yearPercentRoll;
    public float scaledRate;
    public float scaledIterationLength;
    public float scaledIterationPercent;
    public float scaledYearLength;
    public float scaledYearPercent;
    private bool didInit;
    private Geyser.GeyserModification modifier;

    public Geyser.GeyserModification GetModifier() => this.modifier;

    public void Init(bool reinit = false)
    {
      if (this.didInit && !reinit)
        return;
      this.didInit = true;
      this.scaledRate = this.Resample(this.rateRoll, this.geyserType.minRatePerCycle, this.geyserType.maxRatePerCycle);
      this.scaledIterationLength = this.Resample(this.iterationLengthRoll, this.geyserType.minIterationLength, this.geyserType.maxIterationLength);
      this.scaledIterationPercent = this.Resample(this.iterationPercentRoll, this.geyserType.minIterationPercent, this.geyserType.maxIterationPercent);
      this.scaledYearLength = this.Resample(this.yearLengthRoll, this.geyserType.minYearLength, this.geyserType.maxYearLength);
      this.scaledYearPercent = this.Resample(this.yearPercentRoll, this.geyserType.minYearPercent, this.geyserType.maxYearPercent);
    }

    public void SetModifier(Geyser.GeyserModification modifier) => this.modifier = modifier;

    public GeyserConfigurator.GeyserType geyserType => GeyserConfigurator.FindType(this.typeId);

    private float GetModifiedValue(
      float geyserVariable,
      float modifier,
      Geyser.ModificationMethod method)
    {
      float modifiedValue = geyserVariable;
      switch (method)
      {
        case Geyser.ModificationMethod.Values:
          modifiedValue += modifier;
          break;
        case Geyser.ModificationMethod.Percentages:
          modifiedValue += geyserVariable * modifier;
          break;
      }
      return modifiedValue;
    }

    public float GetMaxPressure() => this.GetModifiedValue(this.geyserType.maxPressure, this.modifier.maxPressureModifier, Geyser.maxPressureModificationMethod);

    public float GetIterationLength()
    {
      this.Init();
      return this.GetModifiedValue(this.scaledIterationLength, this.modifier.iterationDurationModifier, Geyser.IterationDurationModificationMethod);
    }

    public float GetIterationPercent()
    {
      this.Init();
      return Mathf.Clamp(this.GetModifiedValue(this.scaledIterationPercent, this.modifier.iterationPercentageModifier, Geyser.IterationPercentageModificationMethod), 0.0f, 1f);
    }

    public float GetOnDuration() => this.GetIterationLength() * this.GetIterationPercent();

    public float GetOffDuration() => this.GetIterationLength() * (1f - this.GetIterationPercent());

    public float GetMassPerCycle()
    {
      this.Init();
      return this.GetModifiedValue(this.scaledRate, this.modifier.massPerCycleModifier, Geyser.massModificationMethod);
    }

    public float GetEmitRate()
    {
      float num = 600f / this.GetIterationLength();
      return this.GetMassPerCycle() / num / this.GetOnDuration();
    }

    public float GetYearLength()
    {
      this.Init();
      return this.GetModifiedValue(this.scaledYearLength, this.modifier.yearDurationModifier, Geyser.yearDurationModificationMethod);
    }

    public float GetYearPercent()
    {
      this.Init();
      return Mathf.Clamp(this.GetModifiedValue(this.scaledYearPercent, this.modifier.yearPercentageModifier, Geyser.yearPercentageModificationMethod), 0.0f, 1f);
    }

    public float GetYearOnDuration() => this.GetYearLength() * this.GetYearPercent();

    public float GetYearOffDuration() => this.GetYearLength() * (1f - this.GetYearPercent());

    public SimHashes GetElement() => !this.modifier.modifyElement || this.modifier.newElement == (SimHashes) 0 ? this.geyserType.element : this.modifier.newElement;

    public float GetTemperature() => this.GetModifiedValue(this.geyserType.temperature, this.modifier.temperatureModifier, Geyser.temperatureModificationMethod);

    public byte GetDiseaseIdx() => this.geyserType.diseaseInfo.idx;

    public int GetDiseaseCount() => this.geyserType.diseaseInfo.count;

    public float GetAverageEmission()
    {
      float num = this.GetEmitRate() * this.GetOnDuration();
      return this.GetYearOnDuration() / this.GetIterationLength() * num / this.GetYearLength();
    }

    private float Resample(float t, float min, float max)
    {
      float num1 = 6f;
      float num2 = 0.002472623f;
      return (float) ((-(double) Mathf.Log((float) (1.0 / (double) (t * (float) (1.0 - (double) num2 * 2.0) + num2) - 1.0)) + (double) num1) / ((double) num1 * 2.0) * ((double) max - (double) min)) + min;
    }
  }
}
