// Decompiled with JetBrains decompiler
// Type: PartyCakeWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class PartyCakeWorkable : Workable
{
  private static readonly HashedString[] WORK_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("salt_pre"),
    HashedString.op_Implicit("salt_loop")
  };
  private static readonly HashedString PST_ANIM = new HashedString("salt_pst");

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Cooking;
    this.alwaysShowProgressBar = true;
    this.resetProgressOnStop = false;
    this.attributeConverter = Db.Get().AttributeConverters.CookingSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_desalinator_kanim"))
    };
    this.workAnims = PartyCakeWorkable.WORK_ANIMS;
    this.workingPstComplete = new HashedString[1]
    {
      PartyCakeWorkable.PST_ANIM
    };
    this.workingPstFailed = new HashedString[1]
    {
      PartyCakeWorkable.PST_ANIM
    };
    this.synchronizeAnims = false;
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    base.OnWorkTick(worker, dt);
    ((Component) this).GetComponent<KBatchedAnimController>().SetPositionPercent(this.GetPercentComplete());
    return false;
  }
}
