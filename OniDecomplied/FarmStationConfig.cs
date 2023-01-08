// Decompiled with JetBrains decompiler
// Type: FarmStationConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class FarmStationConfig : IBuildingConfig
{
  public const string ID = "FarmStation";
  public static Tag MATERIAL_FOR_TINKER = GameTags.Fertilizer;
  public static Tag TINKER_TOOLS = FarmStationToolsConfig.tag;
  public const float MASS_PER_TINKER = 5f;
  public const float OUTPUT_TEMPERATURE = 308.15f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR1 = NOISE_POLLUTION.NOISY.TIER1;
    EffectorValues none = BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FarmStation", 2, 3, "planttender_kanim", 30, 30f, tieR4, allMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
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
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.FarmStationType, false);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    Storage storage = go.AddOrGet<Storage>();
    storage.showInUI = true;
    ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.RequestedItemTag = FarmStationConfig.MATERIAL_FOR_TINKER;
    manualDeliveryKg.refillMass = 5f;
    manualDeliveryKg.capacity = 50f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.FarmFetch.IdHash;
    TinkerStation tinkerStation = go.AddOrGet<TinkerStation>();
    tinkerStation.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_planttender_kanim"))
    };
    tinkerStation.inputMaterial = FarmStationConfig.MATERIAL_FOR_TINKER;
    tinkerStation.massPerTinker = 5f;
    tinkerStation.outputPrefab = FarmStationConfig.TINKER_TOOLS;
    tinkerStation.outputTemperature = 308.15f;
    tinkerStation.requiredSkillPerk = Db.Get().SkillPerks.CanFarmTinker.Id;
    tinkerStation.choreType = Db.Get().ChoreTypes.FarmingFabricate.IdHash;
    tinkerStation.fetchChoreType = Db.Get().ChoreTypes.FarmFetch.IdHash;
    RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
    roomTracker.requiredRoomType = Db.Get().RoomTypes.Farm.Id;
    roomTracker.requirement = RoomTracker.Requirement.Required;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabInitFn += FarmStationConfig.\u003C\u003Ec.\u003C\u003E9__7_0 ?? (FarmStationConfig.\u003C\u003Ec.\u003C\u003E9__7_0 = new KPrefabID.PrefabFn((object) FarmStationConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__7_0)));
  }
}
