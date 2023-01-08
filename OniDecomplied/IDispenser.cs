// Decompiled with JetBrains decompiler
// Type: IDispenser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public interface IDispenser
{
  List<Tag> DispensedItems();

  Tag SelectedItem();

  void SelectItem(Tag tag);

  void OnOrderDispense();

  void OnCancelDispense();

  bool HasOpenChore();

  event System.Action OnStopWorkEvent;
}
