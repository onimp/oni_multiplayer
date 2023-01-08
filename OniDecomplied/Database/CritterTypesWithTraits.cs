// Decompiled with JetBrains decompiler
// Type: Database.CritterTypesWithTraits
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Database
{
  public class CritterTypesWithTraits : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    public Dictionary<Tag, bool> critterTypesToCheck = new Dictionary<Tag, bool>();
    private Tag trait;
    private bool hasTrait;
    private Dictionary<Tag, bool> revisedCritterTypesToCheckState = new Dictionary<Tag, bool>();

    public CritterTypesWithTraits(List<Tag> critterTypes)
    {
      foreach (Tag critterType in critterTypes)
      {
        if (!this.critterTypesToCheck.ContainsKey(critterType))
          this.critterTypesToCheck.Add(critterType, false);
      }
      this.hasTrait = false;
      this.trait = GameTags.Creatures.Wild;
    }

    public override bool Success()
    {
      HashSet<Tag> tamedCritterTypes = ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().tamedCritterTypes;
      bool flag = true;
      foreach (KeyValuePair<Tag, bool> keyValuePair in this.critterTypesToCheck)
        flag = flag && tamedCritterTypes.Contains(keyValuePair.Key);
      this.UpdateSavedState();
      return flag;
    }

    public void UpdateSavedState()
    {
      this.revisedCritterTypesToCheckState.Clear();
      HashSet<Tag> tamedCritterTypes = ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().tamedCritterTypes;
      foreach (KeyValuePair<Tag, bool> keyValuePair in this.critterTypesToCheck)
        this.revisedCritterTypesToCheckState.Add(keyValuePair.Key, tamedCritterTypes.Contains(keyValuePair.Key));
      foreach (KeyValuePair<Tag, bool> keyValuePair in this.revisedCritterTypesToCheckState)
        this.critterTypesToCheck[keyValuePair.Key] = keyValuePair.Value;
    }

    public void Deserialize(IReader reader)
    {
      this.critterTypesToCheck = new Dictionary<Tag, bool>();
      int num = reader.ReadInt32();
      for (int index = 0; index < num; ++index)
        this.critterTypesToCheck.Add(new Tag(reader.ReadKleiString()), reader.ReadByte() > (byte) 0);
      this.hasTrait = reader.ReadByte() > (byte) 0;
      this.trait = GameTags.Creatures.Wild;
    }
  }
}
