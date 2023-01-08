// Decompiled with JetBrains decompiler
// Type: Toggleable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Toggleable")]
public class Toggleable : Workable
{
  private List<KeyValuePair<IToggleHandler, Chore>> targets;

  private Toggleable() => this.SetOffsetTable(OffsetGroups.InvertedStandardTable);

  protected override void OnPrefabInit()
  {
    this.faceTargetWhenWorking = true;
    base.OnPrefabInit();
    this.targets = new List<KeyValuePair<IToggleHandler, Chore>>();
    this.SetWorkTime(3f);
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Toggling;
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_use_remote_kanim"))
    };
    this.synchronizeAnims = false;
  }

  public int SetTarget(IToggleHandler handler)
  {
    this.targets.Add(new KeyValuePair<IToggleHandler, Chore>(handler, (Chore) null));
    return this.targets.Count - 1;
  }

  public IToggleHandler GetToggleHandlerForWorker(Worker worker)
  {
    int targetForWorker = this.GetTargetForWorker(worker);
    return targetForWorker != -1 ? this.targets[targetForWorker].Key : (IToggleHandler) null;
  }

  private int GetTargetForWorker(Worker worker)
  {
    for (int index = 0; index < this.targets.Count; ++index)
    {
      if (this.targets[index].Value != null && Object.op_Inequality((Object) this.targets[index].Value.driver, (Object) null) && Object.op_Equality((Object) ((Component) this.targets[index].Value.driver).gameObject, (Object) ((Component) worker).gameObject))
        return index;
    }
    return -1;
  }

  protected override void OnCompleteWork(Worker worker)
  {
    int targetForWorker = this.GetTargetForWorker(worker);
    if (targetForWorker != -1 && this.targets[targetForWorker].Key != null)
    {
      this.targets[targetForWorker] = new KeyValuePair<IToggleHandler, Chore>(this.targets[targetForWorker].Key, (Chore) null);
      this.targets[targetForWorker].Key.HandleToggle();
    }
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingSwitchToggle);
  }

  private void QueueToggle(int targetIdx)
  {
    if (this.targets[targetIdx].Value != null)
      return;
    if (DebugHandler.InstantBuildMode)
    {
      this.targets[targetIdx].Key.HandleToggle();
    }
    else
    {
      this.targets[targetIdx] = new KeyValuePair<IToggleHandler, Chore>(this.targets[targetIdx].Key, (Chore) new WorkChore<Toggleable>(Db.Get().ChoreTypes.Toggle, (IStateMachineTarget) this, only_when_operational: false, ignore_building_assignment: true));
      ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingSwitchToggle);
    }
  }

  public void Toggle(int targetIdx)
  {
    if (targetIdx >= this.targets.Count)
      return;
    if (this.targets[targetIdx].Value == null)
      this.QueueToggle(targetIdx);
    else
      this.CancelToggle(targetIdx);
  }

  private void CancelToggle(int targetIdx)
  {
    if (this.targets[targetIdx].Value == null)
      return;
    this.targets[targetIdx].Value.Cancel("Toggle cancelled");
    this.targets[targetIdx] = new KeyValuePair<IToggleHandler, Chore>(this.targets[targetIdx].Key, (Chore) null);
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingSwitchToggle);
  }

  public bool IsToggleQueued(int targetIdx) => this.targets[targetIdx].Value != null;
}
