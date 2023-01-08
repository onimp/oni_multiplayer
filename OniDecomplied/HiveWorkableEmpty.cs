// Decompiled with JetBrains decompiler
// Type: HiveWorkableEmpty
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/HiveWorkableEmpty")]
public class HiveWorkableEmpty : Workable
{
  private static readonly HashedString[] WORK_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("working_pre"),
    HashedString.op_Implicit("working_loop")
  };
  private static readonly HashedString PST_ANIM = new HashedString("working_pst");
  public bool wasStung;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Emptying;
    this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    this.workAnims = HiveWorkableEmpty.WORK_ANIMS;
    this.workingPstComplete = new HashedString[1]
    {
      HiveWorkableEmpty.PST_ANIM
    };
    this.workingPstFailed = new HashedString[1]
    {
      HiveWorkableEmpty.PST_ANIM
    };
  }

  protected override void OnCompleteWork(Worker worker)
  {
    base.OnCompleteWork(worker);
    if (this.wasStung)
      return;
    ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().harvestAHiveWithoutGettingStung = true;
  }
}
