// Decompiled with JetBrains decompiler
// Type: LightBufferCompositor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class LightBufferCompositor : MonoBehaviour
{
  [SerializeField]
  private Material material;
  [SerializeField]
  private Material blurMaterial;
  private bool particlesEnabled = true;

  private void Start()
  {
    this.material = new Material(Shader.Find("Klei/PostFX/LightBufferCompositor"));
    this.material.SetTexture("_InvalidTex", (Texture) Assets.instance.invalidAreaTex);
    this.blurMaterial = new Material(Shader.Find("Klei/PostFX/Blur"));
    this.OnShadersReloaded();
    ShaderReloader.Register(new System.Action(this.OnShadersReloaded));
  }

  private void OnEnable() => this.OnShadersReloaded();

  private void DownSample4x(Texture source, RenderTexture dest)
  {
    float num = 1f;
    Graphics.BlitMultiTap(source, dest, this.blurMaterial, new Vector2[4]
    {
      new Vector2(-num, -num),
      new Vector2(-num, num),
      new Vector2(num, num),
      new Vector2(num, -num)
    });
  }

  [ContextMenu("ToggleParticles")]
  private void ToggleParticles()
  {
    this.particlesEnabled = !this.particlesEnabled;
    this.UpdateMaterialState();
  }

  public void SetParticlesEnabled(bool enabled)
  {
    this.particlesEnabled = enabled;
    this.UpdateMaterialState();
  }

  private void UpdateMaterialState()
  {
    if (this.particlesEnabled)
      this.material.DisableKeyword("DISABLE_TEMPERATURE_PARTICLES");
    else
      this.material.EnableKeyword("DISABLE_TEMPERATURE_PARTICLES");
  }

  private void OnRenderImage(RenderTexture src, RenderTexture dest)
  {
    RenderTexture renderTexture = (RenderTexture) null;
    if (Object.op_Inequality((Object) PropertyTextures.instance, (Object) null))
    {
      Texture texture = PropertyTextures.instance.GetTexture(PropertyTextures.Property.Temperature);
      ((Object) texture).name = "temperature_tex";
      renderTexture = RenderTexture.GetTemporary(Screen.width / 8, Screen.height / 8);
      ((Texture) renderTexture).filterMode = (FilterMode) 1;
      Graphics.Blit(texture, renderTexture, this.blurMaterial);
      Shader.SetGlobalTexture("_BlurredTemperature", (Texture) renderTexture);
    }
    this.material.SetTexture("_LightBufferTex", (Texture) LightBuffer.Instance.Texture);
    Graphics.Blit((Texture) src, dest, this.material);
    if (!Object.op_Inequality((Object) renderTexture, (Object) null))
      return;
    RenderTexture.ReleaseTemporary(renderTexture);
  }

  private void OnShadersReloaded()
  {
    if (!Object.op_Inequality((Object) this.material, (Object) null) || !Object.op_Inequality((Object) Lighting.Instance, (Object) null))
      return;
    this.material.SetTexture("_EmberTex", (Texture) Lighting.Instance.Settings.EmberTex);
    this.material.SetTexture("_FrostTex", (Texture) Lighting.Instance.Settings.FrostTex);
    this.material.SetTexture("_Thermal1Tex", (Texture) Lighting.Instance.Settings.Thermal1Tex);
    this.material.SetTexture("_Thermal2Tex", (Texture) Lighting.Instance.Settings.Thermal2Tex);
    this.material.SetTexture("_RadHaze1Tex", (Texture) Lighting.Instance.Settings.Radiation1Tex);
    this.material.SetTexture("_RadHaze2Tex", (Texture) Lighting.Instance.Settings.Radiation2Tex);
    this.material.SetTexture("_RadHaze3Tex", (Texture) Lighting.Instance.Settings.Radiation3Tex);
    this.material.SetTexture("_NoiseTex", (Texture) Lighting.Instance.Settings.NoiseTex);
  }
}
