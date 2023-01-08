// Decompiled with JetBrains decompiler
// Type: NewBaseScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using Klei.AI;
using ProcGenGame;
using System;
using UnityEngine;

public class NewBaseScreen : KScreen
{
  public static NewBaseScreen Instance;
  [SerializeField]
  private CanvasGroup[] disabledUIElements;
  public EventReference ScanSoundMigrated;
  public EventReference BuildBaseSoundMigrated;
  private ITelepadDeliverable[] m_minionStartingStats;
  private Cluster m_clusterLayout;

  public virtual float GetSortKey() => 1f;

  protected virtual void OnPrefabInit()
  {
    NewBaseScreen.Instance = this;
    base.OnPrefabInit();
    TimeOfDay.Instance.SetScale(0.0f);
  }

  protected virtual void OnForcedCleanUp()
  {
    NewBaseScreen.Instance = (NewBaseScreen) null;
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  public static Vector2I SetInitialCamera()
  {
    Vector2I vector2I = Vector2I.op_Addition(SaveLoader.Instance.cachedGSD.baseStartPos, ClusterManager.Instance.GetStartWorld().WorldOffset);
    Vector3 posCcc = Grid.CellToPosCCC(Grid.OffsetCell(Grid.OffsetCell(0, vector2I.x, vector2I.y), 0, -2), Grid.SceneLayer.Background);
    CameraController.Instance.SetMaxOrthographicSize(40f);
    CameraController.Instance.SnapTo(posCcc);
    CameraController.Instance.SetTargetPos(posCcc, 20f, false);
    CameraController.Instance.OrthographicSize = 40f;
    CameraSaveData.valid = false;
    return vector2I;
  }

  protected virtual void OnActivate()
  {
    if (this.disabledUIElements != null)
    {
      foreach (CanvasGroup disabledUiElement in this.disabledUIElements)
      {
        if (Object.op_Inequality((Object) disabledUiElement, (Object) null))
          disabledUiElement.interactable = false;
      }
    }
    NewBaseScreen.SetInitialCamera();
    if (SpeedControlScreen.Instance.IsPaused)
      SpeedControlScreen.Instance.Unpause(false);
    this.Final();
  }

  public void Init(Cluster clusterLayout, ITelepadDeliverable[] startingMinionStats)
  {
    this.m_clusterLayout = clusterLayout;
    this.m_minionStartingStats = startingMinionStats;
  }

  protected virtual void OnDeactivate()
  {
    Game.Instance.Trigger(-122303817, (object) null);
    if (this.disabledUIElements == null)
      return;
    foreach (CanvasGroup disabledUiElement in this.disabledUIElements)
    {
      if (Object.op_Inequality((Object) disabledUiElement, (Object) null))
        disabledUiElement.interactable = true;
    }
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    // ISSUE: unable to decompile the method.
  }

  private void Final()
  {
    SpeedControlScreen.Instance.Unpause(false);
    GameObject telepad = GameUtil.GetTelepad(ClusterManager.Instance.GetStartWorld().id);
    if (Object.op_Implicit((Object) telepad))
      this.SpawnMinions(Grid.PosToCell(telepad));
    Game.Instance.baseAlreadyCreated = true;
    this.Deactivate();
  }

  private void SpawnMinions(int headquartersCell)
  {
    if (headquartersCell == -1)
    {
      Debug.LogWarning((object) "No headquarters in saved base template. Cannot place minions. Confirm there is a headquarters saved to the base template, or consider creating a new one.");
    }
    else
    {
      int x;
      int y;
      Grid.CellToXY(headquartersCell, out x, out y);
      if (Grid.WidthInCells < 64)
        return;
      int baseLeft = this.m_clusterLayout.currentWorld.BaseLeft;
      int baseRight = this.m_clusterLayout.currentWorld.BaseRight;
      Effect a_new_hope = Db.Get().effects.Get("AnewHope");
      for (int index = 0; index < this.m_minionStartingStats.Length; ++index)
      {
        int cell = Grid.XYToCell(x + index % (baseRight - baseLeft) + 1, y);
        GameObject gameObject1 = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(MinionConfig.ID)), (GameObject) null, (string) null);
        Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject1);
        TransformExtensions.SetLocalPosition(gameObject1.transform, Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
        gameObject1.SetActive(true);
        ((MinionStartingStats) this.m_minionStartingStats[index]).Apply(gameObject1);
        GameScheduler.Instance.Schedule("ANewHope", (float) (3.0 + 0.5 * (double) index), (Action<object>) (m =>
        {
          GameObject gameObject2 = m as GameObject;
          if (Object.op_Equality((Object) gameObject2, (Object) null))
            return;
          gameObject2.GetComponent<Effects>().Add(a_new_hope, true);
        }), (object) gameObject1, (SchedulerGroup) null);
      }
      ClusterManager.Instance.activeWorld.SetDupeVisited();
    }
  }
}
