// Decompiled with JetBrains decompiler
// Type: UtilityBuildTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class UtilityBuildTool : BaseUtilityBuildTool
{
  public static UtilityBuildTool Instance;
  private int lastPathHead = -1;

  public static void DestroyInstance() => UtilityBuildTool.Instance = (UtilityBuildTool) null;

  protected override void OnPrefabInit()
  {
    UtilityBuildTool.Instance = this;
    base.OnPrefabInit();
    this.populateHitsList = true;
    this.canChangeDragAxis = false;
  }

  protected override void ApplyPathToConduitSystem()
  {
    if (this.path.Count < 2)
      return;
    for (int index = 1; index < this.path.Count; ++index)
    {
      if (this.path[index - 1].valid && this.path[index].valid)
      {
        int cell1 = this.path[index - 1].cell;
        int cell2 = this.path[index].cell;
        UtilityConnections cell3 = UtilityConnectionsExtensions.DirectionFromToCell(cell1, cell2);
        if (cell3 != (UtilityConnections) 0)
        {
          UtilityConnections new_connection = cell3.InverseDirection();
          string fail_reason;
          if ((!this.conduitMgr.CanAddConnection(cell3, cell1, false, out fail_reason) ? 0 : (this.conduitMgr.CanAddConnection(new_connection, cell2, false, out fail_reason) ? 1 : 0)) != 0)
          {
            this.conduitMgr.AddConnection(cell3, cell1, false);
            this.conduitMgr.AddConnection(new_connection, cell2, false);
          }
          else if (index == this.path.Count - 1 && this.lastPathHead != index)
            PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, fail_reason, (Transform) null, Grid.CellToPosCCC(cell2, ~Grid.SceneLayer.Background));
        }
      }
    }
    this.lastPathHead = this.path.Count - 1;
  }
}
