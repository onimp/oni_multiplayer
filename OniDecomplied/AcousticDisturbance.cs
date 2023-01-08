// Decompiled with JetBrains decompiler
// Type: AcousticDisturbance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class AcousticDisturbance
{
  private static readonly HashedString[] PreAnims = new HashedString[2]
  {
    HashedString.op_Implicit("grid_pre"),
    HashedString.op_Implicit("grid_loop")
  };
  private static readonly HashedString PostAnim = HashedString.op_Implicit("grid_pst");
  private static float distanceDelay = 0.25f;
  private static float duration = 3f;
  private static HashSet<int> cellsInRange = new HashSet<int>();

  public static void Emit(object data, int EmissionRadius)
  {
    GameObject gameObject = (GameObject) data;
    Components.Cmps<MinionIdentity> minionIdentities = Components.LiveMinionIdentities;
    Vector2 pos1 = Vector2.op_Implicit(TransformExtensions.GetPosition(gameObject.transform));
    int cell1 = Grid.PosToCell(pos1);
    int num = EmissionRadius * EmissionRadius;
    AcousticDisturbance.cellsInRange = GameUtil.CollectCellsBreadthFirst(cell1, (Func<int, bool>) (cell => !Grid.Solid[cell]), EmissionRadius);
    AcousticDisturbance.DrawVisualEffect(cell1, AcousticDisturbance.cellsInRange);
    for (int idx = 0; idx < minionIdentities.Count; ++idx)
    {
      MinionIdentity cmp = minionIdentities[idx];
      if (Object.op_Inequality((Object) ((Component) cmp).gameObject, (Object) gameObject.gameObject))
      {
        Vector2 pos2 = Vector2.op_Implicit(TransformExtensions.GetPosition(cmp.transform));
        if ((double) Vector2.SqrMagnitude(Vector2.op_Subtraction(pos1, pos2)) <= (double) num)
        {
          int cell2 = Grid.PosToCell(pos2);
          if (AcousticDisturbance.cellsInRange.Contains(cell2) && ((Component) cmp).GetSMI<StaminaMonitor.Instance>().IsSleeping())
          {
            cmp.Trigger(-527751701, data);
            cmp.Trigger(1621815900, data);
          }
        }
      }
    }
    AcousticDisturbance.cellsInRange.Clear();
  }

  private static void DrawVisualEffect(int center_cell, HashSet<int> cells)
  {
    SoundEvent.PlayOneShot(GlobalResources.Instance().AcousticDisturbanceSound, Grid.CellToPos(center_cell));
    foreach (int cell in cells)
    {
      int gridDistance = AcousticDisturbance.GetGridDistance(cell, center_cell);
      GameScheduler.Instance.Schedule("radialgrid_pre", AcousticDisturbance.distanceDelay * (float) gridDistance, new Action<object>(AcousticDisturbance.SpawnEffect), (object) cell, (SchedulerGroup) null);
    }
  }

  private static void SpawnEffect(object data)
  {
    Grid.SceneLayer layer = Grid.SceneLayer.InteriorWall;
    KBatchedAnimController effect = FXHelpers.CreateEffect("radialgrid_kanim", Grid.CellToPosCCC((int) data, layer), layer: layer);
    effect.destroyOnAnimComplete = false;
    effect.Play(AcousticDisturbance.PreAnims, (KAnim.PlayMode) 0);
    GameScheduler.Instance.Schedule("radialgrid_loop", AcousticDisturbance.duration, new Action<object>(AcousticDisturbance.DestroyEffect), (object) effect, (SchedulerGroup) null);
  }

  private static void DestroyEffect(object data)
  {
    KBatchedAnimController kbatchedAnimController = (KBatchedAnimController) data;
    kbatchedAnimController.destroyOnAnimComplete = true;
    kbatchedAnimController.Play(AcousticDisturbance.PostAnim);
  }

  private static int GetGridDistance(int cell, int center_cell)
  {
    Vector2I vector2I = Vector2I.op_Subtraction(Grid.CellToXY(cell), Grid.CellToXY(center_cell));
    return Math.Abs(vector2I.x) + Math.Abs(vector2I.y);
  }
}
