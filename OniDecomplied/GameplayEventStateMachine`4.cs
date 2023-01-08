// Decompiled with JetBrains decompiler
// Type: GameplayEventStateMachine`4
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public abstract class GameplayEventStateMachine<StateMachineType, StateMachineInstanceType, MasterType, SecondMasterType> : 
  GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType>
  where StateMachineType : GameplayEventStateMachine<StateMachineType, StateMachineInstanceType, MasterType, SecondMasterType>
  where StateMachineInstanceType : GameplayEventStateMachine<StateMachineType, StateMachineInstanceType, MasterType, SecondMasterType>.GameplayEventStateMachineInstance
  where MasterType : IStateMachineTarget
  where SecondMasterType : GameplayEvent<StateMachineInstanceType>
{
  public void MonitorStart(
    StateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.TargetParameter target,
    StateMachineInstanceType smi)
  {
    GameObject gameObject = target.Get(smi);
    if (!Object.op_Inequality((Object) gameObject, (Object) null))
      return;
    EventExtensions.Trigger(gameObject, -1660384580, (object) smi.eventInstance);
  }

  public void MonitorChanged(
    StateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.TargetParameter target,
    StateMachineInstanceType smi)
  {
    GameObject gameObject = target.Get(smi);
    if (!Object.op_Inequality((Object) gameObject, (Object) null))
      return;
    EventExtensions.Trigger(gameObject, -1122598290, (object) smi.eventInstance);
  }

  public void MonitorStop(
    StateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.TargetParameter target,
    StateMachineInstanceType smi)
  {
    GameObject gameObject = target.Get(smi);
    if (!Object.op_Inequality((Object) gameObject, (Object) null))
      return;
    EventExtensions.Trigger(gameObject, -828272459, (object) smi.eventInstance);
  }

  public virtual EventInfoData GenerateEventPopupData(StateMachineInstanceType smi) => (EventInfoData) null;

  public class GameplayEventStateMachineInstance : 
    GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.GameInstance
  {
    public GameplayEventInstance eventInstance;
    public SecondMasterType gameplayEvent;

    public GameplayEventStateMachineInstance(
      MasterType master,
      GameplayEventInstance eventInstance,
      SecondMasterType gameplayEvent)
      : base(master)
    {
      this.gameplayEvent = gameplayEvent;
      this.eventInstance = eventInstance;
      eventInstance.GetEventPopupData = (GameplayEventInstance.GameplayEventPopupDataCallback) (() => this.smi.sm.GenerateEventPopupData(this.smi));
      this.serializationSuffix = ((Resource) (object) gameplayEvent).Id;
    }
  }
}
