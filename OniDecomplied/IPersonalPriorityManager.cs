// Decompiled with JetBrains decompiler
// Type: IPersonalPriorityManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public interface IPersonalPriorityManager
{
  int GetAssociatedSkillLevel(ChoreGroup group);

  int GetPersonalPriority(ChoreGroup group);

  void SetPersonalPriority(ChoreGroup group, int value);

  bool IsChoreGroupDisabled(ChoreGroup group);

  void ResetPersonalPriorities();
}
