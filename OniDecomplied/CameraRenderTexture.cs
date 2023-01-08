// Decompiled with JetBrains decompiler
// Type: CameraRenderTexture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class CameraRenderTexture : MonoBehaviour
{
  public string TextureName;
  private RenderTexture resultTexture;
  private Material material;

  private void Awake() => this.material = new Material(Shader.Find("Klei/PostFX/CameraRenderTexture"));

  private void Start()
  {
    if (Object.op_Inequality((Object) ScreenResize.Instance, (Object) null))
      ScreenResize.Instance.OnResize += new System.Action(this.OnResize);
    this.OnResize();
  }

  private void OnResize()
  {
    if (Object.op_Inequality((Object) this.resultTexture, (Object) null))
      RenderTextureDestroyerExtensions.DestroyRenderTexture(this.resultTexture);
    this.resultTexture = new RenderTexture(Screen.width, Screen.height, 0, (RenderTextureFormat) 0);
    ((Object) this.resultTexture).name = ((Object) this).name;
    ((Texture) this.resultTexture).filterMode = (FilterMode) 0;
    this.resultTexture.autoGenerateMips = false;
    if (!(this.TextureName != ""))
      return;
    Shader.SetGlobalTexture(this.TextureName, (Texture) this.resultTexture);
  }

  private void OnRenderImage(RenderTexture source, RenderTexture dest) => Graphics.Blit((Texture) source, this.resultTexture, this.material);

  public RenderTexture GetTexture() => this.resultTexture;

  public bool ShouldFlip() => false;
}
