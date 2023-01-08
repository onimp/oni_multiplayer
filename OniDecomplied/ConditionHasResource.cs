// Decompiled with JetBrains decompiler
// Type: ConditionHasResource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class ConditionHasResource : ProcessCondition
{
  private Storage storage;
  private SimHashes resource;
  private float thresholdMass;

  public ConditionHasResource(Storage storage, SimHashes resource, float thresholdMass)
  {
    this.storage = storage;
    this.resource = resource;
    this.thresholdMass = thresholdMass;
  }

  public override ProcessCondition.Status EvaluateCondition() => (double) this.storage.GetAmountAvailable(this.resource.CreateTag()) < (double) this.thresholdMass ? ProcessCondition.Status.Warning : ProcessCondition.Status.Ready;

  public override string GetStatusMessage(ProcessCondition.Status status)
  {
    string statusMessage;
    switch (status)
    {
      case ProcessCondition.Status.Failure:
        statusMessage = string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.STATUS.FAILURE, (object) ((Component) this.storage).GetProperName(), (object) ElementLoader.GetElement(this.resource.CreateTag()).name);
        break;
      case ProcessCondition.Status.Ready:
        statusMessage = string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.STATUS.READY, (object) ((Component) this.storage).GetProperName(), (object) ElementLoader.GetElement(this.resource.CreateTag()).name);
        break;
      default:
        statusMessage = string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.STATUS.WARNING, (object) ((Component) this.storage).GetProperName(), (object) ElementLoader.GetElement(this.resource.CreateTag()).name);
        break;
    }
    return statusMessage;
  }

  public override string GetStatusTooltip(ProcessCondition.Status status)
  {
    string statusTooltip;
    switch (status)
    {
      case ProcessCondition.Status.Failure:
        statusTooltip = string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.TOOLTIP.FAILURE, (object) ((Component) this.storage).GetProperName(), (object) GameUtil.GetFormattedMass(this.thresholdMass), (object) ElementLoader.GetElement(this.resource.CreateTag()).name);
        break;
      case ProcessCondition.Status.Ready:
        statusTooltip = string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.TOOLTIP.READY, (object) ((Component) this.storage).GetProperName(), (object) ElementLoader.GetElement(this.resource.CreateTag()).name);
        break;
      default:
        statusTooltip = string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.HAS_RESOURCE.TOOLTIP.WARNING, (object) ((Component) this.storage).GetProperName(), (object) GameUtil.GetFormattedMass(this.thresholdMass), (object) ElementLoader.GetElement(this.resource.CreateTag()).name);
        break;
    }
    return statusTooltip;
  }

  public override bool ShowInUI() => true;
}
