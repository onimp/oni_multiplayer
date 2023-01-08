// Decompiled with JetBrains decompiler
// Type: GameplayEventMinionFilter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class GameplayEventMinionFilter
{
  public string id;
  public GameplayEventMinionFilter.FilterFn filter;

  public delegate bool FilterFn(MinionIdentity minion);
}
