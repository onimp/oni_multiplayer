// Decompiled with JetBrains decompiler
// Type: DietManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DietManager")]
public class DietManager : KMonoBehaviour
{
  private Dictionary<Tag, Diet> diets;
  public static DietManager Instance;

  public static void DestroyInstance() => DietManager.Instance = (DietManager) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.diets = DietManager.CollectDiets((Tag[]) null);
    DietManager.Instance = this;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    foreach (Tag tag in DiscoveredResources.Instance.GetDiscovered())
      this.Discover(tag);
    foreach (KeyValuePair<Tag, Diet> diet in this.diets)
    {
      foreach (Diet.Info info in diet.Value.infos)
      {
        foreach (Tag consumedTag in info.consumedTags)
        {
          if (Object.op_Equality((Object) Assets.GetPrefab(consumedTag), (Object) null))
            Debug.LogError((object) string.Format("Could not find prefab {0}, required by diet for {1}", (object) consumedTag, (object) diet.Key));
        }
      }
    }
    DiscoveredResources.Instance.OnDiscover += new Action<Tag, Tag>(this.OnWorldInventoryDiscover);
  }

  private void Discover(Tag tag)
  {
    foreach (KeyValuePair<Tag, Diet> diet in this.diets)
    {
      if (diet.Value.GetDietInfo(tag) != null)
        DiscoveredResources.Instance.Discover(tag, diet.Key);
    }
  }

  private void OnWorldInventoryDiscover(Tag category_tag, Tag tag) => this.Discover(tag);

  public static Dictionary<Tag, Diet> CollectDiets(Tag[] target_species)
  {
    Dictionary<Tag, Diet> dictionary = new Dictionary<Tag, Diet>();
    foreach (KPrefabID prefab in Assets.Prefabs)
    {
      CreatureCalorieMonitor.Def def1 = ((Component) prefab).GetDef<CreatureCalorieMonitor.Def>();
      BeehiveCalorieMonitor.Def def2 = ((Component) prefab).GetDef<BeehiveCalorieMonitor.Def>();
      Diet diet = (Diet) null;
      if (def1 != null)
        diet = def1.diet;
      else if (def2 != null)
        diet = def2.diet;
      if (diet != null && (target_species == null || Array.IndexOf<Tag>(target_species, ((Component) prefab).GetComponent<CreatureBrain>().species) >= 0))
        dictionary[prefab.PrefabTag] = diet;
    }
    return dictionary;
  }
}
