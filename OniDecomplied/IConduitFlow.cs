// Decompiled with JetBrains decompiler
// Type: IConduitFlow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public interface IConduitFlow
{
  void AddConduitUpdater(Action<float> callback, ConduitFlowPriority priority = ConduitFlowPriority.Default);

  void RemoveConduitUpdater(Action<float> callback);

  bool IsConduitEmpty(int cell);
}
