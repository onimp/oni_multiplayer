// Decompiled with JetBrains decompiler
// Type: GameTagExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class GameTagExtensions
{
  public static GameObject Prefab(this Tag tag) => Assets.GetPrefab(tag);

  public static string ProperName(this Tag tag) => TagManager.GetProperName(tag, false);

  public static string ProperNameStripLink(this Tag tag) => TagManager.GetProperName(tag, true);

  public static Tag Create(SimHashes id) => TagManager.Create(id.ToString());

  public static Tag CreateTag(this SimHashes id) => TagManager.Create(id.ToString());
}
