// Decompiled with JetBrains decompiler
// Type: ClimbableTreeMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ClimbableTreeMonitor : 
  GameStateMachine<ClimbableTreeMonitor, ClimbableTreeMonitor.Instance, IStateMachineTarget, ClimbableTreeMonitor.Def>
{
  private const int MAX_NAV_COST = 2147483647;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.ToggleBehaviour(GameTags.Creatures.WantsToClimbTree, (StateMachine<ClimbableTreeMonitor, ClimbableTreeMonitor.Instance, IStateMachineTarget, ClimbableTreeMonitor.Def>.Transition.ConditionCallback) (smi => smi.UpdateHasClimbable()), (System.Action<ClimbableTreeMonitor.Instance>) (smi => smi.OnClimbComplete()));
  }

  public class Def : StateMachine.BaseDef
  {
    public float searchMinInterval = 60f;
    public float searchMaxInterval = 120f;
  }

  public new class Instance : 
    GameStateMachine<ClimbableTreeMonitor, ClimbableTreeMonitor.Instance, IStateMachineTarget, ClimbableTreeMonitor.Def>.GameInstance
  {
    public GameObject climbTarget;
    public float nextSearchTime;

    public Instance(IStateMachineTarget master, ClimbableTreeMonitor.Def def)
      : base(master, def)
    {
      this.RefreshSearchTime();
    }

    private void RefreshSearchTime() => this.nextSearchTime = Time.time + Mathf.Lerp(this.def.searchMinInterval, this.def.searchMaxInterval, Random.value);

    public bool UpdateHasClimbable()
    {
      if (Object.op_Equality((Object) this.climbTarget, (Object) null))
      {
        if ((double) Time.time < (double) this.nextSearchTime)
          return false;
        this.FindClimbableTree();
        this.RefreshSearchTime();
      }
      return Object.op_Inequality((Object) this.climbTarget, (Object) null);
    }

    private void FindClimbableTree()
    {
      this.climbTarget = (GameObject) null;
      ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
      ListPool<KMonoBehaviour, ClimbableTreeMonitor>.PooledList pooledList = ListPool<KMonoBehaviour, ClimbableTreeMonitor>.Allocate();
      Extents extents = new Extents(Grid.PosToCell(TransformExtensions.GetPosition(this.master.transform)), 10);
      GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.plants, (List<ScenePartitionerEntry>) gathered_entries);
      GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.completeBuildings, (List<ScenePartitionerEntry>) gathered_entries);
      Navigator component1 = this.GetComponent<Navigator>();
      foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
      {
        KMonoBehaviour cmp = partitionerEntry.obj as KMonoBehaviour;
        if (!((Component) cmp).HasTag(GameTags.Creatures.ReservedByCreature))
        {
          int cell = Grid.PosToCell(cmp);
          if (component1.CanReach(cell))
          {
            BuddingTrunk component2 = ((Component) cmp).GetComponent<BuddingTrunk>();
            StorageLocker component3 = ((Component) cmp).GetComponent<StorageLocker>();
            if (Object.op_Inequality((Object) component2, (Object) null))
            {
              if (!component2.ExtraSeedAvailable)
                continue;
            }
            else if (Object.op_Inequality((Object) component3, (Object) null))
            {
              Storage component4 = ((Component) component3).GetComponent<Storage>();
              if (!component4.allowItemRemoval || component4.IsEmpty())
                continue;
            }
            else
              continue;
            ((List<KMonoBehaviour>) pooledList).Add(cmp);
          }
        }
      }
      if (((List<KMonoBehaviour>) pooledList).Count > 0)
      {
        int index = Random.Range(0, ((List<KMonoBehaviour>) pooledList).Count);
        this.climbTarget = ((Component) ((List<KMonoBehaviour>) pooledList)[index]).gameObject;
      }
      gathered_entries.Recycle();
      pooledList.Recycle();
    }

    public void OnClimbComplete() => this.climbTarget = (GameObject) null;
  }
}
