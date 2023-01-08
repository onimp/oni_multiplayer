// Decompiled with JetBrains decompiler
// Type: Rendering.World.DynamicSubMesh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace Rendering.World
{
  public class DynamicSubMesh
  {
    public Vector3[] Vertices = new Vector3[0];
    public Vector2[] UVs = new Vector2[0];
    public int[] Triangles = new int[0];
    public Mesh Mesh;
    public bool SetUVs;
    public bool SetTriangles;
    private int VertexIdx;
    private int UVIdx;
    private int TriangleIdx;
    private int IdxOffset;

    public DynamicSubMesh(string name, Bounds bounds, int idx_offset)
    {
      this.IdxOffset = idx_offset;
      this.Mesh = new Mesh();
      ((Object) this.Mesh).name = name;
      this.Mesh.bounds = bounds;
      this.Mesh.MarkDynamic();
    }

    public void Reserve(int vertex_count, int triangle_count)
    {
      if (vertex_count > this.Vertices.Length)
      {
        this.Vertices = new Vector3[vertex_count];
        this.UVs = new Vector2[vertex_count];
        this.SetUVs = true;
      }
      else
        this.SetUVs = false;
      if (this.Triangles.Length != triangle_count)
      {
        this.Triangles = new int[triangle_count];
        this.SetTriangles = true;
      }
      else
        this.SetTriangles = false;
    }

    public bool AreTrianglesFull() => this.Triangles.Length == this.TriangleIdx;

    public bool AreVerticesFull() => this.Vertices.Length == this.VertexIdx;

    public bool AreUVsFull() => this.UVs.Length == this.UVIdx;

    public void Commit()
    {
      if (this.SetTriangles)
        this.Mesh.Clear();
      this.Mesh.vertices = this.Vertices;
      if (this.SetUVs || this.SetTriangles)
        this.Mesh.uv = this.UVs;
      if (this.SetTriangles)
        this.Mesh.triangles = this.Triangles;
      this.VertexIdx = 0;
      this.UVIdx = 0;
      this.TriangleIdx = 0;
    }

    public void AddTriangle(int triangle) => this.Triangles[this.TriangleIdx++] = triangle + this.IdxOffset;

    public void AddUV(Vector2 uv) => this.UVs[this.UVIdx++] = uv;

    public void AddVertex(Vector3 vertex) => this.Vertices[this.VertexIdx++] = vertex;

    public void Render(
      Vector3 position,
      Quaternion rotation,
      Material material,
      int layer,
      MaterialPropertyBlock property_block)
    {
      Graphics.DrawMesh(this.Mesh, position, rotation, material, layer, (Camera) null, 0, property_block, false, false);
    }
  }
}
