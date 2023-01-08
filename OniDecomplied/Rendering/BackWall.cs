// Decompiled with JetBrains decompiler
// Type: rendering.BackWall
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace rendering
{
  public class BackWall : MonoBehaviour
  {
    [SerializeField]
    public Material backwallMaterial;
    [SerializeField]
    public Texture2DArray array;

    private void Awake() => this.backwallMaterial.SetTexture("images", (Texture) this.array);
  }
}
