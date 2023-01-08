// Decompiled with JetBrains decompiler
// Type: FixGraphicsCorruption
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FixGraphicsCorruption : MonoBehaviour
{
  private void Start()
  {
    Camera component = ((Component) this).GetComponent<Camera>();
    component.transparencySortMode = (TransparencySortMode) 2;
    ((Component) component).tag = "Untagged";
  }

  private void OnRenderImage(RenderTexture source, RenderTexture dest) => Graphics.Blit((Texture) source, dest);
}
