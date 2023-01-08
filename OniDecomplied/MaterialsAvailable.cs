// Decompiled with JetBrains decompiler
// Type: MaterialsAvailable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class MaterialsAvailable : SelectModuleCondition
{
  public override bool IgnoreInSanboxMode() => true;

  public override bool EvaluateCondition(
    GameObject existingModule,
    BuildingDef selectedPart,
    SelectModuleCondition.SelectionContext selectionContext)
  {
    return Object.op_Equality((Object) existingModule, (Object) null) || ProductInfoScreen.MaterialsMet(selectedPart.CraftRecipe);
  }

  public override string GetStatusTooltip(
    bool ready,
    GameObject moduleBase,
    BuildingDef selectedPart)
  {
    if (ready)
      return (string) UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MATERIALS_AVAILABLE.COMPLETE;
    string failed = (string) UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.MATERIALS_AVAILABLE.FAILED;
    foreach (Recipe.Ingredient ingredient in selectedPart.CraftRecipe.Ingredients)
    {
      string str = "\n" + string.Format("{0}{1}: {2}", (object) "    • ", (object) ingredient.tag.ProperName(), (object) GameUtil.GetFormattedMass(ingredient.amount));
      failed += str;
    }
    return failed;
  }
}
