// Decompiled with JetBrains decompiler
// Type: ArtableSelectionSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtableSelectionSideScreen : SideScreenContent
{
  private Artable target;
  public KButton applyButton;
  public KButton clearButton;
  public GameObject stateButtonPrefab;
  private Dictionary<string, MultiToggle> buttons = new Dictionary<string, MultiToggle>();
  [SerializeField]
  private RectTransform scrollTransoform;
  private string selectedStage = "";
  private const int INVALID_SUBSCRIPTION = -1;
  private int workCompleteSub = -1;
  [SerializeField]
  private RectTransform buttonContainer;

  public override bool IsValidForTarget(GameObject target)
  {
    Artable component = target.GetComponent<Artable>();
    return !Object.op_Equality((Object) component, (Object) null) && !(component.CurrentStage == "Default");
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.applyButton.onClick += (System.Action) (() =>
    {
      this.target.SetUserChosenTargetState(this.selectedStage);
      SelectTool.Instance.Select((KSelectable) null, true);
    });
    this.clearButton.onClick += (System.Action) (() =>
    {
      this.selectedStage = "";
      this.target.SetDefault();
      SelectTool.Instance.Select((KSelectable) null, true);
    });
  }

  public override void SetTarget(GameObject target)
  {
    if (this.workCompleteSub != -1)
    {
      KMonoBehaviourExtensions.Unsubscribe(target, this.workCompleteSub);
      this.workCompleteSub = -1;
    }
    base.SetTarget(target);
    this.target = target.GetComponent<Artable>();
    this.workCompleteSub = KMonoBehaviourExtensions.Subscribe(target, -2011693419, new Action<object>(this.OnRefreshTarget));
    this.OnRefreshTarget();
  }

  public override void ClearTarget()
  {
    this.target.Unsubscribe(-2011693419);
    this.workCompleteSub = -1;
    base.ClearTarget();
  }

  private void OnRefreshTarget(object data = null)
  {
    this.GenerateStateButtons();
    this.selectedStage = this.target.CurrentStage;
    this.RefreshButtons();
  }

  public void GenerateStateButtons()
  {
    foreach (KeyValuePair<string, MultiToggle> button in this.buttons)
      Util.KDestroyGameObject(((Component) button.Value).gameObject);
    this.buttons.Clear();
    foreach (ArtableStage prefabStage in Db.GetArtableStages().GetPrefabStages(((Component) this.target).GetComponent<KPrefabID>().PrefabID()))
    {
      if (!(prefabStage.id == "Default"))
      {
        GameObject gameObject = Util.KInstantiateUI(this.stateButtonPrefab, ((Component) this.buttonContainer).gameObject, true);
        PermitPresentationInfo presentationInfo = PermitItems.GetPermitPresentationInfo(prefabStage.PermitId);
        Sprite sprite = presentationInfo.sprite;
        MultiToggle component = gameObject.GetComponent<MultiToggle>();
        ((Component) component).GetComponent<ToolTip>().SetSimpleTooltip(presentationInfo.name);
        ((Component) component).GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = sprite;
        this.buttons.Add(prefabStage.id, component);
      }
    }
  }

  private void RefreshButtons()
  {
    List<ArtableStage> prefabStages = Db.GetArtableStages().GetPrefabStages(((Component) this.target).GetComponent<KPrefabID>().PrefabID());
    ArtableStage artableStage = prefabStages.Find((Predicate<ArtableStage>) (match => match.id == this.target.CurrentStage));
    int num = 0;
    foreach (KeyValuePair<string, MultiToggle> button in this.buttons)
    {
      KeyValuePair<string, MultiToggle> kvp = button;
      ArtableStage stage = prefabStages.Find((Predicate<ArtableStage>) (match => match.id == kvp.Key));
      if (stage != null && artableStage != null && stage.statusItem.StatusType != artableStage.statusItem.StatusType)
        ((Component) kvp.Value).gameObject.SetActive(false);
      else if (!stage.IsUnlocked())
      {
        ((Component) kvp.Value).gameObject.SetActive(false);
      }
      else
      {
        ++num;
        ((Component) kvp.Value).gameObject.SetActive(true);
        kvp.Value.ChangeState(this.selectedStage == kvp.Key ? 1 : 0);
        kvp.Value.onClick += (System.Action) (() =>
        {
          this.selectedStage = stage.id;
          this.RefreshButtons();
        });
      }
    }
    ((Component) this.scrollTransoform).GetComponent<LayoutElement>().preferredHeight = num > 3 ? 200f : 100f;
  }
}
