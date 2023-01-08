// Decompiled with JetBrains decompiler
// Type: BloomEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class BloomEffect : MonoBehaviour
{
  private Material BloomMaskMaterial;
  private Material BloomCompositeMaterial;
  public int iterations = 3;
  public float blurSpread = 0.6f;
  public Shader blurShader;
  private Material m_Material;

  protected Material material
  {
    get
    {
      if (Object.op_Equality((Object) this.m_Material, (Object) null))
      {
        this.m_Material = new Material(this.blurShader);
        ((Object) this.m_Material).hideFlags = (HideFlags) 52;
      }
      return this.m_Material;
    }
  }

  protected void OnDisable()
  {
    if (!Object.op_Implicit((Object) this.m_Material))
      return;
    Object.DestroyImmediate((Object) this.m_Material);
  }

  protected void Start()
  {
    if (!Object.op_Implicit((Object) this.blurShader) || !this.material.shader.isSupported)
    {
      ((Behaviour) this).enabled = false;
    }
    else
    {
      this.BloomMaskMaterial = new Material(Shader.Find("Klei/PostFX/BloomMask"));
      this.BloomCompositeMaterial = new Material(Shader.Find("Klei/PostFX/BloomComposite"));
    }
  }

  public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
  {
    float num = (float) (0.5 + (double) iteration * (double) this.blurSpread);
    Graphics.BlitMultiTap((Texture) source, dest, this.material, new Vector2[4]
    {
      new Vector2(-num, -num),
      new Vector2(-num, num),
      new Vector2(num, num),
      new Vector2(num, -num)
    });
  }

  private void DownSample4x(RenderTexture source, RenderTexture dest)
  {
    float num = 1f;
    Graphics.BlitMultiTap((Texture) source, dest, this.material, new Vector2[4]
    {
      new Vector2(-num, -num),
      new Vector2(-num, num),
      new Vector2(num, num),
      new Vector2(num, -num)
    });
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    RenderTexture temporary1 = RenderTexture.GetTemporary(((Texture) source).width, ((Texture) source).height, 0);
    ((Object) temporary1).name = "bloom_source";
    Graphics.Blit((Texture) source, temporary1, this.BloomMaskMaterial);
    int num1 = Math.Max(((Texture) source).width / 4, 4);
    int num2 = Math.Max(((Texture) source).height / 4, 4);
    RenderTexture renderTexture = RenderTexture.GetTemporary(num1, num2, 0);
    ((Object) renderTexture).name = "bloom_downsampled";
    this.DownSample4x(temporary1, renderTexture);
    RenderTexture.ReleaseTemporary(temporary1);
    for (int iteration = 0; iteration < this.iterations; ++iteration)
    {
      RenderTexture temporary2 = RenderTexture.GetTemporary(num1, num2, 0);
      ((Object) temporary2).name = "bloom_blurred";
      this.FourTapCone(renderTexture, temporary2, iteration);
      RenderTexture.ReleaseTemporary(renderTexture);
      renderTexture = temporary2;
    }
    this.BloomCompositeMaterial.SetTexture("_BloomTex", (Texture) renderTexture);
    Graphics.Blit((Texture) source, destination, this.BloomCompositeMaterial);
    RenderTexture.ReleaseTemporary(renderTexture);
  }
}
