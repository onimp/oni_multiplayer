// Decompiled with JetBrains decompiler
// Type: MeterScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeterScreen : KScreen, IRender1000ms
{
  [SerializeField]
  private LocText currentMinions;
  public ToolTip MinionsTooltip;
  public LocText StressText;
  public ToolTip StressTooltip;
  public GameObject stressSpark;
  public LocText RationsText;
  public ToolTip RationsTooltip;
  public GameObject rationsSpark;
  public LocText SickText;
  public ToolTip SickTooltip;
  public TextStyleSetting ToolTipStyle_Header;
  public TextStyleSetting ToolTipStyle_Property;
  private bool startValuesSet;
  public MultiToggle RedAlertButton;
  public ToolTip RedAlertTooltip;
  private MeterScreen.DisplayInfo stressDisplayInfo = new MeterScreen.DisplayInfo()
  {
    selectedIndex = -1
  };
  private MeterScreen.DisplayInfo immunityDisplayInfo = new MeterScreen.DisplayInfo()
  {
    selectedIndex = -1
  };
  private List<MinionIdentity> worldLiveMinionIdentities;
  private int cachedMinionCount = -1;
  private long cachedCalories = -1;
  private Dictionary<string, float> rationsDict = new Dictionary<string, float>();

  public static MeterScreen Instance { get; private set; }

  public static void DestroyInstance() => MeterScreen.Instance = (MeterScreen) null;

  public bool StartValuesSet => this.startValuesSet;

  protected virtual void OnPrefabInit() => MeterScreen.Instance = this;

  protected virtual void OnSpawn()
  {
    this.StressTooltip.OnToolTip = new Func<string>(this.OnStressTooltip);
    this.SickTooltip.OnToolTip = new Func<string>(this.OnSickTooltip);
    this.RationsTooltip.OnToolTip = new Func<string>(this.OnRationsTooltip);
    this.RedAlertTooltip.OnToolTip = new Func<string>(this.OnRedAlertTooltip);
    this.RedAlertButton.onClick += (System.Action) (() => this.OnRedAlertClick());
    Game.Instance.Subscribe(1983128072, (Action<object>) (data => this.Refresh()));
    Game.Instance.Subscribe(1585324898, (Action<object>) (data => this.RefreshRedAlertButtonState()));
    Game.Instance.Subscribe(-1393151672, (Action<object>) (data => this.RefreshRedAlertButtonState()));
  }

  private void OnRedAlertClick()
  {
    bool on = !ClusterManager.Instance.activeWorld.AlertManager.IsRedAlertToggledOn();
    ClusterManager.Instance.activeWorld.AlertManager.ToggleRedAlert(on);
    if (on)
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open"));
    else
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
  }

  private void RefreshRedAlertButtonState() => this.RedAlertButton.ChangeState(ClusterManager.Instance.activeWorld.IsRedAlert() ? 1 : 0);

  public void Render1000ms(float dt) => this.Refresh();

  public void InitializeValues()
  {
    if (this.startValuesSet)
      return;
    this.startValuesSet = true;
    this.Refresh();
  }

  private void Refresh()
  {
    this.RefreshWorldMinionIdentities();
    this.RefreshMinions();
    this.RefreshRations();
    this.RefreshStress();
    this.RefreshSick();
    this.RefreshRedAlertButtonState();
  }

  private void RefreshWorldMinionIdentities() => this.worldLiveMinionIdentities = new List<MinionIdentity>(Components.LiveMinionIdentities.GetWorldItems(ClusterManager.Instance.activeWorldId).Where<MinionIdentity>((Func<MinionIdentity, bool>) (x => !Util.IsNullOrDestroyed((object) x))));

  private List<MinionIdentity> GetWorldMinionIdentities()
  {
    if (this.worldLiveMinionIdentities == null)
      this.RefreshWorldMinionIdentities();
    return this.worldLiveMinionIdentities;
  }

  private void RefreshMinions()
  {
    int count1 = Components.LiveMinionIdentities.Count;
    int count2 = this.GetWorldMinionIdentities().Count;
    if (count2 == this.cachedMinionCount)
      return;
    this.cachedMinionCount = count2;
    string str;
    if (DlcManager.FeatureClusterSpaceEnabled())
    {
      ClusterGridEntity component = ((Component) ClusterManager.Instance.activeWorld).GetComponent<ClusterGridEntity>();
      str = string.Format((string) UI.TOOLTIPS.METERSCREEN_POPULATION_CLUSTER, (object) component.Name, (object) count2, (object) count1);
      ((TMP_Text) this.currentMinions).text = string.Format("{0}/{1}", (object) count2, (object) count1);
    }
    else
    {
      ((TMP_Text) this.currentMinions).text = string.Format("{0}", (object) count1);
      str = string.Format((string) UI.TOOLTIPS.METERSCREEN_POPULATION, (object) count1.ToString("0"));
    }
    this.MinionsTooltip.ClearMultiStringTooltip();
    this.MinionsTooltip.AddMultiStringTooltip(str, this.ToolTipStyle_Header);
  }

  private void RefreshSick() => ((TMP_Text) this.SickText).text = this.CountSickDupes().ToString();

  private void RefreshRations()
  {
    if (Object.op_Inequality((Object) this.RationsText, (Object) null) && Object.op_Inequality((Object) RationTracker.Get(), (Object) null))
    {
      long calories = (long) RationTracker.Get().CountRations((Dictionary<string, float>) null, ClusterManager.Instance.activeWorld.worldInventory);
      if (this.cachedCalories != calories)
      {
        ((TMP_Text) this.RationsText).text = GameUtil.GetFormattedCalories((float) calories);
        this.cachedCalories = calories;
      }
    }
    this.rationsSpark.GetComponentInChildren<SparkLayer>().SetColor((double) this.cachedCalories > (double) this.GetWorldMinionIdentities().Count * 1000000.0 ? Constants.NEUTRAL_COLOR : Constants.NEGATIVE_COLOR);
    this.rationsSpark.GetComponentInChildren<LineLayer>().RefreshLine(TrackerTool.Instance.GetWorldTracker<KCalTracker>(ClusterManager.Instance.activeWorldId).ChartableData(600f), "kcal");
  }

  private IList<MinionIdentity> GetStressedMinions()
  {
    Amount stress_amount = Db.Get().Amounts.Stress;
    return (IList<MinionIdentity>) new List<MinionIdentity>((IEnumerable<MinionIdentity>) this.GetWorldMinionIdentities()).Where<MinionIdentity>((Func<MinionIdentity, bool>) (x => !Util.IsNullOrDestroyed((object) x))).OrderByDescending<MinionIdentity, float>((Func<MinionIdentity, float>) (x => stress_amount.Lookup((Component) x).value)).ToList<MinionIdentity>();
  }

  private string OnStressTooltip()
  {
    float stressInActiveWorld = GameUtil.GetMaxStressInActiveWorld();
    this.StressTooltip.ClearMultiStringTooltip();
    this.StressTooltip.AddMultiStringTooltip(string.Format((string) UI.TOOLTIPS.METERSCREEN_AVGSTRESS, (object) (Mathf.Round(stressInActiveWorld).ToString() + "%")), this.ToolTipStyle_Header);
    Amount stress = Db.Get().Amounts.Stress;
    IList<MinionIdentity> stressedMinions = this.GetStressedMinions();
    for (int index = 0; index < stressedMinions.Count; ++index)
    {
      MinionIdentity minionIdentity = stressedMinions[index];
      this.AddToolTipAmountPercentLine(this.StressTooltip, stress.Lookup((Component) minionIdentity), minionIdentity, index == this.stressDisplayInfo.selectedIndex);
    }
    return "";
  }

  private string OnSickTooltip()
  {
    int num1 = this.CountSickDupes();
    List<MinionIdentity> minionIdentities = this.GetWorldMinionIdentities();
    this.SickTooltip.ClearMultiStringTooltip();
    this.SickTooltip.AddMultiStringTooltip(string.Format((string) UI.TOOLTIPS.METERSCREEN_SICK_DUPES, (object) num1.ToString()), this.ToolTipStyle_Header);
    for (int index = 0; index < minionIdentities.Count; ++index)
    {
      MinionIdentity minionIdentity = minionIdentities[index];
      if (!Util.IsNullOrDestroyed((object) minionIdentity))
      {
        string str1 = ((Component) minionIdentity).GetComponent<KSelectable>().GetName();
        Sicknesses sicknesses = ((Component) minionIdentity).GetComponent<MinionModifiers>().sicknesses;
        if (sicknesses.IsInfected())
        {
          string str2 = str1 + " (";
          int num2 = 0;
          foreach (SicknessInstance sicknessInstance in (Modifications<Sickness, SicknessInstance>) sicknesses)
          {
            str2 = str2 + (num2 > 0 ? ", " : "") + sicknessInstance.modifier.Name;
            ++num2;
          }
          str1 = str2 + ")";
        }
        bool selected = index == this.immunityDisplayInfo.selectedIndex;
        this.AddToolTipLine(this.SickTooltip, str1, selected);
      }
    }
    return "";
  }

  private int CountSickDupes()
  {
    int num = 0;
    foreach (MinionIdentity worldMinionIdentity in this.GetWorldMinionIdentities())
    {
      if (!Util.IsNullOrDestroyed((object) worldMinionIdentity) && ((Component) worldMinionIdentity).GetComponent<MinionModifiers>().sicknesses.IsInfected())
        ++num;
    }
    return num;
  }

  private void AddToolTipLine(ToolTip tooltip, string str, bool selected)
  {
    if (selected)
      tooltip.AddMultiStringTooltip("<color=#F0B310FF>" + str + "</color>", this.ToolTipStyle_Property);
    else
      tooltip.AddMultiStringTooltip(str, this.ToolTipStyle_Property);
  }

  private void AddToolTipAmountPercentLine(
    ToolTip tooltip,
    AmountInstance amount,
    MinionIdentity id,
    bool selected)
  {
    string str = ((Component) id).GetComponent<KSelectable>().GetName() + ":  " + Mathf.Round(amount.value).ToString() + "%";
    this.AddToolTipLine(tooltip, str, selected);
  }

  private string OnRationsTooltip()
  {
    this.rationsDict.Clear();
    float calories = RationTracker.Get().CountRations(this.rationsDict, ClusterManager.Instance.activeWorld.worldInventory);
    ((TMP_Text) this.RationsText).text = GameUtil.GetFormattedCalories(calories);
    this.RationsTooltip.ClearMultiStringTooltip();
    this.RationsTooltip.AddMultiStringTooltip(string.Format((string) UI.TOOLTIPS.METERSCREEN_MEALHISTORY, (object) GameUtil.GetFormattedCalories(calories)), this.ToolTipStyle_Header);
    this.RationsTooltip.AddMultiStringTooltip("", this.ToolTipStyle_Property);
    foreach (KeyValuePair<string, float> keyValuePair in this.rationsDict.OrderByDescending<KeyValuePair<string, float>, float>((Func<KeyValuePair<string, float>, float>) (x =>
    {
      EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(x.Key);
      return x.Value * (foodInfo != null ? foodInfo.CaloriesPerUnit : -1f);
    })).ToDictionary<KeyValuePair<string, float>, string, float>((Func<KeyValuePair<string, float>, string>) (t => t.Key), (Func<KeyValuePair<string, float>, float>) (t => t.Value)))
    {
      EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(keyValuePair.Key);
      this.RationsTooltip.AddMultiStringTooltip(foodInfo != null ? string.Format("{0}: {1}", (object) foodInfo.Name, (object) GameUtil.GetFormattedCalories(keyValuePair.Value * foodInfo.CaloriesPerUnit)) : string.Format((string) UI.TOOLTIPS.METERSCREEN_INVALID_FOOD_TYPE, (object) keyValuePair.Key), this.ToolTipStyle_Property);
    }
    return "";
  }

  private string OnRedAlertTooltip()
  {
    this.RedAlertTooltip.ClearMultiStringTooltip();
    this.RedAlertTooltip.AddMultiStringTooltip((string) UI.TOOLTIPS.RED_ALERT_TITLE, this.ToolTipStyle_Header);
    this.RedAlertTooltip.AddMultiStringTooltip((string) UI.TOOLTIPS.RED_ALERT_CONTENT, this.ToolTipStyle_Property);
    return "";
  }

  private void RefreshStress()
  {
    ((TMP_Text) this.StressText).text = Mathf.Round(GameUtil.GetMaxStressInActiveWorld()).ToString();
    WorldTracker worldTracker = TrackerTool.Instance.GetWorldTracker<StressTracker>(ClusterManager.Instance.activeWorldId);
    this.stressSpark.GetComponentInChildren<SparkLayer>().SetColor((double) worldTracker.GetCurrentValue() >= (double) TUNING.STRESS.ACTING_OUT_RESET ? Constants.NEGATIVE_COLOR : Constants.NEUTRAL_COLOR);
    this.stressSpark.GetComponentInChildren<LineLayer>().RefreshLine(worldTracker.ChartableData(600f), "stressData");
  }

  public void OnClickStress(BaseEventData base_ev_data)
  {
    IList<MinionIdentity> stressedMinions = this.GetStressedMinions();
    this.UpdateDisplayInfo(base_ev_data, ref this.stressDisplayInfo, stressedMinions);
    this.OnStressTooltip();
    this.StressTooltip.forceRefresh = true;
  }

  private IList<MinionIdentity> GetSickMinions() => (IList<MinionIdentity>) this.GetWorldMinionIdentities();

  public void OnClickImmunity(BaseEventData base_ev_data)
  {
    IList<MinionIdentity> sickMinions = this.GetSickMinions();
    this.UpdateDisplayInfo(base_ev_data, ref this.immunityDisplayInfo, sickMinions);
    this.OnSickTooltip();
    this.SickTooltip.forceRefresh = true;
  }

  private void UpdateDisplayInfo(
    BaseEventData base_ev_data,
    ref MeterScreen.DisplayInfo display_info,
    IList<MinionIdentity> minions)
  {
    if (!(base_ev_data is PointerEventData pointerEventData))
      return;
    List<MinionIdentity> minionIdentities = this.GetWorldMinionIdentities();
    PointerEventData.InputButton button = pointerEventData.button;
    if (button != null)
    {
      if (button != 1)
        return;
      display_info.selectedIndex = -1;
    }
    else
    {
      if (minionIdentities.Count < display_info.selectedIndex)
        display_info.selectedIndex = -1;
      if (minionIdentities.Count <= 0)
        return;
      display_info.selectedIndex = (display_info.selectedIndex + 1) % minionIdentities.Count;
      MinionIdentity minion = minions[display_info.selectedIndex];
      SelectTool.Instance.SelectAndFocus(TransformExtensions.GetPosition(minion.transform), ((Component) minion).GetComponent<KSelectable>(), new Vector3(5f, 0.0f, 0.0f));
    }
  }

  private struct DisplayInfo
  {
    public int selectedIndex;
  }
}
