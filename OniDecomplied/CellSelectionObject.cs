// Decompiled with JetBrains decompiler
// Type: CellSelectionObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CellSelectionObject")]
public class CellSelectionObject : KMonoBehaviour
{
  private static CellSelectionObject selectionObjectA;
  private static CellSelectionObject selectionObjectB;
  [HideInInspector]
  public CellSelectionObject alternateSelectionObject;
  private float zDepth = Grid.GetLayerZ(Grid.SceneLayer.WorldSelection) - 0.5f;
  private float zDepthSelected = Grid.GetLayerZ(Grid.SceneLayer.WorldSelection);
  private KBoxCollider2D mCollider;
  private KSelectable mSelectable;
  private Vector3 offset = new Vector3(0.5f, 0.5f, 0.0f);
  public GameObject SelectedDisplaySprite;
  public Sprite Sprite_Selected;
  public Sprite Sprite_Hover;
  public int mouseCell;
  private int selectedCell;
  public string ElementName;
  public Element element;
  public Element.State state;
  public float Mass;
  public float temperature;
  public Tag tags;
  public byte diseaseIdx;
  public int diseaseCount;
  private float updateTimer;
  private Dictionary<HashedString, Func<bool>> overlayFilterMap = new Dictionary<HashedString, Func<bool>>();
  private bool isAppFocused = true;

  public int SelectedCell => this.selectedCell;

  public float FlowRate => Grid.AccumulatedFlow[this.selectedCell] / 3f;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.mCollider = ((Component) this).GetComponent<KBoxCollider2D>();
    this.mCollider.size = new Vector2(1.1f, 1.1f);
    this.mSelectable = ((Component) this).GetComponent<KSelectable>();
    this.SelectedDisplaySprite.transform.localScale = Vector3.op_Multiply(Vector3.one, 25f / 64f);
    this.SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = this.Sprite_Hover;
    this.Subscribe(((Component) Game.Instance).gameObject, 493375141, new Action<object>(this.ForceRefreshUserMenu));
    this.overlayFilterMap.Add(OverlayModes.Oxygen.ID, (Func<bool>) (() => Grid.Element[this.mouseCell].IsGas));
    this.overlayFilterMap.Add(OverlayModes.GasConduits.ID, (Func<bool>) (() => Grid.Element[this.mouseCell].IsGas));
    this.overlayFilterMap.Add(OverlayModes.LiquidConduits.ID, (Func<bool>) (() => Grid.Element[this.mouseCell].IsLiquid));
    if (Object.op_Equality((Object) CellSelectionObject.selectionObjectA, (Object) null))
      CellSelectionObject.selectionObjectA = this;
    else if (Object.op_Equality((Object) CellSelectionObject.selectionObjectB, (Object) null))
      CellSelectionObject.selectionObjectB = this;
    else
      Debug.LogError((object) "CellSelectionObjects not properly cleaned up.");
  }

  protected virtual void OnCleanUp()
  {
    CellSelectionObject.selectionObjectA = (CellSelectionObject) null;
    CellSelectionObject.selectionObjectB = (CellSelectionObject) null;
    base.OnCleanUp();
  }

  public static bool IsSelectionObject(GameObject testObject) => Object.op_Equality((Object) testObject, (Object) ((Component) CellSelectionObject.selectionObjectA).gameObject) || Object.op_Equality((Object) testObject, (Object) ((Component) CellSelectionObject.selectionObjectB).gameObject);

  private void OnApplicationFocus(bool focusStatus) => this.isAppFocused = focusStatus;

  private void Update()
  {
    if (!this.isAppFocused || Object.op_Equality((Object) SelectTool.Instance, (Object) null) || Object.op_Equality((Object) Game.Instance, (Object) null) || !Game.Instance.GameStarted())
      return;
    this.SelectedDisplaySprite.SetActive(PlayerController.Instance.IsUsingDefaultTool() && !DebugHandler.HideUI);
    if (Object.op_Inequality((Object) SelectTool.Instance.selected, (Object) this.mSelectable))
    {
      this.mouseCell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
      if (Grid.IsValidCell(this.mouseCell) && Grid.IsVisible(this.mouseCell))
      {
        bool flag = true;
        foreach (KeyValuePair<HashedString, Func<bool>> overlayFilter in this.overlayFilterMap)
        {
          if (overlayFilter.Value == null)
            Debug.LogWarning((object) "Filter value is null");
          else if (Object.op_Equality((Object) OverlayScreen.Instance, (Object) null))
            Debug.LogWarning((object) "Overlay screen Instance is null");
          else if (HashedString.op_Equality(OverlayScreen.Instance.GetMode(), overlayFilter.Key))
          {
            flag = false;
            if (((Component) this).gameObject.layer != LayerMask.NameToLayer("MaskedOverlay"))
              ((Component) this).gameObject.layer = LayerMask.NameToLayer("MaskedOverlay");
            if (!overlayFilter.Value())
            {
              this.SelectedDisplaySprite.SetActive(false);
              return;
            }
            break;
          }
        }
        if (flag && ((Component) this).gameObject.layer != LayerMask.NameToLayer("Default"))
          ((Component) this).gameObject.layer = LayerMask.NameToLayer("Default");
        Vector3 vector3 = Vector3.op_Addition(Grid.CellToPos(this.mouseCell, 0.0f, 0.0f, 0.0f), this.offset);
        vector3.z = this.zDepth;
        TransformExtensions.SetPosition(this.transform, vector3);
        this.mSelectable.SetName(Grid.Element[this.mouseCell].name);
      }
      if (Object.op_Inequality((Object) SelectTool.Instance.hover, (Object) this.mSelectable))
        this.SelectedDisplaySprite.SetActive(false);
    }
    this.updateTimer += Time.deltaTime;
    if ((double) this.updateTimer < 0.5)
      return;
    this.updateTimer = 0.0f;
    if (!Object.op_Equality((Object) SelectTool.Instance.selected, (Object) this.mSelectable))
      return;
    this.UpdateValues();
  }

  public void UpdateValues()
  {
    if (!Grid.IsValidCell(this.selectedCell))
      return;
    this.Mass = Grid.Mass[this.selectedCell];
    this.element = Grid.Element[this.selectedCell];
    this.ElementName = this.element.name;
    this.state = this.element.state;
    this.tags = this.element.GetMaterialCategoryTag();
    this.temperature = Grid.Temperature[this.selectedCell];
    this.diseaseIdx = Grid.DiseaseIdx[this.selectedCell];
    this.diseaseCount = Grid.DiseaseCount[this.selectedCell];
    this.mSelectable.SetName(Grid.Element[this.selectedCell].name);
    ((KMonoBehaviour) DetailsScreen.Instance).Trigger(-1514841199, (object) null);
    this.UpdateStatusItem();
    int cell = Grid.CellAbove(this.selectedCell);
    bool flag = this.element.IsLiquid && Grid.IsValidCell(cell) && (Grid.Element[cell].IsGas || Grid.Element[cell].IsVacuum);
    if (this.element.sublimateId != (SimHashes) 0 && this.element.IsSolid | flag)
    {
      this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.SublimationEmitting, (object) this);
      bool all_not_gaseous;
      bool all_over_pressure;
      GameUtil.IsEmissionBlocked(this.selectedCell, out all_not_gaseous, out all_over_pressure);
      if (all_not_gaseous)
      {
        this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.SublimationBlocked, (object) this);
        this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationOverpressure);
      }
      else if (all_over_pressure)
      {
        this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.SublimationOverpressure, (object) this);
        this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationBlocked);
      }
      else
      {
        this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationOverpressure);
        this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationBlocked);
      }
    }
    else
    {
      this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationEmitting);
      this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationBlocked);
      this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationOverpressure);
    }
    if (((Component) Game.Instance).GetComponent<EntombedItemVisualizer>().IsEntombedItem(this.selectedCell))
      this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.BuriedItem, (object) this);
    else
      this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.BuriedItem, true);
    bool space = CellSelectionObject.IsExposedToSpace(this.selectedCell);
    this.mSelectable.ToggleStatusItem(Db.Get().MiscStatusItems.Space, space);
  }

  public static bool IsExposedToSpace(int cell) => Game.Instance.world.zoneRenderData.GetSubWorldZoneType(cell) == 7 && Object.op_Equality((Object) Grid.Objects[cell, 2], (Object) null);

  private void UpdateStatusItem()
  {
    if (this.element.id == SimHashes.Vacuum || this.element.id == SimHashes.Void)
    {
      this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalCategory, true);
      this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalTemperature, true);
      this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalMass, true);
      this.mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalDisease, true);
    }
    else
    {
      if (!this.mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalCategory))
      {
        Func<Element> data = (Func<Element>) (() => this.element);
        this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalCategory, (object) data);
      }
      if (!this.mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalTemperature))
        this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalTemperature, (object) this);
      if (!this.mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalMass))
        this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalMass, (object) this);
      if (this.mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalDisease))
        return;
      this.mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalDisease, (object) this);
    }
  }

  public void OnObjectSelected(object o)
  {
    this.SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = this.Sprite_Hover;
    this.UpdateStatusItem();
    if (!Object.op_Equality((Object) SelectTool.Instance.selected, (Object) this.mSelectable))
      return;
    this.selectedCell = Grid.PosToCell(((Component) this).gameObject);
    this.UpdateValues();
    Vector3 vector3 = Vector3.op_Addition(Grid.CellToPos(this.selectedCell, 0.0f, 0.0f, 0.0f), this.offset);
    vector3.z = this.zDepthSelected;
    TransformExtensions.SetPosition(this.transform, vector3);
    this.SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = this.Sprite_Selected;
  }

  public string MassString() => string.Format("{0:0.00}", (object) this.Mass);

  private void ForceRefreshUserMenu(object data) => Game.Instance.userMenu.Refresh(((Component) this).gameObject);
}
