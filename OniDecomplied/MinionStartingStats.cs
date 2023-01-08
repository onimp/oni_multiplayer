// Decompiled with JetBrains decompiler
// Type: MinionStartingStats
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei.AI;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MinionStartingStats : ITelepadDeliverable
{
  public string Name;
  public string NameStringKey;
  public string GenderStringKey;
  public List<Trait> Traits = new List<Trait>();
  public int rarityBalance;
  public Trait stressTrait;
  public Trait joyTrait;
  public Trait congenitaltrait;
  public string stickerType;
  public int voiceIdx;
  public Dictionary<string, int> StartingLevels = new Dictionary<string, int>();
  public Personality personality;
  public List<Accessory> accessories = new List<Accessory>();
  public bool IsValid;
  public Dictionary<SkillGroup, float> skillAptitudes = new Dictionary<SkillGroup, float>();

  public MinionStartingStats(
    Personality personality,
    string guaranteedAptitudeID = null,
    string guaranteedTraitID = null,
    bool isDebugMinion = false)
  {
    this.personality = personality;
    this.GenerateStats(guaranteedAptitudeID, guaranteedTraitID, isDebugMinion);
  }

  public MinionStartingStats(
    bool is_starter_minion,
    string guaranteedAptitudeID = null,
    string guaranteedTraitID = null,
    bool isDebugMinion = false)
  {
    this.personality = Db.Get().Personalities.GetRandom(true, is_starter_minion);
    this.GenerateStats(guaranteedAptitudeID, guaranteedTraitID, isDebugMinion, is_starter_minion);
  }

  private void GenerateStats(
    string guaranteedAptitudeID = null,
    string guaranteedTraitID = null,
    bool isDebugMinion = false,
    bool is_starter_minion = false)
  {
    this.voiceIdx = Random.Range(0, 4);
    this.Name = this.personality.Name;
    this.NameStringKey = this.personality.nameStringKey;
    this.GenderStringKey = this.personality.genderStringKey;
    this.Traits.Add(Db.Get().traits.Get(MinionConfig.MINION_BASE_TRAIT_ID));
    List<ChoreGroup> disabled_chore_groups = new List<ChoreGroup>();
    this.GenerateAptitudes(guaranteedAptitudeID);
    this.GenerateAttributes(this.GenerateTraits(is_starter_minion, disabled_chore_groups, guaranteedAptitudeID, guaranteedTraitID, isDebugMinion), disabled_chore_groups);
    KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(this.personality);
    foreach (AccessorySlot resource in Db.Get().AccessorySlots.resources)
    {
      if (resource.accessories.Count != 0)
      {
        Accessory accessory = (Accessory) null;
        if (resource == Db.Get().AccessorySlots.HeadShape)
        {
          accessory = resource.Lookup(bodyData.headShape);
          if (accessory == null)
            this.personality.headShape = 0;
        }
        else if (resource == Db.Get().AccessorySlots.Mouth)
        {
          accessory = resource.Lookup(bodyData.mouth);
          if (accessory == null)
            this.personality.mouth = 0;
        }
        else if (resource == Db.Get().AccessorySlots.Eyes)
        {
          accessory = resource.Lookup(bodyData.eyes);
          if (accessory == null)
            this.personality.eyes = 0;
        }
        else if (resource == Db.Get().AccessorySlots.Hair)
        {
          accessory = resource.Lookup(bodyData.hair);
          if (accessory == null)
            this.personality.hair = 0;
        }
        else if (resource == Db.Get().AccessorySlots.HatHair)
          accessory = resource.accessories[0];
        else if (resource == Db.Get().AccessorySlots.Body)
        {
          accessory = resource.Lookup(bodyData.body);
          if (accessory == null)
            this.personality.body = 0;
        }
        else if (resource == Db.Get().AccessorySlots.Arm)
          accessory = resource.Lookup(bodyData.arms);
        else if (resource == Db.Get().AccessorySlots.ArmLower)
          accessory = resource.Lookup(bodyData.armslower);
        else if (resource == Db.Get().AccessorySlots.ArmLowerSkin)
          accessory = resource.Lookup(bodyData.armLowerSkin);
        else if (resource == Db.Get().AccessorySlots.ArmUpperSkin)
          accessory = resource.Lookup(bodyData.armUpperSkin);
        else if (resource == Db.Get().AccessorySlots.LegSkin)
          accessory = resource.Lookup(bodyData.legSkin);
        else if (resource == Db.Get().AccessorySlots.Leg)
          accessory = resource.Lookup(bodyData.legs);
        else if (resource == Db.Get().AccessorySlots.Belt)
          accessory = resource.Lookup(bodyData.belt) ?? resource.accessories[0];
        else if (resource == Db.Get().AccessorySlots.Neck)
          accessory = resource.Lookup(bodyData.neck);
        else if (resource == Db.Get().AccessorySlots.Pelvis)
          accessory = resource.Lookup(bodyData.pelvis);
        else if (resource == Db.Get().AccessorySlots.Foot)
          accessory = resource.Lookup(bodyData.foot) ?? resource.accessories[0];
        else if (resource == Db.Get().AccessorySlots.Skirt)
          accessory = resource.Lookup(bodyData.skirt);
        else if (resource == Db.Get().AccessorySlots.Necklace)
          accessory = resource.Lookup(bodyData.necklace);
        else if (resource == Db.Get().AccessorySlots.Cuff)
          accessory = resource.Lookup(bodyData.cuff) ?? resource.accessories[0];
        else if (resource == Db.Get().AccessorySlots.Hand)
          accessory = resource.Lookup(bodyData.hand) ?? resource.accessories[0];
        this.accessories.Add(accessory);
      }
    }
  }

  private int GenerateTraits(
    bool is_starter_minion,
    List<ChoreGroup> disabled_chore_groups,
    string guaranteedAptitudeID = null,
    string guaranteedTraitID = null,
    bool isDebugMinion = false)
  {
    int statDelta = 0;
    List<string> selectedTraits = new List<string>();
    KRandom randSeed = new KRandom();
    this.stressTrait = Db.Get().traits.Get(this.personality.stresstrait);
    this.joyTrait = Db.Get().traits.Get(this.personality.joyTrait);
    this.stickerType = this.personality.stickerType;
    Trait trait1 = Db.Get().traits.TryGet(this.personality.congenitaltrait);
    this.congenitaltrait = trait1 == null || trait1.Name == "None" ? (Trait) null : trait1;
    Func<List<DUPLICANTSTATS.TraitVal>, bool, bool> func = (Func<List<DUPLICANTSTATS.TraitVal>, bool, bool>) ((traitPossibilities, positiveTrait) =>
    {
      if (this.Traits.Count > DUPLICANTSTATS.MAX_TRAITS)
        return false;
      double num1 = (double) Mathf.Abs(Util.GaussianRandom(0.0f, 1f));
      int count = traitPossibilities.Count;
      int num2;
      if (!positiveTrait)
      {
        if (DUPLICANTSTATS.rarityDeckActive.Count < 1)
          DUPLICANTSTATS.rarityDeckActive.AddRange((IEnumerable<int>) DUPLICANTSTATS.RARITY_DECK);
        if (DUPLICANTSTATS.rarityDeckActive.Count == DUPLICANTSTATS.RARITY_DECK.Count)
          WorldGenUtil.ShuffleSeeded<int>((IList<int>) DUPLICANTSTATS.rarityDeckActive, randSeed);
        num2 = DUPLICANTSTATS.rarityDeckActive[DUPLICANTSTATS.rarityDeckActive.Count - 1];
        DUPLICANTSTATS.rarityDeckActive.RemoveAt(DUPLICANTSTATS.rarityDeckActive.Count - 1);
      }
      else
      {
        List<int> intList = new List<int>();
        if (is_starter_minion)
        {
          intList.Add(this.rarityBalance - 1);
          intList.Add(this.rarityBalance);
          intList.Add(this.rarityBalance);
          intList.Add(this.rarityBalance + 1);
        }
        else
        {
          intList.Add(this.rarityBalance - 2);
          intList.Add(this.rarityBalance - 1);
          intList.Add(this.rarityBalance);
          intList.Add(this.rarityBalance + 1);
          intList.Add(this.rarityBalance + 2);
        }
        WorldGenUtil.ShuffleSeeded<int>((IList<int>) intList, randSeed);
        int num3 = intList[0];
        int num4 = Mathf.Max(DUPLICANTSTATS.RARITY_COMMON, num3);
        num2 = Mathf.Min(DUPLICANTSTATS.RARITY_LEGENDARY, num4);
      }
      List<DUPLICANTSTATS.TraitVal> traitValList = new List<DUPLICANTSTATS.TraitVal>((IEnumerable<DUPLICANTSTATS.TraitVal>) traitPossibilities);
      for (int index = traitValList.Count - 1; index > -1; --index)
      {
        if (traitValList[index].rarity != num2)
        {
          traitValList.RemoveAt(index);
          --count;
        }
      }
      WorldGenUtil.ShuffleSeeded<DUPLICANTSTATS.TraitVal>((IList<DUPLICANTSTATS.TraitVal>) traitValList, randSeed);
      foreach (DUPLICANTSTATS.TraitVal traitVal in traitValList)
      {
        if (!DlcManager.IsContentActive(traitVal.dlcId))
          --count;
        else if (selectedTraits.Contains(traitVal.id))
        {
          --count;
        }
        else
        {
          Trait trait2 = Db.Get().traits.TryGet(traitVal.id);
          if (trait2 == null)
          {
            Debug.LogWarning((object) ("Trying to add nonexistent trait: " + traitVal.id));
            --count;
          }
          else if (!isDebugMinion || trait2.disabledChoreGroups == null || trait2.disabledChoreGroups.Length == 0)
          {
            if (is_starter_minion && !trait2.ValidStarterTrait)
              --count;
            else if (traitVal.doNotGenerateTrait)
              --count;
            else if (this.AreTraitAndAptitudesExclusive(traitVal, this.skillAptitudes))
              --count;
            else if (is_starter_minion && guaranteedAptitudeID != null && this.AreTraitAndArchetypeExclusive(traitVal, guaranteedAptitudeID))
              --count;
            else if (this.AreTraitsMutuallyExclusive(traitVal, selectedTraits))
            {
              --count;
            }
            else
            {
              selectedTraits.Add(traitVal.id);
              statDelta += traitVal.statBonus;
              this.rarityBalance += positiveTrait ? -traitVal.rarity : traitVal.rarity;
              this.Traits.Add(trait2);
              if (trait2.disabledChoreGroups != null)
              {
                for (int index = 0; index < trait2.disabledChoreGroups.Length; ++index)
                  disabled_chore_groups.Add(trait2.disabledChoreGroups[index]);
              }
              return true;
            }
          }
        }
      }
      return false;
    });
    int num5;
    int num6;
    if (is_starter_minion)
    {
      num5 = 1;
      num6 = 1;
    }
    else
    {
      if (DUPLICANTSTATS.podTraitConfigurationsActive.Count < 1)
        DUPLICANTSTATS.podTraitConfigurationsActive.AddRange((IEnumerable<Tuple<int, int>>) DUPLICANTSTATS.POD_TRAIT_CONFIGURATIONS_DECK);
      if (DUPLICANTSTATS.podTraitConfigurationsActive.Count == DUPLICANTSTATS.POD_TRAIT_CONFIGURATIONS_DECK.Count)
        WorldGenUtil.ShuffleSeeded<Tuple<int, int>>((IList<Tuple<int, int>>) DUPLICANTSTATS.podTraitConfigurationsActive, randSeed);
      num5 = DUPLICANTSTATS.podTraitConfigurationsActive[DUPLICANTSTATS.podTraitConfigurationsActive.Count - 1].first;
      num6 = DUPLICANTSTATS.podTraitConfigurationsActive[DUPLICANTSTATS.podTraitConfigurationsActive.Count - 1].second;
      DUPLICANTSTATS.podTraitConfigurationsActive.RemoveAt(DUPLICANTSTATS.podTraitConfigurationsActive.Count - 1);
    }
    int num7 = 0;
    int num8 = 0;
    int num9 = (num6 + num5) * 4;
    if (!string.IsNullOrEmpty(guaranteedTraitID))
    {
      DUPLICANTSTATS.TraitVal traitVal = DUPLICANTSTATS.GetTraitVal(guaranteedTraitID);
      if (traitVal.id == guaranteedTraitID)
      {
        Trait trait3 = Db.Get().traits.TryGet(traitVal.id);
        bool positiveTrait = trait3.PositiveTrait;
        selectedTraits.Add(traitVal.id);
        statDelta += traitVal.statBonus;
        this.rarityBalance += positiveTrait ? -traitVal.rarity : traitVal.rarity;
        this.Traits.Add(trait3);
        if (trait3.disabledChoreGroups != null)
        {
          for (int index = 0; index < trait3.disabledChoreGroups.Length; ++index)
            disabled_chore_groups.Add(trait3.disabledChoreGroups[index]);
        }
        if (positiveTrait)
          ++num7;
        else
          ++num8;
      }
    }
    for (; num9 > 0 && (num8 < num6 || num7 < num5); --num9)
    {
      if (num8 < num6 && func(DUPLICANTSTATS.BADTRAITS, false))
        ++num8;
      if (num7 < num5 && func(DUPLICANTSTATS.GOODTRAITS, true))
        ++num7;
    }
    if (num9 > 0)
      this.IsValid = true;
    return statDelta;
  }

  private void GenerateAptitudes(string guaranteedAptitudeID = null)
  {
    int num = Random.Range(1, 4);
    List<SkillGroup> skillGroupList = new List<SkillGroup>((IEnumerable<SkillGroup>) Db.Get().SkillGroups.resources);
    Util.Shuffle<SkillGroup>((IList<SkillGroup>) skillGroupList);
    if (guaranteedAptitudeID != null)
    {
      this.skillAptitudes.Add(Db.Get().SkillGroups.Get(guaranteedAptitudeID), (float) DUPLICANTSTATS.APTITUDE_BONUS);
      skillGroupList.Remove(Db.Get().SkillGroups.Get(guaranteedAptitudeID));
      --num;
    }
    for (int index = 0; index < num; ++index)
      this.skillAptitudes.Add(skillGroupList[index], (float) DUPLICANTSTATS.APTITUDE_BONUS);
  }

  private void GenerateAttributes(int pointsDelta, List<ChoreGroup> disabled_chore_groups)
  {
    List<string> stringList = new List<string>((IEnumerable<string>) DUPLICANTSTATS.ALL_ATTRIBUTES);
    for (int index = 0; index < stringList.Count; ++index)
    {
      if (!this.StartingLevels.ContainsKey(stringList[index]))
        this.StartingLevels[stringList[index]] = 0;
    }
    foreach (KeyValuePair<SkillGroup, float> skillAptitude in this.skillAptitudes)
    {
      if (skillAptitude.Key.relevantAttributes.Count > 0)
      {
        for (int index = 0; index < skillAptitude.Key.relevantAttributes.Count; ++index)
        {
          if (!this.StartingLevels.ContainsKey(skillAptitude.Key.relevantAttributes[index].Id))
            Debug.LogError((object) ("Need to add " + skillAptitude.Key.relevantAttributes[index].Id + " to TUNING.DUPLICANTSTATS.ALL_ATTRIBUTES"));
          this.StartingLevels[skillAptitude.Key.relevantAttributes[index].Id] += DUPLICANTSTATS.APTITUDE_ATTRIBUTE_BONUSES[this.skillAptitudes.Count - 1];
        }
      }
    }
    int num1 = pointsDelta;
    List<SkillGroup> skillGroupList = new List<SkillGroup>((IEnumerable<SkillGroup>) this.skillAptitudes.Keys);
    if (pointsDelta > 0)
    {
      for (int index1 = num1; index1 > 0; --index1)
      {
        Util.Shuffle<SkillGroup>((IList<SkillGroup>) skillGroupList);
        for (int index2 = 0; index2 < skillGroupList[0].relevantAttributes.Count; ++index2)
          ++this.StartingLevels[skillGroupList[0].relevantAttributes[index2].Id];
      }
    }
    if (disabled_chore_groups.Count <= 0)
      return;
    int num2 = 0;
    int num3 = 0;
    foreach (KeyValuePair<string, int> startingLevel in this.StartingLevels)
    {
      if (startingLevel.Value > num2)
        num2 = startingLevel.Value;
      if (startingLevel.Key == disabled_chore_groups[0].attribute.Id)
        num3 = startingLevel.Value;
    }
    if (num2 != num3)
      return;
    foreach (string key in stringList)
    {
      if (key != disabled_chore_groups[0].attribute.Id)
      {
        int num4 = 0;
        this.StartingLevels.TryGetValue(key, out num4);
        int num5 = 0;
        if (num4 > 0)
          num5 = 1;
        this.StartingLevels[disabled_chore_groups[0].attribute.Id] = num4 - num5;
        this.StartingLevels[key] = num2 + num5;
        break;
      }
    }
  }

  public void Apply(GameObject go)
  {
    MinionIdentity component = go.GetComponent<MinionIdentity>();
    component.SetName(this.Name);
    component.nameStringKey = this.NameStringKey;
    component.genderStringKey = this.GenderStringKey;
    component.personalityResourceId = this.personality.IdHash;
    this.ApplyTraits(go);
    this.ApplyRace(go);
    this.ApplyAptitudes(go);
    this.ApplyAccessories(go);
    this.ApplyExperience(go);
    this.ApplyOutfit(this.personality, go);
  }

  public void ApplyExperience(GameObject go)
  {
    foreach (KeyValuePair<string, int> startingLevel in this.StartingLevels)
      go.GetComponent<AttributeLevels>().SetLevel(startingLevel.Key, startingLevel.Value);
  }

  public void ApplyAccessories(GameObject go)
  {
    Accessorizer component = go.GetComponent<Accessorizer>();
    component.ApplyMinionPersonality(this.personality);
    component.UpdateHairBasedOnHat();
  }

  public void ApplyOutfit(Personality personality, GameObject go)
  {
    if (!personality.outfitIds.ContainsKey(ClothingOutfitUtility.OutfitType.Clothing))
      return;
    Option<ClothingOutfitTarget> option = ClothingOutfitTarget.TryFromId(personality.outfitIds[ClothingOutfitUtility.OutfitType.Clothing]);
    if (!option.HasValue)
      return;
    go.GetComponent<Accessorizer>().ApplyClothingItems(option.Value.ReadItemValues());
  }

  public void ApplyRace(GameObject go) => go.GetComponent<MinionIdentity>().voiceIdx = this.voiceIdx;

  public static KCompBuilder.BodyData CreateBodyData(Personality p) => new KCompBuilder.BodyData()
  {
    eyes = HashCache.Get().Add(string.Format("eyes_{0:000}", (object) p.eyes)),
    hair = HashCache.Get().Add(string.Format("hair_{0:000}", (object) p.hair)),
    headShape = HashCache.Get().Add(string.Format("headshape_{0:000}", (object) p.headShape)),
    mouth = HashCache.Get().Add(string.Format("mouth_{0:000}", (object) p.mouth)),
    neck = HashCache.Get().Add("neck"),
    arms = HashCache.Get().Add(string.Format("arm_sleeve_{0:000}", (object) p.body)),
    armslower = HashCache.Get().Add(string.Format("arm_lower_sleeve_{0:000}", (object) p.body)),
    body = HashCache.Get().Add(string.Format("torso_{0:000}", (object) p.body)),
    hat = HashedString.Invalid,
    faceFX = HashedString.Invalid,
    legs = HashCache.Get().Add("leg"),
    armLowerSkin = HashCache.Get().Add(string.Format("arm_lower_{0:000}", (object) p.headShape)),
    armUpperSkin = HashCache.Get().Add(string.Format("arm_upper_{0:000}", (object) p.headShape)),
    legSkin = HashCache.Get().Add(string.Format("leg_skin_{0:000}", (object) p.headShape)),
    belt = HashCache.Get().Add("belt"),
    pelvis = HashCache.Get().Add("pelvis"),
    foot = HashCache.Get().Add("foot"),
    hand = HashCache.Get().Add("hand_paint"),
    cuff = HashCache.Get().Add("cuff")
  };

  public void ApplyAptitudes(GameObject go)
  {
    MinionResume component = go.GetComponent<MinionResume>();
    foreach (KeyValuePair<SkillGroup, float> skillAptitude in this.skillAptitudes)
      component.SetAptitude(HashedString.op_Implicit(skillAptitude.Key.Id), skillAptitude.Value);
  }

  public void ApplyTraits(GameObject go)
  {
    Klei.AI.Traits component1 = go.GetComponent<Klei.AI.Traits>();
    component1.Clear();
    foreach (Trait trait in this.Traits)
      component1.Add(trait);
    component1.Add(this.stressTrait);
    if (this.congenitaltrait != null)
      component1.Add(this.congenitaltrait);
    component1.Add(this.joyTrait);
    go.GetComponent<MinionIdentity>().SetStickerType(this.stickerType);
    MinionIdentity component2 = go.GetComponent<MinionIdentity>();
    component2.SetName(this.Name);
    component2.nameStringKey = this.NameStringKey;
    go.GetComponent<MinionIdentity>().SetGender(this.GenderStringKey);
  }

  public GameObject Deliver(Vector3 location)
  {
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(MinionConfig.ID)), (GameObject) null, (string) null);
    gameObject.SetActive(true);
    TransformExtensions.SetLocalPosition(gameObject.transform, location);
    this.Apply(gameObject);
    Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
    EmoteChore emoteChore = new EmoteChore((IStateMachineTarget) gameObject.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, HashedString.op_Implicit("anim_interacts_portal_kanim"), Telepad.PortalBirthAnim);
    return gameObject;
  }

  private bool AreTraitAndAptitudesExclusive(
    DUPLICANTSTATS.TraitVal traitVal,
    Dictionary<SkillGroup, float> aptitudes)
  {
    if (traitVal.mutuallyExclusiveAptitudes == null)
      return false;
    foreach (KeyValuePair<SkillGroup, float> skillAptitude in this.skillAptitudes)
    {
      foreach (HashedString exclusiveAptitude in traitVal.mutuallyExclusiveAptitudes)
      {
        if (HashedString.op_Equality(exclusiveAptitude, skillAptitude.Key.IdHash) && (double) skillAptitude.Value > 0.0)
          return true;
      }
    }
    return false;
  }

  private bool AreTraitAndArchetypeExclusive(
    DUPLICANTSTATS.TraitVal traitVal,
    string guaranteedAptitudeID)
  {
    if (!DUPLICANTSTATS.ARCHETYPE_TRAIT_EXCLUSIONS.ContainsKey(guaranteedAptitudeID))
      Debug.LogError((object) ("Need to add attribute " + guaranteedAptitudeID + " to ARCHETYPE_TRAIT_EXCLUSIONS"));
    foreach (string str in DUPLICANTSTATS.ARCHETYPE_TRAIT_EXCLUSIONS[guaranteedAptitudeID])
    {
      if (str == traitVal.id)
        return true;
    }
    return false;
  }

  private bool AreTraitsMutuallyExclusive(
    DUPLICANTSTATS.TraitVal traitVal,
    List<string> selectedTraits)
  {
    foreach (string selectedTrait in selectedTraits)
    {
      foreach (DUPLICANTSTATS.TraitVal traitVal1 in DUPLICANTSTATS.GOODTRAITS)
      {
        if (selectedTrait == traitVal1.id && traitVal1.mutuallyExclusiveTraits != null && traitVal1.mutuallyExclusiveTraits.Contains(traitVal.id))
          return true;
      }
      foreach (DUPLICANTSTATS.TraitVal traitVal2 in DUPLICANTSTATS.BADTRAITS)
      {
        if (selectedTrait == traitVal2.id && traitVal2.mutuallyExclusiveTraits != null && traitVal2.mutuallyExclusiveTraits.Contains(traitVal.id))
          return true;
      }
      foreach (DUPLICANTSTATS.TraitVal traitVal3 in DUPLICANTSTATS.CONGENITALTRAITS)
      {
        if (selectedTrait == traitVal3.id && traitVal3.mutuallyExclusiveTraits != null && traitVal3.mutuallyExclusiveTraits.Contains(traitVal.id))
          return true;
      }
      foreach (DUPLICANTSTATS.TraitVal traitVal4 in DUPLICANTSTATS.SPECIALTRAITS)
      {
        if (selectedTrait == traitVal4.id && traitVal4.mutuallyExclusiveTraits != null && traitVal4.mutuallyExclusiveTraits.Contains(traitVal.id))
          return true;
      }
      if (traitVal.mutuallyExclusiveTraits != null && traitVal.mutuallyExclusiveTraits.Contains(selectedTrait))
        return true;
    }
    return false;
  }
}
