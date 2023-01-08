// Decompiled with JetBrains decompiler
// Type: BuildingInternalConstructorWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class BuildingInternalConstructorWorkable : Workable
{
  private BuildingInternalConstructor.Instance constructorInstance;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
    this.minimumAttributeMultiplier = 0.75f;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
    this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
    this.resetProgressOnStop = false;
    this.multitoolContext = HashedString.op_Implicit("build");
    this.multitoolHitEffectTag = Tag.op_Implicit(EffectConfigs.BuildSplashId);
    this.workingPstComplete = (HashedString[]) null;
    this.workingPstFailed = (HashedString[]) null;
    this.SetOffsetTable(OffsetGroups.InvertedStandardTable);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.constructorInstance = ((Component) this).GetSMI<BuildingInternalConstructor.Instance>();
  }

  protected override void OnCompleteWork(Worker worker) => this.constructorInstance.ConstructionComplete();
}
