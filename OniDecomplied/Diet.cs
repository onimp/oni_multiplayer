// Decompiled with JetBrains decompiler
// Type: Diet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public class Diet
{
  public List<KeyValuePair<Tag, float>> consumedTags;
  public List<KeyValuePair<Tag, float>> producedTags;
  public bool eatsPlantsDirectly;
  private Dictionary<Tag, Diet.Info> consumedTagToInfo = new Dictionary<Tag, Diet.Info>();

  public Diet.Info[] infos { get; private set; }

  public Diet(params Diet.Info[] infos)
  {
    this.infos = infos;
    this.consumedTags = new List<KeyValuePair<Tag, float>>();
    this.producedTags = new List<KeyValuePair<Tag, float>>();
    foreach (Diet.Info info1 in infos)
    {
      Diet.Info info = info1;
      if (info.eatsPlantsDirectly)
        this.eatsPlantsDirectly = true;
      foreach (Tag consumedTag in info.consumedTags)
      {
        Tag tag = consumedTag;
        if (-1 == this.consumedTags.FindIndex((Predicate<KeyValuePair<Tag, float>>) (e => Tag.op_Equality(e.Key, tag))))
          this.consumedTags.Add(new KeyValuePair<Tag, float>(tag, info.caloriesPerKg));
        if (this.consumedTagToInfo.ContainsKey(tag))
          Debug.LogError((object) ("Duplicate diet entry: " + tag.ToString()));
        this.consumedTagToInfo[tag] = info;
      }
      if (Tag.op_Inequality(info.producedElement, Tag.Invalid) && -1 == this.producedTags.FindIndex((Predicate<KeyValuePair<Tag, float>>) (e => Tag.op_Equality(e.Key, info.producedElement))))
        this.producedTags.Add(new KeyValuePair<Tag, float>(info.producedElement, info.producedConversionRate));
    }
  }

  public Diet.Info GetDietInfo(Tag tag)
  {
    Diet.Info dietInfo = (Diet.Info) null;
    this.consumedTagToInfo.TryGetValue(tag, out dietInfo);
    return dietInfo;
  }

  public class Info
  {
    public HashSet<Tag> consumedTags { get; private set; }

    public Tag producedElement { get; private set; }

    public float caloriesPerKg { get; private set; }

    public float producedConversionRate { get; private set; }

    public byte diseaseIdx { get; private set; }

    public float diseasePerKgProduced { get; private set; }

    public bool produceSolidTile { get; private set; }

    public bool eatsPlantsDirectly { get; private set; }

    public Info(
      HashSet<Tag> consumed_tags,
      Tag produced_element,
      float calories_per_kg,
      float produced_conversion_rate = 1f,
      string disease_id = null,
      float disease_per_kg_produced = 0.0f,
      bool produce_solid_tile = false,
      bool eats_plants_directly = false)
    {
      this.consumedTags = consumed_tags;
      this.producedElement = produced_element;
      this.caloriesPerKg = calories_per_kg;
      this.producedConversionRate = produced_conversion_rate;
      this.diseaseIdx = string.IsNullOrEmpty(disease_id) ? byte.MaxValue : Db.Get().Diseases.GetIndex(HashedString.op_Implicit(disease_id));
      this.produceSolidTile = produce_solid_tile;
      this.eatsPlantsDirectly = eats_plants_directly;
    }

    public bool IsMatch(Tag tag) => this.consumedTags.Contains(tag);

    public bool IsMatch(HashSet<Tag> tags)
    {
      if (tags.Count < this.consumedTags.Count)
      {
        foreach (Tag tag in tags)
        {
          if (this.consumedTags.Contains(tag))
            return true;
        }
        return false;
      }
      foreach (Tag consumedTag in this.consumedTags)
      {
        if (tags.Contains(consumedTag))
          return true;
      }
      return false;
    }

    public float ConvertCaloriesToConsumptionMass(float calories) => calories / this.caloriesPerKg;

    public float ConvertConsumptionMassToCalories(float mass) => this.caloriesPerKg * mass;

    public float ConvertConsumptionMassToProducedMass(float consumed_mass) => consumed_mass * this.producedConversionRate;
  }
}
