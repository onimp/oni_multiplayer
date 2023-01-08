// Decompiled with JetBrains decompiler
// Type: NoiseSplat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSplat : IUniformGridObject
{
  public const float noiseFalloff = 0.05f;
  private IPolluter provider;
  private Vector2 position;
  private int radius;
  private Extents effectExtents;
  private Extents baseExtents;
  private HandleVector<int>.Handle partitionerEntry;
  private HandleVector<int>.Handle solidChangedPartitionerEntry;
  private List<Pair<int, float>> decibels = new List<Pair<int, float>>();

  public int dB { get; private set; }

  public float deathTime { get; private set; }

  public string GetName() => this.provider.GetName();

  public IPolluter GetProvider() => this.provider;

  public Vector2 PosMin() => new Vector2(this.position.x - (float) this.radius, this.position.y - (float) this.radius);

  public Vector2 PosMax() => new Vector2(this.position.x + (float) this.radius, this.position.y + (float) this.radius);

  public NoiseSplat(NoisePolluter setProvider, float death_time = 0.0f)
  {
    this.deathTime = death_time;
    this.dB = 0;
    this.radius = 5;
    if (setProvider.dB != null)
      this.dB = (int) setProvider.dB.GetTotalValue();
    int cell = Grid.PosToCell(((Component) setProvider).gameObject);
    if (!NoisePolluter.IsNoiseableCell(cell))
      this.dB = 0;
    if (this.dB == 0)
      return;
    setProvider.Clear();
    OccupyArea occupyArea = setProvider.occupyArea;
    this.baseExtents = occupyArea.GetExtents();
    this.provider = (IPolluter) setProvider;
    this.position = Vector2.op_Implicit(TransformExtensions.GetPosition(setProvider.transform));
    if (setProvider.dBRadius != null)
      this.radius = (int) setProvider.dBRadius.GetTotalValue();
    if (this.radius == 0)
      return;
    int x = 0;
    int y = 0;
    Grid.CellToXY(cell, out x, out y);
    int widthInCells = occupyArea.GetWidthInCells();
    int heightInCells = occupyArea.GetHeightInCells();
    Vector2I vector2I1;
    // ISSUE: explicit constructor call
    ((Vector2I) ref vector2I1).\u002Ector(x - this.radius, y - this.radius);
    Vector2I vector2I2 = Vector2I.op_Addition(vector2I1, new Vector2I(this.radius * 2 + widthInCells, this.radius * 2 + heightInCells));
    Vector2I vector2I3 = Vector2I.Max(vector2I1, Vector2I.zero);
    Vector2I vector2I4 = Vector2I.Min(vector2I2, new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1));
    this.effectExtents = new Extents(vector2I3.x, vector2I3.y, vector2I4.x - vector2I3.x, vector2I4.y - vector2I3.y);
    this.partitionerEntry = GameScenePartitioner.Instance.Add("NoiseSplat.SplatCollectNoisePolluters", (object) ((Component) setProvider).gameObject, this.effectExtents, GameScenePartitioner.Instance.noisePolluterLayer, setProvider.onCollectNoisePollutersCallback);
    this.solidChangedPartitionerEntry = GameScenePartitioner.Instance.Add("NoiseSplat.SplatSolidCheck", (object) ((Component) setProvider).gameObject, this.effectExtents, GameScenePartitioner.Instance.solidChangedLayer, setProvider.refreshPartionerCallback);
  }

  public NoiseSplat(IPolluter setProvider, float death_time = 0.0f)
  {
    this.deathTime = death_time;
    this.provider = setProvider;
    this.provider.Clear();
    this.position = this.provider.GetPosition();
    this.dB = this.provider.GetNoise();
    int cell = Grid.PosToCell(this.position);
    if (!NoisePolluter.IsNoiseableCell(cell))
      this.dB = 0;
    if (this.dB == 0)
      return;
    this.radius = this.provider.GetRadius();
    if (this.radius == 0)
      return;
    int x = 0;
    int y = 0;
    Grid.CellToXY(cell, out x, out y);
    Vector2I vector2I1;
    // ISSUE: explicit constructor call
    ((Vector2I) ref vector2I1).\u002Ector(x - this.radius, y - this.radius);
    Vector2I vector2I2 = Vector2I.op_Addition(vector2I1, new Vector2I(this.radius * 2, this.radius * 2));
    Vector2I vector2I3 = Vector2I.Max(vector2I1, Vector2I.zero);
    Vector2I vector2I4 = Vector2I.Min(vector2I2, new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1));
    this.effectExtents = new Extents(vector2I3.x, vector2I3.y, vector2I4.x - vector2I3.x, vector2I4.y - vector2I3.y);
    this.baseExtents = new Extents(x, y, 1, 1);
    this.AddNoise();
  }

  public void Clear()
  {
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    GameScenePartitioner.Instance.Free(ref this.solidChangedPartitionerEntry);
    this.RemoveNoise();
  }

  private void AddNoise()
  {
    int cell1 = Grid.PosToCell(this.position);
    int val1_1 = this.effectExtents.x + this.effectExtents.width;
    int val1_2 = this.effectExtents.y + this.effectExtents.height;
    int x1 = this.effectExtents.x;
    int y1 = this.effectExtents.y;
    int x2 = 0;
    int y2 = 0;
    ref int local1 = ref x2;
    ref int local2 = ref y2;
    Grid.CellToXY(cell1, out local1, out local2);
    int num1 = Math.Min(val1_1, Grid.WidthInCells);
    int num2 = Math.Min(val1_2, Grid.HeightInCells);
    int num3 = Math.Max(0, x1);
    for (int index1 = Math.Max(0, y1); index1 < num2; ++index1)
    {
      for (int index2 = num3; index2 < num1; ++index2)
      {
        if (Grid.VisibilityTest(x2, y2, index2, index1))
        {
          int cell2 = Grid.XYToCell(index2, index1);
          float dbForCell = this.GetDBForCell(cell2);
          if ((double) dbForCell > 0.0)
          {
            float loudness = AudioEventManager.DBToLoudness(dbForCell);
            Grid.Loudness[cell2] += loudness;
            this.decibels.Add(new Pair<int, float>(cell2, loudness));
          }
        }
      }
    }
  }

  public float GetDBForCell(int cell)
  {
    Vector2 vector2 = Vector2.op_Implicit(Grid.CellToPos2D(cell));
    float num = Mathf.Floor(Vector2.Distance(this.position, vector2));
    if ((double) vector2.x >= (double) this.baseExtents.x && (double) vector2.x < (double) (this.baseExtents.x + this.baseExtents.width) && (double) vector2.y >= (double) this.baseExtents.y && (double) vector2.y < (double) (this.baseExtents.y + this.baseExtents.height))
      num = 0.0f;
    return Mathf.Round((float) this.dB - (float) ((double) this.dB * (double) num * 0.05000000074505806));
  }

  private void RemoveNoise()
  {
    for (int index = 0; index < this.decibels.Count; ++index)
    {
      Pair<int, float> decibel = this.decibels[index];
      float num = Math.Max(0.0f, Grid.Loudness[decibel.first] - decibel.second);
      Grid.Loudness[decibel.first] = (double) num < 1.0 ? 0.0f : num;
    }
    this.decibels.Clear();
  }

  public float GetLoudness(int cell)
  {
    float loudness = 0.0f;
    for (int index = 0; index < this.decibels.Count; ++index)
    {
      Pair<int, float> decibel = this.decibels[index];
      if (decibel.first == cell)
      {
        loudness = decibel.second;
        break;
      }
    }
    return loudness;
  }
}
