// Decompiled with JetBrains decompiler
// Type: ParkSign
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ParkSign")]
public class ParkSign : KMonoBehaviour
{
  private static readonly EventSystem.IntraObjectHandler<ParkSign> TriggerRoomEffectsDelegate = new EventSystem.IntraObjectHandler<ParkSign>((Action<ParkSign, object>) ((component, data) => component.TriggerRoomEffects(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<ParkSign>(-832141045, ParkSign.TriggerRoomEffectsDelegate);
  }

  private void TriggerRoomEffects(object data)
  {
    GameObject gameObject = (GameObject) data;
    Game.Instance.roomProber.GetRoomOfGameObject(((Component) this).gameObject)?.roomType.TriggerRoomEffects(((Component) this).gameObject.GetComponent<KPrefabID>(), gameObject.GetComponent<Effects>());
  }
}
