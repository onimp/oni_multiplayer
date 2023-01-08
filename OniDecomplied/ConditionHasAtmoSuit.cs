// Decompiled with JetBrains decompiler
// Type: ConditionHasAtmoSuit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class ConditionHasAtmoSuit : ProcessCondition
{
  private CommandModule module;

  public ConditionHasAtmoSuit(CommandModule module)
  {
    this.module = module;
    ManualDeliveryKG orAdd = this.module.FindOrAdd<ManualDeliveryKG>();
    orAdd.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    orAdd.SetStorage(module.storage);
    orAdd.RequestedItemTag = GameTags.AtmoSuit;
    orAdd.MinimumMass = 1f;
    orAdd.refillMass = 0.1f;
    orAdd.capacity = 1f;
  }

  public override ProcessCondition.Status EvaluateCondition() => (double) this.module.storage.GetAmountAvailable(GameTags.AtmoSuit) < 1.0 ? ProcessCondition.Status.Failure : ProcessCondition.Status.Ready;

  public override string GetStatusMessage(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.HASSUIT.NAME : (string) UI.STARMAP.NOSUIT.NAME;

  public override string GetStatusTooltip(ProcessCondition.Status status) => status == ProcessCondition.Status.Ready ? (string) UI.STARMAP.HASSUIT.TOOLTIP : (string) UI.STARMAP.NOSUIT.TOOLTIP;

  public override bool ShowInUI() => true;
}
