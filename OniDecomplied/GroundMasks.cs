// Decompiled with JetBrains decompiler
// Type: GroundMasks
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class GroundMasks : ScriptableObject
{
  public TextureAtlas maskAtlas;
  [NonSerialized]
  public Dictionary<string, GroundMasks.BiomeMaskData> biomeMasks;

  public void Initialize()
  {
    if (Object.op_Equality((Object) this.maskAtlas, (Object) null) || this.maskAtlas.items == null)
      return;
    this.biomeMasks = new Dictionary<string, GroundMasks.BiomeMaskData>();
    foreach (TextureAtlas.Item obj in this.maskAtlas.items)
    {
      string name = obj.name;
      int length = name.IndexOf('/');
      string str1 = name.Substring(0, length);
      string str2 = name.Substring(length + 1, 4);
      string str3 = str1.ToLower();
      for (int startIndex = str3.IndexOf('_'); startIndex != -1; startIndex = str3.IndexOf('_'))
        str3 = str3.Remove(startIndex, 1);
      GroundMasks.BiomeMaskData biomeMaskData = (GroundMasks.BiomeMaskData) null;
      if (!this.biomeMasks.TryGetValue(str3, out biomeMaskData))
      {
        biomeMaskData = new GroundMasks.BiomeMaskData(str3);
        this.biomeMasks[str3] = biomeMaskData;
      }
      int int32 = Convert.ToInt32(str2, 2);
      GroundMasks.Tile tile = biomeMaskData.tiles[int32];
      if (tile.variationUVs == null)
      {
        tile.isSource = true;
        tile.variationUVs = new GroundMasks.UVData[1];
      }
      else
      {
        GroundMasks.UVData[] destinationArray = new GroundMasks.UVData[tile.variationUVs.Length + 1];
        Array.Copy((Array) tile.variationUVs, (Array) destinationArray, tile.variationUVs.Length);
        tile.variationUVs = destinationArray;
      }
      Vector4 vector4;
      // ISSUE: explicit constructor call
      ((Vector4) ref vector4).\u002Ector(obj.uvBox.x, obj.uvBox.w, obj.uvBox.z, obj.uvBox.y);
      Vector2 bl;
      // ISSUE: explicit constructor call
      ((Vector2) ref bl).\u002Ector(vector4.x, vector4.y);
      Vector2 br;
      // ISSUE: explicit constructor call
      ((Vector2) ref br).\u002Ector(vector4.z, vector4.y);
      Vector2 tl;
      // ISSUE: explicit constructor call
      ((Vector2) ref tl).\u002Ector(vector4.x, vector4.w);
      Vector2 tr;
      // ISSUE: explicit constructor call
      ((Vector2) ref tr).\u002Ector(vector4.z, vector4.w);
      GroundMasks.UVData uvData = new GroundMasks.UVData(bl, br, tl, tr);
      tile.variationUVs[tile.variationUVs.Length - 1] = uvData;
      biomeMaskData.tiles[int32] = tile;
    }
    foreach (KeyValuePair<string, GroundMasks.BiomeMaskData> biomeMask in this.biomeMasks)
    {
      biomeMask.Value.GenerateRotations();
      biomeMask.Value.Validate();
    }
  }

  [ContextMenu("Print Variations")]
  private void Regenerate()
  {
    this.Initialize();
    string str = "Listing all variations:\n";
    foreach (KeyValuePair<string, GroundMasks.BiomeMaskData> biomeMask in this.biomeMasks)
    {
      GroundMasks.BiomeMaskData biomeMaskData = biomeMask.Value;
      str = str + "Biome: " + biomeMaskData.name + "\n";
      for (int index = 1; index < biomeMaskData.tiles.Length; ++index)
      {
        GroundMasks.Tile tile = biomeMaskData.tiles[index];
        str += string.Format("  tile {0}: {1} variations\n", (object) Convert.ToString(index, 2).PadLeft(4, '0'), (object) tile.variationUVs.Length);
      }
    }
    Debug.Log((object) str);
  }

  public struct UVData
  {
    public Vector2 bl;
    public Vector2 br;
    public Vector2 tl;
    public Vector2 tr;

    public UVData(Vector2 bl, Vector2 br, Vector2 tl, Vector2 tr)
    {
      this.bl = bl;
      this.br = br;
      this.tl = tl;
      this.tr = tr;
    }
  }

  public struct Tile
  {
    public bool isSource;
    public GroundMasks.UVData[] variationUVs;
  }

  public class BiomeMaskData
  {
    public string name;
    public GroundMasks.Tile[] tiles;

    public BiomeMaskData(string name)
    {
      this.name = name;
      this.tiles = new GroundMasks.Tile[16];
    }

    public void GenerateRotations()
    {
      for (int dest_mask = 1; dest_mask < 15; ++dest_mask)
      {
        if (!this.tiles[dest_mask].isSource)
        {
          GroundMasks.Tile tile = this.tiles[dest_mask] with
          {
            variationUVs = this.GetNonNullRotationUVs(dest_mask)
          };
          this.tiles[dest_mask] = tile;
        }
      }
    }

    public GroundMasks.UVData[] GetNonNullRotationUVs(int dest_mask)
    {
      GroundMasks.UVData[] nonNullRotationUvs = (GroundMasks.UVData[]) null;
      int num1 = dest_mask;
      for (int index1 = 0; index1 < 3; ++index1)
      {
        int num2 = num1 & 1;
        int num3 = (num1 & 2) >> 1;
        int num4 = (num1 & 4) >> 2;
        int index2 = (num1 & 8) >> 3 << 2 | num4 | num3 << 3 | num2 << 1;
        if (this.tiles[index2].isSource)
        {
          nonNullRotationUvs = new GroundMasks.UVData[this.tiles[index2].variationUVs.Length];
          for (int index3 = 0; index3 < this.tiles[index2].variationUVs.Length; ++index3)
          {
            GroundMasks.UVData variationUv = this.tiles[index2].variationUVs[index3];
            GroundMasks.UVData uvData = variationUv;
            switch (index1)
            {
              case 0:
                uvData = new GroundMasks.UVData(variationUv.tl, variationUv.bl, variationUv.tr, variationUv.br);
                break;
              case 1:
                uvData = new GroundMasks.UVData(variationUv.tr, variationUv.tl, variationUv.br, variationUv.bl);
                break;
              case 2:
                uvData = new GroundMasks.UVData(variationUv.br, variationUv.tr, variationUv.bl, variationUv.tl);
                break;
              default:
                Debug.LogError((object) "Unhandled rotation case");
                break;
            }
            nonNullRotationUvs[index3] = uvData;
          }
          break;
        }
        num1 = index2;
      }
      return nonNullRotationUvs;
    }

    public void Validate()
    {
      for (int index = 1; index < this.tiles.Length; ++index)
      {
        if (this.tiles[index].variationUVs == null)
          DebugUtil.LogErrorArgs(new object[3]
          {
            (object) this.name,
            (object) "has invalid tile at index",
            (object) index
          });
      }
    }
  }
}
