// Decompiled with JetBrains decompiler
// Type: SweepBotConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

public class SweepBotConfig : IEntityConfig
{
  public const string ID = "SweepBot";
  public const string BASE_TRAIT_ID = "SweepBotBaseTrait";
  public const float STORAGE_CAPACITY = 500f;
  public const float BATTERY_CAPACITY = 9000f;
  public const float BATTERY_DEPLETION_RATE = 17.1428566f;
  public const float MAX_SWEEP_AMOUNT = 10f;
  public const float MOP_SPEED = 10f;
  private string name = (string) STRINGS.ROBOTS.MODELS.SWEEPBOT.NAME;
  private string desc = (string) STRINGS.ROBOTS.MODELS.SWEEPBOT.DESC;
  public static float MASS = 25f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name = this.name;
    string desc = this.desc;
    double mass = (double) SweepBotConfig.MASS;
    EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("sweep_bot_kanim"));
    EffectorValues decor = none;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("SweepBot", name, desc, (float) mass, anim, "idle", Grid.SceneLayer.Creatures, 1, 1, decor, noise);
    placedEntity.AddOrGet<LoopingSounds>();
    placedEntity.GetComponent<KBatchedAnimController>().isMovable = true;
    KPrefabID kprefabId = placedEntity.AddOrGet<KPrefabID>();
    kprefabId.AddTag(GameTags.Creature, false);
    kprefabId.AddTag(GameTags.Robot, false);
    placedEntity.AddComponent<Pickupable>();
    placedEntity.AddOrGet<Clearable>().isClearable = false;
    Trait trait = Db.Get().CreateTrait("SweepBotBaseTrait", this.name, this.name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.maxAttribute.Id, 9000f, this.name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.deltaAttribute.Id, -17.1428566f, this.name));
    Modifiers modifiers = placedEntity.AddOrGet<Modifiers>();
    modifiers.initialTraits.Add("SweepBotBaseTrait");
    modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
    modifiers.initialAmounts.Add(Db.Get().Amounts.InternalBattery.Id);
    placedEntity.AddOrGet<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapto_pivot"), false);
    placedEntity.AddOrGet<Traits>();
    placedEntity.AddOrGet<Effects>();
    placedEntity.AddOrGetDef<AnimInterruptMonitor.Def>();
    placedEntity.AddOrGetDef<StorageUnloadMonitor.Def>();
    RobotBatteryMonitor.Def def = placedEntity.AddOrGetDef<RobotBatteryMonitor.Def>();
    def.batteryAmountId = Db.Get().Amounts.InternalBattery.Id;
    def.canCharge = true;
    def.lowBatteryWarningPercent = 0.5f;
    placedEntity.AddOrGetDef<SweepBotReactMonitor.Def>();
    placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();
    placedEntity.AddOrGetDef<SweepBotTrappedMonitor.Def>();
    placedEntity.AddOrGet<AnimEventHandler>();
    placedEntity.AddOrGet<SnapOn>().snapPoints = new List<SnapOn.SnapPoint>((IEnumerable<SnapOn.SnapPoint>) new SnapOn.SnapPoint[1]
    {
      new SnapOn.SnapPoint()
      {
        pointName = "carry",
        automatic = false,
        context = HashedString.op_Implicit(""),
        buildFile = (KAnimFile) null,
        overrideSymbol = HashedString.op_Implicit("snapTo_ornament")
      }
    });
    SymbolOverrideControllerUtil.AddToPrefab(placedEntity);
    placedEntity.AddComponent<Storage>();
    Storage storage = placedEntity.AddComponent<Storage>();
    storage.capacityKg = 500f;
    storage.storageFXOffset = new Vector3(0.0f, 0.5f, 0.0f);
    placedEntity.AddOrGet<OrnamentReceptacle>().AddDepositTag(GameTags.PedestalDisplayable);
    placedEntity.AddOrGet<DecorProvider>();
    placedEntity.AddOrGet<UserNameable>();
    placedEntity.AddOrGet<CharacterOverlay>();
    placedEntity.AddOrGet<ItemPedestal>();
    Navigator navigator = placedEntity.AddOrGet<Navigator>();
    navigator.NavGridName = "WalkerBabyNavGrid";
    navigator.CurrentNavType = NavType.Floor;
    navigator.defaultSpeed = 1f;
    navigator.updateProber = true;
    navigator.maxProbingRadius = 32;
    navigator.sceneLayer = Grid.SceneLayer.Creatures;
    kprefabId.AddTag(GameTags.Creatures.Walker, false);
    ChoreTable.Builder chore_table = new ChoreTable.Builder().Add((StateMachine.BaseDef) new FallStates.Def()).Add((StateMachine.BaseDef) new AnimInterruptStates.Def()).Add((StateMachine.BaseDef) new SweepBotTrappedStates.Def()).Add((StateMachine.BaseDef) new DeliverToSweepLockerStates.Def()).Add((StateMachine.BaseDef) new ReturnToChargeStationStates.Def()).Add((StateMachine.BaseDef) new SweepStates.Def()).Add((StateMachine.BaseDef) new IdleStates.Def());
    placedEntity.AddOrGet<LoopingSounds>();
    EntityTemplates.AddCreatureBrain(placedEntity, chore_table, GameTags.Robots.Models.SweepBot, (string) null);
    return placedEntity;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
    StorageUnloadMonitor.Instance smi = inst.GetSMI<StorageUnloadMonitor.Instance>();
    smi.sm.internalStorage.Set(inst.GetComponents<Storage>()[1], smi);
    inst.GetComponent<OrnamentReceptacle>();
    inst.GetSMI<CreatureFallMonitor.Instance>().anim = "idle_loop";
  }
}
