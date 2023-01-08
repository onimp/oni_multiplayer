// Decompiled with JetBrains decompiler
// Type: UISounds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UISounds")]
public class UISounds : KMonoBehaviour
{
  [SerializeField]
  private bool logSounds;
  [SerializeField]
  private UISounds.SoundData[] soundData;

  public static UISounds Instance { get; private set; }

  public static void DestroyInstance() => UISounds.Instance = (UISounds) null;

  protected virtual void OnPrefabInit() => UISounds.Instance = this;

  public static void PlaySound(UISounds.Sound sound) => UISounds.Instance.PlaySoundInternal(sound);

  private void PlaySoundInternal(UISounds.Sound sound)
  {
    for (int index = 0; index < this.soundData.Length; ++index)
    {
      if (this.soundData[index].sound == sound)
      {
        if (this.logSounds)
          DebugUtil.LogArgs(new object[2]
          {
            (object) "Play sound",
            (object) this.soundData[index].name
          });
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound(this.soundData[index].name));
      }
    }
  }

  public enum Sound
  {
    NegativeNotification,
    PositiveNotification,
    Select,
    Negative,
    Back,
    ClickObject,
    HUD_Mouseover,
    Object_Mouseover,
    ClickHUD,
  }

  [Serializable]
  private struct SoundData
  {
    public string name;
    public UISounds.Sound sound;
  }
}
