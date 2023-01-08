// Decompiled with JetBrains decompiler
// Type: DoctorStationDoctorWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/DoctorStationDoctorWorkable")]
public class DoctorStationDoctorWorkable : Workable
{
  [MyCmpReq]
  private DoctorStation station;

  private DoctorStationDoctorWorkable() => this.synchronizeAnims = false;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.attributeConverter = Db.Get().AttributeConverters.DoctorSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.MedicalAid.Id;
    this.skillExperienceMultiplier = SKILLS.BARELY_EVER_EXPERIENCE;
  }

  protected override void OnSpawn() => base.OnSpawn();

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    this.station.SetHasDoctor(true);
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    this.station.SetHasDoctor(false);
  }

  protected override void OnCompleteWork(Worker worker)
  {
    base.OnCompleteWork(worker);
    this.station.CompleteDoctoring();
  }
}
