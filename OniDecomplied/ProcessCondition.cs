// Decompiled with JetBrains decompiler
// Type: ProcessCondition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public abstract class ProcessCondition
{
  protected ProcessCondition parentCondition;

  public abstract ProcessCondition.Status EvaluateCondition();

  public abstract bool ShowInUI();

  public abstract string GetStatusMessage(ProcessCondition.Status status);

  public string GetStatusMessage() => this.GetStatusMessage(this.EvaluateCondition());

  public abstract string GetStatusTooltip(ProcessCondition.Status status);

  public string GetStatusTooltip() => this.GetStatusTooltip(this.EvaluateCondition());

  public virtual StatusItem GetStatusItem(ProcessCondition.Status status) => (StatusItem) null;

  public virtual ProcessCondition GetParentCondition() => this.parentCondition;

  public enum ProcessConditionType
  {
    RocketFlight,
    RocketPrep,
    RocketStorage,
    RocketBoard,
    All,
  }

  public enum Status
  {
    Failure,
    Warning,
    Ready,
  }
}
