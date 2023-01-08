// Decompiled with JetBrains decompiler
// Type: Klei.AI.Sickness
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
  [DebuggerDisplay("{base.Id}")]
  public abstract class Sickness : Resource
  {
    private StringKey name;
    private StringKey descriptiveSymptoms;
    private float sicknessDuration = 600f;
    public float fatalityDuration;
    public HashedString id;
    public Sickness.SicknessType sicknessType;
    public Sickness.Severity severity;
    public string recoveryEffect;
    public List<Sickness.InfectionVector> infectionVectors;
    private List<Sickness.SicknessComponent> components = new List<Sickness.SicknessComponent>();
    public Amount amount;
    public Attribute amountDeltaAttribute;
    public Attribute cureSpeedBase;

    public string Name => StringEntry.op_Implicit(Strings.Get(this.name));

    public float SicknessDuration => this.sicknessDuration;

    public StringKey DescriptiveSymptoms => this.descriptiveSymptoms;

    public Sickness(
      string id,
      Sickness.SicknessType type,
      Sickness.Severity severity,
      float immune_attack_strength,
      List<Sickness.InfectionVector> infection_vectors,
      float sickness_duration,
      string recovery_effect = null)
      : base(id, (ResourceSet) null, (string) null)
    {
      this.name = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".NAME");
      this.id = HashedString.op_Implicit(id);
      this.sicknessType = type;
      this.severity = severity;
      this.infectionVectors = infection_vectors;
      this.sicknessDuration = sickness_duration;
      this.recoveryEffect = recovery_effect;
      this.descriptiveSymptoms = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".DESCRIPTIVE_SYMPTOMS");
      this.cureSpeedBase = new Attribute(id + "CureSpeed", false, Attribute.Display.Normal, false);
      this.cureSpeedBase.BaseValue = 1f;
      this.cureSpeedBase.SetFormatter((IAttributeFormatter) new ToPercentAttributeFormatter(1f));
      Db.Get().Attributes.Add(this.cureSpeedBase);
    }

    public object[] Infect(
      GameObject go,
      SicknessInstance diseaseInstance,
      SicknessExposureInfo exposure_info)
    {
      object[] objArray = new object[this.components.Count];
      for (int index = 0; index < this.components.Count; ++index)
        objArray[index] = this.components[index].OnInfect(go, diseaseInstance);
      return objArray;
    }

    public void Cure(GameObject go, object[] componentData)
    {
      for (int index = 0; index < this.components.Count; ++index)
        this.components[index].OnCure(go, componentData[index]);
    }

    public List<Descriptor> GetSymptoms()
    {
      List<Descriptor> symptoms1 = new List<Descriptor>();
      for (int index = 0; index < this.components.Count; ++index)
      {
        List<Descriptor> symptoms2 = this.components[index].GetSymptoms();
        if (symptoms2 != null)
          symptoms1.AddRange((IEnumerable<Descriptor>) symptoms2);
      }
      if ((double) this.fatalityDuration > 0.0)
        symptoms1.Add(new Descriptor(string.Format((string) DUPLICANTS.DISEASES.DEATH_SYMPTOM, (object) GameUtil.GetFormattedCycles(this.fatalityDuration)), string.Format((string) DUPLICANTS.DISEASES.DEATH_SYMPTOM_TOOLTIP, (object) GameUtil.GetFormattedCycles(this.fatalityDuration)), (Descriptor.DescriptorType) 7, false));
      return symptoms1;
    }

    protected void AddSicknessComponent(Sickness.SicknessComponent cmp) => this.components.Add(cmp);

    public T GetSicknessComponent<T>() where T : Sickness.SicknessComponent
    {
      for (int index = 0; index < this.components.Count; ++index)
      {
        if (this.components[index] is T)
          return this.components[index] as T;
      }
      return default (T);
    }

    public virtual List<Descriptor> GetSicknessSourceDescriptors() => new List<Descriptor>();

    public List<Descriptor> GetQualitativeDescriptors()
    {
      List<Descriptor> qualitativeDescriptors = new List<Descriptor>();
      foreach (int infectionVector in this.infectionVectors)
      {
        switch (infectionVector)
        {
          case 0:
            qualitativeDescriptors.Add(new Descriptor((string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SKINBORNE, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SKINBORNE_TOOLTIP, (Descriptor.DescriptorType) 3, false));
            continue;
          case 1:
            qualitativeDescriptors.Add(new Descriptor((string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.FOODBORNE, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.FOODBORNE_TOOLTIP, (Descriptor.DescriptorType) 3, false));
            continue;
          case 2:
            qualitativeDescriptors.Add(new Descriptor((string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.AIRBORNE, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.AIRBORNE_TOOLTIP, (Descriptor.DescriptorType) 3, false));
            continue;
          case 3:
            qualitativeDescriptors.Add(new Descriptor((string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SUNBORNE, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SUNBORNE_TOOLTIP, (Descriptor.DescriptorType) 3, false));
            continue;
          default:
            continue;
        }
      }
      qualitativeDescriptors.Add(new Descriptor(StringEntry.op_Implicit(Strings.Get(this.descriptiveSymptoms)), "", (Descriptor.DescriptorType) 3, false));
      return qualitativeDescriptors;
    }

    public abstract class SicknessComponent
    {
      public abstract object OnInfect(GameObject go, SicknessInstance diseaseInstance);

      public abstract void OnCure(GameObject go, object instance_data);

      public virtual List<Descriptor> GetSymptoms() => (List<Descriptor>) null;
    }

    public enum InfectionVector
    {
      Contact,
      Digestion,
      Inhalation,
      Exposure,
    }

    public enum SicknessType
    {
      Pathogen,
      Ailment,
      Injury,
    }

    public enum Severity
    {
      Benign,
      Minor,
      Major,
      Critical,
    }
  }
}
