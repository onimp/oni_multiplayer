// Decompiled with JetBrains decompiler
// Type: SplatWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class SplatWorkable : Workable
{
  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Mopping;
    this.attributeConverter = Db.Get().AttributeConverters.TidyingSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Basekeeping.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    this.multitoolContext = HashedString.op_Implicit("disinfect");
    this.multitoolHitEffectTag = Tag.op_Implicit("fx_disinfect_splash");
    this.synchronizeAnims = false;
    Prioritizable.AddRef(((Component) this).gameObject);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.SetWorkTime(5f);
  }
}
