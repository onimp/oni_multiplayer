// Decompiled with JetBrains decompiler
// Type: ConditionPilotOnBoard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class ConditionPilotOnBoard : ProcessCondition
{
  private PassengerRocketModule module;

  public ConditionPilotOnBoard(PassengerRocketModule module) => this.module = module;

  public override ProcessCondition.Status EvaluateCondition() => !this.module.CheckPilotBoarded() ? ProcessCondition.Status.Failure : ProcessCondition.Status.Ready;

  public override string GetStatusMessage(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.LAUNCHCHECKLIST.PILOT_BOARDED.READY : (string) UI.STARMAP.LAUNCHCHECKLIST.PILOT_BOARDED.FAILURE;

  public override string GetStatusTooltip(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.LAUNCHCHECKLIST.PILOT_BOARDED.TOOLTIP.READY : (string) UI.STARMAP.LAUNCHCHECKLIST.PILOT_BOARDED.TOOLTIP.FAILURE;

  public override bool ShowInUI() => true;
}
