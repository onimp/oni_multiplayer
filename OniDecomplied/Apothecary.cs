// Decompiled with JetBrains decompiler
// Type: Apothecary
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class Apothecary : ComplexFabricator
{
  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.choreType = Db.Get().ChoreTypes.Compound;
    this.fetchChoreTypeIdHash = Db.Get().ChoreTypes.DoctorFetch.IdHash;
    this.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
    this.workable.AttributeConverter = Db.Get().AttributeConverters.CompoundingSpeed;
    this.workable.SkillExperienceSkillGroup = Db.Get().SkillGroups.MedicalAid.Id;
    this.workable.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    this.workable.requiredSkillPerk = Db.Get().SkillPerks.CanCompound.Id;
    this.workable.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_apothecary_kanim"))
    };
    this.workable.AnimOffset = new Vector3(-1f, 0.0f, 0.0f);
  }
}
