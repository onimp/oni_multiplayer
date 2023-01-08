// Decompiled with JetBrains decompiler
// Type: ShearingStationConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TUNING;
using UnityEngine;

public class ShearingStationConfig : IBuildingConfig
{
  public const string ID = "ShearingStation";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] rawMinerals = MATERIALS.RAW_MINERALS;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ShearingStation", 3, 3, "shearing_station_kanim", 100, 10f, tieR4, rawMinerals, 1600f, BuildLocationRule.OnFloor, none2, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 60f;
    buildingDef.ExhaustKilowattsWhenActive = 0.125f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
    buildingDef.Floodable = true;
    buildingDef.Entombable = true;
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "large";
    buildingDef.UtilityInputOffset = new CellOffset(0, 0);
    buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
    buildingDef.DefaultAnimState = "on";
    buildingDef.ShowInBuildMenu = true;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RanchStationType, false);
    RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
    roomTracker.requiredRoomType = Db.Get().RoomTypes.CreaturePen.Id;
    roomTracker.requirement = RoomTracker.Requirement.Required;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    RanchStation.Def def = go.AddOrGetDef<RanchStation.Def>();
    def.IsCritterEligibleToBeRanchedCb = (Func<GameObject, RanchStation.Instance, bool>) ((creature_go, ranch_station_smi) =>
    {
      IShearable smi = creature_go.GetSMI<IShearable>();
      return smi != null && smi.IsFullyGrown();
    });
    def.OnRanchCompleteCb = (Action<GameObject>) (creature_go => creature_go.GetSMI<IShearable>().Shear());
    def.RancherInteractAnim = HashedString.op_Implicit("anim_interacts_shearingstation_kanim");
    def.WorkTime = 12f;
    def.RanchedPreAnim = HashedString.op_Implicit("shearing_pre");
    def.RanchedLoopAnim = HashedString.op_Implicit("shearing_loop");
    def.RanchedPstAnim = HashedString.op_Implicit("shearing_pst");
    Prioritizable.AddRef(go);
  }
}
