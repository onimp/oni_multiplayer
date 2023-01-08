// Decompiled with JetBrains decompiler
// Type: LimitOneCommandModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class LimitOneCommandModule : SelectModuleCondition
{
  public override bool EvaluateCondition(
    GameObject existingModule,
    BuildingDef selectedPart,
    SelectModuleCondition.SelectionContext selectionContext)
  {
    if (Object.op_Equality((Object) existingModule, (Object) null))
      return true;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(existingModule.GetComponent<AttachableBuilding>()))
    {
      if ((selectionContext != SelectModuleCondition.SelectionContext.ReplaceModule || !Object.op_Equality((Object) gameObject, (Object) existingModule.gameObject)) && (Object.op_Inequality((Object) gameObject.GetComponent<RocketCommandConditions>(), (Object) null) || Object.op_Inequality((Object) gameObject.GetComponent<BuildingUnderConstruction>(), (Object) null) && Object.op_Inequality((Object) gameObject.GetComponent<BuildingUnderConstruction>().Def.BuildingComplete.GetComponent<RocketCommandConditions>(), (Object) null)))
        return false;
    }
    return true;
  }

  public override string GetStatusTooltip(
    bool ready,
    GameObject moduleBase,
    BuildingDef selectedPart)
  {
    return ready ? (string) UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ONE_COMMAND_PER_ROCKET.COMPLETE : (string) UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ONE_COMMAND_PER_ROCKET.FAILED;
  }
}
