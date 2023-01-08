// Decompiled with JetBrains decompiler
// Type: Database.EquippableFacades
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
  public class EquippableFacades : ResourceSet<EquippableFacadeResource>
  {
    public static EquippableFacades.Info[] Infos = new EquippableFacades.Info[12]
    {
      new EquippableFacades.Info("clubshirt", (string) EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.CLUBSHIRT, "CustomClothing", "body_shirt_clubshirt_kanim", "shirt_clubshirt_kanim"),
      new EquippableFacades.Info("cummerbund", (string) EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.CUMMERBUND, "CustomClothing", "body_shirt_cummerbund_kanim", "shirt_cummerbund_kanim"),
      new EquippableFacades.Info("decor_02", (string) EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.DECOR_02, "CustomClothing", "body_shirt_decor02_kanim", "shirt_decor02_kanim"),
      new EquippableFacades.Info("decor_03", (string) EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.DECOR_03, "CustomClothing", "body_shirt_decor03_kanim", "shirt_decor03_kanim"),
      new EquippableFacades.Info("decor_04", (string) EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.DECOR_04, "CustomClothing", "body_shirt_decor04_kanim", "shirt_decor04_kanim"),
      new EquippableFacades.Info("decor_05", (string) EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.DECOR_05, "CustomClothing", "body_shirt_decor05_kanim", "shirt_decor05_kanim"),
      new EquippableFacades.Info("gaudysweater", (string) EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.GAUDYSWEATER, "CustomClothing", "body_shirt_gaudysweater_kanim", "shirt_gaudysweater_kanim"),
      new EquippableFacades.Info("limone", (string) EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.LIMONE, "CustomClothing", "body_suit_limone_kanim", "suit_limone_kanim"),
      new EquippableFacades.Info("mondrian", (string) EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.MONDRIAN, "CustomClothing", "body_shirt_mondrian_kanim", "shirt_mondrian_kanim"),
      new EquippableFacades.Info("overalls", (string) EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.OVERALLS, "CustomClothing", "body_suit_overalls_kanim", "suit_overalls_kanim"),
      new EquippableFacades.Info("triangles", (string) EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.TRIANGLES, "CustomClothing", "body_shirt_triangles_kanim", "shirt_triangles_kanim"),
      new EquippableFacades.Info("workout", (string) EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES.WORKOUT, "CustomClothing", "body_suit_workout_kanim", "suit_workout_kanim")
    };

    public EquippableFacades(ResourceSet parent)
      : base(nameof (EquippableFacades), parent)
    {
      this.Initialize();
      foreach (EquippableFacades.Info info in EquippableFacades.Infos)
        this.Add(info.id, info.name, info.defID, info.buildOverride, info.animFile);
      this.Load();
    }

    public void Load()
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      EquippableFacades.\u003C\u003Ec__DisplayClass3_0 cDisplayClass30 = new EquippableFacades.\u003C\u003Ec__DisplayClass3_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.errors = ListPool<YamlIO.Error, EquippableFacadeResource>.Allocate();
      List<FileHandle> fileHandleList = new List<FileHandle>();
      DirectoryInfo directoryInfo = new DirectoryInfo(FileSystem.Normalize(System.IO.Path.Combine(Db.GetPath("", "equippablefacades"))));
      if (directoryInfo.Exists)
      {
        foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
        {
          fileHandleList.Clear();
          FileSystem.GetFiles(directory.FullName, "*.yaml", (ICollection<FileHandle>) fileHandleList);
          foreach (FileHandle fileHandle in fileHandleList)
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            // ISSUE: method pointer
            EquippableFacadeInfo equippableFacadeInfo = YamlIO.LoadFile<EquippableFacadeInfo>(fileHandle, cDisplayClass30.\u003C\u003E9__0 ?? (cDisplayClass30.\u003C\u003E9__0 = new YamlIO.ErrorHandler((object) cDisplayClass30, __methodptr(\u003CLoad\u003Eb__0))), (List<Tuple<string, System.Type>>) null);
            DebugUtil.DevAssert(string.Equals(directory.Name, equippableFacadeInfo.defID, StringComparison.OrdinalIgnoreCase), "DefID mismatch!", (Object) null);
            if (equippableFacadeInfo.defID != null)
              this.resources.Add(new EquippableFacadeResource(equippableFacadeInfo.id, equippableFacadeInfo.name, equippableFacadeInfo.buildoverride, equippableFacadeInfo.defID, equippableFacadeInfo.animfile));
          }
        }
      }
      this.resources = this.resources.Distinct<EquippableFacadeResource>().ToList<EquippableFacadeResource>();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.errors.Recycle();
    }

    public void Add(string id, string name, string defID, string buildOverride, string animFile) => this.resources.Add(new EquippableFacadeResource(id, name, buildOverride, defID, animFile));

    public struct Info
    {
      public string id;
      public string name;
      public string buildOverride;
      public string defID;
      public string animFile;

      public Info(string id, string name, string defID, string buildOverride, string animFile)
      {
        this.id = id;
        this.name = name;
        this.defID = defID;
        this.buildOverride = buildOverride;
        this.animFile = animFile;
      }
    }
  }
}
