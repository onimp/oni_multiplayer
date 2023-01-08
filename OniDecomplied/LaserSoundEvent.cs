// Decompiled with JetBrains decompiler
// Type: LaserSoundEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class LaserSoundEvent : SoundEvent
{
  public LaserSoundEvent(string file_name, string sound_name, int frame, float min_interval)
    : base(file_name, sound_name, frame, true, true, min_interval, false)
  {
    this.noiseValues = SoundEventVolumeCache.instance.GetVolume(nameof (LaserSoundEvent), sound_name);
  }
}
