// Decompiled with JetBrains decompiler
// Type: Database.ArtifactDropRate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Database
{
  public class ArtifactDropRate : Resource
  {
    public List<Tuple<ArtifactTier, float>> rates = new List<Tuple<ArtifactTier, float>>();
    public float totalWeight;

    public void AddItem(ArtifactTier tier, float weight)
    {
      this.rates.Add(new Tuple<ArtifactTier, float>(tier, weight));
      this.totalWeight += weight;
    }

    public float GetTierWeight(ArtifactTier tier)
    {
      float tierWeight = 0.0f;
      foreach (Tuple<ArtifactTier, float> rate in this.rates)
      {
        if (rate.first == tier)
          tierWeight = rate.second;
      }
      return tierWeight;
    }
  }
}
