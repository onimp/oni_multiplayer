// Decompiled with JetBrains decompiler
// Type: DebugCellDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/DebugCellDrawer")]
public class DebugCellDrawer : KMonoBehaviour
{
  public List<int> cells;

  private void Update()
  {
    for (int index = 0; index < this.cells.Count; ++index)
    {
      if (this.cells[index] != PathFinder.InvalidCell)
        DebugExtension.DebugPoint(Grid.CellToPosCCF(this.cells[index], Grid.SceneLayer.Background), 1f, 0.0f, true);
    }
  }
}
