// Decompiled with JetBrains decompiler
// Type: PowerControlStationConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class PowerControlStationConfig : IBuildingConfig
{
  public const string ID = "PowerControlStation";
  public static Tag MATERIAL_FOR_TINKER = GameTags.RefinedMetal;
  public static Tag TINKER_TOOLS = PowerStationToolsConfig.tag;
  public const float MASS_PER_TINKER = 5f;
  public static string ROLE_PERK = "CanPowerTinker";
  public const float OUTPUT_TEMPERATURE = 308.15f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues tieR1 = NOISE_POLLUTION.NOISY.TIER1;
    EffectorValues none = BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("PowerControlStation", 2, 4, "electricianworkdesk_kanim", 30, 30f, tieR3, refinedMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
    buildingDef.ViewMode = OverlayModes.Rooms.ID;
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "large";
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.PowerStation, false);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    Storage storage = go.AddOrGet<Storage>();
    storage.capacityKg = 50f;
    storage.showInUI = true;
    List<Tag> tagList = new List<Tag>();
    tagList.Add(PowerControlStationConfig.MATERIAL_FOR_TINKER);
    storage.storageFilters = tagList;
    TinkerStation tinkerStation = go.AddOrGet<TinkerStation>();
    tinkerStation.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_electricianworkdesk_kanim"))
    };
    tinkerStation.inputMaterial = PowerControlStationConfig.MATERIAL_FOR_TINKER;
    tinkerStation.massPerTinker = 5f;
    tinkerStation.outputPrefab = PowerControlStationConfig.TINKER_TOOLS;
    tinkerStation.outputTemperature = 308.15f;
    tinkerStation.requiredSkillPerk = PowerControlStationConfig.ROLE_PERK;
    tinkerStation.choreType = Db.Get().ChoreTypes.PowerFabricate.IdHash;
    tinkerStation.useFilteredStorage = true;
    tinkerStation.fetchChoreType = Db.Get().ChoreTypes.PowerFetch.IdHash;
    RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
    roomTracker.requiredRoomType = Db.Get().RoomTypes.PowerPlant.Id;
    roomTracker.requirement = RoomTracker.Requirement.Required;
    Prioritizable.AddRef(go);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabInitFn += PowerControlStationConfig.\u003C\u003Ec.\u003C\u003E9__8_0 ?? (PowerControlStationConfig.\u003C\u003Ec.\u003C\u003E9__8_0 = new KPrefabID.PrefabFn((object) PowerControlStationConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__8_0)));
  }
}
