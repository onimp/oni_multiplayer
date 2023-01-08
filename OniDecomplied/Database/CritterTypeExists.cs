// Decompiled with JetBrains decompiler
// Type: Database.CritterTypeExists
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
  public class CritterTypeExists : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    private List<Tag> critterTypes = new List<Tag>();

    public CritterTypeExists(List<Tag> critterTypes) => this.critterTypes = critterTypes;

    public override bool Success()
    {
      foreach (Component cmp in Components.Capturables.Items)
      {
        if (this.critterTypes.Contains(cmp.PrefabID()))
          return true;
      }
      return false;
    }

    public void Deserialize(IReader reader)
    {
      int capacity = reader.ReadInt32();
      this.critterTypes = new List<Tag>(capacity);
      for (int index = 0; index < capacity; ++index)
        this.critterTypes.Add(new Tag(reader.ReadKleiString()));
    }

    public override string GetProgress(bool complete) => (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.HATCH_A_MORPH;
  }
}
