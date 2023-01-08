// Decompiled with JetBrains decompiler
// Type: ToolMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ToolMenu : KScreen
{
  public static ToolMenu Instance;
  public GameObject Prefab_collectionContainer;
  public GameObject Prefab_collectionContainerWindow;
  public PriorityScreen Prefab_priorityScreen;
  public GameObject toolIconPrefab;
  public GameObject toolIconLargePrefab;
  public GameObject sandboxToolIconPrefab;
  public GameObject collectionIconPrefab;
  public GameObject prefabToolRow;
  public GameObject largeToolSet;
  public GameObject smallToolSet;
  public GameObject smallToolBottomRow;
  public GameObject smallToolTopRow;
  public GameObject sandboxToolSet;
  private PriorityScreen priorityScreen;
  public ToolParameterMenu toolParameterMenu;
  public GameObject sandboxToolParameterMenu;
  private GameObject toolEffectDisplayPlane;
  private Texture2D toolEffectDisplayPlaneTexture;
  public Material toolEffectDisplayMaterial;
  private byte[] toolEffectDisplayBytes;
  private List<List<ToolMenu.ToolCollection>> rows = new List<List<ToolMenu.ToolCollection>>();
  public List<ToolMenu.ToolCollection> basicTools = new List<ToolMenu.ToolCollection>();
  public List<ToolMenu.ToolCollection> sandboxTools = new List<ToolMenu.ToolCollection>();
  public ToolMenu.ToolCollection currentlySelectedCollection;
  public ToolMenu.ToolInfo currentlySelectedTool;
  public InterfaceTool activeTool;
  private Coroutine activeOpenAnimationRoutine;
  private Coroutine activeCloseAnimationRoutine;
  private HashSet<Action> boundRootActions = new HashSet<Action>();
  private HashSet<Action> boundSubgroupActions = new HashSet<Action>();
  private UnityAction inputChangeReceiver;
  private int refreshScaleHandle = -1;
  [SerializeField]
  public TextStyleSetting ToggleToolTipTextStyleSetting;
  [SerializeField]
  public TextStyleSetting CategoryLabelTextStyle_LeftAlign;
  private int smallCollectionMax = 5;
  private HashSet<ToolMenu.CellColorData> colors = new HashSet<ToolMenu.CellColorData>();

  public static void DestroyInstance() => ToolMenu.Instance = (ToolMenu) null;

  public PriorityScreen PriorityScreen => this.priorityScreen;

  public virtual float GetSortKey() => 5f;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ToolMenu.Instance = this;
    Game.Instance.Subscribe(1798162660, new Action<object>(this.OnOverlayChanged));
    this.priorityScreen = Util.KInstantiateUI<PriorityScreen>(((Component) this.Prefab_priorityScreen).gameObject, ((Component) this).gameObject, false);
    this.priorityScreen.InstantiateButtons(new Action<PrioritySetting>(this.OnPriorityClicked), false);
  }

  protected virtual void OnForcedCleanUp()
  {
    // ISSUE: method pointer
    KInputManager.InputChange.RemoveListener(new UnityAction((object) this, __methodptr(OnInputChange)));
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Game.Instance.Unsubscribe(1798162660, new Action<object>(this.OnOverlayChanged));
    Game.Instance.Unsubscribe(this.refreshScaleHandle);
  }

  private void OnOverlayChanged(object overlay_data)
  {
    HashedString hashedString = (HashedString) overlay_data;
    if (!Object.op_Inequality((Object) PlayerController.Instance.ActiveTool, (Object) null) || !HashedString.op_Inequality(PlayerController.Instance.ActiveTool.ViewMode, OverlayModes.None.ID) || !HashedString.op_Inequality(PlayerController.Instance.ActiveTool.ViewMode, hashedString))
      return;
    this.ChooseCollection((ToolMenu.ToolCollection) null);
    this.ChooseTool((ToolMenu.ToolInfo) null);
  }

  protected virtual void OnSpawn()
  {
    this.activateOnSpawn = true;
    base.OnSpawn();
    this.CreateSandBoxTools();
    this.CreateBasicTools();
    this.rows.Add(this.sandboxTools);
    this.rows.Add(this.basicTools);
    this.rows.ForEach((Action<List<ToolMenu.ToolCollection>>) (row => this.InstantiateCollectionsUI((IList<ToolMenu.ToolCollection>) row)));
    this.rows.ForEach((Action<List<ToolMenu.ToolCollection>>) (row => this.BuildRowToggles((IList<ToolMenu.ToolCollection>) row)));
    this.rows.ForEach((Action<List<ToolMenu.ToolCollection>>) (row => this.BuildToolToggles((IList<ToolMenu.ToolCollection>) row)));
    this.ChooseCollection((ToolMenu.ToolCollection) null);
    ((Component) this.priorityScreen).gameObject.SetActive(false);
    this.ToggleSandboxUI();
    // ISSUE: method pointer
    KInputManager.InputChange.AddListener(new UnityAction((object) this, __methodptr(OnInputChange)));
    Game.Instance.Subscribe(-1948169901, new Action<object>(this.ToggleSandboxUI));
    this.ResetToolDisplayPlane();
    this.refreshScaleHandle = Game.Instance.Subscribe(-442024484, new Action<object>(this.RefreshScale));
    this.RefreshScale();
  }

  private void RefreshScale(object data = null)
  {
    int num1 = 14;
    int num2 = 16;
    foreach (ToolMenu.ToolCollection sandboxTool in this.sandboxTools)
    {
      LocText componentInChildren = sandboxTool.toggle.GetComponentInChildren<LocText>();
      if (Object.op_Inequality((Object) componentInChildren, (Object) null))
        ((TMP_Text) componentInChildren).fontSize = ScreenResolutionMonitor.UsingGamepadUIMode() ? (float) num2 : (float) num1;
    }
    foreach (ToolMenu.ToolCollection basicTool in this.basicTools)
    {
      LocText componentInChildren = basicTool.toggle.GetComponentInChildren<LocText>();
      if (Object.op_Inequality((Object) componentInChildren, (Object) null))
        ((TMP_Text) componentInChildren).fontSize = ScreenResolutionMonitor.UsingGamepadUIMode() ? (float) num2 : (float) num1;
    }
  }

  public void OnInputChange()
  {
    this.rows.ForEach((Action<List<ToolMenu.ToolCollection>>) (row => this.BuildRowToggles((IList<ToolMenu.ToolCollection>) row)));
    this.rows.ForEach((Action<List<ToolMenu.ToolCollection>>) (row => this.BuildToolToggles((IList<ToolMenu.ToolCollection>) row)));
  }

  private void ResetToolDisplayPlane()
  {
    this.toolEffectDisplayPlane = this.CreateToolDisplayPlane("Overlay", World.Instance.transform);
    this.toolEffectDisplayPlaneTexture = this.CreatePlaneTexture(out this.toolEffectDisplayBytes, Grid.WidthInCells, Grid.HeightInCells);
    this.toolEffectDisplayPlane.GetComponent<Renderer>().sharedMaterial = this.toolEffectDisplayMaterial;
    this.toolEffectDisplayPlane.GetComponent<Renderer>().sharedMaterial.mainTexture = (Texture) this.toolEffectDisplayPlaneTexture;
    TransformExtensions.SetLocalPosition(this.toolEffectDisplayPlane.transform, new Vector3(Grid.WidthInMeters / 2f, Grid.HeightInMeters / 2f, -6f));
    this.RefreshToolDisplayPlaneColor();
  }

  private GameObject CreateToolDisplayPlane(string layer, Transform parent)
  {
    GameObject primitive = GameObject.CreatePrimitive((PrimitiveType) 4);
    ((Object) primitive).name = "toolEffectDisplayPlane";
    Util.SetLayerRecursively(primitive, LayerMask.NameToLayer(layer));
    Object.Destroy((Object) primitive.GetComponent<Collider>());
    if (Object.op_Inequality((Object) parent, (Object) null))
      primitive.transform.SetParent(parent);
    TransformExtensions.SetPosition(primitive.transform, Vector3.zero);
    primitive.transform.localScale = new Vector3(Grid.WidthInMeters / -10f, 1f, Grid.HeightInMeters / -10f);
    primitive.transform.eulerAngles = new Vector3(270f, 0.0f, 0.0f);
    ((Renderer) primitive.GetComponent<MeshRenderer>()).reflectionProbeUsage = (ReflectionProbeUsage) 0;
    return primitive;
  }

  private Texture2D CreatePlaneTexture(out byte[] textureBytes, int width, int height)
  {
    textureBytes = new byte[width * height * 4];
    Texture2D planeTexture = new Texture2D(width, height, TextureUtil.TextureFormatToGraphicsFormat((TextureFormat) 4), (TextureCreationFlags) 0);
    ((Object) planeTexture).name = "toolEffectDisplayPlane";
    ((Texture) planeTexture).wrapMode = (TextureWrapMode) 1;
    ((Texture) planeTexture).filterMode = (FilterMode) 0;
    return planeTexture;
  }

  private void Update() => this.RefreshToolDisplayPlaneColor();

  private void RefreshToolDisplayPlaneColor()
  {
    if (Object.op_Equality((Object) PlayerController.Instance.ActiveTool, (Object) null) || Object.op_Equality((Object) PlayerController.Instance.ActiveTool, (Object) SelectTool.Instance))
    {
      this.toolEffectDisplayPlane.SetActive(false);
    }
    else
    {
      PlayerController.Instance.ActiveTool.GetOverlayColorData(out this.colors);
      Array.Clear((Array) this.toolEffectDisplayBytes, 0, this.toolEffectDisplayBytes.Length);
      if (this.colors != null)
      {
        foreach (ToolMenu.CellColorData color in this.colors)
        {
          if (Grid.IsValidCell(color.cell))
          {
            int index = color.cell * 4;
            if (index >= 0)
            {
              this.toolEffectDisplayBytes[index] = (byte) ((double) Mathf.Min(color.color.r, 1f) * (double) byte.MaxValue);
              this.toolEffectDisplayBytes[index + 1] = (byte) ((double) Mathf.Min(color.color.g, 1f) * (double) byte.MaxValue);
              this.toolEffectDisplayBytes[index + 2] = (byte) ((double) Mathf.Min(color.color.b, 1f) * (double) byte.MaxValue);
              this.toolEffectDisplayBytes[index + 3] = (byte) ((double) Mathf.Min(color.color.a, 1f) * (double) byte.MaxValue);
            }
          }
        }
      }
      if (!this.toolEffectDisplayPlane.activeSelf)
        this.toolEffectDisplayPlane.SetActive(true);
      this.toolEffectDisplayPlaneTexture.LoadRawTextureData(this.toolEffectDisplayBytes);
      this.toolEffectDisplayPlaneTexture.Apply();
    }
  }

  public void ToggleSandboxUI(object data = null)
  {
    this.ClearSelection();
    PlayerController.Instance.ActivateTool((InterfaceTool) SelectTool.Instance);
    ((Component) ((Component) this.sandboxTools[0].toggle.transform.parent).transform.parent).gameObject.SetActive(Game.Instance.SandboxModeActive);
  }

  public static ToolMenu.ToolCollection CreateToolCollection(
    LocString collection_name,
    string icon_name,
    Action hotkey,
    string tool_name,
    LocString tooltip,
    bool largeIcon)
  {
    ToolMenu.ToolCollection toolCollection = new ToolMenu.ToolCollection((string) collection_name, icon_name, largeIcon: largeIcon);
    ToolMenu.ToolInfo toolInfo = new ToolMenu.ToolInfo((string) collection_name, icon_name, hotkey, tool_name, toolCollection, (string) tooltip);
    return toolCollection;
  }

  private void CreateSandBoxTools()
  {
    this.sandboxTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.SANDBOX.BRUSH.NAME, "brush", (Action) 229, "SandboxBrushTool", STRINGS.UI.SANDBOXTOOLS.SETTINGS.BRUSH.TOOLTIP, false));
    this.sandboxTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.SANDBOX.SPRINKLE.NAME, "sprinkle", (Action) 230, "SandboxSprinkleTool", STRINGS.UI.SANDBOXTOOLS.SETTINGS.SPRINKLE.TOOLTIP, false));
    this.sandboxTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.SANDBOX.FLOOD.NAME, "flood", (Action) 231, "SandboxFloodTool", STRINGS.UI.SANDBOXTOOLS.SETTINGS.FLOOD.TOOLTIP, false));
    this.sandboxTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.SANDBOX.SAMPLE.NAME, "sample", (Action) 233, "SandboxSampleTool", STRINGS.UI.SANDBOXTOOLS.SETTINGS.SAMPLE.TOOLTIP, false));
    this.sandboxTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.SANDBOX.HEATGUN.NAME, "temperature", (Action) 234, "SandboxHeatTool", STRINGS.UI.SANDBOXTOOLS.SETTINGS.HEATGUN.TOOLTIP, false));
    this.sandboxTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.SANDBOX.STRESSTOOL.NAME, "crew_state_happy", (Action) 243, "SandboxStressTool", STRINGS.UI.SANDBOXTOOLS.SETTINGS.STRESS.TOOLTIP, false));
    this.sandboxTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.SANDBOX.SPAWNER.NAME, "spawn", (Action) 237, "SandboxSpawnerTool", STRINGS.UI.SANDBOXTOOLS.SETTINGS.SPAWNER.TOOLTIP, false));
    this.sandboxTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.SANDBOX.CLEAR_FLOOR.NAME, "clear_floor", (Action) 235, "SandboxClearFloorTool", STRINGS.UI.SANDBOXTOOLS.SETTINGS.CLEAR_FLOOR.TOOLTIP, false));
    this.sandboxTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.SANDBOX.DESTROY.NAME, "destroy", (Action) 236, "SandboxDestroyerTool", STRINGS.UI.SANDBOXTOOLS.SETTINGS.DESTROY.TOOLTIP, false));
    this.sandboxTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.SANDBOX.FOW.NAME, "reveal", (Action) 239, "SandboxFOWTool", STRINGS.UI.SANDBOXTOOLS.SETTINGS.FOW.TOOLTIP, false));
    this.sandboxTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.SANDBOX.CRITTER.NAME, "critter", (Action) 241, "SandboxCritterTool", STRINGS.UI.SANDBOXTOOLS.SETTINGS.CRITTER.TOOLTIP, false));
  }

  private void CreateBasicTools()
  {
    this.basicTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.DIG.NAME, "icon_action_dig", (Action) 144, "DigTool", STRINGS.UI.TOOLTIPS.DIGBUTTON, true));
    this.basicTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.CANCEL.NAME, "icon_action_cancel", (Action) 143, "CancelTool", STRINGS.UI.TOOLTIPS.CANCELBUTTON, true));
    this.basicTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.DECONSTRUCT.NAME, "icon_action_deconstruct", (Action) 142, "DeconstructTool", STRINGS.UI.TOOLTIPS.DECONSTRUCTBUTTON, true));
    this.basicTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.PRIORITIZE.NAME, "icon_action_prioritize", (Action) 154, "PrioritizeTool", STRINGS.UI.TOOLTIPS.PRIORITIZEBUTTON, true));
    this.basicTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.DISINFECT.NAME, "icon_action_disinfect", (Action) 152, "DisinfectTool", STRINGS.UI.TOOLTIPS.DISINFECTBUTTON, false));
    this.basicTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.MARKFORSTORAGE.NAME, "icon_action_store", (Action) 151, "ClearTool", STRINGS.UI.TOOLTIPS.CLEARBUTTON, false));
    this.basicTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.ATTACK.NAME, "icon_action_attack", (Action) 145, "AttackTool", STRINGS.UI.TOOLTIPS.ATTACKBUTTON, false));
    this.basicTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.MOP.NAME, "icon_action_mop", (Action) 150, "MopTool", STRINGS.UI.TOOLTIPS.MOPBUTTON, false));
    this.basicTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.CAPTURE.NAME, "icon_action_capture", (Action) 146, "CaptureTool", STRINGS.UI.TOOLTIPS.CAPTUREBUTTON, false));
    this.basicTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.HARVEST.NAME, "icon_action_harvest", (Action) 147, "HarvestTool", STRINGS.UI.TOOLTIPS.HARVESTBUTTON, false));
    this.basicTools.Add(ToolMenu.CreateToolCollection(STRINGS.UI.TOOLS.EMPTY_PIPE.NAME, "icon_action_empty_pipes", (Action) 148, "EmptyPipeTool", STRINGS.UI.TOOLS.EMPTY_PIPE.TOOLTIP, false));
  }

  private void InstantiateCollectionsUI(IList<ToolMenu.ToolCollection> collections)
  {
    GameObject gameObject1 = Util.KInstantiateUI(this.prefabToolRow, ((Component) this).gameObject, true);
    GameObject gameObject2 = Util.KInstantiateUI(this.largeToolSet, gameObject1, true);
    GameObject gameObject3 = Util.KInstantiateUI(this.smallToolSet, gameObject1, true);
    GameObject gameObject4 = Util.KInstantiateUI(this.smallToolBottomRow, gameObject3, true);
    GameObject gameObject5 = Util.KInstantiateUI(this.smallToolTopRow, gameObject3, true);
    GameObject gameObject6 = Util.KInstantiateUI(this.sandboxToolSet, gameObject1, true);
    bool flag = true;
    for (int index1 = 0; index1 < collections.Count; ++index1)
    {
      GameObject gameObject7;
      if (collections == this.sandboxTools)
        gameObject7 = gameObject6;
      else if (collections[index1].largeIcon)
      {
        gameObject7 = gameObject2;
      }
      else
      {
        gameObject7 = flag ? gameObject5 : gameObject4;
        flag = !flag;
      }
      ToolMenu.ToolCollection tc = collections[index1];
      tc.toggle = Util.KInstantiateUI(collections[index1].tools.Count > 1 ? this.collectionIconPrefab : (collections == this.sandboxTools ? this.sandboxToolIconPrefab : (collections[index1].largeIcon ? this.toolIconLargePrefab : this.toolIconPrefab)), gameObject7, true);
      KToggle component = tc.toggle.GetComponent<KToggle>();
      ((WidgetSoundPlayer) component.soundPlayer).Enabled = false;
      component.onClick += (System.Action) (() =>
      {
        if (this.currentlySelectedCollection == tc && tc.tools.Count >= 1)
          KMonoBehaviour.PlaySound(GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound()));
        this.ChooseCollection(tc);
      });
      if (tc.tools != null)
      {
        GameObject gameObject8;
        if (tc.tools.Count < this.smallCollectionMax)
        {
          gameObject8 = Util.KInstantiateUI(this.Prefab_collectionContainer, gameObject7, true);
          gameObject8.transform.SetSiblingIndex(gameObject8.transform.GetSiblingIndex() - 1);
          gameObject8.transform.localScale = Vector3.one;
          Util.rectTransform(gameObject8).sizeDelta = new Vector2((float) (tc.tools.Count * 75), 50f);
          tc.MaskContainer = ((Component) gameObject8.GetComponentInChildren<Mask>()).gameObject;
          gameObject8.SetActive(false);
        }
        else
        {
          gameObject8 = Util.KInstantiateUI(this.Prefab_collectionContainerWindow, gameObject7, true);
          gameObject8.transform.localScale = Vector3.one;
          ((TMP_Text) gameObject8.GetComponentInChildren<LocText>()).SetText(tc.text.ToUpper());
          tc.MaskContainer = ((Component) gameObject8.GetComponentInChildren<GridLayoutGroup>()).gameObject;
          gameObject8.SetActive(false);
        }
        tc.UIMenuDisplay = gameObject8;
        for (int index2 = 0; index2 < tc.tools.Count; ++index2)
        {
          ToolMenu.ToolInfo ti = tc.tools[index2];
          GameObject gameObject9 = Util.KInstantiateUI(collections == this.sandboxTools ? this.sandboxToolIconPrefab : (collections[index1].largeIcon ? this.toolIconLargePrefab : this.toolIconPrefab), tc.MaskContainer, true);
          ((Object) gameObject9).name = ti.text;
          ti.toggle = gameObject9.GetComponent<KToggle>();
          if (ti.collection.tools.Count > 1)
          {
            RectTransform rectTransform = Util.rectTransform((Component) ((Component) ti.toggle).gameObject.GetComponentInChildren<SetTextStyleSetting>());
            if (((Object) gameObject9).name.Length > 12)
            {
              ((Component) rectTransform).GetComponent<SetTextStyleSetting>().SetStyle(this.CategoryLabelTextStyle_LeftAlign);
              rectTransform.anchoredPosition = new Vector2(16f, rectTransform.anchoredPosition.y);
            }
          }
          ti.toggle.onClick += (System.Action) (() => this.ChooseTool(ti));
          tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapse(closure_0 ?? (closure_0 = (Action<object>) (s =>
          {
            this.SetToggleState(tc.toggle.GetComponent<KToggle>(), false);
            tc.UIMenuDisplay.SetActive(false);
          })));
        }
      }
    }
    if (gameObject2.transform.childCount == 0)
      Object.Destroy((Object) gameObject2);
    if (gameObject4.transform.childCount == 0 && gameObject5.transform.childCount == 0)
      Object.Destroy((Object) gameObject3);
    if (gameObject6.transform.childCount != 0)
      return;
    Object.Destroy((Object) gameObject6);
  }

  private void ChooseTool(ToolMenu.ToolInfo tool)
  {
    if (this.currentlySelectedTool == tool)
      return;
    if (this.currentlySelectedTool != tool)
    {
      this.currentlySelectedTool = tool;
      if (this.currentlySelectedTool != null && this.currentlySelectedTool.onSelectCallback != null)
        this.currentlySelectedTool.onSelectCallback((object) this.currentlySelectedTool);
    }
    if (this.currentlySelectedTool != null)
    {
      this.currentlySelectedCollection = this.currentlySelectedTool.collection;
      foreach (InterfaceTool tool1 in PlayerController.Instance.tools)
      {
        if (this.currentlySelectedTool.toolName == ((Object) tool1).name)
        {
          UISounds.PlaySound(UISounds.Sound.ClickObject);
          this.activeTool = tool1;
          PlayerController.Instance.ActivateTool(tool1);
          break;
        }
      }
    }
    else
      PlayerController.Instance.ActivateTool((InterfaceTool) SelectTool.Instance);
    this.rows.ForEach((Action<List<ToolMenu.ToolCollection>>) (row => this.RefreshRowDisplay((IList<ToolMenu.ToolCollection>) row)));
  }

  private void RefreshRowDisplay(IList<ToolMenu.ToolCollection> row)
  {
    for (int index1 = 0; index1 < row.Count; ++index1)
    {
      ToolMenu.ToolCollection tc = row[index1];
      if (this.currentlySelectedTool != null && this.currentlySelectedTool.collection == tc)
      {
        if (!tc.UIMenuDisplay.activeSelf || tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapsing)
        {
          if (tc.tools.Count > 1)
          {
            tc.UIMenuDisplay.SetActive(true);
            if (tc.tools.Count < this.smallCollectionMax)
            {
              float num = Mathf.Clamp((float) (1.0 - (double) tc.tools.Count * 0.15000000596046448), 0.5f, 1f);
              tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().speedScale = num;
            }
            tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Expand((Action<object>) (s => this.SetToggleState(tc.toggle.GetComponent<KToggle>(), true)));
          }
          else
            this.currentlySelectedTool = tc.tools[0];
        }
      }
      else if (tc.UIMenuDisplay.activeSelf && !tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapsing && tc.tools.Count > 0)
        tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapse((Action<object>) (s =>
        {
          this.SetToggleState(tc.toggle.GetComponent<KToggle>(), false);
          tc.UIMenuDisplay.SetActive(false);
        }));
      for (int index2 = 0; index2 < tc.tools.Count; ++index2)
      {
        if (tc.tools[index2] == this.currentlySelectedTool)
          this.SetToggleState(tc.tools[index2].toggle, true);
        else
          this.SetToggleState(tc.tools[index2].toggle, false);
      }
    }
  }

  public void TurnLargeCollectionOff()
  {
    if (this.currentlySelectedCollection == null || this.currentlySelectedCollection.tools.Count <= this.smallCollectionMax)
      return;
    this.ChooseCollection((ToolMenu.ToolCollection) null);
  }

  private void ChooseCollection(ToolMenu.ToolCollection collection, bool autoSelectTool = true)
  {
    if (collection == this.currentlySelectedCollection)
    {
      if (collection != null && collection.tools.Count > 1)
      {
        this.currentlySelectedCollection = (ToolMenu.ToolCollection) null;
        if (this.currentlySelectedTool != null)
          this.ChooseTool((ToolMenu.ToolInfo) null);
      }
      else if (this.currentlySelectedTool != null && this.currentlySelectedCollection.tools.Contains(this.currentlySelectedTool) && this.currentlySelectedCollection.tools.Count == 1)
      {
        this.currentlySelectedCollection = (ToolMenu.ToolCollection) null;
        this.ChooseTool((ToolMenu.ToolInfo) null);
      }
    }
    else
      this.currentlySelectedCollection = collection;
    this.rows.ForEach((Action<List<ToolMenu.ToolCollection>>) (row => this.OpenOrCloseCollectionsInRow((IList<ToolMenu.ToolCollection>) row)));
  }

  private void OpenOrCloseCollectionsInRow(IList<ToolMenu.ToolCollection> row, bool autoSelectTool = true)
  {
    for (int index = 0; index < row.Count; ++index)
    {
      ToolMenu.ToolCollection tc = row[index];
      if (this.currentlySelectedCollection == tc)
      {
        if (((this.currentlySelectedCollection.tools == null ? 0 : (this.currentlySelectedCollection.tools.Count == 1 ? 1 : 0)) | (autoSelectTool ? 1 : 0)) != 0)
          this.ChooseTool(this.currentlySelectedCollection.tools[0]);
      }
      else if (tc.UIMenuDisplay.activeSelf && !tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapsing)
        tc.UIMenuDisplay.GetComponent<ExpandRevealUIContent>().Collapse((Action<object>) (s =>
        {
          this.SetToggleState(tc.toggle.GetComponent<KToggle>(), false);
          tc.UIMenuDisplay.SetActive(false);
        }));
      this.SetToggleState(tc.toggle.GetComponent<KToggle>(), this.currentlySelectedCollection == tc);
    }
  }

  private void SetToggleState(KToggle toggle, bool state)
  {
    if (state)
    {
      ((Selectable) toggle).Select();
      toggle.isOn = true;
    }
    else
    {
      toggle.Deselect();
      toggle.isOn = false;
    }
  }

  public void ClearSelection()
  {
    if (this.currentlySelectedCollection != null)
      this.ChooseCollection((ToolMenu.ToolCollection) null);
    if (this.currentlySelectedTool == null)
      return;
    this.ChooseTool((ToolMenu.ToolInfo) null);
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (!((KInputEvent) e).Consumed)
    {
      if (e.IsAction((Action) 238))
      {
        if (Application.isEditor)
        {
          DebugUtil.LogArgs(new object[1]
          {
            (object) "Force-enabling sandbox mode because we're in editor."
          });
          SaveGame.Instance.sandboxEnabled = true;
        }
        if (SaveGame.Instance.sandboxEnabled)
          Game.Instance.SandboxModeActive = !Game.Instance.SandboxModeActive;
      }
      foreach (List<ToolMenu.ToolCollection> row in this.rows)
      {
        if (row != this.sandboxTools || Game.Instance.SandboxModeActive)
        {
          for (int index1 = 0; index1 < row.Count; ++index1)
          {
            Action toolHotkey = row[index1].hotkey;
            if (toolHotkey != 275 && e.IsAction(toolHotkey) && (this.currentlySelectedCollection == null || this.currentlySelectedCollection != null && this.currentlySelectedCollection.tools.Find((Predicate<ToolMenu.ToolInfo>) (t => GameInputMapping.CompareActionKeyCodes(t.hotkey, toolHotkey))) == null))
            {
              if (this.currentlySelectedCollection != row[index1])
              {
                this.ChooseCollection(row[index1], false);
                this.ChooseTool(row[index1].tools[0]);
                break;
              }
              if (this.currentlySelectedCollection.tools.Count > 1)
              {
                ((KInputEvent) e).Consumed = true;
                this.ChooseCollection((ToolMenu.ToolCollection) null);
                this.ChooseTool((ToolMenu.ToolInfo) null);
                string sound = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound());
                if (sound != null)
                {
                  KMonoBehaviour.PlaySound(sound);
                  break;
                }
                break;
              }
              break;
            }
            for (int index2 = 0; index2 < row[index1].tools.Count; ++index2)
            {
              if (this.currentlySelectedCollection == null && row[index1].tools.Count == 1 || this.currentlySelectedCollection == row[index1] || this.currentlySelectedCollection != null && this.currentlySelectedCollection.tools.Count == 1 && row[index1].tools.Count == 1)
              {
                Action hotkey = row[index1].tools[index2].hotkey;
                if (e.IsAction(hotkey) && e.TryConsume(hotkey))
                {
                  if (row[index1].tools.Count == 1 && this.currentlySelectedCollection != row[index1])
                    this.ChooseCollection(row[index1], false);
                  else if (this.currentlySelectedTool != row[index1].tools[index2])
                    this.ChooseTool(row[index1].tools[index2]);
                }
                else if (GameInputMapping.CompareActionKeyCodes(e.GetAction(), hotkey))
                  ((KInputEvent) e).Consumed = true;
              }
            }
          }
        }
      }
      if ((this.currentlySelectedTool != null || this.currentlySelectedCollection != null) && !((KInputEvent) e).Consumed)
      {
        if (e.TryConsume((Action) 1))
        {
          string sound = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound());
          if (sound != null)
            KMonoBehaviour.PlaySound(sound);
          if (this.currentlySelectedCollection != null)
            this.ChooseCollection((ToolMenu.ToolCollection) null);
          if (this.currentlySelectedTool != null)
            this.ChooseTool((ToolMenu.ToolInfo) null);
          SelectTool.Instance.Activate();
        }
      }
      else if (!PlayerController.Instance.IsUsingDefaultTool() && !((KInputEvent) e).Consumed && e.TryConsume((Action) 1))
        SelectTool.Instance.Activate();
    }
    base.OnKeyDown(e);
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    if (!((KInputEvent) e).Consumed)
    {
      if ((this.currentlySelectedTool != null || this.currentlySelectedCollection != null) && !((KInputEvent) e).Consumed)
      {
        if (PlayerController.Instance.ConsumeIfNotDragging(e, (Action) 5))
        {
          string sound = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound());
          if (sound != null)
            KMonoBehaviour.PlaySound(sound);
          if (this.currentlySelectedCollection != null)
            this.ChooseCollection((ToolMenu.ToolCollection) null);
          if (this.currentlySelectedTool != null)
            this.ChooseTool((ToolMenu.ToolInfo) null);
          SelectTool.Instance.Activate();
        }
      }
      else if (!PlayerController.Instance.IsUsingDefaultTool() && !((KInputEvent) e).Consumed && PlayerController.Instance.ConsumeIfNotDragging(e, (Action) 5))
      {
        SelectTool.Instance.Activate();
        string sound = GlobalAssets.GetSound(PlayerController.Instance.ActiveTool.GetDeactivateSound());
        if (sound != null)
          KMonoBehaviour.PlaySound(sound);
      }
    }
    base.OnKeyUp(e);
  }

  protected void BuildRowToggles(IList<ToolMenu.ToolCollection> row)
  {
    for (int index = 0; index < row.Count; ++index)
    {
      ToolMenu.ToolCollection toolCollection = row[index];
      if (!Object.op_Equality((Object) toolCollection.toggle, (Object) null))
      {
        GameObject toggle = toolCollection.toggle;
        Sprite sprite = Assets.GetSprite(HashedString.op_Implicit(toolCollection.icon));
        if (Object.op_Inequality((Object) sprite, (Object) null))
          ((Component) toggle.transform.Find("FG")).GetComponent<Image>().sprite = sprite;
        Transform transform = toggle.transform.Find("Text");
        if (Object.op_Inequality((Object) transform, (Object) null))
        {
          LocText component = ((Component) transform).GetComponent<LocText>();
          if (Object.op_Inequality((Object) component, (Object) null))
            ((TMP_Text) component).text = toolCollection.text;
        }
        ToolTip component1 = toggle.GetComponent<ToolTip>();
        if (Object.op_Implicit((Object) component1))
        {
          if (row[index].tools.Count == 1)
          {
            string str = GameUtil.ReplaceHotkeyString(row[index].tools[0].tooltip, row[index].tools[0].hotkey);
            component1.ClearMultiStringTooltip();
            component1.AddMultiStringTooltip(str, this.ToggleToolTipTextStyleSetting);
          }
          else
          {
            string template = row[index].tooltip;
            if (row[index].hotkey != 275)
              template = GameUtil.ReplaceHotkeyString(template, row[index].hotkey);
            component1.ClearMultiStringTooltip();
            component1.AddMultiStringTooltip(template, this.ToggleToolTipTextStyleSetting);
          }
        }
      }
    }
  }

  protected void BuildToolToggles(IList<ToolMenu.ToolCollection> row)
  {
    for (int index1 = 0; index1 < row.Count; ++index1)
    {
      ToolMenu.ToolCollection toolCollection = row[index1];
      if (!Object.op_Equality((Object) toolCollection.toggle, (Object) null))
      {
        for (int index2 = 0; index2 < toolCollection.tools.Count; ++index2)
        {
          GameObject gameObject = ((Component) toolCollection.tools[index2].toggle).gameObject;
          Sprite sprite = Assets.GetSprite(HashedString.op_Implicit(toolCollection.icon));
          if (Object.op_Inequality((Object) sprite, (Object) null))
            ((Component) gameObject.transform.Find("FG")).GetComponent<Image>().sprite = sprite;
          Transform transform = gameObject.transform.Find("Text");
          if (Object.op_Inequality((Object) transform, (Object) null))
          {
            LocText component = ((Component) transform).GetComponent<LocText>();
            if (Object.op_Inequality((Object) component, (Object) null))
              ((TMP_Text) component).text = toolCollection.tools[index2].text;
          }
          ToolTip component1 = gameObject.GetComponent<ToolTip>();
          if (Object.op_Implicit((Object) component1))
          {
            string str = toolCollection.tools.Count > 1 ? GameUtil.ReplaceHotkeyString(toolCollection.tools[index2].tooltip, toolCollection.hotkey, toolCollection.tools[index2].hotkey) : GameUtil.ReplaceHotkeyString(toolCollection.tools[index2].tooltip, toolCollection.tools[index2].hotkey);
            component1.ClearMultiStringTooltip();
            component1.AddMultiStringTooltip(str, this.ToggleToolTipTextStyleSetting);
          }
        }
      }
    }
  }

  public bool HasUniqueKeyBindings()
  {
    bool flag = true;
    this.boundRootActions.Clear();
    foreach (List<ToolMenu.ToolCollection> row in this.rows)
    {
      foreach (ToolMenu.ToolCollection toolCollection in row)
      {
        if (this.boundRootActions.Contains(toolCollection.hotkey))
        {
          flag = false;
          break;
        }
        this.boundRootActions.Add(toolCollection.hotkey);
        this.boundSubgroupActions.Clear();
        foreach (ToolMenu.ToolInfo tool in toolCollection.tools)
        {
          if (this.boundSubgroupActions.Contains(tool.hotkey))
          {
            flag = false;
            break;
          }
          this.boundSubgroupActions.Add(tool.hotkey);
        }
      }
    }
    return flag;
  }

  private void OnPriorityClicked(PrioritySetting priority) => this.priorityScreen.SetScreenPriority(priority);

  public class ToolInfo
  {
    public string text;
    public string icon;
    public Action hotkey;
    public string toolName;
    public ToolMenu.ToolCollection collection;
    public string tooltip;
    public KToggle toggle;
    public Action<object> onSelectCallback;
    public object toolData;

    public ToolInfo(
      string text,
      string icon_name,
      Action hotkey,
      string ToolName,
      ToolMenu.ToolCollection toolCollection,
      string tooltip = "",
      Action<object> onSelectCallback = null,
      object toolData = null)
    {
      this.text = text;
      this.icon = icon_name;
      this.hotkey = hotkey;
      this.toolName = ToolName;
      this.collection = toolCollection;
      toolCollection.tools.Add(this);
      this.tooltip = tooltip;
      this.onSelectCallback = onSelectCallback;
      this.toolData = toolData;
    }
  }

  public class ToolCollection
  {
    public string text;
    public string icon;
    public string tooltip;
    public bool useInfoMenu;
    public bool largeIcon;
    public GameObject toggle;
    public List<ToolMenu.ToolInfo> tools = new List<ToolMenu.ToolInfo>();
    public GameObject UIMenuDisplay;
    public GameObject MaskContainer;
    public Action hotkey;

    public ToolCollection(
      string text,
      string icon_name,
      string tooltip = "",
      bool useInfoMenu = false,
      Action hotkey = 275,
      bool largeIcon = false)
    {
      this.text = text;
      this.icon = icon_name;
      this.tooltip = tooltip;
      this.useInfoMenu = useInfoMenu;
      this.hotkey = hotkey;
      this.largeIcon = largeIcon;
    }
  }

  public struct CellColorData
  {
    public int cell;
    public Color color;

    public CellColorData(int cell, Color color)
    {
      this.cell = cell;
      this.color = color;
    }
  }
}
