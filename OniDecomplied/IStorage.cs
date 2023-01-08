// Decompiled with JetBrains decompiler
// Type: IStorage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public interface IStorage
{
  bool ShouldShowInUI();

  bool allowUIItemRemoval { get; set; }

  GameObject Drop(GameObject go, bool do_disease_transfer = true);

  List<GameObject> GetItems();

  bool IsFull();

  bool IsEmpty();

  float Capacity();

  float RemainingCapacity();

  float GetAmountAvailable(Tag tag);

  void ConsumeIgnoringDisease(Tag tag, float amount);
}
