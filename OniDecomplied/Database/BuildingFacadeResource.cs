// Decompiled with JetBrains decompiler
// Type: Database.BuildingFacadeResource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Database
{
  public class BuildingFacadeResource : PermitResource
  {
    public string Description;
    public string PrefabID;
    public string AnimFile;
    public Dictionary<string, string> InteractFile;

    public BuildingFacadeResource(
      string Id,
      string Name,
      string Description,
      PermitRarity Rarity,
      string PrefabID,
      string AnimFile,
      Dictionary<string, string> workables = null)
      : base(Id, Name, PermitCategory.Building, Rarity)
    {
      this.Id = Id;
      this.Description = Description;
      this.PrefabID = PrefabID;
      this.AnimFile = AnimFile;
      this.InteractFile = workables;
    }

    public BuildingFacadeResource(
      string Id,
      string Name,
      string Description,
      PermitRarity Rarity,
      string PrefabID,
      string AnimFile,
      List<FacadeInfo.workable> workables = null)
      : base(Id, Name, PermitCategory.Building, Rarity)
    {
      this.Id = Id;
      this.Description = Description;
      this.PrefabID = PrefabID;
      this.AnimFile = AnimFile;
      this.InteractFile = new Dictionary<string, string>();
      if (workables == null)
        return;
      foreach (FacadeInfo.workable workable in workables)
        this.InteractFile.Add(workable.workableName, workable.workableAnim);
    }

    public void Init()
    {
      GameObject prefab = Assets.GetPrefab(Tag.op_Implicit(this.PrefabID));
      if (Object.op_Equality((Object) prefab, (Object) null))
      {
        Debug.LogWarning((object) ("Missing prefab id " + this.PrefabID + " for facade " + this.Name));
      }
      else
      {
        prefab.AddOrGet<BuildingFacade>();
        BuildingDef def = prefab.GetComponent<Building>().Def;
        if (!Object.op_Inequality((Object) def, (Object) null))
          return;
        def.AddFacade(this.Id);
      }
    }

    public override PermitPresentationInfo GetPermitPresentationInfo()
    {
      PermitPresentationInfo presentationInfo = new PermitPresentationInfo();
      presentationInfo.name = this.Name;
      presentationInfo.description = this.Description;
      presentationInfo.sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(HashedString.op_Implicit(this.AnimFile)));
      presentationInfo.category = this.PermitCategory;
      presentationInfo.SetFacadeForPrefabID(this.PrefabID);
      presentationInfo.SetRarityDetailsFor(this.Rarity);
      presentationInfo.ownedCount = PermitItems.GetOwnedCount((PermitResource) this);
      return presentationInfo;
    }
  }
}
