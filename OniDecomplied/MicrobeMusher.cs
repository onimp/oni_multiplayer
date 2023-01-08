// Decompiled with JetBrains decompiler
// Type: MicrobeMusher
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class MicrobeMusher : ComplexFabricator
{
  [SerializeField]
  public Vector3 mushbarSpawnOffset = Vector3.right;
  private static readonly KAnimHashedString meterRationHash = new KAnimHashedString("meter_ration");
  private static readonly KAnimHashedString canHash = new KAnimHashedString("can");

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.choreType = Db.Get().ChoreTypes.Cook;
    this.fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater)), (object) null, (SchedulerGroup) null);
    this.workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Mushing;
    this.workable.AttributeConverter = Db.Get().AttributeConverters.CookingSpeed;
    this.workable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
    this.workable.SkillExperienceSkillGroup = Db.Get().SkillGroups.Cooking.Id;
    this.workable.SkillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
    this.workable.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[2]
    {
      "meter_target",
      "meter_ration"
    });
    this.workable.meter.meterController.SetSymbolVisiblity(MicrobeMusher.canHash, false);
    this.workable.meter.meterController.SetSymbolVisiblity(MicrobeMusher.meterRationHash, false);
  }

  protected override List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
  {
    List<GameObject> gameObjectList = base.SpawnOrderProduct(recipe);
    foreach (GameObject go in gameObjectList)
    {
      PrimaryElement component = go.GetComponent<PrimaryElement>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        if (Tag.op_Equality(go.PrefabID(), Tag.op_Implicit("MushBar")))
        {
          byte index = Db.Get().Diseases.GetIndex(HashedString.op_Implicit("FoodPoisoning"));
          component.AddDisease(index, 1000, "Made of mud");
        }
        if (go.GetComponent<PrimaryElement>().DiseaseCount > 0)
          Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_DiseaseCooking);
      }
    }
    return gameObjectList;
  }
}
