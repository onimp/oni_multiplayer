// Decompiled with JetBrains decompiler
// Type: StateMachineExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public static class StateMachineExtensions
{
  public static bool IsNullOrStopped(this StateMachine.Instance smi) => smi == null || !smi.IsRunning();
}
