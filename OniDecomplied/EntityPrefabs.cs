// Decompiled with JetBrains decompiler
// Type: EntityPrefabs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EntityPrefabs")]
public class EntityPrefabs : KMonoBehaviour
{
  public GameObject SelectMarker;
  public GameObject ForegroundLayer;

  public static EntityPrefabs Instance { get; private set; }

  public static void DestroyInstance() => EntityPrefabs.Instance = (EntityPrefabs) null;

  protected virtual void OnPrefabInit() => EntityPrefabs.Instance = this;
}
