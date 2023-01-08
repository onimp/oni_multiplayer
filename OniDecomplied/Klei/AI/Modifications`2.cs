// Decompiled with JetBrains decompiler
// Type: Klei.AI.Modifications`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Klei.AI
{
  [SerializationConfig]
  public class Modifications<ModifierType, InstanceType> : ISaveLoadableDetails
    where ModifierType : Resource
    where InstanceType : ModifierInstance<ModifierType>
  {
    public List<InstanceType> ModifierList = new List<InstanceType>();
    private ResourceSet<ModifierType> resources;

    public int Count => this.ModifierList.Count;

    public IEnumerator<InstanceType> GetEnumerator() => (IEnumerator<InstanceType>) this.ModifierList.GetEnumerator();

    public GameObject gameObject { get; private set; }

    public InstanceType this[int idx] => this.ModifierList[idx];

    public ComponentType GetComponent<ComponentType>() => this.gameObject.GetComponent<ComponentType>();

    public void Trigger(GameHashes hash, object data = null) => ((KMonoBehaviour) this.gameObject.GetComponent<KPrefabID>()).Trigger((int) hash, data);

    public virtual InstanceType CreateInstance(ModifierType modifier) => default (InstanceType);

    public Modifications(GameObject go, ResourceSet<ModifierType> resources = null)
    {
      this.resources = resources;
      this.gameObject = go;
    }

    public virtual InstanceType Add(InstanceType instance)
    {
      this.ModifierList.Add(instance);
      return instance;
    }

    public virtual void Remove(InstanceType instance)
    {
      for (int index = 0; index < this.ModifierList.Count; ++index)
      {
        if ((object) this.ModifierList[index] == (object) instance)
        {
          this.ModifierList.RemoveAt(index);
          instance.OnCleanUp();
          break;
        }
      }
    }

    public bool Has(ModifierType modifier) => (object) this.Get(modifier) != null;

    public InstanceType Get(ModifierType modifier)
    {
      foreach (InstanceType modifier1 in this.ModifierList)
      {
        if ((object) modifier1.modifier == (object) modifier)
          return modifier1;
      }
      return default (InstanceType);
    }

    public InstanceType Get(string id)
    {
      foreach (InstanceType modifier in this.ModifierList)
      {
        if (((Resource) (object) modifier.modifier).Id == id)
          return modifier;
      }
      return default (InstanceType);
    }

    public void Serialize(BinaryWriter writer)
    {
      writer.Write(this.ModifierList.Count);
      foreach (InstanceType modifier in this.ModifierList)
      {
        IOHelper.WriteKleiString(writer, ((Resource) (object) modifier.modifier).Id);
        long position1 = writer.BaseStream.Position;
        writer.Write(0);
        long position2 = writer.BaseStream.Position;
        Serializer.SerializeTypeless((object) modifier, writer);
        long position3 = writer.BaseStream.Position;
        long num = position3 - position2;
        writer.BaseStream.Position = position1;
        writer.Write((int) num);
        writer.BaseStream.Position = position3;
      }
    }

    public void Deserialize(IReader reader)
    {
      int num1 = reader.ReadInt32();
      for (int index = 0; index < num1; ++index)
      {
        string id = reader.ReadKleiString();
        int num2 = reader.ReadInt32();
        int position = reader.Position;
        InstanceType instance = this.Get(id);
        if ((object) instance == null && this.resources != null)
        {
          ModifierType modifier = this.resources.TryGet(id);
          if ((object) modifier != null)
            instance = this.CreateInstance(modifier);
        }
        if ((object) instance == null)
        {
          if (id != "Condition")
            DebugUtil.LogWarningArgs(new object[2]
            {
              (object) ((Object) this.gameObject).name,
              (object) ("Missing modifier: " + id)
            });
          reader.SkipBytes(num2);
        }
        else if (!((object) instance is ISaveLoadable))
        {
          reader.SkipBytes(num2);
        }
        else
        {
          Deserializer.DeserializeTypeless((object) instance, reader);
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
