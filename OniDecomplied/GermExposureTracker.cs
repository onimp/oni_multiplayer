// Decompiled with JetBrains decompiler
// Type: GermExposureTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using ProcGen;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/GermExposureTracker")]
public class GermExposureTracker : KMonoBehaviour
{
  public static GermExposureTracker Instance;
  [Serialize]
  private Dictionary<HashedString, float> accumulation = new Dictionary<HashedString, float>();
  private SeededRandom rng;
  private List<GermExposureTracker.WeightedExposure> exposure_candidates = new List<GermExposureTracker.WeightedExposure>();

  protected virtual void OnPrefabInit()
  {
    Debug.Assert(Object.op_Equality((Object) GermExposureTracker.Instance, (Object) null));
    GermExposureTracker.Instance = this;
  }

  protected virtual void OnSpawn() => this.rng = new SeededRandom(GameClock.Instance.GetCycle());

  protected virtual void OnForcedCleanUp() => GermExposureTracker.Instance = (GermExposureTracker) null;

  public void AddExposure(ExposureType exposure_type, float amount)
  {
    float num1;
    this.accumulation.TryGetValue(HashedString.op_Implicit(exposure_type.germ_id), out num1);
    float num2 = num1 + amount;
    if ((double) num2 > 1.0)
    {
      foreach (MinionIdentity cmp in Components.LiveMinionIdentities.Items)
      {
        GermExposureMonitor.Instance smi = ((Component) cmp).GetSMI<GermExposureMonitor.Instance>();
        if (smi.GetExposureState(exposure_type.germ_id) == GermExposureMonitor.ExposureState.Exposed)
        {
          float exposureWeight = ((Component) cmp).GetSMI<GermExposureMonitor.Instance>().GetExposureWeight(exposure_type.germ_id);
          if ((double) exposureWeight > 0.0)
            this.exposure_candidates.Add(new GermExposureTracker.WeightedExposure()
            {
              weight = exposureWeight,
              monitor = smi
            });
        }
      }
      while ((double) num2 > 1.0)
      {
        --num2;
        if (this.exposure_candidates.Count > 0)
        {
          GermExposureTracker.WeightedExposure weightedExposure = WeightedRandom.Choose<GermExposureTracker.WeightedExposure>(this.exposure_candidates, this.rng);
          this.exposure_candidates.Remove(weightedExposure);
          weightedExposure.monitor.ContractGerms(exposure_type.germ_id);
        }
      }
    }
    this.accumulation[HashedString.op_Implicit(exposure_type.germ_id)] = num2;
    this.exposure_candidates.Clear();
  }

  private class WeightedExposure : IWeighted
  {
    public GermExposureMonitor.Instance monitor;

    public float weight { get; set; }
  }
}
