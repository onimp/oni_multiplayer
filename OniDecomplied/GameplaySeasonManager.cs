// Decompiled with JetBrains decompiler
// Type: GameplaySeasonManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplaySeasonManager : 
  GameStateMachine<GameplaySeasonManager, GameplaySeasonManager.Instance, IStateMachineTarget, GameplaySeasonManager.Def>
{
  public override void InitializeStates(out StateMachine.BaseState defaultState)
  {
    defaultState = (StateMachine.BaseState) this.root;
    this.root.Enter((StateMachine<GameplaySeasonManager, GameplaySeasonManager.Instance, IStateMachineTarget, GameplaySeasonManager.Def>.State.Callback) (smi => smi.Initialize())).Update((System.Action<GameplaySeasonManager.Instance, float>) ((smi, dt) => smi.Update(dt)), (UpdateRate) 7);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<GameplaySeasonManager, GameplaySeasonManager.Instance, IStateMachineTarget, GameplaySeasonManager.Def>.GameInstance
  {
    [Serialize]
    public List<GameplaySeasonInstance> activeSeasons;
    [MyCmpGet]
    private WorldContainer m_worldContainer;

    public Instance(IStateMachineTarget master, GameplaySeasonManager.Def def)
      : base(master, def)
    {
      this.activeSeasons = new List<GameplaySeasonInstance>();
    }

    public void Initialize()
    {
      this.activeSeasons.RemoveAll((Predicate<GameplaySeasonInstance>) (item => item.Season == null));
      List<GameplaySeason> gameplaySeasonList = new List<GameplaySeason>();
      if (Object.op_Inequality((Object) this.m_worldContainer, (Object) null))
      {
        ClusterGridEntity component = this.GetComponent<ClusterGridEntity>();
        foreach (string seasonId in this.m_worldContainer.GetSeasonIds())
        {
          GameplaySeason gameplaySeason = Db.Get().GameplaySeasons.TryGet(seasonId);
          if (gameplaySeason == null)
          {
            Debug.LogWarning((object) ("world " + ((Object) component).name + " has invalid season " + seasonId));
          }
          else
          {
            if (gameplaySeason.type != GameplaySeason.Type.World)
              Debug.LogWarning((object) ("world " + ((Object) component).name + " has specified season " + seasonId + ", which is not a world type season"));
            gameplaySeasonList.Add(gameplaySeason);
          }
        }
      }
      else
      {
        Debug.Assert(Object.op_Inequality((Object) this.GetComponent<SaveGame>(), (Object) null));
        gameplaySeasonList = Db.Get().GameplaySeasons.resources.Where<GameplaySeason>((Func<GameplaySeason, bool>) (season => season.type == GameplaySeason.Type.Cluster)).ToList<GameplaySeason>();
      }
      foreach (GameplaySeason seasonType in gameplaySeasonList)
      {
        if (DlcManager.IsContentActive(seasonType.dlcId) && seasonType.startActive && !this.SeasonExists(seasonType))
          this.activeSeasons.Add(seasonType.Instantiate(this.GetWorldId()));
      }
      foreach (GameplaySeasonInstance gameplaySeasonInstance in new List<GameplaySeasonInstance>((IEnumerable<GameplaySeasonInstance>) this.activeSeasons))
      {
        if (!gameplaySeasonList.Contains(gameplaySeasonInstance.Season) || !DlcManager.IsContentActive(gameplaySeasonInstance.Season.dlcId))
          this.activeSeasons.Remove(gameplaySeasonInstance);
      }
    }

    private int GetWorldId() => Object.op_Inequality((Object) this.m_worldContainer, (Object) null) ? this.m_worldContainer.id : -1;

    public void Update(float dt)
    {
      using (List<GameplaySeasonInstance>.Enumerator enumerator = this.activeSeasons.GetEnumerator())
      {
label_6:
        while (enumerator.MoveNext())
        {
          GameplaySeasonInstance current = enumerator.Current;
          if (current.ShouldGenerateEvents() && (double) GameUtil.GetCurrentTimeInCycles() > (double) current.NextEventTime)
          {
            int num = 0;
            while (true)
            {
              if (num < current.Season.numEventsToStartEachPeriod && current.StartEvent())
                ++num;
              else
                goto label_6;
            }
          }
        }
      }
    }

    public void StartNewSeason(GameplaySeason seasonType)
    {
      if (!DlcManager.IsContentActive(seasonType.dlcId))
        return;
      this.activeSeasons.Add(seasonType.Instantiate(this.GetWorldId()));
    }

    public bool SeasonExists(GameplaySeason seasonType) => this.activeSeasons.Find((Predicate<GameplaySeasonInstance>) (e => HashedString.op_Equality(e.Season.IdHash, seasonType.IdHash))) != null;
  }
}
