// Decompiled with JetBrains decompiler
// Type: OnDemandUpdater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class OnDemandUpdater : MonoBehaviour
{
  private List<IUpdateOnDemand> Updaters = new List<IUpdateOnDemand>();
  public static OnDemandUpdater Instance;

  public static void DestroyInstance() => OnDemandUpdater.Instance = (OnDemandUpdater) null;

  private void Awake() => OnDemandUpdater.Instance = this;

  public void Register(IUpdateOnDemand updater)
  {
    if (this.Updaters.Contains(updater))
      return;
    this.Updaters.Add(updater);
  }

  public void Unregister(IUpdateOnDemand updater)
  {
    if (!this.Updaters.Contains(updater))
      return;
    this.Updaters.Remove(updater);
  }

  private void Update()
  {
    for (int index = 0; index < this.Updaters.Count; ++index)
    {
      if (this.Updaters[index] != null)
        this.Updaters[index].UpdateOnDemand();
    }
  }
}
