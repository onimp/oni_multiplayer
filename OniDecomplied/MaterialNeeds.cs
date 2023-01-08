// Decompiled with JetBrains decompiler
// Type: MaterialNeeds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MaterialNeeds")]
public static class MaterialNeeds
{
  public static void UpdateNeed(Tag tag, float amount, int worldId)
  {
    WorldContainer world = ClusterManager.Instance.GetWorld(worldId);
    if (Object.op_Inequality((Object) world, (Object) null))
    {
      Dictionary<Tag, float> materialNeeds = world.materialNeeds;
      float num = 0.0f;
      if (!materialNeeds.TryGetValue(tag, out num))
        materialNeeds[tag] = 0.0f;
      materialNeeds[tag] = num + amount;
    }
    else
      Debug.LogWarning((object) string.Format("MaterialNeeds.UpdateNeed called with invalid worldId {0}", (object) worldId));
  }

  public static float GetAmount(Tag tag, int worldId, bool includeRelatedWorlds)
  {
    WorldContainer world = ClusterManager.Instance.GetWorld(worldId);
    float amount = 0.0f;
    if (Object.op_Inequality((Object) world, (Object) null))
    {
      if (!includeRelatedWorlds)
      {
        float num = 0.0f;
        ClusterManager.Instance.GetWorld(worldId).materialNeeds.TryGetValue(tag, out num);
        amount += num;
      }
      else
      {
        int parentWorldId = world.ParentWorldId;
        foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
        {
          if (worldContainer.ParentWorldId == parentWorldId)
          {
            float num = 0.0f;
            if (worldContainer.materialNeeds.TryGetValue(tag, out num))
              amount += num;
          }
        }
      }
      return amount;
    }
    Debug.LogWarning((object) string.Format("MaterialNeeds.GetAmount called with invalid worldId {0}", (object) worldId));
    return 0.0f;
  }
}
