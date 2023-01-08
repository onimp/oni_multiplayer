// Decompiled with JetBrains decompiler
// Type: SaveLoadRoot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/SaveLoadRoot")]
public class SaveLoadRoot : KMonoBehaviour
{
  private bool hasOnSpawnRun;
  private bool registered = true;
  [SerializeField]
  private List<string> m_optionalComponentTypeNames = new List<string>();
  private static Dictionary<string, ISerializableComponentManager> serializableComponentManagers;
  private static Dictionary<System.Type, string> sTypeToString = new Dictionary<System.Type, string>();

  public static void DestroyStatics() => SaveLoadRoot.serializableComponentManagers = (Dictionary<string, ISerializableComponentManager>) null;

  protected virtual void OnPrefabInit()
  {
    if (SaveLoadRoot.serializableComponentManagers != null)
      return;
    SaveLoadRoot.serializableComponentManagers = new Dictionary<string, ISerializableComponentManager>();
    foreach (FieldInfo field in typeof (GameComps).GetFields())
    {
      IComponentManager icomponentManager = (IComponentManager) field.GetValue((object) null);
      if (typeof (ISerializableComponentManager).IsAssignableFrom(((object) icomponentManager).GetType()))
      {
        System.Type type = ((object) icomponentManager).GetType();
        SaveLoadRoot.serializableComponentManagers[type.ToString()] = (ISerializableComponentManager) icomponentManager;
      }
    }
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.registered)
      SaveLoader.Instance.saveManager.Register(this);
    this.hasOnSpawnRun = true;
  }

  public void DeclareOptionalComponent<T>() where T : KMonoBehaviour => this.m_optionalComponentTypeNames.Add(typeof (T).ToString());

  public void SetRegistered(bool registered)
  {
    if (this.registered == registered)
      return;
    this.registered = registered;
    if (!this.hasOnSpawnRun)
      return;
    if (registered)
      SaveLoader.Instance.saveManager.Register(this);
    else
      SaveLoader.Instance.saveManager.Unregister(this);
  }

  protected virtual void OnCleanUp()
  {
    if (Object.op_Inequality((Object) SaveLoader.Instance, (Object) null) && Object.op_Inequality((Object) SaveLoader.Instance.saveManager, (Object) null))
      SaveLoader.Instance.saveManager.Unregister(this);
    if (!((KComponentManager<WhiteBoard.Data>) GameComps.WhiteBoards).Has((object) ((Component) this).gameObject))
      return;
    GameComps.WhiteBoards.Remove(((Component) this).gameObject);
  }

  public void Save(BinaryWriter writer)
  {
    Transform transform = this.transform;
    Util.Write(writer, TransformExtensions.GetPosition(transform));
    Util.Write(writer, transform.rotation);
    Util.Write(writer, transform.localScale);
    byte num = 0;
    writer.Write(num);
    this.SaveWithoutTransform(writer);
  }

  public void SaveWithoutTransform(BinaryWriter writer)
  {
    KMonoBehaviour[] components = ((Component) this).GetComponents<KMonoBehaviour>();
    if (components == null)
      return;
    int num1 = 0;
    foreach (KMonoBehaviour kmonoBehaviour in components)
    {
      if ((kmonoBehaviour is ISaveLoadableDetails || kmonoBehaviour != null) && !((object) kmonoBehaviour).GetType().IsDefined(typeof (SkipSaveFileSerialization), false))
        ++num1;
    }
    foreach (KeyValuePair<string, ISerializableComponentManager> componentManager in SaveLoadRoot.serializableComponentManagers)
    {
      if (((IComponentManager) componentManager.Value).Has((object) ((Component) this).gameObject))
        ++num1;
    }
    writer.Write(num1);
    foreach (KMonoBehaviour kmonoBehaviour in components)
    {
      if ((kmonoBehaviour is ISaveLoadableDetails || kmonoBehaviour != null) && !((object) kmonoBehaviour).GetType().IsDefined(typeof (SkipSaveFileSerialization), false))
      {
        IOHelper.WriteKleiString(writer, ((object) kmonoBehaviour).GetType().ToString());
        long position1 = writer.BaseStream.Position;
        writer.Write(0);
        long position2 = writer.BaseStream.Position;
        if (kmonoBehaviour is ISaveLoadableDetails)
        {
          ISaveLoadableDetails isaveLoadableDetails = (ISaveLoadableDetails) kmonoBehaviour;
          Serializer.SerializeTypeless((object) kmonoBehaviour, writer);
          BinaryWriter binaryWriter = writer;
          isaveLoadableDetails.Serialize(binaryWriter);
        }
        else if (kmonoBehaviour != null)
          Serializer.SerializeTypeless((object) kmonoBehaviour, writer);
        long position3 = writer.BaseStream.Position;
        long num2 = position3 - position2;
        writer.BaseStream.Position = position1;
        writer.Write((int) num2);
        writer.BaseStream.Position = position3;
      }
    }
    foreach (KeyValuePair<string, ISerializableComponentManager> componentManager1 in SaveLoadRoot.serializableComponentManagers)
    {
      ISerializableComponentManager componentManager2 = componentManager1.Value;
      if (((IComponentManager) componentManager2).Has((object) ((Component) this).gameObject))
      {
        string key = componentManager1.Key;
        IOHelper.WriteKleiString(writer, key);
        componentManager2.Serialize(((Component) this).gameObject, writer);
      }
    }
  }

  public static SaveLoadRoot Load(Tag tag, IReader reader) => SaveLoadRoot.Load(SaveLoader.Instance.saveManager.GetPrefab(tag), reader);

  public static SaveLoadRoot Load(GameObject prefab, IReader reader)
  {
    Vector3 position = Util.ReadVector3(reader);
    Quaternion rotation = Util.ReadQuaternion(reader);
    Vector3 scale = Util.ReadVector3(reader);
    int num = (int) reader.ReadByte();
    if (SaveManager.DEBUG_OnlyLoadThisCellsObjects > -1)
    {
      Vector3 pos = Grid.CellToPos(SaveManager.DEBUG_OnlyLoadThisCellsObjects);
      if (((double) position.x < (double) pos.x || (double) position.x >= (double) pos.x + 1.0 || (double) position.y < (double) pos.y || (double) position.y >= (double) pos.y + 1.0) && ((Object) prefab).name != "SaveGame")
        prefab = (GameObject) null;
      else
        Debug.Log((object) ("Keeping " + ((Object) prefab).name));
    }
    return SaveLoadRoot.Load(prefab, position, rotation, scale, reader);
  }

  public static SaveLoadRoot Load(
    GameObject prefab,
    Vector3 position,
    Quaternion rotation,
    Vector3 scale,
    IReader reader)
  {
    SaveLoadRoot saveLoadRoot = (SaveLoadRoot) null;
    if (Object.op_Inequality((Object) prefab, (Object) null))
    {
      GameObject gameObject = Util.KInstantiate(prefab, position, rotation, (GameObject) null, (string) null, false, 0);
      gameObject.transform.localScale = scale;
      gameObject.SetActive(true);
      saveLoadRoot = gameObject.GetComponent<SaveLoadRoot>();
      if (Object.op_Inequality((Object) saveLoadRoot, (Object) null))
      {
        try
        {
          SaveLoadRoot.LoadInternal(gameObject, reader);
        }
        catch (ArgumentException ex)
        {
          DebugUtil.LogErrorArgs((Object) gameObject, new object[4]
          {
            (object) "Failed to load SaveLoadRoot ",
            (object) ex.Message,
            (object) "\n",
            (object) ex.StackTrace
          });
        }
      }
      else
        Debug.Log((object) "missing SaveLoadRoot", (Object) gameObject);
    }
    else
      SaveLoadRoot.LoadInternal((GameObject) null, reader);
    return saveLoadRoot;
  }

  private static void LoadInternal(GameObject gameObject, IReader reader)
  {
    Dictionary<string, int> dictionary = new Dictionary<string, int>();
    KMonoBehaviour[] kmonoBehaviourArray = Object.op_Inequality((Object) gameObject, (Object) null) ? gameObject.GetComponents<KMonoBehaviour>() : (KMonoBehaviour[]) null;
    int num1 = reader.ReadInt32();
    for (int index1 = 0; index1 < num1; ++index1)
    {
      string key = reader.ReadKleiString();
      int num2 = reader.ReadInt32();
      int position = reader.Position;
      ISerializableComponentManager componentManager;
      if (SaveLoadRoot.serializableComponentManagers.TryGetValue(key, out componentManager))
      {
        componentManager.Deserialize(gameObject, reader);
      }
      else
      {
        int num3 = 0;
        dictionary.TryGetValue(key, out num3);
        KMonoBehaviour kmonoBehaviour = (KMonoBehaviour) null;
        int num4 = 0;
        if (kmonoBehaviourArray != null)
        {
          for (int index2 = 0; index2 < kmonoBehaviourArray.Length; ++index2)
          {
            System.Type type = ((object) kmonoBehaviourArray[index2]).GetType();
            string str;
            if (!SaveLoadRoot.sTypeToString.TryGetValue(type, out str))
            {
              str = type.ToString();
              SaveLoadRoot.sTypeToString[type] = str;
            }
            if (str == key)
            {
              if (num4 == num3)
              {
                kmonoBehaviour = kmonoBehaviourArray[index2];
                break;
              }
              ++num4;
            }
          }
        }
        if (Object.op_Equality((Object) kmonoBehaviour, (Object) null) && Object.op_Inequality((Object) gameObject, (Object) null))
        {
          SaveLoadRoot component = gameObject.GetComponent<SaveLoadRoot>();
          int index3;
          if (Object.op_Inequality((Object) component, (Object) null) && (index3 = component.m_optionalComponentTypeNames.IndexOf(key)) != -1)
          {
            DebugUtil.DevAssert(num3 == 0 && num4 == 0, string.Format("Implementation does not support multiple components with optional components, type {0}, {1}, {2}. Using only the first one and skipping the rest.", (object) key, (object) num3, (object) num4), (Object) null);
            System.Type type = System.Type.GetType(component.m_optionalComponentTypeNames[index3]);
            if (num4 == 0)
              kmonoBehaviour = (KMonoBehaviour) gameObject.AddComponent(type);
          }
        }
        if (Object.op_Equality((Object) kmonoBehaviour, (Object) null))
          reader.SkipBytes(num2);
        else if (kmonoBehaviour == null && !(kmonoBehaviour is ISaveLoadableDetails))
        {
          DebugUtil.LogErrorArgs(new object[3]
          {
            (object) "Component",
            (object) key,
            (object) "is not ISaveLoadable"
          });
          reader.SkipBytes(num2);
        }
        else
        {
          dictionary[key] = num4 + 1;
          if (kmonoBehaviour is ISaveLoadableDetails)
          {
            ISaveLoadableDetails isaveLoadableDetails = (ISaveLoadableDetails) kmonoBehaviour;
            Deserializer.DeserializeTypeless((object) kmonoBehaviour, reader);
            IReader ireader = reader;
            isaveLoadableDetails.Deserialize(ireader);
          }
          else
            Deserializer.DeserializeTypeless((object) kmonoBehaviour, reader);
          if (reader.Position != position + num2)
          {
            DebugUtil.LogWarningArgs(new object[5]
            {
              (object) "Expected to be at offset",
              (object) (position + num2),
              (object) "but was only at offset",
              (object) reader.Position,
              (object) ". Skipping to catch up."
            });
            reader.SkipBytes(position + num2 - reader.Position);
          }
        }
      }
    }
  }
}
