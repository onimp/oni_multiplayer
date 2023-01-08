// Decompiled with JetBrains decompiler
// Type: MemorySnapshot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class MemorySnapshot
{
  public Dictionary<int, MemorySnapshot.TypeData> types = new Dictionary<int, MemorySnapshot.TypeData>();
  public Dictionary<int, MemorySnapshot.FieldCount> fieldCounts = new Dictionary<int, MemorySnapshot.FieldCount>();
  public HashSet<object> walked = new HashSet<object>();
  public List<FieldInfo> statics = new List<FieldInfo>();
  public Dictionary<string, MemorySnapshot.DetailInfo> detailTypeCount = new Dictionary<string, MemorySnapshot.DetailInfo>();
  private static readonly System.Type detailType = typeof (byte[]);
  private static readonly string detailTypeStr = MemorySnapshot.detailType.ToString();
  private List<MemorySnapshot.FieldArgs> fieldsToProcess = new List<MemorySnapshot.FieldArgs>();
  private List<MemorySnapshot.ReferenceArgs> refsToProcess = new List<MemorySnapshot.ReferenceArgs>();

  public static MemorySnapshot.TypeData GetTypeData(
    System.Type type,
    Dictionary<int, MemorySnapshot.TypeData> types)
  {
    int hashCode = type.GetHashCode();
    MemorySnapshot.TypeData typeData = (MemorySnapshot.TypeData) null;
    if (!types.TryGetValue(hashCode, out typeData))
    {
      typeData = new MemorySnapshot.TypeData(type);
      types[hashCode] = typeData;
    }
    return typeData;
  }

  public static void IncrementFieldCount(
    Dictionary<int, MemorySnapshot.FieldCount> field_counts,
    string name)
  {
    int hashCode = name.GetHashCode();
    MemorySnapshot.FieldCount fieldCount = (MemorySnapshot.FieldCount) null;
    if (!field_counts.TryGetValue(hashCode, out fieldCount))
    {
      fieldCount = new MemorySnapshot.FieldCount();
      fieldCount.name = name;
      field_counts[hashCode] = fieldCount;
    }
    ++fieldCount.count;
  }

  private void CountReference(MemorySnapshot.ReferenceArgs refArgs)
  {
    if (MemorySnapshot.ShouldExclude(refArgs.reference_type))
      return;
    if (refArgs.reference_type == MemorySnapshot.detailType)
    {
      string str = !Object.op_Inequality(refArgs.lineage.obj as Object, (Object) null) ? "\"" + MemorySnapshot.detailTypeStr : "\"" + ((Object) refArgs.lineage.obj).name;
      if (refArgs.lineage.parent0 != (System.Type) null)
        str = str + "\",\"" + refArgs.lineage.parent0.ToString();
      if (refArgs.lineage.parent1 != (System.Type) null)
        str = str + "\",\"" + refArgs.lineage.parent1.ToString();
      if (refArgs.lineage.parent2 != (System.Type) null)
        str = str + "\",\"" + refArgs.lineage.parent2.ToString();
      if (refArgs.lineage.parent3 != (System.Type) null)
        str = str + "\",\"" + refArgs.lineage.parent3.ToString();
      if (refArgs.lineage.parent4 != (System.Type) null)
        str = str + "\",\"" + refArgs.lineage.parent4.ToString();
      string key = str + "\"\n";
      MemorySnapshot.DetailInfo detailInfo;
      this.detailTypeCount.TryGetValue(key, out detailInfo);
      ++detailInfo.count;
      if (typeof (Array).IsAssignableFrom(refArgs.reference_type) && refArgs.lineage.obj != null)
      {
        Array array = refArgs.lineage.obj as Array;
        detailInfo.numArrayEntries += array != null ? array.Length : 0;
      }
      this.detailTypeCount[key] = detailInfo;
    }
    if (refArgs.reference_type.IsClass)
    {
      ++MemorySnapshot.GetTypeData(refArgs.reference_type, this.types).refCount;
      MemorySnapshot.IncrementFieldCount(this.fieldCounts, refArgs.field_name);
    }
    if (refArgs.lineage.obj == null)
      return;
    try
    {
      if (refArgs.lineage.obj.GetType().IsClass)
      {
        if (!this.walked.Add(refArgs.lineage.obj))
          return;
      }
    }
    catch
    {
      return;
    }
    MemorySnapshot.TypeData typeData = MemorySnapshot.GetTypeData(refArgs.lineage.obj.GetType(), this.types);
    if (typeData.type.IsClass)
    {
      ++typeData.instanceCount;
      if (typeof (Array).IsAssignableFrom(typeData.type))
      {
        Array array = refArgs.lineage.obj as Array;
        typeData.numArrayEntries += array != null ? array.Length : 0;
      }
      MemorySnapshot.HierarchyNode key = new MemorySnapshot.HierarchyNode(refArgs.lineage.parent0, refArgs.lineage.parent1, refArgs.lineage.parent2, refArgs.lineage.parent3, refArgs.lineage.parent4);
      int num = 0;
      typeData.hierarchies.TryGetValue(key, out num);
      typeData.hierarchies[key] = num + 1;
    }
    foreach (FieldInfo field in typeData.fields)
      this.fieldsToProcess.Add(new MemorySnapshot.FieldArgs(field, new MemorySnapshot.Lineage(refArgs.lineage.obj, refArgs.lineage.parent3, refArgs.lineage.parent2, refArgs.lineage.parent1, refArgs.lineage.parent0, field.DeclaringType)));
    if (!(refArgs.lineage.obj is ICollection collection))
      return;
    System.Type type = typeof (object);
    if (collection.GetType().GetElementType() != (System.Type) null)
      type = collection.GetType().GetElementType();
    else if (collection.GetType().GetGenericArguments().Length != 0)
      type = collection.GetType().GetGenericArguments()[0];
    if (MemorySnapshot.ShouldExclude(type))
      return;
    foreach (object obj in (IEnumerable) collection)
      this.refsToProcess.Add(new MemorySnapshot.ReferenceArgs(type, refArgs.field_name + ".Item", new MemorySnapshot.Lineage(obj, refArgs.lineage.parent3, refArgs.lineage.parent2, refArgs.lineage.parent1, refArgs.lineage.parent0, collection.GetType())));
  }

  private void CountField(MemorySnapshot.FieldArgs fieldArgs)
  {
    if (MemorySnapshot.ShouldExclude(fieldArgs.field.FieldType))
      return;
    object obj = (object) null;
    try
    {
      if (!fieldArgs.field.FieldType.Name.Contains("*"))
        obj = fieldArgs.field.GetValue(fieldArgs.lineage.obj);
    }
    catch
    {
      obj = (object) null;
    }
    string field_name = fieldArgs.field.DeclaringType.ToString() + "." + fieldArgs.field.Name;
    this.refsToProcess.Add(new MemorySnapshot.ReferenceArgs(fieldArgs.field.FieldType, field_name, new MemorySnapshot.Lineage(obj, fieldArgs.lineage.parent3, fieldArgs.lineage.parent2, fieldArgs.lineage.parent1, fieldArgs.lineage.parent0, fieldArgs.field.DeclaringType)));
  }

  private static bool ShouldExclude(System.Type type) => type.IsPrimitive || type.IsEnum || type == typeof (MemorySnapshot);

  private void CountAll()
  {
    while (this.refsToProcess.Count > 0 || this.fieldsToProcess.Count > 0)
    {
      while (this.fieldsToProcess.Count > 0)
      {
        MemorySnapshot.FieldArgs fieldArgs = this.fieldsToProcess[this.fieldsToProcess.Count - 1];
        this.fieldsToProcess.RemoveAt(this.fieldsToProcess.Count - 1);
        this.CountField(fieldArgs);
      }
      while (this.refsToProcess.Count > 0)
      {
        MemorySnapshot.ReferenceArgs refArgs = this.refsToProcess[this.refsToProcess.Count - 1];
        this.refsToProcess.RemoveAt(this.refsToProcess.Count - 1);
        this.CountReference(refArgs);
      }
    }
  }

  public MemorySnapshot()
  {
    MemorySnapshot.Lineage lineage = new MemorySnapshot.Lineage((object) null, (System.Type) null, (System.Type) null, (System.Type) null, (System.Type) null, (System.Type) null);
    foreach (System.Type currentDomainType in App.GetCurrentDomainTypes())
    {
      foreach (FieldInfo field in currentDomainType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
      {
        if (field.IsStatic)
        {
          this.statics.Add(field);
          lineage.parent0 = field.DeclaringType;
          this.fieldsToProcess.Add(new MemorySnapshot.FieldArgs(field, lineage));
        }
      }
    }
    this.CountAll();
    foreach (Object @object in Resources.FindObjectsOfTypeAll(typeof (Object)))
    {
      lineage.obj = (object) @object;
      lineage.parent0 = ((object) @object).GetType();
      this.refsToProcess.Add(new MemorySnapshot.ReferenceArgs(((object) @object).GetType(), "Object." + @object.name, lineage));
    }
    this.CountAll();
  }

  public void WriteTypeDetails(MemorySnapshot compare)
  {
    List<KeyValuePair<string, MemorySnapshot.DetailInfo>> keyValuePairList = (List<KeyValuePair<string, MemorySnapshot.DetailInfo>>) null;
    if (compare != null)
      keyValuePairList = compare.detailTypeCount.ToList<KeyValuePair<string, MemorySnapshot.DetailInfo>>();
    List<KeyValuePair<string, MemorySnapshot.DetailInfo>> list = this.detailTypeCount.ToList<KeyValuePair<string, MemorySnapshot.DetailInfo>>();
    list.Sort((Comparison<KeyValuePair<string, MemorySnapshot.DetailInfo>>) ((x, y) => y.Value.count - x.Value.count));
    using (StreamWriter streamWriter = new StreamWriter(GarbageProfiler.GetFileName("type_details_" + MemorySnapshot.detailTypeStr)))
    {
      streamWriter.WriteLine("Delta,Count,NumArrayEntries,Type");
      foreach (KeyValuePair<string, MemorySnapshot.DetailInfo> keyValuePair1 in list)
      {
        int count = keyValuePair1.Value.count;
        if (keyValuePairList != null)
        {
          foreach (KeyValuePair<string, MemorySnapshot.DetailInfo> keyValuePair2 in keyValuePairList)
          {
            if (keyValuePair2.Key == keyValuePair1.Key)
            {
              count -= keyValuePair2.Value.count;
              break;
            }
          }
        }
        streamWriter.Write(count.ToString() + "," + keyValuePair1.Value.count.ToString() + "," + keyValuePair1.Value.numArrayEntries.ToString() + "," + keyValuePair1.Key);
      }
    }
  }

  public struct HierarchyNode
  {
    public System.Type parent0;
    public System.Type parent1;
    public System.Type parent2;
    public System.Type parent3;
    public System.Type parent4;

    public HierarchyNode(
      System.Type parent_0,
      System.Type parent_1,
      System.Type parent_2,
      System.Type parent_3,
      System.Type parent_4)
    {
      this.parent0 = parent_0;
      this.parent1 = parent_1;
      this.parent2 = parent_2;
      this.parent3 = parent_3;
      this.parent4 = parent_4;
    }

    public bool Equals(MemorySnapshot.HierarchyNode a, MemorySnapshot.HierarchyNode b) => a.parent0 == b.parent0 && a.parent1 == b.parent1 && a.parent2 == b.parent2 && a.parent3 == b.parent3 && a.parent4 == b.parent4;

    public override int GetHashCode()
    {
      int hashCode = 0;
      if (this.parent0 != (System.Type) null)
        hashCode += this.parent0.GetHashCode();
      if (this.parent1 != (System.Type) null)
        hashCode += this.parent1.GetHashCode();
      if (this.parent2 != (System.Type) null)
        hashCode += this.parent2.GetHashCode();
      if (this.parent3 != (System.Type) null)
        hashCode += this.parent3.GetHashCode();
      if (this.parent4 != (System.Type) null)
        hashCode += this.parent4.GetHashCode();
      return hashCode;
    }

    public override string ToString() => this.parent4 != (System.Type) null ? this.parent4.ToString() + "--" + this.parent3.ToString() + "--" + this.parent2.ToString() + "--" + this.parent1.ToString() + "--" + this.parent0.ToString() : (this.parent3 != (System.Type) null ? this.parent3.ToString() + "--" + this.parent2.ToString() + "--" + this.parent1.ToString() + "--" + this.parent0.ToString() : (this.parent2 != (System.Type) null ? this.parent2.ToString() + "--" + this.parent1.ToString() + "--" + this.parent0.ToString() : (this.parent1 != (System.Type) null ? this.parent1.ToString() + "--" + this.parent0.ToString() : this.parent0.ToString())));
  }

  public class FieldCount
  {
    public string name;
    public int count;
  }

  public class TypeData
  {
    public Dictionary<MemorySnapshot.HierarchyNode, int> hierarchies = new Dictionary<MemorySnapshot.HierarchyNode, int>();
    public System.Type type;
    public List<FieldInfo> fields;
    public int instanceCount;
    public int refCount;
    public int numArrayEntries;

    public TypeData(System.Type type)
    {
      this.type = type;
      this.fields = new List<FieldInfo>();
      this.instanceCount = 0;
      this.refCount = 0;
      this.numArrayEntries = 0;
      foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
      {
        if (!field.IsStatic && !MemorySnapshot.ShouldExclude(field.FieldType))
          this.fields.Add(field);
      }
    }
  }

  public struct DetailInfo
  {
    public int count;
    public int numArrayEntries;
  }

  private struct Lineage
  {
    public object obj;
    public System.Type parent0;
    public System.Type parent1;
    public System.Type parent2;
    public System.Type parent3;
    public System.Type parent4;

    public Lineage(
      object obj,
      System.Type parent4,
      System.Type parent3,
      System.Type parent2,
      System.Type parent1,
      System.Type parent0)
    {
      this.obj = obj;
      this.parent0 = parent0;
      this.parent1 = parent1;
      this.parent2 = parent2;
      this.parent3 = parent3;
      this.parent4 = parent4;
    }
  }

  private struct ReferenceArgs
  {
    public System.Type reference_type;
    public string field_name;
    public MemorySnapshot.Lineage lineage;

    public ReferenceArgs(System.Type reference_type, string field_name, MemorySnapshot.Lineage lineage)
    {
      this.reference_type = reference_type;
      this.lineage = lineage;
      this.field_name = field_name;
    }
  }

  private struct FieldArgs
  {
    public FieldInfo field;
    public MemorySnapshot.Lineage lineage;

    public FieldArgs(FieldInfo field, MemorySnapshot.Lineage lineage)
    {
      this.field = field;
      this.lineage = lineage;
    }
  }
}
