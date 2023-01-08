// Decompiled with JetBrains decompiler
// Type: SeedPlantingStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SeedPlantingStates : 
  GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>
{
  private const int MAX_NAVIGATE_DISTANCE = 100;
  public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State findSeed;
  public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State moveToSeed;
  public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State pickupSeed;
  public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State findPlantLocation;
  public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State moveToPlantLocation;
  public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State moveToPlot;
  public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State moveToDirt;
  public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State planting;
  public GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.findSeed;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.PLANTINGSEED.NAME, (string) CREATURES.STATUSITEMS.PLANTINGSEED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).Exit(new StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback(SeedPlantingStates.UnreserveSeed)).Exit(new StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback(SeedPlantingStates.DropAll)).Exit(new StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback(SeedPlantingStates.RemoveMouthOverride));
    this.findSeed.Enter((StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback) (smi =>
    {
      SeedPlantingStates.FindSeed(smi);
      if (Object.op_Equality((Object) smi.targetSeed, (Object) null))
      {
        smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
      }
      else
      {
        SeedPlantingStates.ReserveSeed(smi);
        smi.GoTo((StateMachine.BaseState) this.moveToSeed);
      }
    }));
    this.moveToSeed.MoveTo(new Func<SeedPlantingStates.Instance, int>(SeedPlantingStates.GetSeedCell), this.findPlantLocation, this.behaviourcomplete);
    this.findPlantLocation.Enter((StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback) (smi =>
    {
      if (Object.op_Implicit((Object) smi.targetSeed))
      {
        SeedPlantingStates.FindDirtPlot(smi);
        if (Object.op_Inequality((Object) smi.targetPlot, (Object) null) || smi.targetDirtPlotCell != Grid.InvalidCell)
          smi.GoTo((StateMachine.BaseState) this.pickupSeed);
        else
          smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
      }
      else
        smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
    }));
    this.pickupSeed.PlayAnim("gather").Enter(new StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback(SeedPlantingStates.PickupComplete)).OnAnimQueueComplete(this.moveToPlantLocation);
    this.moveToPlantLocation.Enter((StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback) (smi =>
    {
      if (Object.op_Equality((Object) smi.targetSeed, (Object) null))
        smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
      else if (Object.op_Inequality((Object) smi.targetPlot, (Object) null))
        smi.GoTo((StateMachine.BaseState) this.moveToPlot);
      else if (smi.targetDirtPlotCell != Grid.InvalidCell)
        smi.GoTo((StateMachine.BaseState) this.moveToDirt);
      else
        smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
    }));
    this.moveToDirt.MoveTo((Func<SeedPlantingStates.Instance, int>) (smi => smi.targetDirtPlotCell), this.planting, this.behaviourcomplete);
    this.moveToPlot.Enter((StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback) (smi =>
    {
      if (!Object.op_Equality((Object) smi.targetPlot, (Object) null) && !Object.op_Equality((Object) smi.targetSeed, (Object) null))
        return;
      smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
    })).MoveTo(new Func<SeedPlantingStates.Instance, int>(SeedPlantingStates.GetPlantableCell), this.planting, this.behaviourcomplete);
    this.planting.Enter(new StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback(SeedPlantingStates.RemoveMouthOverride)).PlayAnim("plant").Exit(new StateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.State.Callback(SeedPlantingStates.PlantComplete)).OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToPlantSeed);
  }

  private static void AddMouthOverride(SeedPlantingStates.Instance smi)
  {
    SymbolOverrideController component = smi.GetComponent<SymbolOverrideController>();
    KAnim.Build.Symbol symbol = smi.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol(KAnimHashedString.op_Implicit(smi.def.prefix + "sq_mouth_cheeks"));
    if (symbol == null)
      return;
    component.AddSymbolOverride(HashedString.op_Implicit("sq_mouth"), symbol, 1);
  }

  private static void RemoveMouthOverride(SeedPlantingStates.Instance smi) => smi.GetComponent<SymbolOverrideController>().TryRemoveSymbolOverride(HashedString.op_Implicit("sq_mouth"), 1);

  private static void PickupComplete(SeedPlantingStates.Instance smi)
  {
    if (!Object.op_Implicit((Object) smi.targetSeed))
    {
      Debug.LogWarningFormat("PickupComplete seed {0} is null", new object[1]
      {
        (object) smi.targetSeed
      });
    }
    else
    {
      SeedPlantingStates.UnreserveSeed(smi);
      int cell = Grid.PosToCell((KMonoBehaviour) smi.targetSeed);
      if (smi.seed_cell != cell)
      {
        Debug.LogWarningFormat("PickupComplete seed {0} moved {1} != {2}", new object[3]
        {
          (object) smi.targetSeed,
          (object) cell,
          (object) smi.seed_cell
        });
        smi.targetSeed = (Pickupable) null;
      }
      else if (((Component) smi.targetSeed).HasTag(GameTags.Stored))
      {
        Debug.LogWarningFormat("PickupComplete seed {0} was stored by {1}", new object[2]
        {
          (object) smi.targetSeed,
          (object) smi.targetSeed.storage
        });
        smi.targetSeed = (Pickupable) null;
      }
      else
      {
        smi.targetSeed = EntitySplitter.Split(smi.targetSeed, 1f);
        smi.GetComponent<Storage>().Store(((Component) smi.targetSeed).gameObject);
        SeedPlantingStates.AddMouthOverride(smi);
      }
    }
  }

  private static void PlantComplete(SeedPlantingStates.Instance smi)
  {
    PlantableSeed seed = Object.op_Implicit((Object) smi.targetSeed) ? ((Component) smi.targetSeed).GetComponent<PlantableSeed>() : (PlantableSeed) null;
    PlantablePlot plot;
    if (Object.op_Implicit((Object) seed) && SeedPlantingStates.CheckValidPlotCell(smi, seed, smi.targetDirtPlotCell, out plot))
    {
      if (Object.op_Implicit((Object) plot))
      {
        if (Object.op_Equality((Object) plot.Occupant, (Object) null))
          plot.ForceDeposit(((Component) smi.targetSeed).gameObject);
      }
      else
        seed.TryPlant(true);
    }
    smi.targetSeed = (Pickupable) null;
    smi.seed_cell = Grid.InvalidCell;
    smi.targetPlot = (PlantablePlot) null;
  }

  private static void DropAll(SeedPlantingStates.Instance smi) => smi.GetComponent<Storage>().DropAll(false, false, new Vector3(), true, (List<GameObject>) null);

  private static int GetPlantableCell(SeedPlantingStates.Instance smi)
  {
    int cell = Grid.PosToCell((KMonoBehaviour) smi.targetPlot);
    return Grid.IsValidCell(cell) ? Grid.CellAbove(cell) : cell;
  }

  private static void FindDirtPlot(SeedPlantingStates.Instance smi)
  {
    smi.targetDirtPlotCell = Grid.InvalidCell;
    PlantableSeed component = ((Component) smi.targetSeed).GetComponent<PlantableSeed>();
    PlantableCellQuery query = PathFinderQueries.plantableCellQuery.Reset(component, 20);
    smi.GetComponent<Navigator>().RunQuery((PathFinderQuery) query);
    if (query.result_cells.Count <= 0)
      return;
    smi.targetDirtPlotCell = query.result_cells[Random.Range(0, query.result_cells.Count)];
  }

  private static bool CheckValidPlotCell(
    SeedPlantingStates.Instance smi,
    PlantableSeed seed,
    int cell,
    out PlantablePlot plot)
  {
    plot = (PlantablePlot) null;
    if (!Grid.IsValidCell(cell))
      return false;
    int num = seed.Direction != SingleEntityReceptacle.ReceptacleDirection.Bottom ? Grid.CellBelow(cell) : Grid.CellAbove(cell);
    if (!Grid.IsValidCell(num) || !Grid.Solid[num])
      return false;
    GameObject gameObject = Grid.Objects[num, 1];
    if (!Object.op_Implicit((Object) gameObject))
      return seed.TestSuitableGround(cell);
    plot = gameObject.GetComponent<PlantablePlot>();
    return Object.op_Inequality((Object) plot, (Object) null);
  }

  private static int GetSeedCell(SeedPlantingStates.Instance smi)
  {
    Debug.Assert(Object.op_Implicit((Object) smi.targetSeed));
    Debug.Assert(smi.seed_cell != Grid.InvalidCell);
    return smi.seed_cell;
  }

  private static void FindSeed(SeedPlantingStates.Instance smi)
  {
    Navigator component = smi.GetComponent<Navigator>();
    Pickupable pickupable = (Pickupable) null;
    int num = 100;
    foreach (PlantableSeed plantableSeed in Components.PlantableSeeds)
    {
      if ((((Component) plantableSeed).HasTag(GameTags.Seed) || ((Component) plantableSeed).HasTag(GameTags.CropSeed)) && !((Component) plantableSeed).HasTag(GameTags.Creatures.ReservedByCreature) && (double) Vector2.Distance(Vector2.op_Implicit(smi.transform.position), Vector2.op_Implicit(plantableSeed.transform.position)) <= 25.0)
      {
        int navigationCost = component.GetNavigationCost(Grid.PosToCell((KMonoBehaviour) plantableSeed));
        if (navigationCost != -1 && navigationCost < num)
        {
          pickupable = ((Component) plantableSeed).GetComponent<Pickupable>();
          num = navigationCost;
        }
      }
    }
    smi.targetSeed = pickupable;
    smi.seed_cell = Object.op_Implicit((Object) smi.targetSeed) ? Grid.PosToCell((KMonoBehaviour) smi.targetSeed) : Grid.InvalidCell;
  }

  private static void ReserveSeed(SeedPlantingStates.Instance smi)
  {
    GameObject go = Object.op_Implicit((Object) smi.targetSeed) ? ((Component) smi.targetSeed).gameObject : (GameObject) null;
    if (!Object.op_Inequality((Object) go, (Object) null))
      return;
    DebugUtil.Assert(!go.HasTag(GameTags.Creatures.ReservedByCreature));
    go.AddTag(GameTags.Creatures.ReservedByCreature);
  }

  private static void UnreserveSeed(SeedPlantingStates.Instance smi)
  {
    GameObject go = Object.op_Implicit((Object) smi.targetSeed) ? ((Component) smi.targetSeed).gameObject : (GameObject) null;
    if (!Object.op_Inequality((Object) smi.targetSeed, (Object) null))
      return;
    go.RemoveTag(GameTags.Creatures.ReservedByCreature);
  }

  public class Def : StateMachine.BaseDef
  {
    public string prefix;

    public Def(string prefix) => this.prefix = prefix;
  }

  public new class Instance : 
    GameStateMachine<SeedPlantingStates, SeedPlantingStates.Instance, IStateMachineTarget, SeedPlantingStates.Def>.GameInstance
  {
    public PlantablePlot targetPlot;
    public int targetDirtPlotCell = Grid.InvalidCell;
    public Element plantElement = ElementLoader.FindElementByHash(SimHashes.Dirt);
    public Pickupable targetSeed;
    public int seed_cell = Grid.InvalidCell;

    public Instance(Chore<SeedPlantingStates.Instance> chore, SeedPlantingStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.WantsToPlantSeed);
    }
  }
}
