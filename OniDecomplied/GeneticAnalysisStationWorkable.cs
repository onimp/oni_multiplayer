// Decompiled with JetBrains decompiler
// Type: GeneticAnalysisStationWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class GeneticAnalysisStationWorkable : Workable
{
  [MyCmpAdd]
  public Notifier notifier;
  [MyCmpReq]
  public Storage storage;
  [SerializeField]
  public Vector3 finishedSeedDropOffset;
  private Notification notification;
  public GeneticAnalysisStation.StatesInstance statesInstance;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.requiredSkillPerk = Db.Get().SkillPerks.CanIdentifyMutantSeeds.Id;
    this.workerStatusItem = Db.Get().DuplicantStatusItems.AnalyzingGenes;
    this.attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_genetic_analysisstation_kanim"))
    };
    this.SetWorkTime(150f);
    this.showProgressBar = true;
    this.lightEfficiencyBonus = true;
  }

  protected override void OnCompleteWork(Worker worker)
  {
    base.OnCompleteWork(worker);
    this.IdentifyMutant();
  }

  public void IdentifyMutant()
  {
    GameObject first = this.storage.FindFirst(GameTags.UnidentifiedSeed);
    DebugUtil.DevAssertArgs((Object.op_Inequality((Object) first, (Object) null) ? 1 : 0) != 0, new object[1]
    {
      (object) "AAACCCCKKK!! GeneticAnalysisStation finished studying a seed but we don't have one in storage??"
    });
    if (!Object.op_Inequality((Object) first, (Object) null))
      return;
    Pickupable component1 = first.GetComponent<Pickupable>();
    Pickupable pickupable = (double) component1.PrimaryElement.Units <= 1.0 ? this.storage.Drop(first, true).GetComponent<Pickupable>() : component1.Take(1f);
    TransformExtensions.SetPosition(pickupable.transform, Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), this.finishedSeedDropOffset));
    MutantPlant component2 = ((Component) pickupable).GetComponent<MutantPlant>();
    PlantSubSpeciesCatalog.Instance.IdentifySubSpecies(component2.SubSpeciesID);
    component2.Analyze();
    ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().LogAnalyzedSeed(component2.SpeciesID);
  }
}
