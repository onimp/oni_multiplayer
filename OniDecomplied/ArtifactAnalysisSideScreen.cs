// Decompiled with JetBrains decompiler
// Type: ArtifactAnalysisSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactAnalysisSideScreen : SideScreenContent
{
  [SerializeField]
  private GameObject rowPrefab;
  private GameObject targetArtifactStation;
  [SerializeField]
  private GameObject rowContainer;
  private Dictionary<string, GameObject> rows = new Dictionary<string, GameObject>();
  private GameObject undiscoveredRow;

  public override string GetTitle() => Object.op_Inequality((Object) this.targetArtifactStation, (Object) null) ? string.Format(base.GetTitle(), (object) this.targetArtifactStation.GetProperName()) : base.GetTitle();

  public override void ClearTarget()
  {
    this.targetArtifactStation = (GameObject) null;
    base.ClearTarget();
  }

  public override bool IsValidForTarget(GameObject target) => target.GetSMI<ArtifactAnalysisStation.StatesInstance>() != null;

  private void RefreshRows()
  {
    if (Object.op_Equality((Object) this.undiscoveredRow, (Object) null))
    {
      this.undiscoveredRow = Util.KInstantiateUI(this.rowPrefab, this.rowContainer, true);
      HierarchyReferences component = this.undiscoveredRow.GetComponent<HierarchyReferences>();
      ((TMP_Text) component.GetReference<LocText>("label")).SetText((string) STRINGS.UI.UISIDESCREENS.ARTIFACTANALYSISSIDESCREEN.NO_ARTIFACTS_DISCOVERED);
      ((Component) component).GetComponent<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.UISIDESCREENS.ARTIFACTANALYSISSIDESCREEN.NO_ARTIFACTS_DISCOVERED_TOOLTIP);
      component.GetReference<Image>("icon").sprite = Assets.GetSprite(HashedString.op_Implicit("unknown"));
      ((Graphic) component.GetReference<Image>("icon")).color = Color.grey;
    }
    List<string> analyzedArtifactIds = ArtifactSelector.Instance.GetAnalyzedArtifactIDs();
    this.undiscoveredRow.SetActive(analyzedArtifactIds.Count == 0);
    foreach (string str in analyzedArtifactIds)
    {
      if (!this.rows.ContainsKey(str))
      {
        GameObject gameObject = Util.KInstantiateUI(this.rowPrefab, this.rowContainer, true);
        this.rows.Add(str, gameObject);
        GameObject artifactPrefab = Assets.GetPrefab(Tag.op_Implicit(str));
        HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
        ((TMP_Text) component.GetReference<LocText>("label")).SetText(artifactPrefab.GetProperName());
        component.GetReference<Image>("icon").sprite = Def.GetUISprite((object) artifactPrefab, str).first;
        ((Component) component).GetComponent<KButton>().onClick += (System.Action) (() => this.OpenEvent(artifactPrefab));
      }
    }
  }

  private void OpenEvent(GameObject artifactPrefab)
  {
    SimpleEvent.StatesInstance smi = GameplayEventManager.Instance.StartNewEvent(Db.Get().GameplayEvents.ArtifactReveal).smi as SimpleEvent.StatesInstance;
    smi.artifact = artifactPrefab;
    artifactPrefab.GetComponent<KPrefabID>();
    artifactPrefab.GetComponent<InfoDescription>();
    Tag tag = artifactPrefab.PrefabID();
    string str1 = "STRINGS.UI.SPACEARTIFACTS." + ((Tag) ref tag).Name.ToUpper().Replace("ARTIFACT_", "") + ".ARTIFACT";
    string str2 = string.Format("<b>{0}</b>", (object) artifactPrefab.GetProperName());
    StringEntry stringEntry;
    ref StringEntry local = ref stringEntry;
    Strings.TryGet(str1, ref local);
    if (stringEntry != null && !Util.IsNullOrWhiteSpace(stringEntry.String))
      str2 = str2 + "\n\n" + stringEntry.String;
    if (str2 != null && !Util.IsNullOrWhiteSpace(str2))
      smi.SetTextParameter("desc", str2);
    smi.ShowEventPopup();
  }

  public override void SetTarget(GameObject target)
  {
    this.targetArtifactStation = target;
    base.SetTarget(target);
    this.RefreshRows();
  }
}
