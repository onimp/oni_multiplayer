// Decompiled with JetBrains decompiler
// Type: ModUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KMod;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public static class ModUtil
{
  public static void AddBuildingToPlanScreen(HashedString category, string building_id) => ModUtil.AddBuildingToPlanScreen(category, building_id, "uncategorized");

  public static void AddBuildingToPlanScreen(
    HashedString category,
    string building_id,
    string subcategoryID)
  {
    ModUtil.AddBuildingToPlanScreen(category, building_id, subcategoryID, (string) null);
  }

  public static void AddBuildingToPlanScreen(
    HashedString category,
    string building_id,
    string subcategoryID,
    string relativeBuildingId,
    ModUtil.BuildingOrdering ordering = ModUtil.BuildingOrdering.After)
  {
    int index1 = BUILDINGS.PLANORDER.FindIndex((Predicate<PlanScreen.PlanInfo>) (x => HashedString.op_Equality(x.category, category)));
    if (index1 < 0)
    {
      Debug.LogWarning((object) string.Format("Mod: Unable to add '{0}' as category '{1}' does not exist", (object) building_id, (object) category));
    }
    else
    {
      List<KeyValuePair<string, string>> andSubcategoryData = BUILDINGS.PLANORDER[index1].buildingAndSubcategoryData;
      KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(building_id, subcategoryID);
      if (relativeBuildingId == null)
      {
        andSubcategoryData.Add(keyValuePair);
      }
      else
      {
        int index2 = andSubcategoryData.FindIndex((Predicate<KeyValuePair<string, string>>) (x => x.Key == relativeBuildingId));
        if (index2 == -1)
        {
          andSubcategoryData.Add(keyValuePair);
          Debug.LogWarning((object) ("Mod: Building '" + relativeBuildingId + "' doesn't exist, inserting '" + building_id + "' at the end of the list instead"));
        }
        else
        {
          int index3 = ordering == ModUtil.BuildingOrdering.After ? index2 + 1 : Mathf.Max(index2 - 1, 0);
          andSubcategoryData.Insert(index3, keyValuePair);
        }
      }
    }
  }

  [Obsolete("Use PlanScreen instead")]
  public static void AddBuildingToHotkeyBuildMenu(
    HashedString category,
    string building_id,
    Action hotkey)
  {
    BuildMenu.DisplayInfo info = BuildMenu.OrderedBuildings.GetInfo(category);
    if (HashedString.op_Inequality(info.category, category))
      return;
    (info.data as IList<BuildMenu.BuildingInfo>).Add(new BuildMenu.BuildingInfo(building_id, hotkey));
  }

  public static KAnimFile AddKAnimMod(string name, KAnimFile.Mod anim_mod)
  {
    KAnimFile instance = ScriptableObject.CreateInstance<KAnimFile>();
    instance.mod = anim_mod;
    ((Object) instance).name = name;
    AnimCommandFile animCommandFile = new AnimCommandFile();
    KAnimGroupFile.GroupFile groupFile = new KAnimGroupFile.GroupFile();
    groupFile.groupID = animCommandFile.GetGroupName(instance);
    groupFile.commandDirectory = "assets/" + name;
    animCommandFile.AddGroupFile(groupFile);
    if (KAnimGroupFile.GetGroupFile().AddAnimMod(groupFile, animCommandFile, instance) == null)
      Assets.ModLoadedKAnims.Add(instance);
    return instance;
  }

  public static KAnimFile AddKAnim(
    string name,
    TextAsset anim_file,
    TextAsset build_file,
    IList<Texture2D> textures)
  {
    KAnimFile instance = ScriptableObject.CreateInstance<KAnimFile>();
    instance.Initialize(anim_file, build_file, textures);
    ((Object) instance).name = name;
    AnimCommandFile animCommandFile = new AnimCommandFile();
    KAnimGroupFile.GroupFile groupFile = new KAnimGroupFile.GroupFile();
    groupFile.groupID = animCommandFile.GetGroupName(instance);
    groupFile.commandDirectory = "assets/" + name;
    animCommandFile.AddGroupFile(groupFile);
    KAnimGroupFile.GetGroupFile().AddAnimFile(groupFile, animCommandFile, instance);
    Assets.ModLoadedKAnims.Add(instance);
    return instance;
  }

  public static KAnimFile AddKAnim(
    string name,
    TextAsset anim_file,
    TextAsset build_file,
    Texture2D texture)
  {
    List<Texture2D> textures = new List<Texture2D>();
    textures.Add(texture);
    return ModUtil.AddKAnim(name, anim_file, build_file, (IList<Texture2D>) textures);
  }

  public static Substance CreateSubstance(
    string name,
    Element.State state,
    KAnimFile kanim,
    Material material,
    Color32 colour,
    Color32 ui_colour,
    Color32 conduit_colour)
  {
    return new Substance()
    {
      name = name,
      nameTag = TagManager.Create(name),
      elementID = (SimHashes) Hash.SDBMLower(name),
      anim = kanim,
      colour = colour,
      uiColour = ui_colour,
      conduitColour = conduit_colour,
      material = material,
      renderedByWorld = (state & Element.State.Solid) == Element.State.Solid
    };
  }

  public static void RegisterForTranslation(System.Type locstring_tree_root)
  {
    Localization.RegisterForTranslation(locstring_tree_root);
    Localization.GenerateStringsTemplate(locstring_tree_root, System.IO.Path.Combine(Manager.GetDirectory(), "strings_templates"));
  }

  public enum BuildingOrdering
  {
    Before,
    After,
  }
}
