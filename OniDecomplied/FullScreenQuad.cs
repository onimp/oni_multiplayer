// Decompiled with JetBrains decompiler
// Type: FullScreenQuad
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FullScreenQuad
{
  private Mesh Mesh;
  private Camera Camera;
  private Material Material;
  private int Layer;

  public FullScreenQuad(string name, Camera camera, bool invert = false)
  {
    this.Camera = camera;
    this.Layer = LayerMask.NameToLayer("ForceDraw");
    this.Mesh = new Mesh();
    ((Object) this.Mesh).name = name;
    this.Mesh.vertices = new Vector3[4]
    {
      new Vector3(-1f, -1f, 0.0f),
      new Vector3(-1f, 1f, 0.0f),
      new Vector3(1f, -1f, 0.0f),
      new Vector3(1f, 1f, 0.0f)
    };
    float num1 = 1f;
    float num2 = 0.0f;
    if (invert)
    {
      num1 = 0.0f;
      num2 = 1f;
    }
    this.Mesh.uv = new Vector2[4]
    {
      new Vector2(0.0f, num2),
      new Vector2(0.0f, num1),
      new Vector2(1f, num2),
      new Vector2(1f, num1)
    };
    this.Mesh.triangles = new int[6]{ 0, 1, 2, 2, 1, 3 };
    this.Mesh.bounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
    this.Material = new Material(Shader.Find("Klei/PostFX/FullScreen"));
    this.Camera.cullingMask |= LayerMask.GetMask(new string[1]
    {
      "ForceDraw"
    });
  }

  public void Draw(Texture texture)
  {
    this.Material.mainTexture = texture;
    Graphics.DrawMesh(this.Mesh, Vector3.zero, Quaternion.identity, this.Material, this.Layer, this.Camera, 0, (MaterialPropertyBlock) null, false, false);
  }
}
