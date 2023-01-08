// Decompiled with JetBrains decompiler
// Type: AudioDebug
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AudioDebug")]
public class AudioDebug : KMonoBehaviour
{
  private static AudioDebug instance;
  public bool musicEnabled;
  public bool debugSoundEvents;
  public bool debugFloorSounds;
  public bool debugGameEventSounds;
  public bool debugNotificationSounds;
  public bool debugVoiceSounds;

  public static AudioDebug Get() => AudioDebug.instance;

  protected virtual void OnPrefabInit() => AudioDebug.instance = this;

  public void ToggleMusic()
  {
    if (Object.op_Inequality((Object) Game.Instance, (Object) null))
      Game.Instance.SetMusicEnabled(this.musicEnabled);
    this.musicEnabled = !this.musicEnabled;
  }
}
