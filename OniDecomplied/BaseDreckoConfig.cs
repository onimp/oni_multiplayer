// Decompiled with JetBrains decompiler
// Type: BaseDreckoConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class BaseDreckoConfig
{
  public static GameObject BaseDrecko(
    string id,
    string name,
    string desc,
    string anim_file,
    string trait_id,
    bool is_baby,
    string symbol_override_prefix,
    float warnLowTemp,
    float warnHighTemp)
  {
    string id1 = id;
    string name1 = name;
    string desc1 = desc;
    EffectorValues tieR0 = TUNING.DECOR.BONUS.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(anim_file));
    EffectorValues decor = tieR0;
    EffectorValues noise = new EffectorValues();
    double defaultTemperature = ((double) warnLowTemp + (double) warnHighTemp) / 2.0;
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id1, name1, desc1, 200f, anim, "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, noise, defaultTemperature: ((float) defaultTemperature));
    KPrefabID component = placedEntity.GetComponent<KPrefabID>();
    component.AddTag(GameTags.Creatures.Walker, false);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    component.prefabInitFn += BaseDreckoConfig.\u003C\u003Ec.\u003C\u003E9__0_0 ?? (BaseDreckoConfig.\u003C\u003Ec.\u003C\u003E9__0_0 = new KPrefabID.PrefabFn((object) BaseDreckoConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CBaseDrecko\u003Eb__0_0)));
    string NavGridName = "DreckoNavGrid";
    if (is_baby)
      NavGridName = "DreckoBabyNavGrid";
    EntityTemplates.ExtendEntityToBasicCreature(placedEntity, FactionManager.FactionID.Pest, trait_id, NavGridName, moveSpeed: 1f, onDeathDropCount: 2, entombVulnerable: false, warningLowTemperature: warnLowTemp, warningHighTemperature: warnHighTemp, lethalLowTemperature: (warnLowTemp - 20f), lethalHighTemperature: (warnHighTemp + 20f));
    if (!string.IsNullOrEmpty(symbol_override_prefix))
      placedEntity.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit(anim_file)), symbol_override_prefix);
    placedEntity.AddOrGet<Trappable>();
    placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();
    placedEntity.AddOrGet<LoopingSounds>();
    placedEntity.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
    placedEntity.AddWeapon(1f, 1f);
    EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, true);
    ChoreTable.Builder chore_table = new ChoreTable.Builder().Add((StateMachine.BaseDef) new DeathStates.Def()).Add((StateMachine.BaseDef) new AnimInterruptStates.Def()).Add((StateMachine.BaseDef) new GrowUpStates.Def(), is_baby).Add((StateMachine.BaseDef) new TrappedStates.Def()).Add((StateMachine.BaseDef) new IncubatingStates.Def(), is_baby).Add((StateMachine.BaseDef) new BaggedStates.Def()).Add((StateMachine.BaseDef) new FallStates.Def()).Add((StateMachine.BaseDef) new StunnedStates.Def()).Add((StateMachine.BaseDef) new DrowningStates.Def()).Add((StateMachine.BaseDef) new DebugGoToStates.Def()).Add((StateMachine.BaseDef) new FleeStates.Def()).Add((StateMachine.BaseDef) new AttackStates.Def(), !is_baby).PushInterruptGroup().Add((StateMachine.BaseDef) new FixedCaptureStates.Def()).Add((StateMachine.BaseDef) new RanchedStates.Def(), !is_baby).Add((StateMachine.BaseDef) new LayEggStates.Def(), !is_baby).Add((StateMachine.BaseDef) new EatStates.Def()).Add((StateMachine.BaseDef) new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP)).Add((StateMachine.BaseDef) new CallAdultStates.Def(), is_baby).PopInterruptGroup().Add((StateMachine.BaseDef) new CreatureSleepStates.Def()).Add((StateMachine.BaseDef) new IdleStates.Def()
    {
      customIdleAnim = new IdleStates.Def.IdleAnimCallback(BaseDreckoConfig.CustomIdleAnim)
    });
    EntityTemplates.AddCreatureBrain(placedEntity, chore_table, GameTags.Creatures.Species.DreckoSpecies, symbol_override_prefix);
    return placedEntity;
  }

  private static HashedString CustomIdleAnim(IdleStates.Instance smi, ref HashedString pre_anim)
  {
    CellOffset offset;
    // ISSUE: explicit constructor call
    ((CellOffset) ref offset).\u002Ector(0, -1);
    bool facing = smi.GetComponent<Facing>().GetFacing();
    switch (smi.GetComponent<Navigator>().CurrentNavType)
    {
      case NavType.Floor:
        offset = facing ? new CellOffset(1, -1) : new CellOffset(-1, -1);
        break;
      case NavType.Ceiling:
        offset = facing ? new CellOffset(1, 1) : new CellOffset(-1, 1);
        break;
    }
    HashedString hashedString = HashedString.op_Implicit("idle_loop");
    int num = Grid.OffsetCell(Grid.PosToCell((StateMachine.Instance) smi), offset);
    if (Grid.IsValidCell(num) && !Grid.Solid[num])
    {
      pre_anim = HashedString.op_Implicit("idle_loop_hang_pre");
      hashedString = HashedString.op_Implicit("idle_loop_hang");
    }
    return hashedString;
  }
}
