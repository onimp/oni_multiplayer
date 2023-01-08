// Decompiled with JetBrains decompiler
// Type: DecorMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DecorMonitor : GameStateMachine<DecorMonitor, DecorMonitor.Instance>
{
  public static float MAXIMUM_DECOR_VALUE = 120f;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.ToggleAttributeModifier("DecorSmoother", (Func<DecorMonitor.Instance, AttributeModifier>) (smi => smi.GetDecorModifier()), (Func<DecorMonitor.Instance, bool>) (smi => true)).Update("DecorSensing", (System.Action<DecorMonitor.Instance, float>) ((smi, dt) => smi.Update(dt))).EventHandler(GameHashes.NewDay, (Func<DecorMonitor.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), (StateMachine<DecorMonitor, DecorMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.OnNewDay()));
  }

  public new class Instance : 
    GameStateMachine<DecorMonitor, DecorMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    [Serialize]
    private float cycleTotalDecor;
    [Serialize]
    private float yesterdaysTotalDecor;
    private AmountInstance amount;
    private AttributeModifier modifier;
    private List<KeyValuePair<float, string>> effectLookup = new List<KeyValuePair<float, string>>()
    {
      new KeyValuePair<float, string>(DecorMonitor.MAXIMUM_DECOR_VALUE * -0.25f, "DecorMinus1"),
      new KeyValuePair<float, string>(DecorMonitor.MAXIMUM_DECOR_VALUE * 0.0f, "Decor0"),
      new KeyValuePair<float, string>(DecorMonitor.MAXIMUM_DECOR_VALUE * 0.25f, "Decor1"),
      new KeyValuePair<float, string>(DecorMonitor.MAXIMUM_DECOR_VALUE * 0.5f, "Decor2"),
      new KeyValuePair<float, string>(DecorMonitor.MAXIMUM_DECOR_VALUE * 0.75f, "Decor3"),
      new KeyValuePair<float, string>(DecorMonitor.MAXIMUM_DECOR_VALUE, "Decor4"),
      new KeyValuePair<float, string>(float.MaxValue, "Decor5")
    };

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.cycleTotalDecor = 2250f;
      this.amount = Db.Get().Amounts.Decor.Lookup(this.gameObject);
      this.modifier = new AttributeModifier(Db.Get().Amounts.Decor.deltaAttribute.Id, 1f, (string) DUPLICANTS.NEEDS.DECOR.OBSERVED_DECOR, is_readonly: false);
    }

    public AttributeModifier GetDecorModifier() => this.modifier;

    public void Update(float dt)
    {
      int cell = Grid.PosToCell(this.gameObject);
      if (!Grid.IsValidCell(cell))
        return;
      float decorAtCell = GameUtil.GetDecorAtCell(cell);
      this.cycleTotalDecor += decorAtCell * dt;
      float num1 = 0.0f;
      float num2 = 4.16666651f;
      if ((double) Mathf.Abs(decorAtCell - this.amount.value) > 0.5)
      {
        if ((double) decorAtCell > (double) this.amount.value)
          num1 = 3f * num2;
        else if ((double) decorAtCell < (double) this.amount.value)
          num1 = -num2;
      }
      else
        this.amount.value = decorAtCell;
      this.modifier.SetValue(num1);
    }

    public void OnNewDay()
    {
      this.yesterdaysTotalDecor = this.cycleTotalDecor;
      this.cycleTotalDecor = 0.0f;
      float num = this.yesterdaysTotalDecor / 600f + this.gameObject.GetAttributes().Add(Db.Get().Attributes.DecorExpectation).GetTotalValue();
      Effects component = this.gameObject.GetComponent<Effects>();
      foreach (KeyValuePair<float, string> keyValuePair in this.effectLookup)
      {
        if ((double) num < (double) keyValuePair.Key)
        {
          component.Add(keyValuePair.Value, true);
          break;
        }
      }
    }

    public float GetTodaysAverageDecor() => this.cycleTotalDecor / (GameClock.Instance.GetCurrentCycleAsPercentage() * 600f);

    public float GetYesterdaysAverageDecor() => this.yesterdaysTotalDecor / 600f;
  }
}
