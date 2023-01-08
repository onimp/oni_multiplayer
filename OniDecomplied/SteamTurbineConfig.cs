// Decompiled with JetBrains decompiler
// Type: SteamTurbineConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SteamTurbineConfig : IBuildingConfig
{
  public const string ID = "SteamTurbine";
  private const int HEIGHT = 4;
  private const int WIDTH = 5;
  private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>()
  {
    Storage.StoredItemModifier.Hide,
    Storage.StoredItemModifier.Insulate,
    Storage.StoredItemModifier.Seal
  };

  public override BuildingDef CreateBuildingDef()
  {
    string[] strArray = new string[2]
    {
      "RefinedMetal",
      "Plastic"
    };
    float[] construction_mass = new float[2]
    {
      BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
      BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0]
    };
    string[] construction_materials = strArray;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SteamTurbine", 5, 4, "steamturbine_kanim", 30, 60f, construction_mass, construction_materials, 1600f, BuildLocationRule.Anywhere, none2, noise, 1f);
    buildingDef.GeneratorWattageRating = 2000f;
    buildingDef.GeneratorBaseCapacity = 2000f;
    buildingDef.Entombable = true;
    buildingDef.IsFoundation = false;
    buildingDef.PermittedRotations = PermittedRotations.FlipH;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.RequiresPowerOutput = true;
    buildingDef.PowerOutputOffset = new CellOffset(1, 0);
    buildingDef.OverheatTemperature = 1273.15f;
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
    buildingDef.Deprecated = true;
    return buildingDef;
  }

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
    base.DoPostConfigureUnderConstruction(go);
    go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.CanPowerTinker.Id;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<Storage>().SetDefaultStoredItemModifiers(SteamTurbineConfig.StoredItemModifiers);
    Turbine turbine = go.AddOrGet<Turbine>();
    turbine.srcElem = SimHashes.Steam;
    MakeBaseSolid.Def def = go.AddOrGetDef<MakeBaseSolid.Def>();
    def.solidOffsets = new CellOffset[5];
    for (int index = 0; index < 5; ++index)
      def.solidOffsets[index] = new CellOffset(index - 2, 0);
    turbine.pumpKGRate = 10f;
    turbine.requiredMassFlowDifferential = 3f;
    turbine.minEmitMass = 10f;
    turbine.maxRPM = 4000f;
    turbine.rpmAcceleration = turbine.maxRPM / 30f;
    turbine.rpmDeceleration = turbine.maxRPM / 20f;
    turbine.minGenerationRPM = 3000f;
    turbine.minActiveTemperature = 500f;
    turbine.emitTemperature = 425f;
    go.AddOrGet<Generator>();
    go.AddOrGet<LogicOperationalController>();
    Prioritizable.AddRef(go);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabSpawnFn += SteamTurbineConfig.\u003C\u003Ec.\u003C\u003E9__6_0 ?? (SteamTurbineConfig.\u003C\u003Ec.\u003C\u003E9__6_0 = new KPrefabID.PrefabFn((object) SteamTurbineConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__6_0)));
  }
}
