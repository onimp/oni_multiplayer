// Decompiled with JetBrains decompiler
// Type: EventInfoScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EventInfoScreen : KModalScreen
{
  [SerializeField]
  private float baseCharacterScale = 0.0057f;
  [FormerlySerializedAs("midgroundPrefab")]
  [FormerlySerializedAs("mid")]
  [Header("Prefabs")]
  [SerializeField]
  private GameObject animPrefab;
  [SerializeField]
  private GameObject optionPrefab;
  [SerializeField]
  private GameObject optionIconPrefab;
  [SerializeField]
  private GameObject optionTextPrefab;
  [Header("Groups")]
  [SerializeField]
  private Transform midgroundGroup;
  [SerializeField]
  private GameObject timeGroup;
  [SerializeField]
  private GameObject buttonsGroup;
  [SerializeField]
  private GameObject chainGroup;
  [Header("Text")]
  [SerializeField]
  private LocText eventHeader;
  [SerializeField]
  private LocText eventTimeLabel;
  [SerializeField]
  private LocText eventLocationLabel;
  [SerializeField]
  private LocText eventDescriptionLabel;
  [SerializeField]
  private bool loadMinionFromPersonalities = true;
  [SerializeField]
  private LocText chainCount;
  [Header("Button Colour Styles")]
  [SerializeField]
  private ColorStyleSetting neutralButtonSetting;
  [SerializeField]
  private ColorStyleSetting badButtonSetting;
  [SerializeField]
  private ColorStyleSetting goodButtonSetting;
  private List<KBatchedAnimController> createdAnimations = new List<KBatchedAnimController>();

  public override bool IsModal() => true;

  public void SetEventData(EventInfoData data)
  {
    data.FinalizeText();
    ((TMP_Text) this.eventHeader).text = data.title;
    ((TMP_Text) this.eventDescriptionLabel).text = data.description;
    ((TMP_Text) this.eventLocationLabel).text = data.location;
    ((TMP_Text) this.eventTimeLabel).text = data.whenDescription;
    if (Util.IsNullOrWhiteSpace(data.location) && Util.IsNullOrWhiteSpace(data.location))
      this.timeGroup.gameObject.SetActive(false);
    if (data.options.Count == 0)
      data.AddDefaultOption();
    this.SetEventDataOptions(data);
    this.SetEventDataVisuals(data);
  }

  private void SetEventDataOptions(EventInfoData data)
  {
    foreach (EventInfoData.Option option1 in data.options)
    {
      EventInfoData.Option option = option1;
      GameObject option2 = Util.KInstantiateUI(this.optionPrefab, this.buttonsGroup, false);
      ((Object) option2).name = "Option: " + option.mainText;
      KButton component1 = option2.GetComponent<KButton>();
      component1.isInteractable = option.allowed;
      component1.onClick += (System.Action) (() =>
      {
        if (option.callback != null)
          option.callback();
        base.Deactivate();
      });
      if (!Util.IsNullOrWhiteSpace(option.tooltip))
        option2.GetComponent<ToolTip>().SetSimpleTooltip(option.tooltip);
      else
        ((Behaviour) option2.GetComponent<ToolTip>()).enabled = false;
      foreach (EventInfoData.OptionIcon informationIcon in option.informationIcons)
        this.CreateOptionIcon(option2, informationIcon);
      LocText component2 = Util.KInstantiateUI(this.optionTextPrefab, option2, false).GetComponent<LocText>();
      string str;
      if (option.description != null)
        str = "<b>" + option.mainText + "</b>\n<i>(" + option.description + ")</i>";
      else
        str = "<b>" + option.mainText + "</b>";
      ((TMP_Text) component2).text = str;
      foreach (EventInfoData.OptionIcon consequenceIcon in option.consequenceIcons)
        this.CreateOptionIcon(option2, consequenceIcon);
      option2.SetActive(true);
    }
  }

  public virtual void Deactivate()
  {
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().EventPopupSnapshot);
    base.Deactivate();
  }

  private void CreateOptionIcon(GameObject option, EventInfoData.OptionIcon optionIcon)
  {
    GameObject gameObject = Util.KInstantiateUI(this.optionIconPrefab, option, false);
    gameObject.GetComponent<ToolTip>().SetSimpleTooltip(optionIcon.tooltip);
    HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
    Image reference1 = component.GetReference<Image>("Mask");
    Image reference2 = component.GetReference<Image>("Border");
    Image reference3 = component.GetReference<Image>("Icon");
    if (Object.op_Inequality((Object) optionIcon.sprite, (Object) null))
    {
      Transform transform = ((Component) reference3).transform;
      transform.localScale = Vector3.op_Multiply(transform.localScale, optionIcon.scale);
    }
    Color32 color32 = Color32.op_Implicit(Color.white);
    switch (optionIcon.containerType)
    {
      case EventInfoData.OptionIcon.ContainerType.Neutral:
        reference1.sprite = Assets.GetSprite(HashedString.op_Implicit("container_fill_neutral"));
        reference2.sprite = Assets.GetSprite(HashedString.op_Implicit("container_border_neutral"));
        if (Object.op_Equality((Object) optionIcon.sprite, (Object) null))
          optionIcon.sprite = Assets.GetSprite(HashedString.op_Implicit("knob"));
        color32 = GlobalAssets.Instance.colorSet.eventNeutral;
        break;
      case EventInfoData.OptionIcon.ContainerType.Positive:
        reference1.sprite = Assets.GetSprite(HashedString.op_Implicit("container_fill_positive"));
        reference2.sprite = Assets.GetSprite(HashedString.op_Implicit("container_border_positive"));
        RectTransform rectTransform1 = ((Graphic) reference3).rectTransform;
        ((Transform) rectTransform1).localPosition = Vector3.op_Addition(((Transform) rectTransform1).localPosition, Vector3.op_Multiply(Vector3.down, 1f));
        if (Object.op_Equality((Object) optionIcon.sprite, (Object) null))
          optionIcon.sprite = Assets.GetSprite(HashedString.op_Implicit("icon_positive"));
        color32 = GlobalAssets.Instance.colorSet.eventPositive;
        break;
      case EventInfoData.OptionIcon.ContainerType.Negative:
        reference1.sprite = Assets.GetSprite(HashedString.op_Implicit("container_fill_negative"));
        reference2.sprite = Assets.GetSprite(HashedString.op_Implicit("container_border_negative"));
        RectTransform rectTransform2 = ((Graphic) reference3).rectTransform;
        ((Transform) rectTransform2).localPosition = Vector3.op_Addition(((Transform) rectTransform2).localPosition, Vector3.op_Multiply(Vector3.up, 1f));
        color32 = GlobalAssets.Instance.colorSet.eventNegative;
        if (Object.op_Equality((Object) optionIcon.sprite, (Object) null))
        {
          optionIcon.sprite = Assets.GetSprite(HashedString.op_Implicit("cancel"));
          break;
        }
        break;
      case EventInfoData.OptionIcon.ContainerType.Information:
        reference1.sprite = Assets.GetSprite(HashedString.op_Implicit("requirements"));
        ((Behaviour) reference2).enabled = false;
        break;
    }
    ((Graphic) reference1).color = Color32.op_Implicit(color32);
    reference3.sprite = optionIcon.sprite;
    if (!Object.op_Equality((Object) optionIcon.sprite, (Object) null))
      return;
    ((Component) reference3).gameObject.SetActive(false);
  }

  private void SetEventDataVisuals(EventInfoData data)
  {
    this.createdAnimations.ForEach((Action<KBatchedAnimController>) (x => Object.Destroy((Object) x)));
    this.createdAnimations.Clear();
    KAnimFile anim = Assets.GetAnim(data.animFileName);
    if (Object.op_Equality((Object) anim, (Object) null))
    {
      Debug.LogWarning((object) ("Event " + data.title + " has no anim data"));
    }
    else
    {
      KBatchedAnimController component = ((Component) this.CreateAnimLayer(this.midgroundGroup, anim, data.mainAnim).transform).GetComponent<KBatchedAnimController>();
      if (data.minions != null)
      {
        for (int index = 0; index < data.minions.Length; ++index)
        {
          if (Object.op_Equality((Object) data.minions[index], (Object) null))
            DebugUtil.LogWarningArgs(new object[1]
            {
              (object) string.Format("EventInfoScreen unable to display minion {0}", (object) index)
            });
          string str = string.Format("dupe{0:D2}", (object) (index + 1));
          if (component.HasAnimation(HashedString.op_Implicit(str)))
            this.CreateAnimLayer(this.midgroundGroup, anim, HashedString.op_Implicit(str), data.minions[index]);
        }
      }
      if (!Object.op_Inequality((Object) data.artifact, (Object) null))
        return;
      string str1 = "artifact";
      if (!component.HasAnimation(HashedString.op_Implicit(str1)))
        return;
      this.CreateAnimLayer(this.midgroundGroup, anim, HashedString.op_Implicit(str1), artifact: data.artifact);
    }
  }

  private GameObject CreateAnimLayer(
    Transform parent,
    KAnimFile animFile,
    HashedString animName,
    GameObject minion = null,
    GameObject artifact = null,
    string targetSymbol = null)
  {
    GameObject go = Object.Instantiate<GameObject>(this.animPrefab, parent);
    KBatchedAnimController component1 = go.GetComponent<KBatchedAnimController>();
    this.createdAnimations.Add(component1);
    if (Object.op_Inequality((Object) minion, (Object) null))
      component1.AnimFiles = new KAnimFile[4]
      {
        Assets.GetAnim(HashedString.op_Implicit("body_comp_default_kanim")),
        Assets.GetAnim(HashedString.op_Implicit("head_swap_kanim")),
        Assets.GetAnim(HashedString.op_Implicit("body_swap_kanim")),
        animFile
      };
    else
      component1.AnimFiles = new KAnimFile[1]{ animFile };
    if (Object.op_Inequality((Object) minion, (Object) null))
    {
      SymbolOverrideController component2 = ((Component) component1).GetComponent<SymbolOverrideController>();
      if (this.loadMinionFromPersonalities)
      {
        ((Component) component1).GetComponent<UIDupeSymbolOverride>().Apply(minion.GetComponent<MinionIdentity>());
      }
      else
      {
        foreach (SymbolOverrideController.SymbolEntry getSymbolOverride in minion.GetComponent<SymbolOverrideController>().GetSymbolOverrides)
          component2.AddSymbolOverride(getSymbolOverride.targetSymbol, getSymbolOverride.sourceSymbol, getSymbolOverride.priority);
      }
      MinionConfig.ConfigureSymbols(go, true);
    }
    if (Object.op_Inequality((Object) artifact, (Object) null))
    {
      SymbolOverrideController component3 = ((Component) component1).GetComponent<SymbolOverrideController>();
      KBatchedAnimController component4 = artifact.GetComponent<KBatchedAnimController>();
      string str = component4.initialAnim.Replace("idle_", "artifact_").Replace("_loop", "");
      KAnim.Build.Symbol symbol = component4.AnimFiles[0].GetData().build.GetSymbol(KAnimHashedString.op_Implicit(str));
      if (symbol != null)
        component3.AddSymbolOverride(HashedString.op_Implicit("snapTo_artifact"), symbol);
    }
    if (targetSymbol != null)
      go.AddOrGet<KBatchedAnimTracker>().symbol = HashedString.op_Implicit(targetSymbol);
    go.SetActive(true);
    component1.Play(animName, (KAnim.PlayMode) 0);
    component1.animScale = this.baseCharacterScale;
    return go;
  }

  public static EventInfoScreen ShowPopup(EventInfoData eventInfoData)
  {
    EventInfoScreen eventInfoScreen = (EventInfoScreen) KScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.eventInfoScreen).gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject);
    eventInfoScreen.SetEventData(eventInfoData);
    AudioMixer.instance.Start(AudioMixerSnapshots.Get().EventPopupSnapshot);
    if (eventInfoData.showCallback != null)
      eventInfoData.showCallback();
    if (!Object.op_Inequality((Object) eventInfoData.clickFocus, (Object) null))
      return eventInfoScreen;
    WorldContainer myWorld = ((Component) eventInfoData.clickFocus).gameObject.GetMyWorld();
    if (!Object.op_Inequality((Object) myWorld, (Object) null) || !myWorld.IsDiscovered)
      return eventInfoScreen;
    CameraController.Instance.ActiveWorldStarWipe(myWorld.id, eventInfoData.clickFocus.position);
    return eventInfoScreen;
  }

  public static Notification CreateNotification(
    EventInfoData eventInfoData,
    Notification.ClickCallback clickCallback = null)
  {
    if (eventInfoData == null)
    {
      DebugUtil.LogWarningArgs(new object[1]
      {
        (object) "eventPopup is null in CreateStandardEventNotification"
      });
      return (Notification) null;
    }
    eventInfoData.FinalizeText();
    return new Notification(eventInfoData.title, NotificationType.Event, expires: false, click_focus: eventInfoData.clickFocus)
    {
      customClickCallback = clickCallback != null ? clickCallback : (Notification.ClickCallback) (data => EventInfoScreen.ShowPopup(eventInfoData))
    };
  }
}
