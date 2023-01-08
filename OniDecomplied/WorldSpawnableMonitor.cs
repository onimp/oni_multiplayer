// Decompiled with JetBrains decompiler
// Type: WorldSpawnableMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class WorldSpawnableMonitor : 
  GameStateMachine<WorldSpawnableMonitor, WorldSpawnableMonitor.Instance, IStateMachineTarget, WorldSpawnableMonitor.Def>
{
  public override void InitializeStates(out StateMachine.BaseState default_state) => default_state = (StateMachine.BaseState) this.root;

  public class Def : StateMachine.BaseDef
  {
    public Func<int, int> adjustSpawnLocationCb;
  }

  public new class Instance : 
    GameStateMachine<WorldSpawnableMonitor, WorldSpawnableMonitor.Instance, IStateMachineTarget, WorldSpawnableMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, WorldSpawnableMonitor.Def def)
      : base(master, def)
    {
    }
  }
}
