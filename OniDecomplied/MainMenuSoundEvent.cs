// Decompiled with JetBrains decompiler
// Type: MainMenuSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using UnityEngine;

public class MainMenuSoundEvent : SoundEvent
{
  public MainMenuSoundEvent(string file_name, string sound_name, int frame)
    : base(file_name, sound_name, frame, true, false, (float) SoundEvent.IGNORE_INTERVAL, false)
  {
  }

  public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
  {
    EventInstance eventInstance = KFMOD.BeginOneShot(this.sound, Vector3.zero, 1f);
    if (!((EventInstance) ref eventInstance).isValid())
      return;
    ((EventInstance) ref eventInstance).setParameterByName("frame", (float) this.frame, false);
    KFMOD.EndOneShot(eventInstance);
  }
}
