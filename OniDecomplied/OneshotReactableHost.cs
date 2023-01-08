// Decompiled with JetBrains decompiler
// Type: OneshotReactableHost
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/OneshotReactableHost")]
public class OneshotReactableHost : KMonoBehaviour
{
  private Reactable reactable;
  public float lifetime = 1f;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    GameScheduler.Instance.Schedule("CleanupOneshotReactable", this.lifetime, new Action<object>(this.OnExpire), (object) null, (SchedulerGroup) null);
  }

  public void SetReactable(Reactable reactable) => this.reactable = reactable;

  private void OnExpire(object obj)
  {
    if (!this.reactable.IsReacting)
    {
      this.reactable.Cleanup();
      Object.Destroy((Object) ((Component) this).gameObject);
    }
    else
      GameScheduler.Instance.Schedule("CleanupOneshotReactable", 0.5f, new Action<object>(this.OnExpire), (object) null, (SchedulerGroup) null);
  }
}
