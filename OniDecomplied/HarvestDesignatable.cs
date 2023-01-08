// Decompiled with JetBrains decompiler
// Type: HarvestDesignatable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/HarvestDesignatable")]
public class HarvestDesignatable : KMonoBehaviour
{
  public bool defaultHarvestStateWhenPlanted = true;
  public OccupyArea area;
  [Serialize]
  protected bool isMarkedForHarvest;
  [Serialize]
  private bool isInPlanterBox;
  public bool showUserMenuButtons = true;
  [Serialize]
  protected bool harvestWhenReady;
  public RectTransform HarvestWhenReadyOverlayIcon;
  private Action<object> onEnableOverlayDelegate;
  private static readonly EventSystem.IntraObjectHandler<HarvestDesignatable> OnCancelDelegate = new EventSystem.IntraObjectHandler<HarvestDesignatable>((Action<HarvestDesignatable, object>) ((component, data) => component.OnCancel(data)));
  private static readonly EventSystem.IntraObjectHandler<HarvestDesignatable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<HarvestDesignatable>((Action<HarvestDesignatable, object>) ((component, data) => component.OnRefreshUserMenu(data)));
  private static readonly EventSystem.IntraObjectHandler<HarvestDesignatable> SetInPlanterBoxTrueDelegate = new EventSystem.IntraObjectHandler<HarvestDesignatable>((Action<HarvestDesignatable, object>) ((component, data) => component.SetInPlanterBox(true)));

  public bool InPlanterBox => this.isInPlanterBox;

  public bool MarkedForHarvest
  {
    get => this.isMarkedForHarvest;
    set => this.isMarkedForHarvest = value;
  }

  public bool HarvestWhenReady => this.harvestWhenReady;

  protected HarvestDesignatable() => this.onEnableOverlayDelegate = new Action<object>(this.OnEnableOverlay);

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<HarvestDesignatable>(1309017699, HarvestDesignatable.SetInPlanterBoxTrueDelegate);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.isMarkedForHarvest)
      this.MarkForHarvest();
    Components.HarvestDesignatables.Add(this);
    this.Subscribe<HarvestDesignatable>(493375141, HarvestDesignatable.OnRefreshUserMenuDelegate);
    this.Subscribe<HarvestDesignatable>(2127324410, HarvestDesignatable.OnCancelDelegate);
    Game.Instance.Subscribe(1248612973, this.onEnableOverlayDelegate);
    Game.Instance.Subscribe(1798162660, this.onEnableOverlayDelegate);
    Game.Instance.Subscribe(2015652040, new Action<object>(this.OnDisableOverlay));
    this.area = ((Component) this).GetComponent<OccupyArea>();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Components.HarvestDesignatables.Remove(this);
    this.DestroyOverlayIcon();
    Game.Instance.Unsubscribe(1248612973, this.onEnableOverlayDelegate);
    Game.Instance.Unsubscribe(2015652040, new Action<object>(this.OnDisableOverlay));
    Game.Instance.Unsubscribe(1798162660, this.onEnableOverlayDelegate);
  }

  private void DestroyOverlayIcon()
  {
    if (!Object.op_Inequality((Object) this.HarvestWhenReadyOverlayIcon, (Object) null))
      return;
    Object.Destroy((Object) ((Component) this.HarvestWhenReadyOverlayIcon).gameObject);
    this.HarvestWhenReadyOverlayIcon = (RectTransform) null;
  }

  private void CreateOverlayIcon()
  {
    if (Object.op_Inequality((Object) this.HarvestWhenReadyOverlayIcon, (Object) null) || !Object.op_Equality((Object) ((Component) this).GetComponent<AttackableBase>(), (Object) null))
      return;
    this.HarvestWhenReadyOverlayIcon = Util.KInstantiate((Component) Assets.UIPrefabs.HarvestWhenReadyOverlayIcon, GameScreenManager.Instance.worldSpaceCanvas, (string) null).GetComponent<RectTransform>();
    Extents extents = ((Component) this).GetComponent<OccupyArea>().GetExtents();
    Vector3 vector3;
    if (((Component) this).GetComponent<KPrefabID>().HasTag(GameTags.Hanging))
    {
      // ISSUE: explicit constructor call
      ((Vector3) ref vector3).\u002Ector((float) (extents.x + extents.width / 2) + 0.5f, (float) (extents.y + extents.height));
    }
    else
    {
      // ISSUE: explicit constructor call
      ((Vector3) ref vector3).\u002Ector((float) (extents.x + extents.width / 2) + 0.5f, (float) extents.y);
    }
    TransformExtensions.SetPosition(((Component) this.HarvestWhenReadyOverlayIcon).transform, vector3);
    this.RefreshOverlayIcon();
  }

  private void OnDisableOverlay(object data) => this.DestroyOverlayIcon();

  private void OnEnableOverlay(object data)
  {
    if (HashedString.op_Equality((HashedString) data, OverlayModes.Harvest.ID))
      this.CreateOverlayIcon();
    else
      this.DestroyOverlayIcon();
  }

  private void RefreshOverlayIcon(object data = null)
  {
    if (!Object.op_Inequality((Object) this.HarvestWhenReadyOverlayIcon, (Object) null))
      return;
    if (Grid.IsVisible(Grid.PosToCell(((Component) this).gameObject)) || Object.op_Inequality((Object) CameraController.Instance, (Object) null) && CameraController.Instance.FreeCameraEnabled)
    {
      if (!((Component) this.HarvestWhenReadyOverlayIcon).gameObject.activeSelf)
        ((Component) this.HarvestWhenReadyOverlayIcon).gameObject.SetActive(true);
    }
    else if (((Component) this.HarvestWhenReadyOverlayIcon).gameObject.activeSelf)
      ((Component) this.HarvestWhenReadyOverlayIcon).gameObject.SetActive(false);
    HierarchyReferences component = ((Component) this.HarvestWhenReadyOverlayIcon).GetComponent<HierarchyReferences>();
    if (this.harvestWhenReady)
    {
      Image reference = (Image) component.GetReference("On");
      ((Component) reference).gameObject.SetActive(true);
      ((Graphic) reference).color = Color32.op_Implicit(GlobalAssets.Instance.colorSet.harvestEnabled);
      component.GetReference("Off").gameObject.SetActive(false);
    }
    else
    {
      component.GetReference("On").gameObject.SetActive(false);
      Image reference = (Image) component.GetReference("Off");
      ((Component) reference).gameObject.SetActive(true);
      ((Graphic) reference).color = Color32.op_Implicit(GlobalAssets.Instance.colorSet.harvestDisabled);
    }
  }

  public bool CanBeHarvested()
  {
    Harvestable component = ((Component) this).GetComponent<Harvestable>();
    return !Object.op_Inequality((Object) component, (Object) null) || component.CanBeHarvested;
  }

  public void SetInPlanterBox(bool state)
  {
    if (state)
    {
      if (this.isInPlanterBox)
        return;
      this.isInPlanterBox = true;
      this.SetHarvestWhenReady(this.defaultHarvestStateWhenPlanted);
    }
    else
      this.isInPlanterBox = false;
  }

  public void SetHarvestWhenReady(bool state)
  {
    this.harvestWhenReady = state;
    if (this.harvestWhenReady && this.CanBeHarvested() && !this.isMarkedForHarvest)
      this.MarkForHarvest();
    if (this.isMarkedForHarvest && !this.harvestWhenReady)
    {
      this.OnCancel();
      if (this.CanBeHarvested() && this.isInPlanterBox)
        ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest, (object) this);
    }
    this.Trigger(-266953818, (object) null);
    this.RefreshOverlayIcon();
  }

  protected virtual void OnCancel(object data = null)
  {
  }

  public virtual void MarkForHarvest()
  {
    if (!this.CanBeHarvested())
      return;
    this.isMarkedForHarvest = true;
    Harvestable component = ((Component) this).GetComponent<Harvestable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.OnMarkedForHarvest();
  }

  protected virtual void OnClickHarvestWhenReady() => this.SetHarvestWhenReady(true);

  protected virtual void OnClickCancelHarvestWhenReady() => this.SetHarvestWhenReady(false);

  public virtual void OnRefreshUserMenu(object data)
  {
    if (!this.showUserMenuButtons)
      return;
    Game.Instance.userMenu.AddButton(((Component) this).gameObject, this.harvestWhenReady ? new KIconButtonMenu.ButtonInfo("action_harvest", (string) STRINGS.UI.USERMENUACTIONS.CANCEL_HARVEST_WHEN_READY.NAME, (System.Action) (() =>
    {
      this.OnClickCancelHarvestWhenReady();
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, (string) STRINGS.UI.GAMEOBJECTEFFECTS.PLANT_DO_NOT_HARVEST, this.transform);
    }), tooltipText: ((string) STRINGS.UI.USERMENUACTIONS.CANCEL_HARVEST_WHEN_READY.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_harvest", (string) STRINGS.UI.USERMENUACTIONS.HARVEST_WHEN_READY.NAME, (System.Action) (() =>
    {
      this.OnClickHarvestWhenReady();
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, (string) STRINGS.UI.GAMEOBJECTEFFECTS.PLANT_MARK_FOR_HARVEST, this.transform);
    }), tooltipText: ((string) STRINGS.UI.USERMENUACTIONS.HARVEST_WHEN_READY.TOOLTIP)));
  }
}
