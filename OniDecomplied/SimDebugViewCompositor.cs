// Decompiled with JetBrains decompiler
// Type: SimDebugViewCompositor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class SimDebugViewCompositor : MonoBehaviour
{
  public Material material;
  public static SimDebugViewCompositor Instance;

  private void Awake() => SimDebugViewCompositor.Instance = this;

  private void OnDestroy() => SimDebugViewCompositor.Instance = (SimDebugViewCompositor) null;

  private void Start()
  {
    this.material = new Material(Shader.Find("Klei/PostFX/SimDebugViewCompositor"));
    this.Toggle(false);
  }

  private void OnRenderImage(RenderTexture src, RenderTexture dest) => Graphics.Blit((Texture) src, dest, this.material);

  public void Toggle(bool is_on) => ((Behaviour) this).enabled = is_on;
}
