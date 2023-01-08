// Decompiled with JetBrains decompiler
// Type: FacadeSelectionPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacadeSelectionPanel : KMonoBehaviour
{
  [SerializeField]
  private GameObject togglePrefab;
  [SerializeField]
  private RectTransform toggleContainer;
  [SerializeField]
  private LayoutElement scrollRect;
  private Dictionary<string, FacadeSelectionPanel.FacadeToggle> activeFacadeToggles = new Dictionary<string, FacadeSelectionPanel.FacadeToggle>();
  private List<GameObject> pooledFacadeToggles = new List<GameObject>();
  [SerializeField]
  private KButton storeButton;
  public System.Action OnFacadeSelectionChanged;
  private string selectedBuildingDefID;
  private string _selectedFacade;
  public const string DEFAULT_FACADE_ID = "DEFAULT_FACADE";

  public string SelectedBuildingDefID => this.selectedBuildingDefID;

  public string SelectedFacade
  {
    get => this._selectedFacade;
    set
    {
      if (!(this._selectedFacade != value))
        return;
      this._selectedFacade = value;
      this.RefreshToggles();
      if (this.OnFacadeSelectionChanged == null)
        return;
      this.OnFacadeSelectionChanged();
    }
  }

  public void SetBuildingDef(string defID)
  {
    this.ClearToggles();
    this.selectedBuildingDefID = defID;
    this.SelectedFacade = "DEFAULT_FACADE";
    this.RefreshToggles();
    ((Component) this).gameObject.SetActive(Assets.GetBuildingDef(defID).AvailableFacades.Count != 0);
  }

  private void ClearToggles()
  {
    foreach (KeyValuePair<string, FacadeSelectionPanel.FacadeToggle> activeFacadeToggle in this.activeFacadeToggles)
    {
      List<GameObject> pooledFacadeToggles = this.pooledFacadeToggles;
      FacadeSelectionPanel.FacadeToggle facadeToggle = activeFacadeToggle.Value;
      GameObject gameObject = facadeToggle.gameObject;
      pooledFacadeToggles.Add(gameObject);
      facadeToggle = activeFacadeToggle.Value;
      facadeToggle.gameObject.SetActive(false);
    }
    this.activeFacadeToggles.Clear();
  }

  private void RefreshToggles()
  {
    this.AddDefaultFacadeToggle();
    foreach (string availableFacade in Assets.GetBuildingDef(this.selectedBuildingDefID).AvailableFacades)
    {
      PermitResource permitResource = Db.Get().Permits.TryGet(availableFacade);
      if (permitResource != null && permitResource.IsUnlocked())
        this.AddNewToggle(availableFacade);
    }
    foreach (KeyValuePair<string, FacadeSelectionPanel.FacadeToggle> activeFacadeToggle in this.activeFacadeToggles)
      activeFacadeToggle.Value.multiToggle.ChangeState(this.SelectedFacade == activeFacadeToggle.Key ? 1 : 0);
    this.activeFacadeToggles["DEFAULT_FACADE"].gameObject.transform.SetAsFirstSibling();
    ((Component) this.storeButton).gameObject.transform.SetAsLastSibling();
    LayoutElement component = ((Component) this.scrollRect).GetComponent<LayoutElement>();
    component.minHeight = (float) (58 * (this.activeFacadeToggles.Count <= 5 ? 1 : 2));
    component.preferredHeight = component.minHeight;
  }

  private void AddDefaultFacadeToggle() => this.AddNewToggle("DEFAULT_FACADE");

  private void AddNewToggle(string facadeID)
  {
    if (this.activeFacadeToggles.ContainsKey(facadeID))
      return;
    GameObject gameObject;
    if (this.pooledFacadeToggles.Count > 0)
    {
      gameObject = this.pooledFacadeToggles[0];
      this.pooledFacadeToggles.RemoveAt(0);
    }
    else
      gameObject = Util.KInstantiateUI(this.togglePrefab, ((Component) this.toggleContainer).gameObject, false);
    FacadeSelectionPanel.FacadeToggle newToggle = new FacadeSelectionPanel.FacadeToggle(facadeID, this.selectedBuildingDefID, gameObject);
    newToggle.multiToggle.onClick += (System.Action) (() => this.SelectedFacade = newToggle.id);
    this.activeFacadeToggles.Add(newToggle.id, newToggle);
  }

  private struct FacadeToggle
  {
    public FacadeToggle(string facadeID, string buildingPrefabID, GameObject gameObject)
    {
      this.id = facadeID;
      this.gameObject = gameObject;
      gameObject.SetActive(true);
      this.multiToggle = gameObject.GetComponent<MultiToggle>();
      this.multiToggle.onClick = (System.Action) null;
      if (facadeID != "DEFAULT_FACADE")
      {
        BuildingFacadeResource buildingFacadeResource = Db.GetBuildingFacades().Get(facadeID);
        gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("FGImage").sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(HashedString.op_Implicit(buildingFacadeResource.AnimFile)));
        this.gameObject.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ApplyBoldString(buildingFacadeResource.Name) + "\n\n" + buildingFacadeResource.Description);
      }
      else
      {
        gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("FGImage").sprite = Def.GetUISprite((object) buildingPrefabID).first;
        StringEntry stringEntry1;
        Strings.TryGet("STRINGS.BUILDINGS.PREFABS." + buildingPrefabID.ToUpperInvariant() + ".FACADES.DEFAULT_" + buildingPrefabID.ToUpperInvariant() + ".NAME", ref stringEntry1);
        StringEntry stringEntry2;
        Strings.TryGet("STRINGS.BUILDINGS.PREFABS." + buildingPrefabID.ToUpperInvariant() + ".FACADES.DEFAULT_" + buildingPrefabID.ToUpperInvariant() + ".DESC", ref stringEntry2);
        GameObject prefab = Assets.GetPrefab(Tag.op_Implicit(buildingPrefabID));
        this.gameObject.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ApplyBoldString(stringEntry1 != null ? stringEntry1.String : prefab.GetProperName()) + "\n\n" + (stringEntry2 != null ? stringEntry2.String : ""));
      }
    }

    public string id { get; set; }

    public GameObject gameObject { get; set; }

    public MultiToggle multiToggle { get; set; }
  }
}
