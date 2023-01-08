// Decompiled with JetBrains decompiler
// Type: FoodStorage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

public class FoodStorage : KMonoBehaviour
{
  [Serialize]
  private bool onlyStoreSpicedFood;
  [MyCmpReq]
  public Storage storage;
  private static readonly EventSystem.IntraObjectHandler<FoodStorage> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<FoodStorage>((Action<FoodStorage, object>) ((component, data) => component.OnCopySettings(data)));

  public FilteredStorage FilteredStorage { get; set; }

  public bool SpicedFoodOnly
  {
    get => this.onlyStoreSpicedFood;
    set
    {
      this.onlyStoreSpicedFood = value;
      this.Trigger(1163645216, (object) this.onlyStoreSpicedFood);
      if (this.onlyStoreSpicedFood)
      {
        this.FilteredStorage.AddForbiddenTag(GameTags.UnspicedFood);
        this.storage.DropUnlessHasTag(GameTags.SpicedFood);
      }
      else
        this.FilteredStorage.RemoveForbiddenTag(GameTags.UnspicedFood);
    }
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<FoodStorage>(-905833192, FoodStorage.OnCopySettingsDelegate);
  }

  protected virtual void OnSpawn() => base.OnSpawn();

  private void OnCopySettings(object data)
  {
    FoodStorage component = ((GameObject) data).GetComponent<FoodStorage>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.SpicedFoodOnly = component.SpicedFoodOnly;
  }
}
