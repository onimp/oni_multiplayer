// Decompiled with JetBrains decompiler
// Type: CargoBayIsEmpty
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class CargoBayIsEmpty : ProcessCondition
{
  private CommandModule commandModule;

  public CargoBayIsEmpty(CommandModule module) => this.commandModule = module;

  public override ProcessCondition.Status EvaluateCondition()
  {
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.commandModule).GetComponent<AttachableBuilding>()))
    {
      CargoBay component = gameObject.GetComponent<CargoBay>();
      if (Object.op_Inequality((Object) component, (Object) null) && (double) component.storage.MassStored() != 0.0)
        return ProcessCondition.Status.Failure;
    }
    return ProcessCondition.Status.Ready;
  }

  public override string GetStatusMessage(ProcessCondition.Status status) => (string) UI.STARMAP.CARGOEMPTY.NAME;

  public override string GetStatusTooltip(ProcessCondition.Status status) => (string) UI.STARMAP.CARGOEMPTY.TOOLTIP;

  public override bool ShowInUI() => true;
}
