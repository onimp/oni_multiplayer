// Decompiled with JetBrains decompiler
// Type: ConditionPassengersOnBoard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class ConditionPassengersOnBoard : ProcessCondition
{
  private PassengerRocketModule module;

  public ConditionPassengersOnBoard(PassengerRocketModule module) => this.module = module;

  public override ProcessCondition.Status EvaluateCondition()
  {
    Tuple<int, int> crewBoardedFraction = this.module.GetCrewBoardedFraction();
    return crewBoardedFraction.first != crewBoardedFraction.second ? ProcessCondition.Status.Failure : ProcessCondition.Status.Ready;
  }

  public override string GetStatusMessage(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.READY : (string) UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.FAILURE;

  public override string GetStatusTooltip(ProcessCondition.Status status)
  {
    Tuple<int, int> crewBoardedFraction = this.module.GetCrewBoardedFraction();
    return status == ProcessCondition.Status.Ready ? (crewBoardedFraction.second != 0 ? string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.TOOLTIP.READY, (object) crewBoardedFraction.first, (object) crewBoardedFraction.second) : string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.TOOLTIP.NONE, (object) crewBoardedFraction.first, (object) crewBoardedFraction.second)) : (crewBoardedFraction.first == 0 ? string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.TOOLTIP.FAILURE, (object) crewBoardedFraction.first, (object) crewBoardedFraction.second) : string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.CREW_BOARDED.TOOLTIP.WARNING, (object) crewBoardedFraction.first, (object) crewBoardedFraction.second));
  }

  public override bool ShowInUI() => true;
}
