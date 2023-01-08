// Decompiled with JetBrains decompiler
// Type: RationTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/RationTracker")]
public class RationTracker : KMonoBehaviour, ISaveLoadable
{
  private static RationTracker instance;
  [Serialize]
  public RationTracker.Frame currentFrame;
  [Serialize]
  public RationTracker.Frame previousFrame;
  [Serialize]
  public Dictionary<string, float> caloriesConsumedByFood = new Dictionary<string, float>();
  private static readonly EventSystem.IntraObjectHandler<RationTracker> OnNewDayDelegate = new EventSystem.IntraObjectHandler<RationTracker>((Action<RationTracker, object>) ((component, data) => component.OnNewDay(data)));

  public static void DestroyInstance() => RationTracker.instance = (RationTracker) null;

  public static RationTracker Get() => RationTracker.instance;

  protected virtual void OnPrefabInit() => RationTracker.instance = this;

  protected virtual void OnSpawn() => this.Subscribe<RationTracker>(631075836, RationTracker.OnNewDayDelegate);

  private void OnNewDay(object data)
  {
    this.previousFrame = this.currentFrame;
    this.currentFrame = new RationTracker.Frame();
  }

  public float CountRations(
    Dictionary<string, float> unitCountByFoodType,
    WorldInventory inventory,
    bool excludeUnreachable = true)
  {
    float num = 0.0f;
    ICollection<Pickupable> pickupables = inventory.GetPickupables(GameTags.Edible);
    if (pickupables != null)
    {
      foreach (Pickupable pickupable in (IEnumerable<Pickupable>) pickupables)
      {
        if (!pickupable.KPrefabID.HasTag(GameTags.StoredPrivate))
        {
          Edible component = ((Component) pickupable).GetComponent<Edible>();
          num += component.Calories;
          if (unitCountByFoodType != null)
          {
            if (!unitCountByFoodType.ContainsKey(component.FoodID))
              unitCountByFoodType[component.FoodID] = 0.0f;
            unitCountByFoodType[component.FoodID] += component.Units;
          }
        }
      }
    }
    return num;
  }

  public float CountRationsByFoodType(
    string foodID,
    WorldInventory inventory,
    bool excludeUnreachable = true)
  {
    float num = 0.0f;
    ICollection<Pickupable> pickupables = inventory.GetPickupables(GameTags.Edible);
    if (pickupables != null)
    {
      foreach (Pickupable pickupable in (IEnumerable<Pickupable>) pickupables)
      {
        if (!pickupable.KPrefabID.HasTag(GameTags.StoredPrivate))
        {
          Edible component = ((Component) pickupable).GetComponent<Edible>();
          if (component.FoodID == foodID)
            num += component.Calories;
        }
      }
    }
    return num;
  }

  public void RegisterCaloriesProduced(float calories) => this.currentFrame.caloriesProduced += calories;

  public void RegisterRationsConsumed(Edible edible)
  {
    this.currentFrame.caloriesConsumed += edible.caloriesConsumed;
    if (!this.caloriesConsumedByFood.ContainsKey(edible.FoodInfo.Id))
      this.caloriesConsumedByFood.Add(edible.FoodInfo.Id, edible.caloriesConsumed);
    else
      this.caloriesConsumedByFood[edible.FoodInfo.Id] += edible.caloriesConsumed;
  }

  public float GetCaloiresConsumedByFood(List<string> foodTypes)
  {
    float caloiresConsumedByFood = 0.0f;
    foreach (string foodType in foodTypes)
    {
      if (this.caloriesConsumedByFood.ContainsKey(foodType))
        caloiresConsumedByFood += this.caloriesConsumedByFood[foodType];
    }
    return caloiresConsumedByFood;
  }

  public float GetCaloriesConsumed()
  {
    float caloriesConsumed = 0.0f;
    foreach (KeyValuePair<string, float> keyValuePair in this.caloriesConsumedByFood)
      caloriesConsumed += keyValuePair.Value;
    return caloriesConsumed;
  }

  public struct Frame
  {
    public float caloriesProduced;
    public float caloriesConsumed;
  }
}
