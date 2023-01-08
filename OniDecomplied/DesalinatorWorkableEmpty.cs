// Decompiled with JetBrains decompiler
// Type: DesalinatorWorkableEmpty
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/DesalinatorWorkableEmpty")]
public class DesalinatorWorkableEmpty : Workable
{
  [Serialize]
  public int timesCleaned;
  private static readonly HashedString[] WORK_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("salt_pre"),
    HashedString.op_Implicit("salt_loop")
  };
  private static readonly HashedString PST_ANIM = new HashedString("salt_pst");

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Cleaning;
    this.workingStatusItem = Db.Get().MiscStatusItems.Cleaning;
    this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_desalinator_kanim"))
    };
    this.workAnims = DesalinatorWorkableEmpty.WORK_ANIMS;
    this.workingPstComplete = new HashedString[1]
    {
      DesalinatorWorkableEmpty.PST_ANIM
    };
    this.workingPstFailed = new HashedString[1]
    {
      DesalinatorWorkableEmpty.PST_ANIM
    };
    this.synchronizeAnims = false;
  }

  protected override void OnCompleteWork(Worker worker)
  {
    ++this.timesCleaned;
    base.OnCompleteWork(worker);
  }
}
