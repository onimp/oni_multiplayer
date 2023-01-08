// Decompiled with JetBrains decompiler
// Type: ResearchTypes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class ResearchTypes
{
  public List<ResearchType> Types = new List<ResearchType>();

  public ResearchTypes()
  {
    this.Types.Add(new ResearchType("basic", (string) RESEARCH.TYPES.ALPHA.NAME, (string) RESEARCH.TYPES.ALPHA.DESC, Assets.GetSprite(HashedString.op_Implicit("research_type_alpha_icon")), new Color(0.596078455f, 0.6666667f, 0.9137255f), new Recipe.Ingredient[1]
    {
      new Recipe.Ingredient(TagExtensions.ToTag("Dirt"), 100f)
    }, 600f, HashedString.op_Implicit("research_center_kanim"), new string[1]
    {
      "ResearchCenter"
    }, (string) RESEARCH.TYPES.ALPHA.RECIPEDESC));
    this.Types.Add(new ResearchType("advanced", (string) RESEARCH.TYPES.BETA.NAME, (string) RESEARCH.TYPES.BETA.DESC, Assets.GetSprite(HashedString.op_Implicit("research_type_beta_icon")), new Color(0.6f, 0.384313732f, 0.5686275f), new Recipe.Ingredient[1]
    {
      new Recipe.Ingredient(TagExtensions.ToTag("Water"), 25f)
    }, 1200f, HashedString.op_Implicit("research_center_kanim"), new string[1]
    {
      "AdvancedResearchCenter"
    }, (string) RESEARCH.TYPES.BETA.RECIPEDESC));
    this.Types.Add(new ResearchType("space", (string) RESEARCH.TYPES.GAMMA.NAME, (string) RESEARCH.TYPES.GAMMA.DESC, Assets.GetSprite(HashedString.op_Implicit("research_type_gamma_icon")), Color32.op_Implicit(new Color32((byte) 240, (byte) 141, (byte) 44, byte.MaxValue)), (Recipe.Ingredient[]) null, 2400f, HashedString.op_Implicit("research_center_kanim"), new string[1]
    {
      "CosmicResearchCenter"
    }, (string) RESEARCH.TYPES.GAMMA.RECIPEDESC));
    this.Types.Add(new ResearchType("nuclear", (string) RESEARCH.TYPES.DELTA.NAME, (string) RESEARCH.TYPES.DELTA.DESC, Assets.GetSprite(HashedString.op_Implicit("research_type_delta_icon")), Color32.op_Implicit(new Color32((byte) 231, (byte) 210, (byte) 17, byte.MaxValue)), (Recipe.Ingredient[]) null, 2400f, HashedString.op_Implicit("research_center_kanim"), new string[1]
    {
      "NuclearResearchCenter"
    }, (string) RESEARCH.TYPES.DELTA.RECIPEDESC));
    this.Types.Add(new ResearchType("orbital", (string) RESEARCH.TYPES.ORBITAL.NAME, (string) RESEARCH.TYPES.ORBITAL.DESC, Assets.GetSprite(HashedString.op_Implicit("research_type_orbital_icon")), Color32.op_Implicit(new Color32((byte) 240, (byte) 141, (byte) 44, byte.MaxValue)), (Recipe.Ingredient[]) null, 2400f, HashedString.op_Implicit("research_center_kanim"), new string[2]
    {
      "OrbitalResearchCenter",
      "DLC1CosmicResearchCenter"
    }, (string) RESEARCH.TYPES.ORBITAL.RECIPEDESC));
  }

  public ResearchType GetResearchType(string id)
  {
    foreach (ResearchType type in this.Types)
    {
      if (id == type.id)
        return type;
    }
    Debug.LogWarning((object) string.Format("No research with type id {0} found", (object) id));
    return (ResearchType) null;
  }

  public class ID
  {
    public const string BASIC = "basic";
    public const string ADVANCED = "advanced";
    public const string SPACE = "space";
    public const string NUCLEAR = "nuclear";
    public const string ORBITAL = "orbital";
  }
}
