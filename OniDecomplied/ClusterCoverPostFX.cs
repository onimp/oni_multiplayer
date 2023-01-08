// Decompiled with JetBrains decompiler
// Type: ClusterCoverPostFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ClusterCoverPostFX : MonoBehaviour
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
    ray = this.myCamera.ViewportPointToRay(Vector3.one);
    float num2 = Mathf.Abs(((Ray) ref ray).origin.z / ((Ray) ref ray).direction.z);
    Vector3 point2 = ((Ray) ref ray).GetPoint(num2);
    Vector4 vector4_1;
    vector4_1.x = point1.x;
    vector4_1.y = point1.y;
    vector4_1.z = point2.x - point1.x;
    vector4_1.w = point2.y - point1.y;
    this.material.SetVector("_CameraCoords", vector4_1);
    Vector4 vector4_2;
    if (Object.op_Inequality((Object) ClusterManager.Instance, (Object) null) && !CameraController.Instance.ignoreClusterFX)
    {
      WorldContainer activeWorld = ClusterManager.Instance.activeWorld;
      Vector2I worldOffset = activeWorld.WorldOffset;
      Vector2I worldSize = activeWorld.WorldSize;
      // ISSUE: explicit constructor call
      ((Vector4) ref vector4_2).\u002Ector((float) worldOffset.x, (float) worldOffset.y, (float) worldSize.x, (float) worldSize.y);
      this.material.SetFloat("_HideSurface", ClusterManager.Instance.activeWorld.FullyEnclosedBorder ? 1f : 0.0f);
    }
    else
    {
      // ISSUE: explicit constructor call
      ((Vector4) ref vector4_2).\u002Ector(0.0f, 0.0f, (float) Grid.WidthInCells, (float) Grid.HeightInCells);
      this.material.SetFloat("_HideSurface", 0.0f);
    }
    this.material.SetVector("_WorldCoords", vector4_2);
  }
}
