// Decompiled with JetBrains decompiler
// Type: MultipleRenderTargetProxy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class MultipleRenderTargetProxy : MonoBehaviour
{
  public RenderTexture[] Textures = new RenderTexture[3];
  private bool colouredOverlayBufferEnabled;

  private void Start()
  {
    if (Object.op_Inequality((Object) ScreenResize.Instance, (Object) null))
      ScreenResize.Instance.OnResize += new System.Action(this.OnResize);
    this.CreateRenderTarget();
    ShaderReloader.Register(new System.Action(this.OnShadersReloaded));
  }

  public void ToggleColouredOverlayView(bool enabled)
  {
    this.colouredOverlayBufferEnabled = enabled;
    this.CreateRenderTarget();
  }

  private void CreateRenderTarget()
  {
    RenderBuffer[] renderBufferArray = new RenderBuffer[this.colouredOverlayBufferEnabled ? 3 : 2];
    this.Textures[0] = this.RecreateRT(this.Textures[0], 24, (RenderTextureFormat) 0);
    ((Texture) this.Textures[0]).filterMode = (FilterMode) 0;
    ((Object) this.Textures[0]).name = "MRT0";
    this.Textures[1] = this.RecreateRT(this.Textures[1], 0, (RenderTextureFormat) 16);
    ((Texture) this.Textures[1]).filterMode = (FilterMode) 0;
    ((Object) this.Textures[1]).name = "MRT1";
    renderBufferArray[0] = this.Textures[0].colorBuffer;
    renderBufferArray[1] = this.Textures[1].colorBuffer;
    if (this.colouredOverlayBufferEnabled)
    {
      this.Textures[2] = this.RecreateRT(this.Textures[2], 0, (RenderTextureFormat) 0);
      ((Texture) this.Textures[2]).filterMode = (FilterMode) 1;
      ((Object) this.Textures[2]).name = "MRT2";
      renderBufferArray[2] = this.Textures[2].colorBuffer;
    }
    ((Component) this).GetComponent<Camera>().SetTargetBuffers(renderBufferArray, this.Textures[0].depthBuffer);
    this.OnShadersReloaded();
  }

  private RenderTexture RecreateRT(RenderTexture rt, int depth, RenderTextureFormat format)
  {
    RenderTexture renderTexture = rt;
    if (Object.op_Equality((Object) rt, (Object) null) || ((Texture) rt).width != Screen.width || ((Texture) rt).height != Screen.height || rt.format != format)
    {
      if (Object.op_Inequality((Object) rt, (Object) null))
        RenderTextureDestroyerExtensions.DestroyRenderTexture(rt);
      renderTexture = new RenderTexture(Screen.width, Screen.height, depth, format);
    }
    return renderTexture;
  }

  private void OnResize() => this.CreateRenderTarget();

  private void Update()
  {
    if (this.Textures[0].IsCreated())
      return;
    this.CreateRenderTarget();
  }

  private void OnShadersReloaded()
  {
    Shader.SetGlobalTexture("_MRT0", (Texture) this.Textures[0]);
    Shader.SetGlobalTexture("_MRT1", (Texture) this.Textures[1]);
    if (!this.colouredOverlayBufferEnabled)
      return;
    Shader.SetGlobalTexture("_MRT2", (Texture) this.Textures[2]);
  }
}
