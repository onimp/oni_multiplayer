// Decompiled with JetBrains decompiler
// Type: SolidBooster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class SolidBooster : RocketEngine
{
  public Storage fuelStorage;
  private static readonly EventSystem.IntraObjectHandler<SolidBooster> OnRocketLandedDelegate = new EventSystem.IntraObjectHandler<SolidBooster>((Action<SolidBooster, object>) ((component, data) => component.OnRocketLanded(data)));

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<SolidBooster>(-887025858, SolidBooster.OnRocketLandedDelegate);
  }

  [ContextMenu("Fill Tank")]
  public void FillTank()
  {
    Element element1 = ElementLoader.GetElement(this.fuelTag);
    this.fuelStorage.Store(element1.substance.SpawnResource(TransformExtensions.GetPosition(((Component) this).gameObject.transform), this.fuelStorage.capacityKg / 2f, element1.defaultValues.temperature, byte.MaxValue, 0));
    Element element2 = ElementLoader.GetElement(GameTags.OxyRock);
    this.fuelStorage.Store(element2.substance.SpawnResource(TransformExtensions.GetPosition(((Component) this).gameObject.transform), this.fuelStorage.capacityKg / 2f, element2.defaultValues.temperature, byte.MaxValue, 0));
  }

  private void OnRocketLanded(object data)
  {
    if (!Object.op_Inequality((Object) this.fuelStorage, (Object) null) || this.fuelStorage.items == null)
      return;
    for (int index = this.fuelStorage.items.Count - 1; index >= 0; --index)
      Util.KDestroyGameObject(this.fuelStorage.items[index]);
    this.fuelStorage.items.Clear();
  }
}
