// Decompiled with JetBrains decompiler
// Type: Database.Skills
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;

namespace Database
{
  public class Skills : ResourceSet<Skill>
  {
    public Skill Mining1;
    public Skill Mining2;
    public Skill Mining3;
    public Skill Mining4;
    public Skill Building1;
    public Skill Building2;
    public Skill Building3;
    public Skill Farming1;
    public Skill Farming2;
    public Skill Farming3;
    public Skill Ranching1;
    public Skill Ranching2;
    public Skill Researching1;
    public Skill Researching2;
    public Skill Researching3;
    public Skill Researching4;
    public Skill AtomicResearch;
    public Skill SpaceResearch;
    public Skill Astronomy;
    public Skill RocketPiloting1;
    public Skill RocketPiloting2;
    public Skill Cooking1;
    public Skill Cooking2;
    public Skill Arting1;
    public Skill Arting2;
    public Skill Arting3;
    public Skill Hauling1;
    public Skill Hauling2;
    public Skill ThermalSuits;
    public Skill Suits1;
    public Skill Technicals1;
    public Skill Technicals2;
    public Skill Engineering1;
    public Skill Basekeeping1;
    public Skill Basekeeping2;
    public Skill Astronauting1;
    public Skill Astronauting2;
    public Skill Medicine1;
    public Skill Medicine2;
    public Skill Medicine3;

    public Skills(ResourceSet parent)
      : base(nameof (Skills), parent)
    {
      this.Mining1 = this.AddSkill(new Skill(nameof (Mining1), (string) DUPLICANTS.ROLES.JUNIOR_MINER.NAME, (string) DUPLICANTS.ROLES.JUNIOR_MINER.DESCRIPTION, "", 0, "hat_role_mining1", "skillbadge_role_mining1", Db.Get().SkillGroups.Mining.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseDigSpeedSmall,
        Db.Get().SkillPerks.CanDigVeryFirm
      }));
      this.Mining2 = this.AddSkill(new Skill(nameof (Mining2), (string) DUPLICANTS.ROLES.MINER.NAME, (string) DUPLICANTS.ROLES.MINER.DESCRIPTION, "", 1, "hat_role_mining2", "skillbadge_role_mining2", Db.Get().SkillGroups.Mining.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseDigSpeedMedium,
        Db.Get().SkillPerks.CanDigNearlyImpenetrable
      }, new List<string>() { this.Mining1.Id }));
      this.Mining3 = this.AddSkill(new Skill(nameof (Mining3), (string) DUPLICANTS.ROLES.SENIOR_MINER.NAME, (string) DUPLICANTS.ROLES.SENIOR_MINER.DESCRIPTION, "", 2, "hat_role_mining3", "skillbadge_role_mining3", Db.Get().SkillGroups.Mining.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseDigSpeedLarge,
        Db.Get().SkillPerks.CanDigSuperDuperHard
      }, new List<string>() { this.Mining2.Id }));
      this.Mining4 = this.AddSkill(new Skill(nameof (Mining4), (string) DUPLICANTS.ROLES.MASTER_MINER.NAME, (string) DUPLICANTS.ROLES.MASTER_MINER.DESCRIPTION, "EXPANSION1_ID", 3, "hat_role_mining4", "skillbadge_role_mining4", Db.Get().SkillGroups.Mining.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.CanDigRadioactiveMaterials
      }, new List<string>() { this.Mining3.Id }));
      this.Building1 = this.AddSkill(new Skill(nameof (Building1), (string) DUPLICANTS.ROLES.JUNIOR_BUILDER.NAME, (string) DUPLICANTS.ROLES.JUNIOR_BUILDER.DESCRIPTION, "", 0, "hat_role_building1", "skillbadge_role_building1", Db.Get().SkillGroups.Building.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseConstructionSmall
      }));
      this.Building2 = this.AddSkill(new Skill(nameof (Building2), (string) DUPLICANTS.ROLES.BUILDER.NAME, (string) DUPLICANTS.ROLES.BUILDER.DESCRIPTION, "", 1, "hat_role_building2", "skillbadge_role_building2", Db.Get().SkillGroups.Building.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseConstructionMedium
      }, new List<string>() { this.Building1.Id }));
      this.Building3 = this.AddSkill(new Skill(nameof (Building3), (string) DUPLICANTS.ROLES.SENIOR_BUILDER.NAME, (string) DUPLICANTS.ROLES.SENIOR_BUILDER.DESCRIPTION, "", 2, "hat_role_building3", "skillbadge_role_building3", Db.Get().SkillGroups.Building.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseConstructionLarge,
        Db.Get().SkillPerks.CanDemolish
      }, new List<string>() { this.Building2.Id }));
      this.Farming1 = this.AddSkill(new Skill(nameof (Farming1), (string) DUPLICANTS.ROLES.JUNIOR_FARMER.NAME, (string) DUPLICANTS.ROLES.JUNIOR_FARMER.DESCRIPTION, "", 0, "hat_role_farming1", "skillbadge_role_farming1", Db.Get().SkillGroups.Farming.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseBotanySmall
      }));
      this.Farming2 = this.AddSkill(new Skill(nameof (Farming2), (string) DUPLICANTS.ROLES.FARMER.NAME, (string) DUPLICANTS.ROLES.FARMER.DESCRIPTION, "", 1, "hat_role_farming2", "skillbadge_role_farming2", Db.Get().SkillGroups.Farming.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseBotanyMedium,
        Db.Get().SkillPerks.CanFarmTinker
      }, new List<string>() { this.Farming1.Id }));
      this.Farming3 = this.AddSkill(new Skill(nameof (Farming3), (string) DUPLICANTS.ROLES.SENIOR_FARMER.NAME, (string) DUPLICANTS.ROLES.SENIOR_FARMER.DESCRIPTION, "", 2, "hat_role_farming3", "skillbadge_role_farming3", Db.Get().SkillGroups.Farming.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseBotanyLarge
      }, new List<string>() { this.Farming2.Id }));
      if (DlcManager.FeaturePlantMutationsEnabled())
        this.Farming3.perks.Add(Db.Get().SkillPerks.CanIdentifyMutantSeeds);
      this.Ranching1 = this.AddSkill(new Skill(nameof (Ranching1), (string) DUPLICANTS.ROLES.RANCHER.NAME, (string) DUPLICANTS.ROLES.RANCHER.DESCRIPTION, "", 1, "hat_role_rancher1", "skillbadge_role_rancher1", Db.Get().SkillGroups.Ranching.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.CanWrangleCreatures,
        Db.Get().SkillPerks.CanUseRanchStation,
        Db.Get().SkillPerks.IncreaseRanchingSmall
      }, new List<string>() { this.Farming1.Id }));
      this.Ranching2 = this.AddSkill(new Skill(nameof (Ranching2), (string) DUPLICANTS.ROLES.SENIOR_RANCHER.NAME, (string) DUPLICANTS.ROLES.SENIOR_RANCHER.DESCRIPTION, "", 2, "hat_role_rancher2", "skillbadge_role_rancher2", Db.Get().SkillGroups.Ranching.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseRanchingMedium
      }, new List<string>() { this.Ranching1.Id }));
      this.Researching1 = this.AddSkill(new Skill(nameof (Researching1), (string) DUPLICANTS.ROLES.JUNIOR_RESEARCHER.NAME, (string) DUPLICANTS.ROLES.JUNIOR_RESEARCHER.DESCRIPTION, "", 0, "hat_role_research1", "skillbadge_role_research1", Db.Get().SkillGroups.Research.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseLearningSmall,
        Db.Get().SkillPerks.AllowAdvancedResearch
      }));
      this.Researching2 = this.AddSkill(new Skill(nameof (Researching2), (string) DUPLICANTS.ROLES.RESEARCHER.NAME, (string) DUPLICANTS.ROLES.RESEARCHER.DESCRIPTION, "", 1, "hat_role_research2", "skillbadge_role_research2", Db.Get().SkillGroups.Research.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseLearningMedium,
        Db.Get().SkillPerks.CanStudyWorldObjects,
        Db.Get().SkillPerks.AllowGeyserTuning
      }, new List<string>() { this.Researching1.Id }));
      this.AtomicResearch = this.AddSkill(new Skill(nameof (AtomicResearch), (string) DUPLICANTS.ROLES.NUCLEAR_RESEARCHER.NAME, (string) DUPLICANTS.ROLES.NUCLEAR_RESEARCHER.DESCRIPTION, "EXPANSION1_ID", 2, "hat_role_research5", "skillbadge_role_research3", Db.Get().SkillGroups.Research.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseLearningLarge,
        Db.Get().SkillPerks.AllowNuclearResearch
      }, new List<string>() { this.Researching2.Id }));
      this.Researching4 = this.AddSkill(new Skill(nameof (Researching4), (string) DUPLICANTS.ROLES.NUCLEAR_RESEARCHER.NAME, (string) DUPLICANTS.ROLES.NUCLEAR_RESEARCHER.DESCRIPTION, "EXPANSION1_ID", 2, "hat_role_research4", "skillbadge_role_research3", Db.Get().SkillGroups.Research.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseLearningLarge,
        Db.Get().SkillPerks.AllowNuclearResearch
      }, new List<string>() { this.Researching2.Id }));
      this.Researching4.deprecated = true;
      this.Researching3 = this.AddSkill(new Skill(nameof (Researching3), (string) DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME, (string) DUPLICANTS.ROLES.SENIOR_RESEARCHER.DESCRIPTION, "", 2, "hat_role_research3", "skillbadge_role_research3", Db.Get().SkillGroups.Research.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseLearningLarge,
        Db.Get().SkillPerks.AllowInterstellarResearch,
        Db.Get().SkillPerks.CanMissionControl
      }, new List<string>() { this.Researching2.Id }));
      this.Researching3.deprecated = DlcManager.IsExpansion1Active();
      this.Astronomy = this.AddSkill(new Skill(nameof (Astronomy), (string) DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME, (string) DUPLICANTS.ROLES.SENIOR_RESEARCHER.DESCRIPTION, "EXPANSION1_ID", 1, "hat_role_research3", "skillbadge_role_research2", Db.Get().SkillGroups.Research.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.CanUseClusterTelescope,
        Db.Get().SkillPerks.CanMissionControl
      }, new List<string>() { this.Researching1.Id }));
      this.SpaceResearch = this.AddSkill(new Skill(nameof (SpaceResearch), (string) DUPLICANTS.ROLES.SPACE_RESEARCHER.NAME, (string) DUPLICANTS.ROLES.SPACE_RESEARCHER.DESCRIPTION, "EXPANSION1_ID", 2, "hat_role_research4", "skillbadge_role_research3", Db.Get().SkillGroups.Research.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseLearningLargeSpace,
        Db.Get().SkillPerks.AllowOrbitalResearch
      }, new List<string>() { this.Astronomy.Id }));
      if (DlcManager.IsExpansion1Active())
      {
        string name = (string) DUPLICANTS.ROLES.ROCKETPILOT.NAME;
        string description = (string) DUPLICANTS.ROLES.ROCKETPILOT.DESCRIPTION;
        string id = Db.Get().SkillGroups.Rocketry.Id;
        List<SkillPerk> perks = new List<SkillPerk>();
        perks.Add(Db.Get().SkillPerks.CanUseRocketControlStation);
        List<string> priorSkills = new List<string>();
        this.RocketPiloting1 = this.AddSkill(new Skill(nameof (RocketPiloting1), name, description, "EXPANSION1_ID", 0, "hat_role_astronaut1", "skillbadge_role_rocketry1", id, perks, priorSkills));
        this.RocketPiloting2 = this.AddSkill(new Skill(nameof (RocketPiloting2), (string) DUPLICANTS.ROLES.SENIOR_ROCKETPILOT.NAME, (string) DUPLICANTS.ROLES.SENIOR_ROCKETPILOT.DESCRIPTION, "EXPANSION1_ID", 2, "hat_role_astronaut2", "skillbadge_role_rocketry3", Db.Get().SkillGroups.Rocketry.Id, new List<SkillPerk>()
        {
          Db.Get().SkillPerks.IncreaseRocketSpeedSmall
        }, new List<string>()
        {
          this.RocketPiloting1.Id,
          this.Astronomy.Id
        }));
      }
      this.Cooking1 = this.AddSkill(new Skill(nameof (Cooking1), (string) DUPLICANTS.ROLES.JUNIOR_COOK.NAME, (string) DUPLICANTS.ROLES.JUNIOR_COOK.DESCRIPTION, "", 0, "hat_role_cooking1", "skillbadge_role_cooking1", Db.Get().SkillGroups.Cooking.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseCookingSmall,
        Db.Get().SkillPerks.CanElectricGrill
      }));
      this.Cooking2 = this.AddSkill(new Skill(nameof (Cooking2), (string) DUPLICANTS.ROLES.COOK.NAME, (string) DUPLICANTS.ROLES.COOK.DESCRIPTION, "", 1, "hat_role_cooking2", "skillbadge_role_cooking2", Db.Get().SkillGroups.Cooking.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseCookingMedium,
        Db.Get().SkillPerks.CanSpiceGrinder
      }, new List<string>() { this.Cooking1.Id }));
      this.Arting1 = this.AddSkill(new Skill(nameof (Arting1), (string) DUPLICANTS.ROLES.JUNIOR_ARTIST.NAME, (string) DUPLICANTS.ROLES.JUNIOR_ARTIST.DESCRIPTION, "", 0, "hat_role_art1", "skillbadge_role_art1", Db.Get().SkillGroups.Art.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.CanArt,
        Db.Get().SkillPerks.CanArtUgly,
        Db.Get().SkillPerks.IncreaseArtSmall
      }));
      this.Arting2 = this.AddSkill(new Skill(nameof (Arting2), (string) DUPLICANTS.ROLES.ARTIST.NAME, (string) DUPLICANTS.ROLES.ARTIST.DESCRIPTION, "", 1, "hat_role_art2", "skillbadge_role_art2", Db.Get().SkillGroups.Art.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.CanArtOkay,
        Db.Get().SkillPerks.IncreaseArtMedium,
        Db.Get().SkillPerks.CanClothingAlteration
      }, new List<string>() { this.Arting1.Id }));
      if (DlcManager.FeatureClusterSpaceEnabled())
        this.Arting2.perks.Add(Db.Get().SkillPerks.CanStudyArtifact);
      this.Arting3 = this.AddSkill(new Skill(nameof (Arting3), (string) DUPLICANTS.ROLES.MASTER_ARTIST.NAME, (string) DUPLICANTS.ROLES.MASTER_ARTIST.DESCRIPTION, "", 2, "hat_role_art3", "skillbadge_role_art3", Db.Get().SkillGroups.Art.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.CanArtGreat,
        Db.Get().SkillPerks.IncreaseArtLarge
      }, new List<string>() { this.Arting2.Id }));
      this.Hauling1 = this.AddSkill(new Skill(nameof (Hauling1), (string) DUPLICANTS.ROLES.HAULER.NAME, (string) DUPLICANTS.ROLES.HAULER.DESCRIPTION, "", 0, "hat_role_hauling1", "skillbadge_role_hauling1", Db.Get().SkillGroups.Hauling.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseStrengthGofer,
        Db.Get().SkillPerks.IncreaseCarryAmountSmall
      }));
      this.Hauling2 = this.AddSkill(new Skill(nameof (Hauling2), (string) DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME, (string) DUPLICANTS.ROLES.MATERIALS_MANAGER.DESCRIPTION, "", 1, "hat_role_hauling2", "skillbadge_role_hauling2", Db.Get().SkillGroups.Hauling.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseStrengthCourier,
        Db.Get().SkillPerks.IncreaseCarryAmountMedium
      }, new List<string>() { this.Hauling1.Id }));
      if (DlcManager.IsExpansion1Active())
        this.ThermalSuits = this.AddSkill(new Skill(nameof (ThermalSuits), (string) DUPLICANTS.ROLES.SUIT_DURABILITY.NAME, (string) DUPLICANTS.ROLES.SUIT_DURABILITY.DESCRIPTION, "EXPANSION1_ID", 1, "hat_role_suits1", "skillbadge_role_suits2", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk>()
        {
          Db.Get().SkillPerks.IncreaseAthleticsLarge,
          Db.Get().SkillPerks.ExosuitDurability
        }, new List<string>()
        {
          this.Hauling1.Id,
          this.RocketPiloting1.Id
        }));
      else
        this.ThermalSuits = this.AddSkill(new Skill(nameof (ThermalSuits), (string) DUPLICANTS.ROLES.SUIT_DURABILITY.NAME, (string) DUPLICANTS.ROLES.SUIT_DURABILITY.DESCRIPTION, "", 1, "hat_role_suits1", "skillbadge_role_suits2", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk>()
        {
          Db.Get().SkillPerks.IncreaseAthleticsLarge,
          Db.Get().SkillPerks.ExosuitDurability
        }, new List<string>() { this.Hauling1.Id }));
      this.Suits1 = this.AddSkill(new Skill(nameof (Suits1), (string) DUPLICANTS.ROLES.SUIT_EXPERT.NAME, (string) DUPLICANTS.ROLES.SUIT_EXPERT.DESCRIPTION, "", 2, "hat_role_suits2", "skillbadge_role_suits3", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.ExosuitExpertise,
        Db.Get().SkillPerks.IncreaseAthleticsMedium
      }, new List<string>() { this.ThermalSuits.Id }));
      this.Technicals1 = this.AddSkill(new Skill(nameof (Technicals1), (string) DUPLICANTS.ROLES.MACHINE_TECHNICIAN.NAME, (string) DUPLICANTS.ROLES.MACHINE_TECHNICIAN.DESCRIPTION, "", 0, "hat_role_technicals1", "skillbadge_role_technicals1", Db.Get().SkillGroups.Technicals.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseMachinerySmall
      }));
      this.Technicals2 = this.AddSkill(new Skill(nameof (Technicals2), (string) DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME, (string) DUPLICANTS.ROLES.POWER_TECHNICIAN.DESCRIPTION, "", 1, "hat_role_technicals2", "skillbadge_role_technicals2", Db.Get().SkillGroups.Technicals.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseMachineryMedium,
        Db.Get().SkillPerks.CanPowerTinker
      }, new List<string>() { this.Technicals1.Id }));
      this.Engineering1 = this.AddSkill(new Skill(nameof (Engineering1), (string) DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME, (string) DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.DESCRIPTION, "", 2, "hat_role_engineering1", "skillbadge_role_engineering1", Db.Get().SkillGroups.Technicals.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseMachineryLarge,
        Db.Get().SkillPerks.IncreaseConstructionMechatronics,
        Db.Get().SkillPerks.ConveyorBuild
      }, new List<string>()
      {
        this.Technicals2.Id,
        this.Hauling2.Id
      }));
      this.Basekeeping1 = this.AddSkill(new Skill(nameof (Basekeeping1), (string) DUPLICANTS.ROLES.HANDYMAN.NAME, (string) DUPLICANTS.ROLES.HANDYMAN.DESCRIPTION, "", 0, "hat_role_basekeeping1", "skillbadge_role_basekeeping1", Db.Get().SkillGroups.Basekeeping.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseStrengthGroundskeeper
      }));
      this.Basekeeping2 = this.AddSkill(new Skill(nameof (Basekeeping2), (string) DUPLICANTS.ROLES.PLUMBER.NAME, (string) DUPLICANTS.ROLES.PLUMBER.DESCRIPTION, "", 1, "hat_role_basekeeping2", "skillbadge_role_basekeeping2", Db.Get().SkillGroups.Basekeeping.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.IncreaseStrengthPlumber,
        Db.Get().SkillPerks.CanDoPlumbing
      }, new List<string>() { this.Basekeeping1.Id }));
      if (DlcManager.IsExpansion1Active())
      {
        this.Astronauting1 = this.AddSkill(new Skill(nameof (Astronauting1), (string) DUPLICANTS.ROLES.USELESSSKILL.NAME, (string) DUPLICANTS.ROLES.USELESSSKILL.DESCRIPTION, "EXPANSION1_ID", 3, "hat_role_astronaut1", "skillbadge_role_astronaut1", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk>()
        {
          Db.Get().SkillPerks.IncreaseAthleticsMedium
        }, new List<string>()
        {
          this.Researching3.Id,
          this.Suits1.Id
        }));
        this.Astronauting1.deprecated = true;
        this.Astronauting2 = this.AddSkill(new Skill(nameof (Astronauting2), (string) DUPLICANTS.ROLES.USELESSSKILL.NAME, (string) DUPLICANTS.ROLES.USELESSSKILL.DESCRIPTION, "EXPANSION1_ID", 4, "hat_role_astronaut2", "skillbadge_role_astronaut2", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk>()
        {
          Db.Get().SkillPerks.IncreaseAthleticsMedium
        }, new List<string>() { this.Astronauting1.Id }));
        this.Astronauting2.deprecated = true;
      }
      else
      {
        this.Astronauting1 = this.AddSkill(new Skill(nameof (Astronauting1), (string) DUPLICANTS.ROLES.ASTRONAUTTRAINEE.NAME, (string) DUPLICANTS.ROLES.ASTRONAUTTRAINEE.DESCRIPTION, "", 3, "hat_role_astronaut1", "skillbadge_role_astronaut1", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk>()
        {
          Db.Get().SkillPerks.CanUseRockets
        }, new List<string>()
        {
          this.Researching3.Id,
          this.Suits1.Id
        }));
        this.Astronauting2 = this.AddSkill(new Skill(nameof (Astronauting2), (string) DUPLICANTS.ROLES.ASTRONAUT.NAME, (string) DUPLICANTS.ROLES.ASTRONAUT.DESCRIPTION, "", 4, "hat_role_astronaut2", "skillbadge_role_astronaut2", Db.Get().SkillGroups.Suits.Id, new List<SkillPerk>()
        {
          Db.Get().SkillPerks.FasterSpaceFlight
        }, new List<string>() { this.Astronauting1.Id }));
      }
      this.Medicine1 = this.AddSkill(new Skill(nameof (Medicine1), (string) DUPLICANTS.ROLES.JUNIOR_MEDIC.NAME, (string) DUPLICANTS.ROLES.JUNIOR_MEDIC.DESCRIPTION, "", 0, "hat_role_medicalaid1", "skillbadge_role_medicalaid1", Db.Get().SkillGroups.MedicalAid.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.CanCompound,
        Db.Get().SkillPerks.IncreaseCaringSmall
      }));
      this.Medicine2 = this.AddSkill(new Skill(nameof (Medicine2), (string) DUPLICANTS.ROLES.MEDIC.NAME, (string) DUPLICANTS.ROLES.MEDIC.DESCRIPTION, "", 1, "hat_role_medicalaid2", "skillbadge_role_medicalaid2", Db.Get().SkillGroups.MedicalAid.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.CanDoctor,
        Db.Get().SkillPerks.IncreaseCaringMedium
      }, new List<string>() { this.Medicine1.Id }));
      this.Medicine3 = this.AddSkill(new Skill(nameof (Medicine3), (string) DUPLICANTS.ROLES.SENIOR_MEDIC.NAME, (string) DUPLICANTS.ROLES.SENIOR_MEDIC.DESCRIPTION, "", 2, "hat_role_medicalaid3", "skillbadge_role_medicalaid3", Db.Get().SkillGroups.MedicalAid.Id, new List<SkillPerk>()
      {
        Db.Get().SkillPerks.CanAdvancedMedicine,
        Db.Get().SkillPerks.IncreaseCaringLarge
      }, new List<string>() { this.Medicine2.Id }));
    }

    private Skill AddSkill(Skill skill) => DlcManager.IsContentActive(skill.dlcId) ? this.Add(skill) : skill;

    public List<Skill> GetSkillsWithPerk(string perk)
    {
      List<Skill> skillsWithPerk = new List<Skill>();
      foreach (Skill resource in this.resources)
      {
        if (resource.GivesPerk(HashedString.op_Implicit(perk)))
          skillsWithPerk.Add(resource);
      }
      return skillsWithPerk;
    }

    public List<Skill> GetSkillsWithPerk(SkillPerk perk)
    {
      List<Skill> skillsWithPerk = new List<Skill>();
      foreach (Skill resource in this.resources)
      {
        if (resource.GivesPerk(perk))
          skillsWithPerk.Add(resource);
      }
      return skillsWithPerk;
    }

    public List<Skill> GetAllPriorSkills(Skill skill)
    {
      List<Skill> allPriorSkills = new List<Skill>();
      foreach (string priorSkill in skill.priorSkills)
      {
        Skill skill1 = this.Get(priorSkill);
        allPriorSkills.Add(skill1);
        allPriorSkills.AddRange((IEnumerable<Skill>) this.GetAllPriorSkills(skill1));
      }
      return allPriorSkills;
    }

    public List<Skill> GetTerminalSkills()
    {
      List<Skill> terminalSkills = new List<Skill>();
      foreach (Skill resource1 in this.resources)
      {
        bool flag = true;
        foreach (Skill resource2 in this.resources)
        {
          if (resource2.priorSkills.Contains(resource1.Id))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          terminalSkills.Add(resource1);
      }
      return terminalSkills;
    }
  }
}
