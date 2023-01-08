// Decompiled with JetBrains decompiler
// Type: MouthFlapSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class MouthFlapSoundEvent : SoundEvent
{
  public MouthFlapSoundEvent(string file_name, string sound_name, int frame, bool is_looping)
    : base(file_name, sound_name, frame, false, is_looping, (float) SoundEvent.IGNORE_INTERVAL, true)
  {
  }

  public override void OnPlay(AnimEventManager.EventPlayerData behaviour) => ((Component) behaviour.controller).GetSMI<SpeechMonitor.Instance>().PlaySpeech(this.name, (string) null);
}
