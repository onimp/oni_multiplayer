// Decompiled with JetBrains decompiler
// Type: ConditionSufficientFood
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class ConditionSufficientFood : ProcessCondition
{
  private CommandModule module;

  public ConditionSufficientFood(CommandModule module) => this.module = module;

  public override ProcessCondition.Status EvaluateCondition() => (double) this.module.storage.GetAmountAvailable(GameTags.Edible) <= 1.0 ? ProcessCondition.Status.Failure : ProcessCondition.Status.Ready;

  public override string GetStatusMessage(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.HASFOOD.NAME : (string) UI.STARMAP.NOFOOD.NAME;

  public override string GetStatusTooltip(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.HASFOOD.TOOLTIP : (string) UI.STARMAP.NOFOOD.TOOLTIP;

  public override bool ShowInUI() => true;
}
