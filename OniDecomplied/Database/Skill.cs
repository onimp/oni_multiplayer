// Decompiled with JetBrains decompiler
// Type: Database.Skill
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;

namespace Database
{
  public class Skill : Resource
  {
    public string description;
    public string dlcId;
    public string skillGroup;
    public string hat;
    public string badge;
    public int tier;
    public bool deprecated;
    public List<SkillPerk> perks;
    public List<string> priorSkills;

    public Skill(
      string id,
      string name,
      string description,
      string dlcId,
      int tier,
      string hat,
      string badge,
      string skillGroup,
      List<SkillPerk> perks = null,
      List<string> priorSkills = null)
      : base(id, name)
    {
      this.description = description;
      this.dlcId = dlcId;
      this.tier = tier;
      this.hat = hat;
      this.badge = badge;
      this.skillGroup = skillGroup;
      this.perks = perks;
      if (this.perks == null)
        this.perks = new List<SkillPerk>();
      this.priorSkills = priorSkills;
      if (this.priorSkills != null)
        return;
      this.priorSkills = new List<string>();
    }

    public int GetMoraleExpectation() => SKILLS.SKILL_TIER_MORALE_COST[this.tier];

    public bool GivesPerk(SkillPerk perk) => this.perks.Contains(perk);

    public bool GivesPerk(HashedString perkId)
    {
      foreach (Resource perk in this.perks)
      {
        if (HashedString.op_Equality(perk.IdHash, perkId))
          return true;
      }
      return false;
    }
  }
}
