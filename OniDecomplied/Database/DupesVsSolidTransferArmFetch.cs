// Decompiled with JetBrains decompiler
// Type: Database.DupesVsSolidTransferArmFetch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
  public class DupesVsSolidTransferArmFetch : 
    ColonyAchievementRequirement,
    AchievementRequirementSerialization_Deprecated
  {
    public float percentage;
    public int numCycles;
    public int currentCycleCount;
    public bool armsOutPerformingDupesThisCycle;

    public DupesVsSolidTransferArmFetch(float percentage, int numCycles)
    {
      this.percentage = percentage;
      this.numCycles = numCycles;
    }

    public override bool Success()
    {
      Dictionary<int, int> dupeChoreDeliveries = ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().fetchDupeChoreDeliveries;
      Dictionary<int, int> automatedChoreDeliveries = ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().fetchAutomatedChoreDeliveries;
      int val2 = 0;
      this.currentCycleCount = 0;
      for (int key = GameClock.Instance.GetCycle() - 1; key >= GameClock.Instance.GetCycle() - this.numCycles; --key)
      {
        if (automatedChoreDeliveries.ContainsKey(key))
        {
          if (!dupeChoreDeliveries.ContainsKey(key) || (double) dupeChoreDeliveries[key] < (double) automatedChoreDeliveries[key] * (double) this.percentage)
            ++val2;
          else
            break;
        }
        else if (dupeChoreDeliveries.ContainsKey(key))
        {
          val2 = 0;
          break;
        }
      }
      this.currentCycleCount = Math.Max(this.currentCycleCount, val2);
      return val2 >= this.numCycles;
    }

    public void Deserialize(IReader reader)
    {
      this.numCycles = reader.ReadInt32();
      this.percentage = reader.ReadSingle();
    }
  }
}
