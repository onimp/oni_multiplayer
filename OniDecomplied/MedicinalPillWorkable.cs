// Decompiled with JetBrains decompiler
// Type: MedicinalPillWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/MedicinalPillWorkable")]
public class MedicinalPillWorkable : Workable, IConsumableUIItem
{
  public MedicinalPill pill;

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.SetWorkTime(10f);
    this.showProgressBar = false;
    this.synchronizeAnims = false;
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal);
    this.CreateChore();
  }

  protected override void OnCompleteWork(Worker worker)
  {
    if (!string.IsNullOrEmpty(this.pill.info.effect))
    {
      Effects component = ((Component) worker).GetComponent<Effects>();
      EffectInstance effectInstance = component.Get(this.pill.info.effect);
      if (effectInstance != null)
        effectInstance.timeRemaining = effectInstance.effect.duration;
      else
        component.Add(this.pill.info.effect, true);
    }
    Sicknesses sicknesses = worker.GetSicknesses();
    foreach (string curedSickness in this.pill.info.curedSicknesses)
    {
      SicknessInstance sicknessInstance = sicknesses.Get(curedSickness);
      if (sicknessInstance != null)
      {
        Game.Instance.savedInfo.curedDisease = true;
        sicknessInstance.Cure();
      }
    }
    TracesExtesions.DeleteObject(((Component) this).gameObject);
  }

  private void CreateChore()
  {
    TakeMedicineChore takeMedicineChore = new TakeMedicineChore(this);
  }

  public bool CanBeTakenBy(GameObject consumer)
  {
    if (!string.IsNullOrEmpty(this.pill.info.effect))
    {
      Effects component = consumer.GetComponent<Effects>();
      if (Object.op_Equality((Object) component, (Object) null) || component.HasEffect(this.pill.info.effect))
        return false;
    }
    if (this.pill.info.medicineType == MedicineInfo.MedicineType.Booster)
      return true;
    Sicknesses sicknesses = consumer.GetSicknesses();
    if (this.pill.info.medicineType == MedicineInfo.MedicineType.CureAny && sicknesses.Count > 0)
      return true;
    foreach (ModifierInstance<Sickness> modifierInstance in (Modifications<Sickness, SicknessInstance>) sicknesses)
    {
      if (this.pill.info.curedSicknesses.Contains(modifierInstance.modifier.Id))
        return true;
    }
    return false;
  }

  public string ConsumableId
  {
    get
    {
      Tag tag = ((Component) this).PrefabID();
      return ((Tag) ref tag).Name;
    }
  }

  public string ConsumableName => ((Component) this).GetProperName();

  public int MajorOrder => (int) (this.pill.info.medicineType + 1000);

  public int MinorOrder => 0;

  public bool Display => true;
}
