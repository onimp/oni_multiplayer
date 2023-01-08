// Decompiled with JetBrains decompiler
// Type: IEmptyableCargo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public interface IEmptyableCargo
{
  bool CanEmptyCargo();

  void EmptyCargo();

  IStateMachineTarget master { get; }

  bool CanAutoDeploy { get; }

  bool AutoDeploy { get; set; }

  bool ChooseDuplicant { get; }

  bool ModuleDeployed { get; }

  MinionIdentity ChosenDuplicant { get; set; }
}
