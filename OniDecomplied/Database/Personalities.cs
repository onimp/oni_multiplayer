// Decompiled with JetBrains decompiler
// Type: Database.Personalities
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;

namespace Database
{
  public class Personalities : ResourceSet<Personality>
  {
    public Personalities()
    {
      foreach (Personalities.PersonalityInfo entry in AsyncLoadManager<IGlobalAsyncLoader>.AsyncLoader<Personalities.PersonalityLoader>.Get().entries)
        this.Add(new Personality(entry.Name.ToUpper(), StringEntry.op_Implicit(Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.NAME", (object) entry.Name.ToUpper()))), entry.Gender.ToUpper(), entry.PersonalityType, entry.StressTrait, entry.JoyTrait, entry.StickerType, entry.CongenitalTrait, entry.HeadShape, entry.Mouth, entry.Neck, entry.Eyes, entry.Hair, entry.Body, StringEntry.op_Implicit(Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", (object) entry.Name.ToUpper()))), entry.ValidStarter));
    }

    private void AddTrait(Personality personality, string trait_name)
    {
      Trait trait = Db.Get().traits.TryGet(trait_name);
      if (trait == null)
        return;
      personality.AddTrait(trait);
    }

    private void SetAttribute(Personality personality, string attribute_name, int value)
    {
      Klei.AI.Attribute attribute = Db.Get().Attributes.TryGet(attribute_name);
      if (attribute == null)
        Debug.LogWarning((object) ("Attribute does not exist: " + attribute_name));
      else
        personality.SetAttribute(attribute, value);
    }

    public List<Personality> GetStartingPersonalities() => this.resources.FindAll((Predicate<Personality>) (x => x.startingMinion));

    public List<Personality> GetAll(bool onlyEnabledMinions, bool onlyStartingMinions) => this.resources.FindAll((Predicate<Personality>) (x => (!onlyStartingMinions || x.startingMinion) && (!onlyEnabledMinions || !x.Disabled)));

    public Personality GetRandom(bool onlyEnabledMinions, bool onlyStartingMinions) => Util.GetRandom<Personality>(this.GetAll(onlyEnabledMinions, onlyStartingMinions));

    public Personality GetPersonalityFromNameStringKey(string name_string_key)
    {
      foreach (Personality resource in Db.Get().Personalities.resources)
      {
        if (resource.nameStringKey.Equals(name_string_key, StringComparison.CurrentCultureIgnoreCase))
          return resource;
      }
      return (Personality) null;
    }

    public class PersonalityLoader : 
      AsyncCsvLoader<Personalities.PersonalityLoader, Personalities.PersonalityInfo>
    {
      public PersonalityLoader()
        : base(Assets.instance.personalitiesFile)
      {
      }

      public virtual void Run() => base.Run();
    }

    public class PersonalityInfo : Resource
    {
      public int HeadShape;
      public int Mouth;
      public int Neck;
      public int Eyes;
      public int Hair;
      public int Body;
      public string Gender;
      public string PersonalityType;
      public string StressTrait;
      public string JoyTrait;
      public string StickerType;
      public string CongenitalTrait;
      public string Design;
      public bool ValidStarter;
    }
  }
}
