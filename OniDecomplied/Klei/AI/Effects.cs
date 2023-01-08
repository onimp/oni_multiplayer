// Decompiled with JetBrains decompiler
// Type: Klei.AI.Effects
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
  [AddComponentMenu("KMonoBehaviour/scripts/Effects")]
  public class Effects : KMonoBehaviour, ISaveLoadable, ISim1000ms
  {
    [Serialize]
    private Effects.SaveLoadEffect[] saveLoadEffects;
    [Serialize]
    private Effects.SaveLoadImmunities[] saveLoadImmunities;
    private List<EffectInstance> effects = new List<EffectInstance>();
    private List<EffectInstance> effectsThatExpire = new List<EffectInstance>();
    private List<Effects.EffectImmunity> effectImmunites = new List<Effects.EffectImmunity>();

    protected virtual void OnPrefabInit() => this.autoRegisterSimRender = false;

    protected virtual void OnSpawn()
    {
      if (this.saveLoadImmunities != null)
      {
        foreach (Effects.SaveLoadImmunities saveLoadImmunity in this.saveLoadImmunities)
        {
          if (Db.Get().effects.Exists(saveLoadImmunity.effectID))
            this.AddImmunity(Db.Get().effects.Get(saveLoadImmunity.effectID), saveLoadImmunity.giverID);
        }
      }
      if (this.saveLoadEffects != null)
      {
        foreach (Effects.SaveLoadEffect saveLoadEffect in this.saveLoadEffects)
        {
          if (Db.Get().effects.Exists(saveLoadEffect.id))
          {
            EffectInstance effectInstance = this.Add(Db.Get().effects.Get(saveLoadEffect.id), true);
            if (effectInstance != null)
              effectInstance.timeRemaining = saveLoadEffect.timeRemaining;
          }
        }
      }
      if (this.effectsThatExpire.Count <= 0)
        return;
      SimAndRenderScheduler.instance.Add((object) this, this.simRenderLoadBalance);
    }

    public EffectInstance Get(string effect_id)
    {
      foreach (EffectInstance effect in this.effects)
      {
        if (effect.effect.Id == effect_id)
          return effect;
      }
      return (EffectInstance) null;
    }

    public EffectInstance Get(HashedString effect_id)
    {
      foreach (EffectInstance effect in this.effects)
      {
        if (HashedString.op_Equality(effect.effect.IdHash, effect_id))
          return effect;
      }
      return (EffectInstance) null;
    }

    public EffectInstance Get(Effect effect)
    {
      foreach (EffectInstance effect1 in this.effects)
      {
        if (effect1.effect == effect)
          return effect1;
      }
      return (EffectInstance) null;
    }

    public bool HasImmunityTo(Effect effect)
    {
      foreach (Effects.EffectImmunity effectImmunite in this.effectImmunites)
      {
        if (effectImmunite.effect == effect)
          return true;
      }
      return false;
    }

    public EffectInstance Add(string effect_id, bool should_save) => this.Add(Db.Get().effects.Get(effect_id), should_save);

    public EffectInstance Add(HashedString effect_id, bool should_save) => this.Add(Db.Get().effects.Get(effect_id), should_save);

    public EffectInstance Add(Effect effect, bool should_save)
    {
      if (this.HasImmunityTo(effect))
        return (EffectInstance) null;
      bool flag = true;
      Traits component = ((Component) this).GetComponent<Traits>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.IsEffectIgnored(effect))
        flag = false;
      if (!flag)
        return (EffectInstance) null;
      Attributes attributes = this.GetAttributes();
      EffectInstance effectInstance = this.Get(effect);
      if (!string.IsNullOrEmpty(effect.stompGroup))
      {
        for (int index = this.effects.Count - 1; index >= 0; --index)
        {
          if (this.effects[index] != effectInstance && this.effects[index].effect.stompGroup == effect.stompGroup)
            this.Remove(this.effects[index].effect);
        }
      }
      if (effectInstance == null)
      {
        effectInstance = new EffectInstance(((Component) this).gameObject, effect, should_save);
        effect.AddTo(attributes);
        this.effects.Add(effectInstance);
        if ((double) effect.duration > 0.0)
        {
          this.effectsThatExpire.Add(effectInstance);
          if (this.effectsThatExpire.Count == 1)
            SimAndRenderScheduler.instance.Add((object) this, this.simRenderLoadBalance);
        }
        this.Trigger(-1901442097, (object) effect);
      }
      effectInstance.timeRemaining = effect.duration;
      return effectInstance;
    }

    public void Remove(Effect effect) => this.Remove(effect.IdHash);

    public void Remove(HashedString effect_id)
    {
      for (int index1 = 0; index1 < this.effectsThatExpire.Count; ++index1)
      {
        if (HashedString.op_Equality(this.effectsThatExpire[index1].effect.IdHash, effect_id))
        {
          int index2 = this.effectsThatExpire.Count - 1;
          this.effectsThatExpire[index1] = this.effectsThatExpire[index2];
          this.effectsThatExpire.RemoveAt(index2);
          if (this.effectsThatExpire.Count == 0)
          {
            SimAndRenderScheduler.instance.Remove((object) this);
            break;
          }
          break;
        }
      }
      for (int index3 = 0; index3 < this.effects.Count; ++index3)
      {
        if (HashedString.op_Equality(this.effects[index3].effect.IdHash, effect_id))
        {
          Attributes attributes = this.GetAttributes();
          EffectInstance effect1 = this.effects[index3];
          effect1.OnCleanUp();
          Effect effect2 = effect1.effect;
          effect2.RemoveFrom(attributes);
          int index4 = this.effects.Count - 1;
          this.effects[index3] = this.effects[index4];
          this.effects.RemoveAt(index4);
          this.Trigger(-1157678353, (object) effect2);
          break;
        }
      }
    }

    public void Remove(string effect_id)
    {
      for (int index1 = 0; index1 < this.effectsThatExpire.Count; ++index1)
      {
        if (this.effectsThatExpire[index1].effect.Id == effect_id)
        {
          int index2 = this.effectsThatExpire.Count - 1;
          this.effectsThatExpire[index1] = this.effectsThatExpire[index2];
          this.effectsThatExpire.RemoveAt(index2);
          if (this.effectsThatExpire.Count == 0)
          {
            SimAndRenderScheduler.instance.Remove((object) this);
            break;
          }
          break;
        }
      }
      for (int index3 = 0; index3 < this.effects.Count; ++index3)
      {
        if (this.effects[index3].effect.Id == effect_id)
        {
          Attributes attributes = this.GetAttributes();
          EffectInstance effect1 = this.effects[index3];
          effect1.OnCleanUp();
          Effect effect2 = effect1.effect;
          effect2.RemoveFrom(attributes);
          int index4 = this.effects.Count - 1;
          this.effects[index3] = this.effects[index4];
          this.effects.RemoveAt(index4);
          this.Trigger(-1157678353, (object) effect2);
          break;
        }
      }
    }

    public bool HasEffect(HashedString effect_id)
    {
      foreach (EffectInstance effect in this.effects)
      {
        if (HashedString.op_Equality(effect.effect.IdHash, effect_id))
          return true;
      }
      return false;
    }

    public bool HasEffect(string effect_id)
    {
      foreach (EffectInstance effect in this.effects)
      {
        if (effect.effect.Id == effect_id)
          return true;
      }
      return false;
    }

    public bool HasEffect(Effect effect)
    {
      foreach (EffectInstance effect1 in this.effects)
      {
        if (effect1.effect == effect)
          return true;
      }
      return false;
    }

    public void Sim1000ms(float dt)
    {
      for (int index = 0; index < this.effectsThatExpire.Count; ++index)
      {
        EffectInstance effectInstance = this.effectsThatExpire[index];
        if (effectInstance.IsExpired())
          this.Remove(effectInstance.effect);
        effectInstance.timeRemaining -= dt;
      }
    }

    public void AddImmunity(Effect effect, string giverID, bool shouldSave = true)
    {
      if (giverID != null)
      {
        foreach (Effects.EffectImmunity effectImmunite in this.effectImmunites)
        {
          if (effectImmunite.giverID == giverID && effectImmunite.effect == effect)
            return;
        }
      }
      this.effectImmunites.Add(new Effects.EffectImmunity(effect, giverID, shouldSave));
    }

    public void RemoveImmunity(Effect effect, string ID)
    {
      Effects.EffectImmunity effectImmunity = new Effects.EffectImmunity();
      bool flag = false;
      foreach (Effects.EffectImmunity effectImmunite in this.effectImmunites)
      {
        if (effectImmunite.effect == effect && (ID == null || ID == effectImmunite.giverID))
        {
          effectImmunity = effectImmunite;
          flag = true;
        }
      }
      if (!flag)
        return;
      this.effectImmunites.Remove(effectImmunity);
    }

    [System.Runtime.Serialization.OnSerializing]
    internal void OnSerializing()
    {
      List<Effects.SaveLoadEffect> saveLoadEffectList = new List<Effects.SaveLoadEffect>();
      foreach (EffectInstance effect in this.effects)
      {
        if (effect.shouldSave)
        {
          Effects.SaveLoadEffect saveLoadEffect = new Effects.SaveLoadEffect()
          {
            id = effect.effect.Id,
            timeRemaining = effect.timeRemaining,
            saved = true
          };
          saveLoadEffectList.Add(saveLoadEffect);
        }
      }
      this.saveLoadEffects = saveLoadEffectList.ToArray();
      List<Effects.SaveLoadImmunities> saveLoadImmunitiesList = new List<Effects.SaveLoadImmunities>();
      foreach (Effects.EffectImmunity effectImmunite in this.effectImmunites)
      {
        if (effectImmunite.shouldSave)
        {
          Effect effect = effectImmunite.effect;
          Effects.SaveLoadImmunities saveLoadImmunities = new Effects.SaveLoadImmunities()
          {
            effectID = effect.Id,
            giverID = effectImmunite.giverID,
            saved = true
          };
          saveLoadImmunitiesList.Add(saveLoadImmunities);
        }
      }
      this.saveLoadImmunities = saveLoadImmunitiesList.ToArray();
    }

    public List<Effects.SaveLoadImmunities> GetAllImmunitiesForSerialization()
    {
      List<Effects.SaveLoadImmunities> forSerialization = new List<Effects.SaveLoadImmunities>();
      foreach (Effects.EffectImmunity effectImmunite in this.effectImmunites)
      {
        Effect effect = effectImmunite.effect;
        Effects.SaveLoadImmunities saveLoadImmunities = new Effects.SaveLoadImmunities()
        {
          effectID = effect.Id,
          giverID = effectImmunite.giverID,
          saved = effectImmunite.shouldSave
        };
        forSerialization.Add(saveLoadImmunities);
      }
      return forSerialization;
    }

    public List<Effects.SaveLoadEffect> GetAllEffectsForSerialization()
    {
      List<Effects.SaveLoadEffect> forSerialization = new List<Effects.SaveLoadEffect>();
      foreach (EffectInstance effect in this.effects)
      {
        Effects.SaveLoadEffect saveLoadEffect = new Effects.SaveLoadEffect()
        {
          id = effect.effect.Id,
          timeRemaining = effect.timeRemaining,
          saved = effect.shouldSave
        };
        forSerialization.Add(saveLoadEffect);
      }
      return forSerialization;
    }

    public List<EffectInstance> GetTimeLimitedEffects() => this.effectsThatExpire;

    public void CopyEffects(Effects source)
    {
      foreach (EffectInstance effect in source.effects)
        this.Add(effect.effect, effect.shouldSave).timeRemaining = effect.timeRemaining;
      foreach (EffectInstance effectInstance in source.effectsThatExpire)
        this.Add(effectInstance.effect, effectInstance.shouldSave).timeRemaining = effectInstance.timeRemaining;
    }

    [Serializable]
    public struct EffectImmunity
    {
      public string giverID;
      public Effect effect;
      public bool shouldSave;

      public EffectImmunity(Effect e, string id, bool save = true)
      {
        this.giverID = id;
        this.effect = e;
        this.shouldSave = save;
      }
    }

    [Serializable]
    public struct SaveLoadImmunities
    {
      public string giverID;
      public string effectID;
      public bool saved;
    }

    [Serializable]
    public struct SaveLoadEffect
    {
      public string id;
      public float timeRemaining;
      public bool saved;
    }
  }
}
