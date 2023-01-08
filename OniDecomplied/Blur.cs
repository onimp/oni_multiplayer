// Decompiled with JetBrains decompiler
// Type: Blur
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class Blur
{
  private static Material blurMaterial;

  public static RenderTexture Run(Texture2D image)
  {
    if (Object.op_Equality((Object) Blur.blurMaterial, (Object) null))
      Blur.blurMaterial = new Material(Shader.Find("Klei/PostFX/Blur"));
    return (RenderTexture) null;
  }
}
