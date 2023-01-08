// Decompiled with JetBrains decompiler
// Type: Artable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Artable")]
public class Artable : Workable
{
  [Serialize]
  private string currentStage;
  [Serialize]
  private string userChosenTargetStage;
  private string defaultArtworkId = "Default";
  public string defaultAnimName;
  private WorkChore<Artable> chore;

  public string CurrentStage => this.currentStage;

  protected Artable() => this.faceTargetWhenWorking = true;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Arting;
    this.attributeConverter = Db.Get().AttributeConverters.ArtSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Art.Id;
    this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
    this.requiredSkillPerk = Db.Get().SkillPerks.CanArt.Id;
    this.SetWorkTime(80f);
  }

  protected override void OnSpawn()
  {
    ((Component) this).GetComponent<KPrefabID>().PrefabID();
    if (string.IsNullOrEmpty(this.currentStage) || this.currentStage == this.defaultArtworkId)
      this.SetDefault();
    else
      this.SetStage(this.currentStage, true);
    this.shouldShowSkillPerkStatusItem = false;
    base.OnSpawn();
  }

  [System.Runtime.Serialization.OnDeserialized]
  public void OnDeserialized()
  {
    if (Db.GetArtableStages().TryGet(this.currentStage) != null || !(this.currentStage != this.defaultArtworkId))
      return;
    string str = string.Format("{0}_{1}", (object) ((Component) this).GetComponent<KPrefabID>().PrefabID().ToString(), (object) this.currentStage);
    if (Db.GetArtableStages().TryGet(str) == null)
      Debug.LogError((object) ("Failed up to update " + this.currentStage + " to ArtableStages"));
    else
      this.currentStage = str;
  }

  protected override void OnCompleteWork(Worker worker)
  {
    if (string.IsNullOrEmpty(this.userChosenTargetStage))
    {
      Db db = Db.Get();
      Tag prefab_id = ((Component) this).GetComponent<KPrefabID>().PrefabID();
      List<ArtableStage> prefabStages = Db.GetArtableStages().GetPrefabStages(prefab_id);
      ArtableStatusItem artist_skill = db.ArtableStatuses.LookingUgly;
      MinionResume component = ((Component) worker).GetComponent<MinionResume>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        if (component.HasPerk(HashedString.op_Implicit(db.SkillPerks.CanArtGreat.Id)))
          artist_skill = db.ArtableStatuses.LookingGreat;
        else if (component.HasPerk(HashedString.op_Implicit(db.SkillPerks.CanArtOkay.Id)))
          artist_skill = db.ArtableStatuses.LookingOkay;
      }
      prefabStages.RemoveAll((Predicate<ArtableStage>) (stage => stage.statusItem.StatusType > artist_skill.StatusType || stage.statusItem.StatusType == ArtableStatuses.ArtableStatusType.AwaitingArting));
      prefabStages.Sort((Comparison<ArtableStage>) ((x, y) => y.statusItem.StatusType.CompareTo((object) x.statusItem.StatusType)));
      ArtableStatuses.ArtableStatusType highest_type = prefabStages[0].statusItem.StatusType;
      prefabStages.RemoveAll((Predicate<ArtableStage>) (stage => stage.statusItem.StatusType < highest_type));
      prefabStages.RemoveAll((Predicate<ArtableStage>) (stage => !stage.IsUnlocked()));
      Util.Shuffle<ArtableStage>((IList<ArtableStage>) prefabStages);
      this.SetStage(prefabStages[0].id, false);
      if (prefabStages[0].cheerOnComplete)
      {
        EmoteChore emoteChore1 = new EmoteChore((IStateMachineTarget) ((Component) worker).GetComponent<ChoreProvider>(), db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Cheer);
      }
      else
      {
        EmoteChore emoteChore2 = new EmoteChore((IStateMachineTarget) ((Component) worker).GetComponent<ChoreProvider>(), db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Disappointed);
      }
    }
    else
    {
      this.SetStage(this.userChosenTargetStage, false);
      this.userChosenTargetStage = (string) null;
    }
    this.shouldShowSkillPerkStatusItem = false;
    this.UpdateStatusItem();
    Prioritizable.RemoveRef(((Component) this).gameObject);
  }

  public void SetDefault()
  {
    this.currentStage = this.defaultArtworkId;
    ((Component) this).GetComponent<KBatchedAnimController>().SwapAnims(((Component) this).GetComponent<Building>().Def.AnimFiles);
    ((Component) this).GetComponent<KAnimControllerBase>().Play(HashedString.op_Implicit(this.defaultAnimName));
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    component.SetName(((Component) this).GetComponent<Building>().Def.Name);
    component.SetStatusItem(Db.Get().StatusItemCategories.Main, (StatusItem) Db.Get().ArtableStatuses.AwaitingArting, (object) this);
    this.shouldShowSkillPerkStatusItem = false;
    this.UpdateStatusItem();
    if (!(this.currentStage == this.defaultArtworkId))
      return;
    this.shouldShowSkillPerkStatusItem = true;
    Prioritizable.AddRef(((Component) this).gameObject);
    this.chore = new WorkChore<Artable>(Db.Get().ChoreTypes.Art, (IStateMachineTarget) this);
    this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, (object) this.requiredSkillPerk);
  }

  public virtual void SetStage(string stage_id, bool skip_effect)
  {
    ArtableStage artableStage = Db.GetArtableStages().Get(stage_id);
    if (artableStage == null)
    {
      Debug.LogError((object) ("Missing stage: " + stage_id));
    }
    else
    {
      this.currentStage = artableStage.id;
      ((Component) this).GetComponent<KBatchedAnimController>().SwapAnims(new KAnimFile[1]
      {
        Assets.GetAnim(HashedString.op_Implicit(artableStage.animFile))
      });
      ((Component) this).GetComponent<KAnimControllerBase>().Play(HashedString.op_Implicit(artableStage.anim));
      if (artableStage.decor != 0)
      {
        AttributeModifier modifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, (float) artableStage.decor, "Art Quality");
        this.GetAttributes().Add(modifier);
      }
      KSelectable component = ((Component) this).GetComponent<KSelectable>();
      component.SetName(artableStage.Name);
      component.SetStatusItem(Db.Get().StatusItemCategories.Main, (StatusItem) artableStage.statusItem, (object) this);
      this.shouldShowSkillPerkStatusItem = false;
      this.UpdateStatusItem();
    }
  }

  public void SetUserChosenTargetState(string stageID)
  {
    this.SetDefault();
    this.userChosenTargetStage = stageID;
  }
}
