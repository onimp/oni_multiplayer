// Decompiled with JetBrains decompiler
// Type: Database.MiscStatusItems
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
  public class MiscStatusItems : StatusItems
  {
    public StatusItem AttentionRequired;
    public StatusItem MarkedForDisinfection;
    public StatusItem MarkedForCompost;
    public StatusItem MarkedForCompostInStorage;
    public StatusItem PendingClear;
    public StatusItem PendingClearNoStorage;
    public StatusItem Edible;
    public StatusItem WaitingForDig;
    public StatusItem WaitingForMop;
    public StatusItem OreMass;
    public StatusItem OreTemp;
    public StatusItem ElementalCategory;
    public StatusItem ElementalState;
    public StatusItem ElementalTemperature;
    public StatusItem ElementalMass;
    public StatusItem ElementalDisease;
    public StatusItem TreeFilterableTags;
    public StatusItem SublimationOverpressure;
    public StatusItem SublimationEmitting;
    public StatusItem SublimationBlocked;
    public StatusItem BuriedItem;
    public StatusItem SpoutOverPressure;
    public StatusItem SpoutEmitting;
    public StatusItem SpoutPressureBuilding;
    public StatusItem SpoutIdle;
    public StatusItem SpoutDormant;
    public StatusItem SpicedFood;
    public StatusItem OrderAttack;
    public StatusItem OrderCapture;
    public StatusItem PendingHarvest;
    public StatusItem NotMarkedForHarvest;
    public StatusItem PendingUproot;
    public StatusItem PickupableUnreachable;
    public StatusItem Prioritized;
    public StatusItem Using;
    public StatusItem Operating;
    public StatusItem Cleaning;
    public StatusItem RegionIsBlocked;
    public StatusItem NoClearLocationsAvailable;
    public StatusItem AwaitingStudy;
    public StatusItem Studied;
    public StatusItem StudiedGeyserTimeRemaining;
    public StatusItem Space;
    public StatusItem HighEnergyParticleCount;
    public StatusItem Durability;
    public StatusItem StoredItemDurability;
    public StatusItem ArtifactEntombed;
    public StatusItem TearOpen;
    public StatusItem TearClosed;

    public MiscStatusItems(ResourceSet parent)
      : base(nameof (MiscStatusItems), parent)
    {
      this.CreateStatusItems();
    }

    private StatusItem CreateStatusItem(
      string id,
      string prefix,
      string icon,
      StatusItem.IconType icon_type,
      NotificationType notification_type,
      bool allow_multiples,
      HashedString render_overlay,
      bool showWorldIcon = true,
      int status_overlays = 129022)
    {
      return this.Add(new StatusItem(id, prefix, icon, icon_type, notification_type, allow_multiples, render_overlay, showWorldIcon, status_overlays));
    }

    private StatusItem CreateStatusItem(
      string id,
      string name,
      string tooltip,
      string icon,
      StatusItem.IconType icon_type,
      NotificationType notification_type,
      bool allow_multiples,
      HashedString render_overlay,
      int status_overlays = 129022)
    {
      return this.Add(new StatusItem(id, name, tooltip, icon, icon_type, notification_type, allow_multiples, render_overlay, status_overlays));
    }

    private void CreateStatusItems()
    {
      this.AttentionRequired = this.CreateStatusItem("AttentionRequired", "MISC", "status_item_doubleexclamation", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Edible = this.CreateStatusItem("Edible", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Edible.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        global::Edible edible = (global::Edible) data;
        str = string.Format(str, (object) GameUtil.GetFormattedCalories(edible.Calories));
        return str;
      });
      this.PendingClear = this.CreateStatusItem("PendingClear", "MISC", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.PendingClearNoStorage = this.CreateStatusItem("PendingClearNoStorage", "MISC", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.MarkedForCompost = this.CreateStatusItem("MarkedForCompost", "MISC", "status_item_pending_compost", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.MarkedForCompostInStorage = this.CreateStatusItem("MarkedForCompostInStorage", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.MarkedForDisinfection = this.CreateStatusItem("MarkedForDisinfection", "MISC", "status_item_disinfect", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.Disease.ID);
      this.NoClearLocationsAvailable = this.CreateStatusItem("NoClearLocationsAvailable", "MISC", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.WaitingForDig = this.CreateStatusItem("WaitingForDig", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.WaitingForMop = this.CreateStatusItem("WaitingForMop", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.OreMass = this.CreateStatusItem("OreMass", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.OreMass.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        GameObject gameObject = (GameObject) data;
        str = str.Replace("{Mass}", GameUtil.GetFormattedMass(gameObject.GetComponent<PrimaryElement>().Mass));
        return str;
      });
      this.OreTemp = this.CreateStatusItem("OreTemp", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.OreTemp.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        GameObject gameObject = (GameObject) data;
        str = str.Replace("{Temp}", GameUtil.GetFormattedTemperature(gameObject.GetComponent<PrimaryElement>().Temperature));
        return str;
      });
      this.ElementalState = this.CreateStatusItem("ElementalState", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.ElementalState.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        Element element = ((Func<Element>) data)();
        str = str.Replace("{State}", element.GetStateString());
        return str;
      });
      this.ElementalCategory = this.CreateStatusItem("ElementalCategory", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.ElementalCategory.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        Element element = ((Func<Element>) data)();
        str = str.Replace("{Category}", element.GetMaterialCategoryTag().ProperName());
        return str;
      });
      this.ElementalTemperature = this.CreateStatusItem("ElementalTemperature", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.ElementalTemperature.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        CellSelectionObject cellSelectionObject = (CellSelectionObject) data;
        str = str.Replace("{Temp}", GameUtil.GetFormattedTemperature(cellSelectionObject.temperature));
        return str;
      });
      this.ElementalMass = this.CreateStatusItem("ElementalMass", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.ElementalMass.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        CellSelectionObject cellSelectionObject = (CellSelectionObject) data;
        str = str.Replace("{Mass}", GameUtil.GetFormattedMass(cellSelectionObject.Mass));
        return str;
      });
      this.ElementalDisease = this.CreateStatusItem("ElementalDisease", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.ElementalDisease.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        CellSelectionObject cellSelectionObject = (CellSelectionObject) data;
        str = str.Replace("{Disease}", GameUtil.GetFormattedDisease(cellSelectionObject.diseaseIdx, cellSelectionObject.diseaseCount));
        return str;
      });
      this.ElementalDisease.resolveTooltipCallback = (Func<string, object, string>) ((str, data) =>
      {
        CellSelectionObject cellSelectionObject = (CellSelectionObject) data;
        str = str.Replace("{Disease}", GameUtil.GetFormattedDisease(cellSelectionObject.diseaseIdx, cellSelectionObject.diseaseCount, true));
        return str;
      });
      this.TreeFilterableTags = this.CreateStatusItem("TreeFilterableTags", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.TreeFilterableTags.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        TreeFilterable treeFilterable = (TreeFilterable) data;
        str = str.Replace("{Tags}", treeFilterable.GetTagsAsStatus());
        return str;
      });
      this.SublimationEmitting = this.CreateStatusItem("SublimationEmitting", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.SublimationEmitting.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        CellSelectionObject cellSelectionObject = (CellSelectionObject) data;
        if (cellSelectionObject.element.sublimateId == (SimHashes) 0)
          return str;
        str = str.Replace("{Element}", GameUtil.GetElementNameByElementHash(cellSelectionObject.element.sublimateId));
        str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(cellSelectionObject.FlowRate, GameUtil.TimeSlice.PerSecond));
        return str;
      });
      this.SublimationEmitting.resolveTooltipCallback = this.SublimationEmitting.resolveStringCallback;
      this.SublimationBlocked = this.CreateStatusItem("SublimationBlocked", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.SublimationBlocked.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        CellSelectionObject cellSelectionObject = (CellSelectionObject) data;
        if (cellSelectionObject.element.sublimateId == (SimHashes) 0)
          return str;
        str = str.Replace("{Element}", cellSelectionObject.element.name);
        str = str.Replace("{SubElement}", GameUtil.GetElementNameByElementHash(cellSelectionObject.element.sublimateId));
        return str;
      });
      this.SublimationBlocked.resolveTooltipCallback = this.SublimationBlocked.resolveStringCallback;
      this.SublimationOverpressure = this.CreateStatusItem("SublimationOverpressure", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.SublimationOverpressure.resolveTooltipCallback = (Func<string, object, string>) ((str, data) =>
      {
        CellSelectionObject cellSelectionObject = (CellSelectionObject) data;
        if (cellSelectionObject.element.sublimateId == (SimHashes) 0)
          return str;
        str = str.Replace("{Element}", cellSelectionObject.element.name);
        str = str.Replace("{SubElement}", GameUtil.GetElementNameByElementHash(cellSelectionObject.element.sublimateId));
        return str;
      });
      this.Space = this.CreateStatusItem("Space", "MISC", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID);
      this.BuriedItem = this.CreateStatusItem("BuriedItem", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.SpoutOverPressure = this.CreateStatusItem("SpoutOverPressure", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.SpoutOverPressure.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        Geyser.StatesInstance statesInstance = (Geyser.StatesInstance) data;
        Studyable component = statesInstance.GetComponent<Studyable>();
        str = statesInstance == null || !Object.op_Inequality((Object) component, (Object) null) || !component.Studied ? str.Replace("{StudiedDetails}", "") : str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTOVERPRESSURE.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance.master.RemainingEruptTime())));
        return str;
      });
      this.SpoutEmitting = this.CreateStatusItem("SpoutEmitting", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.SpoutEmitting.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        Geyser.StatesInstance statesInstance = (Geyser.StatesInstance) data;
        Studyable component = statesInstance.GetComponent<Studyable>();
        str = statesInstance == null || !Object.op_Inequality((Object) component, (Object) null) || !component.Studied ? str.Replace("{StudiedDetails}", "") : str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTEMITTING.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance.master.RemainingEruptTime())));
        return str;
      });
      this.SpoutPressureBuilding = this.CreateStatusItem("SpoutPressureBuilding", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.SpoutPressureBuilding.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        Geyser.StatesInstance statesInstance = (Geyser.StatesInstance) data;
        Studyable component = statesInstance.GetComponent<Studyable>();
        str = statesInstance == null || !Object.op_Inequality((Object) component, (Object) null) || !component.Studied ? str.Replace("{StudiedDetails}", "") : str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTPRESSUREBUILDING.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance.master.RemainingNonEruptTime())));
        return str;
      });
      this.SpoutIdle = this.CreateStatusItem("SpoutIdle", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.SpoutIdle.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        Geyser.StatesInstance statesInstance = (Geyser.StatesInstance) data;
        Studyable component = statesInstance.GetComponent<Studyable>();
        str = statesInstance == null || !Object.op_Inequality((Object) component, (Object) null) || !component.Studied ? str.Replace("{StudiedDetails}", "") : str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTIDLE.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance.master.RemainingNonEruptTime())));
        return str;
      });
      this.SpoutDormant = this.CreateStatusItem("SpoutDormant", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.SpicedFood = this.CreateStatusItem("SpicedFood", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.SpicedFood.resolveTooltipCallback = (Func<string, object, string>) ((baseString, data) =>
      {
        string statusItems = baseString;
        string str1 = "\n    • ";
        foreach (SpiceInstance spiceInstance in (List<SpiceInstance>) data)
        {
          Tag id = spiceInstance.Id;
          string str2 = "STRINGS.ITEMS.SPICES." + ((Tag) ref id).Name.ToUpper() + ".NAME";
          StringEntry stringEntry;
          Strings.TryGet(str2, ref stringEntry);
          string str3 = stringEntry == null ? "MISSING " + str2 : stringEntry.String;
          statusItems = statusItems + str1 + str3;
          string linePrefix = "\n        • ";
          if (spiceInstance.StatBonus != null)
            statusItems += Effect.CreateTooltip(spiceInstance.StatBonus, false, linePrefix, false);
        }
        return statusItems;
      });
      this.OrderAttack = this.CreateStatusItem("OrderAttack", "MISC", "status_item_attack", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.OrderCapture = this.CreateStatusItem("OrderCapture", "MISC", "status_item_capture", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.PendingHarvest = this.CreateStatusItem("PendingHarvest", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.NotMarkedForHarvest = this.CreateStatusItem("NotMarkedForHarvest", "MISC", "status_item_building_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.NotMarkedForHarvest.conditionalOverlayCallback = (Func<HashedString, object, bool>) ((viewMode, o) => !HashedString.op_Inequality(viewMode, OverlayModes.None.ID));
      this.PendingUproot = this.CreateStatusItem("PendingUproot", "MISC", "status_item_pending_uproot", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.PickupableUnreachable = this.CreateStatusItem("PickupableUnreachable", "MISC", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Prioritized = this.CreateStatusItem("Prioritized", "MISC", "status_item_prioritized", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Using = this.CreateStatusItem("Using", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Using.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        Workable workable = (Workable) data;
        if (Object.op_Inequality((Object) workable, (Object) null))
        {
          KSelectable component = ((Component) workable).GetComponent<KSelectable>();
          if (Object.op_Inequality((Object) component, (Object) null))
            str = str.Replace("{Target}", component.GetName());
        }
        return str;
      });
      this.Operating = this.CreateStatusItem("Operating", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Cleaning = this.CreateStatusItem("Cleaning", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.RegionIsBlocked = this.CreateStatusItem("RegionIsBlocked", "MISC", "status_item_solids_blocking", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.AwaitingStudy = this.CreateStatusItem("AwaitingStudy", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Studied = this.CreateStatusItem("Studied", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.HighEnergyParticleCount = this.CreateStatusItem("HighEnergyParticleCount", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.HighEnergyParticleCount.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        GameObject gameObject = (GameObject) data;
        str = GameUtil.GetFormattedHighEnergyParticles(Util.IsNullOrDestroyed((object) gameObject) ? 0.0f : gameObject.GetComponent<HighEnergyParticle>().payload);
        return str;
      });
      this.Durability = this.CreateStatusItem("Durability", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Durability.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        global::Durability component = ((GameObject) data).GetComponent<global::Durability>();
        str = str.Replace("{durability}", GameUtil.GetFormattedPercent(component.GetDurability() * 100f));
        return str;
      });
      this.StoredItemDurability = this.CreateStatusItem("StoredItemDurability", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.StoredItemDurability.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        global::Durability component = ((GameObject) data).GetComponent<global::Durability>();
        float percent = Object.op_Inequality((Object) component, (Object) null) ? component.GetDurability() * 100f : 100f;
        str = str.Replace("{durability}", GameUtil.GetFormattedPercent(percent));
        return str;
      });
      this.ArtifactEntombed = this.CreateStatusItem("ArtifactEntombed", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.TearOpen = this.CreateStatusItem("TearOpen", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.TearClosed = this.CreateStatusItem("TearClosed", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
    }
  }
}
