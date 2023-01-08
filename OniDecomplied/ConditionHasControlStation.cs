// Decompiled with JetBrains decompiler
// Type: ConditionHasControlStation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class ConditionHasControlStation : ProcessCondition
{
  private RocketModuleCluster module;

  public ConditionHasControlStation(RocketModuleCluster module) => this.module = module;

  public override ProcessCondition.Status EvaluateCondition() => Components.RocketControlStations.GetWorldItems(((Component) this.module.CraftInterface).GetComponent<WorldContainer>().id).Count <= 0 ? ProcessCondition.Status.Failure : ProcessCondition.Status.Ready;

  public override string GetStatusMessage(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.LAUNCHCHECKLIST.HAS_CONTROLSTATION.STATUS.READY : (string) UI.STARMAP.LAUNCHCHECKLIST.HAS_CONTROLSTATION.STATUS.FAILURE;

  public override string GetStatusTooltip(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.LAUNCHCHECKLIST.HAS_CONTROLSTATION.TOOLTIP.READY : (string) UI.STARMAP.LAUNCHCHECKLIST.HAS_CONTROLSTATION.TOOLTIP.FAILURE;

  public override bool ShowInUI() => this.EvaluateCondition() == ProcessCondition.Status.Failure;
}
