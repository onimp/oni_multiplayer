// Decompiled with JetBrains decompiler
// Type: KTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/KTime")]
public class KTime : KMonoBehaviour
{
  public float UnscaledGameTime { get; set; }

  public static KTime Instance { get; private set; }

  public static void DestroyInstance() => KTime.Instance = (KTime) null;

  protected virtual void OnPrefabInit()
  {
    KTime.Instance = this;
    this.UnscaledGameTime = Time.unscaledTime;
  }

  protected virtual void OnCleanUp() => KTime.Instance = (KTime) null;

  public void Update()
  {
    if (SpeedControlScreen.Instance.IsPaused)
      return;
    this.UnscaledGameTime += Time.unscaledDeltaTime;
  }
}
