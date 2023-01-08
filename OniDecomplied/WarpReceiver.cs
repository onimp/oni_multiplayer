// Decompiled with JetBrains decompiler
// Type: WarpReceiver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WarpReceiver : Workable
{
  [MyCmpAdd]
  public Notifier notifier;
  private WarpReceiver.WarpReceiverSM.Instance warpReceiverSMI;
  private Notification notification;
  [Serialize]
  public bool IsConsumed;
  private Chore chore;
  [Serialize]
  public bool Used;

  protected override void OnPrefabInit() => base.OnPrefabInit();

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.warpReceiverSMI = new WarpReceiver.WarpReceiverSM.Instance(this);
    this.warpReceiverSMI.StartSM();
    Components.WarpReceivers.Add(this);
  }

  public void ReceiveWarpedDuplicant(Worker dupe)
  {
    TransformExtensions.SetPosition(dupe.transform, Grid.CellToPos(Grid.PosToCell((KMonoBehaviour) this), (CellAlignment) 1, Grid.SceneLayer.Move));
    Debug.Assert(this.chore == null);
    KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("anim_interacts_warp_portal_receiver_kanim"));
    ChoreType migrate = Db.Get().ChoreTypes.Migrate;
    KAnimFile kanimFile = anim1;
    ChoreProvider component1 = ((Component) dupe).GetComponent<ChoreProvider>();
    Action<Chore> on_complete = (Action<Chore>) (o => this.CompleteChore());
    KAnimFile override_anims = kanimFile;
    this.chore = (Chore) new WorkChore<Workable>(migrate, (IStateMachineTarget) this, component1, on_complete: on_complete, ignore_schedule_block: true, override_anims: override_anims, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.compulsory);
    Workable component2 = ((Component) this).GetComponent<Workable>();
    component2.workLayer = Grid.SceneLayer.Building;
    component2.workAnims = new HashedString[2]
    {
      HashedString.op_Implicit("printing_pre"),
      HashedString.op_Implicit("printing_loop")
    };
    component2.workingPstComplete = new HashedString[1]
    {
      HashedString.op_Implicit("printing_pst")
    };
    component2.workingPstFailed = new HashedString[1]
    {
      HashedString.op_Implicit("printing_pst")
    };
    component2.synchronizeAnims = true;
    float work_time = 0.0f;
    KAnimFileData data = anim1.GetData();
    for (int index = 0; index < data.animCount; ++index)
    {
      KAnim.Anim anim2 = data.GetAnim(index);
      if (((IEnumerable<HashedString>) component2.workAnims).Contains<HashedString>(anim2.hash))
        work_time += anim2.totalTime;
    }
    component2.SetWorkTime(work_time);
    this.Used = true;
  }

  private void CompleteChore()
  {
    this.chore.Cleanup();
    this.chore = (Chore) null;
    this.warpReceiverSMI.GoTo((StateMachine.BaseState) this.warpReceiverSMI.sm.idle);
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    Components.WarpReceivers.Remove(this);
  }

  public class WarpReceiverSM : 
    GameStateMachine<WarpReceiver.WarpReceiverSM, WarpReceiver.WarpReceiverSM.Instance, WarpReceiver>
  {
    public GameStateMachine<WarpReceiver.WarpReceiverSM, WarpReceiver.WarpReceiverSM.Instance, WarpReceiver, object>.State idle;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.idle.PlayAnim("idle");
    }

    public new class Instance : 
      GameStateMachine<WarpReceiver.WarpReceiverSM, WarpReceiver.WarpReceiverSM.Instance, WarpReceiver, object>.GameInstance
    {
      public Instance(WarpReceiver master)
        : base(master)
      {
      }
    }
  }
}
