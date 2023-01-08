// Decompiled with JetBrains decompiler
// Type: TopOnly
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class TopOnly : SelectModuleCondition
{
  public override bool EvaluateCondition(
    GameObject existingModule,
    BuildingDef selectedPart,
    SelectModuleCondition.SelectionContext selectionContext)
  {
    Debug.Assert(Object.op_Inequality((Object) existingModule, (Object) null), (object) "Existing module is null in top only condition");
    if (selectionContext == SelectModuleCondition.SelectionContext.ReplaceModule)
    {
      Debug.Assert(Object.op_Equality((Object) existingModule.GetComponent<LaunchPad>(), (Object) null), (object) "Trying to replace launch pad with rocket module");
      return Object.op_Equality((Object) existingModule.GetComponent<BuildingAttachPoint>(), (Object) null) || Object.op_Equality((Object) existingModule.GetComponent<BuildingAttachPoint>().points[0].attachedBuilding, (Object) null);
    }
    if (Object.op_Inequality((Object) existingModule.GetComponent<LaunchPad>(), (Object) null))
      return true;
    return Object.op_Inequality((Object) existingModule.GetComponent<BuildingAttachPoint>(), (Object) null) && Object.op_Equality((Object) existingModule.GetComponent<BuildingAttachPoint>().points[0].attachedBuilding, (Object) null);
  }

  public override string GetStatusTooltip(
    bool ready,
    GameObject moduleBase,
    BuildingDef selectedPart)
  {
    return ready ? (string) UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.TOP_ONLY.COMPLETE : (string) UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.TOP_ONLY.FAILED;
  }
}
