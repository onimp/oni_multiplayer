// Decompiled with JetBrains decompiler
// Type: MinionResume
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TUNING;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/MinionResume")]
public class MinionResume : KMonoBehaviour, ISaveLoadable, ISim200ms
{
  [MyCmpReq]
  private MinionIdentity identity;
  [Serialize]
  public Dictionary<string, bool> MasteryByRoleID = new Dictionary<string, bool>();
  [Serialize]
  public Dictionary<string, bool> MasteryBySkillID = new Dictionary<string, bool>();
  [Serialize]
  public List<string> GrantedSkillIDs = new List<string>();
  [Serialize]
  public Dictionary<HashedString, float> AptitudeByRoleGroup = new Dictionary<HashedString, float>();
  [Serialize]
  public Dictionary<HashedString, float> AptitudeBySkillGroup = new Dictionary<HashedString, float>();
  [Serialize]
  private string currentRole = "NoRole";
  [Serialize]
  private string targetRole = "NoRole";
  [Serialize]
  private string currentHat;
  [Serialize]
  private string targetHat;
  private Dictionary<string, bool> ownedHats = new Dictionary<string, bool>();
  [Serialize]
  private float totalExperienceGained;
  private Notification lastSkillNotification;
  private AttributeModifier skillsMoraleExpectationModifier;
  private AttributeModifier skillsMoraleModifier;
  public float DEBUG_PassiveExperienceGained;
  public float DEBUG_ActiveExperienceGained;
  public float DEBUG_SecondsAlive;

  public MinionIdentity GetIdentity => this.identity;

  public float TotalExperienceGained => this.totalExperienceGained;

  public int TotalSkillPointsGained => MinionResume.CalculateTotalSkillPointsGained(this.TotalExperienceGained);

  public static int CalculateTotalSkillPointsGained(float experience) => Mathf.FloorToInt(Mathf.Pow((float) ((double) experience / (double) SKILLS.TARGET_SKILLS_CYCLE / 600.0), 1f / SKILLS.EXPERIENCE_LEVEL_POWER) * (float) SKILLS.TARGET_SKILLS_EARNED);

  public int SkillsMastered
  {
    get
    {
      int skillsMastered = 0;
      foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
      {
        if (keyValuePair.Value)
          ++skillsMastered;
      }
      return skillsMastered;
    }
  }

  public int AvailableSkillpoints => this.TotalSkillPointsGained - this.SkillsMastered + (this.GrantedSkillIDs == null ? 0 : this.GrantedSkillIDs.Count);

  [OnDeserialized]
  private void OnDeserializedMethod()
  {
    if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 7))
    {
      foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryByRoleID)
      {
        if (keyValuePair.Value && keyValuePair.Key != "NoRole")
          this.ForceAddSkillPoint();
      }
      foreach (KeyValuePair<HashedString, float> keyValuePair in this.AptitudeByRoleGroup)
        this.AptitudeBySkillGroup[keyValuePair.Key] = keyValuePair.Value;
    }
    if (this.TotalSkillPointsGained <= 1000 && this.TotalSkillPointsGained >= 0)
      return;
    this.ForceSetSkillPoints(100);
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Components.MinionResumes.Add(this);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.GrantedSkillIDs == null)
      this.GrantedSkillIDs = new List<string>();
    List<string> stringList = new List<string>();
    foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
    {
      if (keyValuePair.Value && Db.Get().Skills.Get(keyValuePair.Key).deprecated)
        stringList.Add(keyValuePair.Key);
    }
    foreach (string skillId in stringList)
      this.UnmasterSkill(skillId);
    foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
    {
      if (keyValuePair.Value)
      {
        Skill skill = Db.Get().Skills.Get(keyValuePair.Key);
        foreach (SkillPerk perk in skill.perks)
        {
          if (perk.OnRemove != null)
            perk.OnRemove(this);
          if (perk.OnApply != null)
            perk.OnApply(this);
        }
        if (!this.ownedHats.ContainsKey(skill.hat))
          this.ownedHats.Add(skill.hat, true);
      }
    }
    this.UpdateExpectations();
    this.UpdateMorale();
    MinionResume.ApplyHat(this.currentHat, ((Component) this).GetComponent<KBatchedAnimController>());
    this.ShowNewSkillPointNotification();
  }

  public void RestoreResume(
    Dictionary<string, bool> MasteryBySkillID,
    Dictionary<HashedString, float> AptitudeBySkillGroup,
    List<string> GrantedSkillIDs,
    float totalExperienceGained)
  {
    this.MasteryBySkillID = MasteryBySkillID;
    this.GrantedSkillIDs = GrantedSkillIDs != null ? GrantedSkillIDs : new List<string>();
    this.AptitudeBySkillGroup = AptitudeBySkillGroup;
    this.totalExperienceGained = totalExperienceGained;
  }

  protected virtual void OnCleanUp()
  {
    Components.MinionResumes.Remove(this);
    if (this.lastSkillNotification != null)
    {
      ((Component) Game.Instance).GetComponent<Notifier>().Remove(this.lastSkillNotification);
      this.lastSkillNotification = (Notification) null;
    }
    base.OnCleanUp();
  }

  public bool HasMasteredSkill(string skillId) => this.MasteryBySkillID.ContainsKey(skillId) && this.MasteryBySkillID[skillId];

  public void UpdateUrge()
  {
    if (this.targetHat != this.currentHat)
    {
      if (((Component) this).gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.LearnSkill))
        return;
      ((Component) this).gameObject.GetComponent<ChoreConsumer>().AddUrge(Db.Get().Urges.LearnSkill);
    }
    else
      ((Component) this).gameObject.GetComponent<ChoreConsumer>().RemoveUrge(Db.Get().Urges.LearnSkill);
  }

  public string CurrentRole => this.currentRole;

  public string CurrentHat => this.currentHat;

  public string TargetHat => this.targetHat;

  public void SetHats(string current, string target)
  {
    this.currentHat = current;
    this.targetHat = target;
  }

  public void SetCurrentRole(string role_id) => this.currentRole = role_id;

  public string TargetRole => this.targetRole;

  private void ApplySkillPerks(string skillId)
  {
    foreach (SkillPerk perk in Db.Get().Skills.Get(skillId).perks)
    {
      if (perk.OnApply != null)
        perk.OnApply(this);
    }
  }

  private void RemoveSkillPerks(string skillId)
  {
    foreach (SkillPerk perk in Db.Get().Skills.Get(skillId).perks)
    {
      if (perk.OnRemove != null)
        perk.OnRemove(this);
    }
  }

  public void Sim200ms(float dt)
  {
    this.DEBUG_SecondsAlive += dt;
    if (((Component) this).GetComponent<KPrefabID>().HasTag(GameTags.Dead))
      return;
    this.DEBUG_PassiveExperienceGained += dt * SKILLS.PASSIVE_EXPERIENCE_PORTION;
    this.AddExperience(dt * SKILLS.PASSIVE_EXPERIENCE_PORTION);
  }

  public bool IsAbleToLearnSkill(string skillId)
  {
    Skill skill = Db.Get().Skills.Get(skillId);
    string choreGroupId = Db.Get().SkillGroups.Get(skill.skillGroup).choreGroupID;
    if (!string.IsNullOrEmpty(choreGroupId))
    {
      Traits component = ((Component) this).GetComponent<Traits>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.IsChoreGroupDisabled(HashedString.op_Implicit(choreGroupId)))
        return false;
    }
    return true;
  }

  public bool BelowMoraleExpectation(Skill skill)
  {
    float totalValue1 = Db.Get().Attributes.QualityOfLife.Lookup((Component) this).GetTotalValue();
    double totalValue2 = (double) Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component) this).GetTotalValue();
    int moraleExpectation = skill.GetMoraleExpectation();
    if (this.AptitudeBySkillGroup.ContainsKey(HashedString.op_Implicit(skill.skillGroup)) && (double) this.AptitudeBySkillGroup[HashedString.op_Implicit(skill.skillGroup)] > 0.0)
      ++totalValue1;
    double num = (double) moraleExpectation;
    return totalValue2 + num <= (double) totalValue1;
  }

  public bool HasMasteredDirectlyRequiredSkillsForSkill(Skill skill)
  {
    for (int index = 0; index < skill.priorSkills.Count; ++index)
    {
      if (!this.HasMasteredSkill(skill.priorSkills[index]))
        return false;
    }
    return true;
  }

  public bool HasSkillPointsRequiredForSkill(Skill skill) => this.AvailableSkillpoints >= 1;

  public bool HasSkillAptitude(Skill skill) => this.AptitudeBySkillGroup.ContainsKey(HashedString.op_Implicit(skill.skillGroup)) && (double) this.AptitudeBySkillGroup[HashedString.op_Implicit(skill.skillGroup)] > 0.0;

  public bool HasBeenGrantedSkill(Skill skill) => this.GrantedSkillIDs != null && this.GrantedSkillIDs.Contains(skill.Id);

  public bool HasBeenGrantedSkill(string id) => this.GrantedSkillIDs != null && this.GrantedSkillIDs.Contains(id);

  public MinionResume.SkillMasteryConditions[] GetSkillMasteryConditions(string skillId)
  {
    List<MinionResume.SkillMasteryConditions> masteryConditionsList = new List<MinionResume.SkillMasteryConditions>();
    Skill skill = Db.Get().Skills.Get(skillId);
    if (this.HasSkillAptitude(skill))
      masteryConditionsList.Add(MinionResume.SkillMasteryConditions.SkillAptitude);
    if (!this.BelowMoraleExpectation(skill))
      masteryConditionsList.Add(MinionResume.SkillMasteryConditions.StressWarning);
    if (!this.IsAbleToLearnSkill(skillId))
      masteryConditionsList.Add(MinionResume.SkillMasteryConditions.UnableToLearn);
    if (!this.HasSkillPointsRequiredForSkill(skill))
      masteryConditionsList.Add(MinionResume.SkillMasteryConditions.NeedsSkillPoints);
    if (!this.HasMasteredDirectlyRequiredSkillsForSkill(skill))
      masteryConditionsList.Add(MinionResume.SkillMasteryConditions.MissingPreviousSkill);
    return masteryConditionsList.ToArray();
  }

  public bool CanMasterSkill(
    MinionResume.SkillMasteryConditions[] masteryConditions)
  {
    return !Array.Exists<MinionResume.SkillMasteryConditions>(masteryConditions, (Predicate<MinionResume.SkillMasteryConditions>) (element => element == MinionResume.SkillMasteryConditions.UnableToLearn || element == MinionResume.SkillMasteryConditions.NeedsSkillPoints || element == MinionResume.SkillMasteryConditions.MissingPreviousSkill));
  }

  public bool OwnsHat(string hatId) => this.ownedHats.ContainsKey(hatId) && this.ownedHats[hatId];

  public void SkillLearned()
  {
    if (((Component) this).gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.LearnSkill))
      ((Component) this).gameObject.GetComponent<ChoreConsumer>().RemoveUrge(Db.Get().Urges.LearnSkill);
    foreach (string key in this.ownedHats.Keys.ToList<string>())
      this.ownedHats[key] = true;
    if (this.targetHat == null || !(this.currentHat != this.targetHat))
      return;
    PutOnHatChore putOnHatChore = new PutOnHatChore((IStateMachineTarget) this, Db.Get().ChoreTypes.SwitchHat);
  }

  public void MasterSkill(string skillId)
  {
    if (!((Component) this).gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.LearnSkill))
      ((Component) this).gameObject.GetComponent<ChoreConsumer>().AddUrge(Db.Get().Urges.LearnSkill);
    this.MasteryBySkillID[skillId] = true;
    this.ApplySkillPerks(skillId);
    this.UpdateExpectations();
    this.UpdateMorale();
    this.TriggerMasterSkillEvents();
    GameScheduler.Instance.Schedule("Morale Tutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Morale)), (object) null, (SchedulerGroup) null);
    if (!this.ownedHats.ContainsKey(Db.Get().Skills.Get(skillId).hat))
      this.ownedHats.Add(Db.Get().Skills.Get(skillId).hat, false);
    if (this.AvailableSkillpoints != 0 || this.lastSkillNotification == null)
      return;
    ((Component) Game.Instance).GetComponent<Notifier>().Remove(this.lastSkillNotification);
    this.lastSkillNotification = (Notification) null;
  }

  public void UnmasterSkill(string skillId)
  {
    if (!this.MasteryBySkillID.ContainsKey(skillId))
      return;
    this.MasteryBySkillID.Remove(skillId);
    this.RemoveSkillPerks(skillId);
    this.UpdateExpectations();
    this.UpdateMorale();
    this.TriggerMasterSkillEvents();
  }

  public void GrantSkill(string skillId)
  {
    if (this.GrantedSkillIDs == null)
      this.GrantedSkillIDs = new List<string>();
    if (this.HasBeenGrantedSkill(skillId))
      return;
    this.MasteryBySkillID[skillId] = true;
    this.ApplySkillPerks(skillId);
    this.GrantedSkillIDs.Add(skillId);
    this.UpdateExpectations();
    this.UpdateMorale();
    this.TriggerMasterSkillEvents();
    if (this.ownedHats.ContainsKey(Db.Get().Skills.Get(skillId).hat))
      return;
    this.ownedHats.Add(Db.Get().Skills.Get(skillId).hat, false);
  }

  private void TriggerMasterSkillEvents()
  {
    this.Trigger(540773776, (object) null);
    Game.Instance.Trigger(-1523247426, (object) this);
  }

  public void ForceSetSkillPoints(int points) => this.totalExperienceGained = MinionResume.CalculatePreviousExperienceBar(points);

  public void ForceAddSkillPoint() => this.AddExperience(MinionResume.CalculateNextExperienceBar(this.TotalSkillPointsGained) - this.totalExperienceGained);

  public static float CalculateNextExperienceBar(int current_skill_points) => (float) ((double) Mathf.Pow((float) (current_skill_points + 1) / (float) SKILLS.TARGET_SKILLS_EARNED, SKILLS.EXPERIENCE_LEVEL_POWER) * (double) SKILLS.TARGET_SKILLS_CYCLE * 600.0);

  public static float CalculatePreviousExperienceBar(int current_skill_points) => (float) ((double) Mathf.Pow((float) current_skill_points / (float) SKILLS.TARGET_SKILLS_EARNED, SKILLS.EXPERIENCE_LEVEL_POWER) * (double) SKILLS.TARGET_SKILLS_CYCLE * 600.0);

  private void UpdateExpectations()
  {
    int num = 0;
    foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
    {
      if (keyValuePair.Value && !this.HasBeenGrantedSkill(keyValuePair.Key))
      {
        Skill skill = Db.Get().Skills.Get(keyValuePair.Key);
        num += skill.tier + 1;
      }
    }
    AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component) this);
    if (this.skillsMoraleExpectationModifier != null)
    {
      attributeInstance.Remove(this.skillsMoraleExpectationModifier);
      this.skillsMoraleExpectationModifier = (AttributeModifier) null;
    }
    if (num <= 0)
      return;
    this.skillsMoraleExpectationModifier = new AttributeModifier(attributeInstance.Id, (float) num, (string) DUPLICANTS.NEEDS.QUALITYOFLIFE.EXPECTATION_MOD_NAME);
    attributeInstance.Add(this.skillsMoraleExpectationModifier);
  }

  private void UpdateMorale()
  {
    int num1 = 0;
    foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
    {
      if (keyValuePair.Value && !this.HasBeenGrantedSkill(keyValuePair.Key))
      {
        Skill skill = Db.Get().Skills.Get(keyValuePair.Key);
        float num2 = 0.0f;
        if (this.AptitudeBySkillGroup.TryGetValue(new HashedString(skill.skillGroup), out num2))
          num1 += (int) num2;
      }
    }
    AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup((Component) this);
    if (this.skillsMoraleModifier != null)
    {
      attributeInstance.Remove(this.skillsMoraleModifier);
      this.skillsMoraleModifier = (AttributeModifier) null;
    }
    if (num1 <= 0)
      return;
    this.skillsMoraleModifier = new AttributeModifier(attributeInstance.Id, (float) num1, (string) DUPLICANTS.NEEDS.QUALITYOFLIFE.APTITUDE_SKILLS_MOD_NAME);
    attributeInstance.Add(this.skillsMoraleModifier);
  }

  private void OnSkillPointGained()
  {
    Game.Instance.Trigger(1505456302, (object) this);
    this.ShowNewSkillPointNotification();
    if (Object.op_Inequality((Object) PopFXManager.Instance, (Object) null))
    {
      string text = MISC.NOTIFICATIONS.SKILL_POINT_EARNED.NAME.Replace("{Duplicant}", this.identity.GetProperName());
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, text, this.transform, new Vector3(0.0f, 0.5f, 0.0f));
    }
    new UpgradeFX.Instance((IStateMachineTarget) ((Component) this).gameObject.GetComponent<KMonoBehaviour>(), new Vector3(0.0f, 0.0f, -0.1f)).StartSM();
  }

  private void ShowNewSkillPointNotification()
  {
    if (this.AvailableSkillpoints != 1)
      return;
    this.lastSkillNotification = (Notification) new ManagementMenuNotification((Action) 116, NotificationValence.Good, ((Object) ((Component) this.identity.GetSoleOwner()).gameObject).GetInstanceID().ToString(), MISC.NOTIFICATIONS.SKILL_POINT_EARNED.NAME.Replace("{Duplicant}", this.identity.GetProperName()), NotificationType.Good, new Func<List<Notification>, object, string>(this.GetSkillPointGainedTooltip), (object) this.identity, custom_click_callback: ((Notification.ClickCallback) (d => ManagementMenu.Instance.OpenSkills(this.identity))));
    ((Component) this).GetComponent<Notifier>().Add(this.lastSkillNotification);
  }

  private string GetSkillPointGainedTooltip(List<Notification> notifications, object data) => MISC.NOTIFICATIONS.SKILL_POINT_EARNED.TOOLTIP.Replace("{Duplicant}", ((MinionIdentity) data).GetProperName());

  public void SetAptitude(HashedString skillGroupID, float amount) => this.AptitudeBySkillGroup[skillGroupID] = amount;

  public float GetAptitudeExperienceMultiplier(
    HashedString skillGroupId,
    float buildingFrequencyMultiplier)
  {
    float num = 0.0f;
    this.AptitudeBySkillGroup.TryGetValue(skillGroupId, out num);
    return (float) (1.0 + (double) num * (double) SKILLS.APTITUDE_EXPERIENCE_MULTIPLIER * (double) buildingFrequencyMultiplier);
  }

  public void AddExperience(float amount)
  {
    float experienceGained = this.totalExperienceGained;
    float nextExperienceBar = MinionResume.CalculateNextExperienceBar(this.TotalSkillPointsGained);
    this.totalExperienceGained += amount;
    if (!this.isSpawned || (double) this.totalExperienceGained < (double) nextExperienceBar || (double) experienceGained >= (double) nextExperienceBar)
      return;
    this.OnSkillPointGained();
  }

  public void AddExperienceWithAptitude(
    string skillGroupId,
    float amount,
    float buildingMultiplier)
  {
    float amount1 = amount * this.GetAptitudeExperienceMultiplier(HashedString.op_Implicit(skillGroupId), buildingMultiplier) * SKILLS.ACTIVE_EXPERIENCE_PORTION;
    this.DEBUG_ActiveExperienceGained += amount1;
    this.AddExperience(amount1);
  }

  public bool HasPerk(HashedString perkId)
  {
    foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
    {
      if (keyValuePair.Value && Db.Get().Skills.Get(keyValuePair.Key).GivesPerk(perkId))
        return true;
    }
    return false;
  }

  public bool HasPerk(SkillPerk perk)
  {
    foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
    {
      if (keyValuePair.Value && Db.Get().Skills.Get(keyValuePair.Key).GivesPerk(perk))
        return true;
    }
    return false;
  }

  public void RemoveHat() => MinionResume.RemoveHat(((Component) this).GetComponent<KBatchedAnimController>());

  public static void RemoveHat(KBatchedAnimController controller)
  {
    AccessorySlot hat = Db.Get().AccessorySlots.Hat;
    Accessorizer component = ((Component) controller).GetComponent<Accessorizer>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      Accessory accessory = component.GetAccessory(hat);
      if (accessory != null)
        component.RemoveAccessory(accessory);
    }
    else
      ((Component) controller).GetComponent<SymbolOverrideController>().TryRemoveSymbolOverride(HashedString.op_Implicit(hat.targetSymbolId), 4);
    controller.SetSymbolVisiblity(hat.targetSymbolId, false);
    controller.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, false);
    controller.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, true);
  }

  public static void AddHat(string hat_id, KBatchedAnimController controller)
  {
    AccessorySlot hat = Db.Get().AccessorySlots.Hat;
    Accessory accessory1 = hat.Lookup(hat_id);
    if (accessory1 == null)
      Debug.LogWarning((object) ("Missing hat: " + hat_id));
    Accessorizer component1 = ((Component) controller).GetComponent<Accessorizer>();
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      Accessory accessory2 = component1.GetAccessory(Db.Get().AccessorySlots.Hat);
      if (accessory2 != null)
        component1.RemoveAccessory(accessory2);
      if (accessory1 != null)
        component1.AddAccessory(accessory1);
    }
    else
    {
      SymbolOverrideController component2 = ((Component) controller).GetComponent<SymbolOverrideController>();
      component2.TryRemoveSymbolOverride(HashedString.op_Implicit(hat.targetSymbolId), 4);
      component2.AddSymbolOverride(HashedString.op_Implicit(hat.targetSymbolId), accessory1.symbol, 4);
    }
    controller.SetSymbolVisiblity(hat.targetSymbolId, true);
    controller.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, true);
    controller.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, false);
  }

  public void ApplyTargetHat()
  {
    MinionResume.ApplyHat(this.targetHat, ((Component) this).GetComponent<KBatchedAnimController>());
    this.currentHat = this.targetHat;
    this.targetHat = (string) null;
  }

  public static void ApplyHat(string hat_id, KBatchedAnimController controller)
  {
    if (Util.IsNullOrWhiteSpace(hat_id))
      MinionResume.RemoveHat(controller);
    else
      MinionResume.AddHat(hat_id, controller);
  }

  public string GetSkillsSubtitle() => string.Format((string) DUPLICANTS.NEEDS.QUALITYOFLIFE.TOTAL_SKILL_POINTS, (object) this.TotalSkillPointsGained);

  public static bool AnyMinionHasPerk(string perk, int worldId = -1)
  {
    foreach (MinionResume minionResume in worldId >= 0 ? Components.MinionResumes.GetWorldItems(worldId, true) : Components.MinionResumes.Items)
    {
      if (minionResume.HasPerk(HashedString.op_Implicit(perk)))
        return true;
    }
    return false;
  }

  public static bool AnyOtherMinionHasPerk(string perk, MinionResume me)
  {
    foreach (MinionResume minionResume in Components.MinionResumes.Items)
    {
      if (!Object.op_Equality((Object) minionResume, (Object) me) && minionResume.HasPerk(HashedString.op_Implicit(perk)))
        return true;
    }
    return false;
  }

  public void ResetSkillLevels(bool returnSkillPoints = true)
  {
    List<string> stringList = new List<string>();
    foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
    {
      if (keyValuePair.Value)
        stringList.Add(keyValuePair.Key);
    }
    foreach (string skillId in stringList)
      this.UnmasterSkill(skillId);
  }

  public enum SkillMasteryConditions
  {
    SkillAptitude,
    StressWarning,
    UnableToLearn,
    NeedsSkillPoints,
    MissingPreviousSkill,
  }
}
