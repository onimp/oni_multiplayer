// Decompiled with JetBrains decompiler
// Type: Klei.AI.AttributeLevel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Klei.AI
{
  public class AttributeLevel
  {
    public float experience;
    public int level;
    public AttributeInstance attribute;
    public AttributeModifier modifier;
    public Notification notification;

    public AttributeLevel(AttributeInstance attribute)
    {
      this.notification = new Notification((string) MISC.NOTIFICATIONS.LEVELUP.NAME, NotificationType.Good, new Func<List<Notification>, object, string>(AttributeLevel.OnLevelUpTooltip));
      this.attribute = attribute;
    }

    public int GetLevel() => this.level;

    public void Apply(AttributeLevels levels)
    {
      Attributes attributes = levels.GetAttributes();
      if (this.modifier != null)
      {
        attributes.Remove(this.modifier);
        this.modifier = (AttributeModifier) null;
      }
      this.modifier = new AttributeModifier(this.attribute.Id, (float) this.GetLevel(), (string) DUPLICANTS.MODIFIERS.SKILLLEVEL.NAME);
      attributes.Add(this.modifier);
    }

    public void SetExperience(float experience) => this.experience = experience;

    public void SetLevel(int level) => this.level = level;

    public float GetExperienceForNextLevel()
    {
      float num = (float) ((double) Mathf.Pow((float) this.level / (float) DUPLICANTSTATS.ATTRIBUTE_LEVELING.MAX_GAINED_ATTRIBUTE_LEVEL, DUPLICANTSTATS.ATTRIBUTE_LEVELING.EXPERIENCE_LEVEL_POWER) * (double) DUPLICANTSTATS.ATTRIBUTE_LEVELING.TARGET_MAX_LEVEL_CYCLE * 600.0);
      return (float) ((double) Mathf.Pow(((float) this.level + 1f) / (float) DUPLICANTSTATS.ATTRIBUTE_LEVELING.MAX_GAINED_ATTRIBUTE_LEVEL, DUPLICANTSTATS.ATTRIBUTE_LEVELING.EXPERIENCE_LEVEL_POWER) * (double) DUPLICANTSTATS.ATTRIBUTE_LEVELING.TARGET_MAX_LEVEL_CYCLE * 600.0) - num;
    }

    public float GetPercentComplete() => this.experience / this.GetExperienceForNextLevel();

    public void LevelUp(AttributeLevels levels)
    {
      ++this.level;
      this.experience = 0.0f;
      this.Apply(levels);
      this.experience = 0.0f;
      if (Object.op_Inequality((Object) PopFXManager.Instance, (Object) null))
        PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, this.attribute.modifier.Name, levels.transform, new Vector3(0.0f, 0.5f, 0.0f));
      ((Component) levels).GetComponent<Notifier>().Add(this.notification, string.Format((string) MISC.NOTIFICATIONS.LEVELUP.SUFFIX, (object) this.attribute.modifier.Name, (object) this.level));
      UpgradeFX.Instance instance = new UpgradeFX.Instance((IStateMachineTarget) ((Component) levels).GetComponent<KMonoBehaviour>(), new Vector3(0.0f, 0.0f, -0.1f));
      ReportManager.Instance.ReportValue(ReportManager.ReportType.LevelUp, 1f, ((Component) levels).GetProperName());
      instance.StartSM();
      levels.Trigger(-110704193, (object) this.attribute.Id);
    }

    public bool AddExperience(AttributeLevels levels, float experience)
    {
      if (this.level >= DUPLICANTSTATS.ATTRIBUTE_LEVELING.MAX_GAINED_ATTRIBUTE_LEVEL)
        return false;
      this.experience += experience;
      this.experience = Mathf.Max(0.0f, this.experience);
      if ((double) this.experience < (double) this.GetExperienceForNextLevel())
        return false;
      this.LevelUp(levels);
      return true;
    }

    private static string OnLevelUpTooltip(List<Notification> notifications, object data) => (string) MISC.NOTIFICATIONS.LEVELUP.TOOLTIP + notifications.ReduceMessages(false);
  }
}
