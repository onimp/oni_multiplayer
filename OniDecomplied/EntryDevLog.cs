// Decompiled with JetBrains decompiler
// Type: EntryDevLog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

public class EntryDevLog
{
  [SerializeField]
  public List<EntryDevLog.ModificationRecord> modificationRecords = new List<EntryDevLog.ModificationRecord>();

  [Conditional("UNITY_EDITOR")]
  public void AddModificationRecord(
    EntryDevLog.ModificationRecord.ActionType actionType,
    string target,
    object newValue)
  {
    string author = this.TrimAuthor();
    this.modificationRecords.Add(new EntryDevLog.ModificationRecord(actionType, target, newValue, author));
  }

  [Conditional("UNITY_EDITOR")]
  public void InsertModificationRecord(
    int index,
    EntryDevLog.ModificationRecord.ActionType actionType,
    string target,
    object newValue)
  {
    string author = this.TrimAuthor();
    this.modificationRecords.Insert(index, new EntryDevLog.ModificationRecord(actionType, target, newValue, author));
  }

  private string TrimAuthor()
  {
    string str = "";
    string[] strArray1 = new string[7]
    {
      "Invoke",
      "CreateInstance",
      "AwakeInternal",
      "Internal",
      "<>",
      "YamlDotNet",
      "Deserialize"
    };
    string[] strArray2 = new string[13]
    {
      ".ctor",
      "Trigger",
      "AddContentContainerRange",
      "AddContentContainer",
      "InsertContentContainer",
      "KInstantiateUI",
      "Start",
      "InitializeComponentAwake",
      nameof (TrimAuthor),
      "InsertModificationRecord",
      "AddModificationRecord",
      "SetValue",
      "Write"
    };
    StackTrace stackTrace = new StackTrace();
    int num1 = 0;
    int index1 = 0;
    int num2 = 3;
    while (num1 < num2)
    {
      ++index1;
      if (stackTrace.FrameCount > index1)
      {
        MethodBase method = stackTrace.GetFrame(index1).GetMethod();
        bool flag = false;
        for (int index2 = 0; index2 < strArray1.Length; ++index2)
          flag = flag || method.Name.Contains(strArray1[index2]);
        for (int index3 = 0; index3 < strArray2.Length; ++index3)
          flag = flag || method.Name.Contains(strArray2[index3]);
        if (!flag && !stackTrace.GetFrame(index1).GetMethod().Name.StartsWith("set_") && !stackTrace.GetFrame(index1).GetMethod().Name.StartsWith("Instantiate"))
        {
          if (num1 != 0)
            str += " < ";
          ++num1;
          str += stackTrace.GetFrame(index1).GetMethod().Name;
        }
      }
      else
        break;
    }
    return str;
  }

  public class ModificationRecord
  {
    public EntryDevLog.ModificationRecord.ActionType actionType { get; private set; }

    public string target { get; private set; }

    public object newValue { get; private set; }

    public string author { get; private set; }

    public ModificationRecord(
      EntryDevLog.ModificationRecord.ActionType actionType,
      string target,
      object newValue,
      string author)
    {
      this.target = target;
      this.newValue = newValue;
      this.author = author;
      this.actionType = actionType;
    }

    public enum ActionType
    {
      Created,
      ChangeSubEntry,
      ChangeContent,
      ValueChange,
      YAMLData,
    }
  }
}
