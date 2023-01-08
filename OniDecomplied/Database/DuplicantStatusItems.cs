// Decompiled with JetBrains decompiler
// Type: Database.DuplicantStatusItems
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using TUNING;
using UnityEngine;

namespace Database
{
  public class DuplicantStatusItems : StatusItems
  {
    public StatusItem Idle;
    public StatusItem Pacified;
    public StatusItem PendingPacification;
    public StatusItem Dead;
    public StatusItem MoveToSuitNotRequired;
    public StatusItem DroppingUnusedInventory;
    public StatusItem MovingToSafeArea;
    public StatusItem BedUnreachable;
    public StatusItem Hungry;
    public StatusItem Starving;
    public StatusItem Rotten;
    public StatusItem Quarantined;
    public StatusItem NoRationsAvailable;
    public StatusItem RationsUnreachable;
    public StatusItem RationsNotPermitted;
    public StatusItem DailyRationLimitReached;
    public StatusItem Scalding;
    public StatusItem Hot;
    public StatusItem Cold;
    public StatusItem QuarantineAreaUnassigned;
    public StatusItem QuarantineAreaUnreachable;
    public StatusItem Tired;
    public StatusItem NervousBreakdown;
    public StatusItem Unhappy;
    public StatusItem Suffocating;
    public StatusItem HoldingBreath;
    public StatusItem ToiletUnreachable;
    public StatusItem NoUsableToilets;
    public StatusItem NoToilets;
    public StatusItem Vomiting;
    public StatusItem Coughing;
    public StatusItem BreathingO2;
    public StatusItem EmittingCO2;
    public StatusItem LowOxygen;
    public StatusItem RedAlert;
    public StatusItem Digging;
    public StatusItem Eating;
    public StatusItem Dreaming;
    public StatusItem Sleeping;
    public StatusItem SleepingInterruptedByLight;
    public StatusItem SleepingInterruptedByNoise;
    public StatusItem SleepingInterruptedByFearOfDark;
    public StatusItem SleepingInterruptedByMovement;
    public StatusItem SleepingPeacefully;
    public StatusItem SleepingBadly;
    public StatusItem SleepingTerribly;
    public StatusItem Cleaning;
    public StatusItem PickingUp;
    public StatusItem Mopping;
    public StatusItem Cooking;
    public StatusItem Arting;
    public StatusItem Mushing;
    public StatusItem Researching;
    public StatusItem MissionControlling;
    public StatusItem Tinkering;
    public StatusItem Storing;
    public StatusItem Building;
    public StatusItem Equipping;
    public StatusItem WarmingUp;
    public StatusItem GeneratingPower;
    public StatusItem Ranching;
    public StatusItem Harvesting;
    public StatusItem Uprooting;
    public StatusItem Emptying;
    public StatusItem Toggling;
    public StatusItem Deconstructing;
    public StatusItem Disinfecting;
    public StatusItem Relocating;
    public StatusItem Upgrading;
    public StatusItem Fabricating;
    public StatusItem Processing;
    public StatusItem Spicing;
    public StatusItem Clearing;
    public StatusItem BodyRegulatingHeating;
    public StatusItem BodyRegulatingCooling;
    public StatusItem EntombedChore;
    public StatusItem EarlyMorning;
    public StatusItem NightTime;
    public StatusItem PoorDecor;
    public StatusItem PoorQualityOfLife;
    public StatusItem PoorFoodQuality;
    public StatusItem GoodFoodQuality;
    public StatusItem SevereWounds;
    public StatusItem Incapacitated;
    public StatusItem Fighting;
    public StatusItem Fleeing;
    public StatusItem Stressed;
    public StatusItem LashingOut;
    public StatusItem LowImmunity;
    public StatusItem Studying;
    public StatusItem Socializing;
    public StatusItem Mingling;
    public StatusItem ContactWithGerms;
    public StatusItem ExposedToGerms;
    public StatusItem LightWorkEfficiencyBonus;
    public StatusItem LaboratoryWorkEfficiencyBonus;
    public StatusItem BeingProductive;
    public StatusItem BalloonArtistPlanning;
    public StatusItem BalloonArtistHandingOut;
    public StatusItem Partying;
    public StatusItem GasLiquidIrritation;
    public StatusItem ExpellingRads;
    public StatusItem AnalyzingGenes;
    public StatusItem AnalyzingArtifact;
    public StatusItem MegaBrainTank_Pajamas_Wearing;
    public StatusItem MegaBrainTank_Pajamas_Sleeping;
    private const int NONE_OVERLAY = 0;

    public DuplicantStatusItems(ResourceSet parent)
      : base(nameof (DuplicantStatusItems), parent)
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
      int status_overlays = 2)
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
      int status_overlays = 2)
    {
      return this.Add(new StatusItem(id, name, tooltip, icon, icon_type, notification_type, allow_multiples, render_overlay, status_overlays));
    }

    private void CreateStatusItems()
    {
      Func<string, object, string> func1 = (Func<string, object, string>) ((str, data) =>
      {
        Workable workable = (Workable) data;
        if (Object.op_Inequality((Object) workable, (Object) null) && Object.op_Inequality((Object) ((Component) workable).GetComponent<KSelectable>(), (Object) null))
          str = str.Replace("{Target}", ((Component) workable).GetComponent<KSelectable>().GetName());
        return str;
      });
      Func<string, object, string> func2 = (Func<string, object, string>) ((str, data) =>
      {
        Workable workable = (Workable) data;
        if (Object.op_Inequality((Object) workable, (Object) null))
        {
          str = str.Replace("{Target}", ((Component) workable).GetComponent<KSelectable>().GetName());
          ComplexFabricatorWorkable fabricatorWorkable = workable as ComplexFabricatorWorkable;
          if (Object.op_Inequality((Object) fabricatorWorkable, (Object) null))
          {
            ComplexRecipe currentWorkingOrder = fabricatorWorkable.CurrentWorkingOrder;
            if (currentWorkingOrder != null)
              str = str.Replace("{Item}", currentWorkingOrder.FirstResult.ProperName());
          }
        }
        return str;
      });
      this.BedUnreachable = this.CreateStatusItem("BedUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.BedUnreachable.AddNotification();
      this.DailyRationLimitReached = this.CreateStatusItem("DailyRationLimitReached", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.DailyRationLimitReached.AddNotification();
      this.HoldingBreath = this.CreateStatusItem("HoldingBreath", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.Hungry = this.CreateStatusItem("Hungry", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.Unhappy = this.CreateStatusItem("Unhappy", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.Unhappy.AddNotification();
      this.NervousBreakdown = this.CreateStatusItem("NervousBreakdown", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID);
      this.NervousBreakdown.AddNotification();
      this.NoRationsAvailable = this.CreateStatusItem("NoRationsAvailable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID);
      this.PendingPacification = this.CreateStatusItem("PendingPacification", "DUPLICANTS", "status_item_pending_pacification", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.QuarantineAreaUnassigned = this.CreateStatusItem("QuarantineAreaUnassigned", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.QuarantineAreaUnassigned.AddNotification();
      this.QuarantineAreaUnreachable = this.CreateStatusItem("QuarantineAreaUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.QuarantineAreaUnreachable.AddNotification();
      this.Quarantined = this.CreateStatusItem("Quarantined", "DUPLICANTS", "status_item_quarantined", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.RationsUnreachable = this.CreateStatusItem("RationsUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.RationsUnreachable.AddNotification();
      this.RationsNotPermitted = this.CreateStatusItem("RationsNotPermitted", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.RationsNotPermitted.AddNotification();
      this.Rotten = this.CreateStatusItem("Rotten", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.Starving = this.CreateStatusItem("Starving", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID);
      this.Starving.AddNotification();
      this.Suffocating = this.CreateStatusItem("Suffocating", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, false, OverlayModes.None.ID);
      this.Suffocating.AddNotification();
      this.Tired = this.CreateStatusItem("Tired", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.Idle = this.CreateStatusItem("Idle", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.Idle.AddNotification();
      this.Pacified = this.CreateStatusItem("Pacified", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Dead = this.CreateStatusItem("Dead", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.Dead.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        Death death = (Death) data;
        return str.Replace("{Death}", death.Name);
      });
      this.MoveToSuitNotRequired = this.CreateStatusItem("MoveToSuitNotRequired", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.DroppingUnusedInventory = this.CreateStatusItem("DroppingUnusedInventory", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.MovingToSafeArea = this.CreateStatusItem("MovingToSafeArea", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.ToiletUnreachable = this.CreateStatusItem("ToiletUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.ToiletUnreachable.AddNotification();
      this.NoUsableToilets = this.CreateStatusItem("NoUsableToilets", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.NoUsableToilets.AddNotification();
      this.NoToilets = this.CreateStatusItem("NoToilets", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.NoToilets.AddNotification();
      this.BreathingO2 = this.CreateStatusItem("BreathingO2", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, status_overlays: 130);
      this.BreathingO2.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        float averageRate = Game.Instance.accumulators.GetAverageRate(((OxygenBreather) data).O2Accumulator);
        return str.Replace("{ConsumptionRate}", GameUtil.GetFormattedMass(-averageRate, GameUtil.TimeSlice.PerSecond));
      });
      this.EmittingCO2 = this.CreateStatusItem("EmittingCO2", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, status_overlays: 130);
      this.EmittingCO2.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        OxygenBreather oxygenBreather = (OxygenBreather) data;
        return str.Replace("{EmittingRate}", GameUtil.GetFormattedMass(oxygenBreather.CO2EmitRate, GameUtil.TimeSlice.PerSecond));
      });
      this.Vomiting = this.CreateStatusItem("Vomiting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.Coughing = this.CreateStatusItem("Coughing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.LowOxygen = this.CreateStatusItem("LowOxygen", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.LowOxygen.AddNotification();
      this.RedAlert = this.CreateStatusItem("RedAlert", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Dreaming = this.CreateStatusItem("Dreaming", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Sleeping = this.CreateStatusItem("Sleeping", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Sleeping.resolveTooltipCallback = (Func<string, object, string>) ((str, data) =>
      {
        if (data is SleepChore.StatesInstance)
        {
          string changeNoiseSource = ((SleepChore.StatesInstance) data).stateChangeNoiseSource;
          if (!string.IsNullOrEmpty(changeNoiseSource))
          {
            string str1 = ((string) DUPLICANTS.STATUSITEMS.SLEEPING.TOOLTIP).Replace("{Disturber}", changeNoiseSource);
            str += str1;
          }
        }
        return str;
      });
      this.SleepingInterruptedByNoise = this.CreateStatusItem("SleepingInterruptedByNoise", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.SleepingInterruptedByLight = this.CreateStatusItem("SleepingInterruptedByLight", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.SleepingInterruptedByFearOfDark = this.CreateStatusItem("SleepingInterruptedByFearOfDark", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.SleepingInterruptedByMovement = this.CreateStatusItem("SleepingInterruptedByMovement", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Eating = this.CreateStatusItem("Eating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Eating.resolveStringCallback = func1;
      this.Digging = this.CreateStatusItem("Digging", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Cleaning = this.CreateStatusItem("Cleaning", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Cleaning.resolveStringCallback = func1;
      this.PickingUp = this.CreateStatusItem("PickingUp", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.PickingUp.resolveStringCallback = func1;
      this.Mopping = this.CreateStatusItem("Mopping", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Cooking = this.CreateStatusItem("Cooking", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Cooking.resolveStringCallback = func2;
      this.Mushing = this.CreateStatusItem("Mushing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Mushing.resolveStringCallback = func2;
      this.Researching = this.CreateStatusItem("Researching", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Researching.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        TechInstance activeResearch = Research.Instance.GetActiveResearch();
        return activeResearch != null ? str.Replace("{Tech}", activeResearch.tech.Name) : str;
      });
      this.MissionControlling = this.CreateStatusItem("MissionControlling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Tinkering = this.CreateStatusItem("Tinkering", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Tinkering.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        Tinkerable tinkerable = (Tinkerable) data;
        return Object.op_Inequality((Object) tinkerable, (Object) null) ? string.Format(str, (object) tinkerable.tinkerMaterialTag.ProperName()) : str;
      });
      this.Storing = this.CreateStatusItem("Storing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Storing.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        Workable workable = (Workable) data;
        if (Object.op_Inequality((Object) workable, (Object) null) && Object.op_Inequality((Object) workable.worker, (Object) null))
        {
          KSelectable component1 = ((Component) workable).GetComponent<KSelectable>();
          if (Object.op_Implicit((Object) component1))
            str = str.Replace("{Target}", component1.GetName());
          Pickupable workCompleteData = workable.worker.workCompleteData as Pickupable;
          if (Object.op_Inequality((Object) workable.worker, (Object) null) && Object.op_Implicit((Object) workCompleteData))
          {
            KSelectable component2 = ((Component) workCompleteData).GetComponent<KSelectable>();
            if (Object.op_Implicit((Object) component2))
              str = str.Replace("{Item}", component2.GetName());
          }
        }
        return str;
      });
      this.Building = this.CreateStatusItem("Building", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Building.resolveStringCallback = func1;
      this.Equipping = this.CreateStatusItem("Equipping", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Equipping.resolveStringCallback = func1;
      this.WarmingUp = this.CreateStatusItem("WarmingUp", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.WarmingUp.resolveStringCallback = func1;
      this.GeneratingPower = this.CreateStatusItem("GeneratingPower", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.GeneratingPower.resolveStringCallback = func1;
      this.Harvesting = this.CreateStatusItem("Harvesting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Ranching = this.CreateStatusItem("Ranching", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Harvesting.resolveStringCallback = func1;
      this.Uprooting = this.CreateStatusItem("Uprooting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Uprooting.resolveStringCallback = func1;
      this.Emptying = this.CreateStatusItem("Emptying", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Emptying.resolveStringCallback = func1;
      this.Toggling = this.CreateStatusItem("Toggling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Toggling.resolveStringCallback = func1;
      this.Deconstructing = this.CreateStatusItem("Deconstructing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Deconstructing.resolveStringCallback = func1;
      this.Disinfecting = this.CreateStatusItem("Disinfecting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Disinfecting.resolveStringCallback = func1;
      this.Upgrading = this.CreateStatusItem("Upgrading", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Upgrading.resolveStringCallback = func1;
      this.Fabricating = this.CreateStatusItem("Fabricating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Fabricating.resolveStringCallback = func2;
      this.Processing = this.CreateStatusItem("Processing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Processing.resolveStringCallback = func2;
      this.Spicing = this.CreateStatusItem("Spicing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Clearing = this.CreateStatusItem("Clearing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Clearing.resolveStringCallback = func1;
      this.GeneratingPower = this.CreateStatusItem("GeneratingPower", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.GeneratingPower.resolveStringCallback = func1;
      this.Cold = this.CreateStatusItem("Cold", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.Cold.resolveTooltipCallback = (Func<string, object, string>) ((str, data) =>
      {
        str = str.Replace("{StressModification}", GameUtil.GetFormattedPercent(Db.Get().effects.Get("ColdAir").SelfModifiers[0].Value, GameUtil.TimeSlice.PerCycle));
        float dtu_s = ((ExternalTemperatureMonitor.Instance) data).temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage * 1000f;
        str = str.Replace("{currentTransferWattage}", GameUtil.GetFormattedHeatEnergyRate(dtu_s));
        AttributeInstance attributeInstance = ((ExternalTemperatureMonitor.Instance) data).attributes.Get("ThermalConductivityBarrier");
        string newValue = "<b>" + attributeInstance.GetFormattedValue() + "</b>";
        for (int index = 0; index != attributeInstance.Modifiers.Count; ++index)
        {
          AttributeModifier modifier = attributeInstance.Modifiers[index];
          newValue = newValue + "\n" + "    • " + modifier.GetDescription() + " <b>" + modifier.GetFormattedString() + "</b>";
        }
        str = str.Replace("{conductivityBarrier}", newValue);
        return str;
      });
      this.Hot = this.CreateStatusItem("Hot", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.Hot.resolveTooltipCallback = (Func<string, object, string>) ((str, data) =>
      {
        str = str.Replace("{StressModification}", GameUtil.GetFormattedPercent(Db.Get().effects.Get("WarmAir").SelfModifiers[0].Value, GameUtil.TimeSlice.PerCycle));
        float dtu_s = ((ExternalTemperatureMonitor.Instance) data).temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage * 1000f;
        str = str.Replace("{currentTransferWattage}", GameUtil.GetFormattedHeatEnergyRate(dtu_s));
        AttributeInstance attributeInstance = ((ExternalTemperatureMonitor.Instance) data).attributes.Get("ThermalConductivityBarrier");
        string newValue = "<b>" + attributeInstance.GetFormattedValue() + "</b>";
        for (int index = 0; index != attributeInstance.Modifiers.Count; ++index)
        {
          AttributeModifier modifier = attributeInstance.Modifiers[index];
          newValue = newValue + "\n" + "    • " + modifier.GetDescription() + " <b>" + modifier.GetFormattedString() + "</b>";
        }
        str = str.Replace("{conductivityBarrier}", newValue);
        return str;
      });
      this.BodyRegulatingHeating = this.CreateStatusItem("BodyRegulatingHeating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.BodyRegulatingHeating.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        WarmBlooded.StatesInstance statesInstance = (WarmBlooded.StatesInstance) data;
        return str.Replace("{TempDelta}", GameUtil.GetFormattedTemperature(statesInstance.TemperatureDelta, GameUtil.TimeSlice.PerSecond, GameUtil.TemperatureInterpretation.Relative));
      });
      this.BodyRegulatingCooling = this.CreateStatusItem("BodyRegulatingCooling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.BodyRegulatingCooling.resolveStringCallback = this.BodyRegulatingHeating.resolveStringCallback;
      this.EntombedChore = this.CreateStatusItem("EntombedChore", "DUPLICANTS", "status_item_entombed", StatusItem.IconType.Custom, NotificationType.DuplicantThreatening, false, OverlayModes.None.ID);
      this.EntombedChore.AddNotification();
      this.EarlyMorning = this.CreateStatusItem("EarlyMorning", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.NightTime = this.CreateStatusItem("NightTime", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.PoorDecor = this.CreateStatusItem("PoorDecor", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.PoorQualityOfLife = this.CreateStatusItem("PoorQualityOfLife", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.PoorFoodQuality = this.CreateStatusItem("PoorFoodQuality", (string) DUPLICANTS.STATUSITEMS.POOR_FOOD_QUALITY.NAME, (string) DUPLICANTS.STATUSITEMS.POOR_FOOD_QUALITY.TOOLTIP, "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.GoodFoodQuality = this.CreateStatusItem("GoodFoodQuality", (string) DUPLICANTS.STATUSITEMS.GOOD_FOOD_QUALITY.NAME, (string) DUPLICANTS.STATUSITEMS.GOOD_FOOD_QUALITY.TOOLTIP, "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Arting = this.CreateStatusItem("Arting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Arting.resolveStringCallback = func1;
      this.SevereWounds = this.CreateStatusItem("SevereWounds", "DUPLICANTS", "status_item_broken", StatusItem.IconType.Custom, NotificationType.Bad, false, OverlayModes.None.ID);
      this.SevereWounds.AddNotification();
      this.Incapacitated = this.CreateStatusItem("Incapacitated", "DUPLICANTS", "status_item_broken", StatusItem.IconType.Custom, NotificationType.DuplicantThreatening, false, OverlayModes.None.ID);
      this.Incapacitated.AddNotification();
      this.Incapacitated.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        IncapacitationMonitor.Instance smi = (IncapacitationMonitor.Instance) data;
        float bleedLifeTime = smi.GetBleedLifeTime(smi);
        str = str.Replace("{CauseOfIncapacitation}", smi.GetCauseOfIncapacitation().Name);
        return str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedTime(bleedLifeTime));
      });
      this.Relocating = this.CreateStatusItem("Relocating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Relocating.resolveStringCallback = func1;
      this.Fighting = this.CreateStatusItem("Fighting", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID);
      this.Fighting.AddNotification();
      this.Fleeing = this.CreateStatusItem("Fleeing", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID);
      this.Fleeing.AddNotification();
      this.Stressed = this.CreateStatusItem("Stressed", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Stressed.AddNotification();
      this.LashingOut = this.CreateStatusItem("LashingOut", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID);
      this.LashingOut.AddNotification();
      this.LowImmunity = this.CreateStatusItem("LowImmunity", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.LowImmunity.AddNotification();
      this.Studying = this.CreateStatusItem("Studying", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Socializing = this.CreateStatusItem("Socializing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID);
      this.Mingling = this.CreateStatusItem("Mingling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID);
      this.ContactWithGerms = this.CreateStatusItem("ContactWithGerms", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.Disease.ID);
      this.ContactWithGerms.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        GermExposureMonitor.ExposureStatusData exposureStatusData = (GermExposureMonitor.ExposureStatusData) data;
        string name = Db.Get().Sicknesses.Get(exposureStatusData.exposure_type.sickness_id).Name;
        str = str.Replace("{Sickness}", name);
        return str;
      });
      this.ContactWithGerms.statusItemClickCallback = (Action<object>) (data =>
      {
        GermExposureMonitor.ExposureStatusData exposureStatusData = (GermExposureMonitor.ExposureStatusData) data;
        CameraController.Instance.CameraGoTo(exposureStatusData.owner.GetLastExposurePosition(exposureStatusData.exposure_type.germ_id));
        if (!HashedString.op_Equality(OverlayScreen.Instance.mode, OverlayModes.None.ID))
          return;
        OverlayScreen.Instance.ToggleOverlay(OverlayModes.Disease.ID);
      });
      this.ExposedToGerms = this.CreateStatusItem("ExposedToGerms", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.Disease.ID);
      this.ExposedToGerms.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        GermExposureMonitor.ExposureStatusData exposureStatusData = (GermExposureMonitor.ExposureStatusData) data;
        string name = Db.Get().Sicknesses.Get(exposureStatusData.exposure_type.sickness_id).Name;
        AttributeInstance attributeInstance = Db.Get().Attributes.GermResistance.Lookup(exposureStatusData.owner.gameObject);
        string lastDiseaseSource = exposureStatusData.owner.GetLastDiseaseSource(exposureStatusData.exposure_type.germ_id);
        GermExposureMonitor.Instance smi = exposureStatusData.owner.GetSMI<GermExposureMonitor.Instance>();
        float num1 = (float) exposureStatusData.exposure_type.base_resistance + GERM_EXPOSURE.EXPOSURE_TIER_RESISTANCE_BONUSES[0];
        float totalValue = attributeInstance.GetTotalValue();
        float resistanceToExposureType = smi.GetResistanceToExposureType(exposureStatusData.exposure_type);
        float contractionChance = GermExposureMonitor.GetContractionChance(resistanceToExposureType);
        float exposureTier = smi.GetExposureTier(exposureStatusData.exposure_type.germ_id);
        float num2 = GERM_EXPOSURE.EXPOSURE_TIER_RESISTANCE_BONUSES[(int) exposureTier - 1] - GERM_EXPOSURE.EXPOSURE_TIER_RESISTANCE_BONUSES[0];
        str = str.Replace("{Severity}", (string) DUPLICANTS.STATUSITEMS.EXPOSEDTOGERMS.EXPOSURE_TIERS[(int) exposureTier - 1]);
        str = str.Replace("{Sickness}", name);
        str = str.Replace("{Source}", lastDiseaseSource);
        str = str.Replace("{Base}", GameUtil.GetFormattedSimple(num1));
        str = str.Replace("{Dupe}", GameUtil.GetFormattedSimple(totalValue));
        str = str.Replace("{Total}", GameUtil.GetFormattedSimple(resistanceToExposureType));
        str = str.Replace("{ExposureLevelBonus}", GameUtil.GetFormattedSimple(num2));
        str = str.Replace("{Chance}", GameUtil.GetFormattedPercent(contractionChance * 100f));
        return str;
      });
      this.ExposedToGerms.statusItemClickCallback = (Action<object>) (data =>
      {
        GermExposureMonitor.ExposureStatusData exposureStatusData = (GermExposureMonitor.ExposureStatusData) data;
        CameraController.Instance.CameraGoTo(exposureStatusData.owner.GetLastExposurePosition(exposureStatusData.exposure_type.germ_id));
        if (!HashedString.op_Equality(OverlayScreen.Instance.mode, OverlayModes.None.ID))
          return;
        OverlayScreen.Instance.ToggleOverlay(OverlayModes.Disease.ID);
      });
      this.LightWorkEfficiencyBonus = this.CreateStatusItem("LightWorkEfficiencyBonus", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID);
      this.LightWorkEfficiencyBonus.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        string str2 = string.Format((string) DUPLICANTS.STATUSITEMS.LIGHTWORKEFFICIENCYBONUS.NO_BUILDING_WORK_ATTRIBUTE, (object) GameUtil.AddPositiveSign(GameUtil.GetFormattedPercent(15.000001f), true));
        return string.Format(str, (object) str2);
      });
      this.LaboratoryWorkEfficiencyBonus = this.CreateStatusItem("LaboratoryWorkEfficiencyBonus", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID);
      this.LaboratoryWorkEfficiencyBonus.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        string str3 = string.Format((string) DUPLICANTS.STATUSITEMS.LABORATORYWORKEFFICIENCYBONUS.NO_BUILDING_WORK_ATTRIBUTE, (object) GameUtil.AddPositiveSign(GameUtil.GetFormattedPercent(10f), true));
        return string.Format(str, (object) str3);
      });
      this.BeingProductive = this.CreateStatusItem("BeingProductive", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.BalloonArtistPlanning = this.CreateStatusItem("BalloonArtistPlanning", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.BalloonArtistHandingOut = this.CreateStatusItem("BalloonArtistHandingOut", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.Partying = this.CreateStatusItem("Partying", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID);
      this.GasLiquidIrritation = this.CreateStatusItem("GasLiquidIrritated", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID);
      this.GasLiquidIrritation.resolveStringCallback = (Func<string, object, string>) ((str, data) => (string) (((GasLiquidExposureMonitor.Instance) data).IsMajorIrritation() ? DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.NAME_MAJOR : DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.NAME_MINOR));
      this.GasLiquidIrritation.resolveTooltipCallback = (Func<string, object, string>) ((str, data) =>
      {
        GasLiquidExposureMonitor.Instance smi = (GasLiquidExposureMonitor.Instance) data;
        string tooltip = (string) DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.TOOLTIP;
        string str4 = "";
        Effect appliedEffect = smi.sm.GetAppliedEffect(smi);
        if (appliedEffect != null)
          str4 = Effect.CreateTooltip(appliedEffect, false);
        string str5 = DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.TOOLTIP_EXPOSED.Replace("{element}", smi.CurrentlyExposedToElement().name);
        float currentExposure = smi.sm.GetCurrentExposure(smi);
        string str6 = (double) currentExposure >= 0.0 ? ((double) currentExposure <= 0.0 ? str5.Replace("{rate}", (string) DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.TOOLTIP_RATE_STAYS) : str5.Replace("{rate}", (string) DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.TOOLTIP_RATE_INCREASE)) : str5.Replace("{rate}", (string) DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.TOOLTIP_RATE_DECREASE);
        float seconds = (smi.exposure - smi.minorIrritationThreshold) / Math.Abs(smi.exposureRate);
        string str7 = DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.TOOLTIP_EXPOSURE_LEVEL.Replace("{time}", GameUtil.GetFormattedTime(seconds));
        return tooltip + "\n\n" + str4 + "\n\n" + str6 + "\n\n" + str7;
      });
      this.ExpellingRads = this.CreateStatusItem("ExpellingRads", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.AnalyzingGenes = this.CreateStatusItem("AnalyzingGenes", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID);
      this.AnalyzingArtifact = this.CreateStatusItem("AnalyzingArtifact", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID);
      this.MegaBrainTank_Pajamas_Wearing = this.CreateStatusItem("MegaBrainTank_Pajamas_Wearing", (string) DUPLICANTS.STATUSITEMS.WEARING_PAJAMAS.NAME, (string) DUPLICANTS.STATUSITEMS.WEARING_PAJAMAS.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID);
      this.MegaBrainTank_Pajamas_Wearing.resolveTooltipCallback_shouldStillCallIfDataIsNull = true;
      this.MegaBrainTank_Pajamas_Wearing.resolveTooltipCallback = (Func<string, object, string>) ((str, data) =>
      {
        string tooltip = (string) DUPLICANTS.STATUSITEMS.WEARING_PAJAMAS.TOOLTIP;
        Effect effect = Db.Get().effects.Get("SleepClinic");
        string str8 = effect == null ? "" : Effect.CreateTooltip(effect, false);
        return tooltip + "\n\n" + str8;
      });
      this.MegaBrainTank_Pajamas_Sleeping = this.CreateStatusItem("MegaBrainTank_Pajamas_Sleeping", (string) DUPLICANTS.STATUSITEMS.DREAMING.NAME, (string) DUPLICANTS.STATUSITEMS.DREAMING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID);
      this.MegaBrainTank_Pajamas_Sleeping.resolveTooltipCallback = (Func<string, object, string>) ((str, data) =>
      {
        ClinicDreamable clinicDreamable = (ClinicDreamable) data;
        return str.Replace("{time}", GameUtil.GetFormattedTime(clinicDreamable.WorkTimeRemaining));
      });
    }
  }
}
