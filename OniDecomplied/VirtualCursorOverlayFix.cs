// Decompiled with JetBrains decompiler
// Type: VirtualCursorOverlayFix
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VirtualCursorOverlayFix : MonoBehaviour
{
  private RenderTexture cursorRendTex;
  public Camera screenSpaceCamera;
  public Image screenSpaceOverlayImage;
  public RawImage actualCursor;

  private void Awake()
  {
    Resolution currentResolution1 = Screen.currentResolution;
    int width = ((Resolution) ref currentResolution1).width;
    Resolution currentResolution2 = Screen.currentResolution;
    int height = ((Resolution) ref currentResolution2).height;
    this.cursorRendTex = new RenderTexture(width, height, 0);
    ((Behaviour) this.screenSpaceCamera).enabled = true;
    this.screenSpaceCamera.targetTexture = this.cursorRendTex;
    ((Graphic) this.screenSpaceOverlayImage).material.SetTexture("_MainTex", (Texture) this.cursorRendTex);
    this.StartCoroutine(this.RenderVirtualCursor());
  }

  private IEnumerator RenderVirtualCursor()
  {
    bool ShowCursor = KInputManager.currentControllerIsGamepad;
    while (Application.isPlaying)
    {
      ShowCursor = KInputManager.currentControllerIsGamepad;
      if (Input.GetKey((KeyCode) 306) && Input.GetKey((KeyCode) 308) && Input.GetKey((KeyCode) 99))
        ShowCursor = true;
      ((Behaviour) this.screenSpaceCamera).enabled = true;
      if (!((Behaviour) this.screenSpaceOverlayImage).enabled & ShowCursor)
        yield return (object) SequenceUtil.WaitForSecondsRealtime(0.1f);
      ((Behaviour) this.actualCursor).enabled = ShowCursor;
      ((Behaviour) this.screenSpaceOverlayImage).enabled = ShowCursor;
      ((Graphic) this.screenSpaceOverlayImage).material.SetTexture("_MainTex", (Texture) this.cursorRendTex);
      yield return (object) null;
    }
  }
}
