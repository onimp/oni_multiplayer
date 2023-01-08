// Decompiled with JetBrains decompiler
// Type: EngineOnBottom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class EngineOnBottom : SelectModuleCondition
{
  public override bool EvaluateCondition(
    GameObject existingModule,
    BuildingDef selectedPart,
    SelectModuleCondition.SelectionContext selectionContext)
  {
    if (Object.op_Equality((Object) existingModule, (Object) null) || Object.op_Inequality((Object) existingModule.GetComponent<LaunchPad>(), (Object) null))
      return true;
    switch (selectionContext)
    {
      case SelectModuleCondition.SelectionContext.AddModuleBelow:
        return Object.op_Equality((Object) existingModule.GetComponent<AttachableBuilding>().GetAttachedTo(), (Object) null);
      case SelectModuleCondition.SelectionContext.ReplaceModule:
        return Object.op_Equality((Object) existingModule.GetComponent<AttachableBuilding>().GetAttachedTo(), (Object) null);
      default:
        return false;
    }
  }

  public override string GetStatusTooltip(
    bool ready,
    GameObject moduleBase,
    BuildingDef selectedPart)
  {
    return ready ? (string) UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ENGINE_AT_BOTTOM.COMPLETE : (string) UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ENGINE_AT_BOTTOM.FAILED;
  }
}
