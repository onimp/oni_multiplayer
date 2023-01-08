// Decompiled with JetBrains decompiler
// Type: Database.SkillGroups
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System.Collections.Generic;

namespace Database
{
  public class SkillGroups : ResourceSet<SkillGroup>
  {
    public SkillGroup Mining;
    public SkillGroup Building;
    public SkillGroup Farming;
    public SkillGroup Ranching;
    public SkillGroup Cooking;
    public SkillGroup Art;
    public SkillGroup Research;
    public SkillGroup Rocketry;
    public SkillGroup Suits;
    public SkillGroup Hauling;
    public SkillGroup Technicals;
    public SkillGroup MedicalAid;
    public SkillGroup Basekeeping;

    public SkillGroups(ResourceSet parent)
      : base(nameof (SkillGroups), parent)
    {
      this.Mining = this.Add(new SkillGroup(nameof (Mining), Db.Get().ChoreGroups.Dig.Id, (string) DUPLICANTS.CHOREGROUPS.DIG.NAME, "icon_errand_dig", "icon_archetype_dig"));
      this.Mining.relevantAttributes = new List<Attribute>()
      {
        Db.Get().ChoreGroups.Dig.attribute
      };
      this.Mining.requiredChoreGroups = new List<string>()
      {
        Db.Get().ChoreGroups.Dig.Id
      };
      this.Building = this.Add(new SkillGroup(nameof (Building), Db.Get().ChoreGroups.Build.Id, (string) DUPLICANTS.CHOREGROUPS.BUILD.NAME, "status_item_pending_repair", "icon_archetype_build"));
      this.Building.relevantAttributes = new List<Attribute>()
      {
        Db.Get().ChoreGroups.Build.attribute
      };
      this.Building.requiredChoreGroups = new List<string>()
      {
        Db.Get().ChoreGroups.Build.Id
      };
      this.Farming = this.Add(new SkillGroup(nameof (Farming), Db.Get().ChoreGroups.Farming.Id, (string) DUPLICANTS.CHOREGROUPS.FARMING.NAME, "icon_errand_farm", "icon_archetype_farm"));
      this.Farming.relevantAttributes = new List<Attribute>()
      {
        Db.Get().ChoreGroups.Farming.attribute
      };
      this.Farming.requiredChoreGroups = new List<string>()
      {
        Db.Get().ChoreGroups.Farming.Id
      };
      this.Ranching = this.Add(new SkillGroup(nameof (Ranching), Db.Get().ChoreGroups.Ranching.Id, (string) DUPLICANTS.CHOREGROUPS.RANCHING.NAME, "icon_errand_ranch", "icon_archetype_ranch"));
      this.Ranching.relevantAttributes = new List<Attribute>()
      {
        Db.Get().ChoreGroups.Ranching.attribute
      };
      this.Ranching.requiredChoreGroups = new List<string>()
      {
        Db.Get().ChoreGroups.Ranching.Id
      };
      this.Cooking = this.Add(new SkillGroup(nameof (Cooking), Db.Get().ChoreGroups.Cook.Id, (string) DUPLICANTS.CHOREGROUPS.COOK.NAME, "icon_errand_cook", "icon_archetype_cook"));
      this.Cooking.relevantAttributes = new List<Attribute>()
      {
        Db.Get().ChoreGroups.Cook.attribute
      };
      this.Cooking.requiredChoreGroups = new List<string>()
      {
        Db.Get().ChoreGroups.Cook.Id
      };
      this.Art = this.Add(new SkillGroup(nameof (Art), Db.Get().ChoreGroups.Art.Id, (string) DUPLICANTS.CHOREGROUPS.ART.NAME, "icon_errand_art", "icon_archetype_art"));
      this.Art.relevantAttributes = new List<Attribute>()
      {
        Db.Get().ChoreGroups.Art.attribute
      };
      this.Art.requiredChoreGroups = new List<string>()
      {
        Db.Get().ChoreGroups.Art.Id
      };
      this.Research = this.Add(new SkillGroup(nameof (Research), Db.Get().ChoreGroups.Research.Id, (string) DUPLICANTS.CHOREGROUPS.RESEARCH.NAME, "icon_errand_research", "icon_archetype_research"));
      this.Research.relevantAttributes = new List<Attribute>()
      {
        Db.Get().ChoreGroups.Research.attribute
      };
      this.Research.requiredChoreGroups = new List<string>()
      {
        Db.Get().ChoreGroups.Research.Id
      };
      if (DlcManager.FeatureClusterSpaceEnabled())
      {
        this.Rocketry = this.Add(new SkillGroup(nameof (Rocketry), Db.Get().ChoreGroups.Rocketry.Id, (string) DUPLICANTS.CHOREGROUPS.ROCKETRY.NAME, "icon_errand_rocketry", "icon_archetype_rocketry"));
        this.Rocketry.relevantAttributes = new List<Attribute>()
        {
          Db.Get().ChoreGroups.Rocketry.attribute
        };
        this.Rocketry.requiredChoreGroups = new List<string>()
        {
          Db.Get().ChoreGroups.Rocketry.Id
        };
      }
      this.Suits = this.Add(new SkillGroup(nameof (Suits), "", (string) DUPLICANTS.ROLES.GROUPS.SUITS, "suit_overlay_icon", "icon_archetype_astronaut"));
      this.Suits.relevantAttributes = new List<Attribute>()
      {
        Db.Get().Attributes.Athletics
      };
      this.Suits.requiredChoreGroups = new List<string>()
      {
        Db.Get().ChoreGroups.Hauling.Id
      };
      this.Hauling = this.Add(new SkillGroup(nameof (Hauling), Db.Get().ChoreGroups.Hauling.Id, (string) DUPLICANTS.CHOREGROUPS.HAULING.NAME, "icon_errand_supply", "icon_archetype_storage"));
      this.Hauling.relevantAttributes = new List<Attribute>()
      {
        Db.Get().ChoreGroups.Hauling.attribute
      };
      this.Hauling.requiredChoreGroups = new List<string>()
      {
        Db.Get().ChoreGroups.Hauling.Id
      };
      this.Technicals = this.Add(new SkillGroup(nameof (Technicals), Db.Get().ChoreGroups.MachineOperating.Id, (string) DUPLICANTS.CHOREGROUPS.MACHINEOPERATING.NAME, "icon_errand_operate", "icon_archetype_operate"));
      this.Technicals.relevantAttributes = new List<Attribute>()
      {
        Db.Get().ChoreGroups.MachineOperating.attribute
      };
      this.Technicals.requiredChoreGroups = new List<string>()
      {
        Db.Get().ChoreGroups.MachineOperating.Id
      };
      this.MedicalAid = this.Add(new SkillGroup(nameof (MedicalAid), Db.Get().ChoreGroups.MedicalAid.Id, (string) DUPLICANTS.CHOREGROUPS.MEDICALAID.NAME, "icon_errand_care", "icon_archetype_care"));
      this.MedicalAid.relevantAttributes = new List<Attribute>()
      {
        Db.Get().ChoreGroups.MedicalAid.attribute
      };
      this.Basekeeping = this.Add(new SkillGroup(nameof (Basekeeping), Db.Get().ChoreGroups.Basekeeping.Id, (string) DUPLICANTS.CHOREGROUPS.BASEKEEPING.NAME, "icon_errand_tidy", "icon_archetype_tidy"));
      this.Basekeeping.relevantAttributes = new List<Attribute>()
      {
        Db.Get().ChoreGroups.Basekeeping.attribute
      };
      this.Basekeeping.requiredChoreGroups = new List<string>()
      {
        Db.Get().ChoreGroups.Basekeeping.Id
      };
    }
  }
}
