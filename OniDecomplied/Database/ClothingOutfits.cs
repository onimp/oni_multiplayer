// Decompiled with JetBrains decompiler
// Type: Database.ClothingOutfits
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Database
{
  public class ClothingOutfits : ResourceSet<ClothingOutfitResource>
  {
    public ClothingOutfits(ResourceSet parent, ClothingItems items_resource)
      : base(nameof (ClothingOutfits), parent)
    {
      this.Initialize();
      this.Add("BasicBlack", new string[4]
      {
        "TopBasicBlack",
        "BottomBasicBlack",
        "GlovesBasicBlack",
        "ShoesBasicBlack"
      }, UI.OUTFITS.BASIC_BLACK.NAME);
      this.Add("BasicWhite", new string[4]
      {
        "TopBasicWhite",
        "BottomBasicWhite",
        "GlovesBasicWhite",
        "ShoesBasicWhite"
      }, UI.OUTFITS.BASIC_WHITE.NAME);
      this.Add("BasicRed", new string[4]
      {
        "TopBasicRed",
        "BottomBasicRed",
        "GlovesBasicRed",
        "ShoesBasicRed"
      }, UI.OUTFITS.BASIC_RED.NAME);
      this.Add("BasicOrange", new string[4]
      {
        "TopBasicOrange",
        "BottomBasicOrange",
        "GlovesBasicOrange",
        "ShoesBasicOrange"
      }, UI.OUTFITS.BASIC_ORANGE.NAME);
      this.Add("BasicYellow", new string[4]
      {
        "TopBasicYellow",
        "BottomBasicYellow",
        "GlovesBasicYellow",
        "ShoesBasicYellow"
      }, UI.OUTFITS.BASIC_YELLOW.NAME);
      this.Add("BasicGreen", new string[4]
      {
        "TopBasicGreen",
        "BottomBasicGreen",
        "GlovesBasicGreen",
        "ShoesBasicGreen"
      }, UI.OUTFITS.BASIC_GREEN.NAME);
      this.Add("BasicAqua", new string[4]
      {
        "TopBasicAqua",
        "BottomBasicAqua",
        "GlovesBasicAqua",
        "ShoesBasicAqua"
      }, UI.OUTFITS.BASIC_AQUA.NAME);
      this.Add("BasicPurple", new string[4]
      {
        "TopBasicPurple",
        "BottomBasicPurple",
        "GlovesBasicPurple",
        "ShoesBasicPurple"
      }, UI.OUTFITS.BASIC_PURPLE.NAME);
      this.Add("BasicPinkOrchid", new string[4]
      {
        "TopBasicPinkOrchid",
        "BottomBasicPinkOrchid",
        "GlovesBasicPinkOrchid",
        "ShoesBasicPinkOrchid"
      }, UI.OUTFITS.BASIC_PINK_ORCHID.NAME);
      this.Load(items_resource);
      ClothingOutfitUtility.LoadClothingOutfitData(this);
    }

    public void Add(string id, string[] items_in_outfit, LocString name) => this.resources.Add(new ClothingOutfitResource(id, items_in_outfit, name));

    public void Load(ClothingItems items_resource)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      ClothingOutfits.\u003C\u003Ec__DisplayClass3_0 cDisplayClass30 = new ClothingOutfits.\u003C\u003Ec__DisplayClass3_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.errors = ListPool<YamlIO.Error, ClothingItemResource>.Allocate();
      List<FileHandle> fileHandleList = new List<FileHandle>();
      DirectoryInfo directoryInfo = new DirectoryInfo(FileSystem.Normalize(System.IO.Path.Combine(Db.GetPath("", "clothing"))));
      if (directoryInfo.Exists)
      {
        FileSystem.GetFiles(directoryInfo.FullName, "*.yaml", (ICollection<FileHandle>) fileHandleList);
        foreach (FileHandle fileHandle in fileHandleList)
        {
          try
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: method pointer
            ClothingOutfits.ClothingOutfitInfo clothingOutfitInfo = YamlIO.LoadFile<ClothingOutfits.ClothingOutfitInfo>(fileHandle, cDisplayClass30.\u003C\u003E9__0 ?? (cDisplayClass30.\u003C\u003E9__0 = new YamlIO.ErrorHandler((object) cDisplayClass30, __methodptr(\u003CLoad\u003Eb__0))), (List<Tuple<string, System.Type>>) null);
            // ISSUE: reference to a compiler-generated field
            if (((List<YamlIO.Error>) cDisplayClass30.errors).Count == 0)
            {
              string[] items_in_outfit = new string[clothingOutfitInfo.items.Count];
              for (int index = 0; index < clothingOutfitInfo.items.Count; ++index)
              {
                ClothingOutfits.ClothingOutfitInfo.ClothingItem clothingItem = clothingOutfitInfo.items[index];
                if (Object.op_Equality((Object) Assets.GetAnim(HashedString.op_Implicit(clothingItem.animFilename)), (Object) null))
                  Debug.LogError((object) ("missing anim file " + clothingItem.animFilename + " for ClothingItem " + clothingItem.id + " in " + fileHandle.full_path));
                else if (clothingItem.id == null)
                {
                  Debug.LogError((object) ("missing clothing item id in " + fileHandle.full_path));
                }
                else
                {
                  if (items_resource.TryGet(clothingItem.id) == null)
                    items_resource.Add(clothingItem.id, clothingItem.name, clothingItem.description, PermitCategories.GetCategoryForId(clothingItem.category), PermitRarity.Unknown, clothingItem.animFilename);
                  items_in_outfit[index] = clothingItem.id;
                }
              }
              if (clothingOutfitInfo.id != null)
                this.Add(clothingOutfitInfo.id, items_in_outfit, (LocString) (clothingOutfitInfo.id + " (yaml)"));
            }
            else
            {
              // ISSUE: reference to a compiler-generated field
              Debug.LogError((object) ("Failed to load clothing outfit " + fileHandle.full_path + " \n " + ((List<YamlIO.Error>) cDisplayClass30.errors)[0].message));
            }
          }
          catch (Exception ex)
          {
            Debug.LogError((object) string.Format("Failed to load clothing outfit {0} error {1}", (object) fileHandle.full_path, (object) ex));
          }
        }
      }
      this.resources = this.resources.Distinct<ClothingOutfitResource>().ToList<ClothingOutfitResource>();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.errors.Recycle();
    }

    public void SetDuplicantPersonalityOutfit(
      string personalityId,
      Option<string> outfit_id,
      ClothingOutfitUtility.OutfitType outfit_type = ClothingOutfitUtility.OutfitType.Clothing)
    {
      Db.Get().Personalities.Get(personalityId).SetOutfit(outfit_type, outfit_id);
      CustomClothingOutfits.Instance.SetDuplicantPerosonalityOutfit(personalityId, outfit_id, outfit_type);
    }

    public class ClothingOutfitInfo
    {
      public string id { get; set; }

      public string name { get; set; }

      public List<ClothingOutfits.ClothingOutfitInfo.ClothingItem> items { get; set; }

      public class ClothingItem
      {
        public string id { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string category { get; set; }

        public string animFilename { get; set; }
      }
    }
  }
}
