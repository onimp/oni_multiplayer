// Decompiled with JetBrains decompiler
// Type: CavityVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGenGame;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CavityVisualizer")]
public class CavityVisualizer : KMonoBehaviour
{
  public List<int> cavityCells = new List<int>();
  public List<int> spawnCells = new List<int>();
  public bool drawCavity = true;
  public bool drawSpawnCells = true;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    foreach (TerrainCell key in MobSpawning.NaturalCavities.Keys)
    {
      foreach (HashSet<int> intSet in MobSpawning.NaturalCavities[key])
      {
        foreach (int num in intSet)
          this.cavityCells.Add(num);
      }
    }
  }

  private void OnDrawGizmosSelected()
  {
    if (this.drawCavity)
    {
      Color[] colorArray = new Color[2]
      {
        Color.blue,
        Color.yellow
      };
      int num = 0;
      foreach (TerrainCell key in MobSpawning.NaturalCavities.Keys)
      {
        Gizmos.color = colorArray[num % colorArray.Length];
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.125f);
        ++num;
        foreach (HashSet<int> intSet in MobSpawning.NaturalCavities[key])
        {
          foreach (int cell in intSet)
            Gizmos.DrawCube(Vector3.op_Addition(Grid.CellToPos(cell), Vector3.op_Addition(Vector3.op_Division(Vector3.right, 2f), Vector3.op_Division(Vector3.up, 2f))), Vector3.one);
        }
      }
    }
    if (this.spawnCells == null || !this.drawSpawnCells)
      return;
    Gizmos.color = new Color(0.0f, 1f, 0.0f, 0.15f);
    foreach (int spawnCell in this.spawnCells)
      Gizmos.DrawCube(Vector3.op_Addition(Grid.CellToPos(spawnCell), Vector3.op_Addition(Vector3.op_Division(Vector3.right, 2f), Vector3.op_Division(Vector3.up, 2f))), Vector3.one);
  }
}
