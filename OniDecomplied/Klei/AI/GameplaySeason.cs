// Decompiled with JetBrains decompiler
// Type: Klei.AI.GameplaySeason
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
  [DebuggerDisplay("{base.Id}")]
  public class GameplaySeason : Resource
  {
    public const float DEFAULT_PERCENTAGE_RANDOMIZED_EVENT_START = 0.05f;
    public const float PERCENTAGE_WARNING = 0.4f;
    public const float USE_DEFAULT = -1f;
    public const int INFINITE = -1;
    public float period;
    public bool synchronizedToPeriod;
    public MathUtil.MinMax randomizedEventStartTime;
    public int finishAfterNumEvents = -1;
    public bool startActive;
    public int numEventsToStartEachPeriod;
    public float minCycle;
    public float maxCycle;
    public List<GameplayEvent> events;
    public GameplaySeason.Type type;
    public string dlcId;

    public GameplaySeason(
      string id,
      GameplaySeason.Type type,
      string dlcId,
      float period,
      bool synchronizedToPeriod,
      float randomizedEventStartTime = -1f,
      bool startActive = false,
      int finishAfterNumEvents = -1,
      float minCycle = 0.0f,
      float maxCycle = float.PositiveInfinity,
      int numEventsToStartEachPeriod = 1)
      : base(id, (ResourceSet) null, (string) null)
    {
      this.type = type;
      this.dlcId = dlcId;
      this.period = period;
      this.synchronizedToPeriod = synchronizedToPeriod;
      Debug.Assert((double) period > 0.0, (object) ("Season " + id + "'s Period cannot be 0 or negative"));
      if ((double) randomizedEventStartTime == -1.0)
      {
        this.randomizedEventStartTime = new MathUtil.MinMax(-0.05f * period, 0.05f * period);
      }
      else
      {
        this.randomizedEventStartTime = new MathUtil.MinMax(-randomizedEventStartTime, randomizedEventStartTime);
        DebugUtil.DevAssert(((double) ((MathUtil.MinMax) ref this.randomizedEventStartTime).max - (double) ((MathUtil.MinMax) ref this.randomizedEventStartTime).min) * 0.40000000596046448 < (double) period, string.Format("Season {0} randomizedEventStartTime is greater than {1}% of its period.", (object) id, (object) 0.4f), (Object) null);
      }
      this.startActive = startActive;
      this.finishAfterNumEvents = finishAfterNumEvents;
      this.minCycle = minCycle;
      this.maxCycle = maxCycle;
      this.events = new List<GameplayEvent>();
      this.numEventsToStartEachPeriod = numEventsToStartEachPeriod;
    }

    public GameplaySeason AddEvent(GameplayEvent evt)
    {
      this.events.Add(evt);
      return this;
    }

    public GameplaySeasonInstance Instantiate(int worldId) => new GameplaySeasonInstance(this, worldId);

    public enum Type
    {
      World,
      Cluster,
    }
  }
}
