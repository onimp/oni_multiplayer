// Decompiled with JetBrains decompiler
// Type: InternalConstructionCompleteCondition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class InternalConstructionCompleteCondition : ProcessCondition
{
  private BuildingInternalConstructor.Instance target;

  public InternalConstructionCompleteCondition(BuildingInternalConstructor.Instance target) => this.target = target;

  public override ProcessCondition.Status EvaluateCondition() => this.target.IsRequestingConstruction() && !this.target.HasOutputInStorage() ? ProcessCondition.Status.Warning : ProcessCondition.Status.Ready;

  public override string GetStatusMessage(ProcessCondition.Status status) => (string) (status == ProcessCondition.Status.Ready ? UI.STARMAP.LAUNCHCHECKLIST.INTERNAL_CONSTRUCTION_COMPLETE.STATUS.READY : UI.STARMAP.LAUNCHCHECKLIST.INTERNAL_CONSTRUCTION_COMPLETE.STATUS.FAILURE);

  public override string GetStatusTooltip(ProcessCondition.Status status) => (string) (status == ProcessCondition.Status.Ready ? UI.STARMAP.LAUNCHCHECKLIST.INTERNAL_CONSTRUCTION_COMPLETE.TOOLTIP.READY : UI.STARMAP.LAUNCHCHECKLIST.INTERNAL_CONSTRUCTION_COMPLETE.TOOLTIP.FAILURE);

  public override bool ShowInUI() => true;
}
