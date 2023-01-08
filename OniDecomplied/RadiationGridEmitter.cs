// Decompiled with JetBrains decompiler
// Type: RadiationGridEmitter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RadiationGridEmitter
{
  private static int MAX_EMIT_DISTANCE = 128;
  public int originCell = -1;
  public int intensity = 1;
  public int projectionCount = 20;
  public int direction;
  public int angle = 360;
  public bool enabled;
  private HashSet<int> scanCells = new HashSet<int>();

  public RadiationGridEmitter(int originCell, int intensity)
  {
    this.originCell = originCell;
    this.intensity = intensity;
  }

  public void Emit()
  {
    this.scanCells.Clear();
    Vector2 vector2_1 = Vector2.op_Implicit(Grid.CellToPosCCC(this.originCell, Grid.SceneLayer.Building));
    for (float num1 = (float) this.direction - (float) this.angle / 2f; (double) num1 < (double) this.direction + (double) this.angle / 2.0; num1 += (float) (this.angle / this.projectionCount))
    {
      float num2 = Random.Range((float) (-this.angle / this.projectionCount) / 2f, (float) (this.angle / this.projectionCount) / 2f);
      Vector2 vector2_2 = new Vector2(Mathf.Cos((float) (((double) num1 + (double) num2) * 3.1415927410125732 / 180.0)), Mathf.Sin((float) (((double) num1 + (double) num2) * 3.1415927410125732 / 180.0)));
      int num3 = 3;
      float num4 = (float) (this.intensity / 4);
      Vector2 vector2_3 = vector2_2;
      int cell;
      for (float num5 = 0.0f; (double) num4 > 0.01 && (double) num5 < (double) RadiationGridEmitter.MAX_EMIT_DISTANCE; num4 = num4 * Mathf.Max(0.0f, (float) (1.0 - (double) Mathf.Pow(Grid.Mass[cell], 1.25f) * (double) Grid.Element[cell].molarMass / 1000000.0)) * Random.Range(0.96f, 0.98f))
      {
        num5 += 1f / (float) num3;
        cell = Grid.PosToCell(Vector2.op_Addition(vector2_1, Vector2.op_Multiply(vector2_3, num5)));
        if (Grid.IsValidCell(cell))
        {
          if (!this.scanCells.Contains(cell))
          {
            SimMessages.ModifyRadiationOnCell(cell, (float) Mathf.RoundToInt(num4));
            this.scanCells.Add(cell);
          }
        }
        else
          break;
      }
    }
  }

  private int CalculateFalloff(float falloffRate, int cell, int origin) => Mathf.Max(1, Mathf.RoundToInt(falloffRate * (float) Mathf.Max(Grid.GetCellDistance(origin, cell), 1)));
}
