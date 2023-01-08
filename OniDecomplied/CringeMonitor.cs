// Decompiled with JetBrains decompiler
// Type: CringeMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class CringeMonitor : GameStateMachine<CringeMonitor, CringeMonitor.Instance>
{
  public GameStateMachine<CringeMonitor, CringeMonitor.Instance, IStateMachineTarget, object>.State idle;
  public GameStateMachine<CringeMonitor, CringeMonitor.Instance, IStateMachineTarget, object>.State cringe;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.idle;
    this.idle.EventHandler(GameHashes.Cringe, new GameStateMachine<CringeMonitor, CringeMonitor.Instance, IStateMachineTarget, object>.GameEvent.Callback(this.TriggerCringe));
    this.cringe.ToggleReactable((Func<CringeMonitor.Instance, Reactable>) (smi => smi.GetReactable())).ToggleStatusItem((Func<CringeMonitor.Instance, StatusItem>) (smi => smi.GetStatusItem())).ScheduleGoTo(3f, (StateMachine.BaseState) this.idle);
  }

  private void TriggerCringe(CringeMonitor.Instance smi, object data)
  {
    if (smi.GetComponent<KPrefabID>().HasTag(GameTags.Suit))
      return;
    smi.SetCringeSourceData(data);
    smi.GoTo((StateMachine.BaseState) this.cringe);
  }

  public new class Instance : 
    GameStateMachine<CringeMonitor, CringeMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    private StatusItem statusItem;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }

    public void SetCringeSourceData(object data) => this.statusItem = new StatusItem("CringeSource", (string) data, (string) null, "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);

    public Reactable GetReactable()
    {
      SelfEmoteReactable reactable = new SelfEmoteReactable(this.master.gameObject, HashedString.op_Implicit("Cringe"), Db.Get().ChoreTypes.EmoteHighPriority, localCooldown: 0.0f);
      reactable.SetEmote(Db.Get().Emotes.Minion.Cringe);
      reactable.preventChoreInterruption = true;
      return (Reactable) reactable;
    }

    public StatusItem GetStatusItem() => this.statusItem;
  }
}
