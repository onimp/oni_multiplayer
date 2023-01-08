// Decompiled with JetBrains decompiler
// Type: LoadingCompleteCondition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class LoadingCompleteCondition : ProcessCondition
{
  private Storage target;
  private IUserControlledCapacity userControlledTarget;

  public LoadingCompleteCondition(Storage target)
  {
    this.target = target;
    this.userControlledTarget = ((Component) target).GetComponent<IUserControlledCapacity>();
  }

  public override ProcessCondition.Status EvaluateCondition() => this.userControlledTarget != null ? ((double) this.userControlledTarget.AmountStored < (double) this.userControlledTarget.UserMaxCapacity ? ProcessCondition.Status.Warning : ProcessCondition.Status.Ready) : (!this.target.IsFull() ? ProcessCondition.Status.Warning : ProcessCondition.Status.Ready);

  public override string GetStatusMessage(ProcessCondition.Status status) => (string) (status == ProcessCondition.Status.Ready ? UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.STATUS.READY : UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.STATUS.WARNING);

  public override string GetStatusTooltip(ProcessCondition.Status status) => (string) (status == ProcessCondition.Status.Ready ? UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.TOOLTIP.READY : UI.STARMAP.LAUNCHCHECKLIST.LOADING_COMPLETE.TOOLTIP.WARNING);

  public override bool ShowInUI() => true;
}
