// Decompiled with JetBrains decompiler
// Type: EggCracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EggCracker")]
public class EggCracker : KMonoBehaviour
{
  [MyCmpReq]
  private ComplexFabricator refinery;
  [MyCmpReq]
  private ComplexFabricatorWorkable workable;
  private KBatchedAnimTracker tracker;
  private GameObject display_egg;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.refinery.choreType = Db.Get().ChoreTypes.Cook;
    this.refinery.fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
    this.workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
    this.workable.AttributeConverter = Db.Get().AttributeConverters.CookingSpeed;
    this.workable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
    this.workable.SkillExperienceSkillGroup = Db.Get().SkillGroups.Cooking.Id;
    this.workable.SkillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
    ComplexFabricatorWorkable workable = this.workable;
    workable.OnWorkableEventCB = workable.OnWorkableEventCB + new Action<Workable, Workable.WorkableEvent>(this.OnWorkableEvent);
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Object.Destroy((Object) this.tracker);
    this.tracker = (KBatchedAnimTracker) null;
  }

  private void OnWorkableEvent(Workable workable, Workable.WorkableEvent e)
  {
    switch (e)
    {
      case Workable.WorkableEvent.WorkStarted:
        ComplexRecipe currentWorkingOrder = this.refinery.CurrentWorkingOrder;
        if (currentWorkingOrder == null)
          break;
        ComplexRecipe.RecipeElement[] ingredients = currentWorkingOrder.ingredients;
        if (ingredients.Length == 0)
          break;
        this.display_egg = this.refinery.buildStorage.FindFirst(ingredients[0].material);
        this.PositionActiveEgg();
        break;
      case Workable.WorkableEvent.WorkCompleted:
        if (!Object.op_Implicit((Object) this.display_egg))
          break;
        this.display_egg.GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("hatching_pst"));
        break;
      case Workable.WorkableEvent.WorkStopped:
        Object.Destroy((Object) this.tracker);
        this.tracker = (KBatchedAnimTracker) null;
        this.display_egg = (GameObject) null;
        break;
    }
  }

  private void PositionActiveEgg()
  {
    if (!Object.op_Implicit((Object) this.display_egg))
      return;
    KBatchedAnimController component1 = this.display_egg.GetComponent<KBatchedAnimController>();
    component1.enabled = true;
    component1.SetSceneLayer(Grid.SceneLayer.BuildingUse);
    KSelectable component2 = this.display_egg.GetComponent<KSelectable>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      ((Behaviour) component2).enabled = true;
    this.tracker = this.display_egg.AddComponent<KBatchedAnimTracker>();
    this.tracker.symbol = HashedString.op_Implicit("snapto_egg");
  }
}
