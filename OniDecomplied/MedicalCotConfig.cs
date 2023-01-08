// Decompiled with JetBrains decompiler
// Type: MedicalCotConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class MedicalCotConfig : IBuildingConfig
{
  public const string ID = "MedicalCot";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] rawMinerals = MATERIALS.RAW_MINERALS;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MedicalCot", 3, 2, "medical_cot_kanim", 10, 10f, tieR3, rawMinerals, 1600f, BuildLocationRule.OnFloor, none2, noise);
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.Clinic, false);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.GetComponent<KAnimControllerBase>().initialAnim = "off";
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.BedType, false);
    Clinic clinic = go.AddOrGet<Clinic>();
    clinic.doctorVisitInterval = 300f;
    clinic.workerInjuredAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_healing_bed_kanim"))
    };
    clinic.workerDiseasedAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_med_cot_sick_kanim"))
    };
    clinic.workLayer = Grid.SceneLayer.BuildingFront;
    string str1 = "MedicalCot";
    string str2 = "MedicalCotDoctored";
    clinic.healthEffect = str1;
    clinic.doctoredHealthEffect = str2;
    clinic.diseaseEffect = str1;
    clinic.doctoredDiseaseEffect = str2;
    clinic.doctoredPlaceholderEffect = "DoctoredOffCotEffect";
    RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
    roomTracker.requiredRoomType = Db.Get().RoomTypes.Hospital.Id;
    roomTracker.requirement = RoomTracker.Requirement.CustomRecommended;
    roomTracker.customStatusItemID = Db.Get().BuildingStatusItems.ClinicOutsideHospital.Id;
    go.AddOrGet<Sleepable>().overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_med_cot_sick_kanim"))
    };
    DoctorChoreWorkable doctorChoreWorkable = go.AddOrGet<DoctorChoreWorkable>();
    doctorChoreWorkable.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_med_cot_doctor_kanim"))
    };
    doctorChoreWorkable.workTime = 45f;
    go.AddOrGet<Ownable>().slotID = Db.Get().AssignableSlots.Clinic.Id;
  }
}
