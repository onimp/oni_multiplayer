// Decompiled with JetBrains decompiler
// Type: KPrefabIDExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class KPrefabIDExtensions
{
  public static Tag PrefabID(this Component cmp) => cmp.GetComponent<KPrefabID>().PrefabID();

  public static Tag PrefabID(this GameObject go) => go.GetComponent<KPrefabID>().PrefabID();

  public static Tag PrefabID(this StateMachine.Instance smi) => smi.GetComponent<KPrefabID>().PrefabID();

  public static bool IsPrefabID(this Component cmp, Tag id) => cmp.GetComponent<KPrefabID>().IsPrefabID(id);

  public static bool IsPrefabID(this GameObject go, Tag id) => go.GetComponent<KPrefabID>().IsPrefabID(id);

  public static bool HasTag(this Component cmp, Tag tag) => cmp.GetComponent<KPrefabID>().HasTag(tag);

  public static bool HasTag(this GameObject go, Tag tag) => go.GetComponent<KPrefabID>().HasTag(tag);

  public static bool HasAnyTags(this Component cmp, Tag[] tags) => cmp.GetComponent<KPrefabID>().HasAnyTags(tags);

  public static bool HasAnyTags(this GameObject go, Tag[] tags) => go.GetComponent<KPrefabID>().HasAnyTags(tags);

  public static bool HasAllTags(this Component cmp, Tag[] tags) => cmp.GetComponent<KPrefabID>().HasAllTags(tags);

  public static bool HasAllTags(this GameObject go, Tag[] tags) => go.GetComponent<KPrefabID>().HasAllTags(tags);

  public static void AddTag(this GameObject go, Tag tag) => go.GetComponent<KPrefabID>().AddTag(tag, false);

  public static void AddTag(this Component cmp, Tag tag) => cmp.GetComponent<KPrefabID>().AddTag(tag, false);

  public static void RemoveTag(this GameObject go, Tag tag) => go.GetComponent<KPrefabID>().RemoveTag(tag);

  public static void RemoveTag(this Component cmp, Tag tag) => cmp.GetComponent<KPrefabID>().RemoveTag(tag);
}
