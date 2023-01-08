// Decompiled with JetBrains decompiler
// Type: GroundRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("KMonoBehaviour/scripts/GroundRenderer")]
public class GroundRenderer : KMonoBehaviour
{
  [SerializeField]
  private GroundMasks masks;
  private GroundMasks.BiomeMaskData[] biomeMasks;
  private Dictionary<SimHashes, GroundRenderer.Materials> elementMaterials = new Dictionary<SimHashes, GroundRenderer.Materials>();
  private bool[,] dirtyChunks;
  private GroundRenderer.WorldChunk[,] worldChunks;
  private const int ChunkEdgeSize = 16;
  private Vector2I size;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ShaderReloader.Register(new System.Action(this.OnShadersReloaded));
    this.OnShadersReloaded();
    this.masks.Initialize();
    SubWorld.ZoneType[] values = (SubWorld.ZoneType[]) Enum.GetValues(typeof (SubWorld.ZoneType));
    this.biomeMasks = new GroundMasks.BiomeMaskData[values.Length];
    for (int index = 0; index < values.Length; ++index)
    {
      SubWorld.ZoneType zone_type = (SubWorld.ZoneType) (int) values[index];
      this.biomeMasks[index] = this.GetBiomeMask(zone_type);
    }
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.size = new Vector2I((Grid.WidthInCells + 16 - 1) / 16, (Grid.HeightInCells + 16 - 1) / 16);
    this.dirtyChunks = new bool[this.size.x, this.size.y];
    this.worldChunks = new GroundRenderer.WorldChunk[this.size.x, this.size.y];
    for (int y = 0; y < this.size.y; ++y)
    {
      for (int x = 0; x < this.size.x; ++x)
      {
        this.worldChunks[x, y] = new GroundRenderer.WorldChunk(x, y);
        this.dirtyChunks[x, y] = true;
      }
    }
  }

  public void Render(Vector2I vis_min, Vector2I vis_max, bool forceVisibleRebuild = false)
  {
    if (!((Behaviour) this).enabled)
      return;
    int layer = LayerMask.NameToLayer("World");
    Vector2I vector2I1;
    // ISSUE: explicit constructor call
    ((Vector2I) ref vector2I1).\u002Ector(vis_min.x / 16, vis_min.y / 16);
    Vector2I vector2I2;
    // ISSUE: explicit constructor call
    ((Vector2I) ref vector2I2).\u002Ector((vis_max.x + 16 - 1) / 16, (vis_max.y + 16 - 1) / 16);
    for (int y = vector2I1.y; y < vector2I2.y; ++y)
    {
      for (int x = vector2I1.x; x < vector2I2.x; ++x)
      {
        GroundRenderer.WorldChunk worldChunk = this.worldChunks[x, y];
        if (this.dirtyChunks[x, y] | forceVisibleRebuild)
        {
          this.dirtyChunks[x, y] = false;
          worldChunk.Rebuild(this.biomeMasks, this.elementMaterials);
        }
        worldChunk.Render(layer);
      }
    }
    this.RebuildDirtyChunks();
  }

  public void RenderAll() => this.Render(new Vector2I(0, 0), new Vector2I(this.worldChunks.GetLength(0) * 16, this.worldChunks.GetLength(1) * 16), true);

  private void RebuildDirtyChunks()
  {
    for (int index1 = 0; index1 < this.dirtyChunks.GetLength(1); ++index1)
    {
      for (int index2 = 0; index2 < this.dirtyChunks.GetLength(0); ++index2)
      {
        if (this.dirtyChunks[index2, index1])
        {
          this.dirtyChunks[index2, index1] = false;
          this.worldChunks[index2, index1].Rebuild(this.biomeMasks, this.elementMaterials);
        }
      }
    }
  }

  public void MarkDirty(int cell)
  {
    Vector2I xy = Grid.CellToXY(cell);
    Vector2I vector2I;
    // ISSUE: explicit constructor call
    ((Vector2I) ref vector2I).\u002Ector(xy.x / 16, xy.y / 16);
    this.dirtyChunks[vector2I.x, vector2I.y] = true;
    int num = xy.x % 16 != 0 ? 0 : (vector2I.x > 0 ? 1 : 0);
    bool flag1 = xy.x % 16 == 15 && vector2I.x < this.size.x - 1;
    bool flag2 = xy.y % 16 == 0 && vector2I.y > 0;
    bool flag3 = xy.y % 16 == 15 && vector2I.y < this.size.y - 1;
    if (num != 0)
    {
      this.dirtyChunks[vector2I.x - 1, vector2I.y] = true;
      if (flag2)
        this.dirtyChunks[vector2I.x - 1, vector2I.y - 1] = true;
      if (flag3)
        this.dirtyChunks[vector2I.x - 1, vector2I.y + 1] = true;
    }
    if (flag2)
      this.dirtyChunks[vector2I.x, vector2I.y - 1] = true;
    if (flag3)
      this.dirtyChunks[vector2I.x, vector2I.y + 1] = true;
    if (!flag1)
      return;
    this.dirtyChunks[vector2I.x + 1, vector2I.y] = true;
    if (flag2)
      this.dirtyChunks[vector2I.x + 1, vector2I.y - 1] = true;
    if (!flag3)
      return;
    this.dirtyChunks[vector2I.x + 1, vector2I.y + 1] = true;
  }

  private Vector2I GetChunkIdx(int cell)
  {
    Vector2I xy = Grid.CellToXY(cell);
    return new Vector2I(xy.x / 16, xy.y / 16);
  }

  private GroundMasks.BiomeMaskData GetBiomeMask(SubWorld.ZoneType zone_type)
  {
    GroundMasks.BiomeMaskData biomeMask = (GroundMasks.BiomeMaskData) null;
    this.masks.biomeMasks.TryGetValue(zone_type.ToString().ToLower(), out biomeMask);
    return biomeMask;
  }

  private void InitOpaqueMaterial(Material material, Element element)
  {
    ((Object) material).name = element.id.ToString() + "_opaque";
    material.renderQueue = RenderQueues.WorldOpaque;
    material.EnableKeyword("OPAQUE");
    material.DisableKeyword("ALPHA");
    this.ConfigureMaterialShine(material);
    material.SetInt("_SrcAlpha", 1);
    material.SetInt("_DstAlpha", 0);
    material.SetInt("_ZWrite", 1);
    material.SetTexture("_AlphaTestMap", (Texture) Texture2D.whiteTexture);
  }

  private void InitAlphaMaterial(Material material, Element element)
  {
    ((Object) material).name = element.id.ToString() + "_alpha";
    material.renderQueue = RenderQueues.WorldTransparent;
    material.EnableKeyword("ALPHA");
    material.DisableKeyword("OPAQUE");
    this.ConfigureMaterialShine(material);
    material.SetTexture("_AlphaTestMap", (Texture) this.masks.maskAtlas.texture);
    material.SetInt("_SrcAlpha", 5);
    material.SetInt("_DstAlpha", 10);
    material.SetInt("_ZWrite", 0);
  }

  private void ConfigureMaterialShine(Material material)
  {
    if (Object.op_Inequality((Object) material.GetTexture("_ShineMask"), (Object) null))
    {
      material.DisableKeyword("MATTE");
      material.EnableKeyword("SHINY");
    }
    else
    {
      material.EnableKeyword("MATTE");
      material.DisableKeyword("SHINY");
    }
  }

  [ContextMenu("Reload Shaders")]
  public void OnShadersReloaded()
  {
    this.FreeMaterials();
    foreach (Element element in ElementLoader.elements)
    {
      if (element.IsSolid)
      {
        if (Object.op_Equality((Object) element.substance.material, (Object) null))
          DebugUtil.LogErrorArgs(new object[2]
          {
            (object) element.name,
            (object) "must have material associated with it in the substance table"
          });
        Material material1 = new Material(element.substance.material);
        this.InitOpaqueMaterial(material1, element);
        Material material2 = new Material(material1);
        this.InitAlphaMaterial(material2, element);
        GroundRenderer.Materials materials = new GroundRenderer.Materials(material1, material2);
        this.elementMaterials[element.id] = materials;
      }
    }
    if (this.worldChunks == null)
      return;
    for (int index1 = 0; index1 < this.dirtyChunks.GetLength(1); ++index1)
    {
      for (int index2 = 0; index2 < this.dirtyChunks.GetLength(0); ++index2)
        this.dirtyChunks[index2, index1] = true;
    }
    GroundRenderer.WorldChunk[,] worldChunks = this.worldChunks;
    int upperBound1 = worldChunks.GetUpperBound(0);
    int upperBound2 = worldChunks.GetUpperBound(1);
    for (int lowerBound1 = worldChunks.GetLowerBound(0); lowerBound1 <= upperBound1; ++lowerBound1)
    {
      for (int lowerBound2 = worldChunks.GetLowerBound(1); lowerBound2 <= upperBound2; ++lowerBound2)
      {
        GroundRenderer.WorldChunk worldChunk = worldChunks[lowerBound1, lowerBound2];
        worldChunk.Clear();
        worldChunk.Rebuild(this.biomeMasks, this.elementMaterials);
      }
    }
  }

  public void FreeResources()
  {
    this.FreeMaterials();
    this.elementMaterials.Clear();
    this.elementMaterials = (Dictionary<SimHashes, GroundRenderer.Materials>) null;
    if (this.worldChunks == null)
      return;
    GroundRenderer.WorldChunk[,] worldChunks = this.worldChunks;
    int upperBound1 = worldChunks.GetUpperBound(0);
    int upperBound2 = worldChunks.GetUpperBound(1);
    for (int lowerBound1 = worldChunks.GetLowerBound(0); lowerBound1 <= upperBound1; ++lowerBound1)
    {
      for (int lowerBound2 = worldChunks.GetLowerBound(1); lowerBound2 <= upperBound2; ++lowerBound2)
        worldChunks[lowerBound1, lowerBound2].FreeResources();
    }
    this.worldChunks = (GroundRenderer.WorldChunk[,]) null;
  }

  private void FreeMaterials()
  {
    foreach (GroundRenderer.Materials materials in this.elementMaterials.Values)
    {
      Object.Destroy((Object) materials.opaque);
      Object.Destroy((Object) materials.alpha);
    }
    this.elementMaterials.Clear();
  }

  [Serializable]
  private struct Materials
  {
    public Material opaque;
    public Material alpha;

    public Materials(Material opaque, Material alpha)
    {
      this.opaque = opaque;
      this.alpha = alpha;
    }
  }

  private class ElementChunk
  {
    public SimHashes element;
    private GroundRenderer.ElementChunk.RenderData alpha;
    private GroundRenderer.ElementChunk.RenderData opaque;
    public int tileCount;

    public ElementChunk(
      SimHashes element,
      Dictionary<SimHashes, GroundRenderer.Materials> materials)
    {
      this.element = element;
      GroundRenderer.Materials material = materials[element];
      this.alpha = new GroundRenderer.ElementChunk.RenderData(material.alpha);
      this.opaque = new GroundRenderer.ElementChunk.RenderData(material.opaque);
      this.Clear();
    }

    public void Clear()
    {
      this.opaque.Clear();
      this.alpha.Clear();
      this.tileCount = 0;
    }

    public void AddOpaqueQuad(int x, int y, GroundMasks.UVData uvs)
    {
      this.opaque.AddQuad(x, y, uvs);
      ++this.tileCount;
    }

    public void AddAlphaQuad(int x, int y, GroundMasks.UVData uvs)
    {
      this.alpha.AddQuad(x, y, uvs);
      ++this.tileCount;
    }

    public void Build()
    {
      this.opaque.Build();
      this.alpha.Build();
    }

    public void Render(int layer, int element_idx)
    {
      float num = Grid.GetLayerZ(Grid.SceneLayer.Ground) - 0.0001f * (float) element_idx;
      this.opaque.Render(new Vector3(0.0f, 0.0f, num), layer);
      this.alpha.Render(new Vector3(0.0f, 0.0f, num), layer);
    }

    public void FreeResources()
    {
      this.alpha.FreeResources();
      this.opaque.FreeResources();
      this.alpha = (GroundRenderer.ElementChunk.RenderData) null;
      this.opaque = (GroundRenderer.ElementChunk.RenderData) null;
    }

    private class RenderData
    {
      public Material material;
      public Mesh mesh;
      public List<Vector3> pos;
      public List<Vector2> uv;
      public List<int> indices;

      public RenderData(Material material)
      {
        this.material = material;
        this.mesh = new Mesh();
        this.mesh.MarkDynamic();
        ((Object) this.mesh).name = nameof (ElementChunk);
        this.pos = new List<Vector3>();
        this.uv = new List<Vector2>();
        this.indices = new List<int>();
      }

      public void ClearMesh()
      {
        if (!Object.op_Inequality((Object) this.mesh, (Object) null))
          return;
        this.mesh.Clear();
        Object.DestroyImmediate((Object) this.mesh);
        this.mesh = (Mesh) null;
      }

      public void Clear()
      {
        if (Object.op_Inequality((Object) this.mesh, (Object) null))
          this.mesh.Clear();
        if (this.pos != null)
          this.pos.Clear();
        if (this.uv != null)
          this.uv.Clear();
        if (this.indices == null)
          return;
        this.indices.Clear();
      }

      public void FreeResources()
      {
        this.ClearMesh();
        this.Clear();
        this.pos = (List<Vector3>) null;
        this.uv = (List<Vector2>) null;
        this.indices = (List<int>) null;
        this.material = (Material) null;
      }

      public void Build()
      {
        this.mesh.SetVertices(this.pos);
        this.mesh.SetUVs(0, this.uv);
        this.mesh.SetTriangles(this.indices, 0);
      }

      public void AddQuad(int x, int y, GroundMasks.UVData uvs)
      {
        int count = this.pos.Count;
        this.indices.Add(count);
        this.indices.Add(count + 1);
        this.indices.Add(count + 3);
        this.indices.Add(count);
        this.indices.Add(count + 3);
        this.indices.Add(count + 2);
        this.pos.Add(new Vector3((float) x - 0.5f, (float) y - 0.5f, 0.0f));
        this.pos.Add(new Vector3((float) ((double) x + 1.0 - 0.5), (float) y - 0.5f, 0.0f));
        this.pos.Add(new Vector3((float) x - 0.5f, (float) ((double) y + 1.0 - 0.5), 0.0f));
        this.pos.Add(new Vector3((float) ((double) x + 1.0 - 0.5), (float) ((double) y + 1.0 - 0.5), 0.0f));
        this.uv.Add(uvs.bl);
        this.uv.Add(uvs.br);
        this.uv.Add(uvs.tl);
        this.uv.Add(uvs.tr);
      }

      public void Render(Vector3 position, int layer)
      {
        if (this.pos.Count == 0)
          return;
        Graphics.DrawMesh(this.mesh, position, Quaternion.identity, this.material, layer, (Camera) null, 0, (MaterialPropertyBlock) null, (ShadowCastingMode) 0, false, (Transform) null, false);
      }
    }
  }

  private struct WorldChunk
  {
    public readonly int chunkX;
    public readonly int chunkY;
    private List<GroundRenderer.ElementChunk> elementChunks;
    private static Element[] elements = new Element[4];
    private static Element[] uniqueElements = new Element[4];
    private static int[] substances = new int[4];
    private static Vector2 NoiseScale = Vector2.op_Implicit(new Vector3(1f, 1f));

    public WorldChunk(int x, int y)
    {
      this.chunkX = x;
      this.chunkY = y;
      this.elementChunks = new List<GroundRenderer.ElementChunk>();
    }

    public void Clear() => this.elementChunks.Clear();

    private static void InsertSorted(Element element, Element[] array, int size)
    {
      int id = (int) element.id;
      for (int index = 0; index < size; ++index)
      {
        Element element1 = array[index];
        if (element1.id > (SimHashes) id)
        {
          array[index] = element;
          element = element1;
          id = (int) element1.id;
        }
      }
      array[size] = element;
    }

    public void Rebuild(
      GroundMasks.BiomeMaskData[] biomeMasks,
      Dictionary<SimHashes, GroundRenderer.Materials> materials)
    {
      foreach (GroundRenderer.ElementChunk elementChunk in this.elementChunks)
        elementChunk.Clear();
      Vector2I vector2I1;
      // ISSUE: explicit constructor call
      ((Vector2I) ref vector2I1).\u002Ector(this.chunkX * 16, this.chunkY * 16);
      Vector2I vector2I2;
      // ISSUE: explicit constructor call
      ((Vector2I) ref vector2I2).\u002Ector(Math.Min(Grid.WidthInCells, (this.chunkX + 1) * 16), Math.Min(Grid.HeightInCells, (this.chunkY + 1) * 16));
      for (int y = vector2I1.y; y < vector2I2.y; ++y)
      {
        int num1 = Math.Max(0, y - 1);
        int num2 = y;
        for (int x = vector2I1.x; x < vector2I2.x; ++x)
        {
          int num3 = Math.Max(0, x - 1);
          int num4 = x;
          int i1 = num1 * Grid.WidthInCells + num3;
          int i2 = num1 * Grid.WidthInCells + num4;
          int i3 = num2 * Grid.WidthInCells + num3;
          int i4 = num2 * Grid.WidthInCells + num4;
          GroundRenderer.WorldChunk.elements[0] = Grid.Element[i1];
          GroundRenderer.WorldChunk.elements[1] = Grid.Element[i2];
          GroundRenderer.WorldChunk.elements[2] = Grid.Element[i3];
          GroundRenderer.WorldChunk.elements[3] = Grid.Element[i4];
          GroundRenderer.WorldChunk.substances[0] = !Grid.RenderedByWorld[i1] || !GroundRenderer.WorldChunk.elements[0].IsSolid ? -1 : GroundRenderer.WorldChunk.elements[0].substance.idx;
          GroundRenderer.WorldChunk.substances[1] = !Grid.RenderedByWorld[i2] || !GroundRenderer.WorldChunk.elements[1].IsSolid ? -1 : GroundRenderer.WorldChunk.elements[1].substance.idx;
          GroundRenderer.WorldChunk.substances[2] = !Grid.RenderedByWorld[i3] || !GroundRenderer.WorldChunk.elements[2].IsSolid ? -1 : GroundRenderer.WorldChunk.elements[2].substance.idx;
          GroundRenderer.WorldChunk.substances[3] = !Grid.RenderedByWorld[i4] || !GroundRenderer.WorldChunk.elements[3].IsSolid ? -1 : GroundRenderer.WorldChunk.elements[3].substance.idx;
          GroundRenderer.WorldChunk.uniqueElements[0] = GroundRenderer.WorldChunk.elements[0];
          GroundRenderer.WorldChunk.InsertSorted(GroundRenderer.WorldChunk.elements[1], GroundRenderer.WorldChunk.uniqueElements, 1);
          GroundRenderer.WorldChunk.InsertSorted(GroundRenderer.WorldChunk.elements[2], GroundRenderer.WorldChunk.uniqueElements, 2);
          GroundRenderer.WorldChunk.InsertSorted(GroundRenderer.WorldChunk.elements[3], GroundRenderer.WorldChunk.uniqueElements, 3);
          int num5 = -1;
          int biomeIdx = GroundRenderer.WorldChunk.GetBiomeIdx(y * Grid.WidthInCells + x);
          GroundMasks.BiomeMaskData biomeMaskData = biomeMasks[biomeIdx] ?? biomeMasks[3];
          for (int index1 = 0; index1 < GroundRenderer.WorldChunk.uniqueElements.Length; ++index1)
          {
            Element uniqueElement = GroundRenderer.WorldChunk.uniqueElements[index1];
            if (uniqueElement.IsSolid)
            {
              int idx = uniqueElement.substance.idx;
              if (idx != num5)
              {
                num5 = idx;
                int index2 = (GroundRenderer.WorldChunk.substances[2] >= idx ? 1 : 0) << 3 | (GroundRenderer.WorldChunk.substances[3] >= idx ? 1 : 0) << 2 | (GroundRenderer.WorldChunk.substances[0] >= idx ? 1 : 0) << 1 | (GroundRenderer.WorldChunk.substances[1] >= idx ? 1 : 0);
                if (index2 > 0)
                {
                  GroundMasks.UVData[] variationUvs = biomeMaskData.tiles[index2].variationUVs;
                  float staticRandom = GroundRenderer.WorldChunk.GetStaticRandom(x, y);
                  int num6 = Mathf.Min(variationUvs.Length - 1, (int) ((double) variationUvs.Length * (double) staticRandom));
                  GroundMasks.UVData uvs = variationUvs[num6 % variationUvs.Length];
                  GroundRenderer.ElementChunk elementChunk = this.GetElementChunk(uniqueElement.id, materials);
                  if (index2 == 15)
                    elementChunk.AddOpaqueQuad(x, y, uvs);
                  else
                    elementChunk.AddAlphaQuad(x, y, uvs);
                }
              }
            }
          }
        }
      }
      foreach (GroundRenderer.ElementChunk elementChunk in this.elementChunks)
        elementChunk.Build();
      for (int index3 = this.elementChunks.Count - 1; index3 >= 0; --index3)
      {
        if (this.elementChunks[index3].tileCount == 0)
        {
          int index4 = this.elementChunks.Count - 1;
          this.elementChunks[index3] = this.elementChunks[index4];
          this.elementChunks.RemoveAt(index4);
        }
      }
    }

    private GroundRenderer.ElementChunk GetElementChunk(
      SimHashes elementID,
      Dictionary<SimHashes, GroundRenderer.Materials> materials)
    {
      GroundRenderer.ElementChunk elementChunk = (GroundRenderer.ElementChunk) null;
      for (int index = 0; index < this.elementChunks.Count; ++index)
      {
        if (this.elementChunks[index].element == elementID)
        {
          elementChunk = this.elementChunks[index];
          break;
        }
      }
      if (elementChunk == null)
      {
        elementChunk = new GroundRenderer.ElementChunk(elementID, materials);
        this.elementChunks.Add(elementChunk);
      }
      return elementChunk;
    }

    private static int GetBiomeIdx(int cell)
    {
      if (!Grid.IsValidCell(cell))
        return 0;
      int biomeIdx = 3;
      if (!Object.op_Inequality((Object) World.Instance, (Object) null) || !Object.op_Inequality((Object) World.Instance.zoneRenderData, (Object) null))
        return biomeIdx;
      World.Instance.zoneRenderData.GetSubWorldZoneType(cell);
      return biomeIdx;
    }

    private static float GetStaticRandom(int x, int y) => PerlinSimplexNoise.noise((float) x * GroundRenderer.WorldChunk.NoiseScale.x, (float) y * GroundRenderer.WorldChunk.NoiseScale.y);

    public void Render(int layer)
    {
      for (int index = 0; index < this.elementChunks.Count; ++index)
      {
        GroundRenderer.ElementChunk elementChunk = this.elementChunks[index];
        elementChunk.Render(layer, ElementLoader.FindElementByHash(elementChunk.element).substance.idx);
      }
    }

    public void FreeResources()
    {
      foreach (GroundRenderer.ElementChunk elementChunk in this.elementChunks)
        elementChunk.FreeResources();
      this.elementChunks.Clear();
      this.elementChunks = (List<GroundRenderer.ElementChunk>) null;
    }
  }
}
