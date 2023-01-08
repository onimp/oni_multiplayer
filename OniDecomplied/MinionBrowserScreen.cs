// Decompiled with JetBrains decompiler
// Type: MinionBrowserScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinionBrowserScreen : KMonoBehaviour
{
  [Header("ItemGalleryColumn")]
  [SerializeField]
  private RectTransform galleryGridContent;
  [SerializeField]
  private GameObject gridItemPrefab;
  private GridLayouter gridLayouter;
  [Header("SelectionDetailsColumn")]
  [SerializeField]
  private KleiPermitDioramaVis permitVis;
  [SerializeField]
  private UIMinion UIMinion;
  [SerializeField]
  private LocText detailsHeaderText;
  [SerializeField]
  private Image detailHeaderIcon;
  [SerializeField]
  private OutfitDescriptionPanel outfitDescriptionPanel;
  [Header("Outfit Cycler")]
  [SerializeField]
  private KButton cycleOutfitTypeLeft;
  [SerializeField]
  private KButton cycleOutfitTypeRight;
  [SerializeField]
  private LocText outfitTypeLabel;
  [SerializeField]
  private KButton editOutfitButton;
  [SerializeField]
  private KButton changeOutfitButton;
  private ClothingOutfitUtility.OutfitType selectedOutfitType;
  private Option<ClothingOutfitTarget> selectedOutfit;
  private string selectedPersonalityId;
  private Dictionary<string, MultiToggle> personalities = new Dictionary<string, MultiToggle>();
  private bool isFirstDisplay = true;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.gridLayouter = new GridLayouter()
    {
      minCellSize = 112f,
      maxCellSize = 144f,
      targetGridLayout = ((Component) this.galleryGridContent).GetComponent<GridLayoutGroup>()
    };
  }

  protected virtual void OnCmpEnable()
  {
    if (this.isFirstDisplay)
    {
      this.isFirstDisplay = false;
      this.PopulateGallery();
      this.RefreshPreview();
      this.cycleOutfitTypeLeft.onClick += (System.Action) (() => this.CycleOutfitSelection(-1));
      this.cycleOutfitTypeRight.onClick += (System.Action) (() => this.CycleOutfitSelection(1));
      this.editOutfitButton.onClick += new System.Action(this.OnClickEditOutfit);
      this.changeOutfitButton.onClick += new System.Action(this.OnClickChangeOutfit);
    }
    else
    {
      this.RefreshGalleryButtons();
      this.RefreshPreview();
    }
    KleiItemsStatusRefresher.AddOrGetListener((Component) this).OnRefreshUI((System.Action) (() =>
    {
      this.RefreshGalleryButtons();
      this.RefreshPreview();
    }));
    KleiItemsStatusRefresher.RequestRefreshFromServer();
  }

  private void Update() => this.gridLayouter.CheckIfShouldResizeGrid();

  public void PopulateGallery()
  {
    foreach (KeyValuePair<string, MultiToggle> personality in this.personalities)
      Util.KDestroyGameObject((Component) personality.Value);
    this.personalities.Clear();
    foreach (Personality personality1 in Db.Get().Personalities.GetAll(true, false))
    {
      Personality personality = personality1;
      GameObject gameObject = Util.KInstantiateUI(this.gridItemPrefab, ((Component) this.galleryGridContent).gameObject, true);
      gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = personality.GetMiniIcon();
      ((TMP_Text) gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("Label")).SetText(personality.Name);
      this.personalities.Add(personality.Id, gameObject.GetComponent<MultiToggle>());
      gameObject.GetComponent<MultiToggle>().onClick += (System.Action) (() => this.SelectMinion(personality.Id));
    }
    this.RefreshGalleryButtons();
    this.SelectMinion(Db.Get().Personalities.resources.First<Personality>((Func<Personality, bool>) (d => !d.Disabled)).Id);
  }

  private void SelectMinion(string personalityId)
  {
    this.selectedPersonalityId = personalityId;
    MinionBrowserScreen.MinionVoice.ByPersonality(personalityId).PlaySoundUI("voice_land");
    this.RefreshGalleryButtons();
    this.RefreshPreview();
  }

  private void RefreshGalleryButtons()
  {
    foreach (KeyValuePair<string, MultiToggle> personality in this.personalities)
      personality.Value.ChangeState(personality.Key == this.selectedPersonalityId ? 1 : 0);
  }

  private void CycleOutfitSelection(int direction)
  {
    int num = (int) (this.selectedOutfitType + direction) % 1;
    if (num < 0)
      ++num;
    this.selectedOutfitType = (ClothingOutfitUtility.OutfitType) num;
    this.RefreshPreview();
  }

  public void RefreshPreview()
  {
    Personality personality = Db.Get().Personalities.Get(this.selectedPersonalityId);
    this.UIMinion.SetMinion(personality);
    this.UIMinion.ReactToPersonalityChange();
    ((TMP_Text) this.detailsHeaderText).SetText(personality.Name);
    this.detailHeaderIcon.sprite = personality.GetMiniIcon();
    this.RefreshOutfitDescription();
    ((TMP_Text) this.outfitTypeLabel).SetText(this.selectedOutfitType.GetName());
  }

  private void RefreshOutfitDescription()
  {
    if (this.selectedOutfitType != ClothingOutfitUtility.OutfitType.Clothing)
      return;
    this.selectedOutfit = ClothingOutfitTarget.TryFromId(Db.Get().Personalities.Get(this.selectedPersonalityId).GetOutfit(this.selectedOutfitType));
    this.UIMinion.SetOutfit(this.selectedOutfit);
    this.outfitDescriptionPanel.Refresh(this.selectedOutfit);
  }

  private void OnClickEditOutfit() => OutfitDesignerScreenConfig.Minion(this.selectedOutfit, Db.Get().Personalities.Get(this.selectedPersonalityId)).ApplyAndOpenScreen();

  private void OnClickChangeOutfit()
  {
    OutfitBrowserScreenConfig browserScreenConfig = OutfitBrowserScreenConfig.Minion(Db.Get().Personalities.Get(this.selectedPersonalityId));
    browserScreenConfig = browserScreenConfig.WithOutfit(this.selectedOutfit);
    browserScreenConfig.ApplyAndOpenScreen();
  }

  public readonly struct MinionVoice
  {
    public readonly int voiceIndex;
    public readonly string voiceId;
    public readonly bool isValid;

    public MinionVoice(int voiceIndex)
    {
      this.voiceIndex = voiceIndex;
      this.voiceId = (voiceIndex + 1).ToString("D2");
      this.isValid = true;
    }

    public static MinionBrowserScreen.MinionVoice ByPersonality(string personalityId) => personalityId == "Jorge" ? new MinionBrowserScreen.MinionVoice(-2) : new MinionBrowserScreen.MinionVoice(Random.Range(0, 4));

    public static MinionBrowserScreen.MinionVoice Random() => new MinionBrowserScreen.MinionVoice(Random.Range(0, 4));

    public string GetSoundAssetName(string localName)
    {
      Debug.Assert(this.isValid);
      string str = localName;
      if (localName.Contains(":"))
        str = localName.Split(':')[0];
      return StringFormatter.Combine("DupVoc_", this.voiceId, "_", str);
    }

    public string GetSoundPath(string localName) => GlobalAssets.GetSound(this.GetSoundAssetName(localName), true);

    public void PlaySoundUI(string localName)
    {
      Debug.Assert(this.isValid);
      string soundPath = this.GetSoundPath(localName);
      try
      {
        if (Object.op_Equality((Object) SoundListenerController.Instance, (Object) null))
          KFMOD.PlayUISound(soundPath);
        else
          KFMOD.PlayOneShot(soundPath, TransformExtensions.GetPosition(((Component) SoundListenerController.Instance).transform), 1f);
      }
      catch
      {
        DebugUtil.LogWarningArgs(new object[1]
        {
          (object) ("AUDIOERROR: Missing [" + soundPath + "]")
        });
      }
    }
  }
}
