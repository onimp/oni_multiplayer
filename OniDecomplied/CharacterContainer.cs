// Decompiled with JetBrains decompiler
// Type: CharacterContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei.AI;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TUNING;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterContainer : KScreen, ITelepadDeliverableContainer
{
  [SerializeField]
  private GameObject contentBody;
  [SerializeField]
  private LocText characterName;
  [SerializeField]
  private EditableTitleBar characterNameTitle;
  [SerializeField]
  private LocText characterJob;
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
  private GameObject iconGroup;
  private List<GameObject> iconGroups;
  [SerializeField]
  private LocText goodTrait;
  [SerializeField]
  private LocText badTrait;
  [SerializeField]
  private GameObject aptitudeEntry;
  [SerializeField]
  private Transform aptitudeLabel;
  [SerializeField]
  private Transform attributeLabelAptitude;
  [SerializeField]
  private Transform attributeLabelTrait;
  [SerializeField]
  private LocText expectationRight;
  private List<LocText> expectationLabels;
  [SerializeField]
  private DropDown archetypeDropDown;
  [SerializeField]
  private Image selectedArchetypeIcon;
  [SerializeField]
  private Sprite noArchetypeIcon;
  [SerializeField]
  private Sprite dropdownArrowIcon;
  private string guaranteedAptitudeID;
  private List<GameObject> aptitudeEntries;
  private List<GameObject> traitEntries;
  [SerializeField]
  private LocText description;
  [SerializeField]
  private KToggle selectButton;
  [SerializeField]
  private KBatchedAnimController fxAnim;
  private MinionStartingStats stats;
  private CharacterSelectionController controller;
  private static List<CharacterContainer> containers;
  private KAnimFile idle_anim;
  [HideInInspector]
  public bool addMinionToIdentityList = true;
  [SerializeField]
  private Sprite enabledSpr;
  [SerializeField]
  private KScrollRect scroll_rect;
  private static readonly Dictionary<HashedString, string[]> traitIdleAnims;
  private static readonly HashedString[] idleAnims;
  public float baseCharacterScale = 0.38f;

  public GameObject GetGameObject() => ((Component) this).gameObject;

  public MinionStartingStats Stats => this.stats;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Initialize();
    this.characterNameTitle.OnStartedEditing += new System.Action(this.OnStartedEditing);
    this.characterNameTitle.OnNameChanged += new Action<string>(this.OnNameChanged);
    this.reshuffleButton.onClick += (System.Action) (() => this.Reshuffle(true));
    List<IListableOption> contentKeys = new List<IListableOption>();
    foreach (SkillGroup skillGroup in new List<SkillGroup>((IEnumerable<SkillGroup>) Db.Get().SkillGroups.resources))
      contentKeys.Add((IListableOption) skillGroup);
    this.archetypeDropDown.Initialize((IEnumerable<IListableOption>) contentKeys, new Action<IListableOption, object>(this.OnArchetypeEntryClick), new Func<IListableOption, IListableOption, object, int>(this.archetypeDropDownSort), new Action<DropDownEntry, object>(this.archetypeDropEntryRefreshAction), false);
    this.archetypeDropDown.CustomizeEmptyRow(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.CHARACTERCONTAINER_NOARCHETYPESELECTED")), this.noArchetypeIcon);
    ((MonoBehaviour) this).StartCoroutine(this.DelayedGeneration());
  }

  public void ForceStopEditingTitle() => this.characterNameTitle.ForceStopEditing();

  public virtual float GetSortKey() => 50f;

  private IEnumerator DelayedGeneration()
  {
    yield return (object) SequenceUtil.WaitForEndOfFrame;
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

  protected virtual void OnForcedCleanUp()
  {
    CharacterContainer.containers.Remove(this);
    ((KMonoBehaviour) this).OnForcedCleanUp();
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
    this.iconGroups = new List<GameObject>();
    this.traitEntries = new List<GameObject>();
    this.expectationLabels = new List<LocText>();
    this.aptitudeEntries = new List<GameObject>();
    if (CharacterContainer.containers == null)
      CharacterContainer.containers = new List<CharacterContainer>();
    CharacterContainer.containers.Add(this);
  }

  private void OnNameChanged(string newName)
  {
    this.stats.Name = newName;
    this.stats.personality.Name = newName;
    ((TMP_Text) this.description).text = this.stats.personality.description;
  }

  private void OnStartedEditing() => KScreenManager.Instance.RefreshStack();

  private void GenerateCharacter(bool is_starter, string guaranteedAptitudeID = null)
  {
    int num = 0;
    do
    {
      this.stats = new MinionStartingStats(is_starter, guaranteedAptitudeID);
      ++num;
    }
    while (this.IsCharacterRedundant() && num < 20);
    if (Object.op_Inequality((Object) this.animController, (Object) null))
    {
      Object.Destroy((Object) ((Component) this.animController).gameObject);
      this.animController = (KBatchedAnimController) null;
    }
    this.SetAnimator();
    this.SetInfoText();
    ((MonoBehaviour) this).StartCoroutine(this.SetAttributes());
    this.selectButton.ClearOnClick();
    if (this.controller.IsStarterMinion)
      return;
    ((Behaviour) this.selectButton).enabled = true;
    this.selectButton.onClick += (System.Action) (() => this.SelectDeliverable());
  }

  private void SetAnimator()
  {
    if (Object.op_Equality((Object) this.animController, (Object) null))
    {
      this.animController = Util.KInstantiateUI(Assets.GetPrefab(new Tag("MinionSelectPreview")), this.contentBody.gameObject, false).GetComponent<KBatchedAnimController>();
      ((Component) this.animController).gameObject.SetActive(true);
      this.animController.animScale = this.baseCharacterScale;
    }
    this.stats.ApplyTraits(((Component) this.animController).gameObject);
    this.stats.ApplyRace(((Component) this.animController).gameObject);
    this.stats.ApplyAccessories(((Component) this.animController).gameObject);
    this.stats.ApplyOutfit(this.stats.personality, ((Component) this.animController).gameObject);
    this.stats.ApplyExperience(((Component) this.animController).gameObject);
    this.idle_anim = Assets.GetAnim(this.GetIdleAnim(this.stats));
    if (Object.op_Inequality((Object) this.idle_anim, (Object) null))
      this.animController.AddAnimOverrides(this.idle_anim);
    KAnimFile anim = Assets.GetAnim(new HashedString("crewSelect_fx_kanim"));
    if (Object.op_Inequality((Object) anim, (Object) null))
      this.animController.AddAnimOverrides(anim);
    this.animController.Queue(HashedString.op_Implicit("idle_default"), (KAnim.PlayMode) 0);
  }

  private HashedString GetIdleAnim(MinionStartingStats minionStartingStats)
  {
    List<HashedString> hashedStringList = new List<HashedString>();
    foreach (KeyValuePair<HashedString, string[]> traitIdleAnim in CharacterContainer.traitIdleAnims)
    {
      foreach (Trait trait in minionStartingStats.Traits)
      {
        if (((IEnumerable<string>) traitIdleAnim.Value).Contains<string>(trait.Id))
          hashedStringList.Add(traitIdleAnim.Key);
      }
      if (((IEnumerable<string>) traitIdleAnim.Value).Contains<string>(minionStartingStats.joyTrait.Id) || ((IEnumerable<string>) traitIdleAnim.Value).Contains<string>(minionStartingStats.stressTrait.Id))
        hashedStringList.Add(traitIdleAnim.Key);
    }
    return hashedStringList.Count > 0 ? hashedStringList.ToArray()[Random.Range(0, hashedStringList.Count)] : CharacterContainer.idleAnims[Random.Range(0, CharacterContainer.idleAnims.Length)];
  }

  private void SetInfoText()
  {
    this.traitEntries.ForEach((Action<GameObject>) (tl => Object.Destroy((Object) tl.gameObject)));
    this.traitEntries.Clear();
    this.characterNameTitle.SetTitle(this.stats.Name);
    for (int index1 = 1; index1 < this.stats.Traits.Count; ++index1)
    {
      Trait trait = this.stats.Traits[index1];
      LocText locText1 = trait.PositiveTrait ? this.goodTrait : this.badTrait;
      LocText locText2 = Util.KInstantiateUI<LocText>(((Component) locText1).gameObject, ((Component) ((TMP_Text) locText1).transform.parent).gameObject, false);
      ((Component) locText2).gameObject.SetActive(true);
      ((TMP_Text) locText2).text = this.stats.Traits[index1].Name;
      ((Graphic) locText2).color = trait.PositiveTrait ? Constants.POSITIVE_COLOR : Constants.NEGATIVE_COLOR;
      ((Component) locText2).GetComponent<ToolTip>().SetSimpleTooltip(trait.GetTooltip());
      for (int index2 = 0; index2 < trait.SelfModifiers.Count; ++index2)
      {
        GameObject gameObject = Util.KInstantiateUI(((Component) this.attributeLabelTrait).gameObject, ((Component) ((TMP_Text) locText1).transform.parent).gameObject, false);
        gameObject.SetActive(true);
        LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
        string format = (string) ((double) trait.SelfModifiers[index2].Value > 0.0 ? STRINGS.UI.CHARACTERCONTAINER_ATTRIBUTEMODIFIER_INCREASED : STRINGS.UI.CHARACTERCONTAINER_ATTRIBUTEMODIFIER_DECREASED);
        ((TMP_Text) componentInChildren).text = string.Format(format, (object) Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + trait.SelfModifiers[index2].AttributeId.ToUpper() + ".NAME"));
        int num = trait.SelfModifiers[index2].AttributeId == "GermResistance" ? 1 : 0;
        Klei.AI.Attribute attrib = Db.Get().Attributes.Get(trait.SelfModifiers[index2].AttributeId);
        string str1 = attrib.Description + "\n\n" + StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + trait.SelfModifiers[index2].AttributeId.ToUpper() + ".NAME")) + ": " + trait.SelfModifiers[index2].GetFormattedString();
        List<AttributeConverter> convertersForAttribute = Db.Get().AttributeConverters.GetConvertersForAttribute(attrib);
        for (int index3 = 0; index3 < convertersForAttribute.Count; ++index3)
        {
          string str2 = convertersForAttribute[index3].DescriptionFromAttribute(convertersForAttribute[index3].multiplier * trait.SelfModifiers[index2].Value, (GameObject) null);
          if (str2 != "")
            str1 = str1 + "\n    • " + str2;
        }
        ((Component) componentInChildren).GetComponent<ToolTip>().SetSimpleTooltip(str1);
        this.traitEntries.Add(gameObject);
      }
      if (trait.disabledChoreGroups != null)
      {
        GameObject gameObject = Util.KInstantiateUI(((Component) this.attributeLabelTrait).gameObject, ((Component) ((TMP_Text) locText1).transform.parent).gameObject, false);
        gameObject.SetActive(true);
        LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
        ((TMP_Text) componentInChildren).text = trait.GetDisabledChoresString(false);
        string str3 = "";
        string str4 = "";
        for (int index4 = 0; index4 < trait.disabledChoreGroups.Length; ++index4)
        {
          if (index4 > 0)
          {
            str3 += ", ";
            str4 += "\n";
          }
          str3 += trait.disabledChoreGroups[index4].Name;
          str4 += trait.disabledChoreGroups[index4].description;
        }
        ((Component) componentInChildren).GetComponent<ToolTip>().SetSimpleTooltip(string.Format((string) DUPLICANTS.TRAITS.CANNOT_DO_TASK_TOOLTIP, (object) str3, (object) str4));
        this.traitEntries.Add(gameObject);
      }
      if (trait.ignoredEffects != null && trait.ignoredEffects.Length != 0)
      {
        GameObject gameObject = Util.KInstantiateUI(((Component) this.attributeLabelTrait).gameObject, ((Component) ((TMP_Text) locText1).transform.parent).gameObject, false);
        gameObject.SetActive(true);
        LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
        ((TMP_Text) componentInChildren).text = trait.GetIgnoredEffectsString(false);
        string str5 = "";
        string str6 = "";
        for (int index5 = 0; index5 < trait.ignoredEffects.Length; ++index5)
        {
          if (index5 > 0)
          {
            str5 += ", ";
            str6 += "\n";
          }
          str5 += StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + trait.ignoredEffects[index5].ToUpper() + ".NAME"));
          str6 += StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + trait.ignoredEffects[index5].ToUpper() + ".CAUSE"));
        }
        ((Component) componentInChildren).GetComponent<ToolTip>().SetSimpleTooltip(string.Format((string) DUPLICANTS.TRAITS.IGNORED_EFFECTS_TOOLTIP, (object) str5, (object) str6));
        this.traitEntries.Add(gameObject);
      }
      StringEntry stringEntry;
      if (Strings.TryGet("STRINGS.DUPLICANTS.TRAITS." + trait.Id.ToUpper() + ".SHORT_DESC", ref stringEntry))
      {
        GameObject gameObject = Util.KInstantiateUI(((Component) this.attributeLabelTrait).gameObject, ((Component) ((TMP_Text) locText1).transform.parent).gameObject, false);
        gameObject.SetActive(true);
        LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
        ((TMP_Text) componentInChildren).text = stringEntry.String;
        ((Component) componentInChildren).GetComponent<ToolTip>().SetSimpleTooltip(StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.TRAITS." + trait.Id.ToUpper() + ".SHORT_DESC_TOOLTIP")));
        this.traitEntries.Add(gameObject);
      }
      this.traitEntries.Add(((Component) locText2).gameObject);
    }
    this.aptitudeEntries.ForEach((Action<GameObject>) (al => Object.Destroy((Object) al.gameObject)));
    this.aptitudeEntries.Clear();
    this.expectationLabels.ForEach((Action<LocText>) (el => Object.Destroy((Object) ((Component) el).gameObject)));
    this.expectationLabels.Clear();
    foreach (KeyValuePair<SkillGroup, float> skillAptitude in this.stats.skillAptitudes)
    {
      if ((double) skillAptitude.Value != 0.0)
      {
        SkillGroup skillGroup = Db.Get().SkillGroups.Get(skillAptitude.Key.IdHash);
        if (skillGroup == null)
        {
          Debug.LogWarningFormat("Role group not found for aptitude: {0}", new object[1]
          {
            (object) skillAptitude.Key
          });
        }
        else
        {
          GameObject gameObject = Util.KInstantiateUI(this.aptitudeEntry.gameObject, ((Component) this.aptitudeEntry.transform.parent).gameObject, false);
          LocText locText3 = Util.KInstantiateUI<LocText>(((Component) this.aptitudeLabel).gameObject, gameObject, false);
          ((Component) locText3).gameObject.SetActive(true);
          ((TMP_Text) locText3).text = skillGroup.Name;
          string str7;
          if (skillGroup.choreGroupID != "")
          {
            ChoreGroup choreGroup = Db.Get().ChoreGroups.Get(skillGroup.choreGroupID);
            str7 = string.Format((string) DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION_CHOREGROUP, (object) skillGroup.Name, (object) DUPLICANTSTATS.APTITUDE_BONUS, (object) choreGroup.description);
          }
          else
            str7 = string.Format((string) DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, (object) skillGroup.Name, (object) DUPLICANTSTATS.APTITUDE_BONUS);
          ((Component) locText3).GetComponent<ToolTip>().SetSimpleTooltip(str7);
          float startingLevel = (float) this.stats.StartingLevels[skillAptitude.Key.relevantAttributes[0].Id];
          LocText locText4 = Util.KInstantiateUI<LocText>(((Component) this.attributeLabelAptitude).gameObject, gameObject, false);
          ((Component) locText4).gameObject.SetActive(true);
          ((TMP_Text) locText4).text = "+" + startingLevel.ToString() + " " + skillAptitude.Key.relevantAttributes[0].Name;
          string str8 = skillAptitude.Key.relevantAttributes[0].Description + "\n\n" + skillAptitude.Key.relevantAttributes[0].Name + ": +" + startingLevel.ToString();
          List<AttributeConverter> convertersForAttribute = Db.Get().AttributeConverters.GetConvertersForAttribute(skillAptitude.Key.relevantAttributes[0]);
          for (int index = 0; index < convertersForAttribute.Count; ++index)
            str8 = str8 + "\n    • " + convertersForAttribute[index].DescriptionFromAttribute(convertersForAttribute[index].multiplier * startingLevel, (GameObject) null);
          ((Component) locText4).GetComponent<ToolTip>().SetSimpleTooltip(str8);
          gameObject.gameObject.SetActive(true);
          this.aptitudeEntries.Add(gameObject);
        }
      }
    }
    if (this.stats.stressTrait != null)
    {
      LocText locText = Util.KInstantiateUI<LocText>(((Component) this.expectationRight).gameObject, ((Component) ((TMP_Text) this.expectationRight).transform.parent).gameObject, false);
      ((Component) locText).gameObject.SetActive(true);
      ((TMP_Text) locText).text = string.Format((string) STRINGS.UI.CHARACTERCONTAINER_STRESSTRAIT, (object) this.stats.stressTrait.Name);
      ((Component) locText).GetComponent<ToolTip>().SetSimpleTooltip(this.stats.stressTrait.GetTooltip());
      this.expectationLabels.Add(locText);
    }
    if (this.stats.joyTrait != null)
    {
      LocText locText = Util.KInstantiateUI<LocText>(((Component) this.expectationRight).gameObject, ((Component) ((TMP_Text) this.expectationRight).transform.parent).gameObject, false);
      ((Component) locText).gameObject.SetActive(true);
      ((TMP_Text) locText).text = string.Format((string) STRINGS.UI.CHARACTERCONTAINER_JOYTRAIT, (object) this.stats.joyTrait.Name);
      ((Component) locText).GetComponent<ToolTip>().SetSimpleTooltip(this.stats.joyTrait.GetTooltip());
      this.expectationLabels.Add(locText);
    }
    ((TMP_Text) this.description).text = this.stats.personality.description;
  }

  private IEnumerator SetAttributes()
  {
    yield return (object) null;
    this.iconGroups.ForEach((Action<GameObject>) (icg => Object.Destroy((Object) icg)));
    this.iconGroups.Clear();
    List<AttributeInstance> source = new List<AttributeInstance>((IEnumerable<AttributeInstance>) ((Component) this.animController).gameObject.GetAttributes().AttributeTable);
    source.RemoveAll((Predicate<AttributeInstance>) (at => at.Attribute.ShowInUI != Klei.AI.Attribute.Display.Skill));
    List<AttributeInstance> list = source.OrderBy<AttributeInstance, string>((Func<AttributeInstance, string>) (at => at.Name)).ToList<AttributeInstance>();
    for (int index = 0; index < list.Count; ++index)
    {
      GameObject gameObject = Util.KInstantiateUI(this.iconGroup.gameObject, ((Component) this.iconGroup.transform.parent).gameObject, false);
      LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
      gameObject.SetActive(true);
      float totalValue = list[index].GetTotalValue();
      if ((double) totalValue > 0.0)
        ((Graphic) componentInChildren).color = Constants.POSITIVE_COLOR;
      else if ((double) totalValue == 0.0)
        ((Graphic) componentInChildren).color = Constants.NEUTRAL_COLOR;
      else
        ((Graphic) componentInChildren).color = Constants.NEGATIVE_COLOR;
      ((TMP_Text) componentInChildren).text = string.Format((string) STRINGS.UI.CHARACTERCONTAINER_SKILL_VALUE, (object) GameUtil.AddPositiveSign(totalValue.ToString(), (double) totalValue > 0.0), (object) list[index].Name);
      AttributeInstance attributeInstance = list[index];
      string str1 = attributeInstance.Description;
      if (attributeInstance.Attribute.converters.Count > 0)
      {
        str1 += "\n";
        foreach (AttributeConverter converter1 in attributeInstance.Attribute.converters)
        {
          AttributeConverterInstance converter2 = ((Component) this.animController).gameObject.GetComponent<Klei.AI.AttributeConverters>().GetConverter(converter1.Id);
          string str2 = converter2.DescriptionFromAttribute(converter2.Evaluate(), converter2.gameObject);
          if (str2 != null)
            str1 = str1 + "\n" + str2;
        }
      }
      gameObject.GetComponent<ToolTip>().SetSimpleTooltip(str1);
      this.iconGroups.Add(gameObject);
    }
  }

  public void SelectDeliverable()
  {
    if (Object.op_Inequality((Object) this.controller, (Object) null))
      this.controller.AddDeliverable((ITelepadDeliverable) this.stats);
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
    this.animController.Play(HashedString.op_Implicit("cheer_pre"));
    this.animController.Play(HashedString.op_Implicit("cheer_loop"), (KAnim.PlayMode) 0);
  }

  public void DeselectDeliverable()
  {
    if (Object.op_Inequality((Object) this.controller, (Object) null))
      this.controller.RemoveDeliverable((ITelepadDeliverable) this.stats);
    ((Component) this.selectButton).GetComponent<ImageToggleState>().SetInactive();
    this.selectButton.Deselect();
    this.selectButton.ClearOnClick();
    this.selectButton.onClick += (System.Action) (() => this.SelectDeliverable());
    this.selectedBorder.SetActive(false);
    ((Graphic) this.titleBar).color = this.deselectedTitleColor;
    this.animController.Queue(HashedString.op_Implicit("cheer_pst"));
    this.animController.Queue(HashedString.op_Implicit("idle_default"), (KAnim.PlayMode) 0);
  }

  private void OnReplacedEvent(ITelepadDeliverable deliverable)
  {
    if (deliverable != this.stats)
      return;
    this.DeselectDeliverable();
  }

  private void OnCharacterSelectionLimitReached()
  {
    if (Object.op_Inequality((Object) this.controller, (Object) null) && this.controller.IsSelected((ITelepadDeliverable) this.stats))
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
    if (Object.op_Inequality((Object) this.controller, (Object) null) && this.controller.IsSelected((ITelepadDeliverable) this.stats))
      return;
    this.selectButton.ClearOnClick();
    this.selectButton.onClick += (System.Action) (() => this.SelectDeliverable());
  }

  public void SetReshufflingState(bool enable)
  {
    ((Component) this.reshuffleButton).gameObject.SetActive(enable);
    ((Component) this.archetypeDropDown).gameObject.SetActive(enable);
  }

  private void Reshuffle(bool is_starter)
  {
    if (Object.op_Inequality((Object) this.controller, (Object) null) && this.controller.IsSelected((ITelepadDeliverable) this.stats))
      this.DeselectDeliverable();
    if (Object.op_Inequality((Object) this.fxAnim, (Object) null))
      this.fxAnim.Play(HashedString.op_Implicit("loop"));
    this.GenerateCharacter(is_starter, this.guaranteedAptitudeID);
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

  private bool IsCharacterRedundant() => Object.op_Inequality((Object) CharacterContainer.containers.Find((Predicate<CharacterContainer>) (c => Object.op_Inequality((Object) c, (Object) null) && c.stats != null && Object.op_Inequality((Object) c, (Object) this) && c.stats.Name == this.stats.Name && c.stats.IsValid)), (Object) null) || Components.LiveMinionIdentities.Items.Any<MinionIdentity>((Func<MinionIdentity, bool>) (id => id.GetProperName() == this.stats.Name));

  public string GetValueColor(bool isPositive) => !isPositive ? "<color=#ff2222ff>" : "<color=green>";

  public virtual void OnPointerEnter(PointerEventData eventData)
  {
    this.scroll_rect.mouseIsOver = true;
    base.OnPointerEnter(eventData);
  }

  public virtual void OnPointerExit(PointerEventData eventData)
  {
    this.scroll_rect.mouseIsOver = false;
    base.OnPointerExit(eventData);
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (e.IsAction((Action) 1) || e.IsAction((Action) 5))
    {
      this.characterNameTitle.ForceStopEditing();
      this.controller.OnPressBack();
      this.archetypeDropDown.scrollRect.gameObject.SetActive(false);
    }
    if (KInputManager.currentControllerIsGamepad)
    {
      if (this.archetypeDropDown.scrollRect.activeInHierarchy)
      {
        KScrollRect component = this.archetypeDropDown.scrollRect.GetComponent<KScrollRect>();
        Vector2 vector2 = Vector2.op_Implicit(((Transform) Util.rectTransform((Component) component)).InverseTransformPoint(KInputManager.GetMousePos()));
        Rect rect = Util.rectTransform((Component) component).rect;
        component.mouseIsOver = ((Rect) ref rect).Contains(vector2);
        component.OnKeyDown(e);
      }
      else
        this.scroll_rect.OnKeyDown(e);
    }
    else
      ((KInputEvent) e).Consumed = true;
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    if (KInputManager.currentControllerIsGamepad)
    {
      if (this.archetypeDropDown.scrollRect.activeInHierarchy)
      {
        KScrollRect component = this.archetypeDropDown.scrollRect.GetComponent<KScrollRect>();
        Vector2 vector2 = Vector2.op_Implicit(((Transform) Util.rectTransform((Component) component)).InverseTransformPoint(KInputManager.GetMousePos()));
        Rect rect = Util.rectTransform((Component) component).rect;
        component.mouseIsOver = ((Rect) ref rect).Contains(vector2);
        component.OnKeyUp(e);
      }
      else
        this.scroll_rect.OnKeyUp(e);
    }
    else
      ((KInputEvent) e).Consumed = true;
  }

  protected virtual void OnCmpEnable()
  {
    this.OnActivate();
    if (this.stats == null)
      return;
    this.SetAnimator();
  }

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    this.characterNameTitle.ForceStopEditing();
  }

  private void OnArchetypeEntryClick(IListableOption skill, object data)
  {
    if (skill != null)
    {
      SkillGroup skillGroup = skill as SkillGroup;
      this.guaranteedAptitudeID = skillGroup.Id;
      this.selectedArchetypeIcon.sprite = Assets.GetSprite(HashedString.op_Implicit(skillGroup.archetypeIcon));
      this.Reshuffle(true);
    }
    else
    {
      this.guaranteedAptitudeID = (string) null;
      this.selectedArchetypeIcon.sprite = this.dropdownArrowIcon;
      this.Reshuffle(true);
    }
  }

  private int archetypeDropDownSort(IListableOption a, IListableOption b, object targetData) => b.Equals((object) "Random") ? -1 : b.GetProperName().CompareTo(a.GetProperName());

  private void archetypeDropEntryRefreshAction(DropDownEntry entry, object targetData)
  {
    if (entry.entryData == null)
      return;
    SkillGroup entryData = entry.entryData as SkillGroup;
    ((Image) entry.image).sprite = Assets.GetSprite(HashedString.op_Implicit(entryData.archetypeIcon));
  }

  static CharacterContainer()
  {
    Dictionary<HashedString, string[]> dictionary = new Dictionary<HashedString, string[]>();
    dictionary.Add(HashedString.op_Implicit("anim_idle_food_kanim"), new string[1]
    {
      "Foodie"
    });
    dictionary.Add(HashedString.op_Implicit("anim_idle_animal_lover_kanim"), new string[1]
    {
      "RanchingUp"
    });
    dictionary.Add(HashedString.op_Implicit("anim_idle_loner_kanim"), new string[1]
    {
      "Loner"
    });
    dictionary.Add(HashedString.op_Implicit("anim_idle_mole_hands_kanim"), new string[1]
    {
      "MoleHands"
    });
    dictionary.Add(HashedString.op_Implicit("anim_idle_buff_kanim"), new string[1]
    {
      "StrongArm"
    });
    dictionary.Add(HashedString.op_Implicit("anim_idle_distracted_kanim"), new string[4]
    {
      "CantResearch",
      "CantBuild",
      "CantCook",
      "CantDig"
    });
    dictionary.Add(HashedString.op_Implicit("anim_idle_coaster_kanim"), new string[1]
    {
      "HappySinger"
    });
    CharacterContainer.traitIdleAnims = dictionary;
    CharacterContainer.idleAnims = new HashedString[6]
    {
      HashedString.op_Implicit("anim_idle_healthy_kanim"),
      HashedString.op_Implicit("anim_idle_susceptible_kanim"),
      HashedString.op_Implicit("anim_idle_keener_kanim"),
      HashedString.op_Implicit("anim_idle_fastfeet_kanim"),
      HashedString.op_Implicit("anim_idle_breatherdeep_kanim"),
      HashedString.op_Implicit("anim_idle_breathershallow_kanim")
    };
  }

  [Serializable]
  public struct ProfessionIcon
  {
    public string professionName;
    public Sprite iconImg;
  }
}
