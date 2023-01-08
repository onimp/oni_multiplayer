// Decompiled with JetBrains decompiler
// Type: MissionControlClusterWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TUNING;
using UnityEngine;

public class MissionControlClusterWorkable : Workable
{
  private Clustercraft targetClustercraft;
  [MyCmpReq]
  private Operational operational;
  private Guid workStatusItem = Guid.Empty;

  public Clustercraft TargetClustercraft
  {
    get => this.targetClustercraft;
    set
    {
      this.WorkTimeRemaining = this.GetWorkTime();
      this.targetClustercraft = value;
    }
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.requiredSkillPerk = Db.Get().SkillPerks.CanMissionControl.Id;
    this.workerStatusItem = Db.Get().DuplicantStatusItems.MissionControlling;
    this.attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_mission_control_station_kanim"))
    };
    this.SetWorkTime(90f);
    this.showProgressBar = true;
    this.lightEfficiencyBonus = true;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    Components.MissionControlClusterWorkables.Add(this);
  }

  protected override void OnCleanUp()
  {
    Components.MissionControlClusterWorkables.Remove(this);
    base.OnCleanUp();
  }

  public static bool IsRocketInRange(AxialI worldLocation, AxialI rocketLocation) => AxialUtil.GetDistance(worldLocation, rocketLocation) <= 2;

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    this.workStatusItem = ((Component) this).gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.MissionControlAssistingRocket, (object) this.TargetClustercraft);
    this.operational.SetActive(true);
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    if (!Object.op_Equality((Object) this.TargetClustercraft, (Object) null) && MissionControlClusterWorkable.IsRocketInRange(((Component) this).gameObject.GetMyWorldLocation(), this.TargetClustercraft.Location))
      return base.OnWorkTick(worker, dt);
    worker.StopWork();
    return true;
  }

  protected override void OnCompleteWork(Worker worker)
  {
    Debug.Assert(Object.op_Inequality((Object) this.TargetClustercraft, (Object) null));
    ((Component) this).gameObject.GetSMI<MissionControlCluster.Instance>().ApplyEffect(this.TargetClustercraft);
    base.OnCompleteWork(worker);
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    ((Component) this).gameObject.GetComponent<KSelectable>().RemoveStatusItem(this.workStatusItem);
    this.TargetClustercraft = (Clustercraft) null;
    this.operational.SetActive(false);
  }
}
