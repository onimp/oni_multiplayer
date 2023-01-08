// Decompiled with JetBrains decompiler
// Type: AutoDisinfectableManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AutoDisinfectableManager")]
public class AutoDisinfectableManager : KMonoBehaviour, ISim1000ms
{
  private List<AutoDisinfectable> autoDisinfectables = new List<AutoDisinfectable>();
  public static AutoDisinfectableManager Instance;

  public static void DestroyInstance() => AutoDisinfectableManager.Instance = (AutoDisinfectableManager) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    AutoDisinfectableManager.Instance = this;
  }

  public void AddAutoDisinfectable(AutoDisinfectable auto_disinfectable) => this.autoDisinfectables.Add(auto_disinfectable);

  public void RemoveAutoDisinfectable(AutoDisinfectable auto_disinfectable)
  {
    auto_disinfectable.CancelChore();
    this.autoDisinfectables.Remove(auto_disinfectable);
  }

  public void Sim1000ms(float dt)
  {
    for (int index = 0; index < this.autoDisinfectables.Count; ++index)
      this.autoDisinfectables[index].RefreshChore();
  }
}
