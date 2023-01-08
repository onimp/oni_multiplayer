// Decompiled with JetBrains decompiler
// Type: MultipleRenderTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

public class MultipleRenderTarget : MonoBehaviour
{
  private MultipleRenderTargetProxy renderProxy;
  private FullScreenQuad quad;
  public bool isFrontEnd;

  public event Action<Camera> onSetupComplete;

  private void Start() => this.StartCoroutine(this.SetupProxy());

  private IEnumerator SetupProxy()
  {
    MultipleRenderTarget multipleRenderTarget = this;
    yield return (object) null;
    Camera component = ((Component) multipleRenderTarget).GetComponent<Camera>();
    Camera camera = new GameObject().AddComponent<Camera>();
    camera.CopyFrom(component);
    multipleRenderTarget.renderProxy = ((Component) camera).gameObject.AddComponent<MultipleRenderTargetProxy>();
    ((Object) camera).name = ((Object) component).name + " MRT";
    ((Component) camera).transform.parent = ((Component) component).transform;
    TransformExtensions.SetLocalPosition(((Component) camera).transform, Vector3.zero);
    camera.depth = component.depth - 1f;
    component.cullingMask = 0;
    component.clearFlags = (CameraClearFlags) 2;
    multipleRenderTarget.quad = new FullScreenQuad(nameof (MultipleRenderTarget), component, true);
    if (multipleRenderTarget.onSetupComplete != null)
      multipleRenderTarget.onSetupComplete(camera);
  }

  private void OnPreCull()
  {
    if (!Object.op_Inequality((Object) this.renderProxy, (Object) null))
      return;
    this.quad.Draw((Texture) this.renderProxy.Textures[0]);
  }

  public void ToggleColouredOverlayView(bool enabled)
  {
    if (!Object.op_Inequality((Object) this.renderProxy, (Object) null))
      return;
    this.renderProxy.ToggleColouredOverlayView(enabled);
  }
}
