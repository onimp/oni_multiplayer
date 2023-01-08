// Decompiled with JetBrains decompiler
// Type: SelectModuleCondition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public abstract class SelectModuleCondition
{
  public abstract bool EvaluateCondition(
    GameObject existingModule,
    BuildingDef selectedPart,
    SelectModuleCondition.SelectionContext selectionContext);

  public abstract string GetStatusTooltip(
    bool ready,
    GameObject moduleBase,
    BuildingDef selectedPart);

  public virtual bool IgnoreInSanboxMode() => false;

  public enum SelectionContext
  {
    AddModuleAbove,
    AddModuleBelow,
    ReplaceModule,
  }
}
