// Decompiled with JetBrains decompiler
// Type: Database.BuildingFacades
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
  public class BuildingFacades : ResourceSet<BuildingFacadeResource>
  {
    public static BuildingFacades.Info[] Infos = new BuildingFacades.Info[26]
    {
      new BuildingFacades.Info("FlowerVase_retro", (string) BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_SUNNY.NAME, (string) BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_SUNNY.DESC, PermitRarity.Nifty, "FlowerVase", "flowervase_retro_kanim"),
      new BuildingFacades.Info("FlowerVase_retro_red", (string) BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BOLD.NAME, (string) BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BOLD.DESC, PermitRarity.Nifty, "FlowerVase", "flowervase_retro_red_kanim"),
      new BuildingFacades.Info("FlowerVase_retro_white", (string) BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_ELEGANT.NAME, (string) BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_ELEGANT.DESC, PermitRarity.Nifty, "FlowerVase", "flowervase_retro_white_kanim"),
      new BuildingFacades.Info("FlowerVase_retro_green", (string) BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BRIGHT.NAME, (string) BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_BRIGHT.DESC, PermitRarity.Nifty, "FlowerVase", "flowervase_retro_green_kanim"),
      new BuildingFacades.Info("FlowerVase_retro_blue", (string) BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_DREAMY.NAME, (string) BUILDINGS.PREFABS.FLOWERVASE.FACADES.RETRO_DREAMY.DESC, PermitRarity.Nifty, "FlowerVase", "flowervase_retro_blue_kanim"),
      new BuildingFacades.Info("LuxuryBed_boat", (string) BUILDINGS.PREFABS.LUXURYBED.FACADES.BOAT.NAME, (string) BUILDINGS.PREFABS.LUXURYBED.FACADES.BOAT.DESC, PermitRarity.Splendid, LuxuryBedConfig.ID, "elegantbed_boat_kanim"),
      new BuildingFacades.Info("LuxuryBed_bouncy", (string) BUILDINGS.PREFABS.LUXURYBED.FACADES.BOUNCY_BED.NAME, (string) BUILDINGS.PREFABS.LUXURYBED.FACADES.BOUNCY_BED.DESC, PermitRarity.Splendid, LuxuryBedConfig.ID, "elegantbed_bouncy_kanim"),
      new BuildingFacades.Info("LuxuryBed_grandprix", (string) BUILDINGS.PREFABS.LUXURYBED.FACADES.GRANDPRIX.NAME, (string) BUILDINGS.PREFABS.LUXURYBED.FACADES.GRANDPRIX.DESC, PermitRarity.Splendid, LuxuryBedConfig.ID, "elegantbed_grandprix_kanim"),
      new BuildingFacades.Info("LuxuryBed_rocket", (string) BUILDINGS.PREFABS.LUXURYBED.FACADES.ROCKET_BED.NAME, (string) BUILDINGS.PREFABS.LUXURYBED.FACADES.ROCKET_BED.DESC, PermitRarity.Splendid, LuxuryBedConfig.ID, "elegantbed_rocket_kanim"),
      new BuildingFacades.Info("LuxuryBed_puft", (string) BUILDINGS.PREFABS.LUXURYBED.FACADES.PUFT_BED.NAME, (string) BUILDINGS.PREFABS.LUXURYBED.FACADES.PUFT_BED.DESC, PermitRarity.Loyalty, LuxuryBedConfig.ID, "elegantbed_puft_kanim"),
      new BuildingFacades.Info("ExteriorWall_pastel_pink", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPINK.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPINK.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_pink_kanim"),
      new BuildingFacades.Info("ExteriorWall_pastel_yellow", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELYELLOW.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELYELLOW.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_yellow_kanim"),
      new BuildingFacades.Info("ExteriorWall_pastel_green", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELGREEN.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELGREEN.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_green_kanim"),
      new BuildingFacades.Info("ExteriorWall_pastel_blue", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELBLUE.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELBLUE.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_blue_kanim"),
      new BuildingFacades.Info("ExteriorWall_pastel_purple", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPURPLE.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPURPLE.DESC, PermitRarity.Common, "ExteriorWall", "walls_pastel_purple_kanim"),
      new BuildingFacades.Info("ExteriorWall_balm_lily", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BALM_LILY.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.BALM_LILY.DESC, PermitRarity.Decent, "ExteriorWall", "walls_balm_lily_kanim"),
      new BuildingFacades.Info("ExteriorWall_clouds", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CLOUDS.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.CLOUDS.DESC, PermitRarity.Decent, "ExteriorWall", "walls_clouds_kanim"),
      new BuildingFacades.Info("ExteriorWall_coffee", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.COFFEE.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.COFFEE.DESC, PermitRarity.Decent, "ExteriorWall", "walls_coffee_kanim"),
      new BuildingFacades.Info("ExteriorWall_mosaic", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.AQUATICMOSAIC.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.AQUATICMOSAIC.DESC, PermitRarity.Decent, "ExteriorWall", "walls_mosaic_kanim"),
      new BuildingFacades.Info("ExteriorWall_mushbar", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.MUSHBAR.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.MUSHBAR.DESC, PermitRarity.Decent, "ExteriorWall", "walls_mushbar_kanim"),
      new BuildingFacades.Info("ExteriorWall_plaid", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLAID.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PLAID.DESC, PermitRarity.Decent, "ExteriorWall", "walls_plaid_kanim"),
      new BuildingFacades.Info("ExteriorWall_rain", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAIN.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAIN.DESC, PermitRarity.Decent, "ExteriorWall", "walls_rain_kanim"),
      new BuildingFacades.Info("ExteriorWall_rainbow", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAINBOW.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.RAINBOW.DESC, PermitRarity.Decent, "ExteriorWall", "walls_rainbow_kanim"),
      new BuildingFacades.Info("ExteriorWall_snow", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SNOW.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SNOW.DESC, PermitRarity.Decent, "ExteriorWall", "walls_snow_kanim"),
      new BuildingFacades.Info("ExteriorWall_sun", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SUN.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.SUN.DESC, PermitRarity.Decent, "ExteriorWall", "walls_sun_kanim"),
      new BuildingFacades.Info("ExteriorWall_polka", (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPOLKA.NAME, (string) BUILDINGS.PREFABS.EXTERIORWALL.FACADES.PASTELPOLKA.DESC, PermitRarity.Decent, "ExteriorWall", "walls_polka_kanim")
    };

    public BuildingFacades(ResourceSet parent)
      : base(nameof (BuildingFacades), parent)
    {
      this.Initialize();
      this.Load();
      foreach (BuildingFacades.Info info in BuildingFacades.Infos)
        this.Add(info.Id, (LocString) info.Name, (LocString) info.Description, info.Rarity, info.PrefabID, info.AnimFile);
    }

    public void Load()
    {
      ListPool<YamlIO.Error, BuildingFacades>.PooledList errors = ListPool<YamlIO.Error, BuildingFacades>.Allocate();
      this.LoadBuildingFacades((List<YamlIO.Error>) errors);
      errors.Recycle();
    }

    private void LoadBuildingFacades(List<YamlIO.Error> errors)
    {
      List<FileHandle> fileHandleList = new List<FileHandle>();
      DirectoryInfo directoryInfo = new DirectoryInfo(FileSystem.Normalize(System.IO.Path.Combine(Db.GetPath("", "buildingfacades"))));
      if (directoryInfo.Exists)
      {
        foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
        {
          fileHandleList.Clear();
          FileSystem.GetFiles(directory.FullName, "*.yaml", (ICollection<FileHandle>) fileHandleList);
          foreach (FileHandle file in fileHandleList)
            this.LoadBuildingFacade(directory.Name, file, errors);
        }
      }
      this.resources = this.resources.Distinct<BuildingFacadeResource>().ToList<BuildingFacadeResource>();
    }

    private void LoadBuildingFacade(string sub_dir, FileHandle file, List<YamlIO.Error> errors)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      BuildingFacades.\u003C\u003Ec__DisplayClass5_0 cDisplayClass50 = new BuildingFacades.\u003C\u003Ec__DisplayClass5_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass50.errors = errors;
      // ISSUE: method pointer
      FacadeInfo facadeInfo = YamlIO.LoadFile<FacadeInfo>(file, new YamlIO.ErrorHandler((object) cDisplayClass50, __methodptr(\u003CLoadBuildingFacade\u003Eb__0)), (List<Tuple<string, System.Type>>) null);
      DebugUtil.DevAssert(string.Equals(sub_dir, facadeInfo.prefabID, StringComparison.OrdinalIgnoreCase), "Mismatched prefab & facade!", (Object) null);
      if (!Assets.TryGetAnim(HashedString.op_Implicit(facadeInfo.animFile), out KAnimFile _))
      {
        Debug.LogWarning((object) ("Building facade on " + facadeInfo.prefabID + " could not be loaded due to missing " + facadeInfo.animFile + " on facade ID '" + facadeInfo.id + "'"));
      }
      else
      {
        bool flag = true;
        if (facadeInfo.workables != null)
        {
          foreach (FacadeInfo.workable workable in facadeInfo.workables)
          {
            if (!Assets.TryGetAnim(HashedString.op_Implicit(workable.workableAnim), out KAnimFile _))
              Debug.LogWarning((object) ("Building facade on " + facadeInfo.prefabID + " could not be loaded due to missing workable anim '" + workable.workableAnim + "' on facade ID '" + facadeInfo.id + "'"));
          }
        }
        if (!flag)
          return;
        // ISSUE: reference to a compiler-generated field
        cDisplayClass50.facade = new BuildingFacadeResource(facadeInfo.id, facadeInfo.name, facadeInfo.description, PermitRarity.Unknown, facadeInfo.prefabID, facadeInfo.animFile, facadeInfo.workables);
        // ISSUE: reference to a compiler-generated method
        if (this.resources.Exists(new Predicate<BuildingFacadeResource>(cDisplayClass50.\u003CLoadBuildingFacade\u003Eb__1)))
        {
          // ISSUE: reference to a compiler-generated field
          Debug.LogWarning((object) ("Building facade on " + facadeInfo.prefabID + " already contains " + cDisplayClass50.facade.Id));
        }
        // ISSUE: reference to a compiler-generated field
        this.resources.Add(cDisplayClass50.facade);
      }
    }

    public void Add(
      string id,
      LocString Name,
      LocString Desc,
      PermitRarity rarity,
      string prefabId,
      string animFile,
      Dictionary<string, string> workables = null)
    {
      this.resources.Add(new BuildingFacadeResource(id, (string) Name, (string) Desc, rarity, prefabId, animFile, workables));
    }

    public void PostProcess()
    {
      foreach (BuildingFacadeResource resource in this.resources)
        resource.Init();
    }

    public struct Info
    {
      public string Id;
      public string Name;
      public string Description;
      public PermitRarity Rarity;
      public string PrefabID;
      public string AnimFile;

      public Info(
        string Id,
        string Name,
        string Description,
        PermitRarity rarity,
        string PrefabID,
        string AnimFile)
      {
        this.Id = Id;
        this.Name = Name;
        this.Description = Description;
        this.Rarity = rarity;
        this.PrefabID = PrefabID;
        this.AnimFile = AnimFile;
      }
    }
  }
}
