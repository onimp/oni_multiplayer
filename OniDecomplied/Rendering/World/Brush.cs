// Decompiled with JetBrains decompiler
// Type: Rendering.World.Brush
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Rendering.World
{
  public class Brush
  {
    private bool dirty;
    private Material material;
    private int layer;
    private HashSet<int> tiles = new HashSet<int>();
    private List<Brush> activeBrushes;
    private List<Brush> dirtyBrushes;
    private int widthInTiles;
    private Mask mask;
    private DynamicMesh mesh;
    private MaterialPropertyBlock propertyBlock;

    public int Id { get; private set; }

    public Brush(
      int id,
      string name,
      Material material,
      Mask mask,
      List<Brush> active_brushes,
      List<Brush> dirty_brushes,
      int width_in_tiles,
      MaterialPropertyBlock property_block)
    {
      this.Id = id;
      this.material = material;
      this.mask = mask;
      this.mesh = new DynamicMesh(name, new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, 0.0f)));
      this.activeBrushes = active_brushes;
      this.dirtyBrushes = dirty_brushes;
      this.layer = LayerMask.NameToLayer("World");
      this.widthInTiles = width_in_tiles;
      this.propertyBlock = property_block;
    }

    public void Add(int tile_idx)
    {
      this.tiles.Add(tile_idx);
      if (this.dirty)
        return;
      this.dirtyBrushes.Add(this);
      this.dirty = true;
    }

    public void Remove(int tile_idx)
    {
      this.tiles.Remove(tile_idx);
      if (this.dirty)
        return;
      this.dirtyBrushes.Add(this);
      this.dirty = true;
    }

    public void SetMaskOffset(int offset) => this.mask.SetOffset(offset);

    public void Refresh()
    {
      bool flag = this.mesh.Meshes.Length != 0;
      int count = this.tiles.Count;
      this.mesh.Reserve(count * 4, count * 6);
      if (this.mesh.SetTriangles)
      {
        int triangle = 0;
        for (int index = 0; index < count; ++index)
        {
          this.mesh.AddTriangle(triangle);
          this.mesh.AddTriangle(2 + triangle);
          this.mesh.AddTriangle(1 + triangle);
          this.mesh.AddTriangle(1 + triangle);
          this.mesh.AddTriangle(2 + triangle);
          this.mesh.AddTriangle(3 + triangle);
          triangle += 4;
        }
      }
      foreach (int tile in this.tiles)
      {
        float num1 = (float) (tile % this.widthInTiles);
        float num2 = (float) (tile / this.widthInTiles);
        float num3 = 0.0f;
        this.mesh.AddVertex(new Vector3(num1 - 0.5f, num2 - 0.5f, num3));
        this.mesh.AddVertex(new Vector3(num1 + 0.5f, num2 - 0.5f, num3));
        this.mesh.AddVertex(new Vector3(num1 - 0.5f, num2 + 0.5f, num3));
        this.mesh.AddVertex(new Vector3(num1 + 0.5f, num2 + 0.5f, num3));
      }
      if (this.mesh.SetUVs)
      {
        for (int index = 0; index < count; ++index)
        {
          this.mesh.AddUV(this.mask.UV0);
          this.mesh.AddUV(this.mask.UV1);
          this.mesh.AddUV(this.mask.UV2);
          this.mesh.AddUV(this.mask.UV3);
        }
      }
      this.dirty = false;
      this.mesh.Commit();
      if (this.mesh.Meshes.Length != 0)
      {
        if (flag)
          return;
        this.activeBrushes.Add(this);
      }
      else
      {
        if (!flag)
          return;
        this.activeBrushes.Remove(this);
      }
    }

    public void Render()
    {
      Vector3 position;
      // ISSUE: explicit constructor call
      ((Vector3) ref position).\u002Ector(0.0f, 0.0f, Grid.GetLayerZ(Grid.SceneLayer.Ground));
      this.mesh.Render(position, Quaternion.identity, this.material, this.layer, this.propertyBlock);
    }

    public void SetMaterial(Material material, MaterialPropertyBlock property_block)
    {
      this.material = material;
      this.propertyBlock = property_block;
    }
  }
}
