// Decompiled with JetBrains decompiler
// Type: LegacyModMain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class LegacyModMain
{
  public static void Load()
  {
    List<System.Type> types1 = new List<System.Type>();
    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
    {
      System.Type[] types2 = assembly.GetTypes();
      if (types2 != null)
        types1.AddRange((IEnumerable<System.Type>) types2);
    }
    EntityTemplates.CreateTemplates();
    EntityTemplates.CreateBaseOreTemplates();
    LegacyModMain.LoadOre(types1);
    LegacyModMain.LoadBuildings(types1);
    LegacyModMain.ConfigElements();
    LegacyModMain.LoadEntities(types1);
    LegacyModMain.LoadEquipment(types1);
    EntityTemplates.DestroyBaseOreTemplates();
  }

  private static void Test()
  {
    Dictionary<System.Type, int> dictionary = new Dictionary<System.Type, int>();
    foreach (Component component in Resources.FindObjectsOfTypeAll(typeof (Component)))
    {
      System.Type type = ((object) component).GetType();
      int num = 0;
      dictionary.TryGetValue(type, out num);
      dictionary[type] = num + 1;
    }
    List<LegacyModMain.Entry> entryList = new List<LegacyModMain.Entry>();
    foreach (KeyValuePair<System.Type, int> keyValuePair in dictionary)
    {
      if (keyValuePair.Key.GetMethod("Update", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy) != (MethodInfo) null)
        entryList.Add(new LegacyModMain.Entry()
        {
          count = keyValuePair.Value,
          type = keyValuePair.Key
        });
    }
    entryList.Sort((Comparison<LegacyModMain.Entry>) ((x, y) => y.count.CompareTo(x.count)));
    string str = "";
    foreach (LegacyModMain.Entry entry in entryList)
      str = str + entry.type.Name + ": " + entry.count.ToString() + "\n";
    Debug.Log((object) str);
  }

  private static void ListUnusedTypes()
  {
    HashSet<System.Type> typeSet = new HashSet<System.Type>();
    foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll(typeof (GameObject)))
    {
      foreach (Component component in gameObject.GetComponents<Component>())
      {
        if (!Object.op_Equality((Object) component, (Object) null))
        {
          for (System.Type type = ((object) component).GetType(); type != typeof (Component); type = type.BaseType)
            typeSet.Add(type);
        }
      }
    }
    HashSet<System.Type> collection = new HashSet<System.Type>();
    foreach (System.Type currentDomainType in App.GetCurrentDomainTypes())
    {
      if (typeof (MonoBehaviour).IsAssignableFrom(currentDomainType) && !typeSet.Contains(currentDomainType))
        collection.Add(currentDomainType);
    }
    List<System.Type> typeList = new List<System.Type>((IEnumerable<System.Type>) collection);
    typeList.Sort((Comparison<System.Type>) ((x, y) => x.FullName.CompareTo(y.FullName)));
    string str = "Unused types:";
    foreach (System.Type type in typeList)
      str = str + "\n" + type.FullName;
    Debug.Log((object) str);
  }

  private static void DebugSelected()
  {
  }

  private static void DebugSelected(GameObject go)
  {
    Constructable component = go.GetComponent<Constructable>();
    int num = 0 + 1;
    Debug.Log((object) component);
  }

  private static void LoadOre(List<System.Type> types) => GeneratedOre.LoadGeneratedOre(types);

  private static void LoadBuildings(List<System.Type> types)
  {
    LocString.CreateLocStringKeys(typeof (BUILDINGS.PREFABS), "STRINGS.BUILDINGS.");
    LocString.CreateLocStringKeys(typeof (BUILDINGS.DAMAGESOURCES), "STRINGS.BUILDINGS.DAMAGESOURCES");
    LocString.CreateLocStringKeys(typeof (BUILDINGS.REPAIRABLE), "STRINGS.BUILDINGS.REPAIRABLE");
    LocString.CreateLocStringKeys(typeof (BUILDINGS.DISINFECTABLE), "STRINGS.BUILDINGS.DISINFECTABLE");
    GeneratedBuildings.LoadGeneratedBuildings(types);
  }

  private static void LoadEntities(List<System.Type> types)
  {
    EntityConfigManager.Instance.LoadGeneratedEntities(types);
    BuildingConfigManager.Instance.ConfigurePost();
  }

  private static void LoadEquipment(List<System.Type> types)
  {
    LocString.CreateLocStringKeys(typeof (EQUIPMENT.PREFABS), "STRINGS.EQUIPMENT.");
    GeneratedEquipment.LoadGeneratedEquipment(types);
  }

  private static void ConfigElements()
  {
    LegacyModMain.ElementInfo[] elementInfoArray = new LegacyModMain.ElementInfo[20]
    {
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Katairite,
        overheatMod = 200f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Cuprite,
        decor = 0.1f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Copper,
        decor = 0.2f,
        overheatMod = 50f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Gold,
        decor = 0.5f,
        overheatMod = 50f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Lead,
        overheatMod = -20f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Granite,
        decor = 0.2f,
        overheatMod = 15f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.SandStone,
        decor = 0.1f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.ToxicSand,
        overheatMod = -10f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Dirt,
        overheatMod = -10f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.IgneousRock,
        overheatMod = 15f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Obsidian,
        overheatMod = 15f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Ceramic,
        overheatMod = 200f,
        decor = 0.2f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.RefinedCarbon,
        overheatMod = 900f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Iron,
        overheatMod = 50f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Tungsten,
        overheatMod = 50f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Steel,
        overheatMod = 200f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.GoldAmalgam,
        overheatMod = 50f,
        decor = 0.1f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Diamond,
        overheatMod = 200f,
        decor = 1f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.Niobium,
        decor = 0.5f,
        overheatMod = 500f
      },
      new LegacyModMain.ElementInfo()
      {
        id = SimHashes.TempConductorSolid,
        overheatMod = 900f
      }
    };
    foreach (LegacyModMain.ElementInfo elementInfo in elementInfoArray)
    {
      Element elementByHash = ElementLoader.FindElementByHash(elementInfo.id);
      if ((double) elementInfo.decor != 0.0)
      {
        AttributeModifier attributeModifier = new AttributeModifier("Decor", elementInfo.decor, elementByHash.name, true);
        elementByHash.attributeModifiers.Add(attributeModifier);
      }
      if ((double) elementInfo.overheatMod != 0.0)
      {
        AttributeModifier attributeModifier = new AttributeModifier(Db.Get().BuildingAttributes.OverheatTemperature.Id, elementInfo.overheatMod, elementByHash.name);
        elementByHash.attributeModifiers.Add(attributeModifier);
      }
    }
  }

  private struct Entry
  {
    public int count;
    public System.Type type;
  }

  private struct ElementInfo
  {
    public SimHashes id;
    public float decor;
    public float overheatMod;
  }
}
