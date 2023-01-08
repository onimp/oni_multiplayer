// Decompiled with JetBrains decompiler
// Type: ColonyDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public abstract class ColonyDiagnostic : ISim4000ms
{
  public string name;
  public string id;
  public string icon = "icon_errand_operate";
  private Dictionary<string, DiagnosticCriterion> criteria = new Dictionary<string, DiagnosticCriterion>();
  public ColonyDiagnostic.PresentationSetting presentationSetting;
  private ColonyDiagnostic.DiagnosticResult latestResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.NO_DATA);
  public Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, Color> colors = new Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, Color>();
  public Tracker tracker;
  protected float trackerSampleCountSeconds = 4f;

  public ColonyDiagnostic(int worldID, string name)
  {
    this.worldID = worldID;
    this.name = name;
    this.id = this.GetType().Name;
    this.colors = new Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, Color>();
    this.colors.Add(ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening, Constants.NEGATIVE_COLOR);
    this.colors.Add(ColonyDiagnostic.DiagnosticResult.Opinion.Bad, Constants.NEGATIVE_COLOR);
    this.colors.Add(ColonyDiagnostic.DiagnosticResult.Opinion.Warning, Constants.NEGATIVE_COLOR);
    this.colors.Add(ColonyDiagnostic.DiagnosticResult.Opinion.Concern, Constants.WARNING_COLOR);
    this.colors.Add(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, Constants.NEUTRAL_COLOR);
    this.colors.Add(ColonyDiagnostic.DiagnosticResult.Opinion.Suggestion, Constants.NEUTRAL_COLOR);
    this.colors.Add(ColonyDiagnostic.DiagnosticResult.Opinion.Tutorial, Constants.NEUTRAL_COLOR);
    this.colors.Add(ColonyDiagnostic.DiagnosticResult.Opinion.Good, Constants.POSITIVE_COLOR);
    SimAndRenderScheduler.instance.Add((object) this, true);
  }

  public int worldID { get; protected set; }

  public virtual string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public void OnCleanUp() => SimAndRenderScheduler.instance.Remove((object) this);

  public void Sim4000ms(float dt) => this.SetResult(ColonyDiagnosticUtility.IgnoreFirstUpdate ? ColonyDiagnosticUtility.NoDataResult : this.Evaluate());

  public DiagnosticCriterion[] GetCriteria()
  {
    DiagnosticCriterion[] array = new DiagnosticCriterion[this.criteria.Values.Count];
    this.criteria.Values.CopyTo(array, 0);
    return array;
  }

  public ColonyDiagnostic.DiagnosticResult LatestResult
  {
    get => this.latestResult;
    private set => this.latestResult = value;
  }

  public virtual string GetAverageValueString() => this.tracker != null ? this.tracker.FormatValueString(Mathf.Round(this.tracker.GetAverageValue(this.trackerSampleCountSeconds))) : "";

  public virtual string GetCurrentValueString() => "";

  protected void AddCriterion(string id, DiagnosticCriterion criterion)
  {
    if (this.criteria.ContainsKey(id))
      return;
    criterion.SetID(id);
    this.criteria.Add(id, criterion);
  }

  public virtual ColonyDiagnostic.DiagnosticResult Evaluate()
  {
    ColonyDiagnostic.DiagnosticResult diagnosticResult1 = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, "");
    foreach (KeyValuePair<string, DiagnosticCriterion> criterion in this.criteria)
    {
      if (ColonyDiagnosticUtility.Instance.IsCriteriaEnabled(this.worldID, this.id, criterion.Key))
      {
        ColonyDiagnostic.DiagnosticResult diagnosticResult2 = criterion.Value.Evaluate();
        if (diagnosticResult2.opinion < diagnosticResult1.opinion)
        {
          diagnosticResult1.opinion = diagnosticResult2.opinion;
          diagnosticResult1.Message = diagnosticResult2.Message;
          diagnosticResult1.clickThroughTarget = diagnosticResult2.clickThroughTarget;
        }
      }
    }
    return diagnosticResult1;
  }

  public void SetResult(ColonyDiagnostic.DiagnosticResult result) => this.LatestResult = result;

  public enum PresentationSetting
  {
    AverageValue,
    CurrentValue,
  }

  public struct DiagnosticResult
  {
    public ColonyDiagnostic.DiagnosticResult.Opinion opinion;
    public Tuple<Vector3, GameObject> clickThroughTarget;
    private string message;

    public DiagnosticResult(
      ColonyDiagnostic.DiagnosticResult.Opinion opinion,
      string message,
      Tuple<Vector3, GameObject> clickThroughTarget = null)
    {
      this.message = message;
      this.opinion = opinion;
      this.clickThroughTarget = (Tuple<Vector3, GameObject>) null;
    }

    public string Message
    {
      set => this.message = value;
      get
      {
        string message;
        switch (this.opinion)
        {
          case ColonyDiagnostic.DiagnosticResult.Opinion.Bad:
            message = "<color=" + Constants.NEGATIVE_COLOR_STR + ">" + this.message + "</color>";
            break;
          case ColonyDiagnostic.DiagnosticResult.Opinion.Warning:
            message = "<color=" + Constants.NEGATIVE_COLOR_STR + ">" + this.message + "</color>";
            break;
          case ColonyDiagnostic.DiagnosticResult.Opinion.Concern:
            message = "<color=" + Constants.WARNING_COLOR_STR + ">" + this.message + "</color>";
            break;
          case ColonyDiagnostic.DiagnosticResult.Opinion.Suggestion:
          case ColonyDiagnostic.DiagnosticResult.Opinion.Normal:
            message = "<color=" + Constants.WHITE_COLOR_STR + ">" + this.message + "</color>";
            break;
          case ColonyDiagnostic.DiagnosticResult.Opinion.Good:
            message = "<color=" + Constants.POSITIVE_COLOR_STR + ">" + this.message + "</color>";
            break;
          default:
            message = this.message;
            break;
        }
        return message;
      }
    }

    public enum Opinion
    {
      Unset,
      DuplicantThreatening,
      Bad,
      Warning,
      Concern,
      Suggestion,
      Tutorial,
      Normal,
      Good,
    }
  }
}
