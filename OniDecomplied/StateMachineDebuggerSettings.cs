// Decompiled with JetBrains decompiler
// Type: StateMachineDebuggerSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineDebuggerSettings : ScriptableObject
{
  public List<StateMachineDebuggerSettings.Entry> entries = new List<StateMachineDebuggerSettings.Entry>();
  private static StateMachineDebuggerSettings _Instance;

  public IEnumerator<StateMachineDebuggerSettings.Entry> GetEnumerator() => (IEnumerator<StateMachineDebuggerSettings.Entry>) this.entries.GetEnumerator();

  public static StateMachineDebuggerSettings Get()
  {
    if (Object.op_Equality((Object) StateMachineDebuggerSettings._Instance, (Object) null))
    {
      StateMachineDebuggerSettings._Instance = Resources.Load<StateMachineDebuggerSettings>(nameof (StateMachineDebuggerSettings));
      StateMachineDebuggerSettings._Instance.Initialize();
    }
    return StateMachineDebuggerSettings._Instance;
  }

  private void Initialize()
  {
    foreach (System.Type currentDomainType in App.GetCurrentDomainTypes())
    {
      if (typeof (StateMachine).IsAssignableFrom(currentDomainType))
        this.CreateEntry(currentDomainType);
    }
    this.entries.RemoveAll((Predicate<StateMachineDebuggerSettings.Entry>) (x => x.type == (System.Type) null));
  }

  public StateMachineDebuggerSettings.Entry CreateEntry(System.Type type)
  {
    foreach (StateMachineDebuggerSettings.Entry entry in this.entries)
    {
      if (type.FullName == entry.typeName)
      {
        entry.type = type;
        return entry;
      }
    }
    StateMachineDebuggerSettings.Entry entry1 = new StateMachineDebuggerSettings.Entry(type);
    this.entries.Add(entry1);
    return entry1;
  }

  public void Clear()
  {
    this.entries.Clear();
    this.Initialize();
  }

  [Serializable]
  public class Entry
  {
    public System.Type type;
    public string typeName;
    public bool breakOnGoTo;
    public bool enableConsoleLogging;
    public bool saveHistory;

    public Entry(System.Type type)
    {
      this.typeName = type.FullName;
      this.type = type;
    }
  }
}
