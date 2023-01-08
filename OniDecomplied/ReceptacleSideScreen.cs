// Decompiled with JetBrains decompiler
// Type: ReceptacleSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReceptacleSideScreen : SideScreenContent, IRender1000ms
{
  [SerializeField]
  protected KButton requestSelectedEntityBtn;
  [SerializeField]
  private string requestStringDeposit;
  [SerializeField]
  private string requestStringCancelDeposit;
  [SerializeField]
  private string requestStringRemove;
  [SerializeField]
  private string requestStringCancelRemove;
  public GameObject activeEntityContainer;
  public GameObject nothingDiscoveredContainer;
  [SerializeField]
  protected LocText descriptionLabel;
  protected Dictionary<SingleEntityReceptacle, int> entityPreviousSelectionMap = new Dictionary<SingleEntityReceptacle, int>();
  [SerializeField]
  private string subtitleStringSelect;
  [SerializeField]
  private string subtitleStringSelectDescription;
  [SerializeField]
  private string subtitleStringAwaitingSelection;
  [SerializeField]
  private string subtitleStringAwaitingDelivery;
  [SerializeField]
  private string subtitleStringEntityDeposited;
  [SerializeField]
  private string subtitleStringAwaitingRemoval;
  [SerializeField]
  private LocText subtitleLabel;
  [SerializeField]
  private List<DescriptorPanel> descriptorPanels;
  public Material defaultMaterial;
  public Material desaturatedMaterial;
  [SerializeField]
  private GameObject requestObjectList;
  [SerializeField]
  private GameObject requestObjectListContainer;
  [SerializeField]
  private GameObject scrollBarContainer;
  [SerializeField]
  private GameObject entityToggle;
  [SerializeField]
  private Sprite buttonSelectedBG;
  [SerializeField]
  private Sprite buttonNormalBG;
  [SerializeField]
  private Sprite elementPlaceholderSpr;
  [SerializeField]
  private bool hideUndiscoveredEntities;
  protected ReceptacleToggle selectedEntityToggle;
  protected SingleEntityReceptacle targetReceptacle;
  protected Tag selectedDepositObjectTag;
  protected Tag selectedDepositObjectAdditionalTag;
  protected Dictionary<ReceptacleToggle, ReceptacleSideScreen.SelectableEntity> depositObjectMap;
  protected List<ReceptacleToggle> entityToggles = new List<ReceptacleToggle>();
  private int onObjectDestroyedHandle = -1;
  private int onOccupantValidChangedHandle = -1;
  private int onStorageChangedHandle = -1;

  public override string GetTitle() => Object.op_Equality((Object) this.targetReceptacle, (Object) null) ? ((object) Strings.Get(this.titleKey)).ToString().Replace("{0}", "") : string.Format(StringEntry.op_Implicit(Strings.Get(this.titleKey)), (object) ((Component) this.targetReceptacle).GetProperName());

  public void Initialize(SingleEntityReceptacle target)
  {
    if (Object.op_Equality((Object) target, (Object) null))
    {
      Debug.LogError((object) "SingleObjectReceptacle provided was null.");
    }
    else
    {
      this.targetReceptacle = target;
      ((Component) this).gameObject.SetActive(true);
      this.depositObjectMap = new Dictionary<ReceptacleToggle, ReceptacleSideScreen.SelectableEntity>();
      this.entityToggles.ForEach((Action<ReceptacleToggle>) (rbi => Object.Destroy((Object) ((Component) rbi).gameObject)));
      this.entityToggles.Clear();
      foreach (Tag depositObjectTag in (IEnumerable<Tag>) this.targetReceptacle.possibleDepositObjectTags)
      {
        List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(depositObjectTag);
        int count = prefabsWithTag.Count;
        List<IHasSortOrder> hasSortOrderList = new List<IHasSortOrder>();
        foreach (GameObject candidate in prefabsWithTag)
        {
          if (!this.targetReceptacle.IsValidEntity(candidate))
          {
            --count;
          }
          else
          {
            IHasSortOrder component = candidate.GetComponent<IHasSortOrder>();
            if (component != null)
              hasSortOrderList.Add(component);
          }
        }
        Debug.Assert(hasSortOrderList.Count == count, (object) "Not all entities in this receptacle implement IHasSortOrder!");
        hasSortOrderList.Sort((Comparison<IHasSortOrder>) ((a, b) => a.sortOrder - b.sortOrder));
        foreach (IHasSortOrder hasSortOrder in hasSortOrderList)
        {
          GameObject gameObject1 = ((Component) (hasSortOrder as MonoBehaviour)).gameObject;
          GameObject gameObject2 = Util.KInstantiateUI(this.entityToggle, this.requestObjectList, false);
          gameObject2.SetActive(true);
          ReceptacleToggle newToggle = gameObject2.GetComponent<ReceptacleToggle>();
          IReceptacleDirection component = gameObject1.GetComponent<IReceptacleDirection>();
          string properName = gameObject1.GetProperName();
          ((TMP_Text) newToggle.title).text = properName;
          Sprite sprite = this.GetEntityIcon(gameObject1.PrefabID());
          if (Object.op_Equality((Object) sprite, (Object) null))
            sprite = this.elementPlaceholderSpr;
          newToggle.image.sprite = sprite;
          newToggle.toggle.onClick += (System.Action) (() => this.ToggleClicked(newToggle));
          // ISSUE: method pointer
          newToggle.toggle.onPointerEnter += new KToggle.PointerEvent((object) this, __methodptr(\u003CInitialize\u003Eb__38_3));
          this.depositObjectMap.Add(newToggle, new ReceptacleSideScreen.SelectableEntity()
          {
            tag = gameObject1.PrefabID(),
            direction = component != null ? component.Direction : SingleEntityReceptacle.ReceptacleDirection.Top,
            asset = gameObject1
          });
          this.entityToggles.Add(newToggle);
        }
      }
      this.RestoreSelectionFromOccupant();
      this.selectedEntityToggle = (ReceptacleToggle) null;
      if (this.entityToggles.Count > 0)
      {
        if (this.entityPreviousSelectionMap.ContainsKey(this.targetReceptacle))
        {
          this.ToggleClicked(this.entityToggles[this.entityPreviousSelectionMap[this.targetReceptacle]]);
        }
        else
        {
          ((TMP_Text) this.subtitleLabel).SetText(((object) Strings.Get(this.subtitleStringSelect)).ToString());
          this.requestSelectedEntityBtn.isInteractable = false;
          ((TMP_Text) this.descriptionLabel).SetText(((object) Strings.Get(this.subtitleStringSelectDescription)).ToString());
          this.HideAllDescriptorPanels();
        }
      }
      this.onStorageChangedHandle = KMonoBehaviourExtensions.Subscribe(((Component) this.targetReceptacle).gameObject, -1697596308, new Action<object>(this.CheckAmountsAndUpdate));
      this.onOccupantValidChangedHandle = KMonoBehaviourExtensions.Subscribe(((Component) this.targetReceptacle).gameObject, -1820564715, new Action<object>(this.OnOccupantValidChanged));
      this.UpdateState((object) null);
      SimAndRenderScheduler.instance.Add((object) this, false);
    }
  }

  protected virtual void UpdateState(object data)
  {
    this.requestSelectedEntityBtn.ClearOnClick();
    if (Object.op_Equality((Object) this.targetReceptacle, (Object) null))
      return;
    if (this.CheckReceptacleOccupied())
    {
      Uprootable uprootable = this.targetReceptacle.Occupant.GetComponent<Uprootable>();
      if (Object.op_Inequality((Object) uprootable, (Object) null) && uprootable.IsMarkedForUproot)
      {
        this.requestSelectedEntityBtn.onClick += (System.Action) (() =>
        {
          uprootable.ForceCancelUproot();
          this.UpdateState((object) null);
        });
        ((TMP_Text) ((Component) this.requestSelectedEntityBtn).GetComponentInChildren<LocText>()).text = ((object) Strings.Get(this.requestStringCancelRemove)).ToString();
        ((TMP_Text) this.subtitleLabel).SetText(string.Format(((object) Strings.Get(this.subtitleStringAwaitingRemoval)).ToString(), (object) this.targetReceptacle.Occupant.GetProperName()));
      }
      else
      {
        this.requestSelectedEntityBtn.onClick += (System.Action) (() =>
        {
          this.targetReceptacle.OrderRemoveOccupant();
          this.UpdateState((object) null);
        });
        ((TMP_Text) ((Component) this.requestSelectedEntityBtn).GetComponentInChildren<LocText>()).text = ((object) Strings.Get(this.requestStringRemove)).ToString();
        ((TMP_Text) this.subtitleLabel).SetText(string.Format(((object) Strings.Get(this.subtitleStringEntityDeposited)).ToString(), (object) this.targetReceptacle.Occupant.GetProperName()));
      }
      this.requestSelectedEntityBtn.isInteractable = true;
      this.ToggleObjectPicker(false);
      this.ConfigureActiveEntity(((Component) this.targetReceptacle.Occupant.GetComponent<KSelectable>()).PrefabID());
      this.SetResultDescriptions(this.targetReceptacle.Occupant);
    }
    else if (this.targetReceptacle.GetActiveRequest != null)
    {
      this.requestSelectedEntityBtn.onClick += (System.Action) (() =>
      {
        this.targetReceptacle.CancelActiveRequest();
        this.ClearSelection();
        this.UpdateAvailableAmounts((object) null);
        this.UpdateState((object) null);
      });
      ((TMP_Text) ((Component) this.requestSelectedEntityBtn).GetComponentInChildren<LocText>()).text = ((object) Strings.Get(this.requestStringCancelDeposit)).ToString();
      this.requestSelectedEntityBtn.isInteractable = true;
      this.ToggleObjectPicker(false);
      this.ConfigureActiveEntity(this.targetReceptacle.GetActiveRequest.tagsFirst);
      GameObject prefab = Assets.GetPrefab(this.targetReceptacle.GetActiveRequest.tagsFirst);
      if (Object.op_Inequality((Object) prefab, (Object) null))
      {
        ((TMP_Text) this.subtitleLabel).SetText(string.Format(((object) Strings.Get(this.subtitleStringAwaitingDelivery)).ToString(), (object) prefab.GetProperName()));
        this.SetResultDescriptions(prefab);
      }
    }
    else if (Object.op_Inequality((Object) this.selectedEntityToggle, (Object) null))
    {
      this.requestSelectedEntityBtn.onClick += (System.Action) (() =>
      {
        this.targetReceptacle.CreateOrder(this.selectedDepositObjectTag, this.selectedDepositObjectAdditionalTag);
        this.UpdateAvailableAmounts((object) null);
        this.UpdateState((object) null);
      });
      ((TMP_Text) ((Component) this.requestSelectedEntityBtn).GetComponentInChildren<LocText>()).text = ((object) Strings.Get(this.requestStringDeposit)).ToString();
      this.targetReceptacle.SetPreview(this.depositObjectMap[this.selectedEntityToggle].tag);
      bool flag = this.CanDepositEntity(this.depositObjectMap[this.selectedEntityToggle]);
      this.requestSelectedEntityBtn.isInteractable = flag;
      this.SetImageToggleState(this.selectedEntityToggle.toggle, flag ? (ImageToggleState.State) 2 : (ImageToggleState.State) 3);
      this.ToggleObjectPicker(true);
      GameObject prefab = Assets.GetPrefab(this.selectedDepositObjectTag);
      if (Object.op_Inequality((Object) prefab, (Object) null))
      {
        ((TMP_Text) this.subtitleLabel).SetText(string.Format(((object) Strings.Get(this.subtitleStringAwaitingSelection)).ToString(), (object) prefab.GetProperName()));
        this.SetResultDescriptions(prefab);
      }
    }
    else
    {
      ((TMP_Text) ((Component) this.requestSelectedEntityBtn).GetComponentInChildren<LocText>()).text = ((object) Strings.Get(this.requestStringDeposit)).ToString();
      this.requestSelectedEntityBtn.isInteractable = false;
      this.ToggleObjectPicker(true);
    }
    this.UpdateAvailableAmounts((object) null);
    this.UpdateListeners();
  }

  private void UpdateListeners()
  {
    if (this.CheckReceptacleOccupied())
    {
      if (this.onObjectDestroyedHandle != -1)
        return;
      this.onObjectDestroyedHandle = KMonoBehaviourExtensions.Subscribe(this.targetReceptacle.Occupant.gameObject, 1969584890, (Action<object>) (d => this.UpdateState((object) null)));
    }
    else
    {
      if (this.onObjectDestroyedHandle == -1)
        return;
      this.onObjectDestroyedHandle = -1;
    }
  }

  private void OnOccupantValidChanged(object obj)
  {
    if (Object.op_Equality((Object) this.targetReceptacle, (Object) null) || this.CheckReceptacleOccupied() || this.targetReceptacle.GetActiveRequest == null)
      return;
    bool flag = false;
    ReceptacleSideScreen.SelectableEntity entity;
    if (this.depositObjectMap.TryGetValue(this.selectedEntityToggle, out entity))
      flag = this.CanDepositEntity(entity);
    if (flag)
      return;
    this.targetReceptacle.CancelActiveRequest();
    this.ClearSelection();
    this.UpdateState((object) null);
    this.UpdateAvailableAmounts((object) null);
  }

  private bool CanDepositEntity(ReceptacleSideScreen.SelectableEntity entity) => this.ValidRotationForDeposit(entity.direction) && (!this.RequiresAvailableAmountToDeposit() || (double) this.GetAvailableAmount(entity.tag) > 0.0) && this.AdditionalCanDepositTest();

  protected virtual bool AdditionalCanDepositTest() => true;

  protected virtual bool RequiresAvailableAmountToDeposit() => true;

  private void ClearSelection()
  {
    foreach (KeyValuePair<ReceptacleToggle, ReceptacleSideScreen.SelectableEntity> depositObject in this.depositObjectMap)
      depositObject.Key.toggle.Deselect();
  }

  private void ToggleObjectPicker(bool Show)
  {
    this.requestObjectListContainer.SetActive(Show);
    if (Object.op_Inequality((Object) this.scrollBarContainer, (Object) null))
      this.scrollBarContainer.SetActive(Show);
    this.requestObjectList.SetActive(Show);
    this.activeEntityContainer.SetActive(!Show);
  }

  private void ConfigureActiveEntity(Tag tag)
  {
    string properName = Assets.GetPrefab(tag).GetProperName();
    ((TMP_Text) KMonoBehaviourExtensions.GetComponentInChildrenOnly<LocText>(this.activeEntityContainer)).text = properName;
    KMonoBehaviourExtensions.GetComponentInChildrenOnly<Image>(((Component) this.activeEntityContainer.transform.GetChild(0)).gameObject).sprite = this.GetEntityIcon(tag);
  }

  protected virtual Sprite GetEntityIcon(Tag prefabTag) => Def.GetUISprite((object) Assets.GetPrefab(prefabTag)).first;

  public override bool IsValidForTarget(GameObject target)
  {
    SingleEntityReceptacle component = target.GetComponent<SingleEntityReceptacle>();
    return Object.op_Inequality((Object) component, (Object) null) && ((Behaviour) component).enabled && Object.op_Equality((Object) target.GetComponent<PlantablePlot>(), (Object) null) && Object.op_Equality((Object) target.GetComponent<EggIncubator>(), (Object) null);
  }

  public override void SetTarget(GameObject target)
  {
    SingleEntityReceptacle component = target.GetComponent<SingleEntityReceptacle>();
    if (Object.op_Equality((Object) component, (Object) null))
    {
      Debug.LogError((object) "The object selected doesn't have a SingleObjectReceptacle!");
    }
    else
    {
      this.Initialize(component);
      this.UpdateState((object) null);
    }
  }

  protected virtual void RestoreSelectionFromOccupant()
  {
  }

  public override void ClearTarget()
  {
    if (!Object.op_Inequality((Object) this.targetReceptacle, (Object) null))
      return;
    if (this.CheckReceptacleOccupied())
    {
      KMonoBehaviourExtensions.Unsubscribe(this.targetReceptacle.Occupant.gameObject, this.onObjectDestroyedHandle);
      this.onObjectDestroyedHandle = -1;
    }
    this.targetReceptacle.Unsubscribe(this.onStorageChangedHandle);
    this.onStorageChangedHandle = -1;
    this.targetReceptacle.Unsubscribe(this.onOccupantValidChangedHandle);
    this.onOccupantValidChangedHandle = -1;
    if (this.targetReceptacle.GetActiveRequest == null)
      this.targetReceptacle.SetPreview(Tag.Invalid);
    SimAndRenderScheduler.instance.Remove((object) this);
    this.targetReceptacle = (SingleEntityReceptacle) null;
  }

  protected void SetImageToggleState(KToggle toggle, ImageToggleState.State state)
  {
    switch ((int) state)
    {
      case 0:
        ((Component) toggle).GetComponent<ImageToggleState>().SetDisabled();
        ((Graphic) KMonoBehaviourExtensions.GetComponentInChildrenOnly<Image>(((Component) toggle).gameObject)).material = this.desaturatedMaterial;
        break;
      case 1:
        ((Component) toggle).GetComponent<ImageToggleState>().SetInactive();
        ((Graphic) KMonoBehaviourExtensions.GetComponentInChildrenOnly<Image>(((Component) toggle).gameObject)).material = this.defaultMaterial;
        break;
      case 2:
        ((Component) toggle).GetComponent<ImageToggleState>().SetActive();
        ((Graphic) KMonoBehaviourExtensions.GetComponentInChildrenOnly<Image>(((Component) toggle).gameObject)).material = this.defaultMaterial;
        break;
      case 3:
        ((Component) toggle).GetComponent<ImageToggleState>().SetDisabledActive();
        ((Graphic) KMonoBehaviourExtensions.GetComponentInChildrenOnly<Image>(((Component) toggle).gameObject)).material = this.desaturatedMaterial;
        break;
    }
  }

  public void Render1000ms(float dt) => this.CheckAmountsAndUpdate((object) null);

  private void CheckAmountsAndUpdate(object data)
  {
    if (Object.op_Equality((Object) this.targetReceptacle, (Object) null) || !this.UpdateAvailableAmounts((object) null))
      return;
    this.UpdateState((object) null);
  }

  private bool UpdateAvailableAmounts(object data)
  {
    bool flag = false;
    foreach (KeyValuePair<ReceptacleToggle, ReceptacleSideScreen.SelectableEntity> depositObject in this.depositObjectMap)
    {
      if (!DebugHandler.InstantBuildMode && this.hideUndiscoveredEntities && !DiscoveredResources.Instance.IsDiscovered(depositObject.Value.tag))
        ((Component) depositObject.Key).gameObject.SetActive(false);
      else if (!((Component) depositObject.Key).gameObject.activeSelf)
        ((Component) depositObject.Key).gameObject.SetActive(true);
      float availableAmount = this.GetAvailableAmount(depositObject.Value.tag);
      if ((double) depositObject.Value.lastAmount != (double) availableAmount)
      {
        flag = true;
        depositObject.Value.lastAmount = availableAmount;
        ((TMP_Text) depositObject.Key.amount).text = availableAmount.ToString();
      }
      if (!this.ValidRotationForDeposit(depositObject.Value.direction) || (double) availableAmount <= 0.0)
      {
        if (Object.op_Inequality((Object) this.selectedEntityToggle, (Object) depositObject.Key))
          this.SetImageToggleState(depositObject.Key.toggle, (ImageToggleState.State) 0);
        else
          this.SetImageToggleState(depositObject.Key.toggle, (ImageToggleState.State) 3);
      }
      else if (Object.op_Inequality((Object) this.selectedEntityToggle, (Object) depositObject.Key))
        this.SetImageToggleState(depositObject.Key.toggle, (ImageToggleState.State) 1);
      else
        this.SetImageToggleState(depositObject.Key.toggle, (ImageToggleState.State) 2);
    }
    return flag;
  }

  private float GetAvailableAmount(Tag tag) => this.targetReceptacle.GetMyWorld().worldInventory.GetAmount(tag, true);

  private bool ValidRotationForDeposit(
    SingleEntityReceptacle.ReceptacleDirection depositDir)
  {
    return Object.op_Equality((Object) this.targetReceptacle.rotatable, (Object) null) || depositDir == this.targetReceptacle.Direction;
  }

  protected virtual void ToggleClicked(ReceptacleToggle toggle)
  {
    if (!this.depositObjectMap.ContainsKey(toggle))
    {
      Debug.LogError((object) "Recipe not found on recipe list.");
    }
    else
    {
      if (Object.op_Inequality((Object) this.selectedEntityToggle, (Object) null))
      {
        bool flag = this.CanDepositEntity(this.depositObjectMap[this.selectedEntityToggle]);
        this.requestSelectedEntityBtn.isInteractable = flag;
        this.SetImageToggleState(this.selectedEntityToggle.toggle, flag ? (ImageToggleState.State) 1 : (ImageToggleState.State) 0);
      }
      this.selectedEntityToggle = toggle;
      this.entityPreviousSelectionMap[this.targetReceptacle] = this.entityToggles.IndexOf(toggle);
      this.selectedDepositObjectTag = this.depositObjectMap[toggle].tag;
      MutantPlant component = this.depositObjectMap[toggle].asset.GetComponent<MutantPlant>();
      this.selectedDepositObjectAdditionalTag = Object.op_Implicit((Object) component) ? component.SubSpeciesID : Tag.Invalid;
      this.UpdateAvailableAmounts((object) null);
      this.UpdateState((object) null);
    }
  }

  private void CreateOrder(bool isInfinite) => this.targetReceptacle.CreateOrder(this.selectedDepositObjectTag, this.selectedDepositObjectAdditionalTag);

  protected bool CheckReceptacleOccupied() => Object.op_Inequality((Object) this.targetReceptacle, (Object) null) && Object.op_Inequality((Object) this.targetReceptacle.Occupant, (Object) null);

  protected virtual void SetResultDescriptions(GameObject go)
  {
    string str = "";
    InfoDescription component1 = go.GetComponent<InfoDescription>();
    if (Object.op_Implicit((Object) component1))
    {
      str = component1.description;
    }
    else
    {
      KPrefabID component2 = go.GetComponent<KPrefabID>();
      if (Object.op_Inequality((Object) component2, (Object) null))
      {
        Element element = ElementLoader.GetElement(component2.PrefabID());
        if (element != null)
          str = element.Description();
      }
      else
        str = go.GetProperName();
    }
    ((TMP_Text) this.descriptionLabel).SetText(str);
  }

  protected virtual void HideAllDescriptorPanels()
  {
    for (int index = 0; index < this.descriptorPanels.Count; ++index)
      ((Component) this.descriptorPanels[index]).gameObject.SetActive(false);
  }

  protected class SelectableEntity
  {
    public Tag tag;
    public SingleEntityReceptacle.ReceptacleDirection direction;
    public GameObject asset;
    public float lastAmount = -1f;
  }
}
