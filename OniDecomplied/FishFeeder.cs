// Decompiled with JetBrains decompiler
// Type: FishFeeder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using UnityEngine;

public class FishFeeder : 
  GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>
{
  public GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.State notoperational;
  public FishFeeder.OperationalState operational;
  public static HashedString[] ballSymbols;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.notoperational;
    this.root.Enter(new StateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.State.Callback(FishFeeder.SetupFishFeederTopAndBot)).Exit(new StateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.State.Callback(FishFeeder.CleanupFishFeederTopAndBot)).EventHandler(GameHashes.OnStorageChange, new GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.GameEvent.Callback(FishFeeder.OnStorageChange)).EventHandler(GameHashes.RefreshUserMenu, new GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.GameEvent.Callback(FishFeeder.OnRefreshUserMenu));
    this.notoperational.TagTransition(GameTags.Operational, (GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.State) this.operational);
    this.operational.DefaultState(this.operational.on).TagTransition(GameTags.Operational, this.notoperational, true);
    this.operational.on.DoNothing();
    int length = 19;
    FishFeeder.ballSymbols = new HashedString[length];
    for (int index = 0; index < length; ++index)
      FishFeeder.ballSymbols[index] = HashedString.op_Implicit("ball" + index.ToString());
  }

  private static void SetupFishFeederTopAndBot(FishFeeder.Instance smi)
  {
    Storage storage = smi.Get<Storage>();
    smi.fishFeederTop = new FishFeeder.FishFeederTop(smi, FishFeeder.ballSymbols, storage.Capacity());
    smi.fishFeederTop.RefreshStorage();
    smi.fishFeederBot = new FishFeeder.FishFeederBot(smi, 10f, FishFeeder.ballSymbols);
    smi.fishFeederBot.RefreshStorage();
    smi.fishFeederTop.ToggleMutantSeedFetches(smi.ForbidMutantSeeds);
    smi.UpdateMutantSeedStatusItem();
  }

  private static void CleanupFishFeederTopAndBot(FishFeeder.Instance smi)
  {
    smi.fishFeederTop.Cleanup();
    smi.fishFeederBot.Cleanup();
  }

  private static void MoveStoredContentsToConsumeOffset(FishFeeder.Instance smi)
  {
    foreach (GameObject data in smi.GetComponent<Storage>().items)
    {
      if (!Object.op_Equality((Object) data, (Object) null))
        FishFeeder.OnStorageChange(smi, (object) data);
    }
  }

  private static void OnStorageChange(FishFeeder.Instance smi, object data)
  {
    if (Object.op_Equality((Object) data, (Object) null))
      return;
    smi.fishFeederTop.RefreshStorage();
    smi.fishFeederBot.RefreshStorage();
  }

  private static void OnRefreshUserMenu(FishFeeder.Instance smi, object data)
  {
    if (!DlcManager.FeatureRadiationEnabled())
      return;
    Game.Instance.userMenu.AddButton(smi.gameObject, new KIconButtonMenu.ButtonInfo("action_switch_toggle", (string) (smi.ForbidMutantSeeds ? UI.USERMENUACTIONS.ACCEPT_MUTANT_SEEDS.ACCEPT : UI.USERMENUACTIONS.ACCEPT_MUTANT_SEEDS.REJECT), (System.Action) (() =>
    {
      smi.ForbidMutantSeeds = !smi.ForbidMutantSeeds;
      FishFeeder.OnRefreshUserMenu(smi, (object) null);
    }), tooltipText: ((string) UI.USERMENUACTIONS.ACCEPT_MUTANT_SEEDS.FISH_FEEDER_TOOLTIP)));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class OperationalState : 
    GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.State
  {
    public GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.State on;
  }

  public new class Instance : 
    GameStateMachine<FishFeeder, FishFeeder.Instance, IStateMachineTarget, FishFeeder.Def>.GameInstance
  {
    private StatusItem mutantSeedStatusItem;
    public FishFeeder.FishFeederTop fishFeederTop;
    public FishFeeder.FishFeederBot fishFeederBot;
    [Serialize]
    private bool forbidMutantSeeds;

    public bool ForbidMutantSeeds
    {
      get => this.forbidMutantSeeds;
      set
      {
        this.forbidMutantSeeds = value;
        this.fishFeederTop.ToggleMutantSeedFetches(this.forbidMutantSeeds);
        this.UpdateMutantSeedStatusItem();
      }
    }

    public Instance(IStateMachineTarget master, FishFeeder.Def def)
      : base(master, def)
    {
      this.mutantSeedStatusItem = new StatusItem("FISHFEEDERACCEPTSMUTANTSEEDS", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false);
    }

    public void UpdateMutantSeedStatusItem() => this.gameObject.GetComponent<KSelectable>().ToggleStatusItem(this.mutantSeedStatusItem, DlcManager.IsContentActive("EXPANSION1_ID") && !this.forbidMutantSeeds);
  }

  public class FishFeederTop : IRenderEveryTick
  {
    private FishFeeder.Instance smi;
    private float mass;
    private float targetMass;
    private HashedString[] ballSymbols;
    private float massPerBall;
    private float timeSinceLastBallAppeared;

    public FishFeederTop(FishFeeder.Instance smi, HashedString[] ball_symbols, float capacity)
    {
      this.smi = smi;
      this.ballSymbols = ball_symbols;
      this.massPerBall = capacity / (float) ball_symbols.Length;
      this.FillFeeder(this.mass);
      SimAndRenderScheduler.instance.Add((object) this, false);
    }

    private void FillFeeder(float mass)
    {
      KBatchedAnimController component1 = this.smi.GetComponent<KBatchedAnimController>();
      SymbolOverrideController component2 = this.smi.GetComponent<SymbolOverrideController>();
      KAnim.Build.Symbol source_symbol = (KAnim.Build.Symbol) null;
      Storage component3 = this.smi.GetComponent<Storage>();
      if (component3.items.Count > 0 && Object.op_Inequality((Object) component3.items[0], (Object) null))
        source_symbol = this.smi.GetComponent<Storage>().items[0].GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol(KAnimHashedString.op_Implicit("algae"));
      for (int index = 0; index < this.ballSymbols.Length; ++index)
      {
        bool is_visible = (double) mass > (double) (index + 1) * (double) this.massPerBall;
        component1.SetSymbolVisiblity(KAnimHashedString.op_Implicit(this.ballSymbols[index]), is_visible);
        if (source_symbol != null)
          component2.AddSymbolOverride(this.ballSymbols[index], source_symbol);
      }
    }

    public void RefreshStorage()
    {
      float num = 0.0f;
      foreach (GameObject gameObject in this.smi.GetComponent<Storage>().items)
      {
        if (!Object.op_Equality((Object) gameObject, (Object) null))
          num += gameObject.GetComponent<PrimaryElement>().Mass;
      }
      this.targetMass = num;
    }

    public void RenderEveryTick(float dt)
    {
      this.timeSinceLastBallAppeared += dt;
      if ((double) this.targetMass <= (double) this.mass || (double) this.timeSinceLastBallAppeared <= 0.02500000037252903)
        return;
      this.mass += Mathf.Min(this.massPerBall, this.targetMass - this.mass);
      this.FillFeeder(this.mass);
      this.timeSinceLastBallAppeared = 0.0f;
    }

    public void Cleanup() => SimAndRenderScheduler.instance.Remove((object) this);

    public void ToggleMutantSeedFetches(bool allow)
    {
      StorageLocker component = this.smi.GetComponent<StorageLocker>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.UpdateForbiddenTag(GameTags.MutatedSeed, !allow);
    }
  }

  public class FishFeederBot
  {
    private KBatchedAnimController anim;
    private Storage topStorage;
    private Storage botStorage;
    private bool refreshingStorage;
    private FishFeeder.Instance smi;
    private float massPerBall;
    private static readonly HashedString HASH_FEEDBALL = HashedString.op_Implicit("feedball");

    public FishFeederBot(FishFeeder.Instance smi, float mass_per_ball, HashedString[] ball_symbols)
    {
      this.smi = smi;
      this.massPerBall = mass_per_ball;
      this.anim = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(nameof (FishFeederBot))), TransformExtensions.GetPosition(smi.transform), Grid.SceneLayer.Front).GetComponent<KBatchedAnimController>();
      ((Component) this.anim).transform.SetParent(smi.transform);
      ((Component) this.anim).gameObject.SetActive(true);
      this.anim.SetSceneLayer(Grid.SceneLayer.Building);
      this.anim.Play(HashedString.op_Implicit("ball"));
      this.anim.Stop();
      foreach (HashedString ballSymbol in ball_symbols)
        this.anim.SetSymbolVisiblity(KAnimHashedString.op_Implicit(ballSymbol), false);
      Storage[] components = smi.gameObject.GetComponents<Storage>();
      this.topStorage = components[0];
      this.botStorage = components[1];
    }

    public void RefreshStorage()
    {
      if (this.refreshingStorage)
        return;
      this.refreshingStorage = true;
      foreach (GameObject gameObject in this.botStorage.items)
      {
        if (!Object.op_Equality((Object) gameObject, (Object) null))
        {
          int cell = Grid.CellBelow(Grid.CellBelow(Grid.PosToCell(TransformExtensions.GetPosition(this.smi.transform))));
          TransformExtensions.SetPosition(gameObject.transform, Grid.CellToPosCBC(cell, Grid.SceneLayer.BuildingBack));
        }
      }
      if (this.botStorage.IsEmpty())
      {
        float num = 0.0f;
        foreach (GameObject gameObject in this.topStorage.items)
        {
          if (!Object.op_Equality((Object) gameObject, (Object) null))
            num += gameObject.GetComponent<PrimaryElement>().Mass;
        }
        if ((double) num > 0.0)
        {
          this.anim.SetSymbolVisiblity(KAnimHashedString.op_Implicit(FishFeeder.FishFeederBot.HASH_FEEDBALL), true);
          this.anim.Play(HashedString.op_Implicit("ball"));
          Pickupable pickupable = this.topStorage.items[0].GetComponent<Pickupable>().Take(this.massPerBall);
          KAnim.Build.Symbol symbol = ((Component) pickupable).GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol(KAnimHashedString.op_Implicit("algae"));
          if (symbol != null)
            ((Component) this.anim).GetComponent<SymbolOverrideController>().AddSymbolOverride(FishFeeder.FishFeederBot.HASH_FEEDBALL, symbol);
          this.botStorage.Store(((Component) pickupable).gameObject);
          int cell = Grid.CellBelow(Grid.CellBelow(Grid.PosToCell(TransformExtensions.GetPosition(this.smi.transform))));
          TransformExtensions.SetPosition(pickupable.transform, Grid.CellToPosCBC(cell, Grid.SceneLayer.BuildingUse));
        }
        else
          this.anim.SetSymbolVisiblity(KAnimHashedString.op_Implicit(FishFeeder.FishFeederBot.HASH_FEEDBALL), false);
      }
      this.refreshingStorage = false;
    }

    public void Cleanup()
    {
    }
  }
}
