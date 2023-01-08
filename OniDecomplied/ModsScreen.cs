// Decompiled with JetBrains decompiler
// Type: ModsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KMod;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModsScreen : KModalScreen
{
  [SerializeField]
  private KButton closeButtonTitle;
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private KButton toggleAllButton;
  [SerializeField]
  private KButton workshopButton;
  [SerializeField]
  private GameObject entryPrefab;
  [SerializeField]
  private Transform entryParent;
  private List<ModsScreen.DisplayedMod> displayedMods = new List<ModsScreen.DisplayedMod>();
  private List<KMod.Label> mod_footprint = new List<KMod.Label>();

  protected override void OnActivate()
  {
    base.OnActivate();
    this.closeButtonTitle.onClick += new System.Action(this.Exit);
    this.closeButton.onClick += new System.Action(this.Exit);
    this.workshopButton.onClick += (System.Action) (() => App.OpenWebURL("http://steamcommunity.com/workshop/browse/?appid=457140"));
    this.UpdateToggleAllButton();
    this.toggleAllButton.onClick += new System.Action(this.OnToggleAllClicked);
    Global.Instance.modManager.Sanitize(((Component) this).gameObject);
    this.mod_footprint.Clear();
    foreach (KMod.Mod mod in Global.Instance.modManager.mods)
    {
      if (mod.IsEnabledForActiveDlc())
      {
        this.mod_footprint.Add(mod.label);
        if ((mod.loaded_content & (Content.LayerableFiles | Content.Strings | Content.DLL | Content.Translation | Content.Animation)) == (mod.available_content & (Content.LayerableFiles | Content.Strings | Content.DLL | Content.Translation | Content.Animation)))
          mod.Uncrash();
      }
    }
    this.BuildDisplay();
    Global.Instance.modManager.on_update += new Manager.OnUpdate(this.RebuildDisplay);
  }

  protected override void OnDeactivate()
  {
    Global.Instance.modManager.on_update -= new Manager.OnUpdate(this.RebuildDisplay);
    base.OnDeactivate();
  }

  private void Exit()
  {
    Global.Instance.modManager.Save();
    if (!Global.Instance.modManager.MatchFootprint(this.mod_footprint, Content.LayerableFiles | Content.Strings | Content.DLL | Content.Translation | Content.Animation))
      Global.Instance.modManager.RestartDialog((string) STRINGS.UI.FRONTEND.MOD_DIALOGS.MODS_SCREEN_CHANGES.TITLE, (string) STRINGS.UI.FRONTEND.MOD_DIALOGS.MODS_SCREEN_CHANGES.MESSAGE, new System.Action(((KScreen) this).Deactivate), true, ((Component) this).gameObject);
    else
      this.Deactivate();
    Global.Instance.modManager.events.Clear();
  }

  private void RebuildDisplay(object change_source)
  {
    if (change_source == this)
      return;
    this.BuildDisplay();
  }

  private bool ShouldDisplayMod(KMod.Mod mod) => mod.status != KMod.Mod.Status.NotInstalled && mod.status != KMod.Mod.Status.UninstallPending && !mod.HasOnlyTranslationContent();

  private void BuildDisplay()
  {
    foreach (ModsScreen.DisplayedMod displayedMod in this.displayedMods)
    {
      if (Object.op_Inequality((Object) displayedMod.rect_transform, (Object) null))
        Object.Destroy((Object) ((Component) displayedMod.rect_transform).gameObject);
    }
    this.displayedMods.Clear();
    ModsScreen.ModOrderingDragListener orderingDragListener = new ModsScreen.ModOrderingDragListener(this, this.displayedMods);
    for (int index = 0; index != Global.Instance.modManager.mods.Count; ++index)
    {
      KMod.Mod mod = Global.Instance.modManager.mods[index];
      if (this.ShouldDisplayMod(mod))
      {
        HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(this.entryPrefab, ((Component) this.entryParent).gameObject, false);
        this.displayedMods.Add(new ModsScreen.DisplayedMod()
        {
          rect_transform = ((Component) hierarchyReferences).gameObject.GetComponent<RectTransform>(),
          mod_index = index
        });
        ((Component) hierarchyReferences).GetComponent<DragMe>().listener = (DragMe.IDragListener) orderingDragListener;
        LocText reference1 = hierarchyReferences.GetReference<LocText>("Title");
        string title = mod.title;
        ((Object) hierarchyReferences).name = mod.title;
        if (mod.available_content == (Content) 0)
        {
          switch (mod.contentCompatability)
          {
            case ModContentCompatability.NoContent:
              title += (string) STRINGS.UI.FRONTEND.MODS.CONTENT_FAILURE.NO_CONTENT;
              break;
            case ModContentCompatability.OldAPI:
              title += (string) STRINGS.UI.FRONTEND.MODS.CONTENT_FAILURE.OLD_API;
              break;
            default:
              title += STRINGS.UI.FRONTEND.MODS.CONTENT_FAILURE.DISABLED_CONTENT.Replace("{Content}", ModsScreen.GetDlcName(DlcManager.GetHighestActiveDlcId()));
              break;
          }
        }
        ((TMP_Text) reference1).text = title;
        LocText reference2 = hierarchyReferences.GetReference<LocText>("Version");
        if (mod.packagedModInfo != null && mod.packagedModInfo.version != null && mod.packagedModInfo.version.Length > 0)
        {
          string str = mod.packagedModInfo.version;
          if (str.StartsWith("V"))
            str = "v" + str.Substring(1, str.Length - 1);
          else if (!str.StartsWith("v"))
            str = "v" + str;
          ((TMP_Text) reference2).text = str;
          ((Component) reference2).gameObject.SetActive(true);
        }
        else
          ((Component) reference2).gameObject.SetActive(false);
        hierarchyReferences.GetReference<ToolTip>("Description").toolTip = mod.description;
        if (mod.crash_count != 0)
          ((Graphic) reference1).color = Color.Lerp(Color.white, Color.red, (float) mod.crash_count / 3f);
        KButton reference3 = hierarchyReferences.GetReference<KButton>("ManageButton");
        ((TMP_Text) ((Component) reference3).GetComponentInChildren<LocText>()).text = (string) (mod.IsLocal ? STRINGS.UI.FRONTEND.MODS.MANAGE_LOCAL : STRINGS.UI.FRONTEND.MODS.MANAGE);
        reference3.isInteractable = mod.is_managed;
        if (reference3.isInteractable)
        {
          ((Component) reference3).GetComponent<ToolTip>().toolTip = (string) mod.manage_tooltip;
          reference3.onClick += mod.on_managed;
        }
        KImage reference4 = hierarchyReferences.GetReference<KImage>("BG");
        MultiToggle toggle = hierarchyReferences.GetReference<MultiToggle>("EnabledToggle");
        toggle.ChangeState(mod.IsEnabledForActiveDlc() ? 1 : 0);
        if (mod.available_content != (Content) 0)
        {
          reference4.defaultState = (KImage.ColorSelector) 1;
          reference4.ColorState = (KImage.ColorSelector) 1;
          toggle.onClick += (System.Action) (() => this.OnToggleClicked(toggle, mod.label));
          ((Component) toggle).GetComponent<ToolTip>().OnToolTip = (Func<string>) (() => (string) (mod.IsEnabledForActiveDlc() ? STRINGS.UI.FRONTEND.MODS.TOOLTIPS.ENABLED : STRINGS.UI.FRONTEND.MODS.TOOLTIPS.DISABLED));
        }
        else
        {
          reference4.defaultState = (KImage.ColorSelector) 2;
          reference4.ColorState = (KImage.ColorSelector) 2;
        }
        ((Component) hierarchyReferences).gameObject.SetActive(true);
      }
    }
    foreach (ModsScreen.DisplayedMod displayedMod in this.displayedMods)
      ((Component) displayedMod.rect_transform).gameObject.SetActive(true);
    int count = this.displayedMods.Count;
  }

  private static string GetDlcName(string dlcId)
  {
    switch (dlcId)
    {
      case "EXPANSION1_ID":
        return (string) STRINGS.UI.DLC1.NAME_ITAL;
      default:
        return (string) STRINGS.UI.VANILLA.NAME_ITAL;
    }
  }

  private void OnToggleClicked(MultiToggle toggle, KMod.Label mod)
  {
    Manager modManager = Global.Instance.modManager;
    bool enabled = !modManager.IsModEnabled(mod);
    toggle.ChangeState(enabled ? 1 : 0);
    modManager.EnableMod(mod, enabled, (object) this);
    this.UpdateToggleAllButton();
  }

  private bool AreAnyModsDisabled() => Global.Instance.modManager.mods.Any<KMod.Mod>((Func<KMod.Mod, bool>) (mod => !mod.IsEmpty() && !mod.IsEnabledForActiveDlc() && this.ShouldDisplayMod(mod)));

  private void UpdateToggleAllButton() => ((TMP_Text) ((Component) this.toggleAllButton).GetComponentInChildren<LocText>()).text = (string) (this.AreAnyModsDisabled() ? STRINGS.UI.FRONTEND.MODS.ENABLE_ALL : STRINGS.UI.FRONTEND.MODS.DISABLE_ALL);

  private void OnToggleAllClicked()
  {
    bool enabled = this.AreAnyModsDisabled();
    Manager modManager = Global.Instance.modManager;
    foreach (KMod.Mod mod in modManager.mods)
    {
      if (this.ShouldDisplayMod(mod))
        modManager.EnableMod(mod.label, enabled, (object) this);
    }
    this.BuildDisplay();
    this.UpdateToggleAllButton();
  }

  private struct DisplayedMod
  {
    public RectTransform rect_transform;
    public int mod_index;
  }

  private class ModOrderingDragListener : DragMe.IDragListener
  {
    private List<ModsScreen.DisplayedMod> mods;
    private ModsScreen screen;
    private int startDragIdx = -1;

    public ModOrderingDragListener(ModsScreen screen, List<ModsScreen.DisplayedMod> mods)
    {
      this.screen = screen;
      this.mods = mods;
    }

    public void OnBeginDrag(Vector2 pos) => this.startDragIdx = this.GetDragIdx(pos, false);

    public void OnEndDrag(Vector2 pos)
    {
      if (this.startDragIdx < 0)
        return;
      int dragIdx = this.GetDragIdx(pos, true);
      if (dragIdx == this.startDragIdx)
        return;
      Global.Instance.modManager.Reinsert(this.mods[this.startDragIdx].mod_index, 0 > dragIdx || dragIdx >= this.mods.Count ? -1 : this.mods[dragIdx].mod_index, dragIdx >= this.mods.Count, (object) this);
      this.screen.BuildDisplay();
    }

    private int GetDragIdx(Vector2 pos, bool halfPosition)
    {
      int dragIdx = -1;
      for (int index = 0; index < this.mods.Count; ++index)
      {
        Vector2 vector2_1;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.mods[index].rect_transform, pos, (Camera) null, ref vector2_1);
        if (!halfPosition)
        {
          Vector2 vector2_2 = vector2_1;
          Rect rect = this.mods[index].rect_transform.rect;
          Vector2 min = ((Rect) ref rect).min;
          vector2_1 = Vector2.op_Addition(vector2_2, min);
        }
        if ((double) vector2_1.y < 0.0)
          dragIdx = index;
        else
          break;
      }
      return dragIdx;
    }
  }
}
