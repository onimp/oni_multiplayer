// Decompiled with JetBrains decompiler
// Type: SpriteSheet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public struct SpriteSheet
{
  public string name;
  public int numFrames;
  public int numXFrames;
  public Vector2 uvFrameSize;
  public int renderLayer;
  public Material material;
  public Texture2D texture;
}
