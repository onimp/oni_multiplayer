// Decompiled with JetBrains decompiler
// Type: MedicinalPill
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/game/MedicinalPill")]
public class MedicinalPill : KMonoBehaviour, IGameObjectEffectDescriptor
{
  public MedicineInfo info;

  protected virtual void OnSpawn() => base.OnSpawn();

  public List<Descriptor> EffectDescriptors(GameObject go)
  {
    List<Descriptor> descriptorList = new List<Descriptor>();
    if (string.IsNullOrEmpty(this.info.doctorStationId))
    {
      if (this.info.medicineType == MedicineInfo.MedicineType.Booster)
        descriptorList.Add(new Descriptor(string.Format((string) DUPLICANTS.DISEASES.MEDICINE.SELF_ADMINISTERED_BOOSTER), string.Format((string) DUPLICANTS.DISEASES.MEDICINE.SELF_ADMINISTERED_BOOSTER_TOOLTIP), (Descriptor.DescriptorType) 1, false));
      else
        descriptorList.Add(new Descriptor(string.Format((string) DUPLICANTS.DISEASES.MEDICINE.SELF_ADMINISTERED_CURE), string.Format((string) DUPLICANTS.DISEASES.MEDICINE.SELF_ADMINISTERED_CURE_TOOLTIP), (Descriptor.DescriptorType) 1, false));
    }
    else
    {
      string properName = Assets.GetPrefab(Tag.op_Implicit(this.info.doctorStationId)).GetProperName();
      if (this.info.medicineType == MedicineInfo.MedicineType.Booster)
        descriptorList.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.DOCTOR_ADMINISTERED_BOOSTER.Replace("{Station}", properName)), string.Format(DUPLICANTS.DISEASES.MEDICINE.DOCTOR_ADMINISTERED_BOOSTER_TOOLTIP.Replace("{Station}", properName)), (Descriptor.DescriptorType) 1, false));
      else
        descriptorList.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.DOCTOR_ADMINISTERED_CURE.Replace("{Station}", properName)), string.Format(DUPLICANTS.DISEASES.MEDICINE.DOCTOR_ADMINISTERED_CURE_TOOLTIP.Replace("{Station}", properName)), (Descriptor.DescriptorType) 1, false));
    }
    switch (this.info.medicineType)
    {
      case MedicineInfo.MedicineType.CureAny:
        descriptorList.Add(new Descriptor(string.Format((string) DUPLICANTS.DISEASES.MEDICINE.CURES_ANY), string.Format((string) DUPLICANTS.DISEASES.MEDICINE.CURES_ANY_TOOLTIP), (Descriptor.DescriptorType) 1, false));
        break;
      case MedicineInfo.MedicineType.CureSpecific:
        List<string> stringList = new List<string>();
        foreach (string curedSickness in this.info.curedSicknesses)
          stringList.Add(StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.DISEASES." + curedSickness.ToUpper() + ".NAME")));
        string str = string.Join(",", stringList.ToArray());
        descriptorList.Add(new Descriptor(string.Format((string) DUPLICANTS.DISEASES.MEDICINE.CURES, (object) str), string.Format((string) DUPLICANTS.DISEASES.MEDICINE.CURES_TOOLTIP, (object) str), (Descriptor.DescriptorType) 1, false));
        break;
    }
    if (!string.IsNullOrEmpty(this.info.effect))
    {
      Effect effect = Db.Get().effects.Get(this.info.effect);
      descriptorList.Add(new Descriptor(string.Format((string) DUPLICANTS.MODIFIERS.MEDICINE_GENERICPILL.EFFECT_DESC, (object) effect.Name), string.Format("{0}\n{1}", (object) effect.description, (object) Effect.CreateTooltip(effect, true)), (Descriptor.DescriptorType) 1, false));
    }
    return descriptorList;
  }

  public List<Descriptor> GetDescriptors(GameObject go) => this.EffectDescriptors(go);
}
