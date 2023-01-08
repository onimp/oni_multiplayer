// Decompiled with JetBrains decompiler
// Type: GasBottler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/GasBottler")]
public class GasBottler : Workable
{
  public Storage storage;
  private GasBottler.Controller.Instance smi;

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.smi = new GasBottler.Controller.Instance(this);
    this.smi.StartSM();
    this.UpdateStoredItemState();
  }

  protected override void OnCleanUp()
  {
    if (this.smi != null)
      this.smi.StopSM(nameof (OnCleanUp));
    base.OnCleanUp();
  }

  private void UpdateStoredItemState()
  {
    this.storage.allowItemRemoval = this.smi != null && this.smi.GetCurrentState() == this.smi.sm.ready;
    foreach (GameObject gameObject in this.storage.items)
    {
      if (Object.op_Inequality((Object) gameObject, (Object) null))
        EventExtensions.Trigger(gameObject, -778359855, (object) this.storage);
    }
  }

  private class Controller : 
    GameStateMachine<GasBottler.Controller, GasBottler.Controller.Instance, GasBottler>
  {
    public GameStateMachine<GasBottler.Controller, GasBottler.Controller.Instance, GasBottler, object>.State empty;
    public GameStateMachine<GasBottler.Controller, GasBottler.Controller.Instance, GasBottler, object>.State filling;
    public GameStateMachine<GasBottler.Controller, GasBottler.Controller.Instance, GasBottler, object>.State ready;
    public GameStateMachine<GasBottler.Controller, GasBottler.Controller.Instance, GasBottler, object>.State pickup;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.empty;
      this.empty.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, this.filling, (StateMachine<GasBottler.Controller, GasBottler.Controller.Instance, GasBottler, object>.Transition.ConditionCallback) (smi => smi.master.storage.IsFull()));
      this.filling.PlayAnim("working").OnAnimQueueComplete(this.ready);
      this.ready.EventTransition(GameHashes.OnStorageChange, this.pickup, (StateMachine<GasBottler.Controller, GasBottler.Controller.Instance, GasBottler, object>.Transition.ConditionCallback) (smi => !smi.master.storage.IsFull())).Enter((StateMachine<GasBottler.Controller, GasBottler.Controller.Instance, GasBottler, object>.State.Callback) (smi =>
      {
        smi.master.storage.allowItemRemoval = true;
        foreach (GameObject gameObject in smi.master.storage.items)
        {
          gameObject.GetComponent<KPrefabID>().AddTag(GameTags.GasSource, false);
          EventExtensions.Trigger(gameObject, -778359855, (object) smi.master.storage);
        }
      })).Exit((StateMachine<GasBottler.Controller, GasBottler.Controller.Instance, GasBottler, object>.State.Callback) (smi =>
      {
        smi.master.storage.allowItemRemoval = false;
        foreach (GameObject gameObject in smi.master.storage.items)
          EventExtensions.Trigger(gameObject, -778359855, (object) smi.master.storage);
      }));
      this.pickup.PlayAnim("pick_up").OnAnimQueueComplete(this.empty);
    }

    public new class Instance : 
      GameStateMachine<GasBottler.Controller, GasBottler.Controller.Instance, GasBottler, object>.GameInstance
    {
      public Instance(GasBottler master)
        : base(master)
      {
      }
    }
  }
}
