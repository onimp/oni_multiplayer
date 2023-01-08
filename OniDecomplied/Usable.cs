// Decompiled with JetBrains decompiler
// Type: Usable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public abstract class Usable : KMonoBehaviour, IStateMachineTarget
{
  private StateMachine.Instance smi;

  public abstract void StartUsing(User user);

  protected void StartUsing(StateMachine.Instance smi, User user)
  {
    DebugUtil.Assert(this.smi == null);
    DebugUtil.Assert(smi != null);
    this.smi = smi;
    smi.OnStop += new Action<string, StateMachine.Status>(user.OnStateMachineStop);
    smi.StartSM();
  }

  public void StopUsing(User user)
  {
    if (this.smi == null)
      return;
    this.smi.OnStop -= new Action<string, StateMachine.Status>(user.OnStateMachineStop);
    this.smi.StopSM("Usable.StopUsing");
    this.smi = (StateMachine.Instance) null;
  }
}
