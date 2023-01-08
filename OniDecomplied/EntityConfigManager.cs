// Decompiled with JetBrains decompiler
// Type: EntityConfigManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EntityConfigManager")]
public class EntityConfigManager : KMonoBehaviour
{
  public static EntityConfigManager Instance;

  public static void DestroyInstance() => EntityConfigManager.Instance = (EntityConfigManager) null;

  protected virtual void OnPrefabInit() => EntityConfigManager.Instance = this;

  private static int GetSortOrder(System.Type type)
  {
    foreach (Attribute customAttribute in type.GetCustomAttributes(true))
    {
      if (customAttribute.GetType() == typeof (EntityConfigOrder))
        return (customAttribute as EntityConfigOrder).sortOrder;
    }
    return 0;
  }

  public void LoadGeneratedEntities(List<System.Type> types)
  {
    System.Type type1 = typeof (IEntityConfig);
    System.Type type2 = typeof (IMultiEntityConfig);
    List<EntityConfigManager.ConfigEntry> configEntryList = new List<EntityConfigManager.ConfigEntry>();
    foreach (System.Type type3 in types)
    {
      if ((type1.IsAssignableFrom(type3) || type2.IsAssignableFrom(type3)) && !type3.IsAbstract && !type3.IsInterface)
      {
        int sortOrder = EntityConfigManager.GetSortOrder(type3);
        EntityConfigManager.ConfigEntry configEntry = new EntityConfigManager.ConfigEntry()
        {
          type = type3,
          sortOrder = sortOrder
        };
        configEntryList.Add(configEntry);
      }
    }
    configEntryList.Sort((Comparison<EntityConfigManager.ConfigEntry>) ((x, y) => x.sortOrder.CompareTo(y.sortOrder)));
    foreach (EntityConfigManager.ConfigEntry configEntry in configEntryList)
    {
      object instance = Activator.CreateInstance(configEntry.type);
      if (instance is IEntityConfig && DlcManager.IsDlcListValidForCurrentContent((instance as IEntityConfig).GetDlcIds()))
        this.RegisterEntity(instance as IEntityConfig);
      if (instance is IMultiEntityConfig)
        this.RegisterEntities(instance as IMultiEntityConfig);
    }
  }

  public void RegisterEntity(IEntityConfig config)
  {
    KPrefabID component = config.CreatePrefab().GetComponent<KPrefabID>();
    IEntityConfig entityConfig1 = config;
    // ISSUE: virtual method pointer
    component.prefabInitFn += new KPrefabID.PrefabFn((object) entityConfig1, __vmethodptr(entityConfig1, OnPrefabInit));
    IEntityConfig entityConfig2 = config;
    // ISSUE: virtual method pointer
    component.prefabSpawnFn += new KPrefabID.PrefabFn((object) entityConfig2, __vmethodptr(entityConfig2, OnSpawn));
    Assets.AddPrefab(component);
  }

  public void RegisterEntities(IMultiEntityConfig config)
  {
    foreach (GameObject prefab in config.CreatePrefabs())
    {
      KPrefabID component = prefab.GetComponent<KPrefabID>();
      IMultiEntityConfig multiEntityConfig1 = config;
      // ISSUE: virtual method pointer
      component.prefabInitFn += new KPrefabID.PrefabFn((object) multiEntityConfig1, __vmethodptr(multiEntityConfig1, OnPrefabInit));
      IMultiEntityConfig multiEntityConfig2 = config;
      // ISSUE: virtual method pointer
      component.prefabSpawnFn += new KPrefabID.PrefabFn((object) multiEntityConfig2, __vmethodptr(multiEntityConfig2, OnSpawn));
      Assets.AddPrefab(component);
    }
  }

  private struct ConfigEntry
  {
    public System.Type type;
    public int sortOrder;
  }
}
