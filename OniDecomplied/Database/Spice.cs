// Decompiled with JetBrains decompiler
// Type: Database.Spice
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

namespace Database
{
  public class Spice : Resource
  {
    public readonly Spice.Ingredient[] Ingredients;
    public readonly float TotalKG;

    public AttributeModifier StatBonus { get; private set; }

    public AttributeModifier FoodModifier { get; private set; }

    public AttributeModifier CalorieModifier { get; private set; }

    public Color PrimaryColor { get; private set; }

    public Color SecondaryColor { get; private set; }

    public string Image { private set; get; }

    public string[] DlcIds { private set; get; } = DlcManager.AVAILABLE_ALL_VERSIONS;

    public Spice(
      ResourceSet parent,
      string id,
      Spice.Ingredient[] ingredients,
      Color primaryColor,
      Color secondaryColor,
      AttributeModifier foodMod = null,
      AttributeModifier statBonus = null,
      string imageName = "unknown",
      string[] dlcID = null)
      : base(id, parent, (string) null)
    {
      if (dlcID != null)
        this.DlcIds = dlcID;
      this.StatBonus = statBonus;
      this.FoodModifier = foodMod;
      this.Ingredients = ingredients;
      this.Image = imageName;
      this.PrimaryColor = primaryColor;
      this.SecondaryColor = secondaryColor;
      for (int index = 0; index < this.Ingredients.Length; ++index)
        this.TotalKG += this.Ingredients[index].AmountKG;
    }

    public class Ingredient : IConfigurableConsumerIngredient
    {
      public Tag[] IngredientSet;
      public float AmountKG;

      public float GetAmount() => this.AmountKG;

      public Tag[] GetIDSets() => this.IngredientSet;
    }
  }
}
