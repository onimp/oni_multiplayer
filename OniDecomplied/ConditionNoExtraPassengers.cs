// Decompiled with JetBrains decompiler
// Type: ConditionNoExtraPassengers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class ConditionNoExtraPassengers : ProcessCondition
{
  private PassengerRocketModule module;

  public ConditionNoExtraPassengers(PassengerRocketModule module) => this.module = module;

  public override ProcessCondition.Status EvaluateCondition() => !this.module.CheckExtraPassengers() ? ProcessCondition.Status.Ready : ProcessCondition.Status.Failure;

  public override string GetStatusMessage(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.LAUNCHCHECKLIST.NO_EXTRA_PASSENGERS.READY : (string) UI.STARMAP.LAUNCHCHECKLIST.NO_EXTRA_PASSENGERS.FAILURE;

  public override string GetStatusTooltip(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.LAUNCHCHECKLIST.NO_EXTRA_PASSENGERS.TOOLTIP.READY : (string) UI.STARMAP.LAUNCHCHECKLIST.NO_EXTRA_PASSENGERS.TOOLTIP.FAILURE;

  public override bool ShowInUI() => true;
}
