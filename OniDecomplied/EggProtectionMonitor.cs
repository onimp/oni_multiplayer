// Decompiled with JetBrains decompiler
// Type: EggProtectionMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class EggProtectionMonitor : 
  GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>
{
  public StateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.BoolParameter hasEggToGuard;
  public GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.State find_egg;
  public EggProtectionMonitor.GuardEggStates guard;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.find_egg;
    this.root.EventHandler(GameHashes.ObjectDestroyed, (GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.GameEvent.Callback) ((smi, d) => smi.Cleanup(d)));
    // ISSUE: method pointer
    this.find_egg.BatchUpdate(new UpdateBucketWithUpdater<EggProtectionMonitor.Instance>.BatchUpdateDelegate((object) null, __methodptr(FindEggToGuard))).ParamTransition<bool>((StateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.Parameter<bool>) this.hasEggToGuard, this.guard.safe, GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.IsTrue);
    this.guard.Enter((StateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.State.Callback) (smi =>
    {
      smi.gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit("pincher_kanim")), smi.def.animPrefix, "_heat");
      smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Hostile);
    })).Exit((StateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.State.Callback) (smi =>
    {
      if (!Util.IsNullOrWhiteSpace(smi.def.animPrefix))
        smi.gameObject.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit("pincher_kanim")), smi.def.animPrefix);
      else
        smi.gameObject.AddOrGet<SymbolOverrideController>().RemoveBuildOverride(Assets.GetAnim(HashedString.op_Implicit("pincher_kanim")).GetData());
      smi.gameObject.AddOrGet<FactionAlignment>().SwitchAlignment(FactionManager.FactionID.Pest);
    })).Update("evaulate_egg", (System.Action<EggProtectionMonitor.Instance, float>) ((smi, dt) => smi.CanProtectEgg()), (UpdateRate) 6, true).ParamTransition<bool>((StateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.Parameter<bool>) this.hasEggToGuard, this.find_egg, GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.IsFalse);
    this.guard.safe.Enter((StateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.State.Callback) (smi => smi.RefreshThreat((object) null))).Update("safe", (System.Action<EggProtectionMonitor.Instance, float>) ((smi, dt) => smi.RefreshThreat((object) null)), load_balance: true).ToggleStatusItem((string) CREATURES.STATUSITEMS.PROTECTINGENTITY.NAME, (string) CREATURES.STATUSITEMS.PROTECTINGENTITY.TOOLTIP, render_overlay: new HashedString());
    this.guard.threatened.ToggleBehaviour(GameTags.Creatures.Defend, (StateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.Transition.ConditionCallback) (smi => Object.op_Inequality((Object) smi.MainThreat, (Object) null)), (System.Action<EggProtectionMonitor.Instance>) (smi => smi.GoTo((StateMachine.BaseState) this.guard.safe))).Update("Threatened", new System.Action<EggProtectionMonitor.Instance, float>(EggProtectionMonitor.CritterUpdateThreats));
  }

  private static void CritterUpdateThreats(EggProtectionMonitor.Instance smi, float dt)
  {
    if (smi.isMasterNull || smi.CheckForThreats())
      return;
    smi.GoTo((StateMachine.BaseState) smi.sm.guard.safe);
  }

  public class Def : StateMachine.BaseDef
  {
    public Tag[] allyTags;
    public string animPrefix;
  }

  public class GuardEggStates : 
    GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.State
  {
    public GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.State safe;
    public GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.State threatened;
  }

  public new class Instance : 
    GameStateMachine<EggProtectionMonitor, EggProtectionMonitor.Instance, IStateMachineTarget, EggProtectionMonitor.Def>.GameInstance
  {
    public GameObject eggToProtect;
    public FactionAlignment alignment;
    private Navigator navigator;
    private GameObject mainThreat;
    private List<FactionAlignment> threats = new List<FactionAlignment>();
    private int maxThreatDistance = 12;
    private System.Action<object> refreshThreatDelegate;
    private static WorkItemCollection<EggProtectionMonitor.Instance.FindEggsTask, List<KPrefabID>> find_eggs_job = new WorkItemCollection<EggProtectionMonitor.Instance.FindEggsTask, List<KPrefabID>>();

    public GameObject MainThreat => this.mainThreat;

    public Instance(IStateMachineTarget master, EggProtectionMonitor.Def def)
      : base(master, def)
    {
      this.alignment = master.GetComponent<FactionAlignment>();
      this.navigator = master.GetComponent<Navigator>();
      this.refreshThreatDelegate = new System.Action<object>(this.RefreshThreat);
    }

    public void CanProtectEgg()
    {
      bool flag = true;
      if (Object.op_Equality((Object) this.eggToProtect, (Object) null))
        flag = false;
      if (flag)
      {
        int num = 150;
        int navigationCost = this.navigator.GetNavigationCost(Grid.PosToCell(this.eggToProtect));
        if (navigationCost == -1 || navigationCost >= num)
          flag = false;
      }
      if (flag)
        return;
      this.SetEggToGuard((GameObject) null);
    }

    public static void FindEggToGuard(
      List<UpdateBucketWithUpdater<EggProtectionMonitor.Instance>.Entry> instances,
      float time_delta)
    {
      ListPool<KPrefabID, EggProtectionMonitor>.PooledList prefab_ids = ListPool<KPrefabID, EggProtectionMonitor>.Allocate();
      ((List<KPrefabID>) prefab_ids).Capacity = Mathf.Max(((List<KPrefabID>) prefab_ids).Capacity, Components.IncubationMonitors.Count);
      foreach (IncubationMonitor.Instance incubationMonitor in Components.IncubationMonitors)
        ((List<KPrefabID>) prefab_ids).Add(incubationMonitor.gameObject.GetComponent<KPrefabID>());
      ListPool<EggProtectionMonitor.Instance.Egg, EggProtectionMonitor>.PooledList eggs = ListPool<EggProtectionMonitor.Instance.Egg, EggProtectionMonitor>.Allocate();
      EggProtectionMonitor.Instance.find_eggs_job.Reset((List<KPrefabID>) prefab_ids);
      for (int start = 0; start < ((List<KPrefabID>) prefab_ids).Count; start += 256)
        EggProtectionMonitor.Instance.find_eggs_job.Add(new EggProtectionMonitor.Instance.FindEggsTask(start, Mathf.Min(start + 256, ((List<KPrefabID>) prefab_ids).Count)));
      GlobalJobManager.Run((IWorkItemCollection) EggProtectionMonitor.Instance.find_eggs_job);
      for (int index = 0; index != EggProtectionMonitor.Instance.find_eggs_job.Count; ++index)
        EggProtectionMonitor.Instance.find_eggs_job.GetWorkItem(index).Finish((List<KPrefabID>) prefab_ids, (List<EggProtectionMonitor.Instance.Egg>) eggs);
      prefab_ids.Recycle();
      EggProtectionMonitor.Instance.find_eggs_job.Reset((List<KPrefabID>) null);
      foreach (UpdateBucketWithUpdater<EggProtectionMonitor.Instance>.Entry entry in new List<UpdateBucketWithUpdater<EggProtectionMonitor.Instance>.Entry>((IEnumerable<UpdateBucketWithUpdater<EggProtectionMonitor.Instance>.Entry>) instances))
      {
        GameObject egg1 = (GameObject) null;
        int num = 100;
        foreach (EggProtectionMonitor.Instance.Egg egg2 in (List<EggProtectionMonitor.Instance.Egg>) eggs)
        {
          int navigationCost = entry.data.navigator.GetNavigationCost(egg2.cell);
          if (navigationCost != -1 && navigationCost < num)
          {
            egg1 = egg2.game_object;
            num = navigationCost;
          }
        }
        entry.data.SetEggToGuard(egg1);
      }
      eggs.Recycle();
    }

    public void SetEggToGuard(GameObject egg)
    {
      this.eggToProtect = egg;
      this.sm.hasEggToGuard.Set(Object.op_Inequality((Object) egg, (Object) null), this.smi);
    }

    public void SetMainThreat(GameObject threat)
    {
      if (Object.op_Equality((Object) threat, (Object) this.mainThreat))
        return;
      if (Object.op_Inequality((Object) this.mainThreat, (Object) null))
      {
        KMonoBehaviourExtensions.Unsubscribe(this.mainThreat, 1623392196, this.refreshThreatDelegate);
        KMonoBehaviourExtensions.Unsubscribe(this.mainThreat, 1969584890, this.refreshThreatDelegate);
        if (Object.op_Equality((Object) threat, (Object) null))
          this.Trigger(2144432245);
      }
      if (Object.op_Inequality((Object) this.mainThreat, (Object) null))
      {
        KMonoBehaviourExtensions.Unsubscribe(this.mainThreat, 1623392196, this.refreshThreatDelegate);
        KMonoBehaviourExtensions.Unsubscribe(this.mainThreat, 1969584890, this.refreshThreatDelegate);
      }
      this.mainThreat = threat;
      if (!Object.op_Inequality((Object) this.mainThreat, (Object) null))
        return;
      KMonoBehaviourExtensions.Subscribe(this.mainThreat, 1623392196, this.refreshThreatDelegate);
      KMonoBehaviourExtensions.Subscribe(this.mainThreat, 1969584890, this.refreshThreatDelegate);
    }

    public void Cleanup(object data)
    {
      if (!Object.op_Implicit((Object) this.mainThreat))
        return;
      KMonoBehaviourExtensions.Unsubscribe(this.mainThreat, 1623392196, this.refreshThreatDelegate);
      KMonoBehaviourExtensions.Unsubscribe(this.mainThreat, 1969584890, this.refreshThreatDelegate);
    }

    public void GoToThreatened() => this.smi.GoTo((StateMachine.BaseState) this.sm.guard.threatened);

    public void RefreshThreat(object data)
    {
      if (!this.IsRunning() || Object.op_Equality((Object) this.eggToProtect, (Object) null))
        return;
      if (this.smi.CheckForThreats())
      {
        this.GoToThreatened();
      }
      else
      {
        if (this.smi.GetCurrentState() == this.sm.guard.safe)
          return;
        this.Trigger(-21431934);
        this.smi.GoTo((StateMachine.BaseState) this.sm.guard.safe);
      }
    }

    public bool CheckForThreats()
    {
      if (Object.op_Equality((Object) this.eggToProtect, (Object) null))
        return false;
      GameObject threat = this.FindThreat();
      this.SetMainThreat(threat);
      return Object.op_Inequality((Object) threat, (Object) null);
    }

    public GameObject FindThreat()
    {
      this.threats.Clear();
      ListPool<ScenePartitionerEntry, ThreatMonitor>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, ThreatMonitor>.Allocate();
      GameScenePartitioner.Instance.GatherEntries(new Extents(Grid.PosToCell(this.eggToProtect), this.maxThreatDistance), GameScenePartitioner.Instance.attackableEntitiesLayer, (List<ScenePartitionerEntry>) gathered_entries);
      for (int index = 0; index < ((List<ScenePartitionerEntry>) gathered_entries).Count; ++index)
      {
        FactionAlignment cmp = ((List<ScenePartitionerEntry>) gathered_entries)[index].obj as FactionAlignment;
        if (!Object.op_Equality((Object) cmp.transform, (Object) null) && !Object.op_Equality((Object) cmp, (Object) this.alignment) && cmp.IsAlignmentActive() && this.navigator.CanReach((IApproachable) cmp.attackable))
        {
          bool flag = false;
          foreach (Tag allyTag in this.def.allyTags)
          {
            if (((Component) cmp).HasTag(allyTag))
              flag = true;
          }
          if (!flag)
            this.threats.Add(cmp);
        }
      }
      gathered_entries.Recycle();
      return this.PickBestTarget(this.threats);
    }

    public GameObject PickBestTarget(List<FactionAlignment> threats)
    {
      float num1 = 1f;
      Vector2 vector2 = Vector2.op_Implicit(TransformExtensions.GetPosition(this.gameObject.transform));
      GameObject gameObject = (GameObject) null;
      float num2 = float.PositiveInfinity;
      for (int index = threats.Count - 1; index >= 0; --index)
      {
        FactionAlignment threat = threats[index];
        float num3 = Vector2.Distance(vector2, Vector2.op_Implicit(TransformExtensions.GetPosition(threat.transform))) / num1;
        if ((double) num3 < (double) num2)
        {
          num2 = num3;
          gameObject = ((Component) threat).gameObject;
        }
      }
      return gameObject;
    }

    private struct Egg
    {
      public GameObject game_object;
      public int cell;
    }

    private struct FindEggsTask : IWorkItem<List<KPrefabID>>
    {
      private static readonly List<Tag> EGG_TAG;
      private ListPool<int, EggProtectionMonitor>.PooledList eggs;
      private int start;
      private int end;

      public FindEggsTask(int start, int end)
      {
        this.start = start;
        this.end = end;
        this.eggs = ListPool<int, EggProtectionMonitor>.Allocate();
      }

      public void Run(List<KPrefabID> prefab_ids)
      {
        for (int start = this.start; start != this.end; ++start)
        {
          if (EggProtectionMonitor.Instance.FindEggsTask.EGG_TAG.Contains(prefab_ids[start].PrefabTag))
            ((List<int>) this.eggs).Add(start);
        }
      }

      public void Finish(List<KPrefabID> prefab_ids, List<EggProtectionMonitor.Instance.Egg> eggs)
      {
        foreach (int egg in (List<int>) this.eggs)
        {
          GameObject gameObject = ((Component) prefab_ids[egg]).gameObject;
          eggs.Add(new EggProtectionMonitor.Instance.Egg()
          {
            game_object = gameObject,
            cell = Grid.PosToCell(gameObject)
          });
        }
        this.eggs.Recycle();
      }

      static FindEggsTask()
      {
        List<Tag> tagList = new List<Tag>();
        tagList.Add(TagExtensions.ToTag("CrabEgg"));
        tagList.Add(TagExtensions.ToTag("CrabWoodEgg"));
        tagList.Add(TagExtensions.ToTag("CrabFreshWaterEgg"));
        EggProtectionMonitor.Instance.FindEggsTask.EGG_TAG = tagList;
      }
    }
  }
}
