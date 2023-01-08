// Decompiled with JetBrains decompiler
// Type: ResearchPointInventory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class ResearchPointInventory
{
  public Dictionary<string, float> PointsByTypeID = new Dictionary<string, float>();

  public ResearchPointInventory()
  {
    foreach (ResearchType type in Research.Instance.researchTypes.Types)
      this.PointsByTypeID.Add(type.id, 0.0f);
  }

  public void AddResearchPoints(string researchTypeID, float points)
  {
    if (!this.PointsByTypeID.ContainsKey(researchTypeID))
      Debug.LogWarning((object) ("Research inventory is missing research point key " + researchTypeID));
    else
      this.PointsByTypeID[researchTypeID] += points;
  }

  public void RemoveResearchPoints(string researchTypeID, float points) => this.AddResearchPoints(researchTypeID, -points);

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    foreach (ResearchType type in Research.Instance.researchTypes.Types)
    {
      if (!this.PointsByTypeID.ContainsKey(type.id))
        this.PointsByTypeID.Add(type.id, 0.0f);
    }
  }
}
