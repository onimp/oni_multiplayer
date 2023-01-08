// Decompiled with JetBrains decompiler
// Type: RocketHeightLimit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class RocketHeightLimit : SelectModuleCondition
{
  public override bool EvaluateCondition(
    GameObject existingModule,
    BuildingDef selectedPart,
    SelectModuleCondition.SelectionContext selectionContext)
  {
    int heightInCells = selectedPart.HeightInCells;
    if (selectionContext == SelectModuleCondition.SelectionContext.ReplaceModule)
      heightInCells -= existingModule.GetComponent<Building>().Def.HeightInCells;
    if (Object.op_Equality((Object) existingModule, (Object) null))
      return true;
    RocketModuleCluster component1 = existingModule.GetComponent<RocketModuleCluster>();
    if (Object.op_Equality((Object) component1, (Object) null))
      return true;
    int num = component1.CraftInterface.MaxHeight;
    if (num <= 0)
      num = TUNING.ROCKETRY.ROCKET_HEIGHT.MAX_MODULE_STACK_HEIGHT;
    RocketEngineCluster component2 = existingModule.GetComponent<RocketEngineCluster>();
    RocketEngineCluster component3 = selectedPart.BuildingComplete.GetComponent<RocketEngineCluster>();
    if (selectionContext == SelectModuleCondition.SelectionContext.ReplaceModule && Object.op_Inequality((Object) component2, (Object) null))
      num = !Object.op_Inequality((Object) component3, (Object) null) ? TUNING.ROCKETRY.ROCKET_HEIGHT.MAX_MODULE_STACK_HEIGHT : component3.maxHeight;
    if (Object.op_Inequality((Object) component3, (Object) null) && selectionContext == SelectModuleCondition.SelectionContext.AddModuleBelow)
      num = component3.maxHeight;
    return num == -1 || component1.CraftInterface.RocketHeight + heightInCells <= num;
  }

  public override string GetStatusTooltip(
    bool ready,
    GameObject moduleBase,
    BuildingDef selectedPart)
  {
    RocketEngineCluster engine = moduleBase.GetComponent<RocketModuleCluster>().CraftInterface.GetEngine();
    RocketEngineCluster component = selectedPart.BuildingComplete.GetComponent<RocketEngineCluster>();
    bool flag = Object.op_Inequality((Object) engine, (Object) null) || Object.op_Inequality((Object) component, (Object) null);
    if (ready)
      return (string) UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MAX_HEIGHT.COMPLETE;
    return flag ? (string) UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MAX_HEIGHT.FAILED : (string) UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MAX_HEIGHT.FAILED_NO_ENGINE;
  }
}
