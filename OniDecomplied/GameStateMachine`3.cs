// Decompiled with JetBrains decompiler
// Type: GameStateMachine`3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public abstract class GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType> : 
  GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>
  where StateMachineType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>
  where StateMachineInstanceType : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.GameInstance
  where MasterType : IStateMachineTarget
{
}
