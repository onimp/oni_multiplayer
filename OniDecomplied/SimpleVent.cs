// Decompiled with JetBrains decompiler
// Type: SimpleVent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SimpleVent")]
public class SimpleVent : KMonoBehaviour
{
  [MyCmpGet]
  private Operational operational;
  private static readonly EventSystem.IntraObjectHandler<SimpleVent> OnChangedDelegate = new EventSystem.IntraObjectHandler<SimpleVent>((Action<SimpleVent, object>) ((component, data) => component.OnChanged(data)));

  protected virtual void OnPrefabInit()
  {
    this.Subscribe<SimpleVent>(-592767678, SimpleVent.OnChangedDelegate);
    this.Subscribe<SimpleVent>(-111137758, SimpleVent.OnChangedDelegate);
  }

  protected virtual void OnSpawn() => this.OnChanged((object) null);

  private void OnChanged(object data)
  {
    if (this.operational.IsFunctional)
      ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, (object) this);
    else
      ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, (StatusItem) null);
  }
}
