// Decompiled with JetBrains decompiler
// Type: Rendering.World.DynamicMesh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace Rendering.World
{
  public class DynamicMesh
  {
    private static int TrianglesPerMesh = 65004;
    private static int VerticesPerMesh = 4 * DynamicMesh.TrianglesPerMesh / 6;
    public bool SetUVs;
    public bool SetTriangles;
    public string Name;
    public Bounds Bounds;
    public DynamicSubMesh[] Meshes = new DynamicSubMesh[0];
    private int VertexCount;
    private int TriangleCount;
    private int VertexIdx;
    private int UVIdx;
    private int TriangleIdx;
    private int TriangleMeshIdx;
    private int VertexMeshIdx;
    private int UVMeshIdx;

    public DynamicMesh(string name, Bounds bounds)
    {
      this.Name = name;
      this.Bounds = bounds;
    }

    public void Reserve(int vertex_count, int triangle_count)
    {
      this.SetUVs = vertex_count > this.VertexCount;
      this.SetTriangles = this.TriangleCount != triangle_count;
      int length = (int) Mathf.Ceil((float) triangle_count / (float) DynamicMesh.TrianglesPerMesh);
      if (length != this.Meshes.Length)
      {
        this.Meshes = new DynamicSubMesh[length];
        for (int index = 0; index < this.Meshes.Length; ++index)
        {
          int idx_offset = -index * DynamicMesh.VerticesPerMesh;
          this.Meshes[index] = new DynamicSubMesh(this.Name, this.Bounds, idx_offset);
        }
        this.SetUVs = true;
        this.SetTriangles = true;
      }
      for (int index = 0; index < this.Meshes.Length; ++index)
      {
        if (index == this.Meshes.Length - 1)
          this.Meshes[index].Reserve(vertex_count % DynamicMesh.VerticesPerMesh, triangle_count % DynamicMesh.TrianglesPerMesh);
        else
          this.Meshes[index].Reserve(DynamicMesh.VerticesPerMesh, DynamicMesh.TrianglesPerMesh);
      }
      this.VertexCount = vertex_count;
      this.TriangleCount = triangle_count;
    }

    public void Commit()
    {
      foreach (DynamicSubMesh mesh in this.Meshes)
        mesh.Commit();
      this.TriangleMeshIdx = 0;
      this.UVMeshIdx = 0;
      this.VertexMeshIdx = 0;
    }

    public void AddTriangle(int triangle)
    {
      if (this.Meshes[this.TriangleMeshIdx].AreTrianglesFull())
      {
        DynamicSubMesh mesh = this.Meshes[++this.TriangleMeshIdx];
      }
      this.Meshes[this.TriangleMeshIdx].AddTriangle(triangle);
    }

    public void AddUV(Vector2 uv)
    {
      DynamicSubMesh mesh = this.Meshes[this.UVMeshIdx];
      if (mesh.AreUVsFull())
        mesh = this.Meshes[++this.UVMeshIdx];
      mesh.AddUV(uv);
    }

    public void AddVertex(Vector3 vertex)
    {
      DynamicSubMesh mesh = this.Meshes[this.VertexMeshIdx];
      if (mesh.AreVerticesFull())
        mesh = this.Meshes[++this.VertexMeshIdx];
      mesh.AddVertex(vertex);
    }

    public void Render(
      Vector3 position,
      Quaternion rotation,
      Material material,
      int layer,
      MaterialPropertyBlock property_block)
    {
      foreach (DynamicSubMesh mesh in this.Meshes)
        mesh.Render(position, rotation, material, layer, property_block);
    }
  }
}
