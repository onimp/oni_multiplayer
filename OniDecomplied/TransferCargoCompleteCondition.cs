// Decompiled with JetBrains decompiler
// Type: TransferCargoCompleteCondition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class TransferCargoCompleteCondition : ProcessCondition
{
  private GameObject target;

  public TransferCargoCompleteCondition(GameObject target) => this.target = target;

  public override ProcessCondition.Status EvaluateCondition()
  {
    LaunchPad component = this.target.GetComponent<LaunchPad>();
    CraftModuleInterface craftModuleInterface;
    if (Object.op_Equality((Object) component, (Object) null))
    {
      craftModuleInterface = this.target.GetComponent<Clustercraft>().ModuleInterface;
    }
    else
    {
      RocketModuleCluster landedRocket = component.LandedRocket;
      if (Object.op_Equality((Object) landedRocket, (Object) null))
        return ProcessCondition.Status.Ready;
      craftModuleInterface = landedRocket.CraftInterface;
    }
    return !craftModuleInterface.HasCargoModule || this.target.HasTag(GameTags.TransferringCargoComplete) ? ProcessCondition.Status.Ready : ProcessCondition.Status.Warning;
  }

  public override string GetStatusMessage(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.LAUNCHCHECKLIST.CARGO_TRANSFER_COMPLETE.STATUS.READY : (string) UI.STARMAP.LAUNCHCHECKLIST.CARGO_TRANSFER_COMPLETE.STATUS.WARNING;

  public override string GetStatusTooltip(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.LAUNCHCHECKLIST.CARGO_TRANSFER_COMPLETE.TOOLTIP.READY : (string) UI.STARMAP.LAUNCHCHECKLIST.CARGO_TRANSFER_COMPLETE.TOOLTIP.WARNING;

  public override bool ShowInUI() => true;
}
