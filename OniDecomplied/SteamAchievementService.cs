// Decompiled with JetBrains decompiler
// Type: SteamAchievementService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Steamworks;
using System.Diagnostics;
using UnityEngine;

public class SteamAchievementService : MonoBehaviour
{
  private Callback<UserStatsReceived_t> cbUserStatsReceived;
  private Callback<UserStatsStored_t> cbUserStatsStored;
  private Callback<UserAchievementStored_t> cbUserAchievementStored;
  private bool setupComplete;
  private static SteamAchievementService instance;

  public static SteamAchievementService Instance => SteamAchievementService.instance;

  public static void Initialize()
  {
    if (!Object.op_Equality((Object) SteamAchievementService.instance, (Object) null))
      return;
    GameObject gameObject = GameObject.Find("/SteamManager");
    SteamAchievementService.instance = gameObject.GetComponent<SteamAchievementService>();
    if (!Object.op_Equality((Object) SteamAchievementService.instance, (Object) null))
      return;
    SteamAchievementService.instance = gameObject.AddComponent<SteamAchievementService>();
  }

  public void Awake()
  {
    this.setupComplete = false;
    Debug.Assert(Object.op_Equality((Object) SteamAchievementService.instance, (Object) null));
    SteamAchievementService.instance = this;
  }

  private void OnDestroy()
  {
    Debug.Assert(Object.op_Equality((Object) SteamAchievementService.instance, (Object) this));
    SteamAchievementService.instance = (SteamAchievementService) null;
  }

  private void Update()
  {
    if (!SteamManager.Initialized || Object.op_Inequality((Object) Game.Instance, (Object) null) || this.setupComplete || !DistributionPlatform.Initialized)
      return;
    this.Setup();
  }

  private void Setup()
  {
    // ISSUE: method pointer
    this.cbUserStatsReceived = Callback<UserStatsReceived_t>.Create(new Callback<UserStatsReceived_t>.DispatchDelegate((object) this, __methodptr(OnUserStatsReceived)));
    // ISSUE: method pointer
    this.cbUserStatsStored = Callback<UserStatsStored_t>.Create(new Callback<UserStatsStored_t>.DispatchDelegate((object) this, __methodptr(OnUserStatsStored)));
    // ISSUE: method pointer
    this.cbUserAchievementStored = Callback<UserAchievementStored_t>.Create(new Callback<UserAchievementStored_t>.DispatchDelegate((object) this, __methodptr(OnUserAchievementStored)));
    this.setupComplete = true;
    this.RefreshStats();
  }

  private void RefreshStats() => SteamUserStats.RequestCurrentStats();

  private void OnUserStatsReceived(UserStatsReceived_t data)
  {
    if (data.m_eResult == 1)
      return;
    DebugUtil.LogWarningArgs(new object[3]
    {
      (object) nameof (OnUserStatsReceived),
      (object) data.m_eResult,
      (object) data.m_steamIDUser
    });
  }

  private void OnUserStatsStored(UserStatsStored_t data)
  {
    if (data.m_eResult == 1)
      return;
    DebugUtil.LogWarningArgs(new object[2]
    {
      (object) nameof (OnUserStatsStored),
      (object) data.m_eResult
    });
  }

  private void OnUserAchievementStored(UserAchievementStored_t data)
  {
  }

  public void Unlock(string achievement_id)
  {
    bool flag = SteamUserStats.SetAchievement(achievement_id);
    Debug.LogFormat("SetAchievement {0} {1}", new object[2]
    {
      (object) achievement_id,
      (object) flag
    });
    Debug.LogFormat("StoreStats {0}", new object[1]
    {
      (object) SteamUserStats.StoreStats()
    });
  }

  [Conditional("UNITY_EDITOR")]
  [ContextMenu("Reset All Achievements")]
  private void ResetAllAchievements()
  {
    bool flag = SteamUserStats.ResetAllStats(true);
    Debug.LogFormat("ResetAllStats {0}", new object[1]
    {
      (object) flag
    });
    if (!flag)
      return;
    this.RefreshStats();
  }
}
