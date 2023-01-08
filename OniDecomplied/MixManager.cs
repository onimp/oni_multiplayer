// Decompiled with JetBrains decompiler
// Type: MixManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class MixManager : MonoBehaviour
{
  private void Update()
  {
    if (AudioMixer.instance == null || !AudioMixer.instance.persistentSnapshotsActive)
      return;
    AudioMixer.instance.UpdatePersistentSnapshotParameters();
  }

  private void OnApplicationFocus(bool hasFocus)
  {
    if (AudioMixer.instance == null || Object.op_Equality((Object) AudioMixerSnapshots.Get(), (Object) null))
      return;
    if (!hasFocus && KPlayerPrefs.GetInt(AudioOptionsScreen.MuteOnFocusLost) == 1)
      AudioMixer.instance.Start(AudioMixerSnapshots.Get().GameNotFocusedSnapshot);
    else
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().GameNotFocusedSnapshot);
  }
}
