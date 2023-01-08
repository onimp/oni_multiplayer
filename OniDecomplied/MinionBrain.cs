// Decompiled with JetBrains decompiler
// Type: MinionBrain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MinionBrain : Brain
{
  [MyCmpReq]
  public Navigator Navigator;
  [MyCmpGet]
  public OxygenBreather OxygenBreather;
  private float lastResearchCompleteEmoteTime;
  private static readonly EventSystem.IntraObjectHandler<MinionBrain> AnimTrackStoredItemDelegate = new EventSystem.IntraObjectHandler<MinionBrain>((Action<MinionBrain, object>) ((component, data) => component.AnimTrackStoredItem(data)));
  private static readonly EventSystem.IntraObjectHandler<MinionBrain> OnUnstableGroundImpactDelegate = new EventSystem.IntraObjectHandler<MinionBrain>((Action<MinionBrain, object>) ((component, data) => component.OnUnstableGroundImpact(data)));

  public bool IsCellClear(int cell)
  {
    if (Grid.Reserved[cell])
      return false;
    GameObject gameObject = Grid.Objects[cell, 0];
    return (!Object.op_Inequality((Object) gameObject, (Object) null) || !Object.op_Inequality((Object) ((Component) this).gameObject, (Object) gameObject) ? 0 : (!gameObject.GetComponent<Navigator>().IsMoving() ? 1 : 0)) == 0;
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Navigator.SetAbilities((PathFinderAbilities) new MinionPathFinderAbilities(this.Navigator));
    this.Subscribe<MinionBrain>(-1697596308, MinionBrain.AnimTrackStoredItemDelegate);
    this.Subscribe<MinionBrain>(-975551167, MinionBrain.OnUnstableGroundImpactDelegate);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    foreach (GameObject go in ((Component) this).GetComponent<Storage>().items)
      this.AddAnimTracker(go);
    Game.Instance.Subscribe(-107300940, new Action<object>(this.OnResearchComplete));
  }

  private void AnimTrackStoredItem(object data)
  {
    Storage component = ((Component) this).GetComponent<Storage>();
    GameObject go = (GameObject) data;
    this.RemoveTracker(go);
    if (!component.items.Contains(go))
      return;
    this.AddAnimTracker(go);
  }

  private void AddAnimTracker(GameObject go)
  {
    KAnimControllerBase component = go.GetComponent<KAnimControllerBase>();
    if (Object.op_Equality((Object) component, (Object) null) || component.AnimFiles == null || component.AnimFiles.Length == 0 || !Object.op_Inequality((Object) component.AnimFiles[0], (Object) null) || !((Component) component).GetComponent<Pickupable>().trackOnPickup)
      return;
    KBatchedAnimTracker kbatchedAnimTracker = go.AddComponent<KBatchedAnimTracker>();
    kbatchedAnimTracker.useTargetPoint = false;
    kbatchedAnimTracker.fadeOut = false;
    kbatchedAnimTracker.symbol = new HashedString("snapTo_chest");
    kbatchedAnimTracker.forceAlwaysVisible = true;
  }

  private void RemoveTracker(GameObject go)
  {
    KBatchedAnimTracker component = go.GetComponent<KBatchedAnimTracker>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    Object.Destroy((Object) component);
  }

  public override void UpdateBrain()
  {
    base.UpdateBrain();
    if (Object.op_Equality((Object) Game.Instance, (Object) null))
      return;
    if (!Game.Instance.savedInfo.discoveredSurface && World.Instance.zoneRenderData.GetSubWorldZoneType(Grid.PosToCell(((Component) this).gameObject)) == 7)
    {
      Game.Instance.savedInfo.discoveredSurface = true;
      DiscoveredSpaceMessage discoveredSpaceMessage = new DiscoveredSpaceMessage(TransformExtensions.GetPosition(((Component) this).gameObject.transform));
      Messenger.Instance.QueueMessage((Message) discoveredSpaceMessage);
      Game.Instance.Trigger(-818188514, (object) ((Component) this).gameObject);
    }
    if (Game.Instance.savedInfo.discoveredOilField || World.Instance.zoneRenderData.GetSubWorldZoneType(Grid.PosToCell(((Component) this).gameObject)) != 6)
      return;
    Game.Instance.savedInfo.discoveredOilField = true;
  }

  private void RegisterReactEmotePair(string reactable_id, Emote emote, float max_trigger_time)
  {
    if (Object.op_Equality((Object) ((Component) this).gameObject, (Object) null))
      return;
    ReactionMonitor.Instance smi = ((Component) this).gameObject.GetSMI<ReactionMonitor.Instance>();
    if (smi == null)
      return;
    EmoteChore emoteChore = new EmoteChore((IStateMachineTarget) ((Component) this).gameObject.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteIdle, emote);
    SelfEmoteReactable reactable = new SelfEmoteReactable(((Component) this).gameObject, HashedString.op_Implicit(reactable_id), Db.Get().ChoreTypes.Cough, max_trigger_time);
    emoteChore.PairReactable(reactable);
    reactable.SetEmote(emote);
    reactable.PairEmote(emoteChore);
    smi.AddOneshotReactable(reactable);
  }

  private void OnResearchComplete(object data)
  {
    if ((double) Time.time - (double) this.lastResearchCompleteEmoteTime <= 1.0)
      return;
    this.RegisterReactEmotePair("ResearchComplete", Db.Get().Emotes.Minion.ResearchComplete, 3f);
    this.lastResearchCompleteEmoteTime = Time.time;
  }

  public Notification CreateCollapseNotification()
  {
    MinionIdentity component = ((Component) this).GetComponent<MinionIdentity>();
    return new Notification((string) MISC.NOTIFICATIONS.TILECOLLAPSE.NAME, NotificationType.Bad, (Func<List<Notification>, object, string>) ((notificationList, data) => (string) MISC.NOTIFICATIONS.TILECOLLAPSE.TOOLTIP + notificationList.ReduceMessages(false)), (object) ("/t• " + component.GetProperName()));
  }

  public void RemoveCollapseNotification(Notification notification)
  {
    Vector3 position = TransformExtensions.GetPosition(notification.clickFocus);
    position.z = -40f;
    WorldContainer myWorld = ((Component) notification.clickFocus).gameObject.GetMyWorld();
    if (Object.op_Inequality((Object) myWorld, (Object) null) && myWorld.IsDiscovered)
      CameraController.Instance.ActiveWorldStarWipe(myWorld.id, position);
    ((Component) this).gameObject.AddOrGet<Notifier>().Remove(notification);
  }

  private void OnUnstableGroundImpact(object data)
  {
    GameObject telepad = GameUtil.GetTelepad(((Component) this).gameObject.GetMyWorld().id);
    Navigator component = ((Component) this).GetComponent<Navigator>();
    Assignable assignable = ((Component) this).GetComponent<MinionIdentity>().GetSoleOwner().GetAssignable(Db.Get().AssignableSlots.Bed);
    int num = !Object.op_Inequality((Object) assignable, (Object) null) ? 0 : (component.CanReach(Grid.PosToCell(TransformExtensions.GetPosition(assignable.transform))) ? 1 : 0);
    bool flag = Object.op_Inequality((Object) telepad, (Object) null) && component.CanReach(Grid.PosToCell(TransformExtensions.GetPosition(telepad.transform)));
    if (num != 0 || flag)
      return;
    this.RegisterReactEmotePair("UnstableGroundShock", Db.Get().Emotes.Minion.Shock, 1f);
    Notification notification = this.CreateCollapseNotification();
    notification.customClickCallback = (Notification.ClickCallback) (o => this.RemoveCollapseNotification(notification));
    ((Component) this).gameObject.AddOrGet<Notifier>().Add(notification);
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    Game.Instance.Unsubscribe(-107300940, new Action<object>(this.OnResearchComplete));
  }
}
