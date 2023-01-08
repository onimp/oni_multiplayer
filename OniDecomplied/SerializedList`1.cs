// Decompiled with JetBrains decompiler
// Type: SerializedList`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using System.IO;

[SerializationConfig]
public class SerializedList<ItemType>
{
  [Serialize]
  private byte[] serializationBuffer;
  private List<ItemType> items = new List<ItemType>();

  public int Count => this.items.Count;

  public IEnumerator<ItemType> GetEnumerator() => (IEnumerator<ItemType>) this.items.GetEnumerator();

  public ItemType this[int idx] => this.items[idx];

  public void Add(ItemType item) => this.items.Add(item);

  public void Remove(ItemType item) => this.items.Remove(item);

  public void RemoveAt(int idx) => this.items.RemoveAt(idx);

  public bool Contains(ItemType item) => this.items.Contains(item);

  public void Clear() => this.items.Clear();

  [System.Runtime.Serialization.OnSerializing]
  private void OnSerializing()
  {
    MemoryStream output = new MemoryStream();
    BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
    binaryWriter.Write(this.items.Count);
    foreach (ItemType itemType in this.items)
    {
      IOHelper.WriteKleiString(binaryWriter, itemType.GetType().FullName);
      long position1 = binaryWriter.BaseStream.Position;
      binaryWriter.Write(0);
      long position2 = binaryWriter.BaseStream.Position;
      Serializer.SerializeTypeless((object) itemType, binaryWriter);
      long position3 = binaryWriter.BaseStream.Position;
      long num = position3 - position2;
      binaryWriter.BaseStream.Position = position1;
      binaryWriter.Write((int) num);
      binaryWriter.BaseStream.Position = position3;
    }
    output.Flush();
    this.serializationBuffer = output.ToArray();
  }

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    if (this.serializationBuffer == null)
      return;
    FastReader fastReader = new FastReader(this.serializationBuffer);
    int num1 = fastReader.ReadInt32();
    for (int index = 0; index < num1; ++index)
    {
      string typeName = fastReader.ReadKleiString();
      int num2 = fastReader.ReadInt32();
      int position = fastReader.Position;
      System.Type type = System.Type.GetType(typeName);
      if (type == (System.Type) null)
      {
        DebugUtil.LogWarningArgs(new object[1]
        {
          (object) ("Type no longer exists: " + typeName)
        });
        fastReader.SkipBytes(num2);
      }
      else
      {
        ItemType itemType = !(typeof (ItemType) != type) ? default (ItemType) : (ItemType) Activator.CreateInstance(type);
        Deserializer.DeserializeTypeless((object) itemType, (IReader) fastReader);
        if (fastReader.Position != position + num2)
        {
          DebugUtil.LogWarningArgs(new object[5]
          {
            (object) "Expected to be at offset",
            (object) (position + num2),
            (object) "but was only at offset",
            (object) fastReader.Position,
            (object) ". Skipping to catch up."
          });
          fastReader.SkipBytes(position + num2 - fastReader.Position);
        }
        this.items.Add(itemType);
      }
    }
  }
}
