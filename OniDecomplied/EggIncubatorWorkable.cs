// Decompiled with JetBrains decompiler
// Type: EggIncubatorWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/EggIncubatorWorkable")]
public class EggIncubatorWorkable : Workable
{
  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.synchronizeAnims = false;
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_incubator_kanim"))
    };
    this.SetWorkTime(15f);
    this.showProgressBar = true;
    this.requiredSkillPerk = Db.Get().SkillPerks.CanWrangleCreatures.Id;
    this.attributeConverter = Db.Get().AttributeConverters.RanchingEffectDuration;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Ranching.Id;
    this.skillExperienceMultiplier = SKILLS.BARELY_EVER_EXPERIENCE;
  }

  protected override void OnCompleteWork(Worker worker)
  {
    EggIncubator component = ((Component) this).GetComponent<EggIncubator>();
    if (!Object.op_Implicit((Object) component) || !Object.op_Implicit((Object) component.Occupant))
      return;
    component.Occupant.GetSMI<IncubationMonitor.Instance>()?.ApplySongBuff();
  }
}
