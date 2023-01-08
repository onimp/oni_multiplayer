// Decompiled with JetBrains decompiler
// Type: CreatureFeeder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CreatureFeeder")]
public class CreatureFeeder : KMonoBehaviour
{
  public string effectId;
  private static readonly EventSystem.IntraObjectHandler<CreatureFeeder> OnAteFromStorageDelegate = new EventSystem.IntraObjectHandler<CreatureFeeder>((Action<CreatureFeeder, object>) ((component, data) => component.OnAteFromStorage(data)));

  protected virtual void OnSpawn()
  {
    Components.CreatureFeeders.Add(this.GetMyWorldId(), this);
    this.Subscribe<CreatureFeeder>(-1452790913, CreatureFeeder.OnAteFromStorageDelegate);
  }

  protected virtual void OnCleanUp() => Components.CreatureFeeders.Remove(this.GetMyWorldId(), this);

  private void OnAteFromStorage(object data)
  {
    if (string.IsNullOrEmpty(this.effectId))
      return;
    (data as GameObject).GetComponent<Effects>().Add(this.effectId, true);
  }
}
