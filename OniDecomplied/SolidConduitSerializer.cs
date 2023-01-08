// Decompiled with JetBrains decompiler
// Type: SolidConduitSerializer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/SolidConduitSerializer")]
public class SolidConduitSerializer : KMonoBehaviour, ISaveLoadableDetails
{
  protected virtual void OnPrefabInit()
  {
  }

  protected virtual void OnSpawn()
  {
  }

  public void Serialize(BinaryWriter writer)
  {
    SolidConduitFlow solidConduitFlow = Game.Instance.solidConduitFlow;
    List<int> cells = solidConduitFlow.GetSOAInfo().Cells;
    int num = 0;
    for (int index = 0; index < cells.Count; ++index)
    {
      int cell = cells[index];
      SolidConduitFlow.ConduitContents contents = solidConduitFlow.GetContents(cell);
      if (contents.pickupableHandle.IsValid() && Object.op_Implicit((Object) solidConduitFlow.GetPickupable(contents.pickupableHandle)))
        ++num;
    }
    writer.Write(num);
    for (int index = 0; index < cells.Count; ++index)
    {
      int cell = cells[index];
      SolidConduitFlow.ConduitContents contents = solidConduitFlow.GetContents(cell);
      if (contents.pickupableHandle.IsValid())
      {
        Pickupable pickupable = solidConduitFlow.GetPickupable(contents.pickupableHandle);
        if (Object.op_Implicit((Object) pickupable))
        {
          writer.Write(cell);
          SaveLoadRoot component = ((Component) pickupable).GetComponent<SaveLoadRoot>();
          if (Object.op_Inequality((Object) component, (Object) null))
          {
            Tag saveLoadTag = ((Component) pickupable).GetComponent<KPrefabID>().GetSaveLoadTag();
            string name = ((Tag) ref saveLoadTag).Name;
            IOHelper.WriteKleiString(writer, name);
            component.Save(writer);
          }
          else
            Debug.Log((object) "Tried to save obj in solid conduit but obj has no SaveLoadRoot", (Object) ((Component) pickupable).gameObject);
        }
      }
    }
  }

  public void Deserialize(IReader reader)
  {
    SolidConduitFlow solidConduitFlow = Game.Instance.solidConduitFlow;
    int num = reader.ReadInt32();
    for (int index = 0; index < num; ++index)
    {
      int cell = reader.ReadInt32();
      Tag tag = TagManager.Create(reader.ReadKleiString());
      SaveLoadRoot saveLoadRoot = SaveLoadRoot.Load(tag, reader);
      if (Object.op_Inequality((Object) saveLoadRoot, (Object) null))
      {
        Pickupable component = ((Component) saveLoadRoot).GetComponent<Pickupable>();
        if (Object.op_Inequality((Object) component, (Object) null))
          solidConduitFlow.SetContents(cell, component);
      }
      else
        Debug.Log((object) ("Tried to deserialize " + tag.ToString() + " into storage but failed"), (Object) ((Component) this).gameObject);
    }
  }
}
