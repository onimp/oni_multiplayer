// Decompiled with JetBrains decompiler
// Type: IEntityConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public interface IEntityConfig
{
  GameObject CreatePrefab();

  void OnPrefabInit(GameObject inst);

  void OnSpawn(GameObject inst);

  string[] GetDlcIds();
}
