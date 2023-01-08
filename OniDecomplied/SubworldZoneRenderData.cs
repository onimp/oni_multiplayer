// Decompiled with JetBrains decompiler
// Type: SubworldZoneRenderData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Delaunay.Geo;
using Klei;
using ProcGen;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SubworldZoneRenderData")]
public class SubworldZoneRenderData : KMonoBehaviour
{
  [SerializeField]
  private Texture2D colourTex;
  [SerializeField]
  private Texture2D indexTex;
  [HideInInspector]
  public SubWorld.ZoneType[] worldZoneTypes;
  [SerializeField]
  [HideInInspector]
  public Color32[] zoneColours = new Color32[15]
  {
    new Color32((byte) 145, (byte) 198, (byte) 213, (byte) 0),
    new Color32((byte) 135, (byte) 82, (byte) 160, (byte) 1),
    new Color32((byte) 123, (byte) 151, (byte) 75, (byte) 2),
    new Color32((byte) 236, (byte) 189, (byte) 89, (byte) 3),
    new Color32((byte) 201, (byte) 152, (byte) 181, (byte) 4),
    new Color32((byte) 222, (byte) 90, (byte) 59, (byte) 5),
    new Color32((byte) 201, (byte) 152, (byte) 181, (byte) 6),
    new Color32(byte.MaxValue, (byte) 0, (byte) 0, (byte) 7),
    new Color32((byte) 201, (byte) 201, (byte) 151, (byte) 8),
    new Color32((byte) 236, (byte) 90, (byte) 110, (byte) 9),
    new Color32((byte) 110, (byte) 236, (byte) 110, (byte) 10),
    new Color32((byte) 145, (byte) 198, (byte) 213, (byte) 11),
    new Color32((byte) 145, (byte) 198, (byte) 213, (byte) 12),
    new Color32((byte) 145, (byte) 198, (byte) 213, (byte) 13),
    new Color32((byte) 173, (byte) 222, (byte) 212, (byte) 14)
  };
  private const int NUM_COLOUR_BYTES = 3;
  public int[] zoneTextureArrayIndices = new int[18]
  {
    0,
    1,
    2,
    3,
    4,
    5,
    5,
    3,
    6,
    7,
    8,
    9,
    10,
    11,
    12,
    7,
    3,
    13
  };

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ShaderReloader.Register(new System.Action(this.OnShadersReloaded));
    this.GenerateTexture();
    this.OnActiveWorldChanged();
    Game.Instance.Subscribe(1983128072, (Action<object>) (worlds => this.OnActiveWorldChanged()));
  }

  public void OnActiveWorldChanged()
  {
    byte[] rawTextureData1 = this.colourTex.GetRawTextureData();
    byte[] rawTextureData2 = this.indexTex.GetRawTextureData();
    WorldDetailSave clusterDetailSave = SaveLoader.Instance.clusterDetailSave;
    Vector2 zero = Vector2.zero;
    for (int index = 0; index < clusterDetailSave.overworldCells.Count; ++index)
    {
      WorldDetailSave.OverworldCell overworldCell = clusterDetailSave.overworldCells[index];
      Polygon poly = overworldCell.poly;
      ref Vector2 local1 = ref zero;
      Rect bounds = poly.bounds;
      double num1 = (double) (int) Mathf.Floor(((Rect) ref bounds).yMin);
      local1.y = (float) num1;
      while (true)
      {
        double y = (double) zero.y;
        bounds = poly.bounds;
        double num2 = (double) Mathf.Ceil(((Rect) ref bounds).yMax);
        if (y < num2)
        {
          ref Vector2 local2 = ref zero;
          bounds = poly.bounds;
          double num3 = (double) (int) Mathf.Floor(((Rect) ref bounds).xMin);
          local2.x = (float) num3;
          while (true)
          {
            double x = (double) zero.x;
            bounds = poly.bounds;
            double num4 = (double) Mathf.Ceil(((Rect) ref bounds).xMax);
            if (x < num4)
            {
              if (poly.Contains(zero))
              {
                int cell = Grid.XYToCell((int) zero.x, (int) zero.y);
                if (Grid.IsValidCell(cell))
                {
                  if (Grid.IsActiveWorld(cell))
                  {
                    rawTextureData2[cell] = overworldCell.zoneType == 7 ? byte.MaxValue : (byte) this.zoneTextureArrayIndices[overworldCell.zoneType];
                    Color32 zoneColour = this.zoneColours[overworldCell.zoneType];
                    rawTextureData1[cell * 3] = zoneColour.r;
                    rawTextureData1[cell * 3 + 1] = zoneColour.g;
                    rawTextureData1[cell * 3 + 2] = zoneColour.b;
                  }
                  else
                  {
                    rawTextureData2[cell] = byte.MaxValue;
                    Color32 zoneColour = this.zoneColours[7];
                    rawTextureData1[cell * 3] = zoneColour.r;
                    rawTextureData1[cell * 3 + 1] = zoneColour.g;
                    rawTextureData1[cell * 3 + 2] = zoneColour.b;
                  }
                }
              }
              ++zero.x;
            }
            else
              break;
          }
          ++zero.y;
        }
        else
          break;
      }
    }
    this.colourTex.LoadRawTextureData(rawTextureData1);
    this.indexTex.LoadRawTextureData(rawTextureData2);
    this.colourTex.Apply();
    this.indexTex.Apply();
    this.OnShadersReloaded();
  }

  public void GenerateTexture()
  {
    byte[] bytes = new byte[Grid.WidthInCells * Grid.HeightInCells];
    byte[] numArray = new byte[Grid.WidthInCells * Grid.HeightInCells * 3];
    this.colourTex = new Texture2D(Grid.WidthInCells, Grid.HeightInCells, (TextureFormat) 3, false);
    ((Object) this.colourTex).name = "SubworldRegionColourData";
    ((Texture) this.colourTex).filterMode = (FilterMode) 1;
    ((Texture) this.colourTex).wrapMode = (TextureWrapMode) 1;
    ((Texture) this.colourTex).anisoLevel = 0;
    this.indexTex = new Texture2D(Grid.WidthInCells, Grid.HeightInCells, (TextureFormat) 1, false);
    ((Object) this.indexTex).name = "SubworldRegionIndexData";
    ((Texture) this.indexTex).filterMode = (FilterMode) 0;
    ((Texture) this.indexTex).wrapMode = (TextureWrapMode) 1;
    ((Texture) this.indexTex).anisoLevel = 0;
    for (int index = 0; index < Grid.CellCount; ++index)
    {
      bytes[index] = byte.MaxValue;
      Color32 zoneColour = this.zoneColours[7];
      numArray[index * 3] = zoneColour.r;
      numArray[index * 3 + 1] = zoneColour.g;
      numArray[index * 3 + 2] = zoneColour.b;
    }
    this.colourTex.LoadRawTextureData(numArray);
    this.indexTex.LoadRawTextureData(bytes);
    this.colourTex.Apply();
    this.indexTex.Apply();
    this.worldZoneTypes = new SubWorld.ZoneType[Grid.CellCount];
    WorldDetailSave clusterDetailSave = SaveLoader.Instance.clusterDetailSave;
    Vector2 zero = Vector2.zero;
    for (int index = 0; index < clusterDetailSave.overworldCells.Count; ++index)
    {
      WorldDetailSave.OverworldCell overworldCell = clusterDetailSave.overworldCells[index];
      Polygon poly = overworldCell.poly;
      ref Vector2 local1 = ref zero;
      Rect bounds = poly.bounds;
      double num1 = (double) (int) Mathf.Floor(((Rect) ref bounds).yMin);
      local1.y = (float) num1;
      while (true)
      {
        double y = (double) zero.y;
        bounds = poly.bounds;
        double num2 = (double) Mathf.Ceil(((Rect) ref bounds).yMax);
        if (y < num2)
        {
          ref Vector2 local2 = ref zero;
          bounds = poly.bounds;
          double num3 = (double) (int) Mathf.Floor(((Rect) ref bounds).xMin);
          local2.x = (float) num3;
          while (true)
          {
            double x = (double) zero.x;
            bounds = poly.bounds;
            double num4 = (double) Mathf.Ceil(((Rect) ref bounds).xMax);
            if (x < num4)
            {
              if (poly.Contains(zero))
              {
                int cell = Grid.XYToCell((int) zero.x, (int) zero.y);
                if (Grid.IsValidCell(cell))
                {
                  bytes[cell] = overworldCell.zoneType == 7 ? byte.MaxValue : (byte) overworldCell.zoneType;
                  this.worldZoneTypes[cell] = overworldCell.zoneType;
                }
              }
              ++zero.x;
            }
            else
              break;
          }
          ++zero.y;
        }
        else
          break;
      }
    }
    this.InitSimZones(bytes);
  }

  private void OnShadersReloaded()
  {
    Shader.SetGlobalTexture("_WorldZoneTex", (Texture) this.colourTex);
    Shader.SetGlobalTexture("_WorldZoneIndexTex", (Texture) this.indexTex);
  }

  public SubWorld.ZoneType GetSubWorldZoneType(int cell) => cell >= 0 && cell < this.worldZoneTypes.Length ? (SubWorld.ZoneType) (int) this.worldZoneTypes[cell] : (SubWorld.ZoneType) 3;

  private SubWorld.ZoneType GetSubWorldZoneType(Vector2I pos)
  {
    WorldDetailSave clusterDetailSave = SaveLoader.Instance.clusterDetailSave;
    if (clusterDetailSave != null)
    {
      for (int index = 0; index < clusterDetailSave.overworldCells.Count; ++index)
      {
        if (clusterDetailSave.overworldCells[index].poly.Contains(Vector2I.op_Implicit(pos)))
          return clusterDetailSave.overworldCells[index].zoneType;
      }
    }
    return (SubWorld.ZoneType) 3;
  }

  private Color32 GetZoneColor(SubWorld.ZoneType zone_type)
  {
    Color32 zoneColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte) 3);
    int num1 = zone_type < this.zoneColours.Length ? 1 : 0;
    int num2 = (int) zone_type;
    string str1 = num2.ToString();
    num2 = this.zoneColours.Length;
    string str2 = num2.ToString();
    string str3 = "Need to add more colours to handle this zone" + str1 + "<" + str2;
    Debug.Assert(num1 != 0, (object) str3);
    return zoneColor;
  }

  private unsafe void InitSimZones(byte[] bytes)
  {
    fixed (byte* msg = bytes)
      Sim.SIM_HandleMessage(-457308393, bytes.Length, msg);
  }
}
