// Decompiled with JetBrains decompiler
// Type: OilFloaterMovementSound
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

internal class OilFloaterMovementSound : KMonoBehaviour
{
  public string sound;
  public bool isPlayingSound;
  public bool isMoving;
  private static readonly EventSystem.IntraObjectHandler<OilFloaterMovementSound> OnObjectMovementStateChangedDelegate = new EventSystem.IntraObjectHandler<OilFloaterMovementSound>((Action<OilFloaterMovementSound, object>) ((component, data) => component.OnObjectMovementStateChanged(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.sound = GlobalAssets.GetSound(this.sound);
    this.Subscribe<OilFloaterMovementSound>(1027377649, OilFloaterMovementSound.OnObjectMovementStateChangedDelegate);
    Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChanged), nameof (OilFloaterMovementSound));
  }

  private void OnObjectMovementStateChanged(object data)
  {
    this.isMoving = (GameHashes) data == GameHashes.ObjectMovementWakeUp;
    this.UpdateSound();
  }

  private void OnCellChanged() => this.UpdateSound();

  private void UpdateSound()
  {
    bool flag = this.isMoving && ((Component) this).GetComponent<Navigator>().CurrentNavType != NavType.Swim;
    if (flag == this.isPlayingSound)
      return;
    LoopingSounds component = ((Component) this).GetComponent<LoopingSounds>();
    if (flag)
      component.StartSound(this.sound);
    else
      component.StopSound(this.sound);
    this.isPlayingSound = flag;
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transform, new System.Action(this.OnCellChanged));
  }
}
