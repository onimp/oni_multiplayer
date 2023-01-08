// Decompiled with JetBrains decompiler
// Type: SpiceGrinderWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;

public class SpiceGrinderWorkable : Workable, IConfigurableConsumer
{
  [MyCmpAdd]
  public Notifier notifier;
  [SerializeField]
  public Vector3 finishedSeedDropOffset;
  private Notification notification;
  public SpiceGrinder.StatesInstance Grinder;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.requiredSkillPerk = Db.Get().SkillPerks.CanSpiceGrinder.Id;
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Spicing;
    this.attributeConverter = Db.Get().AttributeConverters.CookingSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Cooking.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_spice_grinder_kanim"))
    };
    this.SetWorkTime(5f);
    this.showProgressBar = true;
    this.lightEfficiencyBonus = true;
  }

  protected override void OnStartWork(Worker worker)
  {
    if (Object.op_Inequality((Object) this.Grinder.CurrentFood, (Object) null))
    {
      this.SetWorkTime((float) ((double) this.Grinder.CurrentFood.Calories * (1.0 / 1000.0) / 1000.0) * 5f);
    }
    else
    {
      Debug.LogWarning((object) "SpiceGrider attempted to start spicing with no food");
      this.StopWork(worker, true);
    }
    this.Grinder.UpdateFoodSymbol();
  }

  protected override void OnAbortWork(Worker worker)
  {
    if (Object.op_Equality((Object) this.Grinder.CurrentFood, (Object) null))
      return;
    this.Grinder.UpdateFoodSymbol();
  }

  protected override void OnCompleteWork(Worker worker)
  {
    if (Object.op_Equality((Object) this.Grinder.CurrentFood, (Object) null))
      return;
    this.Grinder.SpiceFood();
  }

  public IConfigurableConsumerOption[] GetSettingOptions() => (IConfigurableConsumerOption[]) ((IEnumerable<SpiceGrinder.Option>) SpiceGrinder.SettingOptions.Values).ToArray<SpiceGrinder.Option>();

  public IConfigurableConsumerOption GetSelectedOption() => (IConfigurableConsumerOption) this.Grinder.SelectedOption;

  public void SetSelectedOption(IConfigurableConsumerOption option) => this.Grinder.OnOptionSelected(option as SpiceGrinder.Option);
}
