// Decompiled with JetBrains decompiler
// Type: FogOfWarPostFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FogOfWarPostFX : MonoBehaviour
{
  [SerializeField]
  private Shader shader;
  private Material material;
  private Camera myCamera;

  private void Awake()
  {
    if (!Object.op_Inequality((Object) this.shader, (Object) null))
      return;
    this.material = new Material(this.shader);
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    this.SetupUVs();
    Graphics.Blit((Texture) source, destination, this.material, 0);
  }

  private void SetupUVs()
  {
    if (Object.op_Equality((Object) this.myCamera, (Object) null))
    {
      this.myCamera = ((Component) this).GetComponent<Camera>();
      if (Object.op_Equality((Object) this.myCamera, (Object) null))
        return;
    }
    Ray ray = this.myCamera.ViewportPointToRay(Vector3.zero);
    float num1 = Mathf.Abs(((Ray) ref ray).origin.z / ((Ray) ref ray).direction.z);
    Vector3 point1 = ((Ray) ref ray).GetPoint(num1);
    Vector4 vector4;
    vector4.x = point1.x / Grid.WidthInMeters;
    vector4.y = point1.y / Grid.HeightInMeters;
    ray = this.myCamera.ViewportPointToRay(Vector3.one);
    float num2 = Mathf.Abs(((Ray) ref ray).origin.z / ((Ray) ref ray).direction.z);
    Vector3 point2 = ((Ray) ref ray).GetPoint(num2);
    vector4.z = point2.x / Grid.WidthInMeters - vector4.x;
    vector4.w = point2.y / Grid.HeightInMeters - vector4.y;
    this.material.SetVector("_UVOffsetScale", vector4);
  }
}
