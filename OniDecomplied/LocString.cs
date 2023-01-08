// Decompiled with JetBrains decompiler
// Type: LocString
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class LocString
{
  [SerializeField]
  private string _text;
  [SerializeField]
  private StringKey _key;
  public const BindingFlags data_member_fields = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

  public string text => this._text;

  public StringKey key => this._key;

  public LocString(string text)
  {
    this._text = text;
    this._key = new StringKey();
  }

  public LocString(string text, string keystring)
  {
    this._text = text;
    this._key = new StringKey(keystring);
  }

  public LocString(string text, bool isLocalized)
  {
    this._text = text;
    this._key = new StringKey();
  }

  public static implicit operator LocString(string text) => new LocString(text);

  public static implicit operator string(LocString loc_string) => loc_string.text;

  public override string ToString() => Strings.Get(this.key).String;

  public void SetKey(string key_name) => this._key = new StringKey(key_name);

  public void SetKey(StringKey key) => this._key = key;

  public string Replace(string search, string replacement) => this.ToString().Replace(search, replacement);

  public static void CreateLocStringKeys(System.Type type, string parent_path = "STRINGS.")
  {
    FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
    string parent_path1 = (parent_path ?? "") + type.Name + ".";
    foreach (FieldInfo fieldInfo in fields)
    {
      if (!(fieldInfo.FieldType != typeof (LocString)))
      {
        string key_name = parent_path1 + fieldInfo.Name;
        LocString locString = (LocString) fieldInfo.GetValue((object) null);
        locString.SetKey(key_name);
        string text = locString.text;
        Strings.Add(new string[2]{ key_name, text });
        fieldInfo.SetValue((object) null, (object) locString);
      }
    }
    foreach (System.Type nestedType in type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
      LocString.CreateLocStringKeys(nestedType, parent_path1);
  }

  public static string[] GetStrings(System.Type type)
  {
    List<string> stringList = new List<string>();
    foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
    {
      LocString locString = (LocString) field.GetValue((object) null);
      stringList.Add(locString.text);
    }
    return stringList.ToArray();
  }
}
