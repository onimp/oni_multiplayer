// Decompiled with JetBrains decompiler
// Type: RootMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RootMenu : KScreen
{
  private DetailsScreen detailsScreen;
  private UserMenuScreen userMenu;
  [SerializeField]
  private GameObject detailsScreenPrefab;
  [SerializeField]
  private UserMenuScreen userMenuPrefab;
  private GameObject userMenuParent;
  [SerializeField]
  private TileScreen tileScreen;
  public KScreen buildMenu;
  private List<KScreen> subMenus = new List<KScreen>();
  private TileScreen tileScreenInst;
  public bool canTogglePauseScreen = true;
  public GameObject selectedGO;

  public static void DestroyInstance() => RootMenu.Instance = (RootMenu) null;

  public static RootMenu Instance { get; private set; }

  public virtual float GetSortKey() => -1f;

  protected virtual void OnPrefabInit()
  {
    RootMenu.Instance = this;
    ((KMonoBehaviour) this).Subscribe(((Component) Game.Instance).gameObject, -1503271301, new Action<object>(this.OnSelectObject));
    ((KMonoBehaviour) this).Subscribe(((Component) Game.Instance).gameObject, 288942073, new Action<object>(this.OnUIClear));
    ((KMonoBehaviour) this).Subscribe(((Component) Game.Instance).gameObject, -809948329, new Action<object>(this.OnBuildingStatechanged));
    base.OnPrefabInit();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.detailsScreen = Util.KInstantiateUI(this.detailsScreenPrefab, ((Component) this).gameObject, true).GetComponent<DetailsScreen>();
    ((Component) this.detailsScreen).gameObject.SetActive(true);
    this.userMenuParent = ((Component) this.detailsScreen.UserMenuPanel).gameObject;
    this.userMenu = Util.KInstantiateUI(((Component) this.userMenuPrefab).gameObject, this.userMenuParent, false).GetComponent<UserMenuScreen>();
    ((Component) this.detailsScreen).gameObject.SetActive(false);
    ((Component) this.userMenu).gameObject.SetActive(false);
  }

  private void OnClickCommon() => this.CloseSubMenus();

  public void AddSubMenu(KScreen sub_menu)
  {
    if (sub_menu.activateOnSpawn)
      sub_menu.Show(true);
    this.subMenus.Add(sub_menu);
  }

  public void RemoveSubMenu(KScreen sub_menu) => this.subMenus.Remove(sub_menu);

  private void CloseSubMenus()
  {
    foreach (KScreen subMenu in this.subMenus)
    {
      if (Object.op_Inequality((Object) subMenu, (Object) null))
      {
        if (subMenu.activateOnSpawn)
          ((Component) subMenu).gameObject.SetActive(false);
        else
          subMenu.Deactivate();
      }
    }
    this.subMenus.Clear();
  }

  private void OnSelectObject(object data)
  {
    GameObject testObject = (GameObject) data;
    bool flag = false;
    if (Object.op_Inequality((Object) testObject, (Object) null))
    {
      KPrefabID component = testObject.GetComponent<KPrefabID>();
      if (Object.op_Inequality((Object) component, (Object) null) && !((KMonoBehaviour) component).IsInitialized())
        return;
      flag = Object.op_Inequality((Object) component, (Object) null) || CellSelectionObject.IsSelectionObject(testObject);
    }
    if (Object.op_Inequality((Object) testObject, (Object) this.selectedGO))
    {
      this.selectedGO = (GameObject) null;
      this.CloseSubMenus();
      if (flag)
      {
        this.selectedGO = testObject;
        this.AddSubMenu((KScreen) this.detailsScreen);
        this.AddSubMenu((KScreen) this.userMenu);
      }
      this.userMenu.SetSelected(this.selectedGO);
    }
    this.Refresh();
  }

  public void Refresh()
  {
    if (Object.op_Equality((Object) this.selectedGO, (Object) null))
      return;
    this.detailsScreen.Refresh(this.selectedGO);
    this.userMenu.Refresh(this.selectedGO);
  }

  private void OnBuildingStatechanged(object data)
  {
    GameObject data1 = (GameObject) data;
    if (!Object.op_Equality((Object) data1, (Object) this.selectedGO))
      return;
    this.OnSelectObject((object) data1);
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (!((KInputEvent) e).Consumed && e.TryConsume((Action) 1) && ((Behaviour) SelectTool.Instance).enabled)
    {
      if (!this.canTogglePauseScreen)
        return;
      if (this.AreSubMenusOpen())
      {
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Back"));
        this.CloseSubMenus();
        SelectTool.Instance.Select((KSelectable) null);
      }
      else if (e.IsAction((Action) 1))
      {
        if (!((Behaviour) SelectTool.Instance).enabled)
          KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
        if (PlayerController.Instance.IsUsingDefaultTool())
        {
          if (Object.op_Inequality((Object) SelectTool.Instance.selected, (Object) null))
          {
            SelectTool.Instance.Select((KSelectable) null);
          }
          else
          {
            CameraController.Instance.ForcePanningState(false);
            this.TogglePauseScreen();
          }
        }
        else
          Game.Instance.Trigger(288942073, (object) null);
        ToolMenu.Instance.ClearSelection();
        SelectTool.Instance.Activate();
      }
    }
    base.OnKeyDown(e);
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    base.OnKeyUp(e);
    if (((KInputEvent) e).Consumed || !e.TryConsume((Action) 13) || !Object.op_Inequality((Object) this.tileScreenInst, (Object) null))
      return;
    this.tileScreenInst.Deactivate();
    this.tileScreenInst = (TileScreen) null;
  }

  public void TogglePauseScreen() => PauseScreen.Instance.Show(true);

  public void ExternalClose() => this.OnClickCommon();

  private void OnUIClear(object data)
  {
    this.CloseSubMenus();
    if (Object.op_Inequality((Object) EventSystem.current, (Object) null))
      EventSystem.current.SetSelectedGameObject((GameObject) null);
    else
      Debug.LogWarning((object) "OnUIClear() Event system is null");
  }

  protected virtual void OnActivate() => base.OnActivate();

  private bool AreSubMenusOpen() => this.subMenus.Count > 0;

  private KToggleMenu.ToggleInfo[] GetFillers()
  {
    HashSet<Tag> tagSet = new HashSet<Tag>();
    List<KToggleMenu.ToggleInfo> toggleInfoList = new List<KToggleMenu.ToggleInfo>();
    foreach (Pickupable pickupable in Components.Pickupables.Items)
    {
      KPrefabID kprefabId = pickupable.KPrefabID;
      if (kprefabId.HasTag(GameTags.Filler) && tagSet.Add(kprefabId.PrefabTag))
      {
        string text = ((Component) kprefabId).GetComponent<PrimaryElement>().Element.id.ToString();
        toggleInfoList.Add(new KToggleMenu.ToggleInfo(text));
      }
    }
    return toggleInfoList.ToArray();
  }

  public bool IsBuildingChorePanelActive() => Object.op_Inequality((Object) this.detailsScreen, (Object) null) && this.detailsScreen.GetActiveTab() is BuildingChoresPanel;
}
