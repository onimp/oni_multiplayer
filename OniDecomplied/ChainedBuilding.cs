// Decompiled with JetBrains decompiler
// Type: ChainedBuilding
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ChainedBuilding : 
  GameStateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>
{
  private GameStateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>.State unlinked;
  private GameStateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>.State linked;
  private GameStateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>.State DEBUG_relink;
  private StateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>.BoolParameter isConnectedToHead = new StateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>.BoolParameter();
  private StateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>.Signal doRelink;

  public override void InitializeStates(out StateMachine.BaseState defaultState)
  {
    defaultState = (StateMachine.BaseState) this.unlinked;
    StatusItem status_item = new StatusItem("NotLinkedToHeadStatusItem", (string) BUILDING.STATUSITEMS.NOTLINKEDTOHEAD.NAME, (string) BUILDING.STATUSITEMS.NOTLINKEDTOHEAD.TOOLTIP, "status_item_not_linked", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID);
    status_item.resolveTooltipCallback = (Func<string, object, string>) ((tooltip, obj) =>
    {
      ChainedBuilding.StatesInstance statesInstance = (ChainedBuilding.StatesInstance) obj;
      return tooltip.Replace("{headBuilding}", StringEntry.op_Implicit(Strings.Get("STRINGS.BUILDINGS.PREFABS." + ((Tag) ref statesInstance.def.headBuildingTag).Name.ToUpper() + ".NAME"))).Replace("{linkBuilding}", StringEntry.op_Implicit(Strings.Get("STRINGS.BUILDINGS.PREFABS." + ((Tag) ref statesInstance.def.linkBuildingTag).Name.ToUpper() + ".NAME")));
    });
    this.root.OnSignal(this.doRelink, this.DEBUG_relink);
    this.unlinked.ParamTransition<bool>((StateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>.Parameter<bool>) this.isConnectedToHead, this.linked, GameStateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>.IsTrue).ToggleStatusItem(status_item, (Func<ChainedBuilding.StatesInstance, object>) (smi => (object) smi));
    this.linked.ParamTransition<bool>((StateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>.Parameter<bool>) this.isConnectedToHead, this.unlinked, GameStateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>.IsFalse);
    this.DEBUG_relink.Enter((StateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>.State.Callback) (smi => smi.DEBUG_Relink()));
  }

  public class Def : StateMachine.BaseDef
  {
    public Tag headBuildingTag;
    public Tag linkBuildingTag;
    public ObjectLayer objectLayer;
  }

  public class StatesInstance : 
    GameStateMachine<ChainedBuilding, ChainedBuilding.StatesInstance, IStateMachineTarget, ChainedBuilding.Def>.GameInstance
  {
    private int widthInCells;
    private List<int> neighbourCheckCells;

    public StatesInstance(IStateMachineTarget master, ChainedBuilding.Def def)
      : base(master, def)
    {
      this.widthInCells = master.GetComponent<Building>().Def.WidthInCells;
      int cell = Grid.PosToCell((StateMachine.Instance) this);
      this.neighbourCheckCells = new List<int>()
      {
        Grid.OffsetCell(cell, -(this.widthInCells - 1) / 2 - 1, 0),
        Grid.OffsetCell(cell, this.widthInCells / 2 + 1, 0)
      };
    }

    public override void StartSM()
    {
      base.StartSM();
      bool foundHead = false;
      HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
      this.CollectToChain(ref chain, ref foundHead);
      this.PropogateFoundHead(foundHead, (HashSet<ChainedBuilding.StatesInstance>) chain);
      this.PropagateChangedEvent(this, (HashSet<ChainedBuilding.StatesInstance>) chain);
      chain.Recycle();
    }

    public void DEBUG_Relink()
    {
      bool foundHead = false;
      HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
      this.CollectToChain(ref chain, ref foundHead);
      this.PropogateFoundHead(foundHead, (HashSet<ChainedBuilding.StatesInstance>) chain);
      chain.Recycle();
    }

    protected override void OnCleanUp()
    {
      HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
      foreach (int neighbourCheckCell in this.neighbourCheckCells)
      {
        bool foundHead = false;
        this.CollectNeighbourToChain(neighbourCheckCell, ref chain, ref foundHead, this);
        this.PropogateFoundHead(foundHead, (HashSet<ChainedBuilding.StatesInstance>) chain);
        this.PropagateChangedEvent(this, (HashSet<ChainedBuilding.StatesInstance>) chain);
        ((HashSet<ChainedBuilding.StatesInstance>) chain).Clear();
      }
      chain.Recycle();
      base.OnCleanUp();
    }

    public HashSet<ChainedBuilding.StatesInstance> GetLinkedBuildings(
      ref HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain)
    {
      bool foundHead = false;
      this.CollectToChain(ref chain, ref foundHead);
      return (HashSet<ChainedBuilding.StatesInstance>) chain;
    }

    private void PropogateFoundHead(bool foundHead, HashSet<ChainedBuilding.StatesInstance> chain)
    {
      foreach (ChainedBuilding.StatesInstance smi in chain)
        smi.sm.isConnectedToHead.Set(foundHead, smi);
    }

    private void PropagateChangedEvent(
      ChainedBuilding.StatesInstance changedLink,
      HashSet<ChainedBuilding.StatesInstance> chain)
    {
      foreach (StateMachine.Instance instance in chain)
        instance.Trigger(-1009905786, (object) changedLink);
    }

    private void CollectToChain(
      ref HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain,
      ref bool foundHead,
      ChainedBuilding.StatesInstance ignoredLink = null)
    {
      if (ignoredLink != null && ignoredLink == this || ((HashSet<ChainedBuilding.StatesInstance>) chain).Contains(this))
        return;
      ((HashSet<ChainedBuilding.StatesInstance>) chain).Add(this);
      if (this.HasTag(this.def.headBuildingTag))
        foundHead = true;
      foreach (int neighbourCheckCell in this.neighbourCheckCells)
        this.CollectNeighbourToChain(neighbourCheckCell, ref chain, ref foundHead);
    }

    private void CollectNeighbourToChain(
      int cell,
      ref HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain,
      ref bool foundHead,
      ChainedBuilding.StatesInstance ignoredLink = null)
    {
      GameObject go = Grid.Objects[cell, (int) this.def.objectLayer];
      if (Object.op_Equality((Object) go, (Object) null))
        return;
      KPrefabID component = go.GetComponent<KPrefabID>();
      if (!component.HasTag(this.def.linkBuildingTag) && !component.IsPrefabID(this.def.headBuildingTag))
        return;
      go.GetSMI<ChainedBuilding.StatesInstance>()?.CollectToChain(ref chain, ref foundHead, ignoredLink);
    }
  }
}
