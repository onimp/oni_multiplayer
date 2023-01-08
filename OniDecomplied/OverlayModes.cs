// Decompiled with JetBrains decompiler
// Type: OverlayModes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class OverlayModes
{
  public class GasConduits : OverlayModes.ConduitMode
  {
    public static readonly HashedString ID = HashedString.op_Implicit("GasConduit");

    public override HashedString ViewMode() => OverlayModes.GasConduits.ID;

    public override string GetSoundName() => "GasVent";

    public GasConduits()
      : base((ICollection<Tag>) OverlayScreen.GasVentIDs)
    {
    }
  }

  public class LiquidConduits : OverlayModes.ConduitMode
  {
    public static readonly HashedString ID = HashedString.op_Implicit("LiquidConduit");

    public override HashedString ViewMode() => OverlayModes.LiquidConduits.ID;

    public override string GetSoundName() => "LiquidVent";

    public LiquidConduits()
      : base((ICollection<Tag>) OverlayScreen.LiquidVentIDs)
    {
    }
  }

  public abstract class ConduitMode : OverlayModes.Mode
  {
    private UniformGrid<SaveLoadRoot> partition;
    private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();
    private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();
    private List<int> visited = new List<int>();
    private ICollection<Tag> targetIDs;
    private int objectTargetLayer;
    private int conduitTargetLayer;
    private int cameraLayerMask;
    private int selectionMask;

    public ConduitMode(ICollection<Tag> ids)
    {
      this.objectTargetLayer = LayerMask.NameToLayer("MaskedOverlayBG");
      this.conduitTargetLayer = LayerMask.NameToLayer("MaskedOverlay");
      this.cameraLayerMask = LayerMask.GetMask(new string[2]
      {
        "MaskedOverlay",
        "MaskedOverlayBG"
      });
      this.selectionMask = this.cameraLayerMask;
      this.targetIDs = ids;
    }

    public override void Enable()
    {
      this.RegisterSaveLoadListeners();
      this.partition = OverlayModes.Mode.PopulatePartition<SaveLoadRoot>(this.targetIDs);
      Camera.main.cullingMask |= this.cameraLayerMask;
      SelectTool.Instance.SetLayerMask(this.selectionMask);
      GridCompositor.Instance.ToggleMinor(false);
      base.Enable();
    }

    protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
    {
      if (!this.targetIDs.Contains(((Component) item).GetComponent<KPrefabID>().GetSaveLoadTag()))
        return;
      this.partition.Add(item);
    }

    protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
    {
      if (Object.op_Equality((Object) item, (Object) null) || Object.op_Equality((Object) ((Component) item).gameObject, (Object) null))
        return;
      if (this.layerTargets.Contains(item))
        this.layerTargets.Remove(item);
      this.partition.Remove(item);
    }

    public override void Disable()
    {
      foreach (SaveLoadRoot layerTarget in this.layerTargets)
      {
        float defaultDepth = OverlayModes.Mode.GetDefaultDepth((KMonoBehaviour) layerTarget);
        Vector3 position = TransformExtensions.GetPosition(layerTarget.transform);
        position.z = defaultDepth;
        TransformExtensions.SetPosition(layerTarget.transform, position);
        foreach (KBatchedAnimController componentsInChild in ((Component) layerTarget).GetComponentsInChildren<KBatchedAnimController>())
          this.TriggerResorting(componentsInChild);
      }
      OverlayModes.Mode.ResetDisplayValues<SaveLoadRoot>((ICollection<SaveLoadRoot>) this.layerTargets);
      Camera.main.cullingMask &= ~this.cameraLayerMask;
      SelectTool.Instance.ClearLayerMask();
      this.UnregisterSaveLoadListeners();
      this.partition.Clear();
      this.layerTargets.Clear();
      GridCompositor.Instance.ToggleMinor(false);
      base.Disable();
    }

    public override void Update()
    {
      Vector2I min;
      Vector2I max;
      Grid.GetVisibleExtents(out min, out max);
      OverlayModes.Mode.RemoveOffscreenTargets<SaveLoadRoot>((ICollection<SaveLoadRoot>) this.layerTargets, min, max, (Action<SaveLoadRoot>) (root =>
      {
        if (Object.op_Equality((Object) root, (Object) null))
          return;
        float defaultDepth = OverlayModes.Mode.GetDefaultDepth((KMonoBehaviour) root);
        Vector3 position = TransformExtensions.GetPosition(root.transform);
        position.z = defaultDepth;
        TransformExtensions.SetPosition(root.transform, position);
        foreach (KBatchedAnimController componentsInChild in ((Component) root).GetComponentsInChildren<KBatchedAnimController>())
          this.TriggerResorting(componentsInChild);
      }));
      foreach (SaveLoadRoot instance in this.partition.GetAllIntersecting(new Vector2((float) min.x, (float) min.y), new Vector2((float) max.x, (float) max.y)))
      {
        if (Object.op_Inequality((Object) ((Component) instance).GetComponent<Conduit>(), (Object) null))
          this.AddTargetIfVisible<SaveLoadRoot>(instance, min, max, (ICollection<SaveLoadRoot>) this.layerTargets, this.conduitTargetLayer);
        else
          this.AddTargetIfVisible<SaveLoadRoot>(instance, min, max, (ICollection<SaveLoadRoot>) this.layerTargets, this.objectTargetLayer, (Action<SaveLoadRoot>) (root =>
          {
            Vector3 position = TransformExtensions.GetPosition(root.transform);
            float num = position.z;
            KPrefabID component = ((Component) root).GetComponent<KPrefabID>();
            if (Object.op_Inequality((Object) component, (Object) null))
            {
              if (component.HasTag(GameTags.OverlayInFrontOfConduits))
                num = Grid.GetLayerZ(HashedString.op_Equality(this.ViewMode(), OverlayModes.LiquidConduits.ID) ? Grid.SceneLayer.LiquidConduits : Grid.SceneLayer.GasConduits) - 0.2f;
              else if (component.HasTag(GameTags.OverlayBehindConduits))
                num = Grid.GetLayerZ(HashedString.op_Equality(this.ViewMode(), OverlayModes.LiquidConduits.ID) ? Grid.SceneLayer.LiquidConduits : Grid.SceneLayer.GasConduits) + 0.2f;
            }
            position.z = num;
            TransformExtensions.SetPosition(root.transform, position);
            foreach (KBatchedAnimController componentsInChild in ((Component) root).GetComponentsInChildren<KBatchedAnimController>())
              this.TriggerResorting(componentsInChild);
          }));
      }
      GameObject gameObject = (GameObject) null;
      if (Object.op_Inequality((Object) SelectTool.Instance, (Object) null) && Object.op_Inequality((Object) SelectTool.Instance.hover, (Object) null))
        gameObject = ((Component) SelectTool.Instance.hover).gameObject;
      this.connectedNetworks.Clear();
      float num1 = 1f;
      if (Object.op_Inequality((Object) gameObject, (Object) null))
      {
        IBridgedNetworkItem component = gameObject.GetComponent<IBridgedNetworkItem>();
        if (component != null)
        {
          int networkCell = component.GetNetworkCell();
          UtilityNetworkManager<FlowUtilityNetwork, Vent> mgr = HashedString.op_Equality(this.ViewMode(), OverlayModes.LiquidConduits.ID) ? Game.Instance.liquidConduitSystem : Game.Instance.gasConduitSystem;
          this.visited.Clear();
          this.FindConnectedNetworks(networkCell, (IUtilityNetworkMgr) mgr, (ICollection<UtilityNetwork>) this.connectedNetworks, this.visited);
          this.visited.Clear();
          num1 = OverlayModes.ModeUtil.GetHighlightScale();
        }
      }
      Game.ConduitVisInfo conduitVisInfo = HashedString.op_Equality(this.ViewMode(), OverlayModes.LiquidConduits.ID) ? Game.Instance.liquidConduitVisInfo : Game.Instance.gasConduitVisInfo;
      foreach (SaveLoadRoot layerTarget in this.layerTargets)
      {
        if (!Object.op_Equality((Object) layerTarget, (Object) null) && ((Component) layerTarget).GetComponent<IBridgedNetworkItem>() != null)
        {
          BuildingDef def = ((Component) layerTarget).GetComponent<Building>().Def;
          Color32 color32 = (double) def.ThermalConductivity != 1.0 ? ((double) def.ThermalConductivity >= 1.0 ? GlobalAssets.Instance.colorSet.GetColorByName(conduitVisInfo.overlayRadiantTintName) : GlobalAssets.Instance.colorSet.GetColorByName(conduitVisInfo.overlayInsulatedTintName)) : GlobalAssets.Instance.colorSet.GetColorByName(conduitVisInfo.overlayTintName);
          if (this.connectedNetworks.Count > 0)
          {
            IBridgedNetworkItem component = ((Component) layerTarget).GetComponent<IBridgedNetworkItem>();
            if (component != null && component.IsConnectedToNetworks((ICollection<UtilityNetwork>) this.connectedNetworks))
            {
              color32.r = (byte) ((double) color32.r * (double) num1);
              color32.g = (byte) ((double) color32.g * (double) num1);
              color32.b = (byte) ((double) color32.b * (double) num1);
            }
          }
          ((Component) layerTarget).GetComponent<KBatchedAnimController>().TintColour = color32;
        }
      }
    }

    private void TriggerResorting(KBatchedAnimController kbac)
    {
      if (!kbac.enabled)
        return;
      kbac.enabled = false;
      kbac.enabled = true;
    }

    private void FindConnectedNetworks(
      int cell,
      IUtilityNetworkMgr mgr,
      ICollection<UtilityNetwork> networks,
      List<int> visited)
    {
      if (visited.Contains(cell))
        return;
      visited.Add(cell);
      UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
      if (networkForCell == null)
        return;
      networks.Add(networkForCell);
      int connections = (int) mgr.GetConnections(cell, false);
      if ((connections & 2) != 0)
        this.FindConnectedNetworks(Grid.CellRight(cell), mgr, networks, visited);
      if ((connections & 1) != 0)
        this.FindConnectedNetworks(Grid.CellLeft(cell), mgr, networks, visited);
      if ((connections & 4) != 0)
        this.FindConnectedNetworks(Grid.CellAbove(cell), mgr, networks, visited);
      if ((connections & 8) != 0)
        this.FindConnectedNetworks(Grid.CellBelow(cell), mgr, networks, visited);
      object endpoint = mgr.GetEndpoint(cell);
      if (endpoint == null || !(endpoint is FlowUtilityNetwork.NetworkItem networkItem))
        return;
      networkItem.GameObject.GetComponent<IBridgedNetworkItem>()?.AddNetworks(networks);
    }
  }

  public class Crop : OverlayModes.BasePlantMode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (Crop));
    private Canvas uiRoot;
    private List<OverlayModes.Crop.UpdateCropInfo> updateCropInfo = new List<OverlayModes.Crop.UpdateCropInfo>();
    private int freeHarvestableNotificationIdx;
    private List<GameObject> harvestableNotificationList = new List<GameObject>();
    private GameObject harvestableNotificationPrefab;
    private OverlayModes.ColorHighlightCondition[] highlightConditions = new OverlayModes.ColorHighlightCondition[3]
    {
      new OverlayModes.ColorHighlightCondition((Func<KMonoBehaviour, Color>) (h => Color32.op_Implicit(GlobalAssets.Instance.colorSet.cropHalted)), (Func<KMonoBehaviour, bool>) (h =>
      {
        WiltCondition component = ((Component) h).GetComponent<WiltCondition>();
        return Object.op_Inequality((Object) component, (Object) null) && component.IsWilting();
      })),
      new OverlayModes.ColorHighlightCondition((Func<KMonoBehaviour, Color>) (h => Color32.op_Implicit(GlobalAssets.Instance.colorSet.cropGrowing)), (Func<KMonoBehaviour, bool>) (h => !(h as HarvestDesignatable).CanBeHarvested())),
      new OverlayModes.ColorHighlightCondition((Func<KMonoBehaviour, Color>) (h => Color32.op_Implicit(GlobalAssets.Instance.colorSet.cropGrown)), (Func<KMonoBehaviour, bool>) (h => (h as HarvestDesignatable).CanBeHarvested()))
    };

    public override HashedString ViewMode() => OverlayModes.Crop.ID;

    public override string GetSoundName() => "Harvest";

    public Crop(Canvas ui_root, GameObject harvestable_notification_prefab)
      : base((ICollection<Tag>) OverlayScreen.HarvestableIDs)
    {
      this.uiRoot = ui_root;
      this.harvestableNotificationPrefab = harvestable_notification_prefab;
    }

    public override List<LegendEntry> GetCustomLegendData() => new List<LegendEntry>()
    {
      new LegendEntry((string) STRINGS.UI.OVERLAYS.CROP.FULLY_GROWN, (string) STRINGS.UI.OVERLAYS.CROP.TOOLTIPS.FULLY_GROWN, Color32.op_Implicit(GlobalAssets.Instance.colorSet.cropGrown)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.CROP.GROWING, (string) STRINGS.UI.OVERLAYS.CROP.TOOLTIPS.GROWING, Color32.op_Implicit(GlobalAssets.Instance.colorSet.cropGrowing)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.CROP.GROWTH_HALTED, (string) STRINGS.UI.OVERLAYS.CROP.TOOLTIPS.GROWTH_HALTED, Color32.op_Implicit(GlobalAssets.Instance.colorSet.cropHalted))
    };

    public override void Update()
    {
      this.updateCropInfo.Clear();
      this.freeHarvestableNotificationIdx = 0;
      Vector2I min;
      Vector2I max;
      Grid.GetVisibleExtents(out min, out max);
      OverlayModes.Mode.RemoveOffscreenTargets<HarvestDesignatable>((ICollection<HarvestDesignatable>) this.layerTargets, min, max);
      foreach (HarvestDesignatable instance in this.partition.GetAllIntersecting(new Vector2((float) min.x, (float) min.y), new Vector2((float) max.x, (float) max.y)))
        this.AddTargetIfVisible<HarvestDesignatable>(instance, min, max, (ICollection<HarvestDesignatable>) this.layerTargets, this.targetLayer);
      foreach (HarvestDesignatable layerTarget in this.layerTargets)
      {
        Vector2I xy = Grid.PosToXY(TransformExtensions.GetPosition(layerTarget.transform));
        if (Vector2I.op_LessThanOrEqual(min, xy) && Vector2I.op_LessThanOrEqual(xy, max))
          this.AddCropUI(layerTarget);
      }
      foreach (OverlayModes.Crop.UpdateCropInfo updateCropInfo in this.updateCropInfo)
        updateCropInfo.harvestableUI.GetComponent<HarvestableOverlayWidget>().Refresh(updateCropInfo.harvestable);
      for (int harvestableNotificationIdx = this.freeHarvestableNotificationIdx; harvestableNotificationIdx < this.harvestableNotificationList.Count; ++harvestableNotificationIdx)
      {
        if (this.harvestableNotificationList[harvestableNotificationIdx].activeSelf)
          this.harvestableNotificationList[harvestableNotificationIdx].SetActive(false);
      }
      this.UpdateHighlightTypeOverlay<HarvestDesignatable>(min, max, (ICollection<HarvestDesignatable>) this.layerTargets, this.targetIDs, this.highlightConditions, OverlayModes.BringToFrontLayerSetting.Constant, this.targetLayer);
      base.Update();
    }

    public override void Disable()
    {
      this.DisableHarvestableUINotifications();
      base.Disable();
    }

    private void DisableHarvestableUINotifications()
    {
      this.freeHarvestableNotificationIdx = 0;
      foreach (GameObject harvestableNotification in this.harvestableNotificationList)
        harvestableNotification.SetActive(false);
      this.updateCropInfo.Clear();
    }

    public GameObject GetFreeCropUI()
    {
      GameObject freeCropUi;
      if (this.freeHarvestableNotificationIdx < this.harvestableNotificationList.Count)
      {
        freeCropUi = this.harvestableNotificationList[this.freeHarvestableNotificationIdx];
        if (!freeCropUi.gameObject.activeSelf)
          freeCropUi.gameObject.SetActive(true);
        ++this.freeHarvestableNotificationIdx;
      }
      else
      {
        freeCropUi = Util.KInstantiateUI(this.harvestableNotificationPrefab.gameObject, ((Component) ((Component) this.uiRoot).transform).gameObject, false);
        this.harvestableNotificationList.Add(freeCropUi);
        ++this.freeHarvestableNotificationIdx;
      }
      return freeCropUi;
    }

    private void AddCropUI(HarvestDesignatable harvestable)
    {
      GameObject freeCropUi = this.GetFreeCropUI();
      OverlayModes.Crop.UpdateCropInfo updateCropInfo = new OverlayModes.Crop.UpdateCropInfo(harvestable, freeCropUi);
      Vector3 pos = Grid.CellToPos(Grid.PosToCell((KMonoBehaviour) harvestable), 0.5f, -1.25f, 0.0f);
      TransformExtensions.SetPosition((Transform) freeCropUi.GetComponent<RectTransform>(), Vector3.op_Addition(Vector3.up, pos));
      this.updateCropInfo.Add(updateCropInfo);
    }

    private struct UpdateCropInfo
    {
      public HarvestDesignatable harvestable;
      public GameObject harvestableUI;

      public UpdateCropInfo(HarvestDesignatable harvestable, GameObject harvestableUI)
      {
        this.harvestable = harvestable;
        this.harvestableUI = harvestableUI;
      }
    }
  }

  public class Harvest : OverlayModes.BasePlantMode
  {
    public static readonly HashedString ID = HashedString.op_Implicit("HarvestWhenReady");
    private OverlayModes.ColorHighlightCondition[] highlightConditions = new OverlayModes.ColorHighlightCondition[1]
    {
      new OverlayModes.ColorHighlightCondition((Func<KMonoBehaviour, Color>) (harvestable => new Color(0.65f, 0.65f, 0.65f, 0.65f)), (Func<KMonoBehaviour, bool>) (harvestable => true))
    };

    public override HashedString ViewMode() => OverlayModes.Harvest.ID;

    public override string GetSoundName() => nameof (Harvest);

    public Harvest()
      : base((ICollection<Tag>) OverlayScreen.HarvestableIDs)
    {
    }

    public override void Update()
    {
      Vector2I min;
      Vector2I max;
      Grid.GetVisibleExtents(out min, out max);
      OverlayModes.Mode.RemoveOffscreenTargets<HarvestDesignatable>((ICollection<HarvestDesignatable>) this.layerTargets, min, max);
      foreach (HarvestDesignatable instance in this.partition.GetAllIntersecting(new Vector2((float) min.x, (float) min.y), new Vector2((float) max.x, (float) max.y)))
        this.AddTargetIfVisible<HarvestDesignatable>(instance, min, max, (ICollection<HarvestDesignatable>) this.layerTargets, this.targetLayer);
      this.UpdateHighlightTypeOverlay<HarvestDesignatable>(min, max, (ICollection<HarvestDesignatable>) this.layerTargets, this.targetIDs, this.highlightConditions, OverlayModes.BringToFrontLayerSetting.Constant, this.targetLayer);
      base.Update();
    }
  }

  public abstract class BasePlantMode : OverlayModes.Mode
  {
    protected UniformGrid<HarvestDesignatable> partition;
    protected HashSet<HarvestDesignatable> layerTargets = new HashSet<HarvestDesignatable>();
    protected ICollection<Tag> targetIDs;
    protected int targetLayer;
    private int cameraLayerMask;
    private int selectionMask;

    public BasePlantMode(ICollection<Tag> ids)
    {
      this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
      this.cameraLayerMask = LayerMask.GetMask(new string[2]
      {
        "MaskedOverlay",
        "MaskedOverlayBG"
      });
      this.selectionMask = LayerMask.GetMask(new string[1]
      {
        "MaskedOverlay"
      });
      this.targetIDs = ids;
    }

    public override void Enable()
    {
      this.RegisterSaveLoadListeners();
      this.partition = OverlayModes.Mode.PopulatePartition<HarvestDesignatable>(this.targetIDs);
      Camera.main.cullingMask |= this.cameraLayerMask;
      SelectTool.Instance.SetLayerMask(this.selectionMask);
    }

    protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
    {
      if (!this.targetIDs.Contains(((Component) item).GetComponent<KPrefabID>().GetSaveLoadTag()))
        return;
      HarvestDesignatable component = ((Component) item).GetComponent<HarvestDesignatable>();
      if (Object.op_Equality((Object) component, (Object) null))
        return;
      this.partition.Add(component);
    }

    protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
    {
      if (Object.op_Equality((Object) item, (Object) null) || Object.op_Equality((Object) ((Component) item).gameObject, (Object) null))
        return;
      HarvestDesignatable component = ((Component) item).GetComponent<HarvestDesignatable>();
      if (Object.op_Equality((Object) component, (Object) null))
        return;
      if (this.layerTargets.Contains(component))
        this.layerTargets.Remove(component);
      this.partition.Remove(component);
    }

    public override void Disable()
    {
      this.UnregisterSaveLoadListeners();
      this.DisableHighlightTypeOverlay<HarvestDesignatable>((ICollection<HarvestDesignatable>) this.layerTargets);
      Camera.main.cullingMask &= ~this.cameraLayerMask;
      this.partition.Clear();
      this.layerTargets.Clear();
      SelectTool.Instance.ClearLayerMask();
    }
  }

  public class Decor : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (Decor));
    private UniformGrid<DecorProvider> partition;
    private HashSet<DecorProvider> layerTargets = new HashSet<DecorProvider>();
    private List<DecorProvider> workingTargets = new List<DecorProvider>();
    private HashSet<Tag> targetIDs = new HashSet<Tag>();
    private int targetLayer;
    private int cameraLayerMask;
    private OverlayModes.ColorHighlightCondition[] highlightConditions = new OverlayModes.ColorHighlightCondition[1]
    {
      new OverlayModes.ColorHighlightCondition((Func<KMonoBehaviour, Color>) (dp =>
      {
        Color black = Color.black;
        Color color = Color.black;
        if (Object.op_Inequality((Object) dp, (Object) null))
        {
          int cell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
          float decorForCell1 = (dp as DecorProvider).GetDecorForCell(cell);
          if ((double) decorForCell1 > 0.0)
            color = Color32.op_Implicit(GlobalAssets.Instance.colorSet.decorHighlightPositive);
          else if ((double) decorForCell1 < 0.0)
            color = Color32.op_Implicit(GlobalAssets.Instance.colorSet.decorHighlightNegative);
          else if (Object.op_Inequality((Object) ((Component) dp).GetComponent<MonumentPart>(), (Object) null) && ((Component) dp).GetComponent<MonumentPart>().IsMonumentCompleted())
          {
            foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) dp).GetComponent<AttachableBuilding>()))
            {
              float decorForCell2 = gameObject.GetComponent<DecorProvider>().GetDecorForCell(cell);
              if ((double) decorForCell2 > 0.0)
              {
                color = Color32.op_Implicit(GlobalAssets.Instance.colorSet.decorHighlightPositive);
                break;
              }
              if ((double) decorForCell2 < 0.0)
              {
                color = Color32.op_Implicit(GlobalAssets.Instance.colorSet.decorHighlightNegative);
                break;
              }
            }
          }
        }
        return Color.Lerp(black, color, 0.85f);
      }), (Func<KMonoBehaviour, bool>) (dp => SelectToolHoverTextCard.highlightedObjects.Contains(((Component) dp).gameObject)))
    };

    public override HashedString ViewMode() => OverlayModes.Decor.ID;

    public override string GetSoundName() => nameof (Decor);

    public override List<LegendEntry> GetCustomLegendData() => new List<LegendEntry>()
    {
      new LegendEntry((string) STRINGS.UI.OVERLAYS.DECOR.HIGHDECOR, (string) STRINGS.UI.OVERLAYS.DECOR.TOOLTIPS.HIGHDECOR, Color32.op_Implicit(GlobalAssets.Instance.colorSet.decorPositive)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.DECOR.LOWDECOR, (string) STRINGS.UI.OVERLAYS.DECOR.TOOLTIPS.LOWDECOR, Color32.op_Implicit(GlobalAssets.Instance.colorSet.decorNegative))
    };

    public Decor()
    {
      this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
      this.cameraLayerMask = LayerMask.GetMask(new string[2]
      {
        "MaskedOverlay",
        "MaskedOverlayBG"
      });
    }

    public override void Enable()
    {
      this.RegisterSaveLoadListeners();
      this.targetIDs.UnionWith((IEnumerable<Tag>) Assets.GetPrefabTagsWithComponent<DecorProvider>());
      Tag[] tagArray = new Tag[5]
      {
        new Tag("Tile"),
        new Tag("MeshTile"),
        new Tag("InsulationTile"),
        new Tag("GasPermeableMembrane"),
        new Tag("CarpetTile")
      };
      foreach (Tag tag in tagArray)
        this.targetIDs.Remove(tag);
      foreach (Tag gasVentId in OverlayScreen.GasVentIDs)
        this.targetIDs.Remove(gasVentId);
      foreach (Tag liquidVentId in OverlayScreen.LiquidVentIDs)
        this.targetIDs.Remove(liquidVentId);
      this.partition = OverlayModes.Mode.PopulatePartition<DecorProvider>((ICollection<Tag>) this.targetIDs);
      Camera.main.cullingMask |= this.cameraLayerMask;
    }

    public override void Update()
    {
      Vector2I min;
      Vector2I max;
      Grid.GetVisibleExtents(out min, out max);
      OverlayModes.Mode.RemoveOffscreenTargets<DecorProvider>((ICollection<DecorProvider>) this.layerTargets, min, max);
      this.partition.GetAllIntersecting(new Vector2((float) min.x, (float) min.y), new Vector2((float) max.x, (float) max.y), (ICollection<DecorProvider>) this.workingTargets);
      for (int index = 0; index < this.workingTargets.Count; ++index)
        this.AddTargetIfVisible<DecorProvider>(this.workingTargets[index], min, max, (ICollection<DecorProvider>) this.layerTargets, this.targetLayer);
      this.UpdateHighlightTypeOverlay<DecorProvider>(min, max, (ICollection<DecorProvider>) this.layerTargets, (ICollection<Tag>) this.targetIDs, this.highlightConditions, OverlayModes.BringToFrontLayerSetting.Conditional, this.targetLayer);
      this.workingTargets.Clear();
    }

    protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
    {
      if (!this.targetIDs.Contains(((Component) item).GetComponent<KPrefabID>().GetSaveLoadTag()))
        return;
      DecorProvider component = ((Component) item).GetComponent<DecorProvider>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      this.partition.Add(component);
    }

    protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
    {
      if (Object.op_Equality((Object) item, (Object) null) || Object.op_Equality((Object) ((Component) item).gameObject, (Object) null))
        return;
      DecorProvider component = ((Component) item).GetComponent<DecorProvider>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      if (this.layerTargets.Contains(component))
        this.layerTargets.Remove(component);
      this.partition.Remove(component);
    }

    public override void Disable()
    {
      this.DisableHighlightTypeOverlay<DecorProvider>((ICollection<DecorProvider>) this.layerTargets);
      Camera.main.cullingMask &= ~this.cameraLayerMask;
      this.UnregisterSaveLoadListeners();
      this.partition.Clear();
      this.layerTargets.Clear();
    }
  }

  public class Disease : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (Disease));
    private int cameraLayerMask;
    private int freeDiseaseUI;
    private List<GameObject> diseaseUIList = new List<GameObject>();
    private List<OverlayModes.Disease.UpdateDiseaseInfo> updateDiseaseInfo = new List<OverlayModes.Disease.UpdateDiseaseInfo>();
    private HashSet<KMonoBehaviour> layerTargets = new HashSet<KMonoBehaviour>();
    private HashSet<KMonoBehaviour> privateTargets = new HashSet<KMonoBehaviour>();
    private List<KMonoBehaviour> queuedAdds = new List<KMonoBehaviour>();
    private Canvas diseaseUIParent;
    private GameObject diseaseOverlayPrefab;

    private static float CalculateHUE(Color32 colour)
    {
      byte num1 = Math.Max(colour.r, Math.Max(colour.g, colour.b));
      byte num2 = Math.Min(colour.r, Math.Min(colour.g, colour.b));
      float hue = 0.0f;
      int num3 = (int) num1 - (int) num2;
      if (num3 == 0)
        hue = 0.0f;
      else if ((int) num1 == (int) colour.r)
        hue = (float) ((double) ((int) colour.g - (int) colour.b) / (double) num3 % 6.0);
      else if ((int) num1 == (int) colour.g)
        hue = (float) ((double) ((int) colour.b - (int) colour.r) / (double) num3 + 2.0);
      else if ((int) num1 == (int) colour.b)
        hue = (float) ((double) ((int) colour.r - (int) colour.g) / (double) num3 + 4.0);
      return hue;
    }

    public override HashedString ViewMode() => OverlayModes.Disease.ID;

    public override string GetSoundName() => nameof (Disease);

    public Disease(Canvas diseaseUIParent, GameObject diseaseOverlayPrefab)
    {
      this.diseaseUIParent = diseaseUIParent;
      this.diseaseOverlayPrefab = diseaseOverlayPrefab;
      this.legendFilters = this.CreateDefaultFilters();
      this.cameraLayerMask = LayerMask.GetMask(new string[2]
      {
        "MaskedOverlay",
        "MaskedOverlayBG"
      });
    }

    public override void Enable()
    {
      Infrared.Instance.SetMode(Infrared.Mode.Disease);
      CameraController.Instance.ToggleColouredOverlayView(true);
      Camera.main.cullingMask |= this.cameraLayerMask;
      this.RegisterSaveLoadListeners();
      foreach (DiseaseSourceVisualizer sourceVisualizer in Components.DiseaseSourceVisualizers.Items)
      {
        if (!Object.op_Equality((Object) sourceVisualizer, (Object) null))
          sourceVisualizer.Show(this.ViewMode());
      }
    }

    public override Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters() => new Dictionary<string, ToolParameterMenu.ToggleState>()
    {
      {
        ToolParameterMenu.FILTERLAYERS.ALL,
        ToolParameterMenu.ToggleState.On
      },
      {
        ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT,
        ToolParameterMenu.ToggleState.Off
      },
      {
        ToolParameterMenu.FILTERLAYERS.GASCONDUIT,
        ToolParameterMenu.ToggleState.Off
      }
    };

    public override void OnFiltersChanged()
    {
      Game.Instance.showGasConduitDisease = this.InFilter(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, this.legendFilters);
      Game.Instance.showLiquidConduitDisease = this.InFilter(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, this.legendFilters);
    }

    protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
    {
      if (Object.op_Equality((Object) item, (Object) null))
        return;
      KBatchedAnimController component = ((Component) item).GetComponent<KBatchedAnimController>();
      if (Object.op_Equality((Object) component, (Object) null))
        return;
      InfraredVisualizerComponents.ClearOverlayColour(component);
    }

    protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
    {
    }

    public override void Disable()
    {
      foreach (DiseaseSourceVisualizer sourceVisualizer in Components.DiseaseSourceVisualizers.Items)
      {
        if (!Object.op_Equality((Object) sourceVisualizer, (Object) null))
          sourceVisualizer.Show(OverlayModes.None.ID);
      }
      this.UnregisterSaveLoadListeners();
      Camera.main.cullingMask &= ~this.cameraLayerMask;
      foreach (KMonoBehaviour layerTarget in this.layerTargets)
      {
        if (!Object.op_Equality((Object) layerTarget, (Object) null))
        {
          float defaultDepth = OverlayModes.Mode.GetDefaultDepth(layerTarget);
          Vector3 position = TransformExtensions.GetPosition(layerTarget.transform);
          position.z = defaultDepth;
          TransformExtensions.SetPosition(layerTarget.transform, position);
          KBatchedAnimController component = ((Component) layerTarget).GetComponent<KBatchedAnimController>();
          component.enabled = false;
          component.enabled = true;
        }
      }
      CameraController.Instance.ToggleColouredOverlayView(false);
      Infrared.Instance.SetMode(Infrared.Mode.Disabled);
      Game.Instance.showGasConduitDisease = false;
      Game.Instance.showLiquidConduitDisease = false;
      this.freeDiseaseUI = 0;
      foreach (OverlayModes.Disease.UpdateDiseaseInfo updateDiseaseInfo in this.updateDiseaseInfo)
        ((Component) updateDiseaseInfo.ui).gameObject.SetActive(false);
      this.updateDiseaseInfo.Clear();
      this.privateTargets.Clear();
      this.layerTargets.Clear();
    }

    public override List<LegendEntry> GetCustomLegendData()
    {
      List<LegendEntry> customLegendData = new List<LegendEntry>();
      List<OverlayModes.Disease.DiseaseSortInfo> diseaseSortInfoList = new List<OverlayModes.Disease.DiseaseSortInfo>();
      foreach (Klei.AI.Disease resource in Db.Get().Diseases.resources)
        diseaseSortInfoList.Add(new OverlayModes.Disease.DiseaseSortInfo(resource));
      diseaseSortInfoList.Sort((Comparison<OverlayModes.Disease.DiseaseSortInfo>) ((a, b) => a.sortkey.CompareTo(b.sortkey)));
      foreach (OverlayModes.Disease.DiseaseSortInfo diseaseSortInfo in diseaseSortInfoList)
        customLegendData.Add(new LegendEntry(diseaseSortInfo.disease.Name, diseaseSortInfo.disease.overlayLegendHovertext.ToString(), Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(diseaseSortInfo.disease.overlayColourName))));
      return customLegendData;
    }

    public GameObject GetFreeDiseaseUI()
    {
      GameObject freeDiseaseUi;
      if (this.freeDiseaseUI < this.diseaseUIList.Count)
      {
        freeDiseaseUi = this.diseaseUIList[this.freeDiseaseUI];
        freeDiseaseUi.gameObject.SetActive(true);
        ++this.freeDiseaseUI;
      }
      else
      {
        freeDiseaseUi = Util.KInstantiateUI(this.diseaseOverlayPrefab, ((Component) ((Component) this.diseaseUIParent).transform).gameObject, false);
        this.diseaseUIList.Add(freeDiseaseUi);
        ++this.freeDiseaseUI;
      }
      return freeDiseaseUi;
    }

    private void AddDiseaseUI(MinionIdentity target)
    {
      GameObject freeDiseaseUi = this.GetFreeDiseaseUI();
      DiseaseOverlayWidget component1 = freeDiseaseUi.GetComponent<DiseaseOverlayWidget>();
      OverlayModes.Disease.UpdateDiseaseInfo updateDiseaseInfo = new OverlayModes.Disease.UpdateDiseaseInfo(((Component) target).GetComponent<Modifiers>().amounts.Get(Db.Get().Amounts.ImmuneLevel), component1);
      KAnimControllerBase component2 = ((Component) target).GetComponent<KAnimControllerBase>();
      Vector3 vector3 = Object.op_Inequality((Object) component2, (Object) null) ? component2.GetWorldPivot() : Vector3.op_Addition(TransformExtensions.GetPosition(target.transform), Vector3.down);
      TransformExtensions.SetPosition((Transform) freeDiseaseUi.GetComponent<RectTransform>(), vector3);
      this.updateDiseaseInfo.Add(updateDiseaseInfo);
    }

    public override void Update()
    {
      Vector2I min;
      Vector2I max;
      Grid.GetVisibleExtents(out min, out max);
      KProfiler.Region region;
      // ISSUE: explicit constructor call
      ((KProfiler.Region) ref region).\u002Ector("UpdateDiseaseCarriers", (Object) null);
      try
      {
        this.queuedAdds.Clear();
        foreach (MinionIdentity target in Components.LiveMinionIdentities.Items)
        {
          if (!Object.op_Equality((Object) target, (Object) null))
          {
            Vector2I xy = Grid.PosToXY(TransformExtensions.GetPosition(target.transform));
            if (Vector2I.op_LessThanOrEqual(min, xy) && Vector2I.op_LessThanOrEqual(xy, max) && !this.privateTargets.Contains((KMonoBehaviour) target))
            {
              this.AddDiseaseUI(target);
              this.queuedAdds.Add((KMonoBehaviour) target);
            }
          }
        }
        foreach (KMonoBehaviour queuedAdd in this.queuedAdds)
          this.privateTargets.Add(queuedAdd);
        this.queuedAdds.Clear();
      }
      finally
      {
        region.Dispose();
      }
      foreach (OverlayModes.Disease.UpdateDiseaseInfo updateDiseaseInfo in this.updateDiseaseInfo)
        updateDiseaseInfo.ui.Refresh(updateDiseaseInfo.valueSrc);
      bool flag = false;
      if (Game.Instance.showLiquidConduitDisease)
      {
        foreach (Tag liquidVentId in OverlayScreen.LiquidVentIDs)
        {
          if (!OverlayScreen.DiseaseIDs.Contains(liquidVentId))
          {
            OverlayScreen.DiseaseIDs.Add(liquidVentId);
            flag = true;
          }
        }
      }
      else
      {
        foreach (Tag liquidVentId in OverlayScreen.LiquidVentIDs)
        {
          if (OverlayScreen.DiseaseIDs.Contains(liquidVentId))
          {
            OverlayScreen.DiseaseIDs.Remove(liquidVentId);
            flag = true;
          }
        }
      }
      if (Game.Instance.showGasConduitDisease)
      {
        foreach (Tag gasVentId in OverlayScreen.GasVentIDs)
        {
          if (!OverlayScreen.DiseaseIDs.Contains(gasVentId))
          {
            OverlayScreen.DiseaseIDs.Add(gasVentId);
            flag = true;
          }
        }
      }
      else
      {
        foreach (Tag gasVentId in OverlayScreen.GasVentIDs)
        {
          if (OverlayScreen.DiseaseIDs.Contains(gasVentId))
          {
            OverlayScreen.DiseaseIDs.Remove(gasVentId);
            flag = true;
          }
        }
      }
      if (!flag)
        return;
      this.SetLayerZ(-50f);
    }

    private void SetLayerZ(float offset_z)
    {
      Vector2I min;
      Vector2I max;
      Grid.GetVisibleExtents(out min, out max);
      OverlayModes.Mode.ClearOutsideViewObjects<KMonoBehaviour>((ICollection<KMonoBehaviour>) this.layerTargets, min, max, (ICollection<Tag>) OverlayScreen.DiseaseIDs, (Action<KMonoBehaviour>) (go =>
      {
        if (!Object.op_Inequality((Object) go, (Object) null))
          return;
        float defaultDepth = OverlayModes.Mode.GetDefaultDepth(go);
        Vector3 position = TransformExtensions.GetPosition(go.transform);
        position.z = defaultDepth;
        TransformExtensions.SetPosition(go.transform, position);
        KBatchedAnimController component = ((Component) go).GetComponent<KBatchedAnimController>();
        component.enabled = false;
        component.enabled = true;
      }));
      Dictionary<Tag, List<SaveLoadRoot>> lists = SaveLoader.Instance.saveManager.GetLists();
      foreach (Tag diseaseId in OverlayScreen.DiseaseIDs)
      {
        List<SaveLoadRoot> saveLoadRootList;
        if (lists.TryGetValue(diseaseId, out saveLoadRootList))
        {
          foreach (KMonoBehaviour cmp in saveLoadRootList)
          {
            if (!Object.op_Equality((Object) cmp, (Object) null) && !this.layerTargets.Contains(cmp))
            {
              Vector3 position = TransformExtensions.GetPosition(cmp.transform);
              if (Grid.IsVisible(Grid.PosToCell(position)) && Vector2I.op_LessThanOrEqual(min, Vector2.op_Implicit(position)) && Vector2I.op_LessThanOrEqual(Vector2.op_Implicit(position), max))
              {
                float defaultDepth = OverlayModes.Mode.GetDefaultDepth(cmp);
                position.z = defaultDepth + offset_z;
                TransformExtensions.SetPosition(cmp.transform, position);
                KBatchedAnimController component = ((Component) cmp).GetComponent<KBatchedAnimController>();
                component.enabled = false;
                component.enabled = true;
                this.layerTargets.Add(cmp);
              }
            }
          }
        }
      }
    }

    private struct DiseaseSortInfo
    {
      public float sortkey;
      public Klei.AI.Disease disease;

      public DiseaseSortInfo(Klei.AI.Disease d)
      {
        this.disease = d;
        this.sortkey = OverlayModes.Disease.CalculateHUE(GlobalAssets.Instance.colorSet.GetColorByName(d.overlayColourName));
      }
    }

    private struct UpdateDiseaseInfo
    {
      public DiseaseOverlayWidget ui;
      public AmountInstance valueSrc;

      public UpdateDiseaseInfo(AmountInstance amount_inst, DiseaseOverlayWidget ui)
      {
        this.ui = ui;
        this.valueSrc = amount_inst;
      }
    }
  }

  public class Logic : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (Logic));
    public static HashSet<Tag> HighlightItemIDs = new HashSet<Tag>();
    public static KAnimHashedString RIBBON_WIRE_1_SYMBOL_NAME = KAnimHashedString.op_Implicit("wire1");
    public static KAnimHashedString RIBBON_WIRE_2_SYMBOL_NAME = KAnimHashedString.op_Implicit("wire2");
    public static KAnimHashedString RIBBON_WIRE_3_SYMBOL_NAME = KAnimHashedString.op_Implicit("wire3");
    public static KAnimHashedString RIBBON_WIRE_4_SYMBOL_NAME = KAnimHashedString.op_Implicit("wire4");
    private int conduitTargetLayer;
    private int objectTargetLayer;
    private int cameraLayerMask;
    private int selectionMask;
    private UniformGrid<ILogicUIElement> ioPartition;
    private HashSet<ILogicUIElement> ioTargets = new HashSet<ILogicUIElement>();
    private HashSet<ILogicUIElement> workingIOTargets = new HashSet<ILogicUIElement>();
    private HashSet<KBatchedAnimController> wireControllers = new HashSet<KBatchedAnimController>();
    private HashSet<KBatchedAnimController> ribbonControllers = new HashSet<KBatchedAnimController>();
    private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();
    private List<int> visited = new List<int>();
    private HashSet<OverlayModes.Logic.BridgeInfo> bridgeControllers = new HashSet<OverlayModes.Logic.BridgeInfo>();
    private HashSet<OverlayModes.Logic.BridgeInfo> ribbonBridgeControllers = new HashSet<OverlayModes.Logic.BridgeInfo>();
    private UniformGrid<SaveLoadRoot> gameObjPartition;
    private HashSet<SaveLoadRoot> gameObjTargets = new HashSet<SaveLoadRoot>();
    private LogicModeUI uiAsset;
    private Dictionary<ILogicUIElement, OverlayModes.Logic.EventInfo> uiNodes = new Dictionary<ILogicUIElement, OverlayModes.Logic.EventInfo>();
    private KCompactedVector<OverlayModes.Logic.UIInfo> uiInfo = new KCompactedVector<OverlayModes.Logic.UIInfo>(0);

    public override HashedString ViewMode() => OverlayModes.Logic.ID;

    public override string GetSoundName() => nameof (Logic);

    public override List<LegendEntry> GetCustomLegendData() => new List<LegendEntry>()
    {
      new LegendEntry((string) STRINGS.UI.OVERLAYS.LOGIC.INPUT, (string) STRINGS.UI.OVERLAYS.LOGIC.TOOLTIPS.INPUT, Color.white, sprite: Assets.GetSprite(HashedString.op_Implicit("logicInput"))),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.LOGIC.OUTPUT, (string) STRINGS.UI.OVERLAYS.LOGIC.TOOLTIPS.OUTPUT, Color.white, sprite: Assets.GetSprite(HashedString.op_Implicit("logicOutput"))),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.LOGIC.RIBBON_INPUT, (string) STRINGS.UI.OVERLAYS.LOGIC.TOOLTIPS.RIBBON_INPUT, Color.white, sprite: Assets.GetSprite(HashedString.op_Implicit("logic_ribbon_all_in"))),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.LOGIC.RIBBON_OUTPUT, (string) STRINGS.UI.OVERLAYS.LOGIC.TOOLTIPS.RIBBON_OUTPUT, Color.white, sprite: Assets.GetSprite(HashedString.op_Implicit("logic_ribbon_all_out"))),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.LOGIC.RESET_UPDATE, (string) STRINGS.UI.OVERLAYS.LOGIC.TOOLTIPS.RESET_UPDATE, Color.white, sprite: Assets.GetSprite(HashedString.op_Implicit("logicResetUpdate"))),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.LOGIC.CONTROL_INPUT, (string) STRINGS.UI.OVERLAYS.LOGIC.TOOLTIPS.CONTROL_INPUT, Color.white, sprite: Assets.GetSprite(HashedString.op_Implicit("control_input_frame_legend"))),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.LOGIC.CIRCUIT_STATUS_HEADER, (string) null, Color.white, displaySprite: false),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.LOGIC.ONE, (string) null, Color32.op_Implicit(GlobalAssets.Instance.colorSet.logicOnText)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.LOGIC.ZERO, (string) null, Color32.op_Implicit(GlobalAssets.Instance.colorSet.logicOffText)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.LOGIC.DISCONNECTED, (string) STRINGS.UI.OVERLAYS.LOGIC.TOOLTIPS.DISCONNECTED, Color32.op_Implicit(GlobalAssets.Instance.colorSet.logicDisconnected))
    };

    public Logic(LogicModeUI ui_asset)
    {
      this.conduitTargetLayer = LayerMask.NameToLayer("MaskedOverlay");
      this.objectTargetLayer = LayerMask.NameToLayer("MaskedOverlayBG");
      this.cameraLayerMask = LayerMask.GetMask(new string[2]
      {
        "MaskedOverlay",
        "MaskedOverlayBG"
      });
      this.selectionMask = this.cameraLayerMask;
      this.uiAsset = ui_asset;
    }

    public override void Enable()
    {
      Camera.main.cullingMask |= this.cameraLayerMask;
      SelectTool.Instance.SetLayerMask(this.selectionMask);
      this.RegisterSaveLoadListeners();
      this.gameObjPartition = OverlayModes.Mode.PopulatePartition<SaveLoadRoot>((ICollection<Tag>) OverlayModes.Logic.HighlightItemIDs);
      this.ioPartition = this.CreateLogicUIPartition();
      GridCompositor.Instance.ToggleMinor(true);
      Game.Instance.logicCircuitManager.onElemAdded += new Action<ILogicUIElement>(this.OnUIElemAdded);
      Game.Instance.logicCircuitManager.onElemRemoved += new Action<ILogicUIElement>(this.OnUIElemRemoved);
      AudioMixer.instance.Start(AudioMixerSnapshots.Get().TechFilterLogicOn);
    }

    public override void Disable()
    {
      Game.Instance.logicCircuitManager.onElemAdded -= new Action<ILogicUIElement>(this.OnUIElemAdded);
      Game.Instance.logicCircuitManager.onElemRemoved -= new Action<ILogicUIElement>(this.OnUIElemRemoved);
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().TechFilterLogicOn);
      foreach (SaveLoadRoot gameObjTarget in this.gameObjTargets)
      {
        float defaultDepth = OverlayModes.Mode.GetDefaultDepth((KMonoBehaviour) gameObjTarget);
        Vector3 position = TransformExtensions.GetPosition(gameObjTarget.transform);
        position.z = defaultDepth;
        TransformExtensions.SetPosition(gameObjTarget.transform, position);
        ((Component) gameObjTarget).GetComponent<KBatchedAnimController>().enabled = false;
        ((Component) gameObjTarget).GetComponent<KBatchedAnimController>().enabled = true;
      }
      OverlayModes.Mode.ResetDisplayValues<SaveLoadRoot>((ICollection<SaveLoadRoot>) this.gameObjTargets);
      OverlayModes.Mode.ResetDisplayValues<KBatchedAnimController>((ICollection<KBatchedAnimController>) this.wireControllers);
      OverlayModes.Mode.ResetDisplayValues<KBatchedAnimController>((ICollection<KBatchedAnimController>) this.ribbonControllers);
      this.ResetRibbonSymbolTints<KBatchedAnimController>((ICollection<KBatchedAnimController>) this.ribbonControllers);
      foreach (OverlayModes.Logic.BridgeInfo bridgeController in this.bridgeControllers)
      {
        if (Object.op_Inequality((Object) bridgeController.controller, (Object) null))
          OverlayModes.Mode.ResetDisplayValues(bridgeController.controller);
      }
      foreach (OverlayModes.Logic.BridgeInfo bridgeController in this.ribbonBridgeControllers)
      {
        if (Object.op_Inequality((Object) bridgeController.controller, (Object) null))
          this.ResetRibbonTint(bridgeController.controller);
      }
      Camera.main.cullingMask &= ~this.cameraLayerMask;
      SelectTool.Instance.ClearLayerMask();
      this.UnregisterSaveLoadListeners();
      foreach (OverlayModes.Logic.UIInfo data in this.uiInfo.GetDataList())
        data.Release();
      this.uiInfo.Clear();
      this.uiNodes.Clear();
      this.ioPartition.Clear();
      this.ioTargets.Clear();
      this.gameObjPartition.Clear();
      this.gameObjTargets.Clear();
      this.wireControllers.Clear();
      this.ribbonControllers.Clear();
      this.bridgeControllers.Clear();
      this.ribbonBridgeControllers.Clear();
      GridCompositor.Instance.ToggleMinor(false);
    }

    protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
    {
      Tag saveLoadTag = ((Component) item).GetComponent<KPrefabID>().GetSaveLoadTag();
      if (!OverlayModes.Logic.HighlightItemIDs.Contains(saveLoadTag))
        return;
      this.gameObjPartition.Add(item);
    }

    protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
    {
      if (Object.op_Equality((Object) item, (Object) null) || Object.op_Equality((Object) ((Component) item).gameObject, (Object) null))
        return;
      if (this.gameObjTargets.Contains(item))
        this.gameObjTargets.Remove(item);
      this.gameObjPartition.Remove(item);
    }

    private void OnUIElemAdded(ILogicUIElement elem) => this.ioPartition.Add(elem);

    private void OnUIElemRemoved(ILogicUIElement elem)
    {
      this.ioPartition.Remove(elem);
      if (!this.ioTargets.Contains(elem))
        return;
      this.ioTargets.Remove(elem);
      this.FreeUI(elem);
    }

    public override void Update()
    {
      Vector2I min;
      Vector2I max;
      Grid.GetVisibleExtents(out min, out max);
      Tag wire_id = TagManager.Create("LogicWire");
      Tag ribbon_id = TagManager.Create("LogicRibbon");
      Tag bridge_id = TagManager.Create("LogicWireBridge");
      Tag ribbon_bridge_id = TagManager.Create("LogicRibbonBridge");
      OverlayModes.Mode.RemoveOffscreenTargets<SaveLoadRoot>((ICollection<SaveLoadRoot>) this.gameObjTargets, min, max, (Action<SaveLoadRoot>) (root =>
      {
        if (Object.op_Equality((Object) root, (Object) null))
          return;
        KPrefabID component = ((Component) root).GetComponent<KPrefabID>();
        if (!Object.op_Inequality((Object) component, (Object) null))
          return;
        Tag prefabTag = component.PrefabTag;
        if (Tag.op_Equality(prefabTag, wire_id))
          this.wireControllers.Remove(((Component) root).GetComponent<KBatchedAnimController>());
        else if (Tag.op_Equality(prefabTag, ribbon_id))
        {
          this.ResetRibbonTint(((Component) root).GetComponent<KBatchedAnimController>());
          this.ribbonControllers.Remove(((Component) root).GetComponent<KBatchedAnimController>());
        }
        else if (Tag.op_Equality(prefabTag, bridge_id))
        {
          KBatchedAnimController controller = ((Component) root).GetComponent<KBatchedAnimController>();
          this.bridgeControllers.RemoveWhere((Predicate<OverlayModes.Logic.BridgeInfo>) (x => Object.op_Equality((Object) x.controller, (Object) controller)));
        }
        else if (Tag.op_Equality(prefabTag, ribbon_bridge_id))
        {
          KBatchedAnimController controller = ((Component) root).GetComponent<KBatchedAnimController>();
          this.ResetRibbonTint(controller);
          this.ribbonBridgeControllers.RemoveWhere((Predicate<OverlayModes.Logic.BridgeInfo>) (x => Object.op_Equality((Object) x.controller, (Object) controller)));
        }
        else
        {
          float defaultDepth = OverlayModes.Mode.GetDefaultDepth((KMonoBehaviour) root);
          Vector3 position = TransformExtensions.GetPosition(root.transform);
          position.z = defaultDepth;
          TransformExtensions.SetPosition(root.transform, position);
          ((Component) root).GetComponent<KBatchedAnimController>().enabled = false;
          ((Component) root).GetComponent<KBatchedAnimController>().enabled = true;
        }
      }));
      OverlayModes.Mode.RemoveOffscreenTargets<ILogicUIElement>((ICollection<ILogicUIElement>) this.ioTargets, (ICollection<ILogicUIElement>) this.workingIOTargets, min, max, new Action<ILogicUIElement>(this.FreeUI));
      KProfiler.Region region;
      // ISSUE: explicit constructor call
      ((KProfiler.Region) ref region).\u002Ector("UpdateLogicOverlay", (Object) null);
      try
      {
        foreach (SaveLoadRoot instance in this.gameObjPartition.GetAllIntersecting(new Vector2((float) min.x, (float) min.y), new Vector2((float) max.x, (float) max.y)))
        {
          if (Object.op_Inequality((Object) instance, (Object) null))
          {
            KPrefabID component1 = ((Component) instance).GetComponent<KPrefabID>();
            if (Tag.op_Equality(component1.PrefabTag, wire_id) || Tag.op_Equality(component1.PrefabTag, bridge_id) || Tag.op_Equality(component1.PrefabTag, ribbon_id) || Tag.op_Equality(component1.PrefabTag, ribbon_bridge_id))
              this.AddTargetIfVisible<SaveLoadRoot>(instance, min, max, (ICollection<SaveLoadRoot>) this.gameObjTargets, this.conduitTargetLayer, (Action<SaveLoadRoot>) (root =>
              {
                if (Object.op_Equality((Object) root, (Object) null))
                  return;
                KPrefabID component2 = ((Component) root).GetComponent<KPrefabID>();
                if (!OverlayModes.Logic.HighlightItemIDs.Contains(component2.PrefabTag))
                  return;
                if (Tag.op_Equality(component2.PrefabTag, wire_id))
                  this.wireControllers.Add(((Component) root).GetComponent<KBatchedAnimController>());
                else if (Tag.op_Equality(component2.PrefabTag, ribbon_id))
                  this.ribbonControllers.Add(((Component) root).GetComponent<KBatchedAnimController>());
                else if (Tag.op_Equality(component2.PrefabTag, bridge_id))
                {
                  KBatchedAnimController component3 = ((Component) root).GetComponent<KBatchedAnimController>();
                  int networkCell = ((Component) root).GetComponent<LogicUtilityNetworkLink>().GetNetworkCell();
                  this.bridgeControllers.Add(new OverlayModes.Logic.BridgeInfo()
                  {
                    cell = networkCell,
                    controller = component3
                  });
                }
                else
                {
                  if (!Tag.op_Equality(component2.PrefabTag, ribbon_bridge_id))
                    return;
                  KBatchedAnimController component4 = ((Component) root).GetComponent<KBatchedAnimController>();
                  int networkCell = ((Component) root).GetComponent<LogicUtilityNetworkLink>().GetNetworkCell();
                  this.ribbonBridgeControllers.Add(new OverlayModes.Logic.BridgeInfo()
                  {
                    cell = networkCell,
                    controller = component4
                  });
                }
              }));
            else
              this.AddTargetIfVisible<SaveLoadRoot>(instance, min, max, (ICollection<SaveLoadRoot>) this.gameObjTargets, this.objectTargetLayer, (Action<SaveLoadRoot>) (root =>
              {
                Vector3 position = TransformExtensions.GetPosition(root.transform);
                float num = position.z;
                KPrefabID component5 = ((Component) root).GetComponent<KPrefabID>();
                if (Object.op_Inequality((Object) component5, (Object) null))
                {
                  if (component5.HasTag(GameTags.OverlayInFrontOfConduits))
                    num = Grid.GetLayerZ(Grid.SceneLayer.LogicWires) - 0.2f;
                  else if (component5.HasTag(GameTags.OverlayBehindConduits))
                    num = Grid.GetLayerZ(Grid.SceneLayer.LogicWires) + 0.2f;
                }
                position.z = num;
                TransformExtensions.SetPosition(root.transform, position);
                KBatchedAnimController component6 = ((Component) root).GetComponent<KBatchedAnimController>();
                component6.enabled = false;
                component6.enabled = true;
              }));
          }
        }
        foreach (ILogicUIElement instance in this.ioPartition.GetAllIntersecting(new Vector2((float) min.x, (float) min.y), new Vector2((float) max.x, (float) max.y)))
        {
          if (instance != null)
            this.AddTargetIfVisible<ILogicUIElement>(instance, min, max, (ICollection<ILogicUIElement>) this.ioTargets, this.objectTargetLayer, new Action<ILogicUIElement>(this.AddUI), (Func<KMonoBehaviour, bool>) (kcmp => Object.op_Inequality((Object) kcmp, (Object) null) && OverlayModes.Logic.HighlightItemIDs.Contains(((Component) kcmp).GetComponent<KPrefabID>().PrefabTag)));
        }
        this.connectedNetworks.Clear();
        float num1 = 1f;
        GameObject gameObject = (GameObject) null;
        if (Object.op_Inequality((Object) SelectTool.Instance, (Object) null) && Object.op_Inequality((Object) SelectTool.Instance.hover, (Object) null))
          gameObject = ((Component) SelectTool.Instance.hover).gameObject;
        if (Object.op_Inequality((Object) gameObject, (Object) null))
        {
          IBridgedNetworkItem component = gameObject.GetComponent<IBridgedNetworkItem>();
          if (component != null)
          {
            int networkCell = component.GetNetworkCell();
            this.visited.Clear();
            this.FindConnectedNetworks(networkCell, (IUtilityNetworkMgr) Game.Instance.logicCircuitSystem, (ICollection<UtilityNetwork>) this.connectedNetworks, this.visited);
            this.visited.Clear();
            num1 = OverlayModes.ModeUtil.GetHighlightScale();
          }
        }
        LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
        Color32 logicOn = GlobalAssets.Instance.colorSet.logicOn;
        Color32 logicOff = GlobalAssets.Instance.colorSet.logicOff;
        logicOff.a = logicOn.a = (byte) 0;
        foreach (KBatchedAnimController wireController in this.wireControllers)
        {
          if (!Object.op_Equality((Object) wireController, (Object) null))
          {
            Color32 color32 = logicOff;
            LogicCircuitNetwork networkForCell = logicCircuitManager.GetNetworkForCell(Grid.PosToCell(TransformExtensions.GetPosition(((Component) wireController).transform)));
            if (networkForCell != null)
              color32 = networkForCell.IsBitActive(0) ? logicOn : logicOff;
            if (this.connectedNetworks.Count > 0)
            {
              IBridgedNetworkItem component = ((Component) wireController).GetComponent<IBridgedNetworkItem>();
              if (component != null && component.IsConnectedToNetworks((ICollection<UtilityNetwork>) this.connectedNetworks))
              {
                color32.r = (byte) ((double) color32.r * (double) num1);
                color32.g = (byte) ((double) color32.g * (double) num1);
                color32.b = (byte) ((double) color32.b * (double) num1);
              }
            }
            wireController.TintColour = color32;
          }
        }
        foreach (KBatchedAnimController ribbonController in this.ribbonControllers)
        {
          if (!Object.op_Equality((Object) ribbonController, (Object) null))
          {
            Color32 color32_1 = logicOff;
            Color32 color32_2 = logicOff;
            Color32 color32_3 = logicOff;
            Color32 color32_4 = logicOff;
            LogicCircuitNetwork networkForCell = logicCircuitManager.GetNetworkForCell(Grid.PosToCell(TransformExtensions.GetPosition(((Component) ribbonController).transform)));
            if (networkForCell != null)
            {
              color32_1 = networkForCell.IsBitActive(0) ? logicOn : logicOff;
              color32_2 = networkForCell.IsBitActive(1) ? logicOn : logicOff;
              color32_3 = networkForCell.IsBitActive(2) ? logicOn : logicOff;
              color32_4 = networkForCell.IsBitActive(3) ? logicOn : logicOff;
            }
            if (this.connectedNetworks.Count > 0)
            {
              IBridgedNetworkItem component = ((Component) ribbonController).GetComponent<IBridgedNetworkItem>();
              if (component != null && component.IsConnectedToNetworks((ICollection<UtilityNetwork>) this.connectedNetworks))
              {
                color32_1.r = (byte) ((double) color32_1.r * (double) num1);
                color32_1.g = (byte) ((double) color32_1.g * (double) num1);
                color32_1.b = (byte) ((double) color32_1.b * (double) num1);
                color32_2.r = (byte) ((double) color32_2.r * (double) num1);
                color32_2.g = (byte) ((double) color32_2.g * (double) num1);
                color32_2.b = (byte) ((double) color32_2.b * (double) num1);
                color32_3.r = (byte) ((double) color32_3.r * (double) num1);
                color32_3.g = (byte) ((double) color32_3.g * (double) num1);
                color32_3.b = (byte) ((double) color32_3.b * (double) num1);
                color32_4.r = (byte) ((double) color32_4.r * (double) num1);
                color32_4.g = (byte) ((double) color32_4.g * (double) num1);
                color32_4.b = (byte) ((double) color32_4.b * (double) num1);
              }
            }
            ribbonController.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_1_SYMBOL_NAME, Color32.op_Implicit(color32_1));
            ribbonController.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_2_SYMBOL_NAME, Color32.op_Implicit(color32_2));
            ribbonController.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_3_SYMBOL_NAME, Color32.op_Implicit(color32_3));
            ribbonController.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_4_SYMBOL_NAME, Color32.op_Implicit(color32_4));
          }
        }
        foreach (OverlayModes.Logic.BridgeInfo bridgeController in this.bridgeControllers)
        {
          if (!Object.op_Equality((Object) bridgeController.controller, (Object) null))
          {
            Color32 color32 = logicOff;
            LogicCircuitNetwork networkForCell = logicCircuitManager.GetNetworkForCell(bridgeController.cell);
            if (networkForCell != null)
              color32 = networkForCell.IsBitActive(0) ? logicOn : logicOff;
            if (this.connectedNetworks.Count > 0)
            {
              IBridgedNetworkItem component = ((Component) bridgeController.controller).GetComponent<IBridgedNetworkItem>();
              if (component != null && component.IsConnectedToNetworks((ICollection<UtilityNetwork>) this.connectedNetworks))
              {
                color32.r = (byte) ((double) color32.r * (double) num1);
                color32.g = (byte) ((double) color32.g * (double) num1);
                color32.b = (byte) ((double) color32.b * (double) num1);
              }
            }
            bridgeController.controller.TintColour = color32;
          }
        }
        foreach (OverlayModes.Logic.BridgeInfo bridgeController in this.ribbonBridgeControllers)
        {
          if (!Object.op_Equality((Object) bridgeController.controller, (Object) null))
          {
            Color32 color32_5 = logicOff;
            Color32 color32_6 = logicOff;
            Color32 color32_7 = logicOff;
            Color32 color32_8 = logicOff;
            LogicCircuitNetwork networkForCell = logicCircuitManager.GetNetworkForCell(bridgeController.cell);
            if (networkForCell != null)
            {
              color32_5 = networkForCell.IsBitActive(0) ? logicOn : logicOff;
              color32_6 = networkForCell.IsBitActive(1) ? logicOn : logicOff;
              color32_7 = networkForCell.IsBitActive(2) ? logicOn : logicOff;
              color32_8 = networkForCell.IsBitActive(3) ? logicOn : logicOff;
            }
            if (this.connectedNetworks.Count > 0)
            {
              IBridgedNetworkItem component = ((Component) bridgeController.controller).GetComponent<IBridgedNetworkItem>();
              if (component != null && component.IsConnectedToNetworks((ICollection<UtilityNetwork>) this.connectedNetworks))
              {
                color32_5.r = (byte) ((double) color32_5.r * (double) num1);
                color32_5.g = (byte) ((double) color32_5.g * (double) num1);
                color32_5.b = (byte) ((double) color32_5.b * (double) num1);
                color32_6.r = (byte) ((double) color32_6.r * (double) num1);
                color32_6.g = (byte) ((double) color32_6.g * (double) num1);
                color32_6.b = (byte) ((double) color32_6.b * (double) num1);
                color32_7.r = (byte) ((double) color32_7.r * (double) num1);
                color32_7.g = (byte) ((double) color32_7.g * (double) num1);
                color32_7.b = (byte) ((double) color32_7.b * (double) num1);
                color32_8.r = (byte) ((double) color32_8.r * (double) num1);
                color32_8.g = (byte) ((double) color32_8.g * (double) num1);
                color32_8.b = (byte) ((double) color32_8.b * (double) num1);
              }
            }
            bridgeController.controller.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_1_SYMBOL_NAME, Color32.op_Implicit(color32_5));
            bridgeController.controller.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_2_SYMBOL_NAME, Color32.op_Implicit(color32_6));
            bridgeController.controller.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_3_SYMBOL_NAME, Color32.op_Implicit(color32_7));
            bridgeController.controller.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_4_SYMBOL_NAME, Color32.op_Implicit(color32_8));
          }
        }
      }
      finally
      {
        region.Dispose();
      }
      this.UpdateUI();
    }

    private void UpdateUI()
    {
      Color32 logicOn = GlobalAssets.Instance.colorSet.logicOn;
      Color32 logicOff = GlobalAssets.Instance.colorSet.logicOff;
      Color32 logicDisconnected = GlobalAssets.Instance.colorSet.logicDisconnected;
      logicOff.a = logicOn.a = byte.MaxValue;
      foreach (OverlayModes.Logic.UIInfo data in this.uiInfo.GetDataList())
      {
        LogicCircuitNetwork networkForCell = Game.Instance.logicCircuitManager.GetNetworkForCell(data.cell);
        Color32 color32 = logicDisconnected;
        LogicControlInputUI component1 = data.instance.GetComponent<LogicControlInputUI>();
        if (Object.op_Inequality((Object) component1, (Object) null))
          component1.SetContent(networkForCell);
        else if (data.bitDepth == 4)
        {
          LogicRibbonDisplayUI component2 = data.instance.GetComponent<LogicRibbonDisplayUI>();
          if (Object.op_Inequality((Object) component2, (Object) null))
            component2.SetContent(networkForCell);
        }
        else if (data.bitDepth == 1)
        {
          if (networkForCell != null)
            color32 = networkForCell.IsBitActive(0) ? logicOn : logicOff;
          if (Color.op_Inequality(((Graphic) data.image).color, Color32.op_Implicit(color32)))
            ((Graphic) data.image).color = Color32.op_Implicit(color32);
        }
      }
    }

    private void AddUI(ILogicUIElement ui_elem)
    {
      if (this.uiNodes.ContainsKey(ui_elem))
        return;
      HandleVector<int>.Handle handle = this.uiInfo.Allocate(new OverlayModes.Logic.UIInfo(ui_elem, this.uiAsset));
      this.uiNodes.Add(ui_elem, new OverlayModes.Logic.EventInfo()
      {
        uiHandle = handle
      });
    }

    private void FreeUI(ILogicUIElement item)
    {
      OverlayModes.Logic.EventInfo eventInfo;
      if (item == null || !this.uiNodes.TryGetValue(item, out eventInfo))
        return;
      this.uiInfo.GetData(eventInfo.uiHandle).Release();
      this.uiInfo.Free(eventInfo.uiHandle);
      this.uiNodes.Remove(item);
    }

    protected UniformGrid<ILogicUIElement> CreateLogicUIPartition()
    {
      UniformGrid<ILogicUIElement> logicUiPartition = new UniformGrid<ILogicUIElement>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
      foreach (ILogicUIElement visElement in Game.Instance.logicCircuitManager.GetVisElements())
      {
        if (visElement != null)
          logicUiPartition.Add(visElement);
      }
      return logicUiPartition;
    }

    private bool IsBitActive(int value, int bit) => (value & 1 << bit) > 0;

    private void FindConnectedNetworks(
      int cell,
      IUtilityNetworkMgr mgr,
      ICollection<UtilityNetwork> networks,
      List<int> visited)
    {
      if (visited.Contains(cell))
        return;
      visited.Add(cell);
      UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
      if (networkForCell == null)
        return;
      networks.Add(networkForCell);
      int connections = (int) mgr.GetConnections(cell, false);
      if ((connections & 2) != 0)
        this.FindConnectedNetworks(Grid.CellRight(cell), mgr, networks, visited);
      if ((connections & 1) != 0)
        this.FindConnectedNetworks(Grid.CellLeft(cell), mgr, networks, visited);
      if ((connections & 4) != 0)
        this.FindConnectedNetworks(Grid.CellAbove(cell), mgr, networks, visited);
      if ((connections & 8) == 0)
        return;
      this.FindConnectedNetworks(Grid.CellBelow(cell), mgr, networks, visited);
    }

    private void ResetRibbonSymbolTints<T>(ICollection<T> targets) where T : MonoBehaviour
    {
      foreach (T target in (IEnumerable<T>) targets)
      {
        if (!Object.op_Equality((Object) (object) target, (Object) null))
          this.ResetRibbonTint(((Component) (object) target).GetComponent<KBatchedAnimController>());
      }
    }

    private void ResetRibbonTint(KBatchedAnimController kbac)
    {
      if (!Object.op_Inequality((Object) kbac, (Object) null))
        return;
      kbac.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_1_SYMBOL_NAME, Color.white);
      kbac.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_2_SYMBOL_NAME, Color.white);
      kbac.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_3_SYMBOL_NAME, Color.white);
      kbac.SetSymbolTint(OverlayModes.Logic.RIBBON_WIRE_4_SYMBOL_NAME, Color.white);
    }

    private struct BridgeInfo
    {
      public int cell;
      public KBatchedAnimController controller;
    }

    private struct EventInfo
    {
      public HandleVector<int>.Handle uiHandle;
    }

    private struct UIInfo
    {
      public GameObject instance;
      public Image image;
      public int cell;
      public int bitDepth;

      public UIInfo(ILogicUIElement ui_elem, LogicModeUI ui_data)
      {
        this.cell = ui_elem.GetLogicUICell();
        GameObject gameObject = (GameObject) null;
        Sprite sprite = (Sprite) null;
        this.bitDepth = 1;
        switch (ui_elem.GetLogicPortSpriteType())
        {
          case LogicPortSpriteType.Input:
            gameObject = ui_data.prefab;
            sprite = ui_data.inputSprite;
            break;
          case LogicPortSpriteType.Output:
            gameObject = ui_data.prefab;
            sprite = ui_data.outputSprite;
            break;
          case LogicPortSpriteType.ResetUpdate:
            gameObject = ui_data.prefab;
            sprite = ui_data.resetSprite;
            break;
          case LogicPortSpriteType.ControlInput:
            gameObject = ui_data.controlInputPrefab;
            break;
          case LogicPortSpriteType.RibbonInput:
            gameObject = ui_data.ribbonInputPrefab;
            this.bitDepth = 4;
            break;
          case LogicPortSpriteType.RibbonOutput:
            gameObject = ui_data.ribbonOutputPrefab;
            this.bitDepth = 4;
            break;
        }
        this.instance = Util.KInstantiate(gameObject, Grid.CellToPosCCC(this.cell, Grid.SceneLayer.Front), Quaternion.identity, GameScreenManager.Instance.worldSpaceCanvas, (string) null, true, 0);
        this.instance.SetActive(true);
        this.image = this.instance.GetComponent<Image>();
        if (!Object.op_Inequality((Object) this.image, (Object) null))
          return;
        ((Graphic) this.image).raycastTarget = false;
        this.image.sprite = sprite;
      }

      public void Release() => Util.KDestroyGameObject(this.instance);
    }
  }

  public enum BringToFrontLayerSetting
  {
    None,
    Constant,
    Conditional,
  }

  public class ColorHighlightCondition
  {
    public Func<KMonoBehaviour, Color> highlight_color;
    public Func<KMonoBehaviour, bool> highlight_condition;

    public ColorHighlightCondition(
      Func<KMonoBehaviour, Color> highlight_color,
      Func<KMonoBehaviour, bool> highlight_condition)
    {
      this.highlight_color = highlight_color;
      this.highlight_condition = highlight_condition;
    }
  }

  public class None : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.Invalid;

    public override HashedString ViewMode() => OverlayModes.None.ID;

    public override string GetSoundName() => "Off";
  }

  public class PathProber : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (PathProber));

    public override HashedString ViewMode() => OverlayModes.PathProber.ID;

    public override string GetSoundName() => "Off";
  }

  public class Oxygen : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (Oxygen));

    public override HashedString ViewMode() => OverlayModes.Oxygen.ID;

    public override string GetSoundName() => nameof (Oxygen);

    public override void Enable()
    {
      base.Enable();
      int defaultLayerMask = SelectTool.Instance.GetDefaultLayerMask();
      int mask = LayerMask.GetMask(new string[1]
      {
        "MaskedOverlay"
      });
      SelectTool.Instance.SetLayerMask(defaultLayerMask | mask);
    }

    public override void Disable()
    {
      base.Disable();
      SelectTool.Instance.ClearLayerMask();
    }
  }

  public class Light : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (Light));

    public override HashedString ViewMode() => OverlayModes.Light.ID;

    public override string GetSoundName() => "Lights";
  }

  public class Priorities : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (Priorities));

    public override HashedString ViewMode() => OverlayModes.Priorities.ID;

    public override string GetSoundName() => nameof (Priorities);
  }

  public class ThermalConductivity : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (ThermalConductivity));

    public override HashedString ViewMode() => OverlayModes.ThermalConductivity.ID;

    public override string GetSoundName() => "HeatFlow";
  }

  public class HeatFlow : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (HeatFlow));

    public override HashedString ViewMode() => OverlayModes.HeatFlow.ID;

    public override string GetSoundName() => nameof (HeatFlow);
  }

  public class Rooms : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (Rooms));

    public override HashedString ViewMode() => OverlayModes.Rooms.ID;

    public override string GetSoundName() => nameof (Rooms);

    public override List<LegendEntry> GetCustomLegendData()
    {
      List<LegendEntry> customLegendData = new List<LegendEntry>();
      List<RoomType> roomTypeList = new List<RoomType>((IEnumerable<RoomType>) Db.Get().RoomTypes.resources);
      roomTypeList.Sort((Comparison<RoomType>) ((a, b) => a.sortKey.CompareTo(b.sortKey)));
      foreach (RoomType roomType in roomTypeList)
      {
        string desc = roomType.GetCriteriaString();
        if (roomType.effects != null && roomType.effects.Length != 0)
          desc = desc + "\n\n" + roomType.GetRoomEffectsString();
        customLegendData.Add(new LegendEntry(roomType.Name + "\n" + roomType.effect, desc, Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(roomType.category.colorName))));
      }
      return customLegendData;
    }
  }

  public abstract class Mode
  {
    public Dictionary<string, ToolParameterMenu.ToggleState> legendFilters;
    private static List<KMonoBehaviour> workingTargets = new List<KMonoBehaviour>();

    public static void Clear() => OverlayModes.Mode.workingTargets.Clear();

    public abstract HashedString ViewMode();

    public virtual void Enable()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void Disable()
    {
    }

    public virtual List<LegendEntry> GetCustomLegendData() => (List<LegendEntry>) null;

    public virtual Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters() => (Dictionary<string, ToolParameterMenu.ToggleState>) null;

    public virtual void OnFiltersChanged()
    {
    }

    public virtual void DisableOverlay()
    {
    }

    public abstract string GetSoundName();

    protected bool InFilter(
      string layer,
      Dictionary<string, ToolParameterMenu.ToggleState> filter)
    {
      if (filter.ContainsKey(ToolParameterMenu.FILTERLAYERS.ALL) && filter[ToolParameterMenu.FILTERLAYERS.ALL] == ToolParameterMenu.ToggleState.On)
        return true;
      return filter.ContainsKey(layer) && filter[layer] == ToolParameterMenu.ToggleState.On;
    }

    public void RegisterSaveLoadListeners()
    {
      SaveManager saveManager = SaveLoader.Instance.saveManager;
      saveManager.onRegister += new Action<SaveLoadRoot>(this.OnSaveLoadRootRegistered);
      saveManager.onUnregister += new Action<SaveLoadRoot>(this.OnSaveLoadRootUnregistered);
    }

    public void UnregisterSaveLoadListeners()
    {
      SaveManager saveManager = SaveLoader.Instance.saveManager;
      saveManager.onRegister -= new Action<SaveLoadRoot>(this.OnSaveLoadRootRegistered);
      saveManager.onUnregister -= new Action<SaveLoadRoot>(this.OnSaveLoadRootUnregistered);
    }

    protected virtual void OnSaveLoadRootRegistered(SaveLoadRoot root)
    {
    }

    protected virtual void OnSaveLoadRootUnregistered(SaveLoadRoot root)
    {
    }

    protected void ProcessExistingSaveLoadRoots()
    {
      foreach (KeyValuePair<Tag, List<SaveLoadRoot>> list in SaveLoader.Instance.saveManager.GetLists())
      {
        foreach (SaveLoadRoot root in list.Value)
          this.OnSaveLoadRootRegistered(root);
      }
    }

    protected static UniformGrid<T> PopulatePartition<T>(ICollection<Tag> tags) where T : IUniformGridObject
    {
      Dictionary<Tag, List<SaveLoadRoot>> lists = SaveLoader.Instance.saveManager.GetLists();
      UniformGrid<T> uniformGrid = new UniformGrid<T>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
      foreach (Tag tag in (IEnumerable<Tag>) tags)
      {
        List<SaveLoadRoot> saveLoadRootList = (List<SaveLoadRoot>) null;
        if (lists.TryGetValue(tag, out saveLoadRootList))
        {
          foreach (Component component1 in saveLoadRootList)
          {
            T component2 = component1.GetComponent<T>();
            if ((object) component2 != null)
              uniformGrid.Add(component2);
          }
        }
      }
      return uniformGrid;
    }

    protected static void ResetDisplayValues<T>(ICollection<T> targets) where T : MonoBehaviour
    {
      foreach (T target in (IEnumerable<T>) targets)
      {
        if (!Object.op_Equality((Object) (object) target, (Object) null))
        {
          KBatchedAnimController component = ((Component) (object) target).GetComponent<KBatchedAnimController>();
          if (Object.op_Inequality((Object) component, (Object) null))
            OverlayModes.Mode.ResetDisplayValues(component);
        }
      }
    }

    protected static void ResetDisplayValues(KBatchedAnimController controller)
    {
      controller.SetLayer(0);
      controller.HighlightColour = Color32.op_Implicit(Color.clear);
      controller.TintColour = Color32.op_Implicit(Color.white);
      controller.SetLayer(((Component) controller).GetComponent<KPrefabID>().defaultLayer);
    }

    protected static void RemoveOffscreenTargets<T>(
      ICollection<T> targets,
      Vector2I min,
      Vector2I max,
      Action<T> on_removed = null)
      where T : KMonoBehaviour
    {
      OverlayModes.Mode.ClearOutsideViewObjects<T>(targets, min, max, (ICollection<Tag>) null, (Action<T>) (cmp =>
      {
        if (!Object.op_Inequality((Object) (object) cmp, (Object) null))
          return;
        KBatchedAnimController component = ((Component) (object) cmp).GetComponent<KBatchedAnimController>();
        if (Object.op_Inequality((Object) component, (Object) null))
          OverlayModes.Mode.ResetDisplayValues(component);
        if (on_removed == null)
          return;
        on_removed(cmp);
      }));
      OverlayModes.Mode.workingTargets.Clear();
    }

    protected static void ClearOutsideViewObjects<T>(
      ICollection<T> targets,
      Vector2I vis_min,
      Vector2I vis_max,
      ICollection<Tag> item_ids,
      Action<T> on_remove)
      where T : KMonoBehaviour
    {
      OverlayModes.Mode.workingTargets.Clear();
      foreach (T target in (IEnumerable<T>) targets)
      {
        if (!Object.op_Equality((Object) (object) target, (Object) null))
        {
          Vector2I xy = Grid.PosToXY(TransformExtensions.GetPosition(((KMonoBehaviour) (object) target).transform));
          if (!Vector2I.op_LessThanOrEqual(vis_min, xy) || !Vector2I.op_LessThanOrEqual(xy, vis_max) || ((Component) (object) target).gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
          {
            OverlayModes.Mode.workingTargets.Add((KMonoBehaviour) (object) target);
          }
          else
          {
            KPrefabID component = ((Component) (object) target).GetComponent<KPrefabID>();
            if (item_ids != null && !item_ids.Contains(component.PrefabTag) && ((Component) (object) target).gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
              OverlayModes.Mode.workingTargets.Add((KMonoBehaviour) (object) target);
          }
        }
      }
      foreach (T workingTarget in OverlayModes.Mode.workingTargets)
      {
        if (!Object.op_Equality((Object) (object) workingTarget, (Object) null))
        {
          if (on_remove != null)
            on_remove(workingTarget);
          targets.Remove(workingTarget);
        }
      }
      OverlayModes.Mode.workingTargets.Clear();
    }

    protected static void RemoveOffscreenTargets<T>(
      ICollection<T> targets,
      ICollection<T> working_targets,
      Vector2I vis_min,
      Vector2I vis_max,
      Action<T> on_removed = null,
      Func<T, bool> special_clear_condition = null)
      where T : IUniformGridObject
    {
      OverlayModes.Mode.ClearOutsideViewObjects<T>(targets, working_targets, vis_min, vis_max, (Action<T>) (cmp =>
      {
        if ((object) cmp == null || on_removed == null)
          return;
        on_removed(cmp);
      }));
      if (special_clear_condition == null)
        return;
      working_targets.Clear();
      foreach (T target in (IEnumerable<T>) targets)
      {
        if (special_clear_condition(target))
          working_targets.Add(target);
      }
      foreach (T workingTarget in (IEnumerable<T>) working_targets)
      {
        if ((object) workingTarget != null)
        {
          if (on_removed != null)
            on_removed(workingTarget);
          targets.Remove(workingTarget);
        }
      }
      working_targets.Clear();
    }

    protected static void ClearOutsideViewObjects<T>(
      ICollection<T> targets,
      ICollection<T> working_targets,
      Vector2I vis_min,
      Vector2I vis_max,
      Action<T> on_removed = null)
      where T : IUniformGridObject
    {
      working_targets.Clear();
      foreach (T target in (IEnumerable<T>) targets)
      {
        if ((object) target != null)
        {
          Vector2 vector2_1 = target.PosMin();
          Vector2 vector2_2 = target.PosMin();
          if ((double) vector2_2.x < (double) vis_min.x || (double) vector2_2.y < (double) vis_min.y || (double) vis_max.x < (double) vector2_1.x || (double) vis_max.y < (double) vector2_1.y)
            working_targets.Add(target);
        }
      }
      foreach (T workingTarget in (IEnumerable<T>) working_targets)
      {
        if ((object) workingTarget != null)
        {
          if (on_removed != null)
            on_removed(workingTarget);
          targets.Remove(workingTarget);
        }
      }
      working_targets.Clear();
    }

    protected static float GetDefaultDepth(KMonoBehaviour cmp)
    {
      BuildingComplete component = ((Component) cmp).GetComponent<BuildingComplete>();
      return !Object.op_Inequality((Object) component, (Object) null) ? Grid.GetLayerZ(Grid.SceneLayer.Creatures) : Grid.GetLayerZ(component.Def.SceneLayer);
    }

    protected void UpdateHighlightTypeOverlay<T>(
      Vector2I min,
      Vector2I max,
      ICollection<T> targets,
      ICollection<Tag> item_ids,
      OverlayModes.ColorHighlightCondition[] highlights,
      OverlayModes.BringToFrontLayerSetting bringToFrontSetting,
      int layer)
      where T : KMonoBehaviour
    {
      foreach (T target in (IEnumerable<T>) targets)
      {
        if (!Object.op_Equality((Object) (object) target, (Object) null))
        {
          Vector3 position = TransformExtensions.GetPosition(((KMonoBehaviour) (object) target).transform);
          int cell = Grid.PosToCell(position);
          if (Grid.IsValidCell(cell) && Grid.IsVisible(cell) && Vector2I.op_LessThanOrEqual(min, Vector2.op_Implicit(position)) && Vector2I.op_LessThanOrEqual(Vector2.op_Implicit(position), max))
          {
            KBatchedAnimController component = ((Component) (object) target).GetComponent<KBatchedAnimController>();
            if (!Object.op_Equality((Object) component, (Object) null))
            {
              int layer1 = 0;
              Color32 color32 = Color32.op_Implicit(Color.clear);
              if (highlights != null)
              {
                for (int index = 0; index < highlights.Length; ++index)
                {
                  OverlayModes.ColorHighlightCondition highlight = highlights[index];
                  if (highlight.highlight_condition((KMonoBehaviour) (object) target))
                  {
                    color32 = Color32.op_Implicit(highlight.highlight_color((KMonoBehaviour) (object) target));
                    layer1 = layer;
                    break;
                  }
                }
              }
              switch (bringToFrontSetting)
              {
                case OverlayModes.BringToFrontLayerSetting.Constant:
                  component.SetLayer(layer);
                  break;
                case OverlayModes.BringToFrontLayerSetting.Conditional:
                  component.SetLayer(layer1);
                  break;
              }
              component.HighlightColour = color32;
            }
          }
        }
      }
    }

    protected void DisableHighlightTypeOverlay<T>(ICollection<T> targets) where T : KMonoBehaviour
    {
      Color32 color32 = Color32.op_Implicit(Color.clear);
      foreach (T target in (IEnumerable<T>) targets)
      {
        if (!Object.op_Equality((Object) (object) target, (Object) null))
        {
          KBatchedAnimController component = ((Component) (object) target).GetComponent<KBatchedAnimController>();
          if (Object.op_Inequality((Object) component, (Object) null))
          {
            component.HighlightColour = color32;
            component.SetLayer(0);
          }
        }
      }
      targets.Clear();
    }

    protected void AddTargetIfVisible<T>(
      T instance,
      Vector2I vis_min,
      Vector2I vis_max,
      ICollection<T> targets,
      int layer,
      Action<T> on_added = null,
      Func<KMonoBehaviour, bool> should_add = null)
      where T : IUniformGridObject
    {
      if (instance.Equals((object) null))
        return;
      Vector2 vector2_1 = instance.PosMin();
      Vector2 vector2_2 = instance.PosMax();
      if ((double) vector2_2.x < (double) vis_min.x || (double) vector2_2.y < (double) vis_min.y || (double) vector2_1.x > (double) vis_max.x || (double) vector2_1.y > (double) vis_max.y || targets.Contains(instance))
        return;
      bool flag1 = false;
      for (int y = (int) vector2_1.y; (double) y <= (double) vector2_2.y; ++y)
      {
        for (int x = (int) vector2_1.x; (double) x <= (double) vector2_2.x; ++x)
        {
          int cell = Grid.XYToCell(x, y);
          if (Grid.IsValidCell(cell) && Grid.Visible[cell] > (byte) 20 && (int) Grid.WorldIdx[cell] == ClusterManager.Instance.activeWorldId || !PropertyTextures.IsFogOfWarEnabled)
          {
            flag1 = true;
            break;
          }
        }
      }
      if (!flag1)
        return;
      bool flag2 = true;
      KMonoBehaviour kmonoBehaviour = (object) instance as KMonoBehaviour;
      if (Object.op_Inequality((Object) kmonoBehaviour, (Object) null) && should_add != null)
        flag2 = should_add(kmonoBehaviour);
      if (!flag2)
        return;
      if (Object.op_Inequality((Object) kmonoBehaviour, (Object) null))
      {
        KBatchedAnimController component = ((Component) kmonoBehaviour).GetComponent<KBatchedAnimController>();
        if (Object.op_Inequality((Object) component, (Object) null))
          component.SetLayer(layer);
      }
      targets.Add(instance);
      if (on_added == null)
        return;
      on_added(instance);
    }
  }

  public class ModeUtil
  {
    public static float GetHighlightScale() => Mathf.SmoothStep(0.5f, 1f, Mathf.Abs(Mathf.Sin(Time.unscaledTime * 4f)));
  }

  public class Power : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (Power));
    private int targetLayer;
    private int cameraLayerMask;
    private int selectionMask;
    private List<OverlayModes.Power.UpdatePowerInfo> updatePowerInfo = new List<OverlayModes.Power.UpdatePowerInfo>();
    private List<OverlayModes.Power.UpdateBatteryInfo> updateBatteryInfo = new List<OverlayModes.Power.UpdateBatteryInfo>();
    private Canvas powerLabelParent;
    private LocText powerLabelPrefab;
    private Vector3 powerLabelOffset;
    private BatteryUI batteryUIPrefab;
    private Vector3 batteryUIOffset;
    private Vector3 batteryUITransformerOffset;
    private Vector3 batteryUISmallTransformerOffset;
    private int freePowerLabelIdx;
    private int freeBatteryUIIdx;
    private List<LocText> powerLabels = new List<LocText>();
    private List<BatteryUI> batteryUIList = new List<BatteryUI>();
    private UniformGrid<SaveLoadRoot> partition;
    private List<SaveLoadRoot> queuedAdds = new List<SaveLoadRoot>();
    private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();
    private HashSet<SaveLoadRoot> privateTargets = new HashSet<SaveLoadRoot>();
    private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();
    private List<int> visited = new List<int>();

    public override HashedString ViewMode() => OverlayModes.Power.ID;

    public override string GetSoundName() => nameof (Power);

    public Power(
      Canvas powerLabelParent,
      LocText powerLabelPrefab,
      BatteryUI batteryUIPrefab,
      Vector3 powerLabelOffset,
      Vector3 batteryUIOffset,
      Vector3 batteryUITransformerOffset,
      Vector3 batteryUISmallTransformerOffset)
    {
      this.powerLabelParent = powerLabelParent;
      this.powerLabelPrefab = powerLabelPrefab;
      this.batteryUIPrefab = batteryUIPrefab;
      this.powerLabelOffset = powerLabelOffset;
      this.batteryUIOffset = batteryUIOffset;
      this.batteryUITransformerOffset = batteryUITransformerOffset;
      this.batteryUISmallTransformerOffset = batteryUISmallTransformerOffset;
      this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
      this.cameraLayerMask = LayerMask.GetMask(new string[2]
      {
        "MaskedOverlay",
        "MaskedOverlayBG"
      });
      this.selectionMask = this.cameraLayerMask;
    }

    public override void Enable()
    {
      Camera.main.cullingMask |= this.cameraLayerMask;
      SelectTool.Instance.SetLayerMask(this.selectionMask);
      this.RegisterSaveLoadListeners();
      this.partition = OverlayModes.Mode.PopulatePartition<SaveLoadRoot>((ICollection<Tag>) OverlayScreen.WireIDs);
      GridCompositor.Instance.ToggleMinor(true);
    }

    public override void Disable()
    {
      OverlayModes.Mode.ResetDisplayValues<SaveLoadRoot>((ICollection<SaveLoadRoot>) this.layerTargets);
      Camera.main.cullingMask &= ~this.cameraLayerMask;
      SelectTool.Instance.ClearLayerMask();
      this.UnregisterSaveLoadListeners();
      this.partition.Clear();
      this.layerTargets.Clear();
      this.privateTargets.Clear();
      this.queuedAdds.Clear();
      this.DisablePowerLabels();
      this.DisableBatteryUIs();
      GridCompositor.Instance.ToggleMinor(false);
    }

    protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
    {
      Tag saveLoadTag = ((Component) item).GetComponent<KPrefabID>().GetSaveLoadTag();
      if (!OverlayScreen.WireIDs.Contains(saveLoadTag))
        return;
      this.partition.Add(item);
    }

    protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
    {
      if (Object.op_Equality((Object) item, (Object) null) || Object.op_Equality((Object) ((Component) item).gameObject, (Object) null))
        return;
      if (this.layerTargets.Contains(item))
        this.layerTargets.Remove(item);
      this.partition.Remove(item);
    }

    public override void Update()
    {
      Vector2I min;
      Vector2I max;
      Grid.GetVisibleExtents(out min, out max);
      OverlayModes.Mode.RemoveOffscreenTargets<SaveLoadRoot>((ICollection<SaveLoadRoot>) this.layerTargets, min, max);
      KProfiler.Region region;
      // ISSUE: explicit constructor call
      ((KProfiler.Region) ref region).\u002Ector("UpdatePowerOverlay", (Object) null);
      try
      {
        foreach (SaveLoadRoot instance in this.partition.GetAllIntersecting(new Vector2((float) min.x, (float) min.y), new Vector2((float) max.x, (float) max.y)))
          this.AddTargetIfVisible<SaveLoadRoot>(instance, min, max, (ICollection<SaveLoadRoot>) this.layerTargets, this.targetLayer);
        this.connectedNetworks.Clear();
        float num1 = 1f;
        GameObject gameObject = (GameObject) null;
        if (Object.op_Inequality((Object) SelectTool.Instance, (Object) null) && Object.op_Inequality((Object) SelectTool.Instance.hover, (Object) null))
          gameObject = ((Component) SelectTool.Instance.hover).gameObject;
        if (Object.op_Inequality((Object) gameObject, (Object) null))
        {
          IBridgedNetworkItem component = gameObject.GetComponent<IBridgedNetworkItem>();
          if (component != null)
          {
            int networkCell = component.GetNetworkCell();
            this.visited.Clear();
            this.FindConnectedNetworks(networkCell, (IUtilityNetworkMgr) Game.Instance.electricalConduitSystem, (ICollection<UtilityNetwork>) this.connectedNetworks, this.visited);
            this.visited.Clear();
            num1 = OverlayModes.ModeUtil.GetHighlightScale();
          }
        }
        CircuitManager circuitManager = Game.Instance.circuitManager;
        foreach (SaveLoadRoot layerTarget in this.layerTargets)
        {
          if (!Object.op_Equality((Object) layerTarget, (Object) null))
          {
            IBridgedNetworkItem component1 = ((Component) layerTarget).GetComponent<IBridgedNetworkItem>();
            if (component1 != null)
            {
              KBatchedAnimController component2 = ((Component) (component1 as KMonoBehaviour)).GetComponent<KBatchedAnimController>();
              UtilityNetwork networkForCell = Game.Instance.electricalConduitSystem.GetNetworkForCell(component1.GetNetworkCell());
              ushort circuitID = networkForCell != null ? (ushort) networkForCell.id : ushort.MaxValue;
              float wattsUsedByCircuit = circuitManager.GetWattsUsedByCircuit(circuitID);
              float num2 = circuitManager.GetMaxSafeWattageForCircuit(circuitID) + TUNING.POWER.FLOAT_FUDGE_FACTOR;
              float neededWhenActive = circuitManager.GetWattsNeededWhenActive(circuitID);
              Color32 color32_1 = (double) wattsUsedByCircuit > 0.0 ? ((double) wattsUsedByCircuit <= (double) num2 ? ((double) neededWhenActive <= (double) num2 || (double) num2 <= 0.0 || (double) wattsUsedByCircuit / (double) num2 < 0.75 ? GlobalAssets.Instance.colorSet.powerCircuitSafe : GlobalAssets.Instance.colorSet.powerCircuitStraining) : GlobalAssets.Instance.colorSet.powerCircuitOverloading) : GlobalAssets.Instance.colorSet.powerCircuitUnpowered;
              if (this.connectedNetworks.Count > 0 && component1.IsConnectedToNetworks((ICollection<UtilityNetwork>) this.connectedNetworks))
              {
                color32_1.r = (byte) ((double) color32_1.r * (double) num1);
                color32_1.g = (byte) ((double) color32_1.g * (double) num1);
                color32_1.b = (byte) ((double) color32_1.b * (double) num1);
              }
              Color32 color32_2 = color32_1;
              component2.TintColour = color32_2;
            }
          }
        }
      }
      finally
      {
        region.Dispose();
      }
      this.queuedAdds.Clear();
      // ISSUE: explicit constructor call
      ((KProfiler.Region) ref region).\u002Ector("BatteryUI", (Object) null);
      try
      {
        foreach (Battery battery in Components.Batteries.Items)
        {
          Vector2I xy = Grid.PosToXY(TransformExtensions.GetPosition(battery.transform));
          if (Vector2I.op_LessThanOrEqual(min, xy) && Vector2I.op_LessThanOrEqual(xy, max) && battery.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
          {
            SaveLoadRoot component = ((Component) battery).GetComponent<SaveLoadRoot>();
            if (!this.privateTargets.Contains(component))
            {
              this.AddBatteryUI(battery);
              this.queuedAdds.Add(component);
            }
          }
        }
        foreach (Generator component3 in Components.Generators.Items)
        {
          Vector2I xy = Grid.PosToXY(TransformExtensions.GetPosition(component3.transform));
          if (Vector2I.op_LessThanOrEqual(min, xy) && Vector2I.op_LessThanOrEqual(xy, max) && component3.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
          {
            SaveLoadRoot component4 = ((Component) component3).GetComponent<SaveLoadRoot>();
            if (!this.privateTargets.Contains(component4))
            {
              this.privateTargets.Add(component4);
              if (Object.op_Equality((Object) ((Component) component3).GetComponent<PowerTransformer>(), (Object) null))
                this.AddPowerLabels((KMonoBehaviour) component3);
            }
          }
        }
        foreach (EnergyConsumer component5 in Components.EnergyConsumers.Items)
        {
          Vector2I xy = Grid.PosToXY(TransformExtensions.GetPosition(component5.transform));
          if (Vector2I.op_LessThanOrEqual(min, xy) && Vector2I.op_LessThanOrEqual(xy, max) && component5.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
          {
            SaveLoadRoot component6 = ((Component) component5).GetComponent<SaveLoadRoot>();
            if (!this.privateTargets.Contains(component6))
            {
              this.privateTargets.Add(component6);
              this.AddPowerLabels((KMonoBehaviour) component5);
            }
          }
        }
      }
      finally
      {
        region.Dispose();
      }
      foreach (SaveLoadRoot queuedAdd in this.queuedAdds)
        this.privateTargets.Add(queuedAdd);
      this.queuedAdds.Clear();
      this.UpdatePowerLabels();
    }

    private LocText GetFreePowerLabel()
    {
      LocText freePowerLabel;
      if (this.freePowerLabelIdx < this.powerLabels.Count)
      {
        freePowerLabel = this.powerLabels[this.freePowerLabelIdx];
        ++this.freePowerLabelIdx;
      }
      else
      {
        freePowerLabel = Util.KInstantiateUI<LocText>(((Component) this.powerLabelPrefab).gameObject, ((Component) ((Component) this.powerLabelParent).transform).gameObject, false);
        this.powerLabels.Add(freePowerLabel);
        ++this.freePowerLabelIdx;
      }
      return freePowerLabel;
    }

    private void UpdatePowerLabels()
    {
      foreach (OverlayModes.Power.UpdatePowerInfo updatePowerInfo in this.updatePowerInfo)
      {
        KMonoBehaviour kmonoBehaviour = updatePowerInfo.item;
        LocText powerLabel = updatePowerInfo.powerLabel;
        LocText unitLabel = updatePowerInfo.unitLabel;
        Generator generator = updatePowerInfo.generator;
        IEnergyConsumer consumer = updatePowerInfo.consumer;
        if (Object.op_Equality((Object) updatePowerInfo.item, (Object) null) || ((Component) updatePowerInfo.item).gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
        {
          ((Component) powerLabel).gameObject.SetActive(false);
        }
        else
        {
          ((Component) powerLabel).gameObject.SetActive(true);
          if (Object.op_Inequality((Object) generator, (Object) null) && consumer == null)
          {
            int num = int.MaxValue;
            if (Object.op_Equality((Object) ((Component) generator).GetComponent<ManualGenerator>(), (Object) null))
            {
              ((Component) generator).GetComponent<Operational>();
              num = Mathf.Max(0, Mathf.RoundToInt(generator.WattageRating));
            }
            else
              num = Mathf.Max(0, Mathf.RoundToInt(generator.WattageRating));
            ((TMP_Text) powerLabel).text = num != 0 ? "+" + num.ToString() : num.ToString();
            BuildingEnabledButton component1 = ((Component) kmonoBehaviour).GetComponent<BuildingEnabledButton>();
            Color color = Color32.op_Implicit(!Object.op_Inequality((Object) component1, (Object) null) || component1.IsEnabled ? GlobalAssets.Instance.colorSet.powerGenerator : GlobalAssets.Instance.colorSet.powerBuildingDisabled);
            ((Graphic) powerLabel).color = color;
            ((Graphic) unitLabel).color = color;
            BuildingCellVisualizer component2 = ((Component) generator).GetComponent<BuildingCellVisualizer>();
            if (Object.op_Inequality((Object) component2, (Object) null))
            {
              Image outputIcon = component2.GetOutputIcon();
              if (Object.op_Inequality((Object) outputIcon, (Object) null))
                ((Graphic) outputIcon).color = color;
            }
          }
          if (consumer != null)
          {
            BuildingEnabledButton component = ((Component) kmonoBehaviour).GetComponent<BuildingEnabledButton>();
            Color color = Color32.op_Implicit(!Object.op_Inequality((Object) component, (Object) null) || component.IsEnabled ? GlobalAssets.Instance.colorSet.powerConsumer : GlobalAssets.Instance.colorSet.powerBuildingDisabled);
            int num = Mathf.Max(0, Mathf.RoundToInt(consumer.WattsNeededWhenActive));
            string str = num.ToString();
            ((TMP_Text) powerLabel).text = num != 0 ? "-" + str : str;
            ((Graphic) powerLabel).color = color;
            ((Graphic) unitLabel).color = color;
            Image inputIcon = ((Component) kmonoBehaviour).GetComponentInChildren<BuildingCellVisualizer>().GetInputIcon();
            if (Object.op_Inequality((Object) inputIcon, (Object) null))
              ((Graphic) inputIcon).color = color;
          }
        }
      }
      foreach (OverlayModes.Power.UpdateBatteryInfo updateBatteryInfo in this.updateBatteryInfo)
        updateBatteryInfo.ui.SetContent(updateBatteryInfo.battery);
    }

    private void AddPowerLabels(KMonoBehaviour item)
    {
      if (((Component) item).gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
        return;
      IEnergyConsumer componentInChildren1 = ((Component) item).gameObject.GetComponentInChildren<IEnergyConsumer>();
      Generator componentInChildren2 = ((Component) item).gameObject.GetComponentInChildren<Generator>();
      if (componentInChildren1 == null && !Object.op_Inequality((Object) componentInChildren2, (Object) null))
        return;
      float num = -10f;
      if (Object.op_Inequality((Object) componentInChildren2, (Object) null))
      {
        LocText freePowerLabel = this.GetFreePowerLabel();
        ((Component) freePowerLabel).gameObject.SetActive(true);
        ((Object) ((Component) freePowerLabel).gameObject).name = ((Object) ((Component) item).gameObject).name + "power label";
        LocText component = ((Component) ((TMP_Text) freePowerLabel).transform.GetChild(0)).GetComponent<LocText>();
        ((Component) component).gameObject.SetActive(true);
        ((Behaviour) freePowerLabel).enabled = true;
        ((Behaviour) component).enabled = true;
        Vector3 pos = Grid.CellToPos(componentInChildren2.PowerCell, 0.5f, 0.0f, 0.0f);
        TransformExtensions.SetPosition((Transform) ((TMP_Text) freePowerLabel).rectTransform, Vector3.op_Addition(Vector3.op_Addition(pos, this.powerLabelOffset), Vector3.op_Multiply(Vector3.up, num * 0.02f)));
        if (componentInChildren1 != null && componentInChildren1.PowerCell == componentInChildren2.PowerCell)
          num -= 15f;
        this.SetToolTip(freePowerLabel, (string) STRINGS.UI.OVERLAYS.POWER.WATTS_GENERATED);
        this.updatePowerInfo.Add(new OverlayModes.Power.UpdatePowerInfo(item, freePowerLabel, component, componentInChildren2, (IEnergyConsumer) null));
      }
      if (componentInChildren1 == null || !(componentInChildren1.GetType() != typeof (Battery)))
        return;
      LocText freePowerLabel1 = this.GetFreePowerLabel();
      LocText component1 = ((Component) ((TMP_Text) freePowerLabel1).transform.GetChild(0)).GetComponent<LocText>();
      ((Component) freePowerLabel1).gameObject.SetActive(true);
      ((Component) component1).gameObject.SetActive(true);
      ((Object) ((Component) freePowerLabel1).gameObject).name = ((Object) ((Component) item).gameObject).name + "power label";
      ((Behaviour) freePowerLabel1).enabled = true;
      ((Behaviour) component1).enabled = true;
      Vector3 pos1 = Grid.CellToPos(componentInChildren1.PowerCell, 0.5f, 0.0f, 0.0f);
      TransformExtensions.SetPosition((Transform) ((TMP_Text) freePowerLabel1).rectTransform, Vector3.op_Addition(Vector3.op_Addition(pos1, this.powerLabelOffset), Vector3.op_Multiply(Vector3.up, num * 0.02f)));
      this.SetToolTip(freePowerLabel1, (string) STRINGS.UI.OVERLAYS.POWER.WATTS_CONSUMED);
      this.updatePowerInfo.Add(new OverlayModes.Power.UpdatePowerInfo(item, freePowerLabel1, component1, (Generator) null, componentInChildren1));
    }

    private void DisablePowerLabels()
    {
      this.freePowerLabelIdx = 0;
      foreach (Component powerLabel in this.powerLabels)
        powerLabel.gameObject.SetActive(false);
      this.updatePowerInfo.Clear();
    }

    private void AddBatteryUI(Battery bat)
    {
      BatteryUI freeBatteryUi = this.GetFreeBatteryUI();
      freeBatteryUi.SetContent(bat);
      Vector3 pos = Grid.CellToPos(bat.PowerCell, 0.5f, 0.0f, 0.0f);
      int num1 = Object.op_Inequality((Object) bat.powerTransformer, (Object) null) ? 1 : 0;
      float num2 = 1f;
      Rotatable component = ((Component) bat).GetComponent<Rotatable>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.GetVisualizerFlipX())
        num2 = -1f;
      Vector3 vector3 = this.batteryUIOffset;
      if (num1 != 0)
        vector3 = ((Component) bat).GetComponent<Building>().Def.WidthInCells == 2 ? this.batteryUISmallTransformerOffset : this.batteryUITransformerOffset;
      vector3.x *= num2;
      TransformExtensions.SetPosition((Transform) ((Component) freeBatteryUi).GetComponent<RectTransform>(), Vector3.op_Addition(Vector3.op_Addition(Vector3.up, pos), vector3));
      this.updateBatteryInfo.Add(new OverlayModes.Power.UpdateBatteryInfo(bat, freeBatteryUi));
    }

    private void SetToolTip(LocText label, string text)
    {
      ToolTip component = ((Component) label).GetComponent<ToolTip>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.toolTip = text;
    }

    private void DisableBatteryUIs()
    {
      this.freeBatteryUIIdx = 0;
      foreach (Component batteryUi in this.batteryUIList)
        batteryUi.gameObject.SetActive(false);
      this.updateBatteryInfo.Clear();
    }

    private BatteryUI GetFreeBatteryUI()
    {
      BatteryUI freeBatteryUi;
      if (this.freeBatteryUIIdx < this.batteryUIList.Count)
      {
        freeBatteryUi = this.batteryUIList[this.freeBatteryUIIdx];
        ((Component) freeBatteryUi).gameObject.SetActive(true);
        ++this.freeBatteryUIIdx;
      }
      else
      {
        freeBatteryUi = Util.KInstantiateUI<BatteryUI>(((Component) this.batteryUIPrefab).gameObject, ((Component) ((Component) this.powerLabelParent).transform).gameObject, false);
        this.batteryUIList.Add(freeBatteryUi);
        ++this.freeBatteryUIIdx;
      }
      return freeBatteryUi;
    }

    private void FindConnectedNetworks(
      int cell,
      IUtilityNetworkMgr mgr,
      ICollection<UtilityNetwork> networks,
      List<int> visited)
    {
      if (visited.Contains(cell))
        return;
      visited.Add(cell);
      UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
      if (networkForCell == null)
        return;
      networks.Add(networkForCell);
      int connections = (int) mgr.GetConnections(cell, false);
      if ((connections & 2) != 0)
        this.FindConnectedNetworks(Grid.CellRight(cell), mgr, networks, visited);
      if ((connections & 1) != 0)
        this.FindConnectedNetworks(Grid.CellLeft(cell), mgr, networks, visited);
      if ((connections & 4) != 0)
        this.FindConnectedNetworks(Grid.CellAbove(cell), mgr, networks, visited);
      if ((connections & 8) == 0)
        return;
      this.FindConnectedNetworks(Grid.CellBelow(cell), mgr, networks, visited);
    }

    private struct UpdatePowerInfo
    {
      public KMonoBehaviour item;
      public LocText powerLabel;
      public LocText unitLabel;
      public Generator generator;
      public IEnergyConsumer consumer;

      public UpdatePowerInfo(
        KMonoBehaviour item,
        LocText power_label,
        LocText unit_label,
        Generator g,
        IEnergyConsumer c)
      {
        this.item = item;
        this.powerLabel = power_label;
        this.unitLabel = unit_label;
        this.generator = g;
        this.consumer = c;
      }
    }

    private struct UpdateBatteryInfo
    {
      public Battery battery;
      public BatteryUI ui;

      public UpdateBatteryInfo(Battery battery, BatteryUI ui)
      {
        this.battery = battery;
        this.ui = ui;
      }
    }
  }

  public class Radiation : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (Radiation));

    public override HashedString ViewMode() => OverlayModes.Radiation.ID;

    public override string GetSoundName() => nameof (Radiation);

    public override void Enable() => AudioMixer.instance.Start(AudioMixerSnapshots.Get().TechFilterRadiationOn);

    public override void Disable() => AudioMixer.instance.Stop(AudioMixerSnapshots.Get().TechFilterRadiationOn);
  }

  public class SolidConveyor : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (SolidConveyor));
    private UniformGrid<SaveLoadRoot> partition;
    private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();
    private ICollection<Tag> targetIDs = (ICollection<Tag>) OverlayScreen.SolidConveyorIDs;
    private Color32 tint_color = new Color32((byte) 201, (byte) 201, (byte) 201, (byte) 0);
    private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();
    private List<int> visited = new List<int>();
    private int targetLayer;
    private int cameraLayerMask;
    private int selectionMask;

    public override HashedString ViewMode() => OverlayModes.SolidConveyor.ID;

    public override string GetSoundName() => "LiquidVent";

    public SolidConveyor()
    {
      this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
      this.cameraLayerMask = LayerMask.GetMask(new string[2]
      {
        "MaskedOverlay",
        "MaskedOverlayBG"
      });
      this.selectionMask = this.cameraLayerMask;
    }

    public override void Enable()
    {
      this.RegisterSaveLoadListeners();
      this.partition = OverlayModes.Mode.PopulatePartition<SaveLoadRoot>(this.targetIDs);
      Camera.main.cullingMask |= this.cameraLayerMask;
      SelectTool.Instance.SetLayerMask(this.selectionMask);
      GridCompositor.Instance.ToggleMinor(false);
      base.Enable();
    }

    protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
    {
      if (!this.targetIDs.Contains(((Component) item).GetComponent<KPrefabID>().GetSaveLoadTag()))
        return;
      this.partition.Add(item);
    }

    protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
    {
      if (Object.op_Equality((Object) item, (Object) null) || Object.op_Equality((Object) ((Component) item).gameObject, (Object) null))
        return;
      if (this.layerTargets.Contains(item))
        this.layerTargets.Remove(item);
      this.partition.Remove(item);
    }

    public override void Disable()
    {
      OverlayModes.Mode.ResetDisplayValues<SaveLoadRoot>((ICollection<SaveLoadRoot>) this.layerTargets);
      Camera.main.cullingMask &= ~this.cameraLayerMask;
      SelectTool.Instance.ClearLayerMask();
      this.UnregisterSaveLoadListeners();
      this.partition.Clear();
      this.layerTargets.Clear();
      GridCompositor.Instance.ToggleMinor(false);
      base.Disable();
    }

    public override void Update()
    {
      Vector2I min;
      Vector2I max;
      Grid.GetVisibleExtents(out min, out max);
      OverlayModes.Mode.RemoveOffscreenTargets<SaveLoadRoot>((ICollection<SaveLoadRoot>) this.layerTargets, min, max);
      foreach (SaveLoadRoot instance in this.partition.GetAllIntersecting(new Vector2((float) min.x, (float) min.y), new Vector2((float) max.x, (float) max.y)))
        this.AddTargetIfVisible<SaveLoadRoot>(instance, min, max, (ICollection<SaveLoadRoot>) this.layerTargets, this.targetLayer);
      GameObject gameObject = (GameObject) null;
      if (Object.op_Inequality((Object) SelectTool.Instance, (Object) null) && Object.op_Inequality((Object) SelectTool.Instance.hover, (Object) null))
        gameObject = ((Component) SelectTool.Instance.hover).gameObject;
      this.connectedNetworks.Clear();
      float num = 1f;
      if (Object.op_Inequality((Object) gameObject, (Object) null))
      {
        SolidConduit component = gameObject.GetComponent<SolidConduit>();
        if (Object.op_Inequality((Object) component, (Object) null))
        {
          int cell = Grid.PosToCell((KMonoBehaviour) component);
          UtilityNetworkManager<FlowUtilityNetwork, SolidConduit> solidConduitSystem = Game.Instance.solidConduitSystem;
          this.visited.Clear();
          this.FindConnectedNetworks(cell, (IUtilityNetworkMgr) solidConduitSystem, (ICollection<UtilityNetwork>) this.connectedNetworks, this.visited);
          this.visited.Clear();
          num = OverlayModes.ModeUtil.GetHighlightScale();
        }
      }
      foreach (SaveLoadRoot layerTarget in this.layerTargets)
      {
        if (!Object.op_Equality((Object) layerTarget, (Object) null))
        {
          Color32 tintColor = this.tint_color;
          SolidConduit component = ((Component) layerTarget).GetComponent<SolidConduit>();
          if (Object.op_Inequality((Object) component, (Object) null))
          {
            if (this.connectedNetworks.Count > 0 && this.IsConnectedToNetworks(component, (ICollection<UtilityNetwork>) this.connectedNetworks))
            {
              tintColor.r = (byte) ((double) tintColor.r * (double) num);
              tintColor.g = (byte) ((double) tintColor.g * (double) num);
              tintColor.b = (byte) ((double) tintColor.b * (double) num);
            }
            ((Component) layerTarget).GetComponent<KBatchedAnimController>().TintColour = tintColor;
          }
        }
      }
    }

    public bool IsConnectedToNetworks(SolidConduit conduit, ICollection<UtilityNetwork> networks)
    {
      UtilityNetwork network = conduit.GetNetwork();
      return networks.Contains(network);
    }

    private void FindConnectedNetworks(
      int cell,
      IUtilityNetworkMgr mgr,
      ICollection<UtilityNetwork> networks,
      List<int> visited)
    {
      if (visited.Contains(cell))
        return;
      visited.Add(cell);
      UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
      if (networkForCell == null)
        return;
      networks.Add(networkForCell);
      int connections = (int) mgr.GetConnections(cell, false);
      if ((connections & 2) != 0)
        this.FindConnectedNetworks(Grid.CellRight(cell), mgr, networks, visited);
      if ((connections & 1) != 0)
        this.FindConnectedNetworks(Grid.CellLeft(cell), mgr, networks, visited);
      if ((connections & 4) != 0)
        this.FindConnectedNetworks(Grid.CellAbove(cell), mgr, networks, visited);
      if ((connections & 8) != 0)
        this.FindConnectedNetworks(Grid.CellBelow(cell), mgr, networks, visited);
      object endpoint = mgr.GetEndpoint(cell);
      if (endpoint == null || !(endpoint is FlowUtilityNetwork.NetworkItem networkItem))
        return;
      GameObject gameObject = networkItem.GameObject;
      if (!Object.op_Inequality((Object) gameObject, (Object) null))
        return;
      gameObject.GetComponent<IBridgedNetworkItem>()?.AddNetworks(networks);
    }
  }

  public class Sound : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (Sound));
    private UniformGrid<NoisePolluter> partition;
    private HashSet<NoisePolluter> layerTargets = new HashSet<NoisePolluter>();
    private HashSet<Tag> targetIDs = new HashSet<Tag>();
    private int targetLayer;
    private int cameraLayerMask;
    private OverlayModes.ColorHighlightCondition[] highlightConditions = new OverlayModes.ColorHighlightCondition[1]
    {
      new OverlayModes.ColorHighlightCondition((Func<KMonoBehaviour, Color>) (np =>
      {
        Color black1 = Color.black;
        Color black2 = Color.black;
        float num1 = 0.8f;
        if (Object.op_Inequality((Object) np, (Object) null))
        {
          int cell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
          if ((double) (np as NoisePolluter).GetNoiseForCell(cell) < 36.0)
          {
            num1 = 1f;
            ((Color) ref black2).\u002Ector(0.4f, 0.4f, 0.4f);
          }
        }
        Color color = black2;
        double num2 = (double) num1;
        return Color.Lerp(black1, color, (float) num2);
      }), (Func<KMonoBehaviour, bool>) (np =>
      {
        List<GameObject> highlightedObjects = SelectToolHoverTextCard.highlightedObjects;
        bool flag = false;
        for (int index = 0; index < highlightedObjects.Count; ++index)
        {
          if (Object.op_Inequality((Object) highlightedObjects[index], (Object) null) && Object.op_Equality((Object) highlightedObjects[index], (Object) ((Component) np).gameObject))
          {
            flag = true;
            break;
          }
        }
        return flag;
      }))
    };

    public override HashedString ViewMode() => OverlayModes.Sound.ID;

    public override string GetSoundName() => nameof (Sound);

    public Sound()
    {
      this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
      this.cameraLayerMask = LayerMask.GetMask(new string[2]
      {
        "MaskedOverlay",
        "MaskedOverlayBG"
      });
      this.targetIDs.UnionWith((IEnumerable<Tag>) Assets.GetPrefabTagsWithComponent<NoisePolluter>());
    }

    public override void Enable()
    {
      this.RegisterSaveLoadListeners();
      this.targetIDs.UnionWith((IEnumerable<Tag>) Assets.GetPrefabTagsWithComponent<NoisePolluter>());
      this.partition = OverlayModes.Mode.PopulatePartition<NoisePolluter>((ICollection<Tag>) this.targetIDs);
      Camera.main.cullingMask |= this.cameraLayerMask;
    }

    public override void Update()
    {
      Vector2I min;
      Vector2I max;
      Grid.GetVisibleExtents(out min, out max);
      OverlayModes.Mode.RemoveOffscreenTargets<NoisePolluter>((ICollection<NoisePolluter>) this.layerTargets, min, max);
      foreach (NoisePolluter instance in this.partition.GetAllIntersecting(new Vector2((float) min.x, (float) min.y), new Vector2((float) max.x, (float) max.y)))
        this.AddTargetIfVisible<NoisePolluter>(instance, min, max, (ICollection<NoisePolluter>) this.layerTargets, this.targetLayer);
      this.UpdateHighlightTypeOverlay<NoisePolluter>(min, max, (ICollection<NoisePolluter>) this.layerTargets, (ICollection<Tag>) this.targetIDs, this.highlightConditions, OverlayModes.BringToFrontLayerSetting.Conditional, this.targetLayer);
    }

    protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
    {
      if (!this.targetIDs.Contains(((Component) item).GetComponent<KPrefabID>().GetSaveLoadTag()))
        return;
      this.partition.Add(((Component) item).GetComponent<NoisePolluter>());
    }

    protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
    {
      if (Object.op_Equality((Object) item, (Object) null) || Object.op_Equality((Object) ((Component) item).gameObject, (Object) null))
        return;
      NoisePolluter component = ((Component) item).GetComponent<NoisePolluter>();
      if (this.layerTargets.Contains(component))
        this.layerTargets.Remove(component);
      this.partition.Remove(component);
    }

    public override void Disable()
    {
      this.DisableHighlightTypeOverlay<NoisePolluter>((ICollection<NoisePolluter>) this.layerTargets);
      Camera.main.cullingMask &= ~this.cameraLayerMask;
      this.UnregisterSaveLoadListeners();
      this.partition.Clear();
      this.layerTargets.Clear();
    }
  }

  public class Suit : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (Suit));
    private UniformGrid<SaveLoadRoot> partition;
    private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();
    private ICollection<Tag> targetIDs;
    private List<GameObject> uiList = new List<GameObject>();
    private int freeUiIdx;
    private int targetLayer;
    private int cameraLayerMask;
    private int selectionMask;
    private Canvas uiParent;
    private GameObject overlayPrefab;

    public override HashedString ViewMode() => OverlayModes.Suit.ID;

    public override string GetSoundName() => "SuitRequired";

    public Suit(Canvas ui_parent, GameObject overlay_prefab)
    {
      this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
      this.cameraLayerMask = LayerMask.GetMask(new string[2]
      {
        "MaskedOverlay",
        "MaskedOverlayBG"
      });
      this.selectionMask = this.cameraLayerMask;
      this.targetIDs = (ICollection<Tag>) OverlayScreen.SuitIDs;
      this.uiParent = ui_parent;
      this.overlayPrefab = overlay_prefab;
    }

    public override void Enable()
    {
      this.partition = new UniformGrid<SaveLoadRoot>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
      this.ProcessExistingSaveLoadRoots();
      this.RegisterSaveLoadListeners();
      Camera.main.cullingMask |= this.cameraLayerMask;
      SelectTool.Instance.SetLayerMask(this.selectionMask);
      GridCompositor.Instance.ToggleMinor(false);
      base.Enable();
    }

    public override void Disable()
    {
      this.UnregisterSaveLoadListeners();
      OverlayModes.Mode.ResetDisplayValues<SaveLoadRoot>((ICollection<SaveLoadRoot>) this.layerTargets);
      Camera.main.cullingMask &= ~this.cameraLayerMask;
      SelectTool.Instance.ClearLayerMask();
      this.partition.Clear();
      this.partition = (UniformGrid<SaveLoadRoot>) null;
      this.layerTargets.Clear();
      for (int index = 0; index < this.uiList.Count; ++index)
        this.uiList[index].SetActive(false);
      GridCompositor.Instance.ToggleMinor(false);
      base.Disable();
    }

    protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
    {
      if (!this.targetIDs.Contains(((Component) item).GetComponent<KPrefabID>().GetSaveLoadTag()))
        return;
      this.partition.Add(item);
    }

    protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
    {
      if (Object.op_Equality((Object) item, (Object) null) || Object.op_Equality((Object) ((Component) item).gameObject, (Object) null))
        return;
      if (this.layerTargets.Contains(item))
        this.layerTargets.Remove(item);
      this.partition.Remove(item);
    }

    private GameObject GetFreeUI()
    {
      GameObject freeUi;
      if (this.freeUiIdx >= this.uiList.Count)
      {
        freeUi = Util.KInstantiateUI(this.overlayPrefab, ((Component) ((Component) this.uiParent).transform).gameObject, false);
        this.uiList.Add(freeUi);
      }
      else
        freeUi = this.uiList[this.freeUiIdx++];
      if (!freeUi.activeSelf)
        freeUi.SetActive(true);
      return freeUi;
    }

    public override void Update()
    {
      this.freeUiIdx = 0;
      Vector2I min;
      Vector2I max;
      Grid.GetVisibleExtents(out min, out max);
      OverlayModes.Mode.RemoveOffscreenTargets<SaveLoadRoot>((ICollection<SaveLoadRoot>) this.layerTargets, min, max);
      foreach (SaveLoadRoot instance in this.partition.GetAllIntersecting(new Vector2((float) min.x, (float) min.y), new Vector2((float) max.x, (float) max.y)))
        this.AddTargetIfVisible<SaveLoadRoot>(instance, min, max, (ICollection<SaveLoadRoot>) this.layerTargets, this.targetLayer);
      foreach (SaveLoadRoot layerTarget in this.layerTargets)
      {
        if (!Object.op_Equality((Object) layerTarget, (Object) null))
        {
          ((Component) layerTarget).GetComponent<KBatchedAnimController>().TintColour = Color32.op_Implicit(Color.white);
          bool flag = false;
          if (((Component) layerTarget).GetComponent<KPrefabID>().HasTag(GameTags.Suit))
          {
            flag = true;
          }
          else
          {
            SuitLocker component = ((Component) layerTarget).GetComponent<SuitLocker>();
            if (Object.op_Inequality((Object) component, (Object) null))
              flag = Object.op_Inequality((Object) component.GetStoredOutfit(), (Object) null);
          }
          if (flag)
            TransformExtensions.SetPosition((Transform) this.GetFreeUI().GetComponent<RectTransform>(), TransformExtensions.GetPosition(layerTarget.transform));
        }
      }
      for (int freeUiIdx = this.freeUiIdx; freeUiIdx < this.uiList.Count; ++freeUiIdx)
      {
        if (this.uiList[freeUiIdx].activeSelf)
          this.uiList[freeUiIdx].SetActive(false);
      }
    }
  }

  public class Temperature : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (Temperature));
    public List<LegendEntry> temperatureLegend = new List<LegendEntry>()
    {
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.MAXHOT, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.8901961f, 0.137254909f, 0.129411772f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.EXTREMEHOT, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.9843137f, 0.3254902f, 0.3137255f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.VERYHOT, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(1f, 0.6627451f, 0.141176477f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.HOT, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.9372549f, 1f, 0.0f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.TEMPERATE, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.23137255f, 0.996078432f, 0.2901961f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.COLD, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.121568628f, 0.6313726f, 1f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.VERYCOLD, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.168627456f, 0.796078444f, 1f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.EXTREMECOLD, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.5019608f, 0.996078432f, 0.9411765f))
    };
    public List<LegendEntry> heatFlowLegend = new List<LegendEntry>()
    {
      new LegendEntry((string) STRINGS.UI.OVERLAYS.HEATFLOW.HEATING, (string) STRINGS.UI.OVERLAYS.HEATFLOW.TOOLTIPS.HEATING, new Color(0.9098039f, 0.258823544f, 0.149019614f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.HEATFLOW.NEUTRAL, (string) STRINGS.UI.OVERLAYS.HEATFLOW.TOOLTIPS.NEUTRAL, new Color(0.309803933f, 0.309803933f, 0.309803933f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.HEATFLOW.COOLING, (string) STRINGS.UI.OVERLAYS.HEATFLOW.TOOLTIPS.COOLING, new Color(0.2509804f, 0.6313726f, 0.905882359f))
    };
    public List<LegendEntry> expandedTemperatureLegend = new List<LegendEntry>()
    {
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.MAXHOT, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.8901961f, 0.137254909f, 0.129411772f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.EXTREMEHOT, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.9843137f, 0.3254902f, 0.3137255f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.VERYHOT, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(1f, 0.6627451f, 0.141176477f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.HOT, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.9372549f, 1f, 0.0f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.TEMPERATE, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.23137255f, 0.996078432f, 0.2901961f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.COLD, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.121568628f, 0.6313726f, 1f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.VERYCOLD, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.168627456f, 0.796078444f, 1f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.TEMPERATURE.EXTREMECOLD, (string) STRINGS.UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(0.5019608f, 0.996078432f, 0.9411765f))
    };
    public List<LegendEntry> stateChangeLegend = new List<LegendEntry>()
    {
      new LegendEntry((string) STRINGS.UI.OVERLAYS.STATECHANGE.HIGHPOINT, (string) STRINGS.UI.OVERLAYS.STATECHANGE.TOOLTIPS.HIGHPOINT, new Color(0.8901961f, 0.137254909f, 0.129411772f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.STATECHANGE.STABLE, (string) STRINGS.UI.OVERLAYS.STATECHANGE.TOOLTIPS.STABLE, new Color(0.23137255f, 0.996078432f, 0.2901961f)),
      new LegendEntry((string) STRINGS.UI.OVERLAYS.STATECHANGE.LOWPOINT, (string) STRINGS.UI.OVERLAYS.STATECHANGE.TOOLTIPS.LOWPOINT, new Color(0.5019608f, 0.996078432f, 0.9411765f))
    };

    public override HashedString ViewMode() => OverlayModes.Temperature.ID;

    public override string GetSoundName() => nameof (Temperature);

    public Temperature() => this.legendFilters = this.CreateDefaultFilters();

    public override void Enable()
    {
      base.Enable();
      int num = SimDebugView.Instance.temperatureThresholds.Length - 1;
      for (int index = 0; index < this.temperatureLegend.Count; ++index)
      {
        this.temperatureLegend[index].colour = Color32.op_Implicit(GlobalAssets.Instance.colorSet.GetColorByName(SimDebugView.Instance.temperatureThresholds[num - index].colorName));
        this.temperatureLegend[index].desc_arg = GameUtil.GetFormattedTemperature(SimDebugView.Instance.temperatureThresholds[num - index].value);
      }
    }

    public override Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters() => new Dictionary<string, ToolParameterMenu.ToggleState>()
    {
      {
        ToolParameterMenu.FILTERLAYERS.ABSOLUTETEMPERATURE,
        ToolParameterMenu.ToggleState.On
      },
      {
        ToolParameterMenu.FILTERLAYERS.HEATFLOW,
        ToolParameterMenu.ToggleState.Off
      },
      {
        ToolParameterMenu.FILTERLAYERS.STATECHANGE,
        ToolParameterMenu.ToggleState.Off
      }
    };

    public override List<LegendEntry> GetCustomLegendData()
    {
      switch (Game.Instance.temperatureOverlayMode)
      {
        case Game.TemperatureOverlayModes.AbsoluteTemperature:
          return this.temperatureLegend;
        case Game.TemperatureOverlayModes.AdaptiveTemperature:
          return this.expandedTemperatureLegend;
        case Game.TemperatureOverlayModes.HeatFlow:
          return this.heatFlowLegend;
        case Game.TemperatureOverlayModes.StateChange:
          return this.stateChangeLegend;
        default:
          return this.temperatureLegend;
      }
    }

    public override void OnFiltersChanged()
    {
      if (this.InFilter(ToolParameterMenu.FILTERLAYERS.HEATFLOW, this.legendFilters))
        Game.Instance.temperatureOverlayMode = Game.TemperatureOverlayModes.HeatFlow;
      if (this.InFilter(ToolParameterMenu.FILTERLAYERS.ABSOLUTETEMPERATURE, this.legendFilters))
        Game.Instance.temperatureOverlayMode = Game.TemperatureOverlayModes.AbsoluteTemperature;
      if (this.InFilter(ToolParameterMenu.FILTERLAYERS.ADAPTIVETEMPERATURE, this.legendFilters))
        Game.Instance.temperatureOverlayMode = Game.TemperatureOverlayModes.AdaptiveTemperature;
      if (this.InFilter(ToolParameterMenu.FILTERLAYERS.STATECHANGE, this.legendFilters))
        Game.Instance.temperatureOverlayMode = Game.TemperatureOverlayModes.StateChange;
      switch (Game.Instance.temperatureOverlayMode)
      {
        case Game.TemperatureOverlayModes.AbsoluteTemperature:
          Infrared.Instance.SetMode(Infrared.Mode.Infrared);
          CameraController.Instance.ToggleColouredOverlayView(true);
          break;
        case Game.TemperatureOverlayModes.AdaptiveTemperature:
          Infrared.Instance.SetMode(Infrared.Mode.Infrared);
          CameraController.Instance.ToggleColouredOverlayView(true);
          break;
        case Game.TemperatureOverlayModes.HeatFlow:
          Infrared.Instance.SetMode(Infrared.Mode.Disabled);
          CameraController.Instance.ToggleColouredOverlayView(false);
          break;
        case Game.TemperatureOverlayModes.StateChange:
          Infrared.Instance.SetMode(Infrared.Mode.Disabled);
          CameraController.Instance.ToggleColouredOverlayView(false);
          break;
      }
    }

    public override void Disable()
    {
      Infrared.Instance.SetMode(Infrared.Mode.Disabled);
      CameraController.Instance.ToggleColouredOverlayView(false);
      base.Disable();
    }
  }

  public class TileMode : OverlayModes.Mode
  {
    public static readonly HashedString ID = HashedString.op_Implicit(nameof (TileMode));
    private HashSet<PrimaryElement> layerTargets = new HashSet<PrimaryElement>();
    private HashSet<Tag> targetIDs = new HashSet<Tag>();
    private int targetLayer;
    private int cameraLayerMask;
    private OverlayModes.ColorHighlightCondition[] highlightConditions = new OverlayModes.ColorHighlightCondition[1]
    {
      new OverlayModes.ColorHighlightCondition((Func<KMonoBehaviour, Color>) (primary_element =>
      {
        Color color = Color.black;
        if (Object.op_Inequality((Object) primary_element, (Object) null))
          color = Color32.op_Implicit((primary_element as PrimaryElement).Element.substance.uiColour);
        return color;
      }), (Func<KMonoBehaviour, bool>) (primary_element => ((Component) primary_element).gameObject.GetComponent<KBatchedAnimController>().IsVisible()))
    };

    public override HashedString ViewMode() => OverlayModes.TileMode.ID;

    public override string GetSoundName() => "SuitRequired";

    public TileMode()
    {
      this.targetLayer = LayerMask.NameToLayer("MaskedOverlay");
      this.cameraLayerMask = LayerMask.GetMask(new string[2]
      {
        "MaskedOverlay",
        "MaskedOverlayBG"
      });
      this.legendFilters = this.CreateDefaultFilters();
    }

    public override void Enable()
    {
      base.Enable();
      this.targetIDs.UnionWith((IEnumerable<Tag>) Assets.GetPrefabTagsWithComponent<PrimaryElement>());
      Camera.main.cullingMask |= this.cameraLayerMask;
      int defaultLayerMask = SelectTool.Instance.GetDefaultLayerMask();
      int mask = LayerMask.GetMask(new string[1]
      {
        "MaskedOverlay"
      });
      SelectTool.Instance.SetLayerMask(defaultLayerMask | mask);
    }

    public override void Update()
    {
      Vector2I min;
      Vector2I max;
      Grid.GetVisibleExtents(out min, out max);
      OverlayModes.Mode.RemoveOffscreenTargets<PrimaryElement>((ICollection<PrimaryElement>) this.layerTargets, min, max);
      int height = max.y - min.y;
      int width = max.x - min.x;
      Extents extents = new Extents(min.x, min.y, width, height);
      List<ScenePartitionerEntry> gathered_entries = new List<ScenePartitionerEntry>();
      GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.pickupablesLayer, gathered_entries);
      foreach (ScenePartitionerEntry partitionerEntry in gathered_entries)
      {
        PrimaryElement component = ((Component) partitionerEntry.obj).gameObject.GetComponent<PrimaryElement>();
        if (Object.op_Inequality((Object) component, (Object) null))
          this.TryAddObject(component, min, max);
      }
      gathered_entries.Clear();
      GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.completeBuildings, gathered_entries);
      foreach (ScenePartitionerEntry partitionerEntry in gathered_entries)
      {
        BuildingComplete buildingComplete = (BuildingComplete) partitionerEntry.obj;
        PrimaryElement component = ((Component) buildingComplete).gameObject.GetComponent<PrimaryElement>();
        if (Object.op_Inequality((Object) component, (Object) null) && ((Component) buildingComplete).gameObject.layer == 0)
          this.TryAddObject(component, min, max);
      }
      this.UpdateHighlightTypeOverlay<PrimaryElement>(min, max, (ICollection<PrimaryElement>) this.layerTargets, (ICollection<Tag>) this.targetIDs, this.highlightConditions, OverlayModes.BringToFrontLayerSetting.Conditional, this.targetLayer);
    }

    private void TryAddObject(PrimaryElement pe, Vector2I min, Vector2I max)
    {
      Element element = pe.Element;
      foreach (Tag tileOverlayFilter in Game.Instance.tileOverlayFilters)
      {
        if (element.HasTag(tileOverlayFilter))
        {
          this.AddTargetIfVisible<PrimaryElement>(pe, min, max, (ICollection<PrimaryElement>) this.layerTargets, this.targetLayer);
          break;
        }
      }
    }

    public override void Disable()
    {
      base.Disable();
      this.DisableHighlightTypeOverlay<PrimaryElement>((ICollection<PrimaryElement>) this.layerTargets);
      Camera.main.cullingMask &= ~this.cameraLayerMask;
      this.layerTargets.Clear();
      SelectTool.Instance.ClearLayerMask();
    }

    public override Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters() => new Dictionary<string, ToolParameterMenu.ToggleState>()
    {
      {
        ToolParameterMenu.FILTERLAYERS.ALL,
        ToolParameterMenu.ToggleState.On
      },
      {
        ToolParameterMenu.FILTERLAYERS.METAL,
        ToolParameterMenu.ToggleState.Off
      },
      {
        ToolParameterMenu.FILTERLAYERS.BUILDABLE,
        ToolParameterMenu.ToggleState.Off
      },
      {
        ToolParameterMenu.FILTERLAYERS.FILTER,
        ToolParameterMenu.ToggleState.Off
      },
      {
        ToolParameterMenu.FILTERLAYERS.CONSUMABLEORE,
        ToolParameterMenu.ToggleState.Off
      },
      {
        ToolParameterMenu.FILTERLAYERS.ORGANICS,
        ToolParameterMenu.ToggleState.Off
      },
      {
        ToolParameterMenu.FILTERLAYERS.FARMABLE,
        ToolParameterMenu.ToggleState.Off
      },
      {
        ToolParameterMenu.FILTERLAYERS.GAS,
        ToolParameterMenu.ToggleState.Off
      },
      {
        ToolParameterMenu.FILTERLAYERS.LIQUID,
        ToolParameterMenu.ToggleState.Off
      }
    };

    public override void OnFiltersChanged()
    {
      Game.Instance.tileOverlayFilters.Clear();
      if (this.InFilter(ToolParameterMenu.FILTERLAYERS.METAL, this.legendFilters))
      {
        Game.Instance.tileOverlayFilters.Add(GameTags.Metal);
        Game.Instance.tileOverlayFilters.Add(GameTags.RefinedMetal);
      }
      if (this.InFilter(ToolParameterMenu.FILTERLAYERS.BUILDABLE, this.legendFilters))
      {
        Game.Instance.tileOverlayFilters.Add(GameTags.BuildableRaw);
        Game.Instance.tileOverlayFilters.Add(GameTags.BuildableProcessed);
      }
      if (this.InFilter(ToolParameterMenu.FILTERLAYERS.FILTER, this.legendFilters))
        Game.Instance.tileOverlayFilters.Add(GameTags.Filter);
      if (this.InFilter(ToolParameterMenu.FILTERLAYERS.LIQUIFIABLE, this.legendFilters))
        Game.Instance.tileOverlayFilters.Add(GameTags.Liquifiable);
      if (this.InFilter(ToolParameterMenu.FILTERLAYERS.LIQUID, this.legendFilters))
        Game.Instance.tileOverlayFilters.Add(GameTags.Liquid);
      if (this.InFilter(ToolParameterMenu.FILTERLAYERS.CONSUMABLEORE, this.legendFilters))
        Game.Instance.tileOverlayFilters.Add(GameTags.ConsumableOre);
      if (this.InFilter(ToolParameterMenu.FILTERLAYERS.ORGANICS, this.legendFilters))
        Game.Instance.tileOverlayFilters.Add(GameTags.Organics);
      if (this.InFilter(ToolParameterMenu.FILTERLAYERS.FARMABLE, this.legendFilters))
      {
        Game.Instance.tileOverlayFilters.Add(GameTags.Farmable);
        Game.Instance.tileOverlayFilters.Add(GameTags.Agriculture);
      }
      if (this.InFilter(ToolParameterMenu.FILTERLAYERS.GAS, this.legendFilters))
      {
        Game.Instance.tileOverlayFilters.Add(GameTags.Breathable);
        Game.Instance.tileOverlayFilters.Add(GameTags.Unbreathable);
      }
      this.DisableHighlightTypeOverlay<PrimaryElement>((ICollection<PrimaryElement>) this.layerTargets);
      this.layerTargets.Clear();
      Game.Instance.ForceOverlayUpdate();
    }
  }
}
