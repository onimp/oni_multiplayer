// Decompiled with JetBrains decompiler
// Type: BaseStaterpillarConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class BaseStaterpillarConfig
{
  public static GameObject BaseStaterpillar(
    string id,
    string name,
    string desc,
    string anim_file,
    string trait_id,
    bool is_baby,
    ObjectLayer conduitLayer,
    string connectorDefId,
    Tag inhaleTag,
    string symbolOverridePrefix = null,
    float warningLowTemperature = 283.15f,
    float warningHighTemperature = 293.15f,
    float lethalLowTemperature = 243.15f,
    float lethalHighTemperature = 343.15f,
    InhaleStates.Def inhaleDef = null)
  {
    string id1 = id;
    string name1 = name;
    string desc1 = desc;
    EffectorValues tieR0 = TUNING.DECOR.BONUS.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(anim_file));
    EffectorValues decor = tieR0;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id1, name1, desc1, 200f, anim, "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, noise);
    placedEntity.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Walker, false);
    placedEntity.AddTag(GameTags.Amphibious);
    string NavGridName = "WalkerBabyNavGrid";
    if (!is_baby)
    {
      NavGridName = "DreckoNavGrid";
      placedEntity.AddOrGetDef<ConduitSleepMonitor.Def>().conduitLayer = conduitLayer;
    }
    if (symbolOverridePrefix != null)
      placedEntity.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit(anim_file)), symbolOverridePrefix);
    EntityTemplates.ExtendEntityToBasicCreature(placedEntity, FactionManager.FactionID.Pest, trait_id, NavGridName, moveSpeed: 1f, onDeathDropCount: 2, drownVulnerable: false, entombVulnerable: false, warningLowTemperature: warningLowTemperature, warningHighTemperature: warningHighTemperature, lethalLowTemperature: lethalLowTemperature, lethalHighTemperature: lethalHighTemperature);
    placedEntity.AddOrGet<Trappable>();
    placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();
    placedEntity.AddOrGet<LoopingSounds>();
    Staterpillar staterpillar = placedEntity.AddOrGet<Staterpillar>();
    staterpillar.conduitLayer = conduitLayer;
    staterpillar.connectorDefId = connectorDefId;
    placedEntity.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
    placedEntity.AddWeapon(1f, 1f);
    EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, true);
    ChoreTable.Builder builder = new ChoreTable.Builder().Add((StateMachine.BaseDef) new DeathStates.Def()).Add((StateMachine.BaseDef) new AnimInterruptStates.Def()).Add((StateMachine.BaseDef) new GrowUpStates.Def(), is_baby).Add((StateMachine.BaseDef) new TrappedStates.Def()).Add((StateMachine.BaseDef) new IncubatingStates.Def(), is_baby).Add((StateMachine.BaseDef) new BaggedStates.Def()).Add((StateMachine.BaseDef) new FallStates.Def()).Add((StateMachine.BaseDef) new StunnedStates.Def()).Add((StateMachine.BaseDef) new DebugGoToStates.Def()).Add((StateMachine.BaseDef) new FleeStates.Def()).Add((StateMachine.BaseDef) new AttackStates.Def(), !is_baby).Add((StateMachine.BaseDef) new FixedCaptureStates.Def());
    RanchedStates.Def def = new RanchedStates.Def();
    def.WaitCellOffset = 2;
    int num = !is_baby ? 1 : 0;
    ChoreTable.Builder chore_table = builder.Add((StateMachine.BaseDef) def, num != 0).PushInterruptGroup().Add((StateMachine.BaseDef) new LayEggStates.Def(), !is_baby).Add((StateMachine.BaseDef) new EatStates.Def()).Add((StateMachine.BaseDef) new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP)).Add((StateMachine.BaseDef) inhaleDef, Tag.op_Inequality(inhaleTag, Tag.Invalid)).Add((StateMachine.BaseDef) new ConduitSleepStates.Def()).Add((StateMachine.BaseDef) new CallAdultStates.Def(), is_baby).PopInterruptGroup().Add((StateMachine.BaseDef) new CreatureSleepStates.Def()).Add((StateMachine.BaseDef) new IdleStates.Def()
    {
      customIdleAnim = new IdleStates.Def.IdleAnimCallback(BaseStaterpillarConfig.CustomIdleAnim)
    });
    EntityTemplates.AddCreatureBrain(placedEntity, chore_table, GameTags.Creatures.Species.StaterpillarSpecies, symbolOverridePrefix);
    return placedEntity;
  }

  public static GameObject SetupDiet(GameObject prefab, List<Diet.Info> diet_infos)
  {
    Diet diet = new Diet(diet_infos.ToArray());
    prefab.AddOrGetDef<CreatureCalorieMonitor.Def>().diet = diet;
    prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
    return prefab;
  }

  public static List<Diet.Info> RawMetalDiet(
    Tag poopTag,
    float caloriesPerKg,
    float producedConversionRate,
    string diseaseId,
    float diseasePerKgProduced)
  {
    List<SimHashes> simHashesList = new List<SimHashes>()
    {
      SimHashes.FoolsGold
    };
    List<Diet.Info> infoList = new List<Diet.Info>();
    foreach (Element element in ElementLoader.elements)
    {
      if (element.IsSolid && Tag.op_Equality(element.materialCategory, GameTags.Metal) && element.HasTag(GameTags.Ore) && !element.disabled && !simHashesList.Contains(element.id))
        infoList.Add(new Diet.Info(new HashSet<Tag>((IEnumerable<Tag>) new Tag[1]
        {
          element.tag
        }), poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
    }
    return infoList;
  }

  public static List<Diet.Info> RefinedMetalDiet(
    Tag poopTag,
    float caloriesPerKg,
    float producedConversionRate,
    string diseaseId,
    float diseasePerKgProduced)
  {
    List<Diet.Info> infoList = new List<Diet.Info>();
    foreach (Element element in ElementLoader.elements)
    {
      if (element.IsSolid && Tag.op_Equality(element.materialCategory, GameTags.RefinedMetal) && !element.disabled)
        infoList.Add(new Diet.Info(new HashSet<Tag>((IEnumerable<Tag>) new Tag[1]
        {
          element.tag
        }), poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
    }
    return infoList;
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
