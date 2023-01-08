// Decompiled with JetBrains decompiler
// Type: ColonyDiagnosticScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColonyDiagnosticScreen : KScreen, ISim1000ms
{
  public GameObject linePrefab;
  public static ColonyDiagnosticScreen Instance;
  private List<ColonyDiagnosticScreen.DiagnosticRow> diagnosticRows = new List<ColonyDiagnosticScreen.DiagnosticRow>();
  public GameObject header;
  public GameObject contentContainer;
  public GameObject rootIndicator;
  public MultiToggle seeAllButton;
  public static Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, string> notificationSoundsActive = new Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, string>()
  {
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening,
      "Diagnostic_Active_DuplicantThreatening"
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Bad,
      "Diagnostic_Active_Bad"
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Warning,
      "Diagnostic_Active_Warning"
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Concern,
      "Diagnostic_Active_Concern"
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Suggestion,
      "Diagnostic_Active_Suggestion"
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Tutorial,
      "Diagnostic_Active_Tutorial"
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Normal,
      ""
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Good,
      ""
    }
  };
  public static Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, string> notificationSoundsInactive = new Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, string>()
  {
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening,
      "Diagnostic_Inactive_DuplicantThreatening"
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Bad,
      "Diagnostic_Inactive_Bad"
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Warning,
      "Diagnostic_Inactive_Warning"
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Concern,
      "Diagnostic_Inactive_Concern"
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Suggestion,
      "Diagnostic_Inactive_Suggestion"
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Tutorial,
      "Diagnostic_Inactive_Tutorial"
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Normal,
      ""
    },
    {
      ColonyDiagnostic.DiagnosticResult.Opinion.Good,
      ""
    }
  };

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ColonyDiagnosticScreen.Instance = this;
    this.RefreshSingleWorld();
    Game.Instance.Subscribe(1983128072, new Action<object>(this.RefreshSingleWorld));
    this.seeAllButton.onClick += (System.Action) (() => AllDiagnosticsScreen.Instance.Show(!((Component) AllDiagnosticsScreen.Instance).gameObject.activeSelf));
  }

  protected virtual void OnForcedCleanUp()
  {
    ColonyDiagnosticScreen.Instance = (ColonyDiagnosticScreen) null;
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  private void RefreshSingleWorld(object data = null)
  {
    foreach (ColonyDiagnosticScreen.DiagnosticRow diagnosticRow in this.diagnosticRows)
    {
      diagnosticRow.OnCleanUp();
      Util.KDestroyGameObject(diagnosticRow.gameObject);
    }
    this.diagnosticRows.Clear();
    this.SpawnTrackerLines(ClusterManager.Instance.activeWorldId);
  }

  private void SpawnTrackerLines(int world)
  {
    this.AddDiagnostic<BreathabilityDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<FoodDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<StressDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<RadiationDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<ReactorDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<FloatingRocketDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<RocketFuelDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<RocketOxidizerDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<FarmDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<ToiletDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<BedDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<IdleDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<TrappedDuplicantDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<EntombedDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<PowerUseDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<BatteryDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    this.AddDiagnostic<RocketsInOrbitDiagnostic>(world, this.contentContainer, this.diagnosticRows);
    List<ColonyDiagnosticScreen.DiagnosticRow> diagnosticRowList = new List<ColonyDiagnosticScreen.DiagnosticRow>();
    foreach (ColonyDiagnosticScreen.DiagnosticRow diagnosticRow in this.diagnosticRows)
      diagnosticRowList.Add(diagnosticRow);
    diagnosticRowList.Sort((Comparison<ColonyDiagnosticScreen.DiagnosticRow>) ((a, b) => a.diagnostic.name.CompareTo(b.diagnostic.name)));
    foreach (ColonyDiagnosticScreen.DiagnosticRow diagnosticRow in diagnosticRowList)
      diagnosticRow.gameObject.transform.SetAsLastSibling();
    diagnosticRowList.Clear();
    this.seeAllButton.transform.SetAsLastSibling();
    this.RefreshAll();
  }

  private GameObject AddDiagnostic<T>(
    int worldID,
    GameObject parent,
    List<ColonyDiagnosticScreen.DiagnosticRow> parentCollection)
    where T : ColonyDiagnostic
  {
    T diagnostic = ColonyDiagnosticUtility.Instance.GetDiagnostic<T>(worldID);
    if ((object) diagnostic == null)
      return (GameObject) null;
    GameObject gameObject = Util.KInstantiateUI(this.linePrefab, parent, true);
    parentCollection.Add(new ColonyDiagnosticScreen.DiagnosticRow(worldID, gameObject, (ColonyDiagnostic) diagnostic));
    return gameObject;
  }

  public static void SetIndication(
    ColonyDiagnostic.DiagnosticResult.Opinion opinion,
    GameObject indicatorGameObject)
  {
    ((Graphic) indicatorGameObject.GetComponentInChildren<Image>()).color = ColonyDiagnosticScreen.GetDiagnosticIndicationColor(opinion);
  }

  public static Color GetDiagnosticIndicationColor(ColonyDiagnostic.DiagnosticResult.Opinion opinion)
  {
    switch (opinion)
    {
      case ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening:
      case ColonyDiagnostic.DiagnosticResult.Opinion.Bad:
      case ColonyDiagnostic.DiagnosticResult.Opinion.Warning:
        return Constants.NEGATIVE_COLOR;
      case ColonyDiagnostic.DiagnosticResult.Opinion.Concern:
        return Constants.WARNING_COLOR;
      default:
        return Color.white;
    }
  }

  public void Sim1000ms(float dt) => this.RefreshAll();

  public void RefreshAll()
  {
    string tooltipString = "";
    foreach (ColonyDiagnosticScreen.DiagnosticRow diagnosticRow in this.diagnosticRows)
    {
      if (diagnosticRow.worldID == ClusterManager.Instance.activeWorldId)
      {
        int num = (int) this.UpdateDiagnosticRow(diagnosticRow, tooltipString);
      }
    }
    ColonyDiagnosticScreen.SetIndication(ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(ClusterManager.Instance.activeWorldId), this.rootIndicator);
    ((Behaviour) this.header.GetComponent<ToolTip>()).enabled = !string.IsNullOrEmpty(tooltipString);
    this.header.GetComponent<ToolTip>().SetSimpleTooltip(tooltipString);
    ((TMP_Text) ((Component) this.seeAllButton).GetComponentInChildren<LocText>()).SetText(string.Format((string) STRINGS.UI.DIAGNOSTICS_SCREEN.SEE_ALL, (object) AllDiagnosticsScreen.Instance.GetRowCount()));
  }

  private ColonyDiagnostic.DiagnosticResult.Opinion UpdateDiagnosticRow(
    ColonyDiagnosticScreen.DiagnosticRow row,
    string tooltipString)
  {
    ColonyDiagnostic.DiagnosticResult.Opinion currentDisplayedResult = row.currentDisplayedResult;
    bool activeInHierarchy = row.gameObject.activeInHierarchy;
    if (row.diagnostic.LatestResult.opinion < ColonyDiagnostic.DiagnosticResult.Opinion.Normal)
    {
      if (!string.IsNullOrEmpty(tooltipString))
        tooltipString += "\n";
      tooltipString += row.diagnostic.LatestResult.Message;
    }
    if (ColonyDiagnosticUtility.Instance.IsDiagnosticTutorialDisabled(row.diagnostic.id))
    {
      this.SetRowActive(row, false);
    }
    else
    {
      switch (ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[row.worldID][row.diagnostic.id])
      {
        case ColonyDiagnosticUtility.DisplaySetting.Always:
          this.SetRowActive(row, true);
          break;
        case ColonyDiagnosticUtility.DisplaySetting.AlertOnly:
          this.SetRowActive(row, row.diagnostic.LatestResult.opinion < ColonyDiagnostic.DiagnosticResult.Opinion.Normal);
          break;
        case ColonyDiagnosticUtility.DisplaySetting.Never:
          this.SetRowActive(row, false);
          break;
      }
      if (row.gameObject.activeInHierarchy && (row.currentDisplayedResult < currentDisplayedResult || row.currentDisplayedResult < ColonyDiagnostic.DiagnosticResult.Opinion.Normal && !activeInHierarchy) && row.CheckAllowVisualNotification())
        row.TriggerVisualNotification();
    }
    return row.diagnostic.LatestResult.opinion;
  }

  private void SetRowActive(ColonyDiagnosticScreen.DiagnosticRow row, bool active)
  {
    if (row.gameObject.activeSelf == active)
      return;
    row.gameObject.SetActive(active);
    row.ResolveNotificationRoutine();
  }

  private class DiagnosticRow : ISim4000ms
  {
    private const float displayHistoryPeriod = 600f;
    public ColonyDiagnostic diagnostic;
    public SparkLayer sparkLayer;
    public int worldID;
    private LocText titleLabel;
    private LocText valueLabel;
    private Image indicator;
    private ToolTip tooltip;
    private MultiToggle button;
    private Image image;
    public ColonyDiagnostic.DiagnosticResult.Opinion currentDisplayedResult;
    private Vector2 defaultIndicatorSizeDelta;
    private float timeOfLastNotification;
    private const float MIN_TIME_BETWEEN_NOTIFICATIONS = 300f;
    private Coroutine activeRoutine;

    public DiagnosticRow(int worldID, GameObject gameObject, ColonyDiagnostic diagnostic)
    {
      Debug.Assert(diagnostic != null);
      HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
      this.worldID = worldID;
      this.sparkLayer = component.GetReference<SparkLayer>("SparkLayer");
      this.diagnostic = diagnostic;
      this.titleLabel = component.GetReference<LocText>("TitleLabel");
      this.valueLabel = component.GetReference<LocText>("ValueLabel");
      this.indicator = component.GetReference<Image>("Indicator");
      this.image = component.GetReference<Image>("Image");
      this.tooltip = gameObject.GetComponent<ToolTip>();
      this.gameObject = gameObject;
      ((TMP_Text) this.titleLabel).SetText(diagnostic.name);
      this.sparkLayer.colorRules.setOwnColor = false;
      if (diagnostic.tracker == null)
      {
        ((Component) this.sparkLayer.transform.parent).gameObject.SetActive(false);
      }
      else
      {
        this.sparkLayer.ClearLines();
        this.sparkLayer.NewLine(diagnostic.tracker.ChartableData(600f), diagnostic.name);
      }
      this.button = gameObject.GetComponent<MultiToggle>();
      this.button.onClick += (System.Action) (() =>
      {
        if (diagnostic.LatestResult.clickThroughTarget == null)
          CameraController.Instance.ActiveWorldStarWipe(diagnostic.worldID);
        else
          SelectTool.Instance.SelectAndFocus(diagnostic.LatestResult.clickThroughTarget.first, Object.op_Equality((Object) diagnostic.LatestResult.clickThroughTarget.second, (Object) null) ? (KSelectable) null : diagnostic.LatestResult.clickThroughTarget.second.GetComponent<KSelectable>());
      });
      this.defaultIndicatorSizeDelta = Vector2.zero;
      this.Update();
      SimAndRenderScheduler.instance.Add((object) this, true);
    }

    public void OnCleanUp() => SimAndRenderScheduler.instance.Remove((object) this);

    public void Sim4000ms(float dt) => this.Update();

    public GameObject gameObject { get; private set; }

    public void Update()
    {
      Color white = Color.white;
      Debug.Assert(this.diagnostic.LatestResult.opinion != 0, (object) string.Format("{0} criteria returned no opinion. Make sure the DiagnosticResult parameters are used or an opinion result is otherwise set in all of its criteria", (object) this.diagnostic));
      this.currentDisplayedResult = this.diagnostic.LatestResult.opinion;
      Color color = this.diagnostic.colors[this.diagnostic.LatestResult.opinion];
      if (this.diagnostic.tracker != null)
      {
        this.sparkLayer.RefreshLine(this.diagnostic.tracker.ChartableData(600f), this.diagnostic.name);
        this.sparkLayer.SetColor(color);
      }
      ((Graphic) this.indicator).color = this.diagnostic.colors[this.diagnostic.LatestResult.opinion];
      this.tooltip.SetSimpleTooltip((Util.IsNullOrWhiteSpace(this.diagnostic.LatestResult.Message) ? STRINGS.UI.COLONY_DIAGNOSTICS.GENERIC_STATUS_NORMAL.text : this.diagnostic.LatestResult.Message) + "\n\n" + STRINGS.UI.COLONY_DIAGNOSTICS.MUTE_TUTORIAL.text);
      switch (this.diagnostic.presentationSetting)
      {
        case ColonyDiagnostic.PresentationSetting.CurrentValue:
          ((TMP_Text) this.valueLabel).SetText(this.diagnostic.GetCurrentValueString());
          break;
        default:
          ((TMP_Text) this.valueLabel).SetText(this.diagnostic.GetAverageValueString());
          break;
      }
      if (!string.IsNullOrEmpty(this.diagnostic.icon))
        this.image.sprite = Assets.GetSprite(HashedString.op_Implicit(this.diagnostic.icon));
      if (Color.op_Equality(color, Constants.NEUTRAL_COLOR))
        color = Color.white;
      ((Graphic) this.titleLabel).color = color;
    }

    public bool CheckAllowVisualNotification() => (double) this.timeOfLastNotification == 0.0 || (double) GameClock.Instance.GetTime() >= (double) this.timeOfLastNotification + 300.0;

    public void TriggerVisualNotification()
    {
      if (DebugHandler.NotificationsDisabled || this.activeRoutine != null)
        return;
      this.timeOfLastNotification = GameClock.Instance.GetTime();
      KFMOD.PlayUISound(GlobalAssets.GetSound(ColonyDiagnosticScreen.notificationSoundsActive[this.currentDisplayedResult]));
      this.activeRoutine = ((MonoBehaviour) this.gameObject.GetComponent<KMonoBehaviour>()).StartCoroutine(this.VisualNotificationRoutine());
    }

    private IEnumerator VisualNotificationRoutine()
    {
      this.gameObject.GetComponentInChildren<NotificationAnimator>().Begin(false);
      RectTransform indicator = ((Graphic) this.gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Indicator")).rectTransform;
      this.defaultIndicatorSizeDelta = Vector2.zero;
      indicator.sizeDelta = this.defaultIndicatorSizeDelta;
      float bounceDuration = 3f;
      float i;
      for (i = 0.0f; (double) i < (double) bounceDuration; i += Time.unscaledDeltaTime)
      {
        indicator.sizeDelta = Vector2.op_Addition(this.defaultIndicatorSizeDelta, Vector2.op_Multiply(Vector2.one, (float) Mathf.RoundToInt(Mathf.Sin((float) (6.0 * (3.1415927410125732 * ((double) i / (double) bounceDuration)))))));
        yield return (object) 0;
      }
      for (i = 0.0f; (double) i < (double) bounceDuration; i += Time.unscaledDeltaTime)
      {
        indicator.sizeDelta = Vector2.op_Addition(this.defaultIndicatorSizeDelta, Vector2.op_Multiply(Vector2.one, (float) Mathf.RoundToInt(Mathf.Sin((float) (6.0 * (3.1415927410125732 * ((double) i / (double) bounceDuration)))))));
        yield return (object) 0;
      }
      for (i = 0.0f; (double) i < (double) bounceDuration; i += Time.unscaledDeltaTime)
      {
        indicator.sizeDelta = Vector2.op_Addition(this.defaultIndicatorSizeDelta, Vector2.op_Multiply(Vector2.one, (float) Mathf.RoundToInt(Mathf.Sin((float) (6.0 * (3.1415927410125732 * ((double) i / (double) bounceDuration)))))));
        yield return (object) 0;
      }
      this.ResolveNotificationRoutine();
    }

    public void ResolveNotificationRoutine()
    {
      ((Graphic) this.gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Indicator")).rectTransform.sizeDelta = Vector2.zero;
      ((Transform) this.gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content")).localPosition = Vector2.op_Implicit(Vector2.zero);
      this.activeRoutine = (Coroutine) null;
    }
  }
}
