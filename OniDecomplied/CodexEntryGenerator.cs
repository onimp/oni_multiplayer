// Decompiled with JetBrains decompiler
// Type: CodexEntryGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei;
using Klei.AI;
using ProcGen;
using ProcGenGame;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CodexEntryGenerator
{
  private static string categoryPrefx = "BUILD_CATEGORY_";
  public static readonly string FOOD_CATEGORY_ID = CodexCache.FormatLinkID("FOOD");
  public static readonly string FOOD_EFFECTS_ENTRY_ID = CodexCache.FormatLinkID("id_food_effects");
  public static readonly string TABLE_SALT_ENTRY_ID = CodexCache.FormatLinkID("id_table_salt");
  public static readonly string MINION_MODIFIERS_CATEGORY_ID = CodexCache.FormatLinkID("MINION_MODIFIERS");
  public static Dictionary<Tag, string> room_constraint_to_building_label_dict = new Dictionary<Tag, string>()
  {
    [RoomConstraints.ConstraintTags.IndustrialMachinery] = (string) CODEX.BUILDING_TYPE.INDUSTRIAL_MACHINERY,
    [RoomConstraints.ConstraintTags.RecBuilding] = (string) ROOMS.CRITERIA.REC_BUILDING.NAME,
    [RoomConstraints.ConstraintTags.Clinic] = (string) ROOMS.CRITERIA.CLINIC.NAME,
    [RoomConstraints.ConstraintTags.WashStation] = (string) ROOMS.CRITERIA.WASH_STATION.NAME,
    [RoomConstraints.ConstraintTags.AdvancedWashStation] = (string) ROOMS.CRITERIA.ADVANCED_WASH_STATION.NAME,
    [RoomConstraints.ConstraintTags.ToiletType] = (string) ROOMS.CRITERIA.TOILET.NAME,
    [RoomConstraints.ConstraintTags.FlushToiletType] = (string) ROOMS.CRITERIA.FLUSH_TOILET.NAME,
    [RoomConstraints.ConstraintTags.ScienceBuilding] = (string) ROOMS.CRITERIA.SCIENCE_BUILDING.NAME,
    [GameTags.Decoration] = (string) ROOMS.CRITERIA.DECOR_ITEM_CLASS
  };

  public static Dictionary<string, CodexEntry> GenerateBuildingEntries()
  {
    Dictionary<string, CodexEntry> categoryEntries = new Dictionary<string, CodexEntry>();
    foreach (PlanScreen.PlanInfo category in TUNING.BUILDINGS.PLANORDER)
      CodexEntryGenerator.GenerateEntriesForBuildingsInCategory(category, CodexEntryGenerator.categoryPrefx, ref categoryEntries);
    if (DlcManager.FeatureClusterSpaceEnabled())
      CodexEntryGenerator.GenerateDLC1RocketryEntries();
    CodexEntryGenerator.PopulateCategoryEntries(categoryEntries);
    return categoryEntries;
  }

  private static void GenerateEntriesForBuildingsInCategory(
    PlanScreen.PlanInfo category,
    string categoryPrefx,
    ref Dictionary<string, CodexEntry> categoryEntries)
  {
    string str1 = HashCache.Get().Get(category.category);
    string str2 = CodexCache.FormatLinkID(categoryPrefx + str1);
    Dictionary<string, CodexEntry> entries = new Dictionary<string, CodexEntry>();
    foreach (KeyValuePair<string, string> keyValuePair in category.buildingAndSubcategoryData)
    {
      CodexEntry singleBuildingEntry = CodexEntryGenerator.GenerateSingleBuildingEntry(Assets.GetBuildingDef(keyValuePair.Key), str2);
      if (singleBuildingEntry != null)
        entries.Add(singleBuildingEntry.id, singleBuildingEntry);
    }
    if (entries.Count == 0)
      return;
    CategoryEntry categoryEntry = CodexEntryGenerator.GenerateCategoryEntry(CodexCache.FormatLinkID(str2), StringEntry.op_Implicit(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + str1.ToUpper() + ".NAME")), entries);
    categoryEntry.parentId = "BUILDINGS";
    categoryEntry.category = "BUILDINGS";
    categoryEntry.icon = Assets.GetSprite(HashedString.op_Implicit(PlanScreen.IconNameMap[HashedString.op_Implicit(str1)]));
    categoryEntries.Add(str2, (CodexEntry) categoryEntry);
  }

  private static CodexEntry GenerateSingleBuildingEntry(BuildingDef def, string categoryEntryID)
  {
    if (def.DebugOnly)
      return (CodexEntry) null;
    List<ContentContainer> contentContainerList = new List<ContentContainer>();
    List<ICodexWidget> content = new List<ICodexWidget>();
    content.Add((ICodexWidget) new CodexText(def.Name, CodexTextStyle.Title));
    Tech techForTechItem = Db.Get().Techs.TryGetTechForTechItem(def.PrefabID);
    if (techForTechItem != null)
      content.Add((ICodexWidget) new CodexLabelWithIcon(techForTechItem.Name, CodexTextStyle.Body, new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit("research_type_alpha_icon")), Color.white)));
    content.Add((ICodexWidget) new CodexDividerLine());
    contentContainerList.Add(new ContentContainer(content, ContentContainer.ContentLayout.Vertical));
    CodexEntryGenerator.GenerateImageContainers(def.GetUISprite(), contentContainerList);
    CodexEntryGenerator.GenerateBuildingDescriptionContainers(def, contentContainerList);
    CodexEntryGenerator.GenerateFabricatorContainers(def.BuildingComplete, contentContainerList);
    CodexEntryGenerator.GenerateReceptacleContainers(def.BuildingComplete, contentContainerList);
    CodexEntryGenerator.GenerateConfigurableConsumerContainers(def.BuildingComplete, contentContainerList);
    CodexEntry entry = new CodexEntry(categoryEntryID, contentContainerList, StringEntry.op_Implicit(Strings.Get("STRINGS.BUILDINGS.PREFABS." + def.PrefabID.ToUpper() + ".NAME")));
    entry.icon = def.GetUISprite();
    entry.parentId = categoryEntryID;
    CodexCache.AddEntry(def.PrefabID, entry);
    return entry;
  }

  private static void GenerateDLC1RocketryEntries()
  {
    PlanScreen.PlanInfo planInfo = TUNING.BUILDINGS.PLANORDER.Find((Predicate<PlanScreen.PlanInfo>) (match => HashedString.op_Equality(match.category, new HashedString("Rocketry"))));
    foreach (string prefab_id in SelectModuleSideScreen.moduleButtonSortOrder)
    {
      string str = HashCache.Get().Get(planInfo.category);
      string categoryEntryID = CodexCache.FormatLinkID(CodexEntryGenerator.categoryPrefx + str);
      BuildingDef buildingDef = Assets.GetBuildingDef(prefab_id);
      CodexEntry singleBuildingEntry = CodexEntryGenerator.GenerateSingleBuildingEntry(buildingDef, categoryEntryID);
      List<ICodexWidget> content = new List<ICodexWidget>();
      content.Add((ICodexWidget) new CodexSpacer());
      content.Add((ICodexWidget) new CodexText((string) UI.CLUSTERMAP.ROCKETS.MODULE_STATS.NAME_HEADER, CodexTextStyle.Subtitle));
      content.Add((ICodexWidget) new CodexSpacer());
      content.Add((ICodexWidget) new CodexText((string) UI.CLUSTERMAP.ROCKETS.SPEED.TOOLTIP));
      RocketModuleCluster component1 = buildingDef.BuildingComplete.GetComponent<RocketModuleCluster>();
      float burden = component1.performanceStats.Burden;
      float enginePower = component1.performanceStats.EnginePower;
      RocketEngineCluster component2 = buildingDef.BuildingComplete.GetComponent<RocketEngineCluster>();
      if (Object.op_Inequality((Object) component2, (Object) null))
        content.Add((ICodexWidget) new CodexText("    • " + (string) UI.CLUSTERMAP.ROCKETS.MAX_HEIGHT.NAME_MAX_SUPPORTED + component2.maxHeight.ToString()));
      content.Add((ICodexWidget) new CodexText("    • " + (string) UI.CLUSTERMAP.ROCKETS.MAX_HEIGHT.NAME_RAW + buildingDef.HeightInCells.ToString()));
      if ((double) burden != 0.0)
        content.Add((ICodexWidget) new CodexText("    • " + (string) UI.CLUSTERMAP.ROCKETS.BURDEN_MODULE.NAME + burden.ToString()));
      if ((double) enginePower != 0.0)
        content.Add((ICodexWidget) new CodexText("    • " + (string) UI.CLUSTERMAP.ROCKETS.POWER_MODULE.NAME + enginePower.ToString()));
      ContentContainer container = new ContentContainer(content, ContentContainer.ContentLayout.Vertical);
      singleBuildingEntry.AddContentContainer(container);
    }
  }

  public static void GeneratePageNotFound() => CodexCache.AddEntry("PageNotFound", new CodexEntry("ROOT", new List<ContentContainer>()
  {
    new ContentContainer()
    {
      content = {
        (ICodexWidget) new CodexText((string) CODEX.PAGENOTFOUND.TITLE, CodexTextStyle.Title),
        (ICodexWidget) new CodexText((string) CODEX.PAGENOTFOUND.SUBTITLE, CodexTextStyle.Subtitle),
        (ICodexWidget) new CodexDividerLine(),
        (ICodexWidget) new CodexImage(312, 312, Assets.GetSprite(HashedString.op_Implicit("outhouseMessage")))
      }
    }
  }, (string) CODEX.PAGENOTFOUND.TITLE)
  {
    searchOnly = true
  });

  public static Dictionary<string, CodexEntry> GenerateCreatureEntries()
  {
    Dictionary<string, CodexEntry> results = new Dictionary<string, CodexEntry>();
    List<GameObject> brains = Assets.GetPrefabsWithComponent<CreatureBrain>();
    Action<Tag, string> action = (Action<Tag, string>) ((speciesTag, name) =>
    {
      bool flag = false;
      List<ContentContainer> contentContainers = new List<ContentContainer>();
      CodexEntry entry = new CodexEntry("CREATURES", contentContainers, name);
      foreach (GameObject gameObject in brains)
      {
        if (gameObject.GetDef<BabyMonitor.Def>() == null)
        {
          Sprite sprite = (Sprite) null;
          CreatureBrain component = gameObject.GetComponent<CreatureBrain>();
          if (!Tag.op_Inequality(component.species, speciesTag))
          {
            if (!flag)
            {
              flag = true;
              contentContainers.Add(new ContentContainer(new List<ICodexWidget>()
              {
                (ICodexWidget) new CodexSpacer(),
                (ICodexWidget) new CodexSpacer()
              }, ContentContainer.ContentLayout.Vertical));
              entry.parentId = "CREATURES";
              CodexCache.AddEntry(speciesTag.ToString(), entry);
              results.Add(speciesTag.ToString(), entry);
            }
            List<ContentContainer> contentContainerList = new List<ContentContainer>();
            string symbolPrefix = component.symbolPrefix;
            Sprite first = Def.GetUISprite((object) gameObject, symbolPrefix + "ui").first;
            Tag tag = gameObject.PrefabID();
            GameObject prefab = Assets.TryGetPrefab(Tag.op_Implicit(tag.ToString() + "Baby"));
            if (Object.op_Inequality((Object) prefab, (Object) null))
              sprite = Def.GetUISprite((object) prefab).first;
            if (Object.op_Implicit((Object) sprite))
              CodexEntryGenerator.GenerateImageContainers(new Sprite[2]
              {
                first,
                sprite
              }, contentContainerList, ContentContainer.ContentLayout.Horizontal);
            else
              CodexEntryGenerator.GenerateImageContainers(first, contentContainerList);
            CodexEntryGenerator.GenerateCreatureDescriptionContainers(gameObject, contentContainerList);
            tag = ((Component) component).PrefabID();
            entry.subEntries.Add(new SubEntry(tag.ToString(), speciesTag.ToString(), contentContainerList, ((Component) component).GetProperName())
            {
              icon = first,
              iconColor = Color.white
            });
          }
        }
      }
    });
    action(GameTags.Creatures.Species.PuftSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.PUFTSPECIES);
    action(GameTags.Creatures.Species.PacuSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.PACUSPECIES);
    action(GameTags.Creatures.Species.OilFloaterSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.OILFLOATERSPECIES);
    action(GameTags.Creatures.Species.LightBugSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.LIGHTBUGSPECIES);
    action(GameTags.Creatures.Species.HatchSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.HATCHSPECIES);
    action(GameTags.Creatures.Species.GlomSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.GLOMSPECIES);
    action(GameTags.Creatures.Species.DreckoSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.DRECKOSPECIES);
    action(GameTags.Creatures.Species.MooSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.MOOSPECIES);
    action(GameTags.Creatures.Species.MoleSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.MOLESPECIES);
    action(GameTags.Creatures.Species.SquirrelSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.SQUIRRELSPECIES);
    action(GameTags.Creatures.Species.CrabSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.CRABSPECIES);
    action(GameTags.Robots.Models.ScoutRover, (string) STRINGS.CREATURES.FAMILY_PLURAL.SCOUTROVER);
    action(GameTags.Creatures.Species.StaterpillarSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.STATERPILLARSPECIES);
    action(GameTags.Creatures.Species.BeetaSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.BEETASPECIES);
    action(GameTags.Creatures.Species.DivergentSpecies, (string) STRINGS.CREATURES.FAMILY_PLURAL.DIVERGENTSPECIES);
    action(GameTags.Robots.Models.SweepBot, (string) STRINGS.CREATURES.FAMILY_PLURAL.SWEEPBOT);
    return results;
  }

  public static Dictionary<string, CodexEntry> GenerateRoomsEntries()
  {
    Dictionary<string, CodexEntry> result = new Dictionary<string, CodexEntry>();
    RoomTypes roomTypesData = Db.Get().RoomTypes;
    string parentCategoryName = "ROOMS";
    Action<RoomTypeCategory> action = (Action<RoomTypeCategory>) (roomCategory =>
    {
      bool flag = false;
      CodexEntry entry = new CodexEntry(parentCategoryName, new List<ContentContainer>(), roomCategory.Name);
      for (int index = 0; index < ((ResourceSet) roomTypesData).Count; ++index)
      {
        RoomType roomType = roomTypesData[index];
        if (roomType.category.Id == roomCategory.Id)
        {
          if (!flag)
          {
            flag = true;
            entry.parentId = parentCategoryName;
            entry.name = roomCategory.Name;
            CodexCache.AddEntry(parentCategoryName + roomCategory.Id, entry);
            result.Add(parentCategoryName + roomType.category.Id, entry);
            ContentContainer container = new ContentContainer(new List<ICodexWidget>()
            {
              (ICodexWidget) new CodexImage(312, 312, Assets.GetSprite(HashedString.op_Implicit(roomCategory.icon)))
            }, ContentContainer.ContentLayout.Vertical);
            entry.AddContentContainer(container);
          }
          List<ContentContainer> contentContainerList = new List<ContentContainer>();
          CodexEntryGenerator.GenerateTitleContainers(roomType.Name, contentContainerList);
          CodexEntryGenerator.GenerateRoomTypeDescriptionContainers(roomType, contentContainerList);
          CodexEntryGenerator.GenerateRoomTypeDetailsContainers(roomType, contentContainerList);
          entry.subEntries.Add(new SubEntry(roomType.Id, parentCategoryName + roomType.category.Id, contentContainerList, roomType.Name)
          {
            icon = Assets.GetSprite(HashedString.op_Implicit(roomCategory.icon)),
            iconColor = Color.white
          });
        }
      }
    });
    action(Db.Get().RoomTypeCategories.Agricultural);
    action(Db.Get().RoomTypeCategories.Bathroom);
    action(Db.Get().RoomTypeCategories.Food);
    action(Db.Get().RoomTypeCategories.Hospital);
    action(Db.Get().RoomTypeCategories.Industrial);
    action(Db.Get().RoomTypeCategories.Park);
    action(Db.Get().RoomTypeCategories.Recreation);
    action(Db.Get().RoomTypeCategories.Sleep);
    action(Db.Get().RoomTypeCategories.Science);
    return result;
  }

  public static Dictionary<string, CodexEntry> GeneratePlantEntries()
  {
    Dictionary<string, CodexEntry> plantEntries = new Dictionary<string, CodexEntry>();
    List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Harvestable>();
    prefabsWithComponent.AddRange((IEnumerable<GameObject>) Assets.GetPrefabsWithComponent<WiltCondition>());
    foreach (GameObject gameObject in prefabsWithComponent)
    {
      Dictionary<string, CodexEntry> dictionary1 = plantEntries;
      Tag tag = gameObject.PrefabID();
      string key1 = tag.ToString();
      if (!dictionary1.ContainsKey(key1) && !Object.op_Inequality((Object) gameObject.GetComponent<BudUprootedMonitor>(), (Object) null))
      {
        List<ContentContainer> contentContainerList = new List<ContentContainer>();
        Sprite first = Def.GetUISprite((object) gameObject).first;
        CodexEntryGenerator.GenerateImageContainers(first, contentContainerList);
        CodexEntryGenerator.GeneratePlantDescriptionContainers(gameObject, contentContainerList);
        CodexEntry entry = new CodexEntry("PLANTS", contentContainerList, gameObject.GetProperName());
        entry.parentId = "PLANTS";
        entry.icon = first;
        tag = gameObject.PrefabID();
        CodexCache.AddEntry(tag.ToString(), entry);
        Dictionary<string, CodexEntry> dictionary2 = plantEntries;
        tag = gameObject.PrefabID();
        string key2 = tag.ToString();
        CodexEntry codexEntry = entry;
        dictionary2.Add(key2, codexEntry);
      }
    }
    return plantEntries;
  }

  public static Dictionary<string, CodexEntry> GenerateFoodEntries()
  {
    Dictionary<string, CodexEntry> foodEntries = new Dictionary<string, CodexEntry>();
    foreach (EdiblesManager.FoodInfo allFoodType in EdiblesManager.GetAllFoodTypes())
    {
      GameObject prefab = Assets.GetPrefab(Tag.op_Implicit(allFoodType.Id));
      if (!prefab.HasTag(GameTags.DeprecatedContent) && !prefab.HasTag(GameTags.IncubatableEgg))
      {
        List<ContentContainer> contentContainerList = new List<ContentContainer>();
        CodexEntryGenerator.GenerateTitleContainers(allFoodType.Name, contentContainerList);
        Sprite first = Def.GetUISprite((object) allFoodType.ConsumableId).first;
        CodexEntryGenerator.GenerateImageContainers(first, contentContainerList);
        CodexEntryGenerator.GenerateFoodDescriptionContainers(allFoodType, contentContainerList);
        CodexEntryGenerator.GenerateRecipeContainers(TagExtensions.ToTag(allFoodType.ConsumableId), contentContainerList);
        CodexEntryGenerator.GenerateUsedInRecipeContainers(TagExtensions.ToTag(allFoodType.ConsumableId), contentContainerList);
        CodexEntry entry = new CodexEntry(CodexEntryGenerator.FOOD_CATEGORY_ID, contentContainerList, allFoodType.Name);
        entry.icon = first;
        entry.parentId = CodexEntryGenerator.FOOD_CATEGORY_ID;
        CodexCache.AddEntry(allFoodType.Id, entry);
        foodEntries.Add(allFoodType.Id, entry);
      }
    }
    CodexEntry foodEffectEntry = CodexEntryGenerator.GenerateFoodEffectEntry();
    CodexCache.AddEntry(CodexEntryGenerator.FOOD_EFFECTS_ENTRY_ID, foodEffectEntry);
    foodEntries.Add(CodexEntryGenerator.FOOD_EFFECTS_ENTRY_ID, foodEffectEntry);
    CodexEntry tabelSaltEntry = CodexEntryGenerator.GenerateTabelSaltEntry();
    CodexCache.AddEntry(CodexEntryGenerator.TABLE_SALT_ENTRY_ID, tabelSaltEntry);
    foodEntries.Add(CodexEntryGenerator.TABLE_SALT_ENTRY_ID, tabelSaltEntry);
    return foodEntries;
  }

  private static CodexEntry GenerateFoodEffectEntry()
  {
    List<ICodexWidget> content = new List<ICodexWidget>();
    Dictionary<string, List<EdiblesManager.FoodInfo>> dictionary = new Dictionary<string, List<EdiblesManager.FoodInfo>>();
    foreach (EdiblesManager.FoodInfo allFoodType in EdiblesManager.GetAllFoodTypes())
    {
      foreach (string effect in allFoodType.Effects)
      {
        List<EdiblesManager.FoodInfo> foodInfoList;
        if (!dictionary.TryGetValue(effect, out foodInfoList))
        {
          foodInfoList = new List<EdiblesManager.FoodInfo>();
          dictionary[effect] = foodInfoList;
        }
        foodInfoList.Add(allFoodType);
      }
    }
    foreach (KeyValuePair<string, List<EdiblesManager.FoodInfo>> keyValuePair in dictionary)
    {
      string str1;
      List<EdiblesManager.FoodInfo> foodInfoList1;
      Util.Deconstruct<string, List<EdiblesManager.FoodInfo>>(keyValuePair, ref str1, ref foodInfoList1);
      string str2 = str1;
      List<EdiblesManager.FoodInfo> foodInfoList2 = foodInfoList1;
      Effect effect = Db.Get().effects.Get(str2);
      string text1 = StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + str2.ToUpper() + ".NAME"));
      string text2 = StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + str2.ToUpper() + ".DESCRIPTION"));
      content.Add((ICodexWidget) new CodexText(text1, CodexTextStyle.Title));
      content.Add((ICodexWidget) new CodexText(text2));
      foreach (AttributeModifier selfModifier in effect.SelfModifiers)
      {
        string str3 = StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".NAME"));
        string tooltip = StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".DESC"));
        content.Add((ICodexWidget) new CodexTextWithTooltip("    • " + str3 + ": " + selfModifier.GetFormattedString(), tooltip));
      }
      content.Add((ICodexWidget) new CodexText((string) CODEX.HEADERS.FOODSWITHEFFECT + ": "));
      foreach (EdiblesManager.FoodInfo foodInfo in foodInfoList2)
        content.Add((ICodexWidget) new CodexTextWithTooltip("    • " + foodInfo.Name, foodInfo.Description));
      content.Add((ICodexWidget) new CodexSpacer());
    }
    string foodCategoryId = CodexEntryGenerator.FOOD_CATEGORY_ID;
    List<ContentContainer> contentContainers = new List<ContentContainer>();
    contentContainers.Add(new ContentContainer(content, ContentContainer.ContentLayout.Vertical));
    string foodeffects = (string) CODEX.HEADERS.FOODEFFECTS;
    return new CodexEntry(foodCategoryId, contentContainers, foodeffects)
    {
      parentId = CodexEntryGenerator.FOOD_CATEGORY_ID,
      icon = Assets.GetSprite(HashedString.op_Implicit("icon_category_food"))
    };
  }

  private static CodexEntry GenerateTabelSaltEntry()
  {
    LocString name = ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.NAME;
    LocString desc = ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.DESC;
    Sprite sprite = Assets.GetSprite(HashedString.op_Implicit("ui_food_table_salt"));
    List<ContentContainer> contentContainerList = new List<ContentContainer>();
    CodexEntryGenerator.GenerateImageContainers(sprite, contentContainerList);
    contentContainerList.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexText((string) name, CodexTextStyle.Title),
      (ICodexWidget) new CodexText((string) desc)
    }, ContentContainer.ContentLayout.Vertical));
    return new CodexEntry(CodexEntryGenerator.FOOD_CATEGORY_ID, contentContainerList, (string) name)
    {
      parentId = CodexEntryGenerator.FOOD_CATEGORY_ID,
      icon = sprite
    };
  }

  public static Dictionary<string, CodexEntry> GenerateMinionModifierEntries()
  {
    Dictionary<string, CodexEntry> minionModifierEntries = new Dictionary<string, CodexEntry>();
    foreach (Effect resource in Db.Get().effects.resources)
    {
      if (resource.triggerFloatingText || !resource.showInUI)
      {
        string id = resource.Id;
        string str1 = "AVOID_COLLISIONS_" + id;
        StringEntry stringEntry1;
        StringEntry stringEntry2;
        if (Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + id.ToUpper() + ".NAME", ref stringEntry1) && (Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + id.ToUpper() + ".DESCRIPTION", ref stringEntry2) || Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + id.ToUpper() + ".TOOLTIP", ref stringEntry2)))
        {
          string str2 = stringEntry1.String;
          string str3 = stringEntry2.String;
          List<ContentContainer> contentContainers = new List<ContentContainer>();
          ContentContainer contentContainer = new ContentContainer();
          List<ICodexWidget> content = contentContainer.content;
          content.Add((ICodexWidget) new CodexText(resource.Name, CodexTextStyle.Title));
          content.Add((ICodexWidget) new CodexText(resource.description));
          foreach (AttributeModifier selfModifier in resource.SelfModifiers)
          {
            string str4 = StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".NAME"));
            string tooltip = StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".DESC"));
            content.Add((ICodexWidget) new CodexTextWithTooltip("    • " + str4 + ": " + selfModifier.GetFormattedString(), tooltip));
          }
          content.Add((ICodexWidget) new CodexSpacer());
          contentContainers.Add(contentContainer);
          CodexEntry entry = new CodexEntry(CodexEntryGenerator.MINION_MODIFIERS_CATEGORY_ID, contentContainers, resource.Name);
          entry.icon = Assets.GetSprite(HashedString.op_Implicit(resource.customIcon));
          entry.parentId = CodexEntryGenerator.MINION_MODIFIERS_CATEGORY_ID;
          CodexCache.AddEntry(str1, entry);
          minionModifierEntries.Add(str1, entry);
        }
      }
    }
    return minionModifierEntries;
  }

  public static Dictionary<string, CodexEntry> GenerateTechEntries()
  {
    Dictionary<string, CodexEntry> techEntries = new Dictionary<string, CodexEntry>();
    foreach (Tech resource in Db.Get().Techs.resources)
    {
      List<ContentContainer> contentContainerList = new List<ContentContainer>();
      CodexEntryGenerator.GenerateTitleContainers(resource.Name, contentContainerList);
      CodexEntryGenerator.GenerateTechDescriptionContainers(resource, contentContainerList);
      CodexEntryGenerator.GeneratePrerequisiteTechContainers(resource, contentContainerList);
      CodexEntryGenerator.GenerateUnlockContainers(resource, contentContainerList);
      CodexEntry entry = new CodexEntry("TECH", contentContainerList, resource.Name);
      TechItem techItem = resource.unlockedItems.Count != 0 ? resource.unlockedItems[0] : (TechItem) null;
      entry.icon = techItem == null ? (Sprite) null : techItem.getUISprite("ui", false);
      entry.parentId = "TECH";
      CodexCache.AddEntry(resource.Id, entry);
      techEntries.Add(resource.Id, entry);
    }
    return techEntries;
  }

  public static Dictionary<string, CodexEntry> GenerateRoleEntries()
  {
    Dictionary<string, CodexEntry> roleEntries = new Dictionary<string, CodexEntry>();
    foreach (Skill resource in Db.Get().Skills.resources)
    {
      if (!resource.deprecated)
      {
        List<ContentContainer> contentContainerList = new List<ContentContainer>();
        Sprite sprite = Assets.GetSprite(HashedString.op_Implicit(resource.hat));
        CodexEntryGenerator.GenerateTitleContainers(resource.Name, contentContainerList);
        CodexEntryGenerator.GenerateImageContainers(sprite, contentContainerList);
        CodexEntryGenerator.GenerateGenericDescriptionContainers(resource.description, contentContainerList);
        CodexEntryGenerator.GenerateSkillRequirementsAndPerksContainers(resource, contentContainerList);
        CodexEntryGenerator.GenerateRelatedSkillContainers(resource, contentContainerList);
        CodexEntry entry = new CodexEntry("ROLES", contentContainerList, resource.Name);
        entry.parentId = "ROLES";
        entry.icon = sprite;
        CodexCache.AddEntry(resource.Id, entry);
        roleEntries.Add(resource.Id, entry);
      }
    }
    return roleEntries;
  }

  public static Dictionary<string, CodexEntry> GenerateGeyserEntries()
  {
    Dictionary<string, CodexEntry> geyserEntries = new Dictionary<string, CodexEntry>();
    List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Geyser>();
    if (prefabsWithComponent != null)
    {
      foreach (GameObject go in prefabsWithComponent)
      {
        if (!go.GetComponent<KPrefabID>().HasTag(GameTags.DeprecatedContent))
        {
          List<ContentContainer> contentContainerList = new List<ContentContainer>();
          CodexEntryGenerator.GenerateTitleContainers(go.GetProperName(), contentContainerList);
          Sprite first = Def.GetUISprite((object) go).first;
          CodexEntryGenerator.GenerateImageContainers(first, contentContainerList);
          List<ICodexWidget> content = new List<ICodexWidget>();
          Tag tag = go.PrefabID();
          string upper = tag.ToString().ToUpper();
          string str1 = "GENERICGEYSER_";
          if (upper.StartsWith(str1))
            upper.Remove(0, str1.Length);
          content.Add((ICodexWidget) new CodexText((string) UI.CODEX.GEYSERS.DESC));
          ContentContainer contentContainer = new ContentContainer(content, ContentContainer.ContentLayout.Vertical);
          contentContainerList.Add(contentContainer);
          CodexEntry entry = new CodexEntry("GEYSERS", contentContainerList, go.GetProperName());
          entry.icon = first;
          entry.parentId = "GEYSERS";
          CodexEntry codexEntry = entry;
          tag = go.PrefabID();
          string str2 = tag.ToString();
          codexEntry.id = str2;
          CodexCache.AddEntry(entry.id, entry);
          geyserEntries.Add(entry.id, entry);
        }
      }
    }
    return geyserEntries;
  }

  public static Dictionary<string, CodexEntry> GenerateEquipmentEntries()
  {
    Dictionary<string, CodexEntry> equipmentEntries = new Dictionary<string, CodexEntry>();
    List<GameObject> prefabsWithComponent = Assets.GetPrefabsWithComponent<Equippable>();
    if (prefabsWithComponent != null)
    {
      foreach (GameObject go in prefabsWithComponent)
      {
        bool flag = false;
        Equippable component = go.GetComponent<Equippable>();
        if (component.def.AdditionalTags != null)
        {
          foreach (Tag additionalTag in component.def.AdditionalTags)
          {
            if (Tag.op_Equality(additionalTag, GameTags.DeprecatedContent))
            {
              flag = true;
              break;
            }
          }
        }
        if (!flag && !component.hideInCodex)
        {
          List<ContentContainer> contentContainerList = new List<ContentContainer>();
          CodexEntryGenerator.GenerateTitleContainers(go.GetProperName(), contentContainerList);
          Sprite first = Def.GetUISprite((object) go).first;
          CodexEntryGenerator.GenerateImageContainers(first, contentContainerList);
          List<ICodexWidget> content = new List<ICodexWidget>();
          Tag tag = go.PrefabID();
          content.Add((ICodexWidget) new CodexText(StringEntry.op_Implicit(Strings.Get("STRINGS.EQUIPMENT.PREFABS." + tag.ToString().ToUpper() + ".DESC"))));
          ContentContainer contentContainer = new ContentContainer(content, ContentContainer.ContentLayout.Vertical);
          contentContainerList.Add(contentContainer);
          CodexEntry entry = new CodexEntry("EQUIPMENT", contentContainerList, go.GetProperName());
          entry.icon = first;
          entry.parentId = "EQUIPMENT";
          CodexEntry codexEntry = entry;
          tag = go.PrefabID();
          string str = tag.ToString();
          codexEntry.id = str;
          CodexCache.AddEntry(entry.id, entry);
          equipmentEntries.Add(entry.id, entry);
        }
      }
    }
    return equipmentEntries;
  }

  public static Dictionary<string, CodexEntry> GenerateBiomeEntries()
  {
    Dictionary<string, CodexEntry> biomeEntries = new Dictionary<string, CodexEntry>();
    ListPool<YamlIO.Error, WorldGen>.PooledList world_gen_errors = ListPool<YamlIO.Error, WorldGen>.Allocate();
    string str1 = Application.streamingAssetsPath + "/worldgen/worlds/";
    string str2 = Application.streamingAssetsPath + "/worldgen/biomes/";
    string str3 = Application.streamingAssetsPath + "/worldgen/subworlds/";
    WorldGen.LoadSettings();
    Dictionary<string, List<WeightedSubworldName>> dictionary1 = new Dictionary<string, List<WeightedSubworldName>>();
    foreach (KeyValuePair<string, ClusterLayout> keyValuePair in SettingsCache.clusterLayouts.clusterCache)
    {
      ClusterLayout clusterLayout = keyValuePair.Value;
      string filePath = clusterLayout.filePath;
      foreach (WorldPlacement worldPlacement in clusterLayout.worldPlacements)
      {
        foreach (WeightedSubworldName subworldFile in SettingsCache.worlds.GetWorldData(worldPlacement.world).subworldFiles)
        {
          string str4 = subworldFile.name.Substring(subworldFile.name.LastIndexOf("/"));
          string str5 = subworldFile.name.Substring(0, subworldFile.name.Length - str4.Length);
          string key1 = str5.Substring(str5.LastIndexOf("/") + 1);
          if (!(key1 == "subworlds"))
          {
            if (!dictionary1.ContainsKey(key1))
            {
              Dictionary<string, List<WeightedSubworldName>> dictionary2 = dictionary1;
              string key2 = key1;
              List<WeightedSubworldName> weightedSubworldNameList = new List<WeightedSubworldName>();
              weightedSubworldNameList.Add(subworldFile);
              dictionary2.Add(key2, weightedSubworldNameList);
            }
            else
              dictionary1[key1].Add(subworldFile);
          }
        }
      }
    }
    foreach (KeyValuePair<string, List<WeightedSubworldName>> keyValuePair1 in dictionary1)
    {
      string str6 = CodexCache.FormatLinkID(keyValuePair1.Key);
      Tuple<Sprite, Color> tuple = (Tuple<Sprite, Color>) null;
      string name = StringEntry.op_Implicit(Strings.Get("STRINGS.SUBWORLDS." + str6.ToUpper() + ".NAME"));
      if (name.Contains("MISSING"))
        name = str6 + " (missing string key)";
      List<ContentContainer> contentContainerList = new List<ContentContainer>();
      CodexEntryGenerator.GenerateTitleContainers(name, contentContainerList);
      string str7 = "biomeIcon" + char.ToUpper(str6[0]).ToString() + str6.Substring(1).ToLower();
      Sprite sprite = Assets.GetSprite(HashedString.op_Implicit(str7));
      if (Object.op_Inequality((Object) sprite, (Object) null))
        tuple = new Tuple<Sprite, Color>(sprite, Color.white);
      else
        Debug.LogWarning((object) ("Missing codex biome icon: " + str7));
      string str8 = StringEntry.op_Implicit(Strings.Get("STRINGS.SUBWORLDS." + str6.ToUpper() + ".DESC"));
      string str9 = StringEntry.op_Implicit(Strings.Get("STRINGS.SUBWORLDS." + str6.ToUpper() + ".UTILITY"));
      ContentContainer contentContainer1 = new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexText(string.IsNullOrEmpty(str8) ? "Basic description of the biome." : str8),
        (ICodexWidget) new CodexSpacer(),
        (ICodexWidget) new CodexText(string.IsNullOrEmpty(str9) ? "Description of the biomes utility." : str9),
        (ICodexWidget) new CodexSpacer()
      }, ContentContainer.ContentLayout.Vertical);
      contentContainerList.Add(contentContainer1);
      Dictionary<string, float> source = new Dictionary<string, float>();
      ContentContainer contentContainer2 = new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexSpacer(),
        (ICodexWidget) new CodexText((string) UI.CODEX.SUBWORLDS.ELEMENTS, CodexTextStyle.Subtitle),
        (ICodexWidget) new CodexSpacer()
      }, ContentContainer.ContentLayout.Vertical);
      contentContainerList.Add(contentContainer2);
      ContentContainer contentContainer3 = new ContentContainer();
      contentContainer3.contentLayout = ContentContainer.ContentLayout.Vertical;
      contentContainer3.content = new List<ICodexWidget>();
      contentContainerList.Add(contentContainer3);
      foreach (WeightedSubworldName weightedSubworldName in keyValuePair1.Value)
      {
        SubWorld subworld = SettingsCache.subworlds[weightedSubworldName.name];
        foreach (WeightedBiome biome in SettingsCache.subworlds[weightedSubworldName.name].biomes)
        {
          foreach (ElementGradient elementGradient in (List<ElementGradient>) SettingsCache.biomes.BiomeBackgroundElementBandConfigurations[biome.name])
          {
            if (source.ContainsKey(((Gradient<string>) elementGradient).content))
            {
              source[((Gradient<string>) elementGradient).content] = source[((Gradient<string>) elementGradient).content] + ((Gradient<string>) elementGradient).bandSize;
            }
            else
            {
              if (ElementLoader.FindElementByName(((Gradient<string>) elementGradient).content) == null)
                Debug.LogError((object) ("Biome " + biome.name + " contains non-existent element " + ((Gradient<string>) elementGradient).content));
              source.Add(((Gradient<string>) elementGradient).content, ((Gradient<string>) elementGradient).bandSize);
            }
          }
        }
        foreach (Feature feature in subworld.features)
        {
          foreach (KeyValuePair<string, ElementChoiceGroup<WeightedSimHash>> elementChoiceGroup in SettingsCache.GetCachedFeature(feature.type).ElementChoiceGroups)
          {
            foreach (WeightedSimHash choice in elementChoiceGroup.Value.choices)
            {
              if (source.ContainsKey(choice.element))
                source[choice.element] = source[choice.element] + 1f;
              else
                source.Add(choice.element, 1f);
            }
          }
        }
      }
      foreach (KeyValuePair<string, float> keyValuePair2 in (IEnumerable<KeyValuePair<string, float>>) source.OrderBy<KeyValuePair<string, float>, float>((Func<KeyValuePair<string, float>, float>) (pair => pair.Value)))
      {
        Element elementByName = ElementLoader.FindElementByName(keyValuePair2.Key);
        if (tuple == null)
          tuple = Def.GetUISprite((object) elementByName.substance);
        contentContainer3.content.Add((ICodexWidget) new CodexIndentedLabelWithIcon(elementByName.name, CodexTextStyle.Body, Def.GetUISprite((object) elementByName.substance)));
      }
      List<Tag> tagList1 = new List<Tag>();
      ContentContainer contentContainer4 = new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexSpacer(),
        (ICodexWidget) new CodexText((string) UI.CODEX.SUBWORLDS.PLANTS, CodexTextStyle.Subtitle),
        (ICodexWidget) new CodexSpacer()
      }, ContentContainer.ContentLayout.Vertical);
      contentContainerList.Add(contentContainer4);
      ContentContainer contentContainer5 = new ContentContainer();
      contentContainer5.contentLayout = ContentContainer.ContentLayout.Vertical;
      contentContainer5.content = new List<ICodexWidget>();
      contentContainerList.Add(contentContainer5);
      foreach (WeightedSubworldName weightedSubworldName in keyValuePair1.Value)
      {
        foreach (WeightedBiome biome in SettingsCache.subworlds[weightedSubworldName.name].biomes)
        {
          if (biome.tags != null)
          {
            foreach (string tag in biome.tags)
            {
              if (!tagList1.Contains(Tag.op_Implicit(tag)))
              {
                GameObject prefab = Assets.TryGetPrefab(Tag.op_Implicit(tag));
                if (Object.op_Inequality((Object) prefab, (Object) null) && (Object.op_Inequality((Object) prefab.GetComponent<Harvestable>(), (Object) null) || Object.op_Inequality((Object) prefab.GetComponent<SeedProducer>(), (Object) null)))
                {
                  tagList1.Add(Tag.op_Implicit(tag));
                  contentContainer5.content.Add((ICodexWidget) new CodexIndentedLabelWithIcon(prefab.GetProperName(), CodexTextStyle.Body, Def.GetUISprite((object) prefab)));
                }
              }
            }
          }
        }
        foreach (Feature feature in SettingsCache.subworlds[weightedSubworldName.name].features)
        {
          foreach (MobReference internalMob in SettingsCache.GetCachedFeature(feature.type).internalMobs)
          {
            Tag tag = TagExtensions.ToTag(internalMob.type);
            if (!tagList1.Contains(tag))
            {
              GameObject prefab = Assets.TryGetPrefab(tag);
              if (Object.op_Inequality((Object) prefab, (Object) null) && (Object.op_Inequality((Object) prefab.GetComponent<Harvestable>(), (Object) null) || Object.op_Inequality((Object) prefab.GetComponent<SeedProducer>(), (Object) null)))
              {
                tagList1.Add(tag);
                contentContainer5.content.Add((ICodexWidget) new CodexIndentedLabelWithIcon(prefab.GetProperName(), CodexTextStyle.Body, Def.GetUISprite((object) prefab)));
              }
            }
          }
        }
      }
      if (tagList1.Count == 0)
        contentContainer5.content.Add((ICodexWidget) new CodexIndentedLabelWithIcon((string) UI.CODEX.SUBWORLDS.NONE, CodexTextStyle.Body, new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit("inspectorUI_cannot_build")), Color.red)));
      List<Tag> tagList2 = new List<Tag>();
      ContentContainer contentContainer6 = new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexSpacer(),
        (ICodexWidget) new CodexText((string) UI.CODEX.SUBWORLDS.CRITTERS, CodexTextStyle.Subtitle),
        (ICodexWidget) new CodexSpacer()
      }, ContentContainer.ContentLayout.Vertical);
      contentContainerList.Add(contentContainer6);
      ContentContainer contentContainer7 = new ContentContainer();
      contentContainer7.contentLayout = ContentContainer.ContentLayout.Vertical;
      contentContainer7.content = new List<ICodexWidget>();
      contentContainerList.Add(contentContainer7);
      foreach (WeightedSubworldName weightedSubworldName in keyValuePair1.Value)
      {
        foreach (WeightedBiome biome in SettingsCache.subworlds[weightedSubworldName.name].biomes)
        {
          if (biome.tags != null)
          {
            foreach (string tag in biome.tags)
            {
              if (!tagList2.Contains(Tag.op_Implicit(tag)))
              {
                GameObject prefab = Assets.TryGetPrefab(Tag.op_Implicit(tag));
                if (Object.op_Inequality((Object) prefab, (Object) null) && prefab.HasTag(GameTags.Creature))
                {
                  tagList2.Add(Tag.op_Implicit(tag));
                  contentContainer7.content.Add((ICodexWidget) new CodexIndentedLabelWithIcon(prefab.GetProperName(), CodexTextStyle.Body, Def.GetUISprite((object) prefab)));
                }
              }
            }
          }
        }
        foreach (Feature feature in SettingsCache.subworlds[weightedSubworldName.name].features)
        {
          foreach (MobReference internalMob in SettingsCache.GetCachedFeature(feature.type).internalMobs)
          {
            Tag tag = TagExtensions.ToTag(internalMob.type);
            if (!tagList2.Contains(tag))
            {
              GameObject prefab = Assets.TryGetPrefab(tag);
              if (Object.op_Inequality((Object) prefab, (Object) null) && prefab.HasTag(GameTags.Creature))
              {
                tagList2.Add(tag);
                contentContainer7.content.Add((ICodexWidget) new CodexIndentedLabelWithIcon(prefab.GetProperName(), CodexTextStyle.Body, Def.GetUISprite((object) prefab)));
              }
            }
          }
        }
      }
      if (tagList2.Count == 0)
        contentContainer7.content.Add((ICodexWidget) new CodexIndentedLabelWithIcon((string) UI.CODEX.SUBWORLDS.NONE, CodexTextStyle.Body, new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit("inspectorUI_cannot_build")), Color.red)));
      string str10 = "BIOME" + str6;
      CodexEntry entry = new CodexEntry("BIOMES", contentContainerList, str10);
      entry.name = name;
      entry.parentId = "BIOMES";
      entry.icon = tuple.first;
      entry.iconColor = tuple.second;
      CodexCache.AddEntry(str10, entry);
      biomeEntries.Add(str10, entry);
    }
    if (Application.isPlaying)
    {
      Global.Instance.modManager.HandleErrors((List<YamlIO.Error>) world_gen_errors);
    }
    else
    {
      foreach (YamlIO.Error error in (List<YamlIO.Error>) world_gen_errors)
        YamlIO.LogError(error, false);
    }
    world_gen_errors.Recycle();
    return biomeEntries;
  }

  public static Dictionary<string, CodexEntry> GenerateElementEntries()
  {
    Dictionary<string, CodexEntry> categoryEntries = new Dictionary<string, CodexEntry>();
    Dictionary<string, CodexEntry> entries1 = new Dictionary<string, CodexEntry>();
    Dictionary<string, CodexEntry> entries2 = new Dictionary<string, CodexEntry>();
    Dictionary<string, CodexEntry> entries3 = new Dictionary<string, CodexEntry>();
    Dictionary<string, CodexEntry> entries4 = new Dictionary<string, CodexEntry>();
    string str1 = CodexCache.FormatLinkID("ELEMENTS");
    string str2 = CodexCache.FormatLinkID("ELEMENTS_SOLID");
    string str3 = CodexCache.FormatLinkID("ELEMENTS_LIQUID");
    string str4 = CodexCache.FormatLinkID("ELEMENTS_GAS");
    string str5 = CodexCache.FormatLinkID("ELEMENTS_OTHER");
    string str6 = CodexCache.FormatLinkID("ELEMENTS_CLASSES");
    CodexEntryGenerator.CodexElementMap usedMap1 = new CodexEntryGenerator.CodexElementMap();
    CodexEntryGenerator.CodexElementMap madeMap = new CodexEntryGenerator.CodexElementMap();
    Tag waterTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
    Tag dirtyWaterTag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
    Action<GameObject, CodexEntryGenerator.CodexElementMap, CodexEntryGenerator.CodexElementMap> action1 = (Action<GameObject, CodexEntryGenerator.CodexElementMap, CodexEntryGenerator.CodexElementMap>) ((prefab, usedMap, made) =>
    {
      HashSet<ElementUsage> elementUsageSet1 = new HashSet<ElementUsage>();
      HashSet<ElementUsage> elementUsageSet2 = new HashSet<ElementUsage>();
      EnergyGenerator component1 = prefab.GetComponent<EnergyGenerator>();
      if (Object.op_Implicit((Object) component1))
      {
        foreach (EnergyGenerator.InputItem inputItem in (IEnumerable<EnergyGenerator.InputItem>) component1.formula.inputs ?? Enumerable.Empty<EnergyGenerator.InputItem>())
          elementUsageSet1.Add(new ElementUsage(inputItem.tag, inputItem.consumptionRate, true));
        foreach (EnergyGenerator.OutputItem outputItem in (IEnumerable<EnergyGenerator.OutputItem>) component1.formula.outputs ?? Enumerable.Empty<EnergyGenerator.OutputItem>())
        {
          Tag tag = ElementLoader.FindElementByHash(outputItem.element).tag;
          elementUsageSet2.Add(new ElementUsage(tag, outputItem.creationRate, true));
        }
      }
      foreach (ElementConverter elementConverter in (IEnumerable<ElementConverter>) prefab.GetComponents<ElementConverter>() ?? Enumerable.Empty<ElementConverter>())
      {
        foreach (ElementConverter.ConsumedElement consumedElement in (IEnumerable<ElementConverter.ConsumedElement>) elementConverter.consumedElements ?? Enumerable.Empty<ElementConverter.ConsumedElement>())
          elementUsageSet1.Add(new ElementUsage(consumedElement.Tag, consumedElement.MassConsumptionRate, true));
        foreach (ElementConverter.OutputElement outputElement in (IEnumerable<ElementConverter.OutputElement>) elementConverter.outputElements ?? Enumerable.Empty<ElementConverter.OutputElement>())
        {
          Tag tag = ElementLoader.FindElementByHash(outputElement.elementHash).tag;
          elementUsageSet2.Add(new ElementUsage(tag, outputElement.massGenerationRate, true));
        }
      }
      foreach (ElementConsumer elementConsumer in (IEnumerable<ElementConsumer>) prefab.GetComponents<ElementConsumer>() ?? Enumerable.Empty<ElementConsumer>())
      {
        if (!elementConsumer.storeOnConsume)
        {
          Tag tag = ElementLoader.FindElementByHash(elementConsumer.elementToConsume).tag;
          elementUsageSet1.Add(new ElementUsage(tag, elementConsumer.consumptionRate, true));
        }
      }
      IrrigationMonitor.Def def1 = prefab.GetDef<IrrigationMonitor.Def>();
      if (def1 != null)
      {
        foreach (PlantElementAbsorber.ConsumeInfo consumedElement in def1.consumedElements)
          elementUsageSet1.Add(new ElementUsage(consumedElement.tag, consumedElement.massConsumptionRate, true));
      }
      FertilizationMonitor.Def def2 = prefab.GetDef<FertilizationMonitor.Def>();
      if (def2 != null)
      {
        foreach (PlantElementAbsorber.ConsumeInfo consumedElement in def2.consumedElements)
          elementUsageSet1.Add(new ElementUsage(consumedElement.tag, consumedElement.massConsumptionRate, true));
      }
      FlushToilet component2 = prefab.GetComponent<FlushToilet>();
      if (Object.op_Implicit((Object) component2))
      {
        elementUsageSet1.Add(new ElementUsage(waterTag, component2.massConsumedPerUse, false));
        elementUsageSet2.Add(new ElementUsage(dirtyWaterTag, component2.massEmittedPerUse, false));
      }
      HandSanitizer component3 = prefab.GetComponent<HandSanitizer>();
      if (Object.op_Implicit((Object) component3))
      {
        Tag tag1 = ElementLoader.FindElementByHash(component3.consumedElement).tag;
        elementUsageSet1.Add(new ElementUsage(tag1, component3.massConsumedPerUse, false));
        if (component3.outputElement != SimHashes.Vacuum)
        {
          Tag tag2 = ElementLoader.FindElementByHash(component3.outputElement).tag;
          elementUsageSet2.Add(new ElementUsage(tag2, component3.massConsumedPerUse, false));
        }
      }
      CodexEntryGenerator.ConversionEntry ce = new CodexEntryGenerator.ConversionEntry();
      ce.title = prefab.GetProperName();
      ce.prefab = prefab;
      ce.inSet = elementUsageSet1;
      ce.outSet = elementUsageSet2;
      foreach (ElementUsage elementUsage in elementUsageSet1)
        usedMap.Add(elementUsage.tag, ce);
      foreach (ElementUsage elementUsage in elementUsageSet2)
        madeMap.Add(elementUsage.tag, ce);
    });
    foreach (PlanScreen.PlanInfo planInfo in TUNING.BUILDINGS.PLANORDER)
    {
      foreach (KeyValuePair<string, string> keyValuePair in planInfo.buildingAndSubcategoryData)
      {
        BuildingDef buildingDef = Assets.GetBuildingDef(keyValuePair.Key);
        if (Object.op_Equality((Object) buildingDef, (Object) null))
          Debug.LogError((object) ("Building def for id " + keyValuePair.Key + " is null"));
        if (!buildingDef.Deprecated)
          action1(buildingDef.BuildingComplete, usedMap1, madeMap);
      }
    }
    HashSet<GameObject> gameObjectSet = new HashSet<GameObject>((IEnumerable<GameObject>) Assets.GetPrefabsWithComponent<Harvestable>());
    foreach (GameObject gameObject in Assets.GetPrefabsWithComponent<WiltCondition>())
      gameObjectSet.Add(gameObject);
    foreach (GameObject gameObject in gameObjectSet)
    {
      if (!Object.op_Inequality((Object) gameObject.GetComponent<BudUprootedMonitor>(), (Object) null))
        action1(gameObject, usedMap1, madeMap);
    }
    foreach (GameObject go in Assets.GetPrefabsWithComponent<CreatureBrain>())
    {
      if (go.GetDef<BabyMonitor.Def>() == null)
        action1(go, usedMap1, madeMap);
    }
    foreach (KeyValuePair<Tag, Diet> collectDiet in DietManager.CollectDiets((Tag[]) null))
    {
      GameObject gameObject = Assets.GetPrefab(collectDiet.Key).gameObject;
      if (gameObject.GetDef<BabyMonitor.Def>() == null)
      {
        float num = 0.0f;
        foreach (AttributeModifier selfModifier in Db.Get().traits.Get(gameObject.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers)
        {
          if (selfModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
            num = selfModifier.Value;
        }
        foreach (Diet.Info info in collectDiet.Value.infos)
        {
          float amount1 = -num / info.caloriesPerKg;
          float amount2 = amount1 * info.producedConversionRate;
          foreach (Tag consumedTag in info.consumedTags)
          {
            CodexEntryGenerator.ConversionEntry ce = new CodexEntryGenerator.ConversionEntry()
            {
              title = gameObject.GetProperName(),
              prefab = gameObject,
              inSet = new HashSet<ElementUsage>()
            };
            ce.inSet.Add(new ElementUsage(consumedTag, amount1, true));
            ce.outSet = new HashSet<ElementUsage>();
            ce.outSet.Add(new ElementUsage(info.producedElement, amount2, true));
            usedMap1.Add(consumedTag, ce);
            madeMap.Add(info.producedElement, ce);
          }
        }
      }
    }
    Action<Element, List<ContentContainer>> action2 = (Action<Element, List<ContentContainer>>) ((element, containers) =>
    {
      if (element.highTempTransition != null || element.lowTempTransition != null)
        containers.Add(new ContentContainer(new List<ICodexWidget>()
        {
          (ICodexWidget) new CodexText((string) CODEX.HEADERS.ELEMENTTRANSITIONS, CodexTextStyle.Subtitle),
          (ICodexWidget) new CodexDividerLine()
        }, ContentContainer.ContentLayout.Vertical));
      if (element.highTempTransition != null)
      {
        List<ContentContainer> contentContainerList = containers;
        List<ICodexWidget> content = new List<ICodexWidget>();
        content.Add((ICodexWidget) new CodexImage(32, 32, Def.GetUISprite((object) element.highTempTransition)));
        List<ICodexWidget> codexWidgetList = content;
        string text;
        if (element.highTempTransition == null)
          text = "";
        else
          text = element.highTempTransition.name + " (" + element.highTempTransition.GetStateString() + ")  (" + GameUtil.GetFormattedTemperature(element.highTemp) + ")";
        CodexText codexText = new CodexText(text);
        codexWidgetList.Add((ICodexWidget) codexText);
        ContentContainer contentContainer = new ContentContainer(content, ContentContainer.ContentLayout.Horizontal);
        contentContainerList.Add(contentContainer);
      }
      if (element.lowTempTransition != null)
      {
        List<ContentContainer> contentContainerList = containers;
        List<ICodexWidget> content = new List<ICodexWidget>();
        content.Add((ICodexWidget) new CodexImage(32, 32, Def.GetUISprite((object) element.lowTempTransition)));
        List<ICodexWidget> codexWidgetList = content;
        string text;
        if (element.lowTempTransition == null)
          text = "";
        else
          text = element.lowTempTransition.name + " (" + element.lowTempTransition.GetStateString() + ")  (" + GameUtil.GetFormattedTemperature(element.lowTemp) + ")";
        CodexText codexText = new CodexText(text);
        codexWidgetList.Add((ICodexWidget) codexText);
        ContentContainer contentContainer = new ContentContainer(content, ContentContainer.ContentLayout.Horizontal);
        contentContainerList.Add(contentContainer);
      }
      List<ICodexWidget> content1 = new List<ICodexWidget>();
      List<ICodexWidget> content2 = new List<ICodexWidget>();
      foreach (ComplexRecipe recipe in ComplexRecipeManager.Get().recipes)
      {
        if (((IEnumerable<ComplexRecipe.RecipeElement>) recipe.ingredients).Any<ComplexRecipe.RecipeElement>((Func<ComplexRecipe.RecipeElement, bool>) (i => Tag.op_Equality(i.material, element.tag))))
          content1.Add((ICodexWidget) new CodexRecipePanel(recipe));
        if (((IEnumerable<ComplexRecipe.RecipeElement>) recipe.results).Any<ComplexRecipe.RecipeElement>((Func<ComplexRecipe.RecipeElement, bool>) (i => Tag.op_Equality(i.material, element.tag))))
          content2.Add((ICodexWidget) new CodexRecipePanel(recipe));
      }
      List<CodexEntryGenerator.ConversionEntry> conversionEntryList1;
      if (usedMap1.map.TryGetValue(element.tag, out conversionEntryList1))
      {
        foreach (CodexEntryGenerator.ConversionEntry conversionEntry in conversionEntryList1)
          content1.Add((ICodexWidget) new CodexConversionPanel(conversionEntry.title, conversionEntry.inSet.ToArray<ElementUsage>(), conversionEntry.outSet.ToArray<ElementUsage>(), conversionEntry.prefab));
      }
      List<CodexEntryGenerator.ConversionEntry> conversionEntryList2;
      if (madeMap.map.TryGetValue(element.tag, out conversionEntryList2))
      {
        foreach (CodexEntryGenerator.ConversionEntry conversionEntry in conversionEntryList2)
          content2.Add((ICodexWidget) new CodexConversionPanel(conversionEntry.title, conversionEntry.inSet.ToArray<ElementUsage>(), conversionEntry.outSet.ToArray<ElementUsage>(), conversionEntry.prefab));
      }
      ContentContainer contents1 = new ContentContainer(content1, ContentContainer.ContentLayout.Vertical);
      ContentContainer contents2 = new ContentContainer(content2, ContentContainer.ContentLayout.Vertical);
      if (content1.Count > 0)
      {
        containers.Add(new ContentContainer(new List<ICodexWidget>()
        {
          (ICodexWidget) new CodexSpacer(),
          (ICodexWidget) new CodexCollapsibleHeader((string) CODEX.HEADERS.ELEMENTCONSUMEDBY, contents1)
        }, ContentContainer.ContentLayout.Vertical));
        containers.Add(contents1);
      }
      if (content2.Count > 0)
      {
        containers.Add(new ContentContainer(new List<ICodexWidget>()
        {
          (ICodexWidget) new CodexSpacer(),
          (ICodexWidget) new CodexCollapsibleHeader((string) CODEX.HEADERS.ELEMENTPRODUCEDBY, contents2)
        }, ContentContainer.ContentLayout.Vertical));
        containers.Add(contents2);
      }
      containers.Add(new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexSpacer(),
        (ICodexWidget) new CodexText(element.FullDescription()),
        (ICodexWidget) new CodexSpacer()
      }, ContentContainer.ContentLayout.Vertical));
    });
    foreach (Element element in ElementLoader.elements)
    {
      if (!element.disabled)
      {
        List<ContentContainer> contentContainerList = new List<ContentContainer>();
        string name = element.name + " (" + element.GetStateString() + ")";
        Tuple<Sprite, Color> tuple = Def.GetUISprite((object) element);
        if (Object.op_Equality((Object) tuple.first, (Object) null))
        {
          if (element.id == SimHashes.Void)
          {
            name = element.name;
            tuple = new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit("ui_elements-void")), Color.white);
          }
          else if (element.id == SimHashes.Vacuum)
          {
            name = element.name;
            tuple = new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit("ui_elements-vacuum")), Color.white);
          }
        }
        CodexEntryGenerator.GenerateTitleContainers(name, contentContainerList);
        CodexEntryGenerator.GenerateImageContainers(new Tuple<Sprite, Color>[1]
        {
          tuple
        }, contentContainerList, ContentContainer.ContentLayout.Horizontal);
        action2(element, contentContainerList);
        string str7 = element.id.ToString();
        string category;
        Dictionary<string, CodexEntry> dictionary;
        if (element.IsSolid)
        {
          category = str2;
          dictionary = entries1;
        }
        else if (element.IsLiquid)
        {
          category = str3;
          dictionary = entries2;
        }
        else if (element.IsGas)
        {
          category = str4;
          dictionary = entries3;
        }
        else
        {
          category = str5;
          dictionary = entries4;
        }
        CodexEntry entry = new CodexEntry(category, contentContainerList, name);
        entry.parentId = category;
        entry.icon = tuple.first;
        entry.iconColor = tuple.second;
        CodexCache.AddEntry(str7, entry);
        dictionary.Add(str7, entry);
      }
    }
    string str8 = str2;
    CodexEntry categoryEntry1 = (CodexEntry) CodexEntryGenerator.GenerateCategoryEntry(str8, (string) UI.CODEX.CATEGORYNAMES.ELEMENTSSOLID, entries1, Assets.GetSprite(HashedString.op_Implicit("ui_elements-solid")));
    categoryEntry1.parentId = str1;
    categoryEntry1.category = str1;
    categoryEntries.Add(str8, categoryEntry1);
    string str9 = str3;
    CodexEntry categoryEntry2 = (CodexEntry) CodexEntryGenerator.GenerateCategoryEntry(str9, (string) UI.CODEX.CATEGORYNAMES.ELEMENTSLIQUID, entries2, Assets.GetSprite(HashedString.op_Implicit("ui_elements-liquids")));
    categoryEntry2.parentId = str1;
    categoryEntry2.category = str1;
    categoryEntries.Add(str9, categoryEntry2);
    string str10 = str4;
    CodexEntry categoryEntry3 = (CodexEntry) CodexEntryGenerator.GenerateCategoryEntry(str10, (string) UI.CODEX.CATEGORYNAMES.ELEMENTSGAS, entries3, Assets.GetSprite(HashedString.op_Implicit("ui_elements-gases")));
    categoryEntry3.parentId = str1;
    categoryEntry3.category = str1;
    categoryEntries.Add(str10, categoryEntry3);
    string str11 = str5;
    CodexEntry categoryEntry4 = (CodexEntry) CodexEntryGenerator.GenerateCategoryEntry(str11, (string) UI.CODEX.CATEGORYNAMES.ELEMENTSOTHER, entries4, Assets.GetSprite(HashedString.op_Implicit("ui_elements-other")));
    categoryEntry4.parentId = str1;
    categoryEntry4.category = str1;
    categoryEntries.Add(str11, categoryEntry4);
    Sprite sprite = Assets.GetSprite(HashedString.op_Implicit("ui_elements_classes"));
    \u003C\u003Ef__AnonymousType3<Tag, bool, bool, string>[] dataArray = new \u003C\u003Ef__AnonymousType3<Tag, bool, bool, string>[5]
    {
      new
      {
        tag = GameTags.IceOre,
        checkPrefabs = false,
        solidOnly = false,
        spriteName = "ui_ice"
      },
      new
      {
        tag = GameTags.RefinedMetal,
        checkPrefabs = false,
        solidOnly = true,
        spriteName = "ui_refined_metal"
      },
      new
      {
        tag = GameTags.Filter,
        checkPrefabs = false,
        solidOnly = false,
        spriteName = "ui_filtration_medium"
      },
      new
      {
        tag = GameTags.Compostable,
        checkPrefabs = true,
        solidOnly = false,
        spriteName = "ui_compostable"
      },
      new
      {
        tag = GameTags.CombustibleLiquid,
        checkPrefabs = false,
        solidOnly = false,
        spriteName = "ui_combustible_liquids"
      }
    };
    Dictionary<string, CodexEntry> entries5 = new Dictionary<string, CodexEntry>();
    foreach (var data in dataArray)
    {
      string str12 = data.tag.ToString();
      string name = StringEntry.op_Implicit(Strings.Get("STRINGS.MISC.TAGS." + str12.ToUpper()));
      List<ContentContainer> contentContainerList = new List<ContentContainer>();
      CodexEntryGenerator.GenerateTitleContainers(name, contentContainerList);
      contentContainerList.Add(new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexSpacer(),
        (ICodexWidget) new CodexText(StringEntry.op_Implicit(Strings.Get("STRINGS.MISC.TAGS." + str12.ToUpper() + "_DESC"))),
        (ICodexWidget) new CodexSpacer()
      }, ContentContainer.ContentLayout.Vertical));
      List<ICodexWidget> content = new List<ICodexWidget>();
      if (data.checkPrefabs)
      {
        foreach (GameObject go in Assets.GetPrefabsWithTag(data.tag))
        {
          if (!go.HasTag(GameTags.DeprecatedContent))
            content.Add((ICodexWidget) new CodexIndentedLabelWithIcon(go.GetProperName(), CodexTextStyle.Body, Def.GetUISprite((object) go)));
        }
      }
      else
      {
        foreach (Element element in ElementLoader.elements)
        {
          if (!element.disabled && (!data.solidOnly || element.IsSolid))
          {
            bool flag = Tag.op_Equality(element.materialCategory, data.tag);
            foreach (Tag oreTag in element.oreTags)
            {
              if (Tag.op_Equality(oreTag, data.tag))
              {
                flag = true;
                break;
              }
            }
            if (flag)
              content.Add((ICodexWidget) new CodexIndentedLabelWithIcon(element.name, CodexTextStyle.Body, Def.GetUISprite((object) element.substance)));
          }
        }
      }
      contentContainerList.Add(new ContentContainer(content, ContentContainer.ContentLayout.GridTwoColumn));
      CodexEntry entry = new CodexEntry(str6, contentContainerList, name);
      entry.parentId = str6;
      entry.icon = Assets.GetSprite(HashedString.op_Implicit(data.spriteName));
      CodexCache.AddEntry(CodexCache.FormatLinkID(str12), entry);
      entries5.Add(str12, entry);
    }
    CodexEntry categoryEntry5 = (CodexEntry) CodexEntryGenerator.GenerateCategoryEntry(str6, (string) UI.CODEX.CATEGORYNAMES.ELEMENTSCLASSES, entries5, sprite);
    categoryEntry5.parentId = str1;
    categoryEntry5.category = str1;
    categoryEntries.Add(str6, categoryEntry5);
    CodexEntryGenerator.PopulateCategoryEntries(categoryEntries);
    return categoryEntries;
  }

  public static Dictionary<string, CodexEntry> GenerateDiseaseEntries()
  {
    Dictionary<string, CodexEntry> diseaseEntries = new Dictionary<string, CodexEntry>();
    foreach (Klei.AI.Disease resource in Db.Get().Diseases.resources)
    {
      if (!resource.Disabled)
      {
        List<ContentContainer> contentContainerList = new List<ContentContainer>();
        CodexEntryGenerator.GenerateTitleContainers(resource.Name, contentContainerList);
        CodexEntryGenerator.GenerateDiseaseDescriptionContainers(resource, contentContainerList);
        CodexEntry entry = new CodexEntry("DISEASE", contentContainerList, resource.Name);
        entry.parentId = "DISEASE";
        diseaseEntries.Add(resource.Id, entry);
        entry.icon = Assets.GetSprite(HashedString.op_Implicit("overlay_disease"));
        CodexCache.AddEntry(resource.Id, entry);
      }
    }
    return diseaseEntries;
  }

  public static CategoryEntry GenerateCategoryEntry(
    string id,
    string name,
    Dictionary<string, CodexEntry> entries,
    Sprite icon = null,
    bool largeFormat = true,
    bool sort = true,
    string overrideHeader = null)
  {
    List<ContentContainer> contentContainerList = new List<ContentContainer>();
    CodexEntryGenerator.GenerateTitleContainers(overrideHeader == null ? name : overrideHeader, contentContainerList);
    List<CodexEntry> entriesInCategory = new List<CodexEntry>();
    foreach (KeyValuePair<string, CodexEntry> entry in entries)
    {
      entriesInCategory.Add(entry.Value);
      if (Object.op_Equality((Object) icon, (Object) null))
        icon = entry.Value.icon;
    }
    CategoryEntry entry1 = new CategoryEntry("Root", contentContainerList, name, entriesInCategory, largeFormat, sort);
    entry1.icon = icon;
    CodexCache.AddEntry(id, (CodexEntry) entry1);
    return entry1;
  }

  public static Dictionary<string, CodexEntry> GenerateTutorialNotificationEntries()
  {
    CodexEntry entry1 = new CodexEntry("MISCELLANEOUSTIPS", new List<ContentContainer>()
    {
      new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexSpacer()
      }, ContentContainer.ContentLayout.Vertical)
    }, StringEntry.op_Implicit(Strings.Get("STRINGS.UI.CODEX.CATEGORYNAMES.MISCELLANEOUSTIPS")));
    Dictionary<string, CodexEntry> notificationEntries = new Dictionary<string, CodexEntry>();
    for (int tm = 0; tm < 20; ++tm)
    {
      TutorialMessage tutorialMessage = (TutorialMessage) Tutorial.Instance.TutorialMessage((Tutorial.TutorialMessages) tm, false);
      if (tutorialMessage != null && DlcManager.IsDlcListValidForCurrentContent(tutorialMessage.DLCIDs))
      {
        if (!string.IsNullOrEmpty(tutorialMessage.videoClipId))
        {
          List<ContentContainer> contentContainerList = new List<ContentContainer>();
          CodexEntryGenerator.GenerateTitleContainers(tutorialMessage.GetTitle(), contentContainerList);
          contentContainerList.Add(new ContentContainer(new List<ICodexWidget>()
          {
            (ICodexWidget) new CodexVideo()
            {
              videoName = tutorialMessage.videoClipId,
              overlayName = tutorialMessage.videoOverlayName,
              overlayTexts = new List<string>()
              {
                tutorialMessage.videoTitleText,
                (string) VIDEOS.TUTORIAL_HEADER
              }
            }
          }, ContentContainer.ContentLayout.Vertical));
          contentContainerList.Add(new ContentContainer(new List<ICodexWidget>()
          {
            (ICodexWidget) new CodexText(tutorialMessage.GetMessageBody(), id: tutorialMessage.GetTitle())
          }, ContentContainer.ContentLayout.Vertical));
          CodexEntry entry2 = new CodexEntry("Videos", contentContainerList, UI.FormatAsLink(tutorialMessage.GetTitle(), "videos_" + tm.ToString()));
          entry2.icon = Assets.GetSprite(HashedString.op_Implicit("codexVideo"));
          CodexCache.AddEntry("videos_" + tm.ToString(), entry2);
          notificationEntries.Add(entry2.id, entry2);
        }
        else
        {
          List<ContentContainer> contentContainerList = new List<ContentContainer>();
          CodexEntryGenerator.GenerateTitleContainers(tutorialMessage.GetTitle(), contentContainerList);
          contentContainerList.Add(new ContentContainer(new List<ICodexWidget>()
          {
            (ICodexWidget) new CodexText(tutorialMessage.GetMessageBody(), id: tutorialMessage.GetTitle())
          }, ContentContainer.ContentLayout.Vertical));
          contentContainerList.Add(new ContentContainer(new List<ICodexWidget>()
          {
            (ICodexWidget) new CodexSpacer(),
            (ICodexWidget) new CodexSpacer()
          }, ContentContainer.ContentLayout.Vertical));
          SubEntry subEntry = new SubEntry("MISCELLANEOUSTIPS" + tm.ToString(), "MISCELLANEOUSTIPS", contentContainerList, tutorialMessage.GetTitle());
          entry1.subEntries.Add(subEntry);
        }
      }
    }
    CodexCache.AddEntry("MISCELLANEOUSTIPS", entry1);
    return notificationEntries;
  }

  public static void PopulateCategoryEntries(Dictionary<string, CodexEntry> categoryEntries)
  {
    List<CategoryEntry> categoryEntries1 = new List<CategoryEntry>();
    foreach (KeyValuePair<string, CodexEntry> categoryEntry in categoryEntries)
      categoryEntries1.Add(categoryEntry.Value as CategoryEntry);
    CodexEntryGenerator.PopulateCategoryEntries(categoryEntries1);
  }

  public static void PopulateCategoryEntries(
    List<CategoryEntry> categoryEntries,
    Comparison<CodexEntry> comparison = null)
  {
    foreach (CategoryEntry categoryEntry in categoryEntries)
    {
      List<ContentContainer> contentContainers = categoryEntry.contentContainers;
      List<CodexEntry> codexEntryList = new List<CodexEntry>();
      foreach (CodexEntry codexEntry in categoryEntry.entriesInCategory)
        codexEntryList.Add(codexEntry);
      if (categoryEntry.sort)
      {
        if (comparison == null)
          codexEntryList.Sort((Comparison<CodexEntry>) ((a, b) => UI.StripLinkFormatting(a.name).CompareTo(UI.StripLinkFormatting(b.name))));
        else
          codexEntryList.Sort(comparison);
      }
      if (categoryEntry.largeFormat)
      {
        ContentContainer contentContainer1 = new ContentContainer(new List<ICodexWidget>(), ContentContainer.ContentLayout.Grid);
        foreach (CodexEntry codexEntry in codexEntryList)
          contentContainer1.content.Add((ICodexWidget) new CodexLabelWithLargeIcon(codexEntry.name, CodexTextStyle.BodyWhite, new Tuple<Sprite, Color>(Object.op_Inequality((Object) codexEntry.icon, (Object) null) ? codexEntry.icon : Assets.GetSprite(HashedString.op_Implicit("unknown")), codexEntry.iconColor), codexEntry.id));
        if (categoryEntry.showBeforeGeneratedCategoryLinks)
        {
          contentContainers.Add(contentContainer1);
        }
        else
        {
          ContentContainer contentContainer2 = contentContainers[contentContainers.Count - 1];
          contentContainers.RemoveAt(contentContainers.Count - 1);
          contentContainers.Insert(0, contentContainer2);
          contentContainers.Insert(1, contentContainer1);
          contentContainers.Insert(2, new ContentContainer(new List<ICodexWidget>()
          {
            (ICodexWidget) new CodexSpacer()
          }, ContentContainer.ContentLayout.Vertical));
        }
      }
      else
      {
        ContentContainer contentContainer3 = new ContentContainer(new List<ICodexWidget>(), ContentContainer.ContentLayout.Vertical);
        foreach (CodexEntry codexEntry in codexEntryList)
        {
          if (Object.op_Equality((Object) codexEntry.icon, (Object) null))
            contentContainer3.content.Add((ICodexWidget) new CodexText(codexEntry.name));
          else
            contentContainer3.content.Add((ICodexWidget) new CodexLabelWithIcon(codexEntry.name, CodexTextStyle.Body, new Tuple<Sprite, Color>(codexEntry.icon, codexEntry.iconColor), 64, 48));
        }
        if (categoryEntry.showBeforeGeneratedCategoryLinks)
        {
          contentContainers.Add(contentContainer3);
        }
        else
        {
          ContentContainer contentContainer4 = contentContainers[contentContainers.Count - 1];
          contentContainers.RemoveAt(contentContainers.Count - 1);
          contentContainers.Insert(0, contentContainer4);
          contentContainers.Insert(1, contentContainer3);
        }
      }
    }
  }

  private static void GenerateTitleContainers(string name, List<ContentContainer> containers) => containers.Add(new ContentContainer(new List<ICodexWidget>()
  {
    (ICodexWidget) new CodexText(name, CodexTextStyle.Title),
    (ICodexWidget) new CodexDividerLine()
  }, ContentContainer.ContentLayout.Vertical));

  private static void GeneratePrerequisiteTechContainers(
    Tech tech,
    List<ContentContainer> containers)
  {
    if (tech.requiredTech == null || tech.requiredTech.Count == 0)
      return;
    List<ICodexWidget> content = new List<ICodexWidget>();
    content.Add((ICodexWidget) new CodexText((string) CODEX.HEADERS.PREREQUISITE_TECH, CodexTextStyle.Subtitle));
    content.Add((ICodexWidget) new CodexDividerLine());
    content.Add((ICodexWidget) new CodexSpacer());
    foreach (Tech tech1 in tech.requiredTech)
      content.Add((ICodexWidget) new CodexText(tech1.Name));
    content.Add((ICodexWidget) new CodexSpacer());
    containers.Add(new ContentContainer(content, ContentContainer.ContentLayout.Vertical));
  }

  private static void GenerateSkillRequirementsAndPerksContainers(
    Skill skill,
    List<ContentContainer> containers)
  {
    List<ICodexWidget> content = new List<ICodexWidget>();
    CodexText codexText1 = new CodexText((string) CODEX.HEADERS.ROLE_PERKS, CodexTextStyle.Subtitle);
    CodexText codexText2 = new CodexText((string) CODEX.HEADERS.ROLE_PERKS_DESC);
    content.Add((ICodexWidget) codexText1);
    content.Add((ICodexWidget) new CodexDividerLine());
    content.Add((ICodexWidget) codexText2);
    content.Add((ICodexWidget) new CodexSpacer());
    foreach (Resource perk in skill.perks)
    {
      CodexText codexText3 = new CodexText(perk.Name);
      content.Add((ICodexWidget) codexText3);
    }
    containers.Add(new ContentContainer(content, ContentContainer.ContentLayout.Vertical));
    content.Add((ICodexWidget) new CodexSpacer());
  }

  private static void GenerateRelatedSkillContainers(Skill skill, List<ContentContainer> containers)
  {
    bool flag1 = false;
    List<ICodexWidget> content1 = new List<ICodexWidget>();
    content1.Add((ICodexWidget) new CodexText((string) CODEX.HEADERS.PREREQUISITE_ROLES, CodexTextStyle.Subtitle));
    content1.Add((ICodexWidget) new CodexDividerLine());
    content1.Add((ICodexWidget) new CodexSpacer());
    foreach (string priorSkill in skill.priorSkills)
    {
      CodexText codexText = new CodexText(Db.Get().Skills.Get(priorSkill).Name);
      content1.Add((ICodexWidget) codexText);
      flag1 = true;
    }
    if (flag1)
    {
      content1.Add((ICodexWidget) new CodexSpacer());
      containers.Add(new ContentContainer(content1, ContentContainer.ContentLayout.Vertical));
    }
    bool flag2 = false;
    List<ICodexWidget> content2 = new List<ICodexWidget>();
    CodexText codexText1 = new CodexText((string) CODEX.HEADERS.UNLOCK_ROLES, CodexTextStyle.Subtitle);
    CodexText codexText2 = new CodexText((string) CODEX.HEADERS.UNLOCK_ROLES_DESC);
    content2.Add((ICodexWidget) codexText1);
    content2.Add((ICodexWidget) new CodexDividerLine());
    content2.Add((ICodexWidget) codexText2);
    content2.Add((ICodexWidget) new CodexSpacer());
    foreach (Skill resource in Db.Get().Skills.resources)
    {
      if (!resource.deprecated)
      {
        foreach (string priorSkill in resource.priorSkills)
        {
          if (priorSkill == skill.Id)
          {
            CodexText codexText3 = new CodexText(resource.Name);
            content2.Add((ICodexWidget) codexText3);
            flag2 = true;
          }
        }
      }
    }
    if (!flag2)
      return;
    content2.Add((ICodexWidget) new CodexSpacer());
    containers.Add(new ContentContainer(content2, ContentContainer.ContentLayout.Vertical));
  }

  private static void GenerateUnlockContainers(Tech tech, List<ContentContainer> containers)
  {
    containers.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexText((string) CODEX.HEADERS.TECH_UNLOCKS, CodexTextStyle.Subtitle),
      (ICodexWidget) new CodexDividerLine(),
      (ICodexWidget) new CodexSpacer()
    }, ContentContainer.ContentLayout.Vertical));
    foreach (TechItem unlockedItem in tech.unlockedItems)
      containers.Add(new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexImage(64, 64, unlockedItem.getUISprite("ui", false)),
        (ICodexWidget) new CodexText(unlockedItem.Name)
      }, ContentContainer.ContentLayout.Horizontal));
  }

  private static void GenerateRecipeContainers(Tag prefabID, List<ContentContainer> containers)
  {
    Recipe recipe1 = (Recipe) null;
    foreach (Recipe recipe2 in RecipeManager.Get().recipes)
    {
      if (Tag.op_Equality(recipe2.Result, prefabID))
      {
        recipe1 = recipe2;
        break;
      }
    }
    if (recipe1 == null)
      return;
    containers.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexText((string) CODEX.HEADERS.RECIPE, CodexTextStyle.Subtitle),
      (ICodexWidget) new CodexSpacer(),
      (ICodexWidget) new CodexDividerLine()
    }, ContentContainer.ContentLayout.Vertical));
    Func<Recipe, List<ContentContainer>> func = (Func<Recipe, List<ContentContainer>>) (rec =>
    {
      List<ContentContainer> recipeContainers = new List<ContentContainer>();
      foreach (Recipe.Ingredient ingredient in rec.Ingredients)
      {
        GameObject prefab = Assets.GetPrefab(ingredient.tag);
        if (Object.op_Inequality((Object) prefab, (Object) null))
          recipeContainers.Add(new ContentContainer(new List<ICodexWidget>()
          {
            (ICodexWidget) new CodexImage(64, 64, Def.GetUISprite((object) prefab)),
            (ICodexWidget) new CodexText(string.Format((string) UI.CODEX.RECIPE_ITEM, (object) Assets.GetPrefab(ingredient.tag).GetProperName(), (object) ingredient.amount, ElementLoader.GetElement(ingredient.tag) == null ? (object) "" : (object) UI.UNITSUFFIXES.MASS.KILOGRAM.text))
          }, ContentContainer.ContentLayout.Horizontal));
      }
      return recipeContainers;
    });
    containers.AddRange((IEnumerable<ContentContainer>) func(recipe1));
    GameObject go = recipe1.fabricators == null ? (GameObject) null : Assets.GetPrefab(Tag.op_Implicit(recipe1.fabricators[0]));
    if (!Object.op_Inequality((Object) go, (Object) null))
      return;
    containers.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexText((string) UI.CODEX.RECIPE_FABRICATOR_HEADER, CodexTextStyle.Subtitle),
      (ICodexWidget) new CodexDividerLine()
    }, ContentContainer.ContentLayout.Vertical));
    containers.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexImage(64, 64, Def.GetUISpriteFromMultiObjectAnim(go.GetComponent<KBatchedAnimController>().AnimFiles[0])),
      (ICodexWidget) new CodexText(string.Format((string) UI.CODEX.RECIPE_FABRICATOR, (object) recipe1.FabricationTime, (object) go.GetProperName()))
    }, ContentContainer.ContentLayout.Horizontal));
  }

  private static void GenerateUsedInRecipeContainers(
    Tag prefabID,
    List<ContentContainer> containers)
  {
    List<Recipe> recipeList = new List<Recipe>();
    foreach (Recipe recipe in RecipeManager.Get().recipes)
    {
      foreach (Recipe.Ingredient ingredient in recipe.Ingredients)
      {
        if (Tag.op_Equality(ingredient.tag, prefabID))
          recipeList.Add(recipe);
      }
    }
    if (recipeList.Count == 0)
      return;
    containers.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexText((string) CODEX.HEADERS.USED_IN_RECIPES, CodexTextStyle.Subtitle),
      (ICodexWidget) new CodexSpacer(),
      (ICodexWidget) new CodexDividerLine()
    }, ContentContainer.ContentLayout.Vertical));
    foreach (Recipe recipe in recipeList)
    {
      GameObject prefab = Assets.GetPrefab(recipe.Result);
      containers.Add(new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexImage(64, 64, Def.GetUISprite((object) prefab)),
        (ICodexWidget) new CodexText(prefab.GetProperName())
      }, ContentContainer.ContentLayout.Horizontal));
    }
  }

  private static void GenerateRoomTypeDetailsContainers(
    RoomType roomType,
    List<ContentContainer> containers)
  {
    ContentContainer contentContainer1 = new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexText((string) UI.CODEX.DETAILS, CodexTextStyle.Subtitle),
      (ICodexWidget) new CodexDividerLine()
    }, ContentContainer.ContentLayout.Vertical);
    containers.Add(contentContainer1);
    List<ICodexWidget> content = new List<ICodexWidget>();
    if (!string.IsNullOrEmpty(roomType.effect))
    {
      string roomEffectsString = roomType.GetRoomEffectsString();
      content.Add((ICodexWidget) new CodexText(roomEffectsString));
      content.Add((ICodexWidget) new CodexSpacer());
    }
    if ((roomType.primary_constraint != null ? 1 : (roomType.additional_constraints != null ? 1 : 0)) != 0)
    {
      content.Add((ICodexWidget) new CodexText((string) ROOMS.CRITERIA.HEADER));
      string text = "";
      if (roomType.primary_constraint != null)
        text = text + "    • " + roomType.primary_constraint.name;
      if (roomType.additional_constraints != null)
      {
        for (int index = 0; index < roomType.additional_constraints.Length; ++index)
          text = text + "\n    • " + roomType.additional_constraints[index].name;
      }
      content.Add((ICodexWidget) new CodexText(text));
    }
    ContentContainer contentContainer2 = new ContentContainer(content, ContentContainer.ContentLayout.Vertical);
    containers.Add(contentContainer2);
  }

  private static void GenerateRoomTypeDescriptionContainers(
    RoomType roomType,
    List<ContentContainer> containers)
  {
    ContentContainer contentContainer = new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexText(roomType.description),
      (ICodexWidget) new CodexSpacer()
    }, ContentContainer.ContentLayout.Vertical);
    containers.Add(contentContainer);
  }

  private static void GeneratePlantDescriptionContainers(
    GameObject plant,
    List<ContentContainer> containers)
  {
    SeedProducer component1 = plant.GetComponent<SeedProducer>();
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      GameObject prefab = Assets.GetPrefab(Tag.op_Implicit(component1.seedInfo.seedId));
      containers.Add(new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexText((string) CODEX.HEADERS.GROWNFROMSEED, CodexTextStyle.Subtitle),
        (ICodexWidget) new CodexDividerLine()
      }, ContentContainer.ContentLayout.Vertical));
      containers.Add(new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexImage(48, 48, Def.GetUISprite((object) prefab)),
        (ICodexWidget) new CodexText(prefab.GetProperName())
      }, ContentContainer.ContentLayout.Horizontal));
    }
    List<ICodexWidget> content = new List<ICodexWidget>();
    content.Add((ICodexWidget) new CodexSpacer());
    content.Add((ICodexWidget) new CodexText((string) UI.CODEX.DETAILS, CodexTextStyle.Subtitle));
    content.Add((ICodexWidget) new CodexDividerLine());
    InfoDescription component2 = Assets.GetPrefab(plant.PrefabID()).GetComponent<InfoDescription>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      content.Add((ICodexWidget) new CodexText(component2.description));
    string str1 = "";
    List<Descriptor> requirementDescriptors = GameUtil.GetPlantRequirementDescriptors(plant);
    if (requirementDescriptors.Count > 0)
    {
      string text = str1 + requirementDescriptors[0].text;
      for (int index = 1; index < requirementDescriptors.Count; ++index)
        text = text + "\n    • " + requirementDescriptors[index].text;
      content.Add((ICodexWidget) new CodexText(text));
      content.Add((ICodexWidget) new CodexSpacer());
    }
    string str2 = "";
    List<Descriptor> effectDescriptors = GameUtil.GetPlantEffectDescriptors(plant);
    if (effectDescriptors.Count > 0)
    {
      string text = str2 + effectDescriptors[0].text;
      for (int index = 1; index < effectDescriptors.Count; ++index)
        text = text + "\n    • " + effectDescriptors[index].text;
      CodexText codexText = new CodexText(text);
      content.Add((ICodexWidget) codexText);
      content.Add((ICodexWidget) new CodexSpacer());
    }
    containers.Add(new ContentContainer(content, ContentContainer.ContentLayout.Vertical));
  }

  private static ICodexWidget GetIconWidget(object entity) => (ICodexWidget) new CodexImage(32, 32, Def.GetUISprite(entity));

  private static void GenerateCreatureDescriptionContainers(
    GameObject creature,
    List<ContentContainer> containers)
  {
    containers.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexText(creature.GetComponent<InfoDescription>().description)
    }, ContentContainer.ContentLayout.Vertical));
    RobotBatteryMonitor.Def def1 = creature.GetDef<RobotBatteryMonitor.Def>();
    if (def1 != null)
    {
      Amount batteryAmount = Db.Get().Amounts.Get(def1.batteryAmountId);
      float num = Db.Get().traits.Get(creature.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers.Find((Predicate<AttributeModifier>) (match => match.AttributeId == batteryAmount.maxAttribute.Id)).Value;
      containers.Add(new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexSpacer(),
        (ICodexWidget) new CodexText((string) CODEX.HEADERS.INTERNALBATTERY, CodexTextStyle.Subtitle),
        (ICodexWidget) new CodexText("    • " + string.Format((string) CODEX.ROBOT_DESCRIPTORS.BATTERY.CAPACITY, (object) num))
      }, ContentContainer.ContentLayout.Vertical));
    }
    if (creature.GetDef<StorageUnloadMonitor.Def>() != null)
      containers.Add(new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexSpacer(),
        (ICodexWidget) new CodexText((string) CODEX.HEADERS.INTERNALSTORAGE, CodexTextStyle.Subtitle),
        (ICodexWidget) new CodexText("    • " + string.Format((string) CODEX.ROBOT_DESCRIPTORS.STORAGE.CAPACITY, (object) creature.GetComponents<Storage>()[1].Capacity()))
      }, ContentContainer.ContentLayout.Vertical));
    List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(TagExtensions.ToTag(creature.PrefabID().ToString() + "Egg"));
    if (prefabsWithTag != null && prefabsWithTag.Count > 0)
    {
      containers.Add(new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexSpacer(),
        (ICodexWidget) new CodexText((string) CODEX.HEADERS.HATCHESFROMEGG, CodexTextStyle.Subtitle)
      }, ContentContainer.ContentLayout.Vertical));
      foreach (GameObject go in prefabsWithTag)
        containers.Add(new ContentContainer(new List<ICodexWidget>()
        {
          (ICodexWidget) new CodexIndentedLabelWithIcon(go.GetProperName(), CodexTextStyle.Body, Def.GetUISprite((object) go))
        }, ContentContainer.ContentLayout.Horizontal));
    }
    TemperatureVulnerable component1 = creature.GetComponent<TemperatureVulnerable>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      containers.Add(new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexSpacer(),
        (ICodexWidget) new CodexText((string) CODEX.HEADERS.COMFORTRANGE, CodexTextStyle.Subtitle),
        (ICodexWidget) new CodexText("    • " + string.Format((string) CODEX.CREATURE_DESCRIPTORS.TEMPERATURE.COMFORT_RANGE, (object) GameUtil.GetFormattedTemperature(component1.TemperatureWarningLow), (object) GameUtil.GetFormattedTemperature(component1.TemperatureWarningHigh))),
        (ICodexWidget) new CodexText("    • " + string.Format((string) CODEX.CREATURE_DESCRIPTORS.TEMPERATURE.NON_LETHAL_RANGE, (object) GameUtil.GetFormattedTemperature(component1.TemperatureLethalLow), (object) GameUtil.GetFormattedTemperature(component1.TemperatureLethalHigh)))
      }, ContentContainer.ContentLayout.Vertical));
    int amount = 0;
    string str = (string) null;
    Tag tag = new Tag();
    Butcherable component2 = creature.GetComponent<Butcherable>();
    if (Object.op_Inequality((Object) component2, (Object) null) && component2.drops != null)
    {
      amount = component2.drops.Length;
      if (amount > 0)
        ((Tag) ref tag).Name = str = component2.drops[0];
    }
    string text1 = (string) null;
    string text2 = (string) null;
    if (((Tag) ref tag).IsValid)
    {
      text1 = TagManager.GetProperName(tag, false);
      text2 = "\t" + GameUtil.GetFormattedByTag(tag, (float) amount);
    }
    if (!string.IsNullOrEmpty(text1) && !string.IsNullOrEmpty(text2))
    {
      ContentContainer contentContainer1 = new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexSpacer(),
        (ICodexWidget) new CodexText((string) CODEX.HEADERS.CRITTERDROPS, CodexTextStyle.Subtitle)
      }, ContentContainer.ContentLayout.Vertical);
      ContentContainer contentContainer2 = new ContentContainer(new List<ICodexWidget>()
      {
        (ICodexWidget) new CodexIndentedLabelWithIcon(text1, CodexTextStyle.Body, Def.GetUISprite((object) str)),
        (ICodexWidget) new CodexText(text2)
      }, ContentContainer.ContentLayout.Vertical);
      containers.Add(contentContainer1);
      containers.Add(contentContainer2);
    }
    List<Tag> tagList = new List<Tag>();
    Diet.Info[] infoArray = (Diet.Info[]) null;
    CreatureCalorieMonitor.Def def2 = creature.GetDef<CreatureCalorieMonitor.Def>();
    BeehiveCalorieMonitor.Def def3 = creature.GetDef<BeehiveCalorieMonitor.Def>();
    if (def2 != null)
      infoArray = def2.diet.infos;
    else if (def3 != null)
      infoArray = def3.diet.infos;
    if (infoArray == null || infoArray.Length == 0)
      return;
    float num1 = 0.0f;
    foreach (AttributeModifier selfModifier in Db.Get().traits.Get(creature.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers)
    {
      if (selfModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
        num1 = selfModifier.Value;
    }
    List<ICodexWidget> content = new List<ICodexWidget>();
    foreach (Diet.Info info in infoArray)
    {
      if (info.consumedTags.Count != 0)
      {
        foreach (Tag consumedTag in info.consumedTags)
        {
          Element elementByHash = ElementLoader.FindElementByHash(ElementLoader.GetElementID(consumedTag));
          if (elementByHash.id != SimHashes.Vacuum && elementByHash.id != SimHashes.Void || !Object.op_Equality((Object) Assets.GetPrefab(consumedTag), (Object) null))
          {
            float inputAmount = -num1 / info.caloriesPerKg;
            float outputAmount = inputAmount * info.producedConversionRate;
            content.Add((ICodexWidget) new CodexConversionPanel(consumedTag.ProperName(), consumedTag, inputAmount, true, info.producedElement, outputAmount, true, creature));
          }
        }
      }
    }
    ContentContainer contents = new ContentContainer(content, ContentContainer.ContentLayout.Vertical);
    containers.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexSpacer(),
      (ICodexWidget) new CodexCollapsibleHeader((string) CODEX.HEADERS.DIET, contents)
    }, ContentContainer.ContentLayout.Vertical));
    containers.Add(contents);
    containers.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexSpacer(),
      (ICodexWidget) new CodexSpacer()
    }, ContentContainer.ContentLayout.Vertical));
  }

  private static void GenerateDiseaseDescriptionContainers(
    Klei.AI.Disease disease,
    List<ContentContainer> containers)
  {
    List<ICodexWidget> content = new List<ICodexWidget>();
    content.Add((ICodexWidget) new CodexSpacer());
    StringEntry stringEntry = (StringEntry) null;
    if (Strings.TryGet("STRINGS.DUPLICANTS.DISEASES." + disease.Id.ToUpper() + ".DESC", ref stringEntry))
    {
      content.Add((ICodexWidget) new CodexText(stringEntry.String));
      content.Add((ICodexWidget) new CodexSpacer());
    }
    foreach (Descriptor quantitativeDescriptor in disease.GetQuantitativeDescriptors())
      content.Add((ICodexWidget) new CodexText(quantitativeDescriptor.text));
    content.Add((ICodexWidget) new CodexSpacer());
    containers.Add(new ContentContainer(content, ContentContainer.ContentLayout.Vertical));
  }

  private static void GenerateFoodDescriptionContainers(
    EdiblesManager.FoodInfo food,
    List<ContentContainer> containers)
  {
    List<ICodexWidget> content = new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexText(food.Description),
      (ICodexWidget) new CodexSpacer(),
      (ICodexWidget) new CodexText(string.Format((string) UI.CODEX.FOOD.QUALITY, (object) GameUtil.GetFormattedFoodQuality(food.Quality))),
      (ICodexWidget) new CodexText(string.Format((string) UI.CODEX.FOOD.CALORIES, (object) GameUtil.GetFormattedCalories(food.CaloriesPerUnit))),
      (ICodexWidget) new CodexSpacer(),
      (ICodexWidget) new CodexText(food.CanRot ? string.Format((string) UI.CODEX.FOOD.SPOILPROPERTIES, (object) GameUtil.GetFormattedTemperature(food.RotTemperature), (object) GameUtil.GetFormattedTemperature(food.PreserveTemperature), (object) GameUtil.GetFormattedCycles(food.SpoilTime)) : UI.CODEX.FOOD.NON_PERISHABLE.ToString()),
      (ICodexWidget) new CodexSpacer()
    };
    if (food.Effects.Count > 0)
    {
      content.Add((ICodexWidget) new CodexText((string) CODEX.HEADERS.FOODEFFECTS + ":"));
      foreach (string effect1 in food.Effects)
      {
        Effect effect2 = Db.Get().effects.Get(effect1);
        string text = StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + effect1.ToUpper() + ".NAME"));
        string str1 = StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + effect1.ToUpper() + ".DESCRIPTION"));
        string str2 = "";
        foreach (AttributeModifier selfModifier in effect2.SelfModifiers)
          str2 = str2 + "\n    • " + StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".NAME")) + ": " + selfModifier.GetFormattedString();
        string tooltip = str1 + str2;
        string str3 = UI.FormatAsLink(text, CodexEntryGenerator.FOOD_EFFECTS_ENTRY_ID);
        content.Add((ICodexWidget) new CodexTextWithTooltip("    • " + str3, tooltip));
      }
      content.Add((ICodexWidget) new CodexSpacer());
    }
    containers.Add(new ContentContainer(content, ContentContainer.ContentLayout.Vertical));
  }

  private static void GenerateTechDescriptionContainers(
    Tech tech,
    List<ContentContainer> containers)
  {
    containers.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexText(StringEntry.op_Implicit(Strings.Get("STRINGS.RESEARCH.TECHS." + tech.Id.ToUpper() + ".DESC"))),
      (ICodexWidget) new CodexSpacer()
    }, ContentContainer.ContentLayout.Vertical));
  }

  private static void GenerateGenericDescriptionContainers(
    string description,
    List<ContentContainer> containers)
  {
    containers.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexText(description),
      (ICodexWidget) new CodexSpacer()
    }, ContentContainer.ContentLayout.Vertical));
  }

  private static void GenerateBuildingDescriptionContainers(
    BuildingDef def,
    List<ContentContainer> containers)
  {
    List<ICodexWidget> content = new List<ICodexWidget>();
    content.Add((ICodexWidget) new CodexText(StringEntry.op_Implicit(Strings.Get("STRINGS.BUILDINGS.PREFABS." + def.PrefabID.ToUpper() + ".EFFECT"))));
    content.Add((ICodexWidget) new CodexSpacer());
    List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(def.BuildingComplete);
    List<Descriptor> requirementDescriptors = GameUtil.GetRequirementDescriptors(allDescriptors);
    if (requirementDescriptors.Count > 0)
    {
      content.Add((ICodexWidget) new CodexText((string) CODEX.HEADERS.BUILDINGREQUIREMENTS, CodexTextStyle.Subtitle));
      foreach (Descriptor descriptor in requirementDescriptors)
        content.Add((ICodexWidget) new CodexTextWithTooltip("    " + descriptor.text, descriptor.tooltipText));
      content.Add((ICodexWidget) new CodexSpacer());
    }
    List<Descriptor> effectDescriptors = GameUtil.GetEffectDescriptors(allDescriptors);
    if (effectDescriptors.Count > 0)
    {
      content.Add((ICodexWidget) new CodexText((string) CODEX.HEADERS.BUILDINGEFFECTS, CodexTextStyle.Subtitle));
      foreach (Descriptor descriptor in effectDescriptors)
        content.Add((ICodexWidget) new CodexTextWithTooltip("    " + descriptor.text, descriptor.tooltipText));
      content.Add((ICodexWidget) new CodexSpacer());
    }
    KPrefabID component = def.BuildingComplete.GetComponent<KPrefabID>();
    bool flag = false;
    foreach (Tag tag in component.Tags)
    {
      if (CodexEntryGenerator.room_constraint_to_building_label_dict.ContainsKey(tag))
      {
        flag = true;
        break;
      }
    }
    if (flag)
    {
      content.Add((ICodexWidget) new CodexText((string) CODEX.HEADERS.BUILDINGTYPE, CodexTextStyle.Subtitle));
      foreach (Tag tag in component.Tags)
      {
        string str;
        if (CodexEntryGenerator.room_constraint_to_building_label_dict.TryGetValue(tag, out str))
          content.Add((ICodexWidget) new CodexText("    " + str));
      }
      content.Add((ICodexWidget) new CodexSpacer());
    }
    content.Add((ICodexWidget) new CodexText("<i>" + StringEntry.op_Implicit(Strings.Get("STRINGS.BUILDINGS.PREFABS." + def.PrefabID.ToUpper() + ".DESC")) + "</i>"));
    containers.Add(new ContentContainer(content, ContentContainer.ContentLayout.Vertical));
  }

  private static void GenerateImageContainers(
    Sprite[] sprites,
    List<ContentContainer> containers,
    ContentContainer.ContentLayout layout)
  {
    List<ICodexWidget> content = new List<ICodexWidget>();
    foreach (Sprite sprite in sprites)
    {
      if (!Object.op_Equality((Object) sprite, (Object) null))
      {
        CodexImage codexImage = new CodexImage(128, 128, sprite);
        content.Add((ICodexWidget) codexImage);
      }
    }
    containers.Add(new ContentContainer(content, layout));
  }

  private static void GenerateImageContainers(
    Tuple<Sprite, Color>[] sprites,
    List<ContentContainer> containers,
    ContentContainer.ContentLayout layout)
  {
    List<ICodexWidget> content = new List<ICodexWidget>();
    foreach (Tuple<Sprite, Color> sprite in sprites)
    {
      if (sprite != null)
      {
        CodexImage codexImage = new CodexImage(128, 128, sprite);
        content.Add((ICodexWidget) codexImage);
      }
    }
    containers.Add(new ContentContainer(content, layout));
  }

  private static void GenerateImageContainers(Sprite sprite, List<ContentContainer> containers) => containers.Add(new ContentContainer(new List<ICodexWidget>()
  {
    (ICodexWidget) new CodexImage(128, 128, sprite)
  }, ContentContainer.ContentLayout.Vertical));

  public static void CreateUnlockablesContentContainer(SubEntry subentry) => subentry.lockedContentContainer = new ContentContainer(new List<ICodexWidget>()
  {
    (ICodexWidget) new CodexText((string) CODEX.HEADERS.SECTION_UNLOCKABLES, CodexTextStyle.Subtitle),
    (ICodexWidget) new CodexDividerLine()
  }, ContentContainer.ContentLayout.Vertical)
  {
    showBeforeGeneratedContent = false
  };

  private static void GenerateFabricatorContainers(
    GameObject entity,
    List<ContentContainer> containers)
  {
    ComplexFabricator component = entity.GetComponent<ComplexFabricator>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    containers.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexSpacer(),
      (ICodexWidget) new CodexText(StringEntry.op_Implicit(Strings.Get("STRINGS.CODEX.HEADERS.FABRICATIONS")), CodexTextStyle.Subtitle),
      (ICodexWidget) new CodexDividerLine()
    }, ContentContainer.ContentLayout.Vertical));
    List<ICodexWidget> content = new List<ICodexWidget>();
    foreach (ComplexRecipe recipe in component.GetRecipes())
      content.Add((ICodexWidget) new CodexRecipePanel(recipe));
    containers.Add(new ContentContainer(content, ContentContainer.ContentLayout.Vertical));
  }

  private static void GenerateConfigurableConsumerContainers(
    GameObject buildingComplete,
    List<ContentContainer> containers)
  {
    IConfigurableConsumer component = buildingComplete.GetComponent<IConfigurableConsumer>();
    if (component == null)
      return;
    containers.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexSpacer(),
      (ICodexWidget) new CodexText(StringEntry.op_Implicit(Strings.Get("STRINGS.CODEX.HEADERS.FABRICATIONS")), CodexTextStyle.Subtitle),
      (ICodexWidget) new CodexDividerLine()
    }, ContentContainer.ContentLayout.Vertical));
    List<ICodexWidget> content = new List<ICodexWidget>();
    foreach (IConfigurableConsumerOption settingOption in component.GetSettingOptions())
      content.Add((ICodexWidget) new CodexConfigurableConsumerRecipePanel(settingOption));
    containers.Add(new ContentContainer(content, ContentContainer.ContentLayout.Vertical));
  }

  private static void GenerateReceptacleContainers(
    GameObject entity,
    List<ContentContainer> containers)
  {
    SingleEntityReceptacle plot = entity.GetComponent<SingleEntityReceptacle>();
    if (Object.op_Equality((Object) plot, (Object) null))
      return;
    containers.Add(new ContentContainer(new List<ICodexWidget>()
    {
      (ICodexWidget) new CodexText(StringEntry.op_Implicit(Strings.Get("STRINGS.CODEX.HEADERS.RECEPTACLE")), CodexTextStyle.Subtitle),
      (ICodexWidget) new CodexDividerLine()
    }, ContentContainer.ContentLayout.Vertical));
    foreach (Tag depositObjectTag in (IEnumerable<Tag>) plot.possibleDepositObjectTags)
    {
      List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(depositObjectTag);
      if (Object.op_Equality((Object) plot.rotatable, (Object) null))
        prefabsWithTag.RemoveAll((Predicate<GameObject>) (go =>
        {
          IReceptacleDirection component = go.GetComponent<IReceptacleDirection>();
          return component != null && component.Direction != plot.Direction;
        }));
      foreach (GameObject go in prefabsWithTag)
        containers.Add(new ContentContainer(new List<ICodexWidget>()
        {
          (ICodexWidget) new CodexImage(64, 64, Def.GetUISprite((object) go).first),
          (ICodexWidget) new CodexText(go.GetProperName())
        }, ContentContainer.ContentLayout.Horizontal));
    }
  }

  private class ConversionEntry
  {
    public string title;
    public GameObject prefab;
    public HashSet<ElementUsage> inSet = new HashSet<ElementUsage>();
    public HashSet<ElementUsage> outSet = new HashSet<ElementUsage>();
  }

  private class CodexElementMap
  {
    public Dictionary<Tag, List<CodexEntryGenerator.ConversionEntry>> map = new Dictionary<Tag, List<CodexEntryGenerator.ConversionEntry>>();

    public void Add(Tag t, CodexEntryGenerator.ConversionEntry ce)
    {
      List<CodexEntryGenerator.ConversionEntry> conversionEntryList;
      if (this.map.TryGetValue(t, out conversionEntryList))
        conversionEntryList.Add(ce);
      else
        this.map[t] = new List<CodexEntryGenerator.ConversionEntry>()
        {
          ce
        };
    }
  }
}
