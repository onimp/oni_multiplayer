// Decompiled with JetBrains decompiler
// Type: CarePackageContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarePackageContainer : KScreen, ITelepadDeliverableContainer
{
  [Header("UI References")]
  [SerializeField]
  private GameObject contentBody;
  [SerializeField]
  private LocText characterName;
  public GameObject selectedBorder;
  [SerializeField]
  private Image titleBar;
  [SerializeField]
  private Color selectedTitleColor;
  [SerializeField]
  private Color deselectedTitleColor;
  [SerializeField]
  private KButton reshuffleButton;
  private KBatchedAnimController animController;
  [SerializeField]
  private LocText itemName;
  [SerializeField]
  private LocText quantity;
  [SerializeField]
  private LocText currentQuantity;
  [SerializeField]
  private LocText description;
  [SerializeField]
  private KToggle selectButton;
  private CarePackageInfo info;
  public CarePackageContainer.CarePackageInstanceData carePackageInstanceData;
  private CharacterSelectionController controller;
  private static List<ITelepadDeliverableContainer> containers;
  [SerializeField]
  private Sprite enabledSpr;
  [SerializeField]
  private List<CarePackageContainer.ProfessionIcon> professionIcons;
  private Dictionary<string, Sprite> professionIconMap;
  public float baseCharacterScale = 0.38f;
  private List<GameObject> entryIcons = new List<GameObject>();

  public GameObject GetGameObject() => ((Component) this).gameObject;

  public CarePackageInfo Info => this.info;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Initialize();
    ((MonoBehaviour) this).StartCoroutine(this.DelayedGeneration());
  }

  public virtual float GetSortKey() => 50f;

  private IEnumerator DelayedGeneration()
  {
    yield return (object) SequenceUtil.WaitForEndOfFrame;
    if (Object.op_Inequality((Object) this.controller, (Object) null))
      this.GenerateCharacter(this.controller.IsStarterMinion);
  }

  protected virtual void OnCmpDisable()
  {
    ((KMonoBehaviour) this).OnCmpDisable();
    if (!Object.op_Inequality((Object) this.animController, (Object) null))
      return;
    TracesExtesions.DeleteObject(((Component) this.animController).gameObject);
    this.animController = (KBatchedAnimController) null;
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    if (!Object.op_Inequality((Object) this.controller, (Object) null))
      return;
    this.controller.OnLimitReachedEvent -= new System.Action(this.OnCharacterSelectionLimitReached);
    this.controller.OnLimitUnreachedEvent -= new System.Action(this.OnCharacterSelectionLimitUnReached);
    this.controller.OnReshuffleEvent -= new Action<bool>(this.Reshuffle);
  }

  private void Initialize()
  {
    this.professionIconMap = new Dictionary<string, Sprite>();
    this.professionIcons.ForEach((Action<CarePackageContainer.ProfessionIcon>) (ic => this.professionIconMap.Add(ic.professionName, ic.iconImg)));
    if (CarePackageContainer.containers == null)
      CarePackageContainer.containers = new List<ITelepadDeliverableContainer>();
    CarePackageContainer.containers.Add((ITelepadDeliverableContainer) this);
  }

  private void GenerateCharacter(bool is_starter)
  {
    int num = 0;
    do
    {
      this.info = Immigration.Instance.RandomCarePackage();
      ++num;
    }
    while (this.IsCharacterRedundant() && num < 20);
    if (Object.op_Inequality((Object) this.animController, (Object) null))
    {
      Object.Destroy((Object) ((Component) this.animController).gameObject);
      this.animController = (KBatchedAnimController) null;
    }
    this.carePackageInstanceData = new CarePackageContainer.CarePackageInstanceData();
    this.carePackageInstanceData.info = this.info;
    this.carePackageInstanceData.facadeID = !(this.info.facadeID == "SELECTRANDOM") ? this.info.facadeID : Util.GetRandom<EquippableFacadeResource>(Db.GetEquippableFacades().resources.FindAll((Predicate<EquippableFacadeResource>) (match => match.DefID == this.info.id))).Id;
    this.SetAnimator();
    this.SetInfoText();
    this.selectButton.ClearOnClick();
    if (this.controller.IsStarterMinion)
      return;
    this.selectButton.onClick += (System.Action) (() => this.SelectDeliverable());
  }

  private void SetAnimator()
  {
    GameObject prefab = Assets.GetPrefab(TagExtensions.ToTag(this.info.id));
    EdiblesManager.FoodInfo foodInfo = EdiblesManager.GetFoodInfo(this.info.id);
    int num1 = ElementLoader.FindElementByName(this.info.id) == null ? (foodInfo == null ? (int) this.info.quantity : (int) ((double) this.info.quantity % (double) foodInfo.CaloriesPerUnit)) : 1;
    if (Object.op_Inequality((Object) prefab, (Object) null))
    {
      for (int index = 0; index < num1; ++index)
      {
        GameObject gameObject = Util.KInstantiateUI(this.contentBody, ((Component) this.contentBody.transform.parent).gameObject, false);
        gameObject.SetActive(true);
        Image component = gameObject.GetComponent<Image>();
        Tuple<Sprite, Color> tuple = Util.IsNullOrWhiteSpace(this.carePackageInstanceData.facadeID) ? Def.GetUISprite((object) prefab) : Def.GetUISprite(prefab.PrefabID(), this.carePackageInstanceData.facadeID);
        component.sprite = tuple.first;
        ((Graphic) component).color = tuple.second;
        this.entryIcons.Add(gameObject);
        if (num1 > 1)
        {
          int num2;
          int num3;
          int num4;
          if (num1 % 2 == 1)
          {
            num2 = Mathf.CeilToInt((float) (num1 / 2));
            int num5 = num2 - index;
            num3 = num5 > 0 ? 1 : -1;
            num4 = Mathf.Abs(num5);
          }
          else
          {
            num2 = num1 / 2 - 1;
            if (index <= num2)
            {
              num4 = Mathf.Abs(num2 - index);
              num3 = -1;
            }
            else
            {
              num4 = Mathf.Abs(num2 + 1 - index);
              num3 = 1;
            }
          }
          int num6 = 0;
          if (num1 % 2 == 0)
          {
            num6 = index <= num2 ? -6 : 6;
            Transform transform1 = gameObject.transform;
            Transform transform2 = gameObject.transform;
            Vector3 vector3_1;
            Vector3 vector3_2 = vector3_1 = Vector3.op_Addition(transform2.position, new Vector3((float) num6, 0.0f, 0.0f));
            transform2.position = vector3_1;
            Vector3 vector3_3 = vector3_2;
            TransformExtensions.SetPosition(transform1, vector3_3);
          }
          gameObject.transform.localScale = new Vector3((float) (1.0 - (double) num4 * 0.10000000149011612), (float) (1.0 - (double) num4 * 0.10000000149011612), 1f);
          gameObject.transform.Rotate(0.0f, 0.0f, 3f * (float) num4 * (float) num3);
          TransformExtensions.SetPosition(gameObject.transform, Vector3.op_Addition(Vector3.op_Addition(gameObject.transform.position, new Vector3(25f * (float) num4 * (float) num3, 5f * (float) num4)), new Vector3((float) num6, 0.0f, 0.0f)));
          gameObject.GetComponent<Canvas>().sortingOrder = num1 - num4;
        }
      }
    }
    else
    {
      GameObject gameObject = Util.KInstantiateUI(this.contentBody, ((Component) this.contentBody.transform.parent).gameObject, false);
      gameObject.SetActive(true);
      Image component = gameObject.GetComponent<Image>();
      component.sprite = Def.GetUISpriteFromMultiObjectAnim(ElementLoader.GetElement(TagExtensions.ToTag(this.info.id)).substance.anim);
      ((Graphic) component).color = Color32.op_Implicit(ElementLoader.GetElement(TagExtensions.ToTag(this.info.id)).substance.uiColour);
      this.entryIcons.Add(gameObject);
    }
  }

  private string GetSpawnableName()
  {
    GameObject prefab = Assets.GetPrefab(Tag.op_Implicit(this.info.id));
    if (Object.op_Equality((Object) prefab, (Object) null))
    {
      Element elementByName = ElementLoader.FindElementByName(this.info.id);
      return elementByName != null ? elementByName.substance.name : "";
    }
    return string.IsNullOrEmpty(this.carePackageInstanceData.facadeID) ? prefab.GetProperName() : EquippableFacade.GetNameOverride(this.carePackageInstanceData.info.id, this.carePackageInstanceData.facadeID);
  }

  private string GetSpawnableQuantityOnly()
  {
    if (ElementLoader.GetElement(TagExtensions.ToTag(this.info.id)) != null)
      return string.Format((string) STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, (object) GameUtil.GetFormattedMass(this.info.quantity));
    return EdiblesManager.GetFoodInfo(this.info.id) != null ? string.Format((string) STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, (object) GameUtil.GetFormattedCaloriesForItem(Tag.op_Implicit(this.info.id), this.info.quantity)) : string.Format((string) STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT_ONLY, (object) this.info.quantity.ToString());
  }

  private string GetCurrentQuantity(WorldInventory inventory)
  {
    if (ElementLoader.GetElement(TagExtensions.ToTag(this.info.id)) != null)
    {
      float amount = inventory.GetAmount(TagExtensions.ToTag(this.info.id), false);
      return string.Format((string) STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_CURRENT_AMOUNT, (object) GameUtil.GetFormattedMass(amount));
    }
    if (EdiblesManager.GetFoodInfo(this.info.id) != null)
    {
      float calories = RationTracker.Get().CountRationsByFoodType(this.info.id, inventory);
      return string.Format((string) STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_CURRENT_AMOUNT, (object) GameUtil.GetFormattedCalories(calories));
    }
    float amount1 = inventory.GetAmount(TagExtensions.ToTag(this.info.id), false);
    return string.Format((string) STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_CURRENT_AMOUNT, (object) amount1.ToString());
  }

  private string GetSpawnableQuantity()
  {
    if (ElementLoader.GetElement(TagExtensions.ToTag(this.info.id)) != null)
      return string.Format((string) STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_QUANTITY, (object) GameUtil.GetFormattedMass(this.info.quantity), (object) Assets.GetPrefab(Tag.op_Implicit(this.info.id)).GetProperName());
    return EdiblesManager.GetFoodInfo(this.info.id) != null ? string.Format((string) STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_QUANTITY, (object) GameUtil.GetFormattedCaloriesForItem(Tag.op_Implicit(this.info.id), this.info.quantity), (object) Assets.GetPrefab(Tag.op_Implicit(this.info.id)).GetProperName()) : string.Format((string) STRINGS.UI.IMMIGRANTSCREEN.CARE_PACKAGE_ELEMENT_COUNT, (object) Assets.GetPrefab(Tag.op_Implicit(this.info.id)).GetProperName(), (object) this.info.quantity.ToString());
  }

  private string GetSpawnableDescription()
  {
    Element element = ElementLoader.GetElement(TagExtensions.ToTag(this.info.id));
    if (element != null)
      return element.Description();
    GameObject prefab = Assets.GetPrefab(Tag.op_Implicit(this.info.id));
    if (Object.op_Equality((Object) prefab, (Object) null))
      return "";
    InfoDescription component = prefab.GetComponent<InfoDescription>();
    return Object.op_Inequality((Object) component, (Object) null) ? component.description : prefab.GetProperName();
  }

  private void SetInfoText()
  {
    ((TMP_Text) this.characterName).SetText(this.GetSpawnableName());
    ((TMP_Text) this.description).SetText(this.GetSpawnableDescription());
    ((TMP_Text) this.itemName).SetText(this.GetSpawnableName());
    ((TMP_Text) this.quantity).SetText(this.GetSpawnableQuantityOnly());
    ((TMP_Text) this.currentQuantity).SetText(this.GetCurrentQuantity(ClusterManager.Instance.activeWorld.worldInventory));
  }

  public void SelectDeliverable()
  {
    if (Object.op_Inequality((Object) this.controller, (Object) null))
      this.controller.AddDeliverable((ITelepadDeliverable) this.carePackageInstanceData);
    if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
      MusicManager.instance.SetSongParameter("Music_SelectDuplicant", "songSection", 1f);
    ((Component) this.selectButton).GetComponent<ImageToggleState>().SetActive();
    this.selectButton.ClearOnClick();
    this.selectButton.onClick += (System.Action) (() =>
    {
      this.DeselectDeliverable();
      if (!MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
        return;
      MusicManager.instance.SetSongParameter("Music_SelectDuplicant", "songSection", 0.0f);
    });
    this.selectedBorder.SetActive(true);
    ((Graphic) this.titleBar).color = this.selectedTitleColor;
  }

  public void DeselectDeliverable()
  {
    if (Object.op_Inequality((Object) this.controller, (Object) null))
      this.controller.RemoveDeliverable((ITelepadDeliverable) this.carePackageInstanceData);
    ((Component) this.selectButton).GetComponent<ImageToggleState>().SetInactive();
    this.selectButton.Deselect();
    this.selectButton.ClearOnClick();
    this.selectButton.onClick += (System.Action) (() => this.SelectDeliverable());
    this.selectedBorder.SetActive(false);
    ((Graphic) this.titleBar).color = this.deselectedTitleColor;
  }

  private void OnReplacedEvent(ITelepadDeliverable stats)
  {
    if (stats != this.carePackageInstanceData)
      return;
    this.DeselectDeliverable();
  }

  private void OnCharacterSelectionLimitReached()
  {
    if (Object.op_Inequality((Object) this.controller, (Object) null) && this.controller.IsSelected((ITelepadDeliverable) this.info))
      return;
    this.selectButton.ClearOnClick();
    if (this.controller.AllowsReplacing)
      this.selectButton.onClick += new System.Action(this.ReplaceCharacterSelection);
    else
      this.selectButton.onClick += new System.Action(this.CantSelectCharacter);
  }

  private void CantSelectCharacter() => KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));

  private void ReplaceCharacterSelection()
  {
    if (Object.op_Equality((Object) this.controller, (Object) null))
      return;
    this.controller.RemoveLast();
    this.SelectDeliverable();
  }

  private void OnCharacterSelectionLimitUnReached()
  {
    if (Object.op_Inequality((Object) this.controller, (Object) null) && this.controller.IsSelected((ITelepadDeliverable) this.info))
      return;
    this.selectButton.ClearOnClick();
    this.selectButton.onClick += (System.Action) (() => this.SelectDeliverable());
  }

  public void SetReshufflingState(bool enable) => ((Component) this.reshuffleButton).gameObject.SetActive(enable);

  private void Reshuffle(bool is_starter)
  {
    if (Object.op_Inequality((Object) this.controller, (Object) null) && this.controller.IsSelected((ITelepadDeliverable) this.info))
      this.DeselectDeliverable();
    this.ClearEntryIcons();
    this.GenerateCharacter(is_starter);
  }

  public void SetController(CharacterSelectionController csc)
  {
    if (Object.op_Equality((Object) csc, (Object) this.controller))
      return;
    this.controller = csc;
    this.controller.OnLimitReachedEvent += new System.Action(this.OnCharacterSelectionLimitReached);
    this.controller.OnLimitUnreachedEvent += new System.Action(this.OnCharacterSelectionLimitUnReached);
    this.controller.OnReshuffleEvent += new Action<bool>(this.Reshuffle);
    this.controller.OnReplacedEvent += new Action<ITelepadDeliverable>(this.OnReplacedEvent);
  }

  public void DisableSelectButton()
  {
    this.selectButton.soundPlayer.AcceptClickCondition = (Func<bool>) (() => false);
    ((Component) this.selectButton).GetComponent<ImageToggleState>().SetDisabled();
    ((WidgetSoundPlayer) this.selectButton.soundPlayer).Enabled = false;
  }

  private bool IsCharacterRedundant()
  {
    foreach (ITelepadDeliverableContainer container in CarePackageContainer.containers)
    {
      if (container != this)
      {
        CarePackageContainer packageContainer = container as CarePackageContainer;
        if (Object.op_Inequality((Object) packageContainer, (Object) null) && packageContainer.info == this.info)
          return true;
      }
    }
    return false;
  }

  public string GetValueColor(bool isPositive) => !isPositive ? "<color=#ff2222ff>" : "<color=green>";

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (e.IsAction((Action) 1))
      this.controller.OnPressBack();
    if (KInputManager.currentControllerIsGamepad)
      return;
    ((KInputEvent) e).Consumed = true;
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    if (KInputManager.currentControllerIsGamepad)
      return;
    ((KInputEvent) e).Consumed = true;
  }

  protected virtual void OnCmpEnable()
  {
    this.OnActivate();
    if (this.info == null)
      return;
    this.ClearEntryIcons();
    this.SetAnimator();
    this.SetInfoText();
  }

  private void ClearEntryIcons()
  {
    for (int index = 0; index < this.entryIcons.Count; ++index)
      Object.Destroy((Object) this.entryIcons[index]);
  }

  [Serializable]
  public struct ProfessionIcon
  {
    public string professionName;
    public Sprite iconImg;
  }

  public class CarePackageInstanceData : ITelepadDeliverable
  {
    public CarePackageInfo info;
    public string facadeID;

    public GameObject Deliver(Vector3 position)
    {
      GameObject gameObject = this.info.Deliver(position);
      gameObject.GetComponent<CarePackage>().SetFacade(this.facadeID);
      return gameObject;
    }
  }
}
