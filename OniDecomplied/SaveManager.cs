// Decompiled with JetBrains decompiler
// Type: SaveManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SaveManager")]
public class SaveManager : KMonoBehaviour
{
  public const int SAVE_MAJOR_VERSION_LAST_UNDOCUMENTED = 7;
  public const int SAVE_MAJOR_VERSION = 7;
  public const int SAVE_MINOR_VERSION_EXPLICIT_VALUE_TYPES = 4;
  public const int SAVE_MINOR_VERSION_LAST_UNDOCUMENTED = 7;
  public const int SAVE_MINOR_VERSION_MOD_IDENTIFIER = 8;
  public const int SAVE_MINOR_VERSION_FINITE_SPACE_RESOURCES = 9;
  public const int SAVE_MINOR_VERSION_COLONY_REQ_ACHIEVEMENTS = 10;
  public const int SAVE_MINOR_VERSION_TRACK_NAV_DISTANCE = 11;
  public const int SAVE_MINOR_VERSION_EXPANDED_WORLD_INFO = 12;
  public const int SAVE_MINOR_VERSION_BASIC_COMFORTS_FIX = 13;
  public const int SAVE_MINOR_VERSION_PLATFORM_TRAIT_NAMES = 14;
  public const int SAVE_MINOR_VERSION_ADD_JOY_REACTIONS = 15;
  public const int SAVE_MINOR_VERSION_NEW_AUTOMATION_WARNING = 16;
  public const int SAVE_MINOR_VERSION_ADD_GUID_TO_HEADER = 17;
  public const int SAVE_MINOR_VERSION_EXPANSION_1_INTRODUCED = 20;
  public const int SAVE_MINOR_VERSION_CONTENT_SETTINGS = 21;
  public const int SAVE_MINOR_VERSION_COLONY_REQ_REMOVE_SERIALIZATION = 22;
  public const int SAVE_MINOR_VERSION_ROTTABLE_TUNING = 23;
  public const int SAVE_MINOR_VERSION_LAUNCH_PAD_SOLIDITY = 24;
  public const int SAVE_MINOR_VERSION_BASE_GAME_MERGEDOWN = 25;
  public const int SAVE_MINOR_VERSION_FALLING_WATER_WORLDIDX_SERIALIZATION = 26;
  public const int SAVE_MINOR_VERSION_ROCKET_RANGE_REBALANCE = 27;
  public const int SAVE_MINOR_VERSION_ENTITIES_WRONG_LAYER = 28;
  public const int SAVE_MINOR_VERSION_TAGBITS_REWORK = 29;
  public const int SAVE_MINOR_VERSION_ACCESSORY_SLOT_UPGRADE = 30;
  public const int SAVE_MINOR_VERSION_GEYSER_CAN_BE_RENAMED = 31;
  public const int SAVE_MINOR_VERSION = 31;
  private Dictionary<Tag, GameObject> prefabMap = new Dictionary<Tag, GameObject>();
  private Dictionary<Tag, List<SaveLoadRoot>> sceneObjects = new Dictionary<Tag, List<SaveLoadRoot>>();
  public static int DEBUG_OnlyLoadThisCellsObjects = -1;
  private static readonly char[] SAVE_HEADER = new char[4]
  {
    'K',
    'S',
    'A',
    'V'
  };
  private List<Tag> orderedKeys = new List<Tag>();

  public event Action<SaveLoadRoot> onRegister;

  public event Action<SaveLoadRoot> onUnregister;

  protected virtual void OnPrefabInit() => Assets.RegisterOnAddPrefab(new Action<KPrefabID>(this.OnAddPrefab));

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Assets.UnregisterOnAddPrefab(new Action<KPrefabID>(this.OnAddPrefab));
  }

  private void OnAddPrefab(KPrefabID prefab)
  {
    if (Object.op_Equality((Object) prefab, (Object) null))
      return;
    this.prefabMap[prefab.GetSaveLoadTag()] = ((Component) prefab).gameObject;
  }

  public Dictionary<Tag, List<SaveLoadRoot>> GetLists() => this.sceneObjects;

  private List<SaveLoadRoot> GetSaveLoadRootList(SaveLoadRoot saver)
  {
    KPrefabID component = ((Component) saver).GetComponent<KPrefabID>();
    if (Object.op_Equality((Object) component, (Object) null))
    {
      DebugUtil.LogErrorArgs((Object) ((Component) saver).gameObject, new object[3]
      {
        (object) "All savers must also have a KPrefabID on them but",
        (object) ((Object) ((Component) saver).gameObject).name,
        (object) "does not have one."
      });
      return (List<SaveLoadRoot>) null;
    }
    List<SaveLoadRoot> saveLoadRootList;
    if (!this.sceneObjects.TryGetValue(component.GetSaveLoadTag(), out saveLoadRootList))
    {
      saveLoadRootList = new List<SaveLoadRoot>();
      this.sceneObjects[component.GetSaveLoadTag()] = saveLoadRootList;
    }
    return saveLoadRootList;
  }

  public void Register(SaveLoadRoot root)
  {
    List<SaveLoadRoot> saveLoadRootList = this.GetSaveLoadRootList(root);
    if (saveLoadRootList == null)
      return;
    saveLoadRootList.Add(root);
    if (this.onRegister == null)
      return;
    this.onRegister(root);
  }

  public void Unregister(SaveLoadRoot root)
  {
    if (this.onRegister != null)
      this.onUnregister(root);
    this.GetSaveLoadRootList(root)?.Remove(root);
  }

  public GameObject GetPrefab(Tag tag)
  {
    GameObject prefab = (GameObject) null;
    if (this.prefabMap.TryGetValue(tag, out prefab))
      return prefab;
    DebugUtil.LogArgs(new object[2]
    {
      (object) "Item not found in prefabMap",
      (object) ("[" + ((Tag) ref tag).Name + "]")
    });
    return (GameObject) null;
  }

  public void Save(BinaryWriter writer)
  {
    writer.Write(SaveManager.SAVE_HEADER);
    writer.Write(7);
    writer.Write(31);
    int num = 0;
    foreach (KeyValuePair<Tag, List<SaveLoadRoot>> sceneObject in this.sceneObjects)
    {
      if (sceneObject.Value.Count > 0)
        ++num;
    }
    writer.Write(num);
    this.orderedKeys.Clear();
    this.orderedKeys.AddRange((IEnumerable<Tag>) this.sceneObjects.Keys);
    this.orderedKeys.Remove(((Component) SaveGame.Instance).PrefabID());
    this.orderedKeys = ((IEnumerable<Tag>) ((IEnumerable<Tag>) this.orderedKeys).OrderBy<Tag, bool>((Func<Tag, bool>) (a => ((Tag) ref a).Name == "StickerBomb"))).ToList<Tag>();
    this.orderedKeys = ((IEnumerable<Tag>) ((IEnumerable<Tag>) this.orderedKeys).OrderBy<Tag, bool>((Func<Tag, bool>) (a => ((Tag) ref a).Name.Contains("UnderConstruction")))).ToList<Tag>();
    this.Write(((Component) SaveGame.Instance).PrefabID(), new List<SaveLoadRoot>((IEnumerable<SaveLoadRoot>) new SaveLoadRoot[1]
    {
      ((Component) SaveGame.Instance).GetComponent<SaveLoadRoot>()
    }), writer);
    foreach (Tag orderedKey in this.orderedKeys)
    {
      List<SaveLoadRoot> sceneObject = this.sceneObjects[orderedKey];
      if (sceneObject.Count > 0)
      {
        foreach (SaveLoadRoot saveLoadRoot in sceneObject)
        {
          if (!Object.op_Equality((Object) saveLoadRoot, (Object) null) && Object.op_Inequality((Object) ((Component) saveLoadRoot).GetComponent<SimCellOccupier>(), (Object) null))
          {
            this.Write(orderedKey, sceneObject, writer);
            break;
          }
        }
      }
    }
    foreach (Tag orderedKey in this.orderedKeys)
    {
      List<SaveLoadRoot> sceneObject = this.sceneObjects[orderedKey];
      if (sceneObject.Count > 0)
      {
        foreach (SaveLoadRoot saveLoadRoot in sceneObject)
        {
          if (!Object.op_Equality((Object) saveLoadRoot, (Object) null) && Object.op_Equality((Object) ((Component) saveLoadRoot).GetComponent<SimCellOccupier>(), (Object) null))
          {
            this.Write(orderedKey, sceneObject, writer);
            break;
          }
        }
      }
    }
  }

  private void Write(Tag key, List<SaveLoadRoot> value, BinaryWriter writer)
  {
    int count = value.Count;
    Tag tag = key;
    IOHelper.WriteKleiString(writer, ((Tag) ref tag).Name);
    writer.Write(count);
    long position1 = writer.BaseStream.Position;
    int num1 = -1;
    writer.Write(num1);
    long position2 = writer.BaseStream.Position;
    foreach (SaveLoadRoot saveLoadRoot in value)
    {
      if (Object.op_Inequality((Object) saveLoadRoot, (Object) null))
        saveLoadRoot.Save(writer);
      else
        DebugUtil.LogWarningArgs(new object[1]
        {
          (object) "Null game object when saving"
        });
    }
    long position3 = writer.BaseStream.Position;
    long num2 = position3 - position2;
    writer.BaseStream.Position = position1;
    writer.Write((int) num2);
    writer.BaseStream.Position = position3;
  }

  public bool Load(IReader reader)
  {
    char[] chArray = reader.ReadChars(SaveManager.SAVE_HEADER.Length);
    if (chArray == null || chArray.Length != SaveManager.SAVE_HEADER.Length)
      return false;
    for (int index = 0; index < SaveManager.SAVE_HEADER.Length; ++index)
    {
      if ((int) chArray[index] != (int) SaveManager.SAVE_HEADER[index])
        return false;
    }
    int num1 = reader.ReadInt32();
    int num2 = reader.ReadInt32();
    if (num1 != 7 || num2 > 31)
    {
      DebugUtil.LogWarningArgs(new object[1]
      {
        (object) string.Format("SAVE FILE VERSION MISMATCH! Expected {0}.{1} but got {2}.{3}", (object) 7, (object) 31, (object) num1, (object) num2)
      });
      return false;
    }
    this.ClearScene();
    try
    {
      int num3 = reader.ReadInt32();
      for (int index1 = 0; index1 < num3; ++index1)
      {
        string str = reader.ReadKleiString();
        int capacity = reader.ReadInt32();
        int num4 = reader.ReadInt32();
        Tag key = TagManager.Create(str);
        GameObject prefab;
        if (!this.prefabMap.TryGetValue(key, out prefab))
        {
          DebugUtil.LogWarningArgs(new object[1]
          {
            (object) ("Could not find prefab '" + str + "'")
          });
          reader.SkipBytes(num4);
        }
        else
        {
          List<SaveLoadRoot> saveLoadRootList = new List<SaveLoadRoot>(capacity);
          this.sceneObjects[key] = saveLoadRootList;
          for (int index2 = 0; index2 < capacity; ++index2)
          {
            SaveLoadRoot saveLoadRoot = SaveLoadRoot.Load(prefab, reader);
            if (SaveManager.DEBUG_OnlyLoadThisCellsObjects == -1 && Object.op_Equality((Object) saveLoadRoot, (Object) null))
            {
              Debug.LogError((object) ("Error loading data [" + str + "]"));
              return false;
            }
          }
        }
      }
    }
    catch (Exception ex)
    {
      DebugUtil.LogErrorArgs(new object[2]
      {
        (object) "Error deserializing prefabs\n\n",
        (object) ex.ToString()
      });
      throw ex;
    }
    return true;
  }

  private void ClearScene()
  {
    foreach (KeyValuePair<Tag, List<SaveLoadRoot>> sceneObject in this.sceneObjects)
    {
      foreach (Component component in sceneObject.Value)
        Object.Destroy((Object) component.gameObject);
    }
    this.sceneObjects.Clear();
  }

  private enum BoundaryTag : uint
  {
    Prefab = 3131961357, // 0xBAADF00D
    Component = 3735928559, // 0xDEADBEEF
    Complete = 3735929054, // 0xDEADC0DE
  }
}
