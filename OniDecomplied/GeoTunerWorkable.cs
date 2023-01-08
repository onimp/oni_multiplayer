// Decompiled with JetBrains decompiler
// Type: GeoTunerWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;

public class GeoTunerWorkable : Workable
{
  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.SetWorkTime(30f);
    this.requiredSkillPerk = Db.Get().SkillPerks.AllowGeyserTuning.Id;
    this.SetWorkerStatusItem(Db.Get().DuplicantStatusItems.Studying);
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_geotuner_kanim"))
    };
    this.attributeConverter = Db.Get().AttributeConverters.GeotuningSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    this.lightEfficiencyBonus = true;
  }
}
