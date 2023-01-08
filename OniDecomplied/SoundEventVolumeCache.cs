// Decompiled with JetBrains decompiler
// Type: SoundEventVolumeCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class SoundEventVolumeCache : Singleton<SoundEventVolumeCache>
{
  public Dictionary<HashedString, EffectorValues> volumeCache = new Dictionary<HashedString, EffectorValues>();

  public static SoundEventVolumeCache instance => Singleton<SoundEventVolumeCache>.Instance;

  public void AddVolume(string animFile, string eventName, EffectorValues vals)
  {
    HashedString key;
    // ISSUE: explicit constructor call
    ((HashedString) ref key).\u002Ector(animFile + ":" + eventName);
    if (!this.volumeCache.ContainsKey(key))
      this.volumeCache.Add(key, vals);
    else
      this.volumeCache[key] = vals;
  }

  public EffectorValues GetVolume(string animFile, string eventName)
  {
    HashedString key;
    // ISSUE: explicit constructor call
    ((HashedString) ref key).\u002Ector(animFile + ":" + eventName);
    return !this.volumeCache.ContainsKey(key) ? new EffectorValues() : this.volumeCache[key];
  }
}
