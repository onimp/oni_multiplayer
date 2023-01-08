// Decompiled with JetBrains decompiler
// Type: ChoreProvider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/ChoreProvider")]
public class ChoreProvider : KMonoBehaviour
{
  public Dictionary<int, List<Chore>> choreWorldMap = new Dictionary<int, List<Chore>>();

  public string Name { get; private set; }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Game.Instance.Subscribe(880851192, new Action<object>(this.OnWorldParentChanged));
    Game.Instance.Subscribe(586301400, new Action<object>(this.OnMinionMigrated));
  }

  protected virtual void OnSpawn()
  {
    if (Object.op_Inequality((Object) ClusterManager.Instance, (Object) null))
      ClusterManager.Instance.Subscribe(-1078710002, new Action<object>(this.OnWorldRemoved));
    base.OnSpawn();
    this.Name = ((Object) this).name;
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Game.Instance.Unsubscribe(880851192, new Action<object>(this.OnWorldParentChanged));
    Game.Instance.Unsubscribe(586301400, new Action<object>(this.OnMinionMigrated));
    if (!Object.op_Inequality((Object) ClusterManager.Instance, (Object) null))
      return;
    ClusterManager.Instance.Unsubscribe(-1078710002, new Action<object>(this.OnWorldRemoved));
  }

  protected virtual void OnWorldRemoved(object data)
  {
    int num = (int) data;
    List<Chore> chores;
    if (!this.choreWorldMap.TryGetValue(ClusterManager.Instance.GetWorld(num).ParentWorldId, out chores))
      return;
    this.ClearWorldChores<Chore>(chores, num);
  }

  protected virtual void OnWorldParentChanged(object data)
  {
    List<Chore> oldChores;
    if (((!(data is WorldParentChangedEventArgs changedEventArgs) ? 0 : (changedEventArgs.lastParentId != (int) ClusterManager.INVALID_WORLD_IDX ? 1 : 0)) == 0 ? 0 : (changedEventArgs.lastParentId != changedEventArgs.world.ParentWorldId ? 1 : 0)) == 0 || !this.choreWorldMap.TryGetValue(changedEventArgs.lastParentId, out oldChores))
      return;
    List<Chore> newChores;
    if (!this.choreWorldMap.TryGetValue(changedEventArgs.world.ParentWorldId, out newChores))
      this.choreWorldMap[changedEventArgs.world.ParentWorldId] = newChores = new List<Chore>();
    this.TransferChores<Chore>(oldChores, newChores, changedEventArgs.world.ParentWorldId);
  }

  protected virtual void OnMinionMigrated(object data)
  {
    List<Chore> oldChores;
    if (((!(data is MinionMigrationEventArgs migrationEventArgs) ? 0 : (Object.op_Equality((Object) ((Component) migrationEventArgs.minionId).gameObject, (Object) ((Component) this).gameObject) ? 1 : 0)) == 0 ? 0 : (migrationEventArgs.prevWorldId != migrationEventArgs.targetWorldId ? 1 : 0)) == 0 || !this.choreWorldMap.TryGetValue(migrationEventArgs.prevWorldId, out oldChores))
      return;
    List<Chore> newChores;
    if (!this.choreWorldMap.TryGetValue(migrationEventArgs.targetWorldId, out newChores))
      this.choreWorldMap[migrationEventArgs.targetWorldId] = newChores = new List<Chore>();
    this.TransferChores<Chore>(oldChores, newChores, migrationEventArgs.targetWorldId);
  }

  protected void TransferChores<T>(List<T> oldChores, List<T> newChores, int transferId) where T : Chore
  {
    int index1 = oldChores.Count - 1;
    for (int index2 = index1; index2 >= 0; --index2)
    {
      T oldChore = oldChores[index2];
      if (oldChore.isNull)
        DebugUtil.DevLogError("[" + oldChore.GetType().Name + "] " + oldChore.GetReportName((string) null) + " has no target");
      else if (oldChore.gameObject.GetMyParentWorldId() == transferId)
      {
        newChores.Add(oldChore);
        oldChores[index2] = oldChores[index1];
        oldChores.RemoveAt(index1--);
      }
    }
  }

  protected void ClearWorldChores<T>(List<T> chores, int worldId) where T : Chore
  {
    int index1 = chores.Count - 1;
    for (int index2 = index1; index2 >= 0; --index2)
    {
      if (chores[index2].gameObject.GetMyWorldId() == worldId)
      {
        chores[index2] = chores[index1];
        chores.RemoveAt(index1--);
      }
    }
  }

  public virtual void AddChore(Chore chore)
  {
    chore.provider = this;
    List<Chore> choreList = (List<Chore>) null;
    int myParentWorldId = chore.gameObject.GetMyParentWorldId();
    if (!this.choreWorldMap.TryGetValue(myParentWorldId, out choreList))
      this.choreWorldMap[myParentWorldId] = choreList = new List<Chore>();
    choreList.Add(chore);
  }

  public virtual void RemoveChore(Chore chore)
  {
    if (chore == null)
      return;
    chore.provider = (ChoreProvider) null;
    List<Chore> choreList = (List<Chore>) null;
    if (!this.choreWorldMap.TryGetValue(chore.gameObject.GetMyParentWorldId(), out choreList))
      return;
    choreList.Remove(chore);
  }

  public virtual void CollectChores(
    ChoreConsumerState consumer_state,
    List<Chore.Precondition.Context> succeeded,
    List<Chore.Precondition.Context> failed_contexts)
  {
    List<Chore> choreList = (List<Chore>) null;
    if (!this.choreWorldMap.TryGetValue(consumer_state.gameObject.GetMyParentWorldId(), out choreList))
      return;
    for (int index = choreList.Count - 1; index >= 0; --index)
    {
      Chore chore = choreList[index];
      if (Object.op_Equality((Object) chore.provider, (Object) null))
      {
        chore.Cancel("no provider");
        choreList[index] = choreList[choreList.Count - 1];
        choreList.RemoveAt(choreList.Count - 1);
      }
      else
        chore.CollectChores(consumer_state, succeeded, failed_contexts, false);
    }
  }
}
