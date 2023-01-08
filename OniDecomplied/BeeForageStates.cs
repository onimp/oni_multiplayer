// Decompiled with JetBrains decompiler
// Type: BeeForageStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BeeForageStates : 
  GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>
{
  private const int MAX_NAVIGATE_DISTANCE = 100;
  private const string oreSymbol = "snapto_thing";
  private const string oreLegSymbol = "legBeeOre";
  private const string noOreLegSymbol = "legBeeNoOre";
  public BeeForageStates.CollectionBehaviourStates collect;
  public BeeForageStates.StorageBehaviourStates storage;
  public BeeForageStates.ExitStates behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.collect.findTarget;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.FORAGINGMATERIAL.NAME, (string) CREATURES.STATUSITEMS.FORAGINGMATERIAL.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).Exit(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(BeeForageStates.UnreserveTarget)).Exit(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(BeeForageStates.DropAll));
    this.collect.findTarget.Enter((StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback) (smi =>
    {
      BeeForageStates.FindTarget(smi);
      smi.targetHive = smi.master.GetComponent<Bee>().FindHiveInRoom();
      if (Object.op_Inequality((Object) smi.targetHive, (Object) null))
      {
        if (Object.op_Inequality((Object) smi.forageTarget, (Object) null))
        {
          BeeForageStates.ReserveTarget(smi);
          smi.GoTo((StateMachine.BaseState) this.collect.forage.moveToTarget);
          return;
        }
        if (Grid.IsValidCell(smi.targetMiningCell))
        {
          smi.GoTo((StateMachine.BaseState) this.collect.mine.moveToTarget);
          return;
        }
      }
      smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
    }));
    this.collect.forage.moveToTarget.MoveTo(new Func<BeeForageStates.Instance, int>(BeeForageStates.GetOreCell), this.collect.forage.pickupTarget, (GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State) this.behaviourcomplete);
    this.collect.forage.pickupTarget.PlayAnim("pickup_pre").Enter(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(BeeForageStates.PickupComplete)).OnAnimQueueComplete(this.storage.moveToHive);
    this.collect.mine.moveToTarget.MoveTo((Func<BeeForageStates.Instance, int>) (smi => smi.targetMiningCell), this.collect.mine.mineTarget, (GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State) this.behaviourcomplete);
    this.collect.mine.mineTarget.PlayAnim("mining_pre").QueueAnim("mining_loop").QueueAnim("mining_pst").Enter(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(BeeForageStates.MineTarget)).OnAnimQueueComplete(this.storage.moveToHive);
    this.storage.Enter(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(this.HoldOre)).Exit(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(this.DropOre));
    this.storage.moveToHive.Enter((StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback) (smi =>
    {
      if (!Object.op_Implicit((Object) smi.targetHive))
        smi.targetHive = smi.master.GetComponent<Bee>().FindHiveInRoom();
      if (Object.op_Implicit((Object) smi.targetHive))
        return;
      smi.GoTo((StateMachine.BaseState) this.storage.dropMaterial);
    })).MoveTo((Func<BeeForageStates.Instance, int>) (smi => Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(((KMonoBehaviour) smi.targetHive).transform)), smi.hiveCellOffset)), this.storage.storeMaterial, (GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State) this.behaviourcomplete);
    this.storage.storeMaterial.PlayAnim("deposit").Exit(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(BeeForageStates.StoreOre)).OnAnimQueueComplete(this.behaviourcomplete.pre);
    this.storage.dropMaterial.Enter((StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback) (smi => smi.GoTo((StateMachine.BaseState) this.behaviourcomplete))).Exit(new StateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State.Callback(BeeForageStates.DropAll));
    this.behaviourcomplete.DefaultState(this.behaviourcomplete.pst);
    this.behaviourcomplete.pre.PlayAnim("spawn").OnAnimQueueComplete(this.behaviourcomplete.pst);
    this.behaviourcomplete.pst.BehaviourComplete(GameTags.Creatures.WantsToForage);
  }

  private static void FindTarget(BeeForageStates.Instance smi)
  {
    if (BeeForageStates.FindOre(smi))
      return;
    BeeForageStates.FindMineableCell(smi);
  }

  private void HoldOre(BeeForageStates.Instance smi)
  {
    GameObject first = smi.GetComponent<Storage>().FindFirst(smi.def.oreTag);
    if (!Object.op_Implicit((Object) first))
      return;
    KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
    KAnim.Build.Symbol symbol = first.GetComponent<KBatchedAnimController>().CurrentAnim.animFile.build.symbols[0];
    ((Component) component).GetComponent<SymbolOverrideController>().AddSymbolOverride(HashedString.op_Implicit(smi.oreSymbolHash), symbol, 5);
    component.SetSymbolVisiblity(smi.oreSymbolHash, true);
    component.SetSymbolVisiblity(smi.oreLegSymbolHash, true);
    component.SetSymbolVisiblity(smi.noOreLegSymbolHash, false);
  }

  private void DropOre(BeeForageStates.Instance smi)
  {
    KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
    component.SetSymbolVisiblity(smi.oreSymbolHash, false);
    component.SetSymbolVisiblity(smi.oreLegSymbolHash, false);
    component.SetSymbolVisiblity(smi.noOreLegSymbolHash, true);
  }

  private static void PickupComplete(BeeForageStates.Instance smi)
  {
    if (!Object.op_Implicit((Object) smi.forageTarget))
    {
      Debug.LogWarningFormat("PickupComplete forageTarget {0} is null", new object[1]
      {
        (object) smi.forageTarget
      });
    }
    else
    {
      BeeForageStates.UnreserveTarget(smi);
      int cell = Grid.PosToCell((KMonoBehaviour) smi.forageTarget);
      if (smi.forageTarget_cell != cell)
      {
        Debug.LogWarningFormat("PickupComplete forageTarget {0} moved {1} != {2}", new object[3]
        {
          (object) smi.forageTarget,
          (object) cell,
          (object) smi.forageTarget_cell
        });
        smi.forageTarget = (Pickupable) null;
      }
      else if (((Component) smi.forageTarget).HasTag(GameTags.Stored))
      {
        Debug.LogWarningFormat("PickupComplete forageTarget {0} was stored by {1}", new object[2]
        {
          (object) smi.forageTarget,
          (object) smi.forageTarget.storage
        });
        smi.forageTarget = (Pickupable) null;
      }
      else
      {
        smi.forageTarget = EntitySplitter.Split(smi.forageTarget, 10f);
        smi.GetComponent<Storage>().Store(((Component) smi.forageTarget).gameObject);
      }
    }
  }

  private static void MineTarget(BeeForageStates.Instance smi)
  {
    Storage storage = smi.master.GetComponent<Storage>();
    HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add((System.Action<Sim.MassConsumedCallback, object>) ((mass_cb_info, data) =>
    {
      if ((double) mass_cb_info.mass <= 0.0)
        return;
      storage.AddOre(ElementLoader.elements[(int) mass_cb_info.elemIdx].id, mass_cb_info.mass, mass_cb_info.temperature, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount);
    }), (object) null, "BeetaMine");
    SimMessages.ConsumeMass(smi.cellToMine, Grid.Element[smi.cellToMine].id, smi.def.amountToMine, (byte) 1, handle.index);
  }

  private static void StoreOre(BeeForageStates.Instance smi)
  {
    smi.master.GetComponent<Storage>().Transfer(((Component) smi.targetHive).GetComponent<Storage>());
    smi.forageTarget = (Pickupable) null;
    smi.forageTarget_cell = Grid.InvalidCell;
    smi.targetHive = (KPrefabID) null;
  }

  private static void DropAll(BeeForageStates.Instance smi) => smi.GetComponent<Storage>().DropAll(false, false, new Vector3(), true, (List<GameObject>) null);

  private static bool FindMineableCell(BeeForageStates.Instance smi)
  {
    smi.targetMiningCell = Grid.InvalidCell;
    MineableCellQuery query = PathFinderQueries.mineableCellQuery.Reset(smi.def.oreTag, 20);
    smi.GetComponent<Navigator>().RunQuery((PathFinderQuery) query);
    if (query.result_cells.Count > 0)
    {
      smi.targetMiningCell = query.result_cells[Random.Range(0, query.result_cells.Count)];
      foreach (Direction d in MineableCellQuery.DIRECTION_CHECKS)
      {
        int cellInDirection = Grid.GetCellInDirection(smi.targetMiningCell, d);
        if (Grid.IsValidCell(cellInDirection) && Grid.IsSolidCell(cellInDirection) && Tag.op_Equality(Grid.Element[cellInDirection].tag, smi.def.oreTag))
        {
          smi.cellToMine = cellInDirection;
          return true;
        }
      }
    }
    return false;
  }

  private static bool FindOre(BeeForageStates.Instance smi)
  {
    Navigator component = smi.GetComponent<Navigator>();
    Vector3 position = TransformExtensions.GetPosition(smi.transform);
    Pickupable pickupable = (Pickupable) null;
    int num = 100;
    Extents extents = new Extents((int) position.x, (int) position.y, 15);
    ListPool<ScenePartitionerEntry, BeeForageStates>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, BeeForageStates>.Allocate();
    GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>) gathered_entries);
    Element element = ElementLoader.GetElement(smi.def.oreTag);
    foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
    {
      Pickupable cmp = partitionerEntry.obj as Pickupable;
      if (Object.op_Implicit((Object) cmp) && Object.op_Implicit((Object) ((Component) cmp).GetComponent<ElementChunk>()) && Object.op_Implicit((Object) ((Component) cmp).GetComponent<PrimaryElement>()) && ((Component) cmp).GetComponent<PrimaryElement>().Element == element && !((Component) cmp).HasTag(GameTags.Creatures.ReservedByCreature))
      {
        int navigationCost = component.GetNavigationCost(Grid.PosToCell((KMonoBehaviour) cmp));
        if (navigationCost != -1 && navigationCost < num)
        {
          pickupable = ((Component) cmp).GetComponent<Pickupable>();
          num = navigationCost;
        }
      }
    }
    smi.forageTarget = pickupable;
    smi.forageTarget_cell = Object.op_Implicit((Object) smi.forageTarget) ? Grid.PosToCell((KMonoBehaviour) smi.forageTarget) : Grid.InvalidCell;
    return Object.op_Inequality((Object) smi.forageTarget, (Object) null);
  }

  private static void ReserveTarget(BeeForageStates.Instance smi)
  {
    GameObject go = Object.op_Implicit((Object) smi.forageTarget) ? ((Component) smi.forageTarget).gameObject : (GameObject) null;
    if (!Object.op_Inequality((Object) go, (Object) null))
      return;
    DebugUtil.Assert(!go.HasTag(GameTags.Creatures.ReservedByCreature));
    go.AddTag(GameTags.Creatures.ReservedByCreature);
  }

  private static void UnreserveTarget(BeeForageStates.Instance smi)
  {
    GameObject go = Object.op_Implicit((Object) smi.forageTarget) ? ((Component) smi.forageTarget).gameObject : (GameObject) null;
    if (!Object.op_Inequality((Object) smi.forageTarget, (Object) null))
      return;
    go.RemoveTag(GameTags.Creatures.ReservedByCreature);
  }

  private static int GetOreCell(BeeForageStates.Instance smi)
  {
    Debug.Assert(Object.op_Implicit((Object) smi.forageTarget));
    Debug.Assert(smi.forageTarget_cell != Grid.InvalidCell);
    return smi.forageTarget_cell;
  }

  public class Def : StateMachine.BaseDef
  {
    public Tag oreTag;
    public float amountToMine;

    public Def(Tag tag, float amount_to_mine)
    {
      this.oreTag = tag;
      this.amountToMine = amount_to_mine;
    }
  }

  public new class Instance : 
    GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.GameInstance
  {
    public int targetMiningCell = Grid.InvalidCell;
    public int cellToMine = Grid.InvalidCell;
    public Pickupable forageTarget;
    public int forageTarget_cell = Grid.InvalidCell;
    public KPrefabID targetHive;
    public KAnimHashedString oreSymbolHash;
    public KAnimHashedString oreLegSymbolHash;
    public KAnimHashedString noOreLegSymbolHash;
    public CellOffset hiveCellOffset = new CellOffset(1, 1);

    public Instance(Chore<BeeForageStates.Instance> chore, BeeForageStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      this.oreSymbolHash = new KAnimHashedString("snapto_thing");
      this.oreLegSymbolHash = new KAnimHashedString("legBeeOre");
      this.noOreLegSymbolHash = new KAnimHashedString("legBeeNoOre");
      this.smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(this.smi.oreSymbolHash, false);
      this.smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(this.smi.oreLegSymbolHash, false);
      this.smi.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(this.smi.noOreLegSymbolHash, true);
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.WantsToForage);
    }
  }

  public class ForageBehaviourStates : 
    GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State
  {
    public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State moveToTarget;
    public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State pickupTarget;
  }

  public class MiningBehaviourStates : 
    GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State
  {
    public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State moveToTarget;
    public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State mineTarget;
  }

  public class CollectionBehaviourStates : 
    GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State
  {
    public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State findTarget;
    public BeeForageStates.ForageBehaviourStates forage;
    public BeeForageStates.MiningBehaviourStates mine;
  }

  public class StorageBehaviourStates : 
    GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State
  {
    public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State moveToHive;
    public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State storeMaterial;
    public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State dropMaterial;
  }

  public class ExitStates : 
    GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State
  {
    public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State pre;
    public GameStateMachine<BeeForageStates, BeeForageStates.Instance, IStateMachineTarget, BeeForageStates.Def>.State pst;
  }
}
