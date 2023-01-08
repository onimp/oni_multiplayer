// Decompiled with JetBrains decompiler
// Type: DiseaseContainers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei;
using Klei.AI.DiseaseGrowthRules;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseContainers : KGameObjectSplitComponentManager<DiseaseHeader, DiseaseContainer>
{
  public HandleVector<int>.Handle Add(GameObject go, byte disease_idx, int disease_count)
  {
    DiseaseHeader header = new DiseaseHeader()
    {
      diseaseIdx = disease_idx,
      diseaseCount = disease_count,
      primaryElement = go.GetComponent<PrimaryElement>()
    };
    DiseaseContainer container = new DiseaseContainer(go, header.primaryElement.Element.idx);
    if (disease_idx != byte.MaxValue)
      this.EvaluateGrowthConstants(header, ref container);
    return this.Add(go, header, ref container);
  }

  protected virtual void OnCleanUp(HandleVector<int>.Handle h)
  {
    AutoDisinfectable autoDisinfectable = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).GetPayload(h).autoDisinfectable;
    if (Object.op_Inequality((Object) autoDisinfectable, (Object) null))
      AutoDisinfectableManager.Instance.RemoveAutoDisinfectable(autoDisinfectable);
    ((KSplitComponentManager<DiseaseHeader, DiseaseContainer>) this).OnCleanUp(h);
  }

  public virtual void Sim200ms(float dt)
  {
    ListPool<int, DiseaseContainers>.PooledList pooledList = ListPool<int, DiseaseContainers>.Allocate();
    ((List<int>) pooledList).Capacity = Math.Max(((List<int>) pooledList).Capacity, ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).headers.Count);
    for (int index = 0; index < ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).headers.Count; ++index)
    {
      DiseaseHeader header = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).headers[index];
      if (header.diseaseIdx != byte.MaxValue && Object.op_Inequality((Object) header.primaryElement, (Object) null))
        ((List<int>) pooledList).Add(index);
    }
    bool radiation_enabled = Sim.IsRadiationEnabled();
    foreach (int index in (List<int>) pooledList)
    {
      DiseaseContainer payload = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).payloads[index];
      DiseaseHeader header = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).headers[index];
      Klei.AI.Disease disease = Db.Get().Diseases[(int) header.diseaseIdx];
      float num1 = DiseaseContainers.CalculateDelta(header, ref payload, disease, dt, radiation_enabled) + payload.accumulatedError;
      int num2 = (int) num1;
      payload.accumulatedError = num1 - (float) num2;
      if (header.diseaseCount > payload.overpopulationCount != header.diseaseCount + num2 > payload.overpopulationCount)
        this.EvaluateGrowthConstants(header, ref payload);
      header.diseaseCount += num2;
      if (header.diseaseCount <= 0)
      {
        payload.accumulatedError = 0.0f;
        header.diseaseCount = 0;
        header.diseaseIdx = byte.MaxValue;
      }
      ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).headers[index] = header;
      ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).payloads[index] = payload;
    }
    pooledList.Recycle();
  }

  private static float CalculateDelta(
    DiseaseHeader header,
    ref DiseaseContainer container,
    Klei.AI.Disease disease,
    float dt,
    bool radiation_enabled)
  {
    return DiseaseContainers.CalculateDelta(header.diseaseCount, container.elemIdx, header.primaryElement.Mass, Grid.PosToCell(TransformExtensions.GetPosition(header.primaryElement.transform)), header.primaryElement.Temperature, container.instanceGrowthRate, disease, dt, radiation_enabled);
  }

  public static float CalculateDelta(
    int disease_count,
    ushort element_idx,
    float mass,
    int environment_cell,
    float temperature,
    float tags_multiplier_base,
    Klei.AI.Disease disease,
    float dt,
    bool radiation_enabled)
  {
    float num1 = 0.0f + disease.elemGrowthInfo[(int) element_idx].CalculateDiseaseCountDelta(disease_count, mass, dt);
    float growthRate = Klei.AI.Disease.HalfLifeToGrowthRate(Klei.AI.Disease.CalculateRangeHalfLife(temperature, ref disease.temperatureRange, ref disease.temperatureHalfLives), dt);
    float num2 = num1 + ((float) disease_count * growthRate - (float) disease_count);
    float num3 = Mathf.Pow(tags_multiplier_base, dt);
    float delta = num2 + ((float) disease_count * num3 - (float) disease_count);
    if (Grid.IsValidCell(environment_cell))
    {
      ushort index = Grid.ElementIdx[environment_cell];
      ElemExposureInfo elemExposureInfo = disease.elemExposureInfo[(int) index];
      delta += elemExposureInfo.CalculateExposureDiseaseCountDelta(disease_count, dt);
      if (radiation_enabled)
      {
        float num4 = Grid.Radiation[environment_cell];
        if ((double) num4 > 0.0)
          delta -= num4 * disease.radiationKillRate;
      }
    }
    return delta;
  }

  public int ModifyDiseaseCount(HandleVector<int>.Handle h, int disease_count_delta)
  {
    DiseaseHeader header = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).GetHeader(h);
    header.diseaseCount = Math.Max(0, header.diseaseCount + disease_count_delta);
    if (header.diseaseCount == 0)
    {
      header.diseaseIdx = byte.MaxValue;
      DiseaseContainer payload = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).GetPayload(h) with
      {
        accumulatedError = 0.0f
      };
      ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).SetPayload(h, ref payload);
    }
    ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).SetHeader(h, header);
    return header.diseaseCount;
  }

  public int AddDisease(HandleVector<int>.Handle h, byte disease_idx, int disease_count)
  {
    DiseaseHeader header;
    DiseaseContainer container;
    ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).GetData(h, ref header, ref container);
    SimUtil.DiseaseInfo finalDiseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, disease_count, header.diseaseIdx, header.diseaseCount);
    int num = (int) header.diseaseIdx != (int) finalDiseaseInfo.idx ? 1 : 0;
    header.diseaseIdx = finalDiseaseInfo.idx;
    header.diseaseCount = finalDiseaseInfo.count;
    if (num != 0 && finalDiseaseInfo.idx != byte.MaxValue)
    {
      this.EvaluateGrowthConstants(header, ref container);
      ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).SetData(h, header, ref container);
    }
    else
      ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).SetHeader(h, header);
    if (num != 0)
      header.primaryElement.Trigger(-283306403, (object) null);
    return header.diseaseCount;
  }

  private void GetVisualDiseaseIdxAndCount(
    DiseaseHeader header,
    ref DiseaseContainer payload,
    out int disease_idx,
    out int disease_count)
  {
    if (Object.op_Equality((Object) payload.visualDiseaseProvider, (Object) null))
    {
      disease_idx = (int) header.diseaseIdx;
      disease_count = header.diseaseCount;
    }
    else
    {
      disease_idx = (int) byte.MaxValue;
      disease_count = 0;
      HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(payload.visualDiseaseProvider);
      if (!HandleVector<int>.Handle.op_Inequality(handle, HandleVector<int>.InvalidHandle))
        return;
      DiseaseHeader header1 = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) GameComps.DiseaseContainers).GetHeader(handle);
      disease_idx = (int) header1.diseaseIdx;
      disease_count = header1.diseaseCount;
    }
  }

  public void UpdateOverlayColours()
  {
    GridArea visibleArea = GridVisibleArea.GetVisibleArea();
    Diseases diseases = Db.Get().Diseases;
    Color32 color32_1;
    // ISSUE: explicit constructor call
    ((Color32) ref color32_1).\u002Ector((byte) 0, (byte) 0, (byte) 0, byte.MaxValue);
    for (int index1 = 0; index1 < ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).headers.Count; ++index1)
    {
      DiseaseContainer payload = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).payloads[index1];
      DiseaseHeader header1 = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).headers[index1];
      KBatchedAnimController controller = payload.controller;
      if (Object.op_Inequality((Object) controller, (Object) null))
      {
        Color32 color32_2 = color32_1;
        Vector3 position = TransformExtensions.GetPosition(((Component) controller).transform);
        if (Vector2I.op_LessThanOrEqual(visibleArea.Min, Vector2.op_Implicit(position)) && Vector2I.op_LessThanOrEqual(Vector2.op_Implicit(position), visibleArea.Max))
        {
          int count = 0;
          int disease_idx = (int) byte.MaxValue;
          int disease_count = 0;
          this.GetVisualDiseaseIdxAndCount(header1, ref payload, out disease_idx, out disease_count);
          if (disease_idx != (int) byte.MaxValue)
          {
            color32_2 = GlobalAssets.Instance.colorSet.GetColorByName(diseases[disease_idx].overlayColourName);
            count = disease_count;
          }
          if (payload.isContainer)
          {
            List<GameObject> items = ((Component) header1.primaryElement).GetComponent<Storage>().items;
            for (int index2 = 0; index2 < items.Count; ++index2)
            {
              GameObject gameObject = items[index2];
              if (Object.op_Inequality((Object) gameObject, (Object) null))
              {
                HandleVector<int>.Handle handle = this.GetHandle(gameObject);
                if (handle.IsValid())
                {
                  DiseaseHeader header2 = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).GetHeader(handle);
                  if (header2.diseaseCount > count && header2.diseaseIdx != byte.MaxValue)
                  {
                    count = header2.diseaseCount;
                    color32_2 = GlobalAssets.Instance.colorSet.GetColorByName(diseases[(int) header2.diseaseIdx].overlayColourName);
                  }
                }
              }
            }
          }
          color32_2.a = SimUtil.DiseaseCountToAlpha254(count);
          if (payload.conduitType != ConduitType.None)
          {
            ConduitFlow.ConduitContents contents = Conduit.GetFlowManager(payload.conduitType).GetContents(Grid.PosToCell(position));
            if (contents.diseaseIdx != byte.MaxValue && contents.diseaseCount > count)
            {
              int diseaseCount = contents.diseaseCount;
              color32_2 = GlobalAssets.Instance.colorSet.GetColorByName(diseases[(int) contents.diseaseIdx].overlayColourName);
              color32_2.a = byte.MaxValue;
            }
          }
        }
        controller.OverlayColour = Color32.op_Implicit(color32_2);
      }
    }
  }

  private void EvaluateGrowthConstants(DiseaseHeader header, ref DiseaseContainer container)
  {
    Klei.AI.Disease disease = Db.Get().Diseases[(int) header.diseaseIdx];
    KPrefabID component = ((Component) header.primaryElement).GetComponent<KPrefabID>();
    ElemGrowthInfo elemGrowthInfo = disease.elemGrowthInfo[(int) header.diseaseIdx];
    container.overpopulationCount = (int) ((double) elemGrowthInfo.maxCountPerKG * (double) header.primaryElement.Mass);
    container.instanceGrowthRate = disease.GetGrowthRateForTags(component.Tags, header.diseaseCount > container.overpopulationCount);
  }

  public virtual void Clear()
  {
    ((KSplitComponentManager<DiseaseHeader, DiseaseContainer>) this).Clear();
    for (int index = 0; index < ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).payloads.Count; ++index)
      ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).payloads[index].Clear();
    ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).headers.Clear();
    ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) this).payloads.Clear();
    ((KCompactedVectorBase) this).handles.Clear();
  }
}
