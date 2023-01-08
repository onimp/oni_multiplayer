// Decompiled with JetBrains decompiler
// Type: CameraReferenceTexture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class CameraReferenceTexture : MonoBehaviour
{
  public Camera referenceCamera;
  private FullScreenQuad quad;

  private void OnPreCull()
  {
    if (this.quad == null)
      this.quad = new FullScreenQuad(nameof (CameraReferenceTexture), ((Component) this).GetComponent<Camera>(), ((Component) this.referenceCamera).GetComponent<CameraRenderTexture>().ShouldFlip());
    if (!Object.op_Inequality((Object) this.referenceCamera, (Object) null))
      return;
    this.quad.Draw((Texture) ((Component) this.referenceCamera).GetComponent<CameraRenderTexture>().GetTexture());
  }
}
