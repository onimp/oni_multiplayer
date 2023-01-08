// Decompiled with JetBrains decompiler
// Type: StateMachineSerializer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using System.IO;

public class StateMachineSerializer
{
  public const int SERIALIZER_PRE_DLC1 = 10;
  public const int SERIALIZER_TYPE_SUFFIX = 11;
  public const int SERIALIZER_OPTIMIZE_BUFFERS = 12;
  public const int SERIALIZER_EXPANSION1 = 20;
  private static int SERIALIZER_VERSION = 20;
  private const string TargetParameterName = "TargetParameter";
  private List<StateMachineSerializer.Entry> entries = new List<StateMachineSerializer.Entry>();

  public void Serialize(List<StateMachine.Instance> state_machines, BinaryWriter writer)
  {
    writer.Write(StateMachineSerializer.SERIALIZER_VERSION);
    long position1 = writer.BaseStream.Position;
    writer.Write(0);
    long position2 = writer.BaseStream.Position;
    try
    {
      int position3 = (int) writer.BaseStream.Position;
      int num = 0;
      writer.Write(num);
      foreach (StateMachine.Instance stateMachine in state_machines)
      {
        if (StateMachineSerializer.Entry.TrySerialize(stateMachine, writer))
          ++num;
      }
      int position4 = (int) writer.BaseStream.Position;
      writer.BaseStream.Position = (long) position3;
      writer.Write(num);
      writer.BaseStream.Position = (long) position4;
    }
    catch (Exception ex)
    {
      Debug.Log((object) "StateMachines: ");
      foreach (object stateMachine in state_machines)
        Debug.Log((object) stateMachine.ToString());
      Debug.LogError((object) ex);
    }
    long position5 = writer.BaseStream.Position;
    long num1 = position5 - position2;
    writer.BaseStream.Position = position1;
    writer.Write((int) num1);
    writer.BaseStream.Position = position5;
  }

  public void Deserialize(IReader reader)
  {
    int serializerVersion = reader.ReadInt32();
    int num = reader.ReadInt32();
    if (serializerVersion < 10)
    {
      Debug.LogWarning((object) ("State machine serializer version mismatch: " + serializerVersion.ToString() + "!=" + StateMachineSerializer.SERIALIZER_VERSION.ToString() + "\nDiscarding data."));
      reader.SkipBytes(num);
    }
    else if (serializerVersion < 12)
    {
      this.entries = StateMachineSerializer.OldEntryV11.DeserializeOldEntries(reader, serializerVersion);
    }
    else
    {
      int capacity = reader.ReadInt32();
      this.entries = new List<StateMachineSerializer.Entry>(capacity);
      for (int index = 0; index < capacity; ++index)
      {
        StateMachineSerializer.Entry entry = StateMachineSerializer.Entry.Deserialize(reader, serializerVersion);
        if (entry != null)
          this.entries.Add(entry);
      }
    }
  }

  private static string TrimAssemblyInfo(string type_name)
  {
    int length = type_name.IndexOf("[[");
    return length != -1 ? type_name.Substring(0, length) : type_name;
  }

  public bool Restore(StateMachine.Instance instance)
  {
    System.Type type = instance.GetType();
    for (int index = 0; index < this.entries.Count; ++index)
    {
      StateMachineSerializer.Entry entry = this.entries[index];
      if (entry.type == type && instance.serializationSuffix == entry.typeSuffix)
      {
        this.entries.RemoveAt(index);
        return entry.Restore(instance);
      }
    }
    return false;
  }

  private static bool DoesVersionHaveTypeSuffix(int version) => version >= 20 || version == 11;

  private class Entry
  {
    public int version;
    public System.Type type;
    public string typeSuffix;
    public string currentState;
    public FastReader entryData;

    public static bool TrySerialize(StateMachine.Instance smi, BinaryWriter writer)
    {
      if (!smi.IsRunning())
        return false;
      int position1 = (int) writer.BaseStream.Position;
      writer.Write(0);
      IOHelper.WriteKleiString(writer, smi.GetType().FullName);
      IOHelper.WriteKleiString(writer, smi.serializationSuffix);
      IOHelper.WriteKleiString(writer, smi.GetCurrentState().name);
      int position2 = (int) writer.BaseStream.Position;
      writer.Write(0);
      int position3 = (int) writer.BaseStream.Position;
      Serializer.SerializeTypeless((object) smi, writer);
      if (smi.GetStateMachine().serializable == StateMachine.SerializeType.ParamsOnly || smi.GetStateMachine().serializable == StateMachine.SerializeType.Both_DEPRECATED)
      {
        StateMachine.Parameter.Context[] parameterContexts = smi.GetParameterContexts();
        writer.Write(parameterContexts.Length);
        foreach (StateMachine.Parameter.Context context in parameterContexts)
        {
          long position4 = (long) (int) writer.BaseStream.Position;
          writer.Write(0);
          long position5 = (long) (int) writer.BaseStream.Position;
          IOHelper.WriteKleiString(writer, context.GetType().FullName);
          IOHelper.WriteKleiString(writer, context.parameter.name);
          context.Serialize(writer);
          long position6 = (long) (int) writer.BaseStream.Position;
          writer.BaseStream.Position = position4;
          long num = position6 - position5;
          writer.Write((int) num);
          writer.BaseStream.Position = position6;
        }
      }
      int num1 = (int) writer.BaseStream.Position - position3;
      if (num1 > 0)
      {
        int position7 = (int) writer.BaseStream.Position;
        writer.BaseStream.Position = (long) position2;
        writer.Write(num1);
        writer.BaseStream.Position = (long) position7;
        return true;
      }
      writer.BaseStream.Position = (long) position1;
      writer.BaseStream.SetLength((long) position1);
      return false;
    }

    public static StateMachineSerializer.Entry Deserialize(IReader reader, int serializerVersion)
    {
      StateMachineSerializer.Entry entry = new StateMachineSerializer.Entry();
      reader.ReadInt32();
      entry.version = serializerVersion;
      string typeName = reader.ReadKleiString();
      entry.type = System.Type.GetType(typeName);
      entry.typeSuffix = StateMachineSerializer.DoesVersionHaveTypeSuffix(serializerVersion) ? reader.ReadKleiString() : (string) null;
      entry.currentState = reader.ReadKleiString();
      int num = reader.ReadInt32();
      entry.entryData = new FastReader(reader.ReadBytes(num));
      return entry.type == (System.Type) null ? (StateMachineSerializer.Entry) null : entry;
    }

    public bool Restore(StateMachine.Instance smi)
    {
      if (Manager.HasDeserializationMapping(smi.GetType()))
        Deserializer.DeserializeTypeless((object) smi, (IReader) this.entryData);
      StateMachine.SerializeType serializable = smi.GetStateMachine().serializable;
      if (serializable == StateMachine.SerializeType.Never)
        return false;
      if ((serializable == StateMachine.SerializeType.Both_DEPRECATED || serializable == StateMachine.SerializeType.ParamsOnly) && !this.entryData.IsFinished)
      {
        StateMachine.Parameter.Context[] parameterContexts = smi.GetParameterContexts();
        int num1 = this.entryData.ReadInt32();
        for (int index = 0; index < num1; ++index)
        {
          int num2 = this.entryData.ReadInt32();
          int position = this.entryData.Position;
          string str1 = this.entryData.ReadKleiString().Replace("Version=2.0.0.0", "Version=4.0.0.0");
          string str2 = this.entryData.ReadKleiString();
          foreach (StateMachine.Parameter.Context context in parameterContexts)
          {
            if (context.parameter.name == str2 && (this.version > 10 || !(context.parameter.GetType().Name == "TargetParameter")) && context.GetType().FullName == str1)
            {
              context.Deserialize((IReader) this.entryData, smi);
              break;
            }
          }
          this.entryData.SkipBytes(num2 - (this.entryData.Position - position));
        }
      }
      if (serializable == StateMachine.SerializeType.Both_DEPRECATED || serializable == StateMachine.SerializeType.CurrentStateOnly_DEPRECATED)
      {
        StateMachine.BaseState state = smi.GetStateMachine().GetState(this.currentState);
        if (state != null)
        {
          smi.GoTo(state);
          return true;
        }
      }
      return false;
    }
  }

  private class OldEntryV11
  {
    public int version;
    public int dataPos;
    public System.Type type;
    public string typeSuffix;
    public string currentState;

    public OldEntryV11(
      int version,
      int dataPos,
      System.Type type,
      string typeSuffix,
      string currentState)
    {
      this.version = version;
      this.dataPos = dataPos;
      this.type = type;
      this.typeSuffix = typeSuffix;
      this.currentState = currentState;
    }

    public static List<StateMachineSerializer.Entry> DeserializeOldEntries(
      IReader reader,
      int serializerVersion)
    {
      Debug.Assert(serializerVersion < 12);
      List<StateMachineSerializer.OldEntryV11> oldEntryV11List = StateMachineSerializer.OldEntryV11.ReadEntries(reader, serializerVersion);
      byte[] numArray = StateMachineSerializer.OldEntryV11.ReadEntryData(reader);
      List<StateMachineSerializer.Entry> entryList = new List<StateMachineSerializer.Entry>(oldEntryV11List.Count);
      foreach (StateMachineSerializer.OldEntryV11 oldEntryV11 in oldEntryV11List)
      {
        StateMachineSerializer.Entry entry = new StateMachineSerializer.Entry();
        entry.version = serializerVersion;
        entry.type = oldEntryV11.type;
        entry.typeSuffix = oldEntryV11.typeSuffix;
        entry.currentState = oldEntryV11.currentState;
        entry.entryData = new FastReader(numArray);
        entry.entryData.SkipBytes(oldEntryV11.dataPos);
        entryList.Add(entry);
      }
      return entryList;
    }

    private static StateMachineSerializer.OldEntryV11 Deserialize(
      IReader reader,
      int serializerVersion)
    {
      int version = reader.ReadInt32();
      int dataPos = reader.ReadInt32();
      string typeName = reader.ReadKleiString();
      string typeSuffix = StateMachineSerializer.DoesVersionHaveTypeSuffix(serializerVersion) ? reader.ReadKleiString() : (string) null;
      string currentState = reader.ReadKleiString();
      System.Type type = System.Type.GetType(typeName);
      return type == (System.Type) null ? (StateMachineSerializer.OldEntryV11) null : new StateMachineSerializer.OldEntryV11(version, dataPos, type, typeSuffix, currentState);
    }

    private static List<StateMachineSerializer.OldEntryV11> ReadEntries(
      IReader reader,
      int serializerVersion)
    {
      List<StateMachineSerializer.OldEntryV11> oldEntryV11List = new List<StateMachineSerializer.OldEntryV11>();
      int num = reader.ReadInt32();
      for (int index = 0; index < num; ++index)
      {
        StateMachineSerializer.OldEntryV11 oldEntryV11 = StateMachineSerializer.OldEntryV11.Deserialize(reader, serializerVersion);
        if (oldEntryV11 != null)
          oldEntryV11List.Add(oldEntryV11);
      }
      return oldEntryV11List;
    }

    private static byte[] ReadEntryData(IReader reader)
    {
      int num = reader.ReadInt32();
      return reader.ReadBytes(num);
    }
  }
}
