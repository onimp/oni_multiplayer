// Decompiled with JetBrains decompiler
// Type: ScenariosMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Steamworks;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenariosMenu : KModalScreen, SteamUGCService.IClient
{
  public const string TAG_SCENARIO = "scenario";
  public KButton textButton;
  public KButton dismissButton;
  public KButton closeButton;
  public KButton workshopButton;
  public KButton loadScenarioButton;
  [Space]
  public GameObject ugcContainer;
  public GameObject ugcButtonPrefab;
  public LocText noScenariosText;
  public RectTransform contentRoot;
  public RectTransform detailsRoot;
  public LocText scenarioTitle;
  public LocText scenarioDetails;
  private PublishedFileId_t activeItem;
  private List<GameObject> buttons = new List<GameObject>();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.dismissButton.onClick += (System.Action) (() => this.Deactivate());
    ((TMP_Text) ((Component) this.dismissButton).GetComponent<HierarchyReferences>().GetReference<LocText>("Title")).SetText((string) STRINGS.UI.FRONTEND.OPTIONS_SCREEN.BACK);
    this.closeButton.onClick += (System.Action) (() => this.Deactivate());
    this.workshopButton.onClick += (System.Action) (() => this.OnClickOpenWorkshop());
    this.RebuildScreen();
  }

  private void RebuildScreen()
  {
    foreach (Object button in this.buttons)
      Object.Destroy(button);
    this.buttons.Clear();
    this.RebuildUGCButtons();
  }

  private void RebuildUGCButtons()
  {
    ListPool<SteamUGCService.Mod, ScenariosMenu>.PooledList pooledList = ListPool<SteamUGCService.Mod, ScenariosMenu>.Allocate();
    bool flag1 = ((List<SteamUGCService.Mod>) pooledList).Count > 0;
    ((Component) this.noScenariosText).gameObject.SetActive(!flag1);
    ((Component) this.contentRoot).gameObject.SetActive(flag1);
    bool flag2 = true;
    if (((List<SteamUGCService.Mod>) pooledList).Count != 0)
    {
      for (int index1 = 0; index1 < ((List<SteamUGCService.Mod>) pooledList).Count; ++index1)
      {
        GameObject gameObject = Util.KInstantiateUI(this.ugcButtonPrefab, this.ugcContainer, false);
        ((Object) gameObject).name = ((List<SteamUGCService.Mod>) pooledList)[index1].title + "_button";
        gameObject.gameObject.SetActive(true);
        HierarchyReferences component1 = gameObject.GetComponent<HierarchyReferences>();
        ((TMP_Text) component1.GetReference<LocText>("Title")).SetText(((List<SteamUGCService.Mod>) pooledList)[index1].title);
        Texture2D previewImage = ((List<SteamUGCService.Mod>) pooledList)[index1].previewImage;
        if (Object.op_Inequality((Object) previewImage, (Object) null))
          component1.GetReference<Image>("Image").sprite = Sprite.Create(previewImage, new Rect(Vector2.zero, new Vector2((float) ((Texture) previewImage).width, (float) ((Texture) previewImage).height)), Vector2.op_Multiply(Vector2.one, 0.5f));
        KButton component2 = gameObject.GetComponent<KButton>();
        int index2 = index1;
        PublishedFileId_t item = ((List<SteamUGCService.Mod>) pooledList)[index2].fileId;
        component2.onClick += (System.Action) (() => this.ShowDetails(item));
        component2.onDoubleClick += (System.Action) (() => this.LoadScenario(item));
        this.buttons.Add(gameObject);
        if (PublishedFileId_t.op_Equality(item, this.activeItem))
          flag2 = false;
      }
    }
    if (flag2)
      this.HideDetails();
    pooledList.Recycle();
  }

  private void LoadScenario(PublishedFileId_t item)
  {
    ulong num1;
    string str;
    uint num2;
    SteamUGC.GetItemInstallInfo(item, ref num1, ref str, 1024U, ref num2);
    DebugUtil.LogArgs(new object[4]
    {
      (object) nameof (LoadScenario),
      (object) str,
      (object) num1,
      (object) num2
    });
    byte[] bytesFromZip = SteamUGCService.GetBytesFromZip(item, new string[1]
    {
      ".sav"
    }, out System.DateTime _);
    string path = System.IO.Path.Combine(SaveLoader.GetSavePrefix(), "scenario.sav");
    File.WriteAllBytes(path, bytesFromZip);
    SaveLoader.SetActiveSaveFilePath(path);
    Time.timeScale = 0.0f;
    App.LoadScene("backend");
  }

  private ConfirmDialogScreen GetConfirmDialog()
  {
    KScreen component = KScreenManager.AddChild(((Component) ((KMonoBehaviour) this).transform.parent).gameObject, ((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject).GetComponent<KScreen>();
    component.Activate();
    return ((Component) component).GetComponent<ConfirmDialogScreen>();
  }

  private void ShowDetails(PublishedFileId_t item)
  {
    this.activeItem = item;
    SteamUGCService.Mod mod = SteamUGCService.Instance.FindMod(item);
    if (mod != null)
    {
      ((TMP_Text) this.scenarioTitle).text = mod.title;
      ((TMP_Text) this.scenarioDetails).text = mod.description;
    }
    this.loadScenarioButton.onClick += (System.Action) (() => this.LoadScenario(item));
    ((Component) this.detailsRoot).gameObject.SetActive(true);
  }

  private void HideDetails() => ((Component) this.detailsRoot).gameObject.SetActive(false);

  protected override void OnActivate()
  {
    base.OnActivate();
    SteamUGCService.Instance.AddClient((SteamUGCService.IClient) this);
    this.HideDetails();
  }

  protected override void OnDeactivate()
  {
    base.OnDeactivate();
    SteamUGCService.Instance.RemoveClient((SteamUGCService.IClient) this);
  }

  private void OnClickOpenWorkshop() => App.OpenWebURL("http://steamcommunity.com/workshop/browse/?appid=457140&requiredtags[]=scenario");

  public void UpdateMods(
    IEnumerable<PublishedFileId_t> added,
    IEnumerable<PublishedFileId_t> updated,
    IEnumerable<PublishedFileId_t> removed,
    IEnumerable<SteamUGCService.Mod> loaded_previews)
  {
    this.RebuildScreen();
  }
}
