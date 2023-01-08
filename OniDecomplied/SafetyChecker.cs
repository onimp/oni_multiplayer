// Decompiled with JetBrains decompiler
// Type: SafetyChecker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class SafetyChecker
{
  public SafetyChecker.Condition[] conditions { get; private set; }

  public SafetyChecker(SafetyChecker.Condition[] conditions) => this.conditions = conditions;

  public int GetSafetyConditions(
    int cell,
    int cost,
    SafetyChecker.Context context,
    out bool all_conditions_met)
  {
    int safetyConditions = 0;
    int num = 0;
    for (int index = 0; index < this.conditions.Length; ++index)
    {
      SafetyChecker.Condition condition = this.conditions[index];
      if (condition.callback(cell, cost, context))
      {
        safetyConditions |= condition.mask;
        ++num;
      }
    }
    all_conditions_met = num == this.conditions.Length;
    return safetyConditions;
  }

  public struct Condition
  {
    public SafetyChecker.Condition.Callback callback { get; private set; }

    public int mask { get; private set; }

    public Condition(
      string id,
      int condition_mask,
      SafetyChecker.Condition.Callback condition_callback)
      : this()
    {
      this.callback = condition_callback;
      this.mask = condition_mask;
    }

    public delegate bool Callback(int cell, int cost, SafetyChecker.Context context);
  }

  public struct Context
  {
    public Navigator navigator;
    public OxygenBreather oxygenBreather;
    public SimTemperatureTransfer temperatureTransferer;
    public PrimaryElement primaryElement;
    public MinionBrain minionBrain;
    public int cell;

    public Context(KMonoBehaviour cmp)
    {
      this.cell = Grid.PosToCell(cmp);
      this.navigator = ((Component) cmp).GetComponent<Navigator>();
      this.oxygenBreather = ((Component) cmp).GetComponent<OxygenBreather>();
      this.minionBrain = ((Component) cmp).GetComponent<MinionBrain>();
      this.temperatureTransferer = ((Component) cmp).GetComponent<SimTemperatureTransfer>();
      this.primaryElement = ((Component) cmp).GetComponent<PrimaryElement>();
    }
  }
}
