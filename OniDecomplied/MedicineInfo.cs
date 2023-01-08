// Decompiled with JetBrains decompiler
// Type: MedicineInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

[Serializable]
public class MedicineInfo
{
  public string id;
  public string effect;
  public MedicineInfo.MedicineType medicineType;
  public List<string> curedSicknesses;
  public string doctorStationId;

  public MedicineInfo(
    string id,
    string effect,
    MedicineInfo.MedicineType medicineType,
    string doctorStationId,
    string[] curedDiseases = null)
  {
    Debug.Assert(!string.IsNullOrEmpty(effect) || curedDiseases != null && curedDiseases.Length != 0, (object) "Medicine should have an effect or cure diseases");
    this.id = id;
    this.effect = effect;
    this.medicineType = medicineType;
    this.doctorStationId = doctorStationId;
    if (curedDiseases != null)
      this.curedSicknesses = new List<string>((IEnumerable<string>) curedDiseases);
    else
      this.curedSicknesses = new List<string>();
  }

  public Tag GetSupplyTag() => MedicineInfo.GetSupplyTagForStation(this.doctorStationId);

  public static Tag GetSupplyTagForStation(string stationID)
  {
    string str = stationID;
    Tag medicalSupplies = GameTags.MedicalSupplies;
    string name = ((Tag) ref medicalSupplies).Name;
    return new Tag(str + name);
  }

  public enum MedicineType
  {
    Booster,
    CureAny,
    CureSpecific,
  }
}
