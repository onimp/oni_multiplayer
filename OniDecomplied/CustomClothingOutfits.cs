// Decompiled with JetBrains decompiler
// Type: CustomClothingOutfits
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomClothingOutfits
{
  private OutfitData outfitData = new OutfitData();
  private static CustomClothingOutfits _instance;

  public OutfitData OutfitData => this.outfitData;

  public static CustomClothingOutfits Instance
  {
    get
    {
      if (CustomClothingOutfits._instance == null)
        CustomClothingOutfits._instance = new CustomClothingOutfits();
      return CustomClothingOutfits._instance;
    }
  }

  public void EditOutfit(string outfit_name, string[] outfit_items)
  {
    this.outfitData.CustomOutfits[outfit_name] = outfit_items;
    ClothingOutfitUtility.SaveClothingOutfitData();
  }

  public void RenameOutfit(string old_outfit_name, string new_outfit_name)
  {
    if (!this.outfitData.CustomOutfits.ContainsKey(old_outfit_name))
      throw new ArgumentException("Can't rename outfit \"" + old_outfit_name + "\" to \"" + new_outfit_name + "\": missing \"" + old_outfit_name + "\" entry");
    if (this.outfitData.CustomOutfits.ContainsKey(new_outfit_name))
      throw new ArgumentException("Can't rename outfit \"" + old_outfit_name + "\" to \"" + new_outfit_name + "\": entry \"" + new_outfit_name + "\" already exists");
    this.outfitData.CustomOutfits.Add(new_outfit_name, this.outfitData.CustomOutfits[old_outfit_name]);
    foreach (KeyValuePair<string, Dictionary<ClothingOutfitUtility.OutfitType, string>> duplicantOutfit in this.outfitData.DuplicantOutfits)
    {
      string str;
      Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary1;
      Util.Deconstruct<string, Dictionary<ClothingOutfitUtility.OutfitType, string>>(duplicantOutfit, ref str, ref dictionary1);
      string name_string_key = str;
      Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary2 = dictionary1;
      if (dictionary2 != null)
      {
        using (ListPool<ClothingOutfitUtility.OutfitType, CustomClothingOutfits>.PooledList pooledList = PoolsFor<CustomClothingOutfits>.AllocateList<ClothingOutfitUtility.OutfitType>())
        {
          foreach (KeyValuePair<ClothingOutfitUtility.OutfitType, string> keyValuePair in dictionary2)
          {
            ClothingOutfitUtility.OutfitType outfitType1;
            Util.Deconstruct<ClothingOutfitUtility.OutfitType, string>(keyValuePair, ref outfitType1, ref str);
            ClothingOutfitUtility.OutfitType outfitType2 = outfitType1;
            if (str == old_outfit_name)
              ((List<ClothingOutfitUtility.OutfitType>) pooledList).Add(outfitType2);
          }
          foreach (ClothingOutfitUtility.OutfitType outfitType in (List<ClothingOutfitUtility.OutfitType>) pooledList)
          {
            dictionary2[outfitType] = new_outfit_name;
            Personality fromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(name_string_key);
            if (Util.IsNullOrDestroyed((object) fromNameStringKey))
              DebugUtil.DevAssert(false, "<Renaming Outfit Error> Couldn't find personality \"" + name_string_key + "\" to switch their outfit preference from \"" + old_outfit_name + "\" to \"" + new_outfit_name + "\"", (Object) null);
            else
              fromNameStringKey.SetOutfit(outfitType, (Option<string>) new_outfit_name);
          }
        }
      }
    }
    this.outfitData.CustomOutfits.Remove(old_outfit_name);
    ClothingOutfitUtility.SaveClothingOutfitData();
  }

  public void RemoveOutfit(string outfit_name)
  {
    if (!this.outfitData.CustomOutfits.Remove(outfit_name))
      return;
    foreach (KeyValuePair<string, Dictionary<ClothingOutfitUtility.OutfitType, string>> duplicantOutfit in this.outfitData.DuplicantOutfits)
    {
      string str;
      Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary1;
      Util.Deconstruct<string, Dictionary<ClothingOutfitUtility.OutfitType, string>>(duplicantOutfit, ref str, ref dictionary1);
      string name_string_key = str;
      Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary2 = dictionary1;
      if (dictionary2 != null)
      {
        using (ListPool<ClothingOutfitUtility.OutfitType, CustomClothingOutfits>.PooledList pooledList = PoolsFor<CustomClothingOutfits>.AllocateList<ClothingOutfitUtility.OutfitType>())
        {
          foreach (KeyValuePair<ClothingOutfitUtility.OutfitType, string> keyValuePair in dictionary2)
          {
            ClothingOutfitUtility.OutfitType outfitType1;
            Util.Deconstruct<ClothingOutfitUtility.OutfitType, string>(keyValuePair, ref outfitType1, ref str);
            ClothingOutfitUtility.OutfitType outfitType2 = outfitType1;
            if (str == outfit_name)
              ((List<ClothingOutfitUtility.OutfitType>) pooledList).Add(outfitType2);
          }
          foreach (ClothingOutfitUtility.OutfitType outfitType in (List<ClothingOutfitUtility.OutfitType>) pooledList)
          {
            dictionary2.Remove(outfitType);
            Personality fromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(name_string_key);
            if (Util.IsNullOrDestroyed((object) fromNameStringKey))
              DebugUtil.DevAssert(false, "<Deleting Outfit Error> Couldn't find personality \"" + name_string_key + "\" to clear their outfit preference", (Object) null);
            else
              fromNameStringKey.SetOutfit(outfitType, (Option<string>) Option.None);
          }
        }
      }
    }
    ClothingOutfitUtility.SaveClothingOutfitData();
  }

  public void SetDuplicantPerosonalityOutfit(
    string personalityId,
    Option<string> outfit_id,
    ClothingOutfitUtility.OutfitType outfit_type)
  {
    if (outfit_id.HasValue)
    {
      if (!this.outfitData.DuplicantOutfits.ContainsKey(personalityId))
        this.outfitData.DuplicantOutfits.Add(personalityId, new Dictionary<ClothingOutfitUtility.OutfitType, string>());
      this.outfitData.DuplicantOutfits[personalityId][outfit_type] = outfit_id.Value;
    }
    else
    {
      Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary;
      if (this.outfitData.DuplicantOutfits.TryGetValue(personalityId, out dictionary))
      {
        dictionary.Remove(outfit_type);
        if (dictionary.Count == 0)
          this.outfitData.DuplicantOutfits.Remove(personalityId);
      }
    }
    ClothingOutfitUtility.SaveClothingOutfitData();
  }
}
