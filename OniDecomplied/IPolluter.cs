// Decompiled with JetBrains decompiler
// Type: IPolluter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public interface IPolluter
{
  int GetRadius();

  int GetNoise();

  GameObject GetGameObject();

  void SetAttributes(Vector2 pos, int dB, GameObject go, string name = null);

  string GetName();

  Vector2 GetPosition();

  void Clear();

  void SetSplat(NoiseSplat splat);
}
