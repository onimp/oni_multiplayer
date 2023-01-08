// Decompiled with JetBrains decompiler
// Type: PlaceSpaceAvailable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class PlaceSpaceAvailable : SelectModuleCondition
{
  public override bool EvaluateCondition(
    GameObject existingModule,
    BuildingDef selectedPart,
    SelectModuleCondition.SelectionContext selectionContext)
  {
    BuildingAttachPoint component1 = existingModule.GetComponent<BuildingAttachPoint>();
    switch (selectionContext)
    {
      case SelectModuleCondition.SelectionContext.AddModuleAbove:
        if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Inequality((Object) component1.points[0].attachedBuilding, (Object) null) && !((Component) component1.points[0].attachedBuilding).GetComponent<ReorderableBuilding>().CanMoveVertically(selectedPart.HeightInCells))
          return false;
        int cell1 = Grid.OffsetCell(Grid.PosToCell(existingModule), 0, existingModule.GetComponent<Building>().Def.HeightInCells);
        foreach (CellOffset placementOffset in selectedPart.PlacementOffsets)
        {
          if (!ReorderableBuilding.CheckCellClear(Grid.OffsetCell(cell1, placementOffset), existingModule))
            return false;
        }
        return true;
      case SelectModuleCondition.SelectionContext.AddModuleBelow:
        if (!existingModule.GetComponent<ReorderableBuilding>().CanMoveVertically(selectedPart.HeightInCells))
          return false;
        int cell2 = Grid.PosToCell(existingModule);
        foreach (CellOffset placementOffset in selectedPart.PlacementOffsets)
        {
          if (!ReorderableBuilding.CheckCellClear(Grid.OffsetCell(cell2, placementOffset), existingModule))
            return false;
        }
        return true;
      case SelectModuleCondition.SelectionContext.ReplaceModule:
        int moveAmount = selectedPart.HeightInCells - existingModule.GetComponent<Building>().Def.HeightInCells;
        if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Inequality((Object) component1.points[0].attachedBuilding, (Object) null))
        {
          ReorderableBuilding component2 = existingModule.GetComponent<ReorderableBuilding>();
          if (!((Component) component1.points[0].attachedBuilding).GetComponent<ReorderableBuilding>().CanMoveVertically(moveAmount, ((Component) component2).gameObject))
            return false;
        }
        ReorderableBuilding component3 = existingModule.GetComponent<ReorderableBuilding>();
        foreach (CellOffset placementOffset in selectedPart.PlacementOffsets)
        {
          if (!ReorderableBuilding.CheckCellClear(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) component3), placementOffset), ((Component) component3).gameObject))
            return false;
        }
        return true;
      default:
        return true;
    }
  }

  public override string GetStatusTooltip(
    bool ready,
    GameObject moduleBase,
    BuildingDef selectedPart)
  {
    return ready ? (string) UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.SPACE_AVAILABLE.COMPLETE : (string) UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.SPACE_AVAILABLE.FAILED;
  }
}
