// Decompiled with JetBrains decompiler
// Type: WorldSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSelector : KScreen, ISim4000ms
{
  public static WorldSelector Instance;
  public Dictionary<int, MultiToggle> worldRows;
  public TextStyleSetting titleTextSetting;
  public TextStyleSetting bodyTextSetting;
  public GameObject worldRowPrefab;
  public GameObject worldRowContainer;
  private Dictionary<int, ColonyDiagnostic.DiagnosticResult.Opinion> previousWorldDiagnosticStatus = new Dictionary<int, ColonyDiagnostic.DiagnosticResult.Opinion>();
  private Dictionary<int, List<GameObject>> worldStatusIcons = new Dictionary<int, List<GameObject>>();

  public static void DestroyInstance() => WorldSelector.Instance = (WorldSelector) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    WorldSelector.Instance = this;
  }

  protected virtual void OnSpawn()
  {
    if (!DlcManager.FeatureClusterSpaceEnabled())
    {
      this.Deactivate();
    }
    else
    {
      base.OnSpawn();
      this.worldRows = new Dictionary<int, MultiToggle>();
      this.SpawnToggles();
      this.RefreshToggles();
      Game.Instance.Subscribe(1983128072, (Action<object>) (data => this.RefreshToggles()));
      Game.Instance.Subscribe(-521212405, (Action<object>) (data => this.RefreshToggles()));
      Game.Instance.Subscribe(880851192, (Action<object>) (data => this.SortRows()));
      ClusterManager.Instance.Subscribe(-1280433810, (Action<object>) (data => this.AddWorld(data)));
      ClusterManager.Instance.Subscribe(-1078710002, (Action<object>) (data => this.RemoveWorld(data)));
      ClusterManager.Instance.Subscribe(1943181844, (Action<object>) (data => this.RefreshToggles()));
    }
  }

  private void SpawnToggles()
  {
    foreach (KeyValuePair<int, MultiToggle> worldRow in this.worldRows)
      Util.KDestroyGameObject((Component) worldRow.Value);
    this.worldRows.Clear();
    foreach (int key in ClusterManager.Instance.GetWorldIDsSorted())
    {
      MultiToggle component = Util.KInstantiateUI(this.worldRowPrefab, this.worldRowContainer, false).GetComponent<MultiToggle>();
      this.worldRows.Add(key, component);
      this.previousWorldDiagnosticStatus.Add(key, ColonyDiagnostic.DiagnosticResult.Opinion.Normal);
      int id = key;
      component.onClick += (System.Action) (() => this.OnWorldRowClicked(id));
      ((Component) component).GetComponentInChildren<AlertVignette>().worldID = key;
    }
  }

  private void AddWorld(object data)
  {
    int key = (int) data;
    MultiToggle component = Util.KInstantiateUI(this.worldRowPrefab, this.worldRowContainer, false).GetComponent<MultiToggle>();
    this.worldRows.Add(key, component);
    this.previousWorldDiagnosticStatus.Add(key, ColonyDiagnostic.DiagnosticResult.Opinion.Normal);
    int id = key;
    component.onClick += (System.Action) (() => this.OnWorldRowClicked(id));
    ((Component) component).GetComponentInChildren<AlertVignette>().worldID = key;
    this.RefreshToggles();
  }

  private void RemoveWorld(object data)
  {
    int key = (int) data;
    MultiToggle multiToggle;
    if (this.worldRows.TryGetValue(key, out multiToggle))
      TracesExtesions.DeleteObject((Component) multiToggle);
    this.worldRows.Remove(key);
    this.previousWorldDiagnosticStatus.Remove(key);
    this.RefreshToggles();
  }

  public void OnWorldRowClicked(int id)
  {
    WorldContainer world = ClusterManager.Instance.GetWorld(id);
    if (!Object.op_Inequality((Object) world, (Object) null) || !world.IsDiscovered)
      return;
    CameraController.Instance.ActiveWorldStarWipe(id);
  }

  private void RefreshToggles()
  {
    foreach (KeyValuePair<int, MultiToggle> worldRow in this.worldRows)
    {
      WorldContainer world = ClusterManager.Instance.GetWorld(worldRow.Key);
      ClusterGridEntity component1 = ((Component) world).GetComponent<ClusterGridEntity>();
      HierarchyReferences component2 = ((Component) worldRow.Value).GetComponent<HierarchyReferences>();
      if (Object.op_Inequality((Object) world, (Object) null))
      {
        component2.GetReference<Image>("Icon").sprite = component1.GetUISprite();
        ((TMP_Text) component2.GetReference<LocText>("Label")).SetText(((Component) world).GetComponent<ClusterGridEntity>().Name);
      }
      else
        component2.GetReference<Image>("Icon").sprite = Assets.GetSprite(HashedString.op_Implicit("unknown_far"));
      if (worldRow.Key == CameraController.Instance.cameraActiveCluster)
      {
        worldRow.Value.ChangeState(1);
        ((Component) worldRow.Value).gameObject.SetActive(true);
      }
      else if (Object.op_Inequality((Object) world, (Object) null) && world.IsDiscovered)
      {
        worldRow.Value.ChangeState(0);
        ((Component) worldRow.Value).gameObject.SetActive(true);
      }
      else
      {
        worldRow.Value.ChangeState(0);
        ((Component) worldRow.Value).gameObject.SetActive(false);
      }
      this.RefreshToggleTooltips();
      ((Component) worldRow.Value).GetComponentInChildren<AlertVignette>().worldID = worldRow.Key;
    }
    this.RefreshWorldStatus();
    this.SortRows();
  }

  private void RefreshWorldStatus()
  {
    foreach (KeyValuePair<int, MultiToggle> worldRow in this.worldRows)
    {
      if (!this.worldStatusIcons.ContainsKey(worldRow.Key))
        this.worldStatusIcons.Add(worldRow.Key, new List<GameObject>());
      foreach (GameObject gameObject in this.worldStatusIcons[worldRow.Key])
        Util.KDestroyGameObject(gameObject);
      LocText reference = ((Component) worldRow.Value).GetComponent<HierarchyReferences>().GetReference<LocText>("StatusLabel");
      ((TMP_Text) reference).SetText(ClusterManager.Instance.GetWorld(worldRow.Key).GetStatus());
      ((Graphic) reference).color = ColonyDiagnosticScreen.GetDiagnosticIndicationColor(ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(worldRow.Key));
    }
  }

  private void RefreshToggleTooltips()
  {
    int num = 0;
    List<int> asteroidIdsSorted = ClusterManager.Instance.GetDiscoveredAsteroidIDsSorted();
    foreach (KeyValuePair<int, MultiToggle> worldRow in this.worldRows)
    {
      ClusterGridEntity component1 = ((Component) ClusterManager.Instance.GetWorld(worldRow.Key)).GetComponent<ClusterGridEntity>();
      ToolTip component2 = ((Component) worldRow.Value).GetComponent<ToolTip>();
      component2.ClearMultiStringTooltip();
      WorldContainer world = ClusterManager.Instance.GetWorld(worldRow.Key);
      if (Object.op_Inequality((Object) world, (Object) null))
      {
        component2.AddMultiStringTooltip(component1.Name, this.titleTextSetting);
        if (!world.IsModuleInterior)
        {
          int idx = asteroidIdsSorted.IndexOf(world.id);
          if (idx != -1 && idx <= 9)
          {
            component2.AddMultiStringTooltip(" ", this.bodyTextSetting);
            if (KInputManager.currentControllerIsGamepad)
              component2.AddMultiStringTooltip(STRINGS.UI.FormatAsHotkey(GameUtil.GetActionString(this.IdxToHotkeyAction(idx))), this.bodyTextSetting);
            else
              component2.AddMultiStringTooltip(STRINGS.UI.FormatAsHotkey("[" + GameUtil.GetActionString(this.IdxToHotkeyAction(idx)) + "]"), this.bodyTextSetting);
          }
        }
      }
      else
        component2.AddMultiStringTooltip((string) STRINGS.UI.CLUSTERMAP.UNKNOWN_DESTINATION, this.titleTextSetting);
      if (ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(world.id) < ColonyDiagnostic.DiagnosticResult.Opinion.Normal)
        component2.AddMultiStringTooltip(ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResultTooltip(world.id), this.bodyTextSetting);
      ++num;
    }
  }

  private void SortRows()
  {
    List<KeyValuePair<int, MultiToggle>> list = this.worldRows.ToList<KeyValuePair<int, MultiToggle>>();
    list.Sort((Comparison<KeyValuePair<int, MultiToggle>>) ((x, y) => (ClusterManager.Instance.GetWorld(x.Key).IsModuleInterior ? float.PositiveInfinity : ClusterManager.Instance.GetWorld(x.Key).DiscoveryTimestamp).CompareTo(ClusterManager.Instance.GetWorld(y.Key).IsModuleInterior ? float.PositiveInfinity : ClusterManager.Instance.GetWorld(y.Key).DiscoveryTimestamp)));
    for (int index = 0; index < list.Count; ++index)
      list[index].Value.transform.SetSiblingIndex(index);
    foreach (KeyValuePair<int, MultiToggle> keyValuePair1 in list)
    {
      ((Component) keyValuePair1.Value).GetComponent<HierarchyReferences>().GetReference<RectTransform>("Indent").anchoredPosition = Vector2.zero;
      ((Component) keyValuePair1.Value).GetComponent<HierarchyReferences>().GetReference<RectTransform>("Status").anchoredPosition = Vector2.op_Multiply(Vector2.right, 24f);
      WorldContainer world = ClusterManager.Instance.GetWorld(keyValuePair1.Key);
      if (world.ParentWorldId != world.id && world.ParentWorldId != (int) ClusterManager.INVALID_WORLD_IDX)
      {
        foreach (KeyValuePair<int, MultiToggle> keyValuePair2 in list)
        {
          if (keyValuePair2.Key == world.ParentWorldId)
          {
            int siblingIndex = ((Component) keyValuePair2.Value).gameObject.transform.GetSiblingIndex();
            ((Component) keyValuePair1.Value).gameObject.transform.SetSiblingIndex(siblingIndex + 1);
            ((Component) keyValuePair1.Value).GetComponent<HierarchyReferences>().GetReference<RectTransform>("Indent").anchoredPosition = Vector2.op_Multiply(Vector2.right, 32f);
            ((Component) keyValuePair1.Value).GetComponent<HierarchyReferences>().GetReference<RectTransform>("Status").anchoredPosition = Vector2.op_Multiply(Vector2.right, -8f);
            break;
          }
        }
      }
    }
  }

  private Action IdxToHotkeyAction(int idx)
  {
    Action hotkeyAction;
    switch (idx)
    {
      case 0:
        hotkeyAction = (Action) 258;
        break;
      case 1:
        hotkeyAction = (Action) 259;
        break;
      case 2:
        hotkeyAction = (Action) 260;
        break;
      case 3:
        hotkeyAction = (Action) 261;
        break;
      case 4:
        hotkeyAction = (Action) 262;
        break;
      case 5:
        hotkeyAction = (Action) 263;
        break;
      case 6:
        hotkeyAction = (Action) 264;
        break;
      case 7:
        hotkeyAction = (Action) 265;
        break;
      case 8:
        hotkeyAction = (Action) 266;
        break;
      case 9:
        hotkeyAction = (Action) 267;
        break;
      default:
        Debug.LogError((object) "Action must be a SwitchActiveWorld Action");
        hotkeyAction = (Action) 258;
        break;
    }
    return hotkeyAction;
  }

  public void Sim4000ms(float dt)
  {
    foreach (KeyValuePair<int, MultiToggle> worldRow in this.worldRows)
    {
      ColonyDiagnostic.DiagnosticResult.Opinion diagnosticResult = ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(worldRow.Key);
      ColonyDiagnosticScreen.SetIndication(diagnosticResult, ((Component) worldRow.Value).GetComponent<HierarchyReferences>().GetReference("Indicator").gameObject);
      if (this.previousWorldDiagnosticStatus[worldRow.Key] > diagnosticResult && ClusterManager.Instance.activeWorldId != worldRow.Key)
        this.TriggerVisualNotification(worldRow.Key, diagnosticResult);
      this.previousWorldDiagnosticStatus[worldRow.Key] = diagnosticResult;
    }
    this.RefreshWorldStatus();
    this.RefreshToggleTooltips();
  }

  public void TriggerVisualNotification(
    int worldID,
    ColonyDiagnostic.DiagnosticResult.Opinion result)
  {
    foreach (KeyValuePair<int, MultiToggle> worldRow in this.worldRows)
    {
      if (worldRow.Key == worldID)
      {
        KFMOD.PlayUISound(GlobalAssets.GetSound(ColonyDiagnosticScreen.notificationSoundsInactive[result]));
        if (((Component) worldRow.Value).gameObject.activeInHierarchy)
          ((MonoBehaviour) worldRow.Value).StartCoroutine(this.VisualNotificationRoutine(((Component) ((Component) worldRow.Value).GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content")).gameObject, ((Component) worldRow.Value).GetComponent<HierarchyReferences>().GetReference<RectTransform>("Indicator"), ((Component) ((Component) worldRow.Value).GetComponent<HierarchyReferences>().GetReference<RectTransform>("Spacer")).gameObject));
      }
    }
  }

  private IEnumerator VisualNotificationRoutine(
    GameObject contentGameObject,
    RectTransform indicator,
    GameObject spacer)
  {
    spacer.GetComponent<NotificationAnimator>().Begin(false);
    Vector2 defaultIndicatorSize = new Vector2(8f, 8f);
    float bounceDuration = 1.5f;
    float i;
    for (i = 0.0f; (double) i < (double) bounceDuration; i += Time.unscaledDeltaTime)
    {
      indicator.sizeDelta = Vector2.op_Addition(defaultIndicatorSize, Vector2.op_Multiply(Vector2.one, (float) Mathf.RoundToInt(Mathf.Sin((float) (6.0 * (3.1415927410125732 * ((double) i / (double) bounceDuration)))))));
      yield return (object) 0;
    }
    for (i = 0.0f; (double) i < (double) bounceDuration; i += Time.unscaledDeltaTime)
    {
      indicator.sizeDelta = Vector2.op_Addition(defaultIndicatorSize, Vector2.op_Multiply(Vector2.one, (float) Mathf.RoundToInt(Mathf.Sin((float) (6.0 * (3.1415927410125732 * ((double) i / (double) bounceDuration)))))));
      yield return (object) 0;
    }
    for (i = 0.0f; (double) i < (double) bounceDuration; i += Time.unscaledDeltaTime)
    {
      indicator.sizeDelta = Vector2.op_Addition(defaultIndicatorSize, Vector2.op_Multiply(Vector2.one, (float) Mathf.RoundToInt(Mathf.Sin((float) (6.0 * (3.1415927410125732 * ((double) i / (double) bounceDuration)))))));
      yield return (object) 0;
    }
    defaultIndicatorSize = new Vector2(8f, 8f);
    indicator.sizeDelta = defaultIndicatorSize;
    ((Transform) Util.rectTransform(contentGameObject)).localPosition = Vector2.op_Implicit(Vector2.zero);
  }
}
