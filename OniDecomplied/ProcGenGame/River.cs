// Decompiled with JetBrains decompiler
// Type: ProcGenGame.River
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenGame
{
  public class River : River, SymbolicMapElement
  {
    public River(River other)
      : base(other, true)
    {
    }

    public void ConvertToMap(
      Chunk world,
      TerrainCell.SetValuesFunction SetValues,
      float temperatureMin,
      float temperatureRange,
      SeededRandom rnd)
    {
      Element elementByName1 = ElementLoader.FindElementByName(this.backgroundElement);
      Sim.PhysicsData defaultValues1 = elementByName1.defaultValues;
      Element elementByName2 = ElementLoader.FindElementByName(this.element);
      Sim.PhysicsData defaultValues2 = elementByName2.defaultValues with
      {
        temperature = this.temperature
      };
      Sim.DiseaseCell invalid = Sim.DiseaseCell.Invalid;
      for (int index1 = 0; index1 < ((Path) this).pathElements.Count; ++index1)
      {
        Segment pathElement = ((Path) this).pathElements[index1];
        Vector2 vector2_1 = Vector2.op_Subtraction(pathElement.e1, pathElement.e0);
        Vector2 vector2_2 = new Vector2(-vector2_1.y, vector2_1.x);
        Vector2 normalized = ((Vector2) ref vector2_2).normalized;
        List<Vector2I> line = Util.GetLine(pathElement.e0, pathElement.e1);
        for (int index2 = 0; index2 < line.Count; ++index2)
        {
          for (float num = 0.5f; (double) num <= (double) this.widthCenter; ++num)
          {
            Vector2 vector2_3 = Vector2.op_Addition(Vector2I.op_Implicit(line[index2]), Vector2.op_Multiply(normalized, num));
            int cell1 = Grid.XYToCell((int) vector2_3.x, (int) vector2_3.y);
            if (Grid.IsValidCell(cell1))
              SetValues(cell1, (object) elementByName2, defaultValues2, invalid);
            Vector2 vector2_4 = Vector2.op_Subtraction(Vector2I.op_Implicit(line[index2]), Vector2.op_Multiply(normalized, num));
            int cell2 = Grid.XYToCell((int) vector2_4.x, (int) vector2_4.y);
            if (Grid.IsValidCell(cell2))
              SetValues(cell2, (object) elementByName2, defaultValues2, invalid);
          }
          for (float num = 0.5f; (double) num <= (double) this.widthBorder; ++num)
          {
            Vector2 vector2_5 = Vector2.op_Addition(Vector2I.op_Implicit(line[index2]), Vector2.op_Multiply(normalized, this.widthCenter + num));
            int cell3 = Grid.XYToCell((int) vector2_5.x, (int) vector2_5.y);
            if (Grid.IsValidCell(cell3))
            {
              defaultValues1.temperature = temperatureMin + world.heatOffset[cell3] * temperatureRange;
              SetValues(cell3, (object) elementByName1, defaultValues1, invalid);
            }
            Vector2 vector2_6 = Vector2.op_Subtraction(Vector2I.op_Implicit(line[index2]), Vector2.op_Multiply(normalized, this.widthCenter + num));
            int cell4 = Grid.XYToCell((int) vector2_6.x, (int) vector2_6.y);
            if (Grid.IsValidCell(cell4))
            {
              defaultValues1.temperature = temperatureMin + world.heatOffset[cell4] * temperatureRange;
              SetValues(cell4, (object) elementByName1, defaultValues1, invalid);
            }
          }
        }
      }
    }

    public static void ProcessRivers(
      Chunk world,
      List<River> rivers,
      Sim.Cell[] cells,
      Sim.DiseaseCell[] dcs)
    {
      TerrainCell.SetValuesFunction SetValues = (TerrainCell.SetValuesFunction) ((index, elem, pd, dc) =>
      {
        if (Grid.IsValidCell(index))
        {
          cells[index].SetValues(elem as Element, pd, ElementLoader.elements);
          dcs[index] = dc;
        }
        else
          Debug.LogError((object) ("Process::SetValuesFunction Index [" + index.ToString() + "] is not valid. cells.Length [" + cells.Length.ToString() + "]"));
      });
      float temperatureMin = 265f;
      float temperatureRange = 30f;
      for (int index = 0; index < rivers.Count; ++index)
        rivers[index].ConvertToMap(world, SetValues, temperatureMin, temperatureRange, (SeededRandom) null);
    }

    public static River GetRiverForCell(List<River> rivers, int cell) => new River(rivers.Find((Predicate<River>) (river => Grid.PosToCell(river.SourcePosition()) == cell || Grid.PosToCell(river.SinkPosition()) == cell)));

    private static void GetRiverLocation(List<River> rivers, ref GameSpawnData gsd)
    {
      for (int index = 0; index < rivers.Count; ++index)
      {
        if ((double) rivers[index].SourcePosition().y < (double) rivers[index].SinkPosition().y)
          ;
      }
    }
  }
}
