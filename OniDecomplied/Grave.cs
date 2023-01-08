// Decompiled with JetBrains decompiler
// Type: Grave
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Grave : StateMachineComponent<Grave.StatesInstance>
{
  [Serialize]
  public string graveName;
  [Serialize]
  public int epitaphIdx;
  [Serialize]
  public float burialTime = -1f;
  private static readonly CellOffset[] DELIVERY_OFFSETS = new CellOffset[1];
  private static readonly EventSystem.IntraObjectHandler<Grave> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<Grave>((Action<Grave, object>) ((component, data) => component.OnStorageChanged(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<Grave>(-1697596308, Grave.OnStorageChangedDelegate);
    this.epitaphIdx = Random.Range(0, int.MaxValue);
  }

  protected virtual void OnSpawn()
  {
    ((Component) this).GetComponent<Storage>().SetOffsets(Grave.DELIVERY_OFFSETS);
    Storage component = ((Component) this).GetComponent<Storage>();
    Storage storage = component;
    storage.OnWorkableEventCB = storage.OnWorkableEventCB + new Action<Workable, Workable.WorkableEvent>(this.OnWorkEvent);
    KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("anim_bury_dupe_kanim"));
    int num = 0;
    KAnim.Anim anim2;
    while (true)
    {
      anim2 = anim1.GetData().GetAnim(num);
      if (anim2 != null)
      {
        if (!(anim2.name == "working_pre"))
          ++num;
        else
          break;
      }
      else
        goto label_5;
    }
    float work_time = (float) (anim2.numFrames - 3) / anim2.frameRate;
    component.SetWorkTime(work_time);
label_5:
    base.OnSpawn();
    this.smi.StartSM();
    Components.Graves.Add(this);
  }

  protected override void OnCleanUp()
  {
    Components.Graves.Remove(this);
    base.OnCleanUp();
  }

  private void OnStorageChanged(object data)
  {
    GameObject gameObject = (GameObject) data;
    if (!Object.op_Inequality((Object) gameObject, (Object) null))
      return;
    this.graveName = ((Object) gameObject).name;
    Util.KDestroyGameObject(gameObject);
  }

  private void OnWorkEvent(Workable workable, Workable.WorkableEvent evt)
  {
  }

  public class StatesInstance : 
    GameStateMachine<Grave.States, Grave.StatesInstance, Grave, object>.GameInstance
  {
    private FetchChore chore;

    public StatesInstance(Grave master)
      : base(master)
    {
    }

    public void CreateFetchTask()
    {
      ChoreType fetchCritical = Db.Get().ChoreTypes.FetchCritical;
      Storage component = this.GetComponent<Storage>();
      HashSet<Tag> tags = new HashSet<Tag>();
      tags.Add(GameTags.Minion);
      Tag corpse = GameTags.Corpse;
      this.chore = new FetchChore(fetchCritical, component, 1f, tags, FetchChore.MatchCriteria.MatchID, corpse);
      this.chore.allowMultifetch = false;
    }

    public void CancelFetchTask()
    {
      this.chore.Cancel("Exit State");
      this.chore = (FetchChore) null;
    }
  }

  public class States : GameStateMachine<Grave.States, Grave.StatesInstance, Grave>
  {
    public GameStateMachine<Grave.States, Grave.StatesInstance, Grave, object>.State empty;
    public GameStateMachine<Grave.States, Grave.StatesInstance, Grave, object>.State full;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.empty;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.empty.PlayAnim("open").Enter("CreateFetchTask", (StateMachine<Grave.States, Grave.StatesInstance, Grave, object>.State.Callback) (smi => smi.CreateFetchTask())).Exit("CancelFetchTask", (StateMachine<Grave.States, Grave.StatesInstance, Grave, object>.State.Callback) (smi => smi.CancelFetchTask())).ToggleMainStatusItem(Db.Get().BuildingStatusItems.GraveEmpty).EventTransition(GameHashes.OnStorageChange, this.full);
      this.full.PlayAnim("closed").ToggleMainStatusItem(Db.Get().BuildingStatusItems.Grave).Enter((StateMachine<Grave.States, Grave.StatesInstance, Grave, object>.State.Callback) (smi =>
      {
        if ((double) smi.master.burialTime >= 0.0)
          return;
        smi.master.burialTime = GameClock.Instance.GetTime();
      }));
    }
  }
}
