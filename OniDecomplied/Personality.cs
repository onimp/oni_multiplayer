// Decompiled with JetBrains decompiler
// Type: Personality
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Personality : Resource
{
  public List<Personality.StartingAttribute> attributes = new List<Personality.StartingAttribute>();
  public List<Trait> traits = new List<Trait>();
  public int headShape;
  public int mouth;
  public int neck;
  public int eyes;
  public int hair;
  public int body;
  public Dictionary<ClothingOutfitUtility.OutfitType, string> outfitIds;
  public string nameStringKey;
  public string genderStringKey;
  public string personalityType;
  public string stresstrait;
  public string joyTrait;
  public string stickerType;
  public string congenitaltrait;
  public string unformattedDescription;
  public bool startingMinion;

  public string description => this.GetDescription();

  [Obsolete("Modders: Use constructor with isStartingMinion parameter")]
  public Personality(
    string name_string_key,
    string name,
    string Gender,
    string PersonalityType,
    string StressTrait,
    string JoyTrait,
    string StickerType,
    string CongenitalTrait,
    int headShape,
    int mouth,
    int neck,
    int eyes,
    int hair,
    int body,
    string description)
    : this(name_string_key, name, Gender, PersonalityType, StressTrait, JoyTrait, StickerType, CongenitalTrait, headShape, mouth, neck, eyes, hair, body, description, true)
  {
  }

  public Personality(
    string name_string_key,
    string name,
    string Gender,
    string PersonalityType,
    string StressTrait,
    string JoyTrait,
    string StickerType,
    string CongenitalTrait,
    int headShape,
    int mouth,
    int neck,
    int eyes,
    int hair,
    int body,
    string description,
    bool isStartingMinion)
    : base(name_string_key, name)
  {
    this.nameStringKey = name_string_key;
    this.genderStringKey = Gender;
    this.personalityType = PersonalityType;
    this.stresstrait = StressTrait;
    this.joyTrait = JoyTrait;
    this.stickerType = StickerType;
    this.congenitaltrait = CongenitalTrait;
    this.unformattedDescription = description;
    this.headShape = headShape;
    this.mouth = mouth;
    this.neck = neck;
    this.eyes = eyes;
    this.hair = hair;
    this.body = body;
    this.startingMinion = isStartingMinion;
    this.outfitIds = new Dictionary<ClothingOutfitUtility.OutfitType, string>();
  }

  public string GetDescription()
  {
    this.unformattedDescription = this.unformattedDescription.Replace("{0}", this.Name);
    return this.unformattedDescription;
  }

  public void SetAttribute(Klei.AI.Attribute attribute, int value) => this.attributes.Add(new Personality.StartingAttribute(attribute, value));

  public void AddTrait(Trait trait) => this.traits.Add(trait);

  public void SetOutfit(ClothingOutfitUtility.OutfitType outfitType, Option<string> outfit)
  {
    if (outfit.HasValue)
      this.outfitIds[outfitType] = (string) outfit;
    else
      this.outfitIds.Remove(outfitType);
  }

  public string GetOutfit(ClothingOutfitUtility.OutfitType outfitType) => this.outfitIds.ContainsKey(outfitType) ? this.outfitIds[outfitType] : (string) null;

  public Sprite GetMiniIcon() => string.IsNullOrWhiteSpace(this.nameStringKey) ? Assets.GetSprite(HashedString.op_Implicit("unknown")) : Assets.GetSprite(HashedString.op_Implicit("dreamIcon_" + (!(this.nameStringKey == "MIMA") ? this.nameStringKey[0].ToString() + this.nameStringKey.Substring(1).ToLower() : "Mi-Ma")));

  public class StartingAttribute
  {
    public Klei.AI.Attribute attribute;
    public int value;

    public StartingAttribute(Klei.AI.Attribute attribute, int value)
    {
      this.attribute = attribute;
      this.value = value;
    }
  }
}
