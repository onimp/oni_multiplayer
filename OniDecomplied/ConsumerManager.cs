// Decompiled with JetBrains decompiler
// Type: ConsumerManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/ConsumerManager")]
public class ConsumerManager : KMonoBehaviour, ISaveLoadable
{
  public static ConsumerManager instance;
  [Serialize]
  private List<Tag> undiscoveredConsumableTags = new List<Tag>();
  [Serialize]
  private List<Tag> defaultForbiddenTagsList = new List<Tag>();

  public static void DestroyInstance() => ConsumerManager.instance = (ConsumerManager) null;

  public event Action<Tag> OnDiscover;

  public List<Tag> DefaultForbiddenTagsList => this.defaultForbiddenTagsList;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ConsumerManager.instance = this;
    this.RefreshDiscovered();
    DiscoveredResources.Instance.OnDiscover += new Action<Tag, Tag>(this.OnWorldInventoryDiscover);
    Game.Instance.Subscribe(-107300940, new Action<object>(this.RefreshDiscovered));
  }

  public bool isDiscovered(Tag id) => !this.undiscoveredConsumableTags.Contains(id);

  private void OnWorldInventoryDiscover(Tag category_tag, Tag tag)
  {
    if (!this.undiscoveredConsumableTags.Contains(tag))
      return;
    this.RefreshDiscovered();
  }

  public void RefreshDiscovered(object data = null)
  {
    foreach (EdiblesManager.FoodInfo allFoodType in EdiblesManager.GetAllFoodTypes())
    {
      if (!this.ShouldBeDiscovered(TagExtensions.ToTag(allFoodType.Id)) && !this.undiscoveredConsumableTags.Contains(TagExtensions.ToTag(allFoodType.Id)))
      {
        this.undiscoveredConsumableTags.Add(TagExtensions.ToTag(allFoodType.Id));
        if (this.OnDiscover != null)
          this.OnDiscover(TagExtensions.ToTag("UndiscoveredSomething"));
      }
      else if (this.undiscoveredConsumableTags.Contains(TagExtensions.ToTag(allFoodType.Id)) && this.ShouldBeDiscovered(TagExtensions.ToTag(allFoodType.Id)))
      {
        this.undiscoveredConsumableTags.Remove(TagExtensions.ToTag(allFoodType.Id));
        if (this.OnDiscover != null)
          this.OnDiscover(TagExtensions.ToTag(allFoodType.Id));
        if (!DiscoveredResources.Instance.IsDiscovered(TagExtensions.ToTag(allFoodType.Id)))
        {
          if ((double) allFoodType.CaloriesPerUnit == 0.0)
            DiscoveredResources.Instance.Discover(TagExtensions.ToTag(allFoodType.Id), GameTags.CookingIngredient);
          else
            DiscoveredResources.Instance.Discover(TagExtensions.ToTag(allFoodType.Id), GameTags.Edible);
        }
      }
    }
  }

  private bool ShouldBeDiscovered(Tag food_id)
  {
    if (DiscoveredResources.Instance.IsDiscovered(food_id))
      return true;
    foreach (Recipe recipe in RecipeManager.Get().recipes)
    {
      if (Tag.op_Equality(recipe.Result, food_id))
      {
        foreach (string fabricator in recipe.fabricators)
        {
          if (Db.Get().TechItems.IsTechItemComplete(fabricator))
            return true;
        }
      }
    }
    foreach (Crop crop in Components.Crops.Items)
    {
      if (Grid.IsVisible(Grid.PosToCell(((Component) crop).gameObject)) && crop.cropId == ((Tag) ref food_id).Name)
        return true;
    }
    return false;
  }
}
