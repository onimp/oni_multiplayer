// Decompiled with JetBrains decompiler
// Type: Klei.AI.AttributeLevels
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
  [SerializationConfig]
  [AddComponentMenu("KMonoBehaviour/scripts/AttributeLevels")]
  public class AttributeLevels : KMonoBehaviour, ISaveLoadable
  {
    private List<AttributeLevel> levels = new List<AttributeLevel>();
    [Serialize]
    private AttributeLevels.LevelSaveLoad[] saveLoadLevels = new AttributeLevels.LevelSaveLoad[0];

    public IEnumerator<AttributeLevel> GetEnumerator() => (IEnumerator<AttributeLevel>) this.levels.GetEnumerator();

    public AttributeLevels.LevelSaveLoad[] SaveLoadLevels
    {
      get => this.saveLoadLevels;
      set => this.saveLoadLevels = value;
    }

    protected virtual void OnPrefabInit()
    {
      foreach (AttributeInstance attribute in this.GetAttributes())
      {
        if (attribute.Attribute.IsTrainable)
        {
          AttributeLevel attributeLevel = new AttributeLevel(attribute);
          this.levels.Add(attributeLevel);
          attributeLevel.Apply(this);
        }
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    public void OnSerializing()
    {
      this.saveLoadLevels = new AttributeLevels.LevelSaveLoad[this.levels.Count];
      for (int index = 0; index < this.levels.Count; ++index)
      {
        this.saveLoadLevels[index].attributeId = this.levels[index].attribute.Attribute.Id;
        this.saveLoadLevels[index].experience = this.levels[index].experience;
        this.saveLoadLevels[index].level = this.levels[index].level;
      }
    }

    [System.Runtime.Serialization.OnDeserialized]
    public void OnDeserialized()
    {
      foreach (AttributeLevels.LevelSaveLoad saveLoadLevel in this.saveLoadLevels)
      {
        this.SetExperience(saveLoadLevel.attributeId, saveLoadLevel.experience);
        this.SetLevel(saveLoadLevel.attributeId, saveLoadLevel.level);
      }
    }

    public int GetLevel(Attribute attribute)
    {
      foreach (AttributeLevel level in this.levels)
      {
        if (attribute == level.attribute.Attribute)
          return level.GetLevel();
      }
      return 1;
    }

    public AttributeLevel GetAttributeLevel(string attribute_id)
    {
      foreach (AttributeLevel level in this.levels)
      {
        if (level.attribute.Attribute.Id == attribute_id)
          return level;
      }
      return (AttributeLevel) null;
    }

    public bool AddExperience(string attribute_id, float time_spent, float multiplier)
    {
      AttributeLevel attributeLevel = this.GetAttributeLevel(attribute_id);
      if (attributeLevel == null)
      {
        Debug.LogWarning((object) (attribute_id + " has no level."));
        return false;
      }
      time_spent *= multiplier;
      AttributeConverterInstance converterInstance = Db.Get().AttributeConverters.TrainingSpeed.Lookup((Component) this);
      if (converterInstance != null)
      {
        float num = converterInstance.Evaluate();
        time_spent += time_spent * num;
      }
      bool flag = attributeLevel.AddExperience(this, time_spent);
      attributeLevel.Apply(this);
      return flag;
    }

    public void SetLevel(string attribute_id, int level)
    {
      AttributeLevel attributeLevel = this.GetAttributeLevel(attribute_id);
      if (attributeLevel == null)
        return;
      attributeLevel.SetLevel(level);
      attributeLevel.Apply(this);
    }

    public void SetExperience(string attribute_id, float experience)
    {
      AttributeLevel attributeLevel = this.GetAttributeLevel(attribute_id);
      if (attributeLevel == null)
        return;
      attributeLevel.SetExperience(experience);
      attributeLevel.Apply(this);
    }

    public float GetPercentComplete(string attribute_id) => this.GetAttributeLevel(attribute_id).GetPercentComplete();

    public int GetMaxLevel()
    {
      int maxLevel = 0;
      foreach (AttributeLevel attributeLevel in this)
      {
        if (attributeLevel.GetLevel() > maxLevel)
          maxLevel = attributeLevel.GetLevel();
      }
      return maxLevel;
    }

    [Serializable]
    public struct LevelSaveLoad
    {
      public string attributeId;
      public float experience;
      public int level;
    }
  }
}
