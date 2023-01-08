// Decompiled with JetBrains decompiler
// Type: BaseMoleConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public static class BaseMoleConfig
{
  private static readonly string[] SolidIdleAnims = new string[4]
  {
    "idle1",
    "idle2",
    "idle3",
    "idle4"
  };

  public static GameObject BaseMole(
    string id,
    string name,
    string desc,
    string traitId,
    string anim_file,
    bool is_baby,
    string symbolOverridePrefix = null,
    int on_death_drop_count = 10)
  {
    string id1 = id;
    string name1 = name;
    string desc1 = desc;
    EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(anim_file));
    EffectorValues decor = none;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id1, name1, desc1, 25f, anim, "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, noise);
    EntityTemplates.ExtendEntityToBasicCreature(placedEntity, FactionManager.FactionID.Pest, traitId, "DiggerNavGrid", onDeathDropCount: on_death_drop_count, entombVulnerable: false, warningLowTemperature: 123.149994f, warningHighTemperature: 673.15f, lethalLowTemperature: 73.1499939f, lethalHighTemperature: 773.15f);
    if (symbolOverridePrefix != null)
      placedEntity.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit(anim_file)), symbolOverridePrefix);
    placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();
    placedEntity.AddOrGet<Trappable>();
    placedEntity.AddOrGetDef<DiggerMonitor.Def>().depthToDig = MoleTuning.DEPTH_TO_HIDE;
    EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, true);
    placedEntity.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Walker, false);
    ChoreTable.Builder chore_table = new ChoreTable.Builder().Add((StateMachine.BaseDef) new DeathStates.Def()).Add((StateMachine.BaseDef) new AnimInterruptStates.Def()).Add((StateMachine.BaseDef) new FallStates.Def()).Add((StateMachine.BaseDef) new StunnedStates.Def()).Add((StateMachine.BaseDef) new DrowningStates.Def()).Add((StateMachine.BaseDef) new DiggerStates.Def()).Add((StateMachine.BaseDef) new GrowUpStates.Def(), is_baby).Add((StateMachine.BaseDef) new TrappedStates.Def()).Add((StateMachine.BaseDef) new IncubatingStates.Def(), is_baby).Add((StateMachine.BaseDef) new BaggedStates.Def()).Add((StateMachine.BaseDef) new DebugGoToStates.Def()).Add((StateMachine.BaseDef) new FleeStates.Def()).Add((StateMachine.BaseDef) new AttackStates.Def(), !is_baby).PushInterruptGroup().Add((StateMachine.BaseDef) new FixedCaptureStates.Def()).Add((StateMachine.BaseDef) new RanchedStates.Def(), !is_baby).Add((StateMachine.BaseDef) new LayEggStates.Def(), !is_baby).Add((StateMachine.BaseDef) new CreatureSleepStates.Def()).Add((StateMachine.BaseDef) new EatStates.Def()).Add((StateMachine.BaseDef) new NestingPoopState.Def(is_baby ? Tag.Invalid : SimHashes.Regolith.CreateTag())).Add((StateMachine.BaseDef) new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP)).PopInterruptGroup().Add((StateMachine.BaseDef) new IdleStates.Def()
    {
      customIdleAnim = new IdleStates.Def.IdleAnimCallback(BaseMoleConfig.CustomIdleAnim)
    });
    EntityTemplates.AddCreatureBrain(placedEntity, chore_table, GameTags.Creatures.Species.MoleSpecies, symbolOverridePrefix);
    return placedEntity;
  }

  public static List<Diet.Info> SimpleOreDiet(
    List<Tag> elementTags,
    float caloriesPerKg,
    float producedConversionRate)
  {
    List<Diet.Info> infoList1 = new List<Diet.Info>();
    foreach (Tag elementTag in elementTags)
    {
      List<Diet.Info> infoList2 = infoList1;
      HashSet<Tag> consumed_tags = new HashSet<Tag>();
      consumed_tags.Add(elementTag);
      Diet.Info info = new Diet.Info(consumed_tags, elementTag, caloriesPerKg, producedConversionRate, produce_solid_tile: true);
      infoList2.Add(info);
    }
    return infoList1;
  }

  private static HashedString CustomIdleAnim(IdleStates.Instance smi, ref HashedString pre_anim)
  {
    if (smi.gameObject.GetComponent<Navigator>().CurrentNavType == NavType.Solid)
    {
      int index = Random.Range(0, BaseMoleConfig.SolidIdleAnims.Length);
      return HashedString.op_Implicit(BaseMoleConfig.SolidIdleAnims[index]);
    }
    return smi.gameObject.GetDef<BabyMonitor.Def>() != null && Random.Range(0, 100) >= 90 ? HashedString.op_Implicit("drill_fail") : HashedString.op_Implicit("idle_loop");
  }
}
