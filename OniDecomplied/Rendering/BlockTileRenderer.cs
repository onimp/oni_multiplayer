// Decompiled with JetBrains decompiler
// Type: Rendering.BlockTileRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering
{
  public class BlockTileRenderer : MonoBehaviour
  {
    [SerializeField]
    private bool forceRebuild;
    [SerializeField]
    private Color highlightColour = new Color(1.25f, 1.25f, 1.25f, 1f);
    [SerializeField]
    private Color selectColour = new Color(1.5f, 1.5f, 1.5f, 1f);
    [SerializeField]
    private Color invalidPlaceColour = Color.red;
    private const float TILE_ATLAS_WIDTH = 2048f;
    private const float TILE_ATLAS_HEIGHT = 2048f;
    private const int chunkEdgeSize = 16;
    protected Dictionary<KeyValuePair<BuildingDef, BlockTileRenderer.RenderInfoLayer>, BlockTileRenderer.RenderInfo> renderInfo = new Dictionary<KeyValuePair<BuildingDef, BlockTileRenderer.RenderInfoLayer>, BlockTileRenderer.RenderInfo>();
    private int selectedCell = -1;
    private int highlightCell = -1;
    private int invalidPlaceCell = -1;

    public static BlockTileRenderer.RenderInfoLayer GetRenderInfoLayer(
      bool isReplacement,
      SimHashes element)
    {
      if (isReplacement)
        return BlockTileRenderer.RenderInfoLayer.Replacement;
      return element == SimHashes.Void ? BlockTileRenderer.RenderInfoLayer.UnderConstruction : BlockTileRenderer.RenderInfoLayer.Built;
    }

    public bool ForceRebuild => this.forceRebuild;

    public BlockTileRenderer() => this.forceRebuild = false;

    public void FreeResources()
    {
      foreach (KeyValuePair<KeyValuePair<BuildingDef, BlockTileRenderer.RenderInfoLayer>, BlockTileRenderer.RenderInfo> keyValuePair in this.renderInfo)
      {
        if (keyValuePair.Value != null)
          keyValuePair.Value.FreeResources();
      }
      this.renderInfo.Clear();
    }

    private static bool MatchesDef(GameObject go, BuildingDef def) => Object.op_Inequality((Object) go, (Object) null) && Object.op_Equality((Object) go.GetComponent<Building>().Def, (Object) def);

    public virtual BlockTileRenderer.Bits GetConnectionBits(int x, int y, int query_layer)
    {
      BlockTileRenderer.Bits connectionBits = (BlockTileRenderer.Bits) 0;
      GameObject gameObject = Grid.Objects[y * Grid.WidthInCells + x, query_layer];
      BuildingDef def = Object.op_Inequality((Object) gameObject, (Object) null) ? gameObject.GetComponent<Building>().Def : (BuildingDef) null;
      if (y > 0)
      {
        int cell = (y - 1) * Grid.WidthInCells + x;
        if (x > 0 && BlockTileRenderer.MatchesDef(Grid.Objects[cell - 1, query_layer], def))
          connectionBits |= BlockTileRenderer.Bits.DownLeft;
        if (BlockTileRenderer.MatchesDef(Grid.Objects[cell, query_layer], def))
          connectionBits |= BlockTileRenderer.Bits.Down;
        if (x < Grid.WidthInCells - 1 && BlockTileRenderer.MatchesDef(Grid.Objects[cell + 1, query_layer], def))
          connectionBits |= BlockTileRenderer.Bits.DownRight;
      }
      int num = y * Grid.WidthInCells + x;
      if (x > 0 && BlockTileRenderer.MatchesDef(Grid.Objects[num - 1, query_layer], def))
        connectionBits |= BlockTileRenderer.Bits.Left;
      if (x < Grid.WidthInCells - 1 && BlockTileRenderer.MatchesDef(Grid.Objects[num + 1, query_layer], def))
        connectionBits |= BlockTileRenderer.Bits.Right;
      if (y < Grid.HeightInCells - 1)
      {
        int cell = (y + 1) * Grid.WidthInCells + x;
        if (x > 0 && BlockTileRenderer.MatchesDef(Grid.Objects[cell - 1, query_layer], def))
          connectionBits |= BlockTileRenderer.Bits.UpLeft;
        if (BlockTileRenderer.MatchesDef(Grid.Objects[cell, query_layer], def))
          connectionBits |= BlockTileRenderer.Bits.Up;
        if (x < Grid.WidthInCells + 1 && BlockTileRenderer.MatchesDef(Grid.Objects[cell + 1, query_layer], def))
          connectionBits |= BlockTileRenderer.Bits.UpRight;
      }
      return connectionBits;
    }

    private bool IsDecorConnectable(GameObject src, GameObject target)
    {
      if (Object.op_Inequality((Object) src, (Object) null) && Object.op_Inequality((Object) target, (Object) null))
      {
        IBlockTileInfo component1 = src.GetComponent<IBlockTileInfo>();
        IBlockTileInfo component2 = target.GetComponent<IBlockTileInfo>();
        if (component1 != null && component2 != null)
          return component1.GetBlockTileConnectorID() == component2.GetBlockTileConnectorID();
      }
      return false;
    }

    public virtual BlockTileRenderer.Bits GetDecorConnectionBits(int x, int y, int query_layer)
    {
      BlockTileRenderer.Bits decorConnectionBits = (BlockTileRenderer.Bits) 0;
      GameObject src = Grid.Objects[y * Grid.WidthInCells + x, query_layer];
      if (y > 0)
      {
        int cell = (y - 1) * Grid.WidthInCells + x;
        if (x > 0 && Object.op_Inequality((Object) Grid.Objects[cell - 1, query_layer], (Object) null))
          decorConnectionBits |= BlockTileRenderer.Bits.DownLeft;
        if (Object.op_Inequality((Object) Grid.Objects[cell, query_layer], (Object) null))
          decorConnectionBits |= BlockTileRenderer.Bits.Down;
        if (x < Grid.WidthInCells - 1 && Object.op_Inequality((Object) Grid.Objects[cell + 1, query_layer], (Object) null))
          decorConnectionBits |= BlockTileRenderer.Bits.DownRight;
      }
      int num = y * Grid.WidthInCells + x;
      if (x > 0 && this.IsDecorConnectable(src, Grid.Objects[num - 1, query_layer]))
        decorConnectionBits |= BlockTileRenderer.Bits.Left;
      if (x < Grid.WidthInCells - 1 && this.IsDecorConnectable(src, Grid.Objects[num + 1, query_layer]))
        decorConnectionBits |= BlockTileRenderer.Bits.Right;
      if (y < Grid.HeightInCells - 1)
      {
        int cell = (y + 1) * Grid.WidthInCells + x;
        if (x > 0 && Object.op_Inequality((Object) Grid.Objects[cell - 1, query_layer], (Object) null))
          decorConnectionBits |= BlockTileRenderer.Bits.UpLeft;
        if (Object.op_Inequality((Object) Grid.Objects[cell, query_layer], (Object) null))
          decorConnectionBits |= BlockTileRenderer.Bits.Up;
        if (x < Grid.WidthInCells + 1 && Object.op_Inequality((Object) Grid.Objects[cell + 1, query_layer], (Object) null))
          decorConnectionBits |= BlockTileRenderer.Bits.UpRight;
      }
      return decorConnectionBits;
    }

    public void LateUpdate() => this.Render();

    private void Render()
    {
      Vector2I vector2I1;
      Vector2I vector2I2;
      if (GameUtil.IsCapturingTimeLapse())
      {
        // ISSUE: explicit constructor call
        ((Vector2I) ref vector2I1).\u002Ector(0, 0);
        // ISSUE: explicit constructor call
        ((Vector2I) ref vector2I2).\u002Ector(Grid.WidthInCells / 16, Grid.HeightInCells / 16);
      }
      else
      {
        GridArea visibleArea = GridVisibleArea.GetVisibleArea();
        // ISSUE: explicit constructor call
        ((Vector2I) ref vector2I1).\u002Ector(visibleArea.Min.x / 16, visibleArea.Min.y / 16);
        // ISSUE: explicit constructor call
        ((Vector2I) ref vector2I2).\u002Ector((visibleArea.Max.x + 16 - 1) / 16, (visibleArea.Max.y + 16 - 1) / 16);
      }
      foreach (KeyValuePair<KeyValuePair<BuildingDef, BlockTileRenderer.RenderInfoLayer>, BlockTileRenderer.RenderInfo> keyValuePair in this.renderInfo)
      {
        BlockTileRenderer.RenderInfo renderInfo = keyValuePair.Value;
        for (int y = vector2I1.y; y < vector2I2.y; ++y)
        {
          for (int x = vector2I1.x; x < vector2I2.x; ++x)
          {
            renderInfo.Rebuild(this, x, y, MeshUtil.vertices, MeshUtil.uvs, MeshUtil.indices, MeshUtil.colours);
            renderInfo.Render(x, y);
          }
        }
      }
    }

    public Color GetCellColour(int cell, SimHashes element) => cell != this.selectedCell ? (cell != this.invalidPlaceCell || element != SimHashes.Void ? (cell != this.highlightCell ? Color.white : this.highlightColour) : this.invalidPlaceColour) : this.selectColour;

    public static Vector2I GetChunkIdx(int cell)
    {
      Vector2I xy = Grid.CellToXY(cell);
      return new Vector2I(xy.x / 16, xy.y / 16);
    }

    public void AddBlock(
      int renderLayer,
      BuildingDef def,
      bool isReplacement,
      SimHashes element,
      int cell)
    {
      KeyValuePair<BuildingDef, BlockTileRenderer.RenderInfoLayer> key = new KeyValuePair<BuildingDef, BlockTileRenderer.RenderInfoLayer>(def, BlockTileRenderer.GetRenderInfoLayer(isReplacement, element));
      BlockTileRenderer.RenderInfo renderInfo;
      if (!this.renderInfo.TryGetValue(key, out renderInfo))
      {
        renderInfo = new BlockTileRenderer.RenderInfo(this, isReplacement ? (int) def.ReplacementLayer : (int) def.TileLayer, renderLayer, def, element);
        this.renderInfo[key] = renderInfo;
      }
      renderInfo.AddCell(cell);
    }

    public void RemoveBlock(BuildingDef def, bool isReplacement, SimHashes element, int cell)
    {
      BlockTileRenderer.RenderInfo renderInfo;
      if (!this.renderInfo.TryGetValue(new KeyValuePair<BuildingDef, BlockTileRenderer.RenderInfoLayer>(def, BlockTileRenderer.GetRenderInfoLayer(isReplacement, element)), out renderInfo))
        return;
      renderInfo.RemoveCell(cell);
    }

    public void Rebuild(ObjectLayer layer, int cell)
    {
      foreach (KeyValuePair<KeyValuePair<BuildingDef, BlockTileRenderer.RenderInfoLayer>, BlockTileRenderer.RenderInfo> keyValuePair in this.renderInfo)
      {
        if (keyValuePair.Key.Key.TileLayer == layer)
          keyValuePair.Value.MarkDirty(cell);
      }
    }

    public void SelectCell(int cell, bool enabled) => this.UpdateCellStatus(ref this.selectedCell, cell, enabled);

    public void HighlightCell(int cell, bool enabled) => this.UpdateCellStatus(ref this.highlightCell, cell, enabled);

    public void SetInvalidPlaceCell(int cell, bool enabled) => this.UpdateCellStatus(ref this.invalidPlaceCell, cell, enabled);

    private void UpdateCellStatus(ref int cell_status, int cell, bool enabled)
    {
      if (enabled)
      {
        if (cell == cell_status)
          return;
        if (cell_status != -1)
        {
          foreach (KeyValuePair<KeyValuePair<BuildingDef, BlockTileRenderer.RenderInfoLayer>, BlockTileRenderer.RenderInfo> keyValuePair in this.renderInfo)
            keyValuePair.Value.MarkDirtyIfOccupied(cell_status);
        }
        cell_status = cell;
        foreach (KeyValuePair<KeyValuePair<BuildingDef, BlockTileRenderer.RenderInfoLayer>, BlockTileRenderer.RenderInfo> keyValuePair in this.renderInfo)
          keyValuePair.Value.MarkDirtyIfOccupied(cell_status);
      }
      else
      {
        if (cell_status != cell)
          return;
        foreach (KeyValuePair<KeyValuePair<BuildingDef, BlockTileRenderer.RenderInfoLayer>, BlockTileRenderer.RenderInfo> keyValuePair in this.renderInfo)
          keyValuePair.Value.MarkDirty(cell_status);
        cell_status = -1;
      }
    }

    public enum RenderInfoLayer
    {
      Built,
      UnderConstruction,
      Replacement,
    }

    [Flags]
    public enum Bits
    {
      UpLeft = 128, // 0x00000080
      Up = 64, // 0x00000040
      UpRight = 32, // 0x00000020
      Left = 16, // 0x00000010
      Right = 8,
      DownLeft = 4,
      Down = 2,
      DownRight = 1,
    }

    protected class RenderInfo
    {
      private BlockTileRenderer.RenderInfo.AtlasInfo[] atlasInfo;
      private bool[,] dirtyChunks;
      private int queryLayer;
      private Material material;
      private int renderLayer;
      private Mesh[,] meshChunks;
      private BlockTileRenderer.DecorRenderInfo decorRenderInfo;
      private Vector2 trimUVSize;
      private Vector3 rootPosition;
      private Dictionary<int, int> occupiedCells = new Dictionary<int, int>();
      private SimHashes element;
      private float decorZOffset = -1f;
      private const float scale = 0.5f;
      private const float core_size = 256f;
      private const float trim_size = 64f;
      private const float cell_size = 1f;
      private const float world_trim_size = 0.25f;

      public RenderInfo(
        BlockTileRenderer renderer,
        int queryLayer,
        int renderLayer,
        BuildingDef def,
        SimHashes element)
      {
        this.queryLayer = queryLayer;
        this.renderLayer = renderLayer;
        this.rootPosition = new Vector3(0.0f, 0.0f, Grid.GetLayerZ(def.SceneLayer));
        this.element = element;
        this.material = new Material(def.BlockTileMaterial);
        if (def.BlockTileIsTransparent)
        {
          this.material.renderQueue = RenderQueues.Liquid;
          this.decorZOffset = (float) ((double) Grid.GetLayerZ(Grid.SceneLayer.TileFront) - (double) Grid.GetLayerZ(Grid.SceneLayer.Liquid) - 1.0);
        }
        else if (def.SceneLayer == Grid.SceneLayer.TileMain)
          this.material.renderQueue = RenderQueues.BlockTiles;
        this.material.DisableKeyword("ENABLE_SHINE");
        if (element != SimHashes.Void)
        {
          this.material.SetTexture("_MainTex", (Texture) def.BlockTileAtlas.texture);
          ((Object) this.material).name = ((Object) def.BlockTileAtlas).name + "Mat";
          if (Object.op_Inequality((Object) def.BlockTileShineAtlas, (Object) null))
          {
            this.material.SetTexture("_SpecularTex", (Texture) def.BlockTileShineAtlas.texture);
            this.material.EnableKeyword("ENABLE_SHINE");
          }
        }
        else
        {
          this.material.SetTexture("_MainTex", (Texture) def.BlockTilePlaceAtlas.texture);
          ((Object) this.material).name = ((Object) def.BlockTilePlaceAtlas).name + "Mat";
        }
        int num_x_chunks = Grid.WidthInCells / 16 + 1;
        int num_y_chunks = Grid.HeightInCells / 16 + 1;
        this.meshChunks = new Mesh[num_x_chunks, num_y_chunks];
        this.dirtyChunks = new bool[num_x_chunks, num_y_chunks];
        for (int index1 = 0; index1 < num_y_chunks; ++index1)
        {
          for (int index2 = 0; index2 < num_x_chunks; ++index2)
            this.dirtyChunks[index2, index1] = true;
        }
        BlockTileDecorInfo decorInfo = element == SimHashes.Void ? def.DecorPlaceBlockTileInfo : def.DecorBlockTileInfo;
        if (Object.op_Implicit((Object) decorInfo))
          this.decorRenderInfo = new BlockTileRenderer.DecorRenderInfo(num_x_chunks, num_y_chunks, queryLayer, def, decorInfo);
        int startIndex1 = def.BlockTileAtlas.items[0].name.Length - 4 - 8;
        int startIndex2 = startIndex1 - 1 - 8;
        this.atlasInfo = new BlockTileRenderer.RenderInfo.AtlasInfo[def.BlockTileAtlas.items.Length];
        for (int index = 0; index < this.atlasInfo.Length; ++index)
        {
          TextureAtlas.Item obj = def.BlockTileAtlas.items[index];
          string str1 = obj.name.Substring(startIndex2, 8);
          string str2 = obj.name.Substring(startIndex1, 8);
          int int32_1 = Convert.ToInt32(str1, 2);
          int int32_2 = Convert.ToInt32(str2, 2);
          this.atlasInfo[index].requiredConnections = (BlockTileRenderer.Bits) int32_1;
          this.atlasInfo[index].forbiddenConnections = (BlockTileRenderer.Bits) int32_2;
          this.atlasInfo[index].uvBox = obj.uvBox;
          this.atlasInfo[index].name = obj.name;
        }
        this.trimUVSize = new Vector2(1f / 32f, 1f / 32f);
      }

      public void FreeResources()
      {
        Object.DestroyImmediate((Object) this.material);
        this.material = (Material) null;
        this.atlasInfo = (BlockTileRenderer.RenderInfo.AtlasInfo[]) null;
        for (int index1 = 0; index1 < this.meshChunks.GetLength(0); ++index1)
        {
          for (int index2 = 0; index2 < this.meshChunks.GetLength(1); ++index2)
          {
            if (Object.op_Inequality((Object) this.meshChunks[index1, index2], (Object) null))
            {
              Object.DestroyImmediate((Object) this.meshChunks[index1, index2]);
              this.meshChunks[index1, index2] = (Mesh) null;
            }
          }
        }
        this.meshChunks = (Mesh[,]) null;
        this.decorRenderInfo = (BlockTileRenderer.DecorRenderInfo) null;
        this.occupiedCells.Clear();
      }

      public void AddCell(int cell)
      {
        int num = 0;
        this.occupiedCells.TryGetValue(cell, out num);
        this.occupiedCells[cell] = num + 1;
        this.MarkDirty(cell);
      }

      public void RemoveCell(int cell)
      {
        int num = 0;
        this.occupiedCells.TryGetValue(cell, out num);
        if (num > 1)
          this.occupiedCells[cell] = num - 1;
        else
          this.occupiedCells.Remove(cell);
        this.MarkDirty(cell);
      }

      public void MarkDirty(int cell)
      {
        Vector2I chunkIdx = BlockTileRenderer.GetChunkIdx(cell);
        this.dirtyChunks[chunkIdx.x, chunkIdx.y] = true;
      }

      public void MarkDirtyIfOccupied(int cell)
      {
        if (!this.occupiedCells.ContainsKey(cell))
          return;
        this.MarkDirty(cell);
      }

      public void Render(int x, int y)
      {
        if (Object.op_Inequality((Object) this.meshChunks[x, y], (Object) null))
          Graphics.DrawMesh(this.meshChunks[x, y], this.rootPosition, Quaternion.identity, this.material, this.renderLayer);
        if (this.decorRenderInfo == null)
          return;
        this.decorRenderInfo.Render(x, y, Vector3.op_Subtraction(this.rootPosition, new Vector3(0.0f, 0.0f, 0.5f)), this.renderLayer);
      }

      public void Rebuild(
        BlockTileRenderer renderer,
        int chunk_x,
        int chunk_y,
        List<Vector3> vertices,
        List<Vector2> uvs,
        List<int> indices,
        List<Color> colours)
      {
        if (!this.dirtyChunks[chunk_x, chunk_y] && !renderer.ForceRebuild)
          return;
        this.dirtyChunks[chunk_x, chunk_y] = false;
        vertices.Clear();
        uvs.Clear();
        indices.Clear();
        colours.Clear();
        for (int y = chunk_y * 16; y < chunk_y * 16 + 16; ++y)
        {
          for (int x = chunk_x * 16; x < chunk_x * 16 + 16; ++x)
          {
            int num1 = y * Grid.WidthInCells + x;
            if (this.occupiedCells.ContainsKey(num1))
            {
              BlockTileRenderer.Bits connectionBits = renderer.GetConnectionBits(x, y, this.queryLayer);
              for (int index = 0; index < this.atlasInfo.Length; ++index)
              {
                int num2 = (this.atlasInfo[index].requiredConnections & connectionBits) == this.atlasInfo[index].requiredConnections ? 1 : 0;
                bool flag = (this.atlasInfo[index].forbiddenConnections & connectionBits) != 0;
                if (num2 != 0 && !flag)
                {
                  Color cellColour = renderer.GetCellColour(num1, this.element);
                  this.AddVertexInfo(this.atlasInfo[index], this.trimUVSize, x, y, connectionBits, cellColour, vertices, uvs, indices, colours);
                  break;
                }
              }
            }
          }
        }
        Mesh mesh = this.meshChunks[chunk_x, chunk_y];
        if (vertices.Count > 0)
        {
          if (Object.op_Equality((Object) mesh, (Object) null))
          {
            mesh = new Mesh();
            ((Object) mesh).name = "BlockTile";
            this.meshChunks[chunk_x, chunk_y] = mesh;
          }
          mesh.Clear();
          mesh.SetVertices(vertices);
          mesh.SetUVs(0, uvs);
          mesh.SetColors(colours);
          mesh.SetTriangles(indices, 0);
        }
        else if (Object.op_Inequality((Object) mesh, (Object) null))
          this.meshChunks[chunk_x, chunk_y] = (Mesh) null;
        if (this.decorRenderInfo == null)
          return;
        this.decorRenderInfo.Rebuild(renderer, this.occupiedCells, chunk_x, chunk_y, this.decorZOffset, 16, vertices, uvs, colours, indices, this.element);
      }

      private void AddVertexInfo(
        BlockTileRenderer.RenderInfo.AtlasInfo atlas_info,
        Vector2 uv_trim_size,
        int x,
        int y,
        BlockTileRenderer.Bits connection_bits,
        Color color,
        List<Vector3> vertices,
        List<Vector2> uvs,
        List<int> indices,
        List<Color> colours)
      {
        Vector2 vector2_1;
        // ISSUE: explicit constructor call
        ((Vector2) ref vector2_1).\u002Ector((float) x, (float) y);
        Vector2 vector2_2 = Vector2.op_Addition(vector2_1, new Vector2(1f, 1f));
        Vector2 vector2_3;
        // ISSUE: explicit constructor call
        ((Vector2) ref vector2_3).\u002Ector(atlas_info.uvBox.x, atlas_info.uvBox.w);
        Vector2 vector2_4;
        // ISSUE: explicit constructor call
        ((Vector2) ref vector2_4).\u002Ector(atlas_info.uvBox.z, atlas_info.uvBox.y);
        if ((connection_bits & BlockTileRenderer.Bits.Left) == (BlockTileRenderer.Bits) 0)
          vector2_1.x -= 0.25f;
        else
          vector2_3.x += uv_trim_size.x;
        if ((connection_bits & BlockTileRenderer.Bits.Right) == (BlockTileRenderer.Bits) 0)
          vector2_2.x += 0.25f;
        else
          vector2_4.x -= uv_trim_size.x;
        if ((connection_bits & BlockTileRenderer.Bits.Up) == (BlockTileRenderer.Bits) 0)
          vector2_2.y += 0.25f;
        else
          vector2_4.y -= uv_trim_size.y;
        if ((connection_bits & BlockTileRenderer.Bits.Down) == (BlockTileRenderer.Bits) 0)
          vector2_1.y -= 0.25f;
        else
          vector2_3.y += uv_trim_size.y;
        int count = vertices.Count;
        vertices.Add(Vector2.op_Implicit(vector2_1));
        vertices.Add(Vector2.op_Implicit(new Vector2(vector2_2.x, vector2_1.y)));
        vertices.Add(Vector2.op_Implicit(vector2_2));
        vertices.Add(Vector2.op_Implicit(new Vector2(vector2_1.x, vector2_2.y)));
        uvs.Add(vector2_3);
        uvs.Add(new Vector2(vector2_4.x, vector2_3.y));
        uvs.Add(vector2_4);
        uvs.Add(new Vector2(vector2_3.x, vector2_4.y));
        indices.Add(count);
        indices.Add(count + 1);
        indices.Add(count + 2);
        indices.Add(count);
        indices.Add(count + 2);
        indices.Add(count + 3);
        colours.Add(color);
        colours.Add(color);
        colours.Add(color);
        colours.Add(color);
      }

      private struct AtlasInfo
      {
        public BlockTileRenderer.Bits requiredConnections;
        public BlockTileRenderer.Bits forbiddenConnections;
        public Vector4 uvBox;
        public string name;
      }
    }

    private class DecorRenderInfo
    {
      private int queryLayer;
      private BlockTileDecorInfo decorInfo;
      private Mesh[,] meshChunks;
      private Material material;
      private List<BlockTileRenderer.DecorRenderInfo.TriangleInfo> triangles = new List<BlockTileRenderer.DecorRenderInfo.TriangleInfo>();
      private static Vector2 simplex_scale = new Vector2(92.41f, 87.16f);

      public DecorRenderInfo(
        int num_x_chunks,
        int num_y_chunks,
        int query_layer,
        BuildingDef def,
        BlockTileDecorInfo decorInfo)
      {
        this.decorInfo = decorInfo;
        this.queryLayer = query_layer;
        this.material = new Material(def.BlockTileMaterial);
        if (def.BlockTileIsTransparent)
          this.material.renderQueue = RenderQueues.Liquid;
        else if (def.SceneLayer == Grid.SceneLayer.TileMain)
          this.material.renderQueue = RenderQueues.BlockTiles;
        this.material.SetTexture("_MainTex", (Texture) decorInfo.atlas.texture);
        if (Object.op_Inequality((Object) decorInfo.atlasSpec, (Object) null))
        {
          this.material.SetTexture("_SpecularTex", (Texture) decorInfo.atlasSpec.texture);
          this.material.EnableKeyword("ENABLE_SHINE");
        }
        else
          this.material.DisableKeyword("ENABLE_SHINE");
        this.meshChunks = new Mesh[num_x_chunks, num_y_chunks];
      }

      public void FreeResources()
      {
        this.decorInfo = (BlockTileDecorInfo) null;
        Object.DestroyImmediate((Object) this.material);
        this.material = (Material) null;
        for (int index1 = 0; index1 < this.meshChunks.GetLength(0); ++index1)
        {
          for (int index2 = 0; index2 < this.meshChunks.GetLength(1); ++index2)
          {
            if (Object.op_Inequality((Object) this.meshChunks[index1, index2], (Object) null))
            {
              Object.DestroyImmediate((Object) this.meshChunks[index1, index2]);
              this.meshChunks[index1, index2] = (Mesh) null;
            }
          }
        }
        this.meshChunks = (Mesh[,]) null;
        this.triangles.Clear();
      }

      public void Render(int x, int y, Vector3 position, int renderLayer)
      {
        if (!Object.op_Inequality((Object) this.meshChunks[x, y], (Object) null))
          return;
        Graphics.DrawMesh(this.meshChunks[x, y], position, Quaternion.identity, this.material, renderLayer);
      }

      public void Rebuild(
        BlockTileRenderer renderer,
        Dictionary<int, int> occupiedCells,
        int chunk_x,
        int chunk_y,
        float z_offset,
        int chunkEdgeSize,
        List<Vector3> vertices,
        List<Vector2> uvs,
        List<Color> colours,
        List<int> indices,
        SimHashes element)
      {
        vertices.Clear();
        uvs.Clear();
        this.triangles.Clear();
        colours.Clear();
        indices.Clear();
        for (int y = chunk_y * chunkEdgeSize; y < chunk_y * chunkEdgeSize + chunkEdgeSize; ++y)
        {
          for (int x = chunk_x * chunkEdgeSize; x < chunk_x * chunkEdgeSize + chunkEdgeSize; ++x)
          {
            int num = y * Grid.WidthInCells + x;
            if (occupiedCells.ContainsKey(num))
            {
              Color cellColour = renderer.GetCellColour(num, element);
              BlockTileRenderer.Bits decorConnectionBits = renderer.GetDecorConnectionBits(x, y, this.queryLayer);
              this.AddDecor(x, y, z_offset, decorConnectionBits, cellColour, vertices, uvs, this.triangles, colours);
            }
          }
        }
        if (vertices.Count > 0)
        {
          Mesh mesh = this.meshChunks[chunk_x, chunk_y];
          if (Object.op_Equality((Object) mesh, (Object) null))
          {
            mesh = new Mesh();
            ((Object) mesh).name = "DecorRender";
            this.meshChunks[chunk_x, chunk_y] = mesh;
          }
          this.triangles.Sort((Comparison<BlockTileRenderer.DecorRenderInfo.TriangleInfo>) ((a, b) => a.sortOrder.CompareTo(b.sortOrder)));
          for (int index = 0; index < this.triangles.Count; ++index)
          {
            indices.Add(this.triangles[index].i0);
            indices.Add(this.triangles[index].i1);
            indices.Add(this.triangles[index].i2);
          }
          mesh.Clear();
          mesh.SetVertices(vertices);
          mesh.SetUVs(0, uvs);
          mesh.SetColors(colours);
          mesh.SetTriangles(indices, 0);
        }
        else
          this.meshChunks[chunk_x, chunk_y] = (Mesh) null;
      }

      private void AddDecor(
        int x,
        int y,
        float z_offset,
        BlockTileRenderer.Bits connection_bits,
        Color colour,
        List<Vector3> vertices,
        List<Vector2> uvs,
        List<BlockTileRenderer.DecorRenderInfo.TriangleInfo> triangles,
        List<Color> colours)
      {
        for (int index1 = 0; index1 < this.decorInfo.decor.Length; ++index1)
        {
          BlockTileDecorInfo.Decor decor = this.decorInfo.decor[index1];
          if (decor.variants != null && decor.variants.Length != 0)
          {
            int num1 = (connection_bits & decor.requiredConnections) == decor.requiredConnections ? 1 : 0;
            bool flag = (connection_bits & decor.forbiddenConnections) != 0;
            if (num1 != 0 && !flag)
            {
              float num2 = PerlinSimplexNoise.noise((float) (index1 + x + connection_bits) * BlockTileRenderer.DecorRenderInfo.simplex_scale.x, (float) (index1 + y + connection_bits) * BlockTileRenderer.DecorRenderInfo.simplex_scale.y);
              if ((double) num2 >= (double) decor.probabilityCutoff)
              {
                int index2 = (int) ((double) (decor.variants.Length - 1) * (double) num2);
                int count = vertices.Count;
                Vector3 vector3 = Vector3.op_Addition(new Vector3((float) x, (float) y, z_offset), decor.variants[index2].offset);
                foreach (Vector3 vertex in decor.variants[index2].atlasItem.vertices)
                {
                  vertices.Add(Vector3.op_Addition(vertex, vector3));
                  colours.Add(colour);
                }
                uvs.AddRange((IEnumerable<Vector2>) decor.variants[index2].atlasItem.uvs);
                int[] indices = decor.variants[index2].atlasItem.indices;
                for (int index3 = 0; index3 < indices.Length; index3 += 3)
                  triangles.Add(new BlockTileRenderer.DecorRenderInfo.TriangleInfo()
                  {
                    sortOrder = decor.sortOrder,
                    i0 = indices[index3] + count,
                    i1 = indices[index3 + 1] + count,
                    i2 = indices[index3 + 2] + count
                  });
              }
            }
          }
        }
      }

      private struct TriangleInfo
      {
        public int sortOrder;
        public int i0;
        public int i1;
        public int i2;
      }
    }
  }
}
