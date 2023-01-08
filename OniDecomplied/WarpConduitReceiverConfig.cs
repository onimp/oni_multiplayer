// Decompiled with JetBrains decompiler
// Type: WarpConduitReceiverConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class WarpConduitReceiverConfig : IBuildingConfig
{
  public const string ID = "WarpConduitReceiver";
  private ConduitPortInfo liquidOutputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 1));
  private ConduitPortInfo gasOutputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(-1, 1));
  private ConduitPortInfo solidOutputPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(1, 1));

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues none = BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR5;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("WarpConduitReceiver", 4, 3, "warp_conduit_receiver_kanim", 250, 10f, tieR2, allMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
    buildingDef.DefaultAnimState = "off";
    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.ShowInBuildMenu = false;
    buildingDef.Disinfectable = false;
    buildingDef.Invincible = true;
    buildingDef.Repairable = false;
    return buildingDef;
  }

  private void AttachPorts(GameObject go)
  {
    go.AddComponent<ConduitSecondaryOutput>().portInfo = this.liquidOutputPort;
    go.AddComponent<ConduitSecondaryOutput>().portInfo = this.gasOutputPort;
    go.AddComponent<ConduitSecondaryOutput>().portInfo = this.solidOutputPort;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    Prioritizable.AddRef(go);
    PrimaryElement component = go.GetComponent<PrimaryElement>();
    component.SetElement(SimHashes.Unobtanium);
    component.Temperature = 294.15f;
    WarpConduitReceiver warpConduitReceiver = go.AddOrGet<WarpConduitReceiver>();
    warpConduitReceiver.liquidPortInfo = this.liquidOutputPort;
    warpConduitReceiver.gasPortInfo = this.gasOutputPort;
    warpConduitReceiver.solidPortInfo = this.solidOutputPort;
    Activatable activatable = go.AddOrGet<Activatable>();
    activatable.synchronizeAnims = true;
    activatable.workAnims = new HashedString[2]
    {
      HashedString.op_Implicit("touchpanel_interact_pre"),
      HashedString.op_Implicit("touchpanel_interact_loop")
    };
    activatable.workingPstComplete = new HashedString[1]
    {
      HashedString.op_Implicit("touchpanel_interact_pst")
    };
    activatable.workingPstFailed = new HashedString[1]
    {
      HashedString.op_Implicit("touchpanel_interact_pst")
    };
    activatable.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_warp_conduit_receiver_kanim"))
    };
    activatable.SetWorkTime(30f);
    go.AddComponent<ConduitSecondaryOutput>();
    go.GetComponent<KPrefabID>().AddTag(GameTags.Gravitas, false);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<BuildingCellVisualizer>();
    go.GetComponent<Deconstructable>().SetAllowDeconstruction(false);
    go.GetComponent<Activatable>().requiredSkillPerk = Db.Get().SkillPerks.CanStudyWorldObjects.Id;
  }

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
  {
    base.DoPostConfigurePreview(def, go);
    go.AddOrGet<BuildingCellVisualizer>();
    this.AttachPorts(go);
  }

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
    base.DoPostConfigureUnderConstruction(go);
    go.AddOrGet<BuildingCellVisualizer>();
    this.AttachPorts(go);
  }
}
