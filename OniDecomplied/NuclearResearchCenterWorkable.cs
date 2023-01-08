// Decompiled with JetBrains decompiler
// Type: NuclearResearchCenterWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using TUNING;
using UnityEngine;

public class NuclearResearchCenterWorkable : Workable
{
  [MyCmpReq]
  private Operational operational;
  [Serialize]
  private float pointsProduced;
  private NuclearResearchCenter nrc;
  private HighEnergyParticleStorage radiationStorage;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Researching;
    this.attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
    this.skillExperienceMultiplier = SKILLS.ALL_DAY_EXPERIENCE;
    this.radiationStorage = ((Component) this).GetComponent<HighEnergyParticleStorage>();
    this.nrc = ((Component) this).GetComponent<NuclearResearchCenter>();
    this.lightEfficiencyBonus = true;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.SetWorkTime(float.PositiveInfinity);
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    float num1 = dt / this.nrc.timePerPoint;
    if (Game.Instance.FastWorkersModeActive)
      num1 *= 2f;
    double num2 = (double) this.radiationStorage.ConsumeAndGet(num1 * this.nrc.materialPerPoint);
    this.pointsProduced += num1;
    if ((double) this.pointsProduced >= 1.0)
    {
      int points = Mathf.FloorToInt(this.pointsProduced);
      this.pointsProduced -= (float) points;
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Research, Research.Instance.GetResearchType("nuclear").name, this.transform);
      Research.Instance.AddResearchPoints("nuclear", (float) points);
    }
    TechInstance activeResearch = Research.Instance.GetActiveResearch();
    return this.radiationStorage.IsEmpty() || activeResearch == null || (double) activeResearch.PercentageCompleteResearchType("nuclear") >= 1.0;
  }

  protected override void OnAbortWork(Worker worker) => base.OnAbortWork(worker);

  protected override void OnStopWork(Worker worker) => base.OnStopWork(worker);

  public override float GetPercentComplete()
  {
    if (Research.Instance.GetActiveResearch() == null)
      return 0.0f;
    float num1 = Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID["nuclear"];
    float num2 = 0.0f;
    return !Research.Instance.GetActiveResearch().tech.costsByResearchTypeID.TryGetValue("nuclear", out num2) ? 1f : num1 / num2;
  }

  public override bool InstantlyFinish(Worker worker) => false;
}
