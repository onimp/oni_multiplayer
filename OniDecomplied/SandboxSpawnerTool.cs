// Decompiled with JetBrains decompiler
// Type: SandboxSpawnerTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SandboxSpawnerTool : InterfaceTool
{
  protected Color radiusIndicatorColor = new Color(0.5f, 0.7f, 0.5f, 0.2f);
  private int currentCell;

  public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
  {
    colors = new HashSet<ToolMenu.CellColorData>();
    colors.Add(new ToolMenu.CellColorData(this.currentCell, this.radiusIndicatorColor));
  }

  public override void OnMouseMove(Vector3 cursorPos)
  {
    base.OnMouseMove(cursorPos);
    this.currentCell = Grid.PosToCell(cursorPos);
  }

  public override void OnLeftClickDown(Vector3 cursor_pos) => this.Place(Grid.PosToCell(cursor_pos));

  private void Place(int cell)
  {
    if (!Grid.IsValidBuildingCell(cell))
      return;
    string stringSetting = SandboxToolParameterMenu.instance.settings.GetStringSetting("SandboxTools.SelectedEntity");
    GameObject prefab = Assets.GetPrefab(Tag.op_Implicit(stringSetting));
    if (stringSetting == MinionConfig.ID)
      this.SpawnMinion();
    else if (Object.op_Inequality((Object) prefab.GetComponent<Building>(), (Object) null))
    {
      BuildingDef def = prefab.GetComponent<Building>().Def;
      def.Build(cell, Orientation.Neutral, (Storage) null, (IList<Tag>) def.DefaultElements(), 298.15f);
    }
    else
      GameUtil.KInstantiate(prefab, Grid.CellToPosCBC(this.currentCell, Grid.SceneLayer.Creatures), Grid.SceneLayer.Creatures).SetActive(true);
    UISounds.PlaySound(UISounds.Sound.ClickObject);
  }

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    ((Component) SandboxToolParameterMenu.instance).gameObject.SetActive(true);
    SandboxToolParameterMenu.instance.DisableParameters();
    SandboxToolParameterMenu.instance.entitySelector.row.SetActive(true);
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    base.OnDeactivateTool(new_tool);
    ((Component) SandboxToolParameterMenu.instance).gameObject.SetActive(false);
  }

  private void SpawnMinion()
  {
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(MinionConfig.ID)), (GameObject) null, (string) null);
    ((Object) gameObject).name = ((Object) Assets.GetPrefab(Tag.op_Implicit(MinionConfig.ID))).name;
    Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
    Vector3 posCbc = Grid.CellToPosCBC(this.currentCell, Grid.SceneLayer.Move);
    TransformExtensions.SetLocalPosition(gameObject.transform, posCbc);
    gameObject.SetActive(true);
    new MinionStartingStats(false).Apply(gameObject);
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 242))
    {
      int cell = Grid.PosToCell(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
      List<ObjectLayer> objectLayerList = new List<ObjectLayer>();
      objectLayerList.Add(ObjectLayer.Pickupables);
      objectLayerList.Add(ObjectLayer.Plants);
      objectLayerList.Add(ObjectLayer.Minion);
      objectLayerList.Add(ObjectLayer.Building);
      if (Grid.IsValidCell(cell))
      {
        foreach (ObjectLayer layer in objectLayerList)
        {
          GameObject go = Grid.Objects[cell, (int) layer];
          if (Object.op_Implicit((Object) go))
          {
            SandboxToolParameterMenu.instance.settings.SetStringSetting("SandboxTools.SelectedEntity", go.PrefabID().ToString());
            break;
          }
        }
      }
    }
    if (((KInputEvent) e).Consumed)
      return;
    base.OnKeyDown(e);
  }
}
