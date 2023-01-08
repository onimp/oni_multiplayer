// Decompiled with JetBrains decompiler
// Type: LonelyMinionMailbox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class LonelyMinionMailbox : KMonoBehaviour
{
  public LonelyMinionHouse.Instance House;

  public void Initialize(LonelyMinionHouse.Instance house)
  {
    this.House = house;
    SingleEntityReceptacle component = ((Component) this).GetComponent<SingleEntityReceptacle>();
    component.occupyingObjectRelativePosition = this.transform.InverseTransformPoint(house.GetParcelPosition());
    component.occupyingObjectRelativePosition.z = -1f;
    StoryInstance storyInstance = StoryManager.Instance.GetStoryInstance(Db.Get().Stories.LonelyMinion.HashId);
    storyInstance.StoryStateChanged += new Action<StoryInstance.State>(this.OnStoryStateChanged);
    this.OnStoryStateChanged(storyInstance.CurrentState);
  }

  protected virtual void OnSpawn()
  {
    if (!StoryManager.Instance.CheckState(StoryInstance.State.COMPLETE, Db.Get().Stories.LonelyMinion))
      return;
    ((Component) this).gameObject.AddOrGet<Deconstructable>().allowDeconstruction = true;
  }

  protected virtual void OnCleanUp()
  {
    StoryManager.Instance.GetStoryInstance(Db.Get().Stories.LonelyMinion.HashId).StoryStateChanged -= new Action<StoryInstance.State>(this.OnStoryStateChanged);
    ((Component) this).GetComponent<KBatchedAnimController>().onAnimComplete -= new KAnimControllerBase.KAnimEvent(this.OnAnimQueueComplete);
  }

  private void OnStoryStateChanged(StoryInstance.State state)
  {
    QuestInstance quest = QuestManager.GetInstance(this.House.QuestOwnerId, Db.Get().Quests.LonelyMinionFoodQuest);
    if (state == StoryInstance.State.IN_PROGRESS)
    {
      this.Subscribe(-731304873, new Action<object>(this.OnStorageChanged));
      SingleEntityReceptacle entityReceptacle = ((Component) this).gameObject.AddOrGet<SingleEntityReceptacle>();
      ((Behaviour) entityReceptacle).enabled = true;
      entityReceptacle.AddAdditionalCriteria((Func<GameObject, bool>) (candidate =>
      {
        EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(((Tag) ref candidate.GetComponent<KPrefabID>().PrefabTag).Name);
        int valueHandle = 0;
        if (foodInfo == null)
          return false;
        return quest.DataSatisfiesCriteria(new Quest.ItemData()
        {
          CriteriaId = LonelyMinionConfig.FoodCriteriaId,
          QualifyingTag = GameTags.Edible,
          CurrentValue = (float) foodInfo.Quality
        }, ref valueHandle);
      }));
      RootMenu.Instance.Refresh();
      this.OnStorageChanged((object) entityReceptacle.Occupant);
    }
    if (state != StoryInstance.State.COMPLETE)
      return;
    this.Unsubscribe(-731304873, new Action<object>(this.OnStorageChanged));
    ((Component) this).gameObject.AddOrGet<Deconstructable>().allowDeconstruction = true;
  }

  private void OnStorageChanged(object data) => this.House.MailboxContentChanged(data as GameObject);

  private void OnAnimQueueComplete(HashedString name)
  {
    if (HashedString.op_Inequality(name, LonelyMinionMailboxConfig.CleanupAnimation))
      return;
    Object.Destroy((Object) ((Component) this).gameObject);
  }
}
