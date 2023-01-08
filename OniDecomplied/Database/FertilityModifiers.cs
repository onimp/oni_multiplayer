// Decompiled with JetBrains decompiler
// Type: Database.FertilityModifiers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;

namespace Database
{
  public class FertilityModifiers : ResourceSet<FertilityModifier>
  {
    public List<FertilityModifier> GetForTag(Tag searchTag)
    {
      List<FertilityModifier> forTag = new List<FertilityModifier>();
      foreach (FertilityModifier resource in this.resources)
      {
        if (Tag.op_Equality(resource.TargetTag, searchTag))
          forTag.Add(resource);
      }
      return forTag;
    }
  }
}
