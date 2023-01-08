// Decompiled with JetBrains decompiler
// Type: INToggleSideScreenControl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public interface INToggleSideScreenControl
{
  string SidescreenTitleKey { get; }

  List<LocString> Options { get; }

  List<LocString> Tooltips { get; }

  string Description { get; }

  int SelectedOption { get; }

  int QueuedOption { get; }

  void QueueSelectedOption(int option);
}
