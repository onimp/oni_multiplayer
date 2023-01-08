// Decompiled with JetBrains decompiler
// Type: ConditionHasAstronaut
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class ConditionHasAstronaut : ProcessCondition
{
  private CommandModule module;

  public ConditionHasAstronaut(CommandModule module) => this.module = module;

  public override ProcessCondition.Status EvaluateCondition()
  {
    List<MinionStorage.Info> storedMinionInfo = ((Component) this.module).GetComponent<MinionStorage>().GetStoredMinionInfo();
    return storedMinionInfo.Count > 0 && storedMinionInfo[0].serializedMinion != null ? ProcessCondition.Status.Ready : ProcessCondition.Status.Failure;
  }

  public override string GetStatusMessage(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUT_TITLE : (string) UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUGHT;

  public override string GetStatusTooltip(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.LAUNCHCHECKLIST.HASASTRONAUT : (string) UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUGHT;

  public override bool ShowInUI() => true;
}
