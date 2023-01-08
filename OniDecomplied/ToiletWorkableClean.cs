// Decompiled with JetBrains decompiler
// Type: ToiletWorkableClean
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ToiletWorkableClean")]
public class ToiletWorkableClean : Workable
{
  [Serialize]
  public int timesCleaned;
  private static readonly HashedString[] CLEAN_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("unclog_pre"),
    HashedString.op_Implicit("unclog_loop")
  };
  private static readonly HashedString PST_ANIM = new HashedString("unclog_pst");

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
    this.workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
    this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    this.workAnims = ToiletWorkableClean.CLEAN_ANIMS;
    this.workingPstComplete = new HashedString[1]
    {
      ToiletWorkableClean.PST_ANIM
    };
    this.workingPstFailed = new HashedString[1]
    {
      ToiletWorkableClean.PST_ANIM
    };
  }

  protected override void OnCompleteWork(Worker worker)
  {
    ++this.timesCleaned;
    base.OnCompleteWork(worker);
  }
}
