// Decompiled with JetBrains decompiler
// Type: Klei.Input.DigAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.Input
{
  [ActionType("InterfaceTool", "Dig", true)]
  public abstract class DigAction
  {
    public void Uproot(int cell)
    {
      ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
      int x;
      int y;
      Grid.CellToXY(cell, out x, out y);
      GameScenePartitioner.Instance.GatherEntries(x, y, 1, 1, GameScenePartitioner.Instance.plants, (List<ScenePartitionerEntry>) gathered_entries);
      if (((List<ScenePartitionerEntry>) gathered_entries).Count > 0)
        this.Uproot((((List<ScenePartitionerEntry>) gathered_entries)[0].obj as Component).GetComponent<Uprootable>());
      gathered_entries.Recycle();
    }

    public abstract void Dig(int cell, int distFromOrigin);

    protected abstract void Uproot(Uprootable uprootable);
  }
}
