// Decompiled with JetBrains decompiler
// Type: Klei.AI.Sicknesses
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace Klei.AI
{
  public class Sicknesses : Modifications<Sickness, SicknessInstance>
  {
    public Sicknesses(GameObject go)
      : base(go, (ResourceSet<Sickness>) Db.Get().Sicknesses)
    {
    }

    public void Infect(SicknessExposureInfo exposure_info)
    {
      Sickness modifier = Db.Get().Sicknesses.Get(exposure_info.sicknessID);
      if (this.Has(modifier))
        return;
      this.CreateInstance(modifier).ExposureInfo = exposure_info;
    }

    public override SicknessInstance CreateInstance(Sickness sickness)
    {
      SicknessInstance instance = new SicknessInstance(this.gameObject, sickness);
      this.Add(instance);
      this.Trigger(GameHashes.SicknessAdded, (object) instance);
      ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseAdded, 1f, this.gameObject.GetProperName());
      return instance;
    }

    public bool IsInfected() => this.Count > 0;

    public bool Cure(Sickness sickness) => this.Cure(sickness.Id);

    public bool Cure(string sickness_id)
    {
      SicknessInstance sicknessInstance1 = (SicknessInstance) null;
      foreach (SicknessInstance sicknessInstance2 in (Modifications<Sickness, SicknessInstance>) this)
      {
        if (sicknessInstance2.modifier.Id == sickness_id)
        {
          sicknessInstance1 = sicknessInstance2;
          break;
        }
      }
      if (sicknessInstance1 == null)
        return false;
      this.Remove(sicknessInstance1);
      this.Trigger(GameHashes.SicknessCured, (object) sicknessInstance1);
      ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseAdded, -1f, this.gameObject.GetProperName());
      return true;
    }
  }
}
