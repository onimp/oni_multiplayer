// Decompiled with JetBrains decompiler
// Type: AmbientSoundManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AmbientSoundManager")]
public class AmbientSoundManager : KMonoBehaviour
{
  [MyCmpAdd]
  private LoopingSounds loopingSounds;

  public static AmbientSoundManager Instance { get; private set; }

  public static void Destroy() => AmbientSoundManager.Instance = (AmbientSoundManager) null;

  protected virtual void OnPrefabInit() => AmbientSoundManager.Instance = this;

  protected virtual void OnSpawn() => base.OnSpawn();

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    AmbientSoundManager.Instance = (AmbientSoundManager) null;
  }
}
