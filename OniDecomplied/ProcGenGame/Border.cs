// Decompiled with JetBrains decompiler
// Type: ProcGenGame.Border
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenGame
{
  public class Border : Path
  {
    public Neighbors neighbors;
    public List<WeightedSimHash> element;
    public float width;

    public Border(Neighbors neighbors, Vector2 e0, Vector2 e1)
    {
      this.neighbors = neighbors;
      Vector2 vector2_1 = Vector2.op_Subtraction(e1, e0);
      Vector2 vector2_2 = new Vector2(-vector2_1.y, vector2_1.x);
      Vector2 normalized = ((Vector2) ref vector2_2).normalized;
      Vector2 vector2_3 = Vector2.op_Addition(Vector2.op_Addition(e0, Vector2.op_Division(vector2_1, 2f)), normalized);
      if (neighbors.n0.poly.Contains(vector2_3))
        this.AddSegment(e0, e1);
      else
        this.AddSegment(e1, e0);
    }

    private void SetCell(
      int gridCell,
      float defaultTemperature,
      TerrainCell.SetValuesFunction SetValues,
      SeededRandom rnd)
    {
      WeightedSimHash weightedSimHash = WeightedRandom.Choose<WeightedSimHash>(this.element, rnd);
      TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(weightedSimHash.element, weightedSimHash.overrides);
      if (!elementOverride.overrideTemperature)
        elementOverride.pdelement.temperature = defaultTemperature;
      SetValues(gridCell, (object) elementOverride.element, elementOverride.pdelement, elementOverride.dc);
    }

    public void ConvertToMap(
      Chunk world,
      TerrainCell.SetValuesFunction SetValues,
      float neighbour0Temperature,
      float neighbour1Temperature,
      float midTemp,
      SeededRandom rnd,
      int snapLastCells)
    {
      for (int index1 = 0; index1 < this.pathElements.Count; ++index1)
      {
        Vector2 vector2_1 = Vector2.op_Subtraction(this.pathElements[index1].e1, this.pathElements[index1].e0);
        Vector2 vector2_2 = new Vector2(-vector2_1.y, vector2_1.x);
        Vector2 normalized = ((Vector2) ref vector2_2).normalized;
        List<Vector2I> line = Util.GetLine(this.pathElements[index1].e0, this.pathElements[index1].e1);
        for (int index2 = 0; index2 < line.Count; ++index2)
        {
          int cell1 = Grid.XYToCell(line[index2].x, line[index2].y);
          if (Grid.IsValidCell(cell1))
            this.SetCell(cell1, midTemp, SetValues, rnd);
          for (float num1 = 0.5f; (double) num1 <= (double) this.width; ++num1)
          {
            float num2 = Mathf.Clamp01((float) (((double) num1 - 0.5) / ((double) this.width - 0.5)));
            if ((double) num1 + (double) snapLastCells > (double) this.width)
              num2 = 1f;
            Vector2 vector2_3 = Vector2.op_Addition(Vector2I.op_Implicit(line[index2]), Vector2.op_Multiply(normalized, num1));
            float defaultTemperature1 = midTemp + (neighbour0Temperature - midTemp) * num2;
            int cell2 = Grid.XYToCell((int) vector2_3.x, (int) vector2_3.y);
            if (Grid.IsValidCell(cell2))
              this.SetCell(cell2, defaultTemperature1, SetValues, rnd);
            Vector2 vector2_4 = Vector2.op_Subtraction(Vector2I.op_Implicit(line[index2]), Vector2.op_Multiply(normalized, num1));
            float defaultTemperature2 = midTemp + (neighbour1Temperature - midTemp) * num2;
            int cell3 = Grid.XYToCell((int) vector2_4.x, (int) vector2_4.y);
            if (Grid.IsValidCell(cell3))
              this.SetCell(cell3, defaultTemperature2, SetValues, rnd);
          }
        }
      }
    }
  }
}
