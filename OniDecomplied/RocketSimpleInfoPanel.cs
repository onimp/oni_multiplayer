// Decompiled with JetBrains decompiler
// Type: RocketSimpleInfoPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RocketSimpleInfoPanel : SimpleInfoPanel
{
  private Dictionary<string, GameObject> cargoBayLabels = new Dictionary<string, GameObject>();
  private Dictionary<string, GameObject> artifactModuleLabels = new Dictionary<string, GameObject>();

  public RocketSimpleInfoPanel(SimpleInfoScreen simpleInfoScreen)
    : base(simpleInfoScreen)
  {
  }

  public override void Refresh(
    CollapsibleDetailContentPanel rocketStatusContainer,
    GameObject selectedTarget)
  {
    if (Object.op_Equality((Object) selectedTarget, (Object) null))
    {
      this.simpleInfoRoot.StoragePanel.gameObject.SetActive(false);
    }
    else
    {
      RocketModuleCluster rocketModuleCluster = (RocketModuleCluster) null;
      Clustercraft clusterCraft = (Clustercraft) null;
      CraftModuleInterface craftModuleInterface = (CraftModuleInterface) null;
      RocketSimpleInfoPanel.GetRocketStuffFromTarget(selectedTarget, ref rocketModuleCluster, ref clusterCraft, ref craftModuleInterface);
      ((Component) rocketStatusContainer).gameObject.SetActive(Object.op_Inequality((Object) craftModuleInterface, (Object) null) || Object.op_Inequality((Object) rocketModuleCluster, (Object) null));
      if (Object.op_Inequality((Object) craftModuleInterface, (Object) null))
      {
        RocketEngineCluster engine = craftModuleInterface.GetEngine();
        string str1;
        string str2;
        if (Object.op_Inequality((Object) engine, (Object) null) && Object.op_Inequality((Object) ((Component) engine).GetComponent<HEPFuelTank>(), (Object) null))
        {
          str1 = GameUtil.GetFormattedHighEnergyParticles(craftModuleInterface.FuelPerHex);
          str2 = GameUtil.GetFormattedHighEnergyParticles(craftModuleInterface.FuelRemaining);
        }
        else
        {
          str1 = GameUtil.GetFormattedMass(craftModuleInterface.FuelPerHex);
          str2 = GameUtil.GetFormattedMass(craftModuleInterface.FuelRemaining);
        }
        string tooltip1 = (string) UI.CLUSTERMAP.ROCKETS.RANGE.TOOLTIP + "\n    • " + string.Format((string) UI.CLUSTERMAP.ROCKETS.FUEL_PER_HEX.NAME, (object) str1) + "\n    • " + (string) UI.CLUSTERMAP.ROCKETS.FUEL_REMAINING.NAME + str2 + "\n    • " + (string) UI.CLUSTERMAP.ROCKETS.OXIDIZER_REMAINING.NAME + GameUtil.GetFormattedMass(craftModuleInterface.OxidizerPowerRemaining);
        rocketStatusContainer.SetLabel("RangeRemaining", (string) UI.CLUSTERMAP.ROCKETS.RANGE.NAME + GameUtil.GetFormattedRocketRange(craftModuleInterface.Range, GameUtil.TimeSlice.None), tooltip1);
        string[] strArray = new string[7]
        {
          (string) UI.CLUSTERMAP.ROCKETS.SPEED.TOOLTIP,
          "\n    • ",
          (string) UI.CLUSTERMAP.ROCKETS.POWER_TOTAL.NAME,
          null,
          null,
          null,
          null
        };
        float num1 = craftModuleInterface.EnginePower;
        strArray[3] = num1.ToString();
        strArray[4] = "\n    • ";
        strArray[5] = (string) UI.CLUSTERMAP.ROCKETS.BURDEN_TOTAL.NAME;
        num1 = craftModuleInterface.TotalBurden;
        strArray[6] = num1.ToString();
        string tooltip2 = string.Concat(strArray);
        rocketStatusContainer.SetLabel("Speed", (string) UI.CLUSTERMAP.ROCKETS.SPEED.NAME + GameUtil.GetFormattedRocketRange(craftModuleInterface.Speed, GameUtil.TimeSlice.PerCycle), tooltip2);
        if (Object.op_Inequality((Object) craftModuleInterface.GetEngine(), (Object) null))
        {
          string tooltip3 = string.Format((string) UI.CLUSTERMAP.ROCKETS.MAX_HEIGHT.TOOLTIP, (object) ((Component) craftModuleInterface.GetEngine()).GetProperName(), (object) craftModuleInterface.MaxHeight.ToString());
          rocketStatusContainer.SetLabel("MaxHeight", string.Format((string) UI.CLUSTERMAP.ROCKETS.MAX_HEIGHT.NAME, (object) craftModuleInterface.RocketHeight.ToString(), (object) craftModuleInterface.MaxHeight.ToString()), tooltip3);
        }
        rocketStatusContainer.SetLabel("RocketSpacer2", "", "");
        if (Object.op_Inequality((Object) clusterCraft, (Object) null))
        {
          foreach (KeyValuePair<string, GameObject> artifactModuleLabel in this.artifactModuleLabels)
            artifactModuleLabel.Value.SetActive(false);
          int num2 = 0;
          foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) clusterCraft.ModuleInterface.ClusterModules)
          {
            ArtifactModule component = ((Component) clusterModule.Get()).GetComponent<ArtifactModule>();
            if (Object.op_Inequality((Object) component, (Object) null))
            {
              GameObject storageLabel = this.simpleInfoRoot.AddOrGetStorageLabel(this.artifactModuleLabels, ((Component) rocketStatusContainer).gameObject, "artifactModule_" + num2.ToString());
              ++num2;
              string str3 = !Object.op_Inequality((Object) component.Occupant, (Object) null) ? string.Format("{0}: {1}", (object) ((Component) component).GetProperName(), (object) UI.CLUSTERMAP.ROCKETS.ARTIFACT_MODULE.EMPTY) : ((Component) component).GetProperName() + ": " + component.Occupant.GetProperName();
              ((TMP_Text) storageLabel.GetComponentInChildren<LocText>()).text = str3;
              storageLabel.SetActive(true);
            }
          }
          List<CargoBayCluster> allCargoBays = clusterCraft.GetAllCargoBays();
          if ((allCargoBays == null ? 0 : (allCargoBays.Count > 0 ? 1 : 0)) != 0)
          {
            foreach (KeyValuePair<string, GameObject> cargoBayLabel in this.cargoBayLabels)
              cargoBayLabel.Value.SetActive(false);
            ListPool<Tuple<string, TextStyleSetting>, SimpleInfoScreen>.PooledList pooledList = ListPool<Tuple<string, TextStyleSetting>, SimpleInfoScreen>.Allocate();
            int num3 = 0;
            foreach (CargoBayCluster cargoBayCluster in allCargoBays)
            {
              ((List<Tuple<string, TextStyleSetting>>) pooledList).Clear();
              GameObject storageLabel = this.simpleInfoRoot.AddOrGetStorageLabel(this.cargoBayLabels, ((Component) rocketStatusContainer).gameObject, "cargoBay_" + num3.ToString());
              Storage storage = cargoBayCluster.storage;
              string str4 = string.Format("{0}: {1}/{2}", (object) ((Component) ((Component) cargoBayCluster).GetComponent<KPrefabID>()).GetProperName(), (object) GameUtil.GetFormattedMass(storage.MassStored()), (object) GameUtil.GetFormattedMass(storage.capacityKg));
              foreach (GameObject gameObject in storage.GetItems())
              {
                KPrefabID component1 = gameObject.GetComponent<KPrefabID>();
                PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
                string str5 = string.Format("{0} : {1}", (object) ((Component) component1).GetProperName(), (object) GameUtil.GetFormattedMass(component2.Mass));
                ((List<Tuple<string, TextStyleSetting>>) pooledList).Add(new Tuple<string, TextStyleSetting>(str5, PluginAssets.Instance.defaultTextStyleSetting));
              }
              ++num3;
              ((TMP_Text) storageLabel.GetComponentInChildren<LocText>()).text = str4;
              string str6 = "";
              for (int index = 0; index < ((List<Tuple<string, TextStyleSetting>>) pooledList).Count; ++index)
              {
                str6 += ((List<Tuple<string, TextStyleSetting>>) pooledList)[index].first;
                if (index != ((List<Tuple<string, TextStyleSetting>>) pooledList).Count - 1)
                  str6 += "\n";
              }
              storageLabel.GetComponentInChildren<ToolTip>().SetSimpleTooltip(str6);
            }
            pooledList.Recycle();
          }
        }
      }
      if (Object.op_Inequality((Object) rocketModuleCluster, (Object) null))
      {
        rocketStatusContainer.SetLabel("ModuleStats", (string) UI.CLUSTERMAP.ROCKETS.MODULE_STATS.NAME + selectedTarget.GetProperName(), (string) UI.CLUSTERMAP.ROCKETS.MODULE_STATS.TOOLTIP);
        float burden = rocketModuleCluster.performanceStats.Burden;
        float enginePower = rocketModuleCluster.performanceStats.EnginePower;
        if ((double) burden != 0.0)
          rocketStatusContainer.SetLabel("LocalBurden", "    • " + (string) UI.CLUSTERMAP.ROCKETS.BURDEN_MODULE.NAME + burden.ToString(), string.Format((string) UI.CLUSTERMAP.ROCKETS.BURDEN_MODULE.TOOLTIP, (object) burden));
        if ((double) enginePower != 0.0)
          rocketStatusContainer.SetLabel("LocalPower", "    • " + (string) UI.CLUSTERMAP.ROCKETS.POWER_MODULE.NAME + enginePower.ToString(), string.Format((string) UI.CLUSTERMAP.ROCKETS.POWER_MODULE.TOOLTIP, (object) enginePower));
      }
      rocketStatusContainer.Commit();
    }
  }

  public static void GetRocketStuffFromTarget(
    GameObject selectedTarget,
    ref RocketModuleCluster rocketModuleCluster,
    ref Clustercraft clusterCraft,
    ref CraftModuleInterface craftModuleInterface)
  {
    rocketModuleCluster = selectedTarget.GetComponent<RocketModuleCluster>();
    clusterCraft = selectedTarget.GetComponent<Clustercraft>();
    craftModuleInterface = (CraftModuleInterface) null;
    if (Object.op_Inequality((Object) rocketModuleCluster, (Object) null))
    {
      craftModuleInterface = rocketModuleCluster.CraftInterface;
      if (!Object.op_Equality((Object) clusterCraft, (Object) null))
        return;
      clusterCraft = ((Component) craftModuleInterface).GetComponent<Clustercraft>();
    }
    else
    {
      if (!Object.op_Inequality((Object) clusterCraft, (Object) null))
        return;
      craftModuleInterface = clusterCraft.ModuleInterface;
    }
  }
}
