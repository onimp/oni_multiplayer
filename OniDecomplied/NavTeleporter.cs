// Decompiled with JetBrains decompiler
// Type: NavTeleporter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class NavTeleporter : KMonoBehaviour
{
  private NavTeleporter target;
  private int lastRegisteredCell = Grid.InvalidCell;
  public CellOffset offset;
  private int overrideCell = -1;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.NavTeleporters, false);
    this.Register();
    Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChanged), "NavTeleporterCellChanged");
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    int cell = this.GetCell();
    if (cell != Grid.InvalidCell)
      Grid.HasNavTeleporter[cell] = false;
    this.Deregister();
    Components.NavTeleporters.Remove(this);
  }

  public void SetOverrideCell(int cell) => this.overrideCell = cell;

  public int GetCell() => this.overrideCell >= 0 ? this.overrideCell : Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.offset);

  public void TwoWayTarget(NavTeleporter nt)
  {
    if (Object.op_Inequality((Object) this.target, (Object) null))
    {
      if (Object.op_Inequality((Object) nt, (Object) null))
        nt.SetTarget((NavTeleporter) null);
      this.BreakLink();
    }
    this.target = nt;
    if (!Object.op_Inequality((Object) this.target, (Object) null))
      return;
    this.SetLink();
    if (!Object.op_Inequality((Object) nt, (Object) null))
      return;
    nt.SetTarget(this);
  }

  public void EnableTwoWayTarget(bool enable)
  {
    if (enable)
    {
      this.target.SetLink();
      this.SetLink();
    }
    else
    {
      this.target.BreakLink();
      this.BreakLink();
    }
  }

  public void SetTarget(NavTeleporter nt)
  {
    if (Object.op_Inequality((Object) this.target, (Object) null))
      this.BreakLink();
    this.target = nt;
    if (!Object.op_Inequality((Object) this.target, (Object) null))
      return;
    this.SetLink();
  }

  private void Register()
  {
    int cell = this.GetCell();
    if (!Grid.IsValidCell(cell))
    {
      this.lastRegisteredCell = Grid.InvalidCell;
    }
    else
    {
      Grid.HasNavTeleporter[cell] = true;
      Pathfinding.Instance.AddDirtyNavGridCell(cell);
      this.lastRegisteredCell = cell;
      if (!Object.op_Inequality((Object) this.target, (Object) null))
        return;
      this.SetLink();
    }
  }

  private void SetLink()
  {
    int cell = this.target.GetCell();
    Pathfinding.Instance.GetNavGrid(MinionConfig.MINION_NAV_GRID_NAME).teleportTransitions[this.lastRegisteredCell] = cell;
    Pathfinding.Instance.AddDirtyNavGridCell(this.lastRegisteredCell);
  }

  private void Deregister()
  {
    if (this.lastRegisteredCell == Grid.InvalidCell)
      return;
    this.BreakLink();
    Grid.HasNavTeleporter[this.lastRegisteredCell] = false;
    Pathfinding.Instance.AddDirtyNavGridCell(this.lastRegisteredCell);
    this.lastRegisteredCell = Grid.InvalidCell;
  }

  private void BreakLink()
  {
    Pathfinding.Instance.GetNavGrid(MinionConfig.MINION_NAV_GRID_NAME).teleportTransitions.Remove(this.lastRegisteredCell);
    Pathfinding.Instance.AddDirtyNavGridCell(this.lastRegisteredCell);
  }

  private void OnCellChanged()
  {
    this.Deregister();
    this.Register();
    if (!Object.op_Inequality((Object) this.target, (Object) null))
      return;
    NavTeleporter component = ((Component) this.target).GetComponent<NavTeleporter>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.SetTarget(this);
  }
}
